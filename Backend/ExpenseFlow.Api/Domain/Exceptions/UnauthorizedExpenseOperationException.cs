namespace ExpenseFlow.Api.Domain.Exceptions;

public class UnauthorizedExpenseOperationException : DomainException
{
    public UnauthorizedExpenseOperationException() : base($"Unauthorized operation.")
    {
    }
}