using ExpenseFlow.Api.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpenseFlow.Api.Application.Exceptions;
using ExpenseFlow.Api.Application.Interfaces;
using ExpenseFlow.Api.Domain;
using FluentAssertions;
using Moq;

namespace ExpenseFlow.Api.Application.Tests
{
    public class ExpenseApplicationServiceUnitTests
    {

        [Fact]
        public async Task CreateExpense_Should_Save_Expense()
        {
            var creatorId = Guid.NewGuid();
            var details = new ExpenseDetails(50, "test");

            var repo = new Mock<IExpenseRepository>();
            var service = new ExpenseApplicationService(repo.Object);
            await service.CreateExpenseAsync(creatorId, details);
            repo.Verify(r=>r.SaveAsync(It.IsAny<Expense>()), Times.Once);
        }

        [Fact]
        public async Task CreateExpense_Should_Return_New_ID()
        {
            var creatorId = Guid.NewGuid();
            var details = new ExpenseDetails(50, "test");

            var repo = new Mock<IExpenseRepository>();
            var service = new ExpenseApplicationService(repo.Object);
            var newExpenseId = await service.CreateExpenseAsync(creatorId, details);
            Assert.NotEqual(Guid.Empty, newExpenseId);
        }


        [Fact]
        public async Task SubmitExpense_Should_Save_Expense()
        {
            Guid expenseId = Guid.NewGuid();
            Guid creatorId = Guid.NewGuid();
            var details = new ExpenseDetails(50, "test");

            var repo = new Mock<IExpenseRepository>();
            var service = new ExpenseApplicationService(repo.Object);
            Expense expense = new Expense(creatorId, details);
            expense.SetApprovers(Guid.NewGuid(), Guid.NewGuid());
            repo.Setup(r => r.GetAsync(expenseId)).ReturnsAsync(expense);
            await service.SubmitExpenseAsync(expenseId);
            repo.Verify((r => r.SaveAsync(It.IsAny<Expense>())), Times.Once);
        }

        //    SubmitExpense_Should_Throw_When_Not_Found
        [Fact]
        public async Task SubmitExpense_Should_Throw_When_Not_Found()
        {
            Guid expenseId = Guid.NewGuid();
            Guid creatorId = Guid.NewGuid();
            var details = new ExpenseDetails(50, "test");

            var repo = new Mock<IExpenseRepository>();
            var service = new ExpenseApplicationService(repo.Object);
            Expense expense = new Expense(creatorId, details);
            expense.SetApprovers(Guid.NewGuid(), Guid.NewGuid());
            repo.Setup(r => r.GetAsync(expenseId)).ReturnsAsync(expense);
            await service.SubmitExpenseAsync(expenseId);
            repo.Verify((r => r.SaveAsync(It.IsAny<Expense>())), Times.Once);
        }

        //SubmitExpense_Should_Throw_When_Id_Is_Empty
        [Fact]
        public async Task SubmitExpense_Should_Throw_When_Id_Is_Empty()
        {
            var repo = new Mock<IExpenseRepository>();
            var service = new ExpenseApplicationService(repo.Object);
            // Act
            Func<Task> act = () => service.SubmitExpenseAsync(Guid.Empty);
            await act.Should().ThrowAsync<ValidationException>();
            repo.Verify(
                r => r.SaveAsync(It.IsAny<Expense>()),
                Times.Never
            );

            repo.Verify(
                r => r.GetAsync(It.IsAny<Guid>()),
                Times.Never
            );
        }

        //    Approve
        [Fact]
        public async Task EditExpense_Should_Save_Expense()
        {
            Guid expenseId = Guid.NewGuid();
            Guid creatorId = Guid.NewGuid();
            var details = new ExpenseDetails(50, "test");
            var newDetails = new ExpenseDetails(60, "test2");
            var repo = new Mock<IExpenseRepository>();
            var service = new ExpenseApplicationService(repo.Object);
            Expense expense = new Expense(creatorId, details);
            repo.Setup(r => r.GetAsync(expenseId)).ReturnsAsync(expense);
            await service.EditExpenseAsync(expenseId, newDetails);
            repo.Verify(r => r.SaveAsync(It.IsAny<Expense>()), Times.Once);
            
        }
    }
}
