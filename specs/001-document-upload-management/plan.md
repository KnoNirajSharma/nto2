# Implementation Plan: Document Upload and Management

**Branch**: `001-document-upload-management` | **Date**: 2026-09-15 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `/specs/001-document-upload-management/spec.md`

## Summary

This feature adds a secure document repository to ContosoDashboard that lets employees upload, classify, search, preview, share, and audit work-related files without disrupting the current dashboard workflow. The implementation will use the repository’s existing Blazor Server + EF Core architecture, keep the training environment offline-first, and apply a storage abstraction so the same business logic can later swap local file storage for Azure Blob Storage without changing the UI or main service flow.

## Technical Context

**Language/Version**: C# with ASP.NET Core 8.0 / Blazor Server; repository currently reflects the .NET 8 stack with compatibility expectations for the current .NET 10 build environment.
**Primary Dependencies**: ASP.NET Core, Blazor Server, EF Core, SQLite, Cookie-based authentication, Bootstrap UI.
**Storage**: SQLite database for metadata and local filesystem within AppData/uploads for document payloads in the training implementation; future Azure Blob Storage + Queue Storage path for async scanning.
**Testing**: xUnit for service and authorization validation; bUnit for Blazor component flows; integration tests for upload/search/security checks; queue-trigger validation for malware-scanning jobs.
**Target Platform**: Local Windows workstation / development environment for training; future ready for Azure deployment patterns.
**Project Type**: Web application (Blazor Server + Razor Pages) with an async background-processing extension for malware scanning.
**Performance Goals**: Upload within 30 seconds for 25 MB files; document list and search under 2 seconds for 500 records; preview under 3 seconds for common document types; scan queue processing under a predictable asynchronous SLA for accepted uploads.
**Constraints**: Offline-first training app, local files only, custom cookie auth, zero major rewrite, role-based access controls, no out-of-scope features, virus scanning must be asynchronous to avoid blocking upload UX.
**Scale/Scope**: Up to 5,000 employees, role-based document visibility, project and personal document collections, dashboard integration on a lightweight app model, and a queue-driven malware screening flow for uploaded files.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

The proposed implementation aligns with the constitution:

- Offline-First Training Integrity: This feature keeps storage local and uses an abstraction for cloud migration without forcing Azure dependencies into the learning environment.
- Security-by-Default: Access checks will be enforced in the service layer and at the page or endpoint boundary; audit logging will be attached to all document actions; uploaded files will be queued for asynchronous malware scanning before approval.
- Test-First Behavior: Upload, authorization, search, sharing, and scan-status flows will be specified with failing tests before implementation.
- Separation of Concerns: Document metadata remains in EF Core models; storage logic is isolated behind a file service abstraction; background scanning is isolated behind an async queue-trigger worker; UI remains focused on interaction and display.
- Simple, Explainable Architecture: One document domain model, one storage abstraction, one document service, and one async scanning worker keep the feature understandable while supporting enterprise-grade malware handling.

No constitution violations are present that require a waiver.

## Project Structure

### Documentation (this feature)

```text
specs/001-document-upload-management/
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
├── contracts/           # Phase 1 output
├── spec.md              # Source feature specification
└── checklists/
    └── requirements.md
```

### Source Code (repository root)

```text
ContosoDashboard/
├── Data/
│   └── ApplicationDbContext.cs
├── Models/
│   ├── User.cs
│   ├── Project.cs
│   ├── TaskItem.cs
│   ├── Notification.cs
│   ├── ProjectMember.cs
│   └── Announcement.cs
├── Services/
│   ├── IUserService.cs
│   ├── UserService.cs
│   ├── ITaskService.cs
│   ├── TaskService.cs
│   ├── IProjectService.cs
│   ├── ProjectService.cs
│   ├── INotificationService.cs
│   ├── NotificationService.cs
│   ├── IDashboardService.cs
│   ├── DashboardService.cs
│   ├── CustomAuthenticationStateProvider.cs
│   └── [new] DocumentService, IFileStorageService, LocalFileStorageService, DocumentScanJobMessage, DocumentScanStatusService
├── Pages/
│   ├── Index.razor
│   ├── Projects.razor
│   ├── Tasks.razor
│   ├── ProjectDetails.razor
│   └── [new] Documents.razor, DocumentUpload.razor, SharedDocuments.razor
├── Shared/
│   ├── MainLayout.razor
│   └── NavMenu.razor
├── wwwroot/
│   └── css/
├── Program.cs
├── appsettings.json
└── appsettings.Development.json
```

**Structure Decision**: The feature will extend the current Blazor Server service and page model rather than introducing a separate app. Document metadata belongs in the existing EF Core model layer, file handling will be isolated behind a service abstraction, and malware scanning will be handled by a dedicated background worker pattern that can be implemented as an Azure Function on Queue Storage in production while the training app remains local-first.

### Background virus-scanning job

The upload flow will enqueue a document-scan request immediately after a file is successfully staged in local storage. The scan request will include the document ID, storage path, file name, and security-state metadata. A queue-based worker will process the request asynchronously so the user is not blocked during the scan. In the production Azure design, the same contract is implemented as an Azure Function triggered by Azure Queue Storage messages; in local training mode the worker can run as a lightweight hosted background process or an emulated queue consumer.

The expected production pattern is:

1. User uploads file and metadata are persisted with a `PendingScan` status.
2. `DocumentService` writes a scan message to a queue with a document reference and file path.
3. Azure Function triggered by Queue Storage reads the queue item and performs the malware or antivirus validation.
4. The function updates the document record to `Scanned`, `Blocked`, or `Failed`, records the scan result in the audit log, and triggers notifications or cleanup as appropriate.
5. The UI polls or refreshes the document status and only exposes approved files after the scan succeeds.

This keeps the UX responsive while preserving a secure-by-default model for document publication.

## Complexity Tracking

No constitution violations require a complexity waiver for this feature.
