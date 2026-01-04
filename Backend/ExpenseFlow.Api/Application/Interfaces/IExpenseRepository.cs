using ExpenseFlow.Api.Domain;

namespace ExpenseFlow.Api.Application.Interfaces
{
    public interface IExpenseRepository
    {

        /// <summary>
        /// Retrieves an expense aggregate by its identifier.
        /// </summary>
        Task<Expense> GetAsync(Guid uid);
        /// <summary>
        /// Persists the expense aggregate (insert or update as needed).
        /// </summary>
        Task SaveAsync(Expense expense);
    }
}
