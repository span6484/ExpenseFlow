namespace ExpenseFlow.Api.Domain;

public sealed class ExpenseDetails
{
    public int Amount { get; }
    public string Description { get; }

    public ExpenseDetails(int amount, string description)
    {
        Amount = amount;
        Description = description;
    }
}