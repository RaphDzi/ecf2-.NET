using BookHub.Shared.DTOs;
using BookHub.LoanService.Domain.Entities;
using BookHub.LoanService.Domain.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BookHub.LoanService.Application.Services
{
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _repository;

        public LoanService(ILoanRepository repository)
        {
            _repository = repository;
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
            var loan = new Loan
            {
                UserId = dto.UserId,
                BookId = dto.BookId,
                LoanDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(14),
                Status = LoanStatus.Active
            };

            await _repository.AddAsync(loan, cancellationToken);

            return MapToDto(loan);
        }

        public async Task<bool> ReturnLoanAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var loan = await _repository.GetByIdAsync(id, cancellationToken);
            if (loan == null) return false;

            loan.ReturnDate = DateTime.UtcNow;
            loan.Status = LoanStatus.Returned;

            await _repository.UpdateAsync(loan, cancellationToken);

            return true;
        }

        private LoanDto MapToDto(Loan loan)
        {
            return new LoanDto(
                loan.Id,
                loan.UserId,
                loan.BookId,
                loan.BookTitle,
                loan.UserEmail,
                loan.LoanDate,
                loan.DueDate,
                loan.ReturnDate,
                loan.Status,
                loan.PenaltyAmount
            );
        }
    }
}
