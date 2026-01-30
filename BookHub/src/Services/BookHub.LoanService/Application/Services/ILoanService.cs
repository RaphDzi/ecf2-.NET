using BookHub.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BookHub.LoanService.Application.Services
{
    public interface ILoanService
    {
        Task<IEnumerable<LoanDto>> GetAllLoansAsync(CancellationToken cancellationToken = default);

        Task<LoanDto?> GetLoanByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<IEnumerable<LoanDto>> GetLoansByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

        Task<IEnumerable<LoanDto>> GetOverdueLoansAsync(CancellationToken cancellationToken = default);

        Task<LoanDto> CreateLoanAsync(CreateLoanDto dto, CancellationToken cancellationToken = default);

        Task<bool> ReturnLoanAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
