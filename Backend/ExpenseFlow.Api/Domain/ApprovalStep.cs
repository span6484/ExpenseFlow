using ExpenseFlow.Api.Domain.Exceptions;

namespace ExpenseFlow.Api.Domain
{
    internal class ApprovalStep
    {

        public Guid Id { get; }
        public Guid OperateUserId { get; }
        public Guid ExpenseId { get; }
        public ApprovalStatus Status { get; private set; }

        public ApprovalStep(Guid expenseId, Guid operateUserId)
        {
            Id = Guid.NewGuid();
            ExpenseId = expenseId;
            OperateUserId = operateUserId;
            Status = ApprovalStatus.Pending;
        }

        public void Approve()
        {
            if (Status != ApprovalStatus.Pending)
                throw new InvalidApprovalStateException(Status, ApprovalStatus.Pending);    
            Status = ApprovalStatus.Approved;
        }

        public void Reject()
        {
            if (Status != ApprovalStatus.Pending)
                throw new InvalidApprovalStateException(Status, ApprovalStatus.Pending);
            Status = ApprovalStatus.Rejected;
        }
    }

    public enum ApprovalStatus
    {
        Pending,
        Approved,
        Rejected
    }
}
