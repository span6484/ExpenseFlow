namespace ExpenseFlow.Api.Domain.Exceptions
{
    public class InvalidExpenseStateException : DomainException
    {
        public InvalidExpenseStateException(ExpenseStatus currentStatus,
            ExpenseStatus expectedStatus) : base($"Invalid expense state. Current: {currentStatus}, Expected: {expectedStatus}.")
        {
            CurrentStatus = currentStatus;
            ExpectedStatus = expectedStatus;
        }
        public ExpenseStatus CurrentStatus { get; }
        public ExpenseStatus ExpectedStatus { get; }
    }
}
