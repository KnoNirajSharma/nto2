<!-- Sync Impact Report
Version change: 0.0.0 -> 1.0.0
Modified principles: N/A (new constitution established)
Added sections: Training Constraints; Development Workflow
Removed sections: N/A
Follow-up TODOs: None
-->

# ContosoDashboard Constitution

## Core Principles

### I. Offline-First Training Integrity
This project exists for learning and demonstration, so every feature must remain runnable offline without external cloud services or secret-bearing infrastructure. Training scenarios must work from the local workstation and use mock identity, local storage, and sample data unless a specific exercise explicitly calls for a cloud dependency.

### II. Security-by-Default
All protected pages and service methods must enforce authorization before exposing data or state. No user may access records outside their allowed scope; role checks, page attributes, and service validation must all agree. The mock authentication and authorization model is for training and must never be treated as production security.
### III. Test-First Behavior
Any new behavior or change in access rules must start with a failing specification or test description. Features are implemented only after the expected user-visible behavior is written down and validated. The team must verify that tasks, projects, notifications, and authorization rules behave correctly before merging.

### IV. Separation of Concerns
Business logic, data access, authentication, and UI concerns must remain distinct. Services own domain rules and authorization decisions; pages and components orchestrate user interaction; data models 
and the DbContext define persistence boundaries. Avoid mixing infrastructure concerns directly into the UI.

### V. Simple, Explainable Architecture
Prefer small, readable components and clear service boundaries over clever abstractions. The codebase must remain understandable to learners, with names that reflect the domain, straightforward flows, and minimal hidden state. When a pattern improves clarity and maintainability, prefer it over complexity for its own sake.

## Training Constraints

ContosoDashboard is a sandbox application for Spec-Driven Development education and not a production reference implementation. The codebase must remain intentionally local-first, self-contained, and safe for offline use. LocalDB, filesystem storage, mock authentication, and in-memory or seed-based scenarios are acceptable. Features that require external APIs, managed identity, production databases, or cloud hosting must be clearly labeled as future work and must not be assumed to exist in the current application.

## Development Workflow

The team must use the project’s specification workflow before implementation changes. Requirements, design intent, and acceptance criteria must be captured in the repo before code changes are made. Pull requests must show how the work aligns with the current specification, explain any security or authorization impact, and verify that the relevant behavior still works.

## Governance

This constitution governs project decisions that affect architecture, security, workflow, and training scope. Changes to these rules require a written amendment, a clear rationale, and review before they are accepted. When a change alters behavior in a way that affects learners, the team must update the relevant specification or documentation and confirm the change remains aligned with the project’s training purpose.

All PRs and code changes must be checked against this constitution for compliance with the principles above, especially around offline-first execution, authorization enforcement, and clear separation of concerns. Major re-architecture or security-related changes must be documented and approved before they are merged. Versioning follows semantic versioning: MAJOR for incompatible governance changes, MINOR for new principles or materially expanded guidance, and PATCH for clarifications or wording fixes.

**Version**: 1.0.0 | **Ratified**: 2026-09-15 | **Last Amended**: 2026-09-15
