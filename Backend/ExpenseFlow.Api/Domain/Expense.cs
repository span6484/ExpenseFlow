using ExpenseFlow.Api.Domain.Exceptions;

namespace ExpenseFlow.Api.Domain
{
    public class Expense
    {
        public ExpenseStatus Status { get; private set; }

        public void Submit()
        {
            if (Status != ExpenseStatus.Draft)
                throw new InvalidExpenseStateException(
                    currentStatus: Status,
                    expectedStatus: ExpenseStatus.Draft);   
            Status = ExpenseStatus.Submitted;
        }
    }

    public enum ExpenseStatus
    {
        Draft,
        Submitted,
        Approved,
        Rejected
    }
}
