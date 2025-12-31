namespace ExpenseFlow.Api.Domain.Exceptions
{
    public class InvalidProcessException : DomainException
    {
        public InvalidProcessException(string msg) : base($"{msg}")
        {

        }
    }
}
