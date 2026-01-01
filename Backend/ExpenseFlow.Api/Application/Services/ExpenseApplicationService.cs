using ExpenseFlow.Api.Application.Exceptions;
using ExpenseFlow.Api.Application.Interfaces;
using ExpenseFlow.Api.Domain;

namespace ExpenseFlow.Api.Application.Services
{
    public class ExpenseApplicationService
    {
        private readonly IExpenseRepository _expenseRepository;

        public ExpenseApplicationService(IExpenseRepository expenseRepository)
        {
            _expenseRepository = expenseRepository;

        }

        public Guid CreatExpense(Guid createdByUserId)
        {
            if (createdByUserId == Guid.Empty)
            {
                throw new ValidationException(nameof(createdByUserId), "createdByUserId cannot be empty");
            }
            Expense expense = new Expense(createdByUserId);
            _expenseRepository.Save(expense); 
            return expense.Id;
        }
    }
}
