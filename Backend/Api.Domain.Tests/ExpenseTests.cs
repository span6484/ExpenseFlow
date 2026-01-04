using ExpenseFlow.Api.Domain;
using ExpenseFlow.Api.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace Api.Domain.Tests
{
    public class ExpenseTests
    {
        [Fact]
        public void Submit_Should_Change_Status_To_Submitted()
        {
            // Arrange
            var creatorId = Guid.NewGuid();
            var departmentManagerId = Guid.NewGuid();
            var financeManagerId = Guid.NewGuid();
            var expenseDetail = new ExpenseDetails(60, "test2");
            var expense = new Expense(creatorId, expenseDetail);
            expense.SetApprovers(departmentManagerId, financeManagerId);

            // Act
            expense.Submit();
            expense.ApproveCurrentStep(departmentManagerId);
            expense.ApproveCurrentStep(financeManagerId);
            // Assert
            Assert.Equal(ExpenseStatus.Approved, expense.Status);
        }

        [Fact]
        public void Approve_Should_Throw_When_Expense_Is_Not_Submitted()
        {
            var expenseDetail = new ExpenseDetails(60, "test2");

            var creatorId = Guid.NewGuid();
            var expense = new Expense(creatorId, expenseDetail);

            // Act
            var act = () => expense.ApproveCurrentStep(Guid.NewGuid());

            // Assert 
            act.Should().Throw<InvalidExpenseStateException>();
        }

        [Fact]
        public void Approve_Should_Throw_When_Approver_Is_Creator()
        {
            var expenseDetail = new ExpenseDetails(60, "test2");

            var creatorId = Guid.NewGuid();
            var expense = new Expense(creatorId, expenseDetail);
            var departmentManagerId = Guid.NewGuid();
            var financeManagerId = Guid.NewGuid();
            expense.SetApprovers(departmentManagerId, financeManagerId);
            expense.Submit();
            var act = () => expense.ApproveCurrentStep(creatorId);

            act.Should().Throw<InvalidProcessException>();
        }

        [Fact]
        public void One_Approve_Status_Still_Submitted()
        {
            var expenseDetail = new ExpenseDetails(60, "test2");

            var creatorId = Guid.NewGuid();
            var expense = new Expense(creatorId, expenseDetail);
            var departmentManagerId = Guid.NewGuid();
            var financeManagerId = Guid.NewGuid();
            expense.SetApprovers(departmentManagerId, financeManagerId);
            expense.Submit();
            expense.ApproveCurrentStep(departmentManagerId);
            Assert.Equal(ExpenseStatus.Submitted, expense.Status);
        }

        [Fact]
        public void Submit_Should_Throw_When_No_Approver_Set()
        {


            var expenseDetail = new ExpenseDetails(60, "test2");
            var creatorId = Guid.NewGuid();
            var expense = new Expense(creatorId, expenseDetail);
            var act = () => expense.Submit();
            act.Should().Throw<InvalidProcessException>();
        }

        [Fact]
        public void Approve_Should_Throw_When_Same_Step_Approved_Twice()
        {
            var creatorId = Guid.NewGuid();
            var dept = Guid.NewGuid();
            var finance = Guid.NewGuid();
            var expenseDetail = new ExpenseDetails(60, "test2");

            var expense = new Expense(creatorId, expenseDetail);
            expense.SetApprovers(dept, finance);
            expense.Submit();

            expense.ApproveCurrentStep(dept);

            var act = () => expense.ApproveCurrentStep(dept);

            act.Should().Throw<InvalidProcessException>();
        }


        [Fact]
        public void Approve_Should_Throw_When_Expense_Already_Approved()
        {
            var creatorId = Guid.NewGuid();
            var dept = Guid.NewGuid();
            var finance = Guid.NewGuid();
            var expenseDetail = new ExpenseDetails(60, "test2");

            var expense = new Expense(creatorId, expenseDetail);
            expense.SetApprovers(dept, finance);
            expense.Submit();

            expense.ApproveCurrentStep(dept);
            expense.ApproveCurrentStep(finance);

            var act = () => expense.ApproveCurrentStep(finance);

            act.Should().Throw<InvalidExpenseStateException>();
        }

    }
}
