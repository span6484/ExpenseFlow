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
        public IReadOnlyList<ApprovalStep> ApprovalSteps => _approvalSteps;

        public Expense(Guid createdByUserId)
        {
            Id = Guid.NewGuid();
            CreatedByUserId = createdByUserId;
            Status = ExpenseStatus.Draft;
            CurrentStepIndex = 0;
        }
        public void Submit(IEnumerable<ApprovalStep> steps)
        {
            if (Status != ExpenseStatus.Draft)
                throw new InvalidExpenseStateException(
                    currentStatus: Status,
                    expectedStatus: ExpenseStatus.Draft);   
            _approvalSteps.Clear();
            _approvalSteps.AddRange(steps);
            CurrentStepIndex = 0;
            Status = ExpenseStatus.Submitted;
        }

        /// <summary>
        /// Only existed when the expense submitted but haven't been processed 
        /// </summary>
        /// <exception cref="InvalidExpenseStateException"></exception>
        public void Withdraw()
        {
            if (Status != ExpenseStatus.Submitted)
                throw new UnauthorizedExpenseOperationException();
            CurrentStepIndex = 0;
            _approvalSteps.Clear();
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
