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


        public async Task ConfigureApproverAsync(Guid expenseId, Guid departmentManagerId, Guid financeManagerId)
        {
            var expense = await _expenseRepository.GetAsync(expenseId);
            if (expense == null)
            {
                throw new NotFoundException(nameof(Expense), expenseId);
            }
            expense.SetApprovers(departmentManagerId, financeManagerId);
            await _expenseRepository.SaveAsync(expense);
        }

        /// <summary>
        /// Creates a new Expense in Draft state.
        /// </summary>
        /// <param name="createdByUserId"></param>
        /// <returns></returns>
        /// <exception cref="ValidationException"></exception>
        public async Task<Guid> CreateExpenseAsync(Guid createdByUserId, ExpenseDetails details)
        {
            if (createdByUserId == Guid.Empty)
            {
                throw new ValidationException(nameof(createdByUserId), "createdByUserId cannot be empty");
            }
            if (details == null)
                throw new ValidationException(nameof(details), "Expense details must not be null");
            Expense expense = new Expense(createdByUserId, details);
            await _expenseRepository.SaveAsync(expense); 
            return expense.Id;
        }

        /// <summary>
        /// Updates the details of an existing Expense.
        /// </summary>
        /// <param name="expenseId"></param>
        /// <param name="_newExpenseDetails"></param>
        /// <returns></returns>
        /// <exception cref="ValidationException"></exception>
        /// <exception cref="NotFoundException"></exception>
        public async Task<Guid> EditExpenseAsync(Guid expenseId, ExpenseDetails _newExpenseDetails)
        {
            if (expenseId == Guid.Empty)
            {
                throw new ValidationException(nameof(expenseId), "expenseId cannot be empty");
            }
            if (_newExpenseDetails == null)
            {
                throw new ValidationException(
                    nameof(_newExpenseDetails),
                    "Expense details must not be null"
                );
            }

            Expense expense = await _expenseRepository.GetAsync(expenseId);
            if (expense == null)
            {
                throw new NotFoundException(nameof(Expense), expenseId);
            }
            expense.Edit(_newExpenseDetails);
            await _expenseRepository.SaveAsync(expense);
            return expenseId;
        }

        /// <summary>
        /// Submits an Expense for approval and starts the approval workflow.
        /// </summary>
        public async Task SubmitExpenseAsync(Guid expenseId)
        {       
            if (expenseId == Guid.Empty)
            {
                throw new ValidationException(nameof(expenseId), "Expense identifier must not be empty");
            }

            Expense expense = await _expenseRepository.GetAsync(expenseId);
            if (expense == null)
            {
                throw new NotFoundException(nameof(Expense), expenseId);
            }
            expense.Submit();
            await _expenseRepository.SaveAsync(expense);

        }
    }
}
