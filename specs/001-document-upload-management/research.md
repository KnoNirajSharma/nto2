# Research: Document Upload and Management

## Decision: Use local file storage with a storage abstraction, role-aware service checks, and queued malware scanning

The application will keep its training implementation offline-first by storing uploaded document payloads in a local directory and persisting metadata in SQLite through the existing `ApplicationDbContext`. A `IFileStorageService` abstraction will be introduced so the repository can later switch to Azure Blob Storage without changing the document service, page behavior, or business rules. File uploads will also be queued for asynchronous malware scanning so the upload experience remains responsive while a background worker validates the file.

### Rationale

- The constitution requires offline-first training behavior and explicit separation of concerns.
- The current project already uses service abstractions and dependency injection patterns for business logic.
- A local filesystem keeps the app self-contained and avoids requiring Azure credentials in a learning environment.
- The same model works cleanly for a future cloud-backed implementation by swapping only the storage provider.

### Alternatives considered

1. Store documents directly in `wwwroot`.
   - Rejected because it exposes application files and weakens the security model.

2. Store only metadata and no file payload.
   - Rejected because the feature requires actual file upload, preview, and download behavior.

3. Use Azure Blob Storage immediately.
   - Rejected for this training context because the app must run offline and without cloud dependencies.

## Decision: Keep document metadata in integer-keyed EF entities aligned to the current schema

Document records will follow the current project pattern where the primary key is an integer ID, consistent with `User`, `Project`, and existing task entities. Category values will remain text strings rather than enum-backed integers to preserve clarity in training and to simplify filters and reporting.

### Rationale

- The stakeholder brief explicitly requires integer document IDs and text categories.
- The existing data model uses integer IDs throughout the application and the repository already follows this pattern.
- Storing category values as text reduces complexity for users and for UI filters without sacrificing maintainability.

### Alternatives considered

1. GUID-based document IDs.
   - Rejected because the repo’s existing schema and data conventions use integer keys.

2. Enum-backed categories.
   - Rejected because the feature requires simple text-based categorization and reporting.

## Decision: Use a document service for all authorization, audit logic, and queued scan requests

The feature will be built around a dedicated `DocumentService` that validates file type and size, checks project membership, enforces access rules, writes metadata records, triggers notifications, writes audit events, and enqueues a scan job for uploaded files. UI pages will delegate to this service rather than directly reading database records or file paths.

### Rationale

- This matches the project’s service layer architecture and reduces the chance of unauthorized access.
- Authorization rules can be enforced centrally and reused across dashboard pages, task views, and project document views.
- Audit logging becomes easier to standardize and report centrally.

### Alternatives considered

1. Authorize directly inside pages and components.
   - Rejected because it duplicates logic and increases the risk of bypassing checks.

2. Grant broad access to all project members.
   - Rejected because the requirement explicitly requires role-aware and project-aware security boundaries.

## Decision: Keep core feature scope to upload, browse, search, share, preview, and audit, with Azure Functions queue-driven scanning in production

The implementation will not include version history, soft delete, quotas, collaborative editing, or external integrations as part of the initial release. These are explicitly out of scope and will remain separate work if required later. For the production Azure pattern, the scan worker will be implemented as an Azure Function with a Queue Storage trigger, while the training implementation can use a local background job or simulated queue processor to preserve offline use.

### Rationale

- It fits the 8-10 week delivery constraint.
- It reduces implementation risk while still delivering a high-value MVP.
- It keeps the feature aligned with the project’s simplified training scope.

### Alternatives considered

1. Include version history and trash workflows upfront.
   - Rejected because they add complexity and are explicitly excluded from the feature requirements.

2. Build external integration at the same time.
   - Rejected because the app is intentionally offline-first and the feature scope excludes those integrations.

## Unknowns resolved

The following clarifications were resolved during the planning process:

- Storage pattern: Local file system under a secure path outside `wwwroot` with GUID-based file names.
- Metadata model: Integer document keys and text categories.
- Security model: Shared access must be enforced by project membership and role checks.
- Delivery scope: Upload, search, preview, sharing, notifications, and audit are in scope; versioning and trash behavior are not.
- Malware-scan pattern: A background queue job will process file validation asynchronously; in Azure, an Azure Function with Queue Storage trigger is the recommended production implementation.
