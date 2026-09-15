# Document Management Contract

## Purpose

This document describes the expected service and API-level contract for the ContosoDashboard document management feature. The implementation is intended to fit the current Blazor Server architecture but can be represented as a contract for backend operations and page actions.

## Core operations

### Upload document

**Operation**: `UploadDocument`

**Request**
- User context: authenticated employee
- File payload: one or more files
- Metadata: title, description, category, projectId, tags
- Validation: file extension allowlist, file size check, queue-based antivirus request, authorization check

**Response**
- Success: document object with id, title, category, file metadata, and scan status (`Pending`/`Passed`/`Blocked`)
- Error: validation or security failure with a clear user-facing message

**Background processing**
- After a valid upload is persisted, the system emits a queue message referencing the document ID and storage path.
- In Azure, this queue message will be consumed by an Azure Function with a Queue Storage trigger.
- The scan worker updates the document status and the audit log when scanning completes.

### List documents

**Operation**: `GetDocuments`

**Request**
- User context: authenticated user
- Optional filters: category, projectId, date range, uploadedBy

**Response**
- Collection of documents the current user is permitted to view
- Includes title, category, project association, uploader, upload date, and size

### Search documents

**Operation**: `SearchDocuments`

**Request**
- Query string for title, description, uploader, tag, or project
- User context for authorization filtering

**Response**
- Matching documents sorted by relevance and newest first
- Restricted to authorized results only

### Preview document

**Operation**: `PreviewDocument`

**Request**
- Document identifier
- User context

**Response**
- Inline preview payload for supported file types such as PDF or image
- Access denied when the user cannot view the document

### Download document

**Operation**: `DownloadDocument`

**Request**
- Document identifier
- User context

**Response**
- File stream or binary payload for permissioned users
- Access denied when the user lacks authorization

### Share document

**Operation**: `ShareDocument`

**Request**
- Document identifier
- Share target: user or project
- Share actor: authenticated user
- Optional message

**Response**
- Share record created
- Notification emitted to recipients if access is granted
- Audit event recorded

### Delete document

**Operation**: `DeleteDocument`

**Request**
- Document identifier
- User context
- User confirmation

**Response**
- Document removed from active list
- Security and audit records preserved

## Security contract

- Only authenticated users may upload or access documents.
- Document access decisions must be enforced in the service layer.
- Unauthorized attempts must be rejected before file content is served.
- Every document action must produce an audit event.
- Failed security scans must block uploads before storage.

## Data contract

Document records should provide enough metadata for browsing and display without exposing paths outside the authorized context. The system should return user-safe metadata only, with actual file system details preserved internally.

## Notes

This contract is intentionally lightweight and aligned to the existing Blazor Server + service architecture. It abstracts the storage details while preserving the requirements for secure access, auditability, and offline-first training use.
