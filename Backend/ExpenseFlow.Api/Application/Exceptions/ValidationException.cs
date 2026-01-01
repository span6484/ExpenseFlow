
namespace ExpenseFlow.Api.Application.Exceptions
{
    public class ValidationException : ApplicationException
    {
        public string Field { get; }

        public ValidationException(string field, string message)
            : base(message)
        {
            Field = field;
        }
    }

}
