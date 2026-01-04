using ExpenseFlow.Api.Application.Exceptions;
using ExpenseFlow.Api.Application.Interfaces;
using ExpenseFlow.Api.Application.Services;
using ExpenseFlow.Api.Domain;
using FluentAssertions;
using Moq;

namespace ExpenseFlow.Api.Application.Tests
{
    public class ExpenseApplicationServiceFlowTests
    {
        private readonly ExpenseApplicationService _service;
        private readonly InMemoryExpenseRepository _repository;
        public ExpenseApplicationServiceFlowTests()
        {
            _repository = new InMemoryExpenseRepository();
            _service = new ExpenseApplicationService(_repository);
        }
        [Fact]
        public async Task CreateExpense_Should_Create_Draft_Expense()
        {

            var userId = Guid.NewGuid();
            var expenseDetail = new ExpenseDetails(50, "test1");

            Guid expenseId = await _service.CreateExpenseAsync(userId, expenseDetail);
            // Assert
            var expense = await _repository.GetAsync(expenseId);
            Assert.NotNull(expense);
            Assert.Equal(userId, expense.CreatedByUserId);
            Assert.Equal(ExpenseStatus.Draft, expense.Status);
        }
        [Fact]
        public async Task EditExpense_Should_Update_ExpenseDetails()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var expenseDetail = new ExpenseDetails(60, "test2");

            var expenseId = await _service.CreateExpenseAsync(userId, expenseDetail);
            var newDetails = new ExpenseDetails(100,"updateTest2");


            // Act
            var editExpenseId = await _service.EditExpenseAsync(expenseId, newDetails);
            var editExpense = await _repository.GetAsync(editExpenseId);
            Assert.NotNull(editExpense);
            Assert.Equal(100, editExpense.Details.Amount);
            Assert.Equal("updateTest2", editExpense.Details.Description);
        }

        [Fact]
        public async Task SubmitExpense_Should_Throw_When_Expense_Not_Found()
        {
            // Arrange
            var expenseId = Guid.NewGuid();
            // Act
            Func<Task> act = () => _service.SubmitExpenseAsync(expenseId);

            // Assert

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task SubmitExpense_EndToEnd_Should_Change_Status()
        {
            // Arrange

            var userId = Guid.NewGuid();
            var expenseDetail = new ExpenseDetails(50, "test1");
            
            Guid expenseId = await _service.CreateExpenseAsync(userId, expenseDetail);
            
            // Act
            await _service.ConfigureApproverAsync(expenseId, Guid.NewGuid(), Guid.NewGuid());
            await _service.SubmitExpenseAsync(expenseId);
            var expense = await _repository.GetAsync(expenseId);

            // Assert
            Assert.Equal(ExpenseStatus.Submitted,expense.Status);

        }


    }
}