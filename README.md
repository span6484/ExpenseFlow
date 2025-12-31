# ExpenseFlow

ExpenseFlow is a role-based, multi-level approval workflow system designed to model how real-world enterprise approval processes work.  
It focuses on **workflow correctness, authorization boundaries, and system design**, rather than UI-heavy or over-engineered features.

This project is intentionally scoped to demonstrate **backend and system design capability** in a realistic enterprise context.

---

## Why ExpenseFlow?

In many internal systems, approval logic is often oversimplified as a single `status` field or a hard-coded approver ID.  
ExpenseFlow takes a different approach by modeling approvals as **explicit workflow steps**, allowing:

- Multi-level approvals
- Role-based approval pools
- Clear ownership and authorization rules
- Audit-safe and deterministic state transitions

The goal of this project is not to build a full SaaS product, but to design a **clean, extensible approval workflow core**.

---

## Core Concepts

### 1. Approval as Workflow Steps

Instead of assigning an expense to a single approver, each request progresses through a sequence of approval steps.

Each step represents:
- **Who can approve** (by role and department)
- **Whether the step is pending, approved, or rejected**
- **The order of execution in the workflow**

This allows the system to support complex approval scenarios without hardcoding logic.

---

### 2. Role-Based Approval Pools

Approvals are handled by **role-based pools**, not individual users.

For example:
- Any eligible approver within a department can process a pending approval
- The first qualified user to act completes the step
- Once processed, the approval disappears from all other users’ pending lists

This reflects how approvals are commonly handled in real enterprise systems.

---

### 3. Request-Driven, Not Event-Driven

ExpenseFlow does **not** rely on:
- Background listeners
- Database polling
- Message queues

Pending approvals and notification badges are **derived from queries**, not pushed messages.

> If an approval step exists and is pending, it appears.  
> Once processed, it disappears automatically.

---

## High-Level Architecture

```text
User
 └─ belongs to Department
      └─ exposes approval pool

Expense
 └─ progresses through ApprovalSteps
      └─ each step defines who can act and when
```







## Domain vs models

“I modelled expenses as objects that enforce their own state transitions,
 so invalid actions are impossible from the API layer.”



## 你可以用这个“铁律”判断是否正确（非常重要）

> **`ApprovalSteps[CurrentStepIndex]`
>  必须永远指向“当前 Pending 的那一步”。**







注意 

​    `internal class ApprovalStep`

> “ApprovalStep is not an aggregate root, it can only be created by Expense.”