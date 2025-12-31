namespace ExpenseFlow.Api.Domain.Exceptions;

public class InvalidApprovalStateException : DomainException
{
    public InvalidApprovalStateException(ApprovalStatus currentStatus,
        ApprovalStatus expectedStatus) : base($"Invalid expense state. Current: {currentStatus}, Expected: {expectedStatus}.")
    {
        CurrentStatus = currentStatus;
        ExpectedStatus = expectedStatus;
    }
    public ApprovalStatus CurrentStatus { get; }
    public ApprovalStatus ExpectedStatus { get; }
}