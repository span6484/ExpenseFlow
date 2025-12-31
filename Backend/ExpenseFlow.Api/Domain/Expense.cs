using ExpenseFlow.Api.Domain.Exceptions;

namespace ExpenseFlow.Api.Domain
{
    public class Expense
    {
        public Guid Id { get; }
        public Guid CreatedByUserId { get; }
        public ExpenseStatus Status { get; private set; }
        public int CurrentStepIndex { get; private set; }
        private readonly List<ApprovalStep> _approvalSteps = new();
        private IReadOnlyList<ApprovalStep> ApprovalSteps => _approvalSteps.AsReadOnly();
        private ExpenseDetails _details;

        public Expense(Guid createdByUserId)
        {
            Id = Guid.NewGuid();
            CreatedByUserId = createdByUserId;
            Status = ExpenseStatus.Draft;
            CurrentStepIndex = 0;
        }

        /// <summary>
        /// Change the status from draft to submitted
        /// </summary>
        /// <param name="steps"></param>
        /// <exception cref="InvalidExpenseStateException"></exception>
        public void Submit(IEnumerable<ApprovalStep> steps)
        {
            if (Status != ExpenseStatus.Draft && Status != ExpenseStatus.Rejected) 
                throw new InvalidExpenseStateException(
                    currentStatus: Status,
                    expectedStatus: ExpenseStatus.Draft);   
            _approvalSteps.Clear();
            _approvalSteps.AddRange(steps);
            CurrentStepIndex = 0;
            Status = ExpenseStatus.Submitted;
        }

        /// <summary>
        /// Approval is a step-level decision.The aggregate updates its overall status only when the final step is approved.
        /// </summary>
        /// <exception cref="InvalidExpenseStateException"></exception>
        public void ApproveCurrentStep()
        {
            if (Status != ExpenseStatus.Submitted)
                throw new InvalidExpenseStateException(
                    currentStatus: Status,
                    expectedStatus: ExpenseStatus.Submitted);
            var step = _approvalSteps[CurrentStepIndex];
            step.Approve();
            if (CurrentStepIndex == _approvalSteps.Count - 1)
            {
                Status = ExpenseStatus.Approved;
            }
            else
            {
                CurrentStepIndex++;
            }
        }

        public void RejectCurrentStep()
        {
            if (Status != ExpenseStatus.Submitted)
                throw new InvalidExpenseStateException(
                    currentStatus: Status,
                    expectedStatus: ExpenseStatus.Submitted);
            var step = _approvalSteps[CurrentStepIndex];
            step.Reject();
            Status = ExpenseStatus.Rejected;
        }
        /// <summary>
        /// existed when the expense submitted but haven't been processed 
        /// </summary>
        /// <exception cref="InvalidExpenseStateException"></exception>
        public void Withdraw()
        {
            if (Status != ExpenseStatus.Submitted)
                throw new InvalidExpenseStateException(
                    currentStatus: Status,
                    expectedStatus: ExpenseStatus.Submitted);
            CurrentStepIndex = 0;
            _approvalSteps.Clear();
            Status = ExpenseStatus.Draft;
        }
        /// <summary>
        /// Edit when the expense in Draft or Rejected Status
        /// </summary>
        /// <param name="update"></param>
        /// <exception cref="InvalidExpenseStateException"></exception>
        public void Edit(ExpenseDetails update)
        {
            if(Status != ExpenseStatus.Draft && Status != ExpenseStatus.Rejected)
                throw new InvalidExpenseStateException(
                    currentStatus: Status,
                    expectedStatus: ExpenseStatus.Draft);
            _details = update;
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
