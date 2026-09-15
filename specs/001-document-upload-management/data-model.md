# Data Model: Document Upload and Management

## Overview

This feature extends the existing data model used by the dashboard with document metadata, secure storage references, sharing records, and audit logging. The design keeps the current patterns of integer primary keys, service-layer validation, and project-aware access rules.

## Entities

### Document

Represents a single uploaded document and its metadata.

**Fields**
- DocumentId: integer, primary key
- Title: string, required
- Description: string, optional
- Category: string, required
- FileName: string, required
- StoredFileName: string, required
- FilePath: string, required
- FileSizeBytes: long, required
- MimeType: string, required, up to 255 characters
- UploadedByUserId: integer, required
- ProjectId: integer, optional
- UploadedAtUtc: DateTime, required
- UpdatedAtUtc: DateTime, required
- IsActive: bool, required
- SecurityScanStatus: string, required (Pending, Passed, Failed, Blocked)
- ScanRequestedAtUtc: DateTime, optional
- ScanCompletedAtUtc: DateTime, optional
- ScanResultSummary: string, optional

**Relationships**
- Many-to-one with `User` via `UploadedByUserId`
- Many-to-one with `Project` via `ProjectId` when the file belongs to a project
- One-to-many with `DocumentShare`
- One-to-many with `DocumentAuditLog`

### DocumentShare

Tracks who a document is shared with and the context of the share event.

**Fields**
- DocumentShareId: integer, primary key
- DocumentId: integer, required
- SharedByUserId: integer, required
- SharedWithUserId: integer, optional
- SharedWithProjectId: integer, optional
- ShareType: string, required (for example: User or Project)
- SharedAtUtc: DateTime, required
- Message: string, optional

**Relationships**
- Many-to-one with `Document`
- Many-to-one with `User` for the actor and recipient
- Many-to-one with `Project` for project-level share context

### DocumentAuditLog

Captures record-level audit events for uploads, downloads, preview actions, sharing, deleting, and security rejections, including the asynchronous scan lifecycle.

**Fields**
- DocumentAuditLogId: integer, primary key
- DocumentId: integer, optional
- ActorUserId: integer, required
- EventType: string, required
- EventDescription: string, required
- EventAtUtc: DateTime, required
- Outcome: string, required
- IpAddress: string, optional
- Metadata: string, optional
- QueueMessageId: string, optional

**Relationships**
- Many-to-one with `Document` when the event is tied to a document
- Many-to-one with `User` for the acting employee

## Existing model relationships reused

- `User` continues to own the personal document list and role membership.
- `Project` remains the organizational boundary for project documents and access checks.
- `TaskItem` may reference document attachments but does not replace the document repository model.

## Validation rules

- Title is required and should be trimmed to a non-empty value.
- Category must be one of the allowed values defined in the feature: Project Documents, Team Resources, Personal Files, Reports, Presentations, Other.
- File type must be from the approved allowlist.
- File size must be <= 25 MB.
- Document path must be generated before database insertion and must not rely on user-provided file names.
- Only project members or authorized roles may view or manage project-specific documents.
- Audit records are mandatory for document upload, share, download, preview, and delete events.

## State transitions

### Document lifecycle

- Draft/Uploaded: file accepted and stored, metadata saved, security scan pending or passed
- Active: visible in authorized document lists and search results
- Shared: document available to one or more additional users or project scopes
- Updated: metadata or content replaced by an authorized user
- Deleted: removed from active lists and recorded in the audit log

### Security scan status

- Pending
- Passed
- Failed
- Blocked

## Notes

This model intentionally keeps business logic separate from storage details. The document payload path remains a value in the database, while the actual file storage implementation is behind the storage abstraction.
