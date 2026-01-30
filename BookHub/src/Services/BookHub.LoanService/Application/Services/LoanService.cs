using BookHub.LoanService.Domain.Entities;
using BookHub.LoanService.Domain.Ports;
using BookHub.Shared.DTOs;
using Microsoft.Extensions.Logging;

namespace BookHub.LoanService.Application.Services;

public interface ILoanRepository
{
    Task<IEnumerable<Loan>> GetAllAsync(CancellationToken cancellationToken);
    Task<Loan?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Loan>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);

    Task<IEnumerable<Loan>> GetOverdueAsync(CancellationToken cancellationToken);
    Task<IEnumerable<Loan>> GetActiveLoansByUserIdAsync(Guid userId, CancellationToken cancellationToken);

    Task AddAsync(Loan loan, CancellationToken cancellationToken);
    Task UpdateAsync(Loan loan, CancellationToken cancellationToken);
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
        return loans.Select(MapToDto).ToList();
    }

    public async Task<LoanDto?> GetLoanByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var loan = await _repository.GetByIdAsync(id, cancellationToken);
        return loan == null ? null : MapToDto(loan);
    }

    public async Task<IEnumerable<LoanDto>> GetLoansByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var loans = await _repository.GetByUserIdAsync(userId, cancellationToken);
        return loans.Select(MapToDto).ToList();
    }

    public async Task<IEnumerable<LoanDto>> GetOverdueLoansAsync(CancellationToken cancellationToken = default)
    {
        var loans = await _repository.GetOverdueAsync(cancellationToken);
        return loans.Select(MapToDto).ToList();
    }

    public async Task<LoanDto> CreateLoanAsync(CreateLoanDto dto, CancellationToken cancellationToken = default)
    {
        // Vérifier que l'utilisateur existe
        var user = await _userClient.GetUserAsync(dto.UserId, cancellationToken);
        if (user == null) throw new InvalidOperationException($"User {dto.UserId} not found.");

        // Vérifier le nombre d'emprunts actifs
        var userLoans = await _repository.GetActiveLoansByUserIdAsync(dto.UserId, cancellationToken);
        if (userLoans.Count() >= 5)
            throw new InvalidOperationException($"User {dto.UserId} cannot borrow more than 5 books simultaneously.");

        // Vérifier que le livre existe
        var book = await _catalogClient.GetBookAsync(dto.BookId, cancellationToken);
        if (book == null) throw new InvalidOperationException($"Book {dto.BookId} not found.");

        // Vérifier disponibilité du livre
        var available = await _catalogClient.DecrementAvailabilityAsync(dto.BookId, cancellationToken);
        if (!available) throw new InvalidOperationException($"Book {dto.BookId} is not available.");

        // Limiter la durée max à 21 jours
        int duration = Math.Min(dto.DurationDays, 21);

        var loan = new Loan
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            BookId = dto.BookId,
            BookTitle = book.Title,
            UserEmail = user.Email,
            LoanDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(duration),
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

        if (loan.DueDate < loan.ReturnDate)
        {
            loan.PenaltyAmount = loan.CalculatePenalty();
        }

        await _repository.UpdateAsync(loan, cancellationToken);
        await _catalogClient.IncrementAvailabilityAsync(loan.BookId, cancellationToken);

        _logger.LogInformation("Loan returned: {LoanId}", loan.Id);
        return MapToDto(loan);
    }

    private static LoanDto MapToDto(Loan loan)
    {
        return new LoanDto
        {
            Id = loan.Id,
            UserId = loan.UserId,
            BookId = loan.BookId,
            BookTitle = loan.BookTitle,
            UserEmail = loan.UserEmail,
            LoanDate = loan.LoanDate,
            DueDate = loan.DueDate,
            ReturnDate = loan.ReturnDate,
            Status = (Shared.DTOs.LoanStatus)(int)loan.Status,
            PenaltyAmount = loan.PenaltyAmount
        };
    }
}
