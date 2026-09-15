# Tasks: Document Upload and Management

**Input**: Design documents from `/specs/001-document-upload-management/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Prepare the repository and shared configuration for the document feature.

- [X] T001 Create feature folder structure and document planning artifacts in `specs/001-document-upload-management/`
- [X] T002 [P] Add local upload and async scan configuration entries to `ContosoDashboard/appsettings.json` and `ContosoDashboard/appsettings.Development.json`
- [X] T003 [P] Add EF Core document model scaffolding to `ContosoDashboard/Data/ApplicationDbContext.cs` for `Document`, `DocumentShare`, and `DocumentAuditLog`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Complete the shared storage, security, and queue foundations before implementing user-facing stories.

- [X] T004 Implement file storage abstraction in `ContosoDashboard/Services/IFileStorageService.cs`
- [X] T005 [P] Implement local file-backed storage in `ContosoDashboard/Services/LocalFileStorageService.cs`
- [X] T006 [P] Add document domain entities in `ContosoDashboard/Models/Document.cs`, `ContosoDashboard/Models/DocumentShare.cs`, and `ContosoDashboard/Models/DocumentAuditLog.cs`
- [X] T007 Add queue-based scan job message contract in `ContosoDashboard/Services/DocumentScanJobMessage.cs`
- [X] T008 Implement scan status service in `ContosoDashboard/Services/DocumentScanStatusService.cs`
- [X] T009 Add document service shell and dependency registration in `ContosoDashboard/Services/DocumentService.cs` and `ContosoDashboard/Program.cs`
- [ ] T010 Add validation, logging, and audit plumbing for document actions in `ContosoDashboard/Services/DocumentService.cs` and `ContosoDashboard/Services/NotificationService.cs`

**Checkpoint**: Foundation ready - user story implementation can begin in parallel.

---

## Phase 3: User Story 1 - Upload and classify work documents (Priority: P1) 🎯 MVP

**Goal**: Employees can upload valid documents, supply metadata, and see them in the correct personal or project list.

**Independent Test**: An authenticated employee can upload a supported document with category and project metadata and verify that it appears only in the correct document views.

### Tests for User Story 1

- [X] T011 [P] [US1] Add failing upload validation tests in `ContosoDashboard.Tests/Services/DocumentServiceUploadTests.cs`
- [X] T012 [P] [US1] Add failing security-scan pending-state tests in `ContosoDashboard.Tests/Services/DocumentServiceScanTests.cs`

### Implementation for User Story 1

- [X] T013 [US1] Implement upload validation, file extension checks, size limits, and scan enqueue in `ContosoDashboard/Services/DocumentService.cs`
- [X] T014 [P] [US1] Add upload UI and metadata form in `ContosoDashboard/Pages/Documents.razor` and `ContosoDashboard/Pages/DocumentUpload.razor`
- [X] T015 [US1] Add project and personal document persistence logic in `ContosoDashboard/Data/ApplicationDbContext.cs` and `ContosoDashboard/Models/Document.cs`
- [X] T016 [US1] Add upload success/error feedback and status handling in `ContosoDashboard/Pages/Documents.razor`

**Checkpoint**: User Story 1 should be independently functional and testable.

---

## Phase 4: User Story 2 - Search, preview, and access shared documents (Priority: P1)

**Goal**: Employees can find documents quickly, preview supported files, and access only documents the current user is permitted to view.

**Independent Test**: A user can search by title, tag, or project and successfully preview or download a matching document without seeing unrelated files.

### Tests for User Story 2

- [ ] T017 [P] [US2] Add failing search and authorization tests in `ContosoDashboard.Tests/Services/DocumentServiceSearchTests.cs`
- [ ] T018 [P] [US2] Add failing preview access tests in `ContosoDashboard.Tests/Services/DocumentServicePreviewTests.cs`

### Implementation for User Story 2

- [ ] T019 [US2] Implement `GetDocuments`, `SearchDocuments`, and authorization filtering in `ContosoDashboard/Services/DocumentService.cs`
- [ ] T020 [P] [US2] Add document listing and search UI in `ContosoDashboard/Pages/Documents.razor` and `ContosoDashboard/Pages/ProjectDetails.razor`
- [ ] T021 [US2] Add in-browser preview and download flow in `ContosoDashboard/Pages/Documents.razor`
- [ ] T022 [US2] Enforce project membership and role checks before preview or file access in `ContosoDashboard/Services/DocumentService.cs`

**Checkpoint**: User Stories 1 and 2 both work independently.

---

## Phase 5: User Story 3 - Manage and share documents with team members (Priority: P2)

**Goal**: Authorized users can update metadata, replace files, delete documents, and share them with project or user audiences.

**Independent Test**: A user can edit document metadata, replace a file, and share it with a teammate who receives a notification and sees the document in the shared list only if access is allowed.

### Tests for User Story 3

- [ ] T023 [P] [US3] Add failing metadata-edit and share tests in `ContosoDashboard.Tests/Services/DocumentServiceShareTests.cs`
- [ ] T024 [P] [US3] Add failing replacement-and-delete coverage in `ContosoDashboard.Tests/Services/DocumentServiceLifecycleTests.cs`

### Implementation for User Story 3

- [ ] T025 [US3] Implement metadata update, replacement, delete, and share logic in `ContosoDashboard/Services/DocumentService.cs`
- [ ] T026 [P] [US3] Add document actions UI in `ContosoDashboard/Pages/Documents.razor`
- [ ] T027 [US3] Send in-app share notifications in `ContosoDashboard/Services/NotificationService.cs` and `ContosoDashboard/Pages/Index.razor`
- [ ] T028 [US3] Add share-record persistence and document recipient visibility logic in `ContosoDashboard/Models/DocumentShare.cs` and `ContosoDashboard/Data/ApplicationDbContext.cs`

**Checkpoint**: The document lifecycle is independently usable for authorized users.

---

## Phase 6: User Story 4 - Use document features from dashboard workflows (Priority: P2)

**Goal**: Documents feel native to the dashboard experience and can be attached to task workflows.

**Independent Test**: A user can attach a valid document to a task and see recent document activity in the dashboard without leaving the main workflow.

### Tests for User Story 4

- [ ] T029 [P] [US4] Add failing dashboard and task integration tests in `ContosoDashboard.Tests/Services/DashboardServiceDocumentTests.cs`

### Implementation for User Story 4

- [ ] T030 [P] [US4] Add recent document summary and count logic in `ContosoDashboard/Services/DashboardService.cs`
- [ ] T031 [US4] Update dashboard home and summary widgets in `ContosoDashboard/Pages/Index.razor`
- [ ] T032 [P] [US4] Add task attachment support in `ContosoDashboard/Pages/Tasks.razor` and `ContosoDashboard/Services/TaskService.cs`
- [ ] T033 [US4] Link document actions back to project and task context in `ContosoDashboard/Pages/ProjectDetails.razor`

**Checkpoint**: Dashboard and task integration is independently usable.

---

## Phase 7: User Story 5 - Monitor document activity and maintain compliance (Priority: P3)

**Goal**: Administrators can review document activity and confirm uploads, downloads, deletes, and scan outcomes are auditable.

**Independent Test**: An administrator can review a document activity log and verify that scan failures, access denials, and successful actions are recorded.

### Tests for User Story 5

- [ ] T034 [P] [US5] Add failing document audit log tests in `ContosoDashboard.Tests/Services/DocumentAuditTests.cs`
- [ ] T035 [P] [US5] Add failing blocked-scan status tests in `ContosoDashboard.Tests/Services/DocumentScanStatusTests.cs`

### Implementation for User Story 5

- [ ] T036 [US5] Implement audit logging for upload, download, preview, delete, share, and scan events in `ContosoDashboard/Services/DocumentService.cs`
- [ ] T037 [P] [US5] Implement document reporting and admin-view logic in `ContosoDashboard/Pages/Index.razor` or a new admin report page file under `ContosoDashboard/Pages/`
- [ ] T038 [US5] Add scan-result status transitions and blocked-document handling in `ContosoDashboard/Services/DocumentScanStatusService.cs`

**Checkpoint**: Administrators have independent audit visibility and document compliance evidence.

---

## Phase 8: Polish & Cross-Cutting Concerns

**Purpose**: Hardening, documentation, and final validation across all stories.

- [ ] T039 [P] Security hardening for file path generation and authorization checks in `ContosoDashboard/Services/DocumentService.cs` and `ContosoDashboard/Services/LocalFileStorageService.cs`
- [ ] T040 [P] Add final documentation, usage notes, and quickstart updates in `README.md` and `specs/001-document-upload-management/quickstart.md`
- [ ] T041 Run end-to-end validation of upload, search, preview, share, and admin audit flows using the quickstart and smoke checks
- [ ] T042 [P] Optimize document list and search queries for the 500-document performance requirement in `ContosoDashboard/Services/DocumentService.cs` and `ContosoDashboard/Data/ApplicationDbContext.cs`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately.
- **Foundational (Phase 2)**: Depends on Setup completion; blocks all user-story work.
- **User Stories (Phase 3–7)**: All depend on Foundational completion; they can proceed in parallel by team if needed.
- **Polish (Phase 8)**: Depends on all desired user stories being complete.

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational; no dependencies on other stories.
- **User Story 2 (P1)**: Can start after Foundational; depends on the shared document service, but should be independently testable.
- **User Story 3 (P2)**: Can start after foundational and may integrate with User Story 1/2, but remains independently testable.
- **User Story 4 (P2)**: Depends on document creation/search infrastructure and shares some service contracts with User Story 1 and 2.
- **User Story 5 (P3)**: Depends on document lifecycle events and audit logging; can be started once the base document service is complete.

### Parallel Opportunities

- All Setup tasks marked `[P]` can run in parallel.
- All Foundational tasks marked `[P]` can run in parallel within Phase 2.
- Story-specific tests in each phase can run in parallel.
- Different stories can be implemented by different contributors once the foundation is ready.

---

## Parallel Example: User Story 1

```bash
# Launch upload validation and pending-scan tests together
Task: "Add failing upload validation tests in ContosoDashboard.Tests/Services/DocumentServiceUploadTests.cs"
Task: "Add failing security-scan pending-state tests in ContosoDashboard.Tests/Services/DocumentServiceScanTests.cs"

# Launch model and UI work in parallel once the service contract is ready
Task: "Add upload UI and metadata form in ContosoDashboard/Pages/Documents.razor and ContosoDashboard/Pages/DocumentUpload.razor"
Task: "Add project and personal document persistence logic in ContosoDashboard/Data/ApplicationDbContext.cs and ContosoDashboard/Models/Document.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational
3. Complete Phase 3: User Story 1
4. Stop and validate the upload flow independently
5. Extend to User Story 2 and beyond only after the core upload path is stable

### Incremental Delivery

1. Setup + Foundational -> shared storage and security foundation
2. User Story 1 -> upload and classification capability
3. User Story 2 -> search, preview, and access controls
4. User Story 3 -> management and sharing
5. User Story 4 -> dashboard integration
6. User Story 5 -> audit and compliance reporting
7. Final polish and performance tuning

### Parallel Team Strategy

With multiple contributors:

1. Shared setup and foundation work as a single team effort.
2. Developer A: User Story 1
3. Developer B: User Story 2
4. Developer C: User Story 3
5. Developer D: User Story 4 and User Story 5
6. Final validation and hardening by all contributors together

---

## Notes

- `[P]` tasks are safe to run in parallel when they target different files or independent code paths.
- Each story remains independently testable and does not require a full end-to-end implementation before validation.
- Follow the TDD flow: add failing tests, implement the minimum root-cause fix, then verify the story behavior before moving on.
- Keep the local first implementation self-contained while preserving the Azure queue-driven scan pattern for future production deployment.
