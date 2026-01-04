using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpenseFlow.Api.Application.Interfaces;
using ExpenseFlow.Api.Domain;

namespace ExpenseFlow.Api.Application.Tests
{
    public class InMemoryExpenseRepository : IExpenseRepository
    {
        private readonly Dictionary<Guid, Expense> _store = new();
        public Task<Expense> GetAsync(Guid id)
        {
            _store.TryGetValue(id, out var expense);
            return Task.FromResult(expense);
        }

        public Task SaveAsync(Expense expense)
        {
            _store[expense.Id] = expense;
            return Task.CompletedTask;
        }
    }
}
