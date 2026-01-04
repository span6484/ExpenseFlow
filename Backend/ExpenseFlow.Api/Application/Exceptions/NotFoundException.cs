namespace ExpenseFlow.Api.Application.Exceptions
{
    public class NotFoundException : ApplicationException
    {
        public string ResourceName { get; }
        public object Key { get; }

        public NotFoundException(string resourceName, object key)
            : base($"{resourceName} with key '{key}' was not found.")
        {
            ResourceName = resourceName;
            Key = key;
        }
    }
}
