namespace BookHub.Shared.DTOs;

public record LoanDto(
    Guid Id,
    Guid UserId,
    Guid BookId,
    string BookTitle,
    string UserEmail,
    DateTime LoanDate,
    DateTime DueDate,
    DateTime? ReturnDate,
    LoanStatus Status,
    decimal PenaltyAmount
);

public class CreateLoanDto
{
    public Guid UserId { get; set; }
    public Guid BookId { get; set; }
    public int DurationDays { get; set; }
}

public record ReturnLoanDto(
    DateTime ReturnDate
);

public enum LoanStatus
{
    Active,
    Returned,
    Overdue
}
