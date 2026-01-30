using BookHub.LoanService.Domain.Entities;
using BookHub.LoanService.Domain.Ports;
using BookHub.Shared.DTOs;

namespace BookHub.LoanService.Application.Services;

public interface ILoanService
{
    Task<IEnumerable<LoanDto>> GetAllLoansAsync(CancellationToken cancellationToken);
    Task<LoanDto?> GetLoanByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<LoanDto>> GetLoansByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<IEnumerable<LoanDto>> GetOverdueLoansAsync(CancellationToken cancellationToken);
    Task<LoanDto> CreateLoanAsync(CreateLoanDto dto, CancellationToken cancellationToken);
    Task<LoanDto?> ReturnLoanAsync(Guid id, CancellationToken cancellationToken);
}

public class LoanService : ILoanService
{
    private readonly ILoanRepository _repository;
    private readonly ICatalogServiceClient _catalogClient;
    private readonly IUserServiceClient _userClient;
    private readonly ILogger<LoanService> _logger;

    public LoanService(
        ILoanRepository repository,
        ICatalogServiceClient catalogClient,
        IUserServiceClient userClient,
        ILogger<LoanService> logger)
    {
        _repository = repository;
        _catalogClient = catalogClient;
        _userClient = userClient;
        _logger = logger;
    }

    public async Task<IEnumerable<LoanDto>> GetAllLoansAsync(CancellationToken cancellationToken = default)
    {
        var loans = await _repository.GetAllAsync(cancellationToken);
        return loans.Select(MapToDto);
    }

    public async Task<LoanDto?> GetLoanByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var loan = await _repository.GetByIdAsync(id, cancellationToken);
        return loan == null ? null : MapToDto(loan);
    }

    public async Task<IEnumerable<LoanDto>> GetLoansByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var loans = await _repository.GetByUserIdAsync(userId, cancellationToken);
        return loans.Select(MapToDto);
    }

    public async Task<IEnumerable<LoanDto>> GetOverdueLoansAsync(CancellationToken cancellationToken = default)
    {
        var loans = await _repository.GetOverdueAsync(cancellationToken);
        return loans.Select(MapToDto);
    }

    public async Task<LoanDto> CreateLoanAsync(CreateLoanDto dto, CancellationToken cancellationToken = default)
    {
        // Vérifier que le livre existe
        var book = await _catalogClient.GetBookByIdAsync(dto.BookId, cancellationToken);
        if (book == null)
            throw new InvalidOperationException($"Book {dto.BookId} not found.");

        // Vérifier que l'utilisateur existe
        var user = await _userClient.GetUserByIdAsync(dto.UserId, cancellationToken);
        if (user == null)
            throw new InvalidOperationException($"User {dto.UserId} not found.");

        var loan = new Loan
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            BookId = dto.BookId,
            BookTitle = book.Title,
            UserEmail = user.Email,
            LoanDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(dto.DurationDays),
            Status = LoanStatus.Active,
            PenaltyAmount = 0
        };

        await _repository.AddAsync(loan, cancellationToken);
        _logger.LogInformation("Loan created: {LoanId} for User {UserId} and Book {BookId}", loan.Id, loan.UserId, loan.BookId);

        return MapToDto(loan);
    }

    public async Task<LoanDto?> ReturnLoanAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var loan = await _repository.GetByIdAsync(id, cancellationToken);
        if (loan == null) return null;

        loan.ReturnDate = DateTime.UtcNow;
        loan.Status = LoanStatus.Returned;

        // Calculer la pénalité si en retard
        if (loan.DueDate < loan.ReturnDate)
        {
            loan.PenaltyAmount = loan.CalculatePenalty();
        }

        await _repository.UpdateAsync(loan, cancellationToken);
        _logger.LogInformation("Loan returned: {LoanId}", loan.Id);

        return MapToDto(loan);
    }

    private static LoanDto MapToDto(Loan loan) => new(
        loan.Id,
        loan.UserId,
        loan.BookId,
        loan.BookTitle,
        loan.UserEmail,
        loan.LoanDate,
        loan.DueDate,
        loan.ReturnDate,
        (Shared.DTOs.LoanStatus)(int)loan.Status,
        loan.PenaltyAmount
    );
}
