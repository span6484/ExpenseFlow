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
        public ExpenseDetails Details { get; private set; }

        // mock audit users
        private Guid _departmentManagerId;
        private Guid _financeManagerId;

        public Expense(Guid createdByUserId, ExpenseDetails details)
        {
            Id = Guid.NewGuid();
            CreatedByUserId = createdByUserId;
            Status = ExpenseStatus.Draft;
            CurrentStepIndex = 0;
            Details = details ?? throw new ArgumentNullException(nameof(details));
        }

        /// <summary>
        /// Change the status from draft to submitted
        /// </summary>
        /// <param name="steps"></param>
        /// <exception cref="InvalidExpenseStateException"></exception>
        public void Submit()
        {
            if (Status != ExpenseStatus.Draft && Status != ExpenseStatus.Rejected) 
                throw new InvalidExpenseStateException(
                    currentStatus: Status,
                    expectedStatus: ExpenseStatus.Draft);

            if (_departmentManagerId == Guid.Empty || _financeManagerId == Guid.Empty)
                throw new InvalidProcessException("Approvers are not configured.");
            _approvalSteps.Clear();

            _approvalSteps.Add(new ApprovalStep(Id, _departmentManagerId));
            _approvalSteps.Add(new ApprovalStep(Id, _financeManagerId));
            CurrentStepIndex = 0;
            Status = ExpenseStatus.Submitted;
        }

        /// <summary>
        /// Approval is a step-level decision.The aggregate updates its overall status only when the final step is approved.
        /// </summary>
        /// <exception cref="InvalidExpenseStateException"></exception>
        public void ApproveCurrentStep(Guid userId)
        {
            if (Status != ExpenseStatus.Submitted)
                throw new InvalidExpenseStateException(
                    currentStatus: Status,
                    expectedStatus: ExpenseStatus.Submitted);

            var step = _approvalSteps[CurrentStepIndex];
            if (step.OperateUserId != userId)
                throw new InvalidProcessException("User is not allowed to approve this step.");
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
        public void WithdrawToDraft()
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
            Details = update;
        }


        /// <summary>
        /// template set mock users id
        /// </summary>
        /// <param name="departmentManagerId"></param>
        /// <param name="financeManagerId"></param>
        public void SetApprovers(Guid departmentManagerId, Guid financeManagerId)
        {
            if (departmentManagerId == Guid.Empty)
                throw new ArgumentException("Department manager is required");

            if (financeManagerId == Guid.Empty)
                throw new ArgumentException("Finance manager is required");

            if (Status != ExpenseStatus.Draft)
                throw new InvalidExpenseStateException(Status, ExpenseStatus.Draft);
            _departmentManagerId = departmentManagerId;
            _financeManagerId = financeManagerId;
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
