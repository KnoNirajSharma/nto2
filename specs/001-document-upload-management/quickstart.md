# Quickstart: Document Upload and Management Validation

## Prerequisites

- .NET 8 SDK or later available on the workstation
- The ContosoDashboard project is running locally
- A browser is available to access the dashboard login page
- A seeded employee account such as `ni.kang@contoso.com` or `camille.nicole@contoso.com` is available

## Run the application

1. Open a terminal in the repository root.
2. Run the app from the project directory:
   `dotnet run --project ContosoDashboard/ContosoDashboard.csproj`
3. Open the login page and sign in as a seeded user.

## Validation scenarios

### 1. Upload a document and queue scan processing

- Navigate to the document area from the dashboard or project page.
- Select a valid PDF, Office document, image, or text file below 25 MB.
- Add metadata: title, category, description, project, and optional tags.
- Submit the upload.
- Confirm the upload success message appears and the document enters a pending-scanning state before approval.
- Observe the scan status changing from `Pending` to `Passed` or `Blocked` when the background worker completes.

Expected result: the document is stored, categorized, and only becomes fully usable after the scan job completes successfully.

### 2. Search for a document

- Search by title, project name, uploader, or tag value.
- Confirm results appear within the expected response window.
- Validate that only authorized documents appear in the results.

Expected result: search results are restricted to the current user’s access scope and match the query fields.

### 3. Preview and download

- Pick a PDF or image from the document list.
- Use preview to open the document in-browser.
- Use download on an authorized document.

Expected result: the requested document is rendered or downloaded without exposing unauthorized files.

### 4. Share a document

- Share a document with another project member or team member.
- Log in as the recipient and confirm the notification appears.
- Check the shared documents list for the received item.

Expected result: the recipient sees the file only when within authorized access rules.

### 5. Audit and admin reporting

- Perform upload, download, delete, or share actions.
- Log in as an administrator.
- Review the audit activity for the relevant document.

Expected result: the system records the event, the actor, and the outcome for reporting.

## Exit criteria

The feature is considered ready for deeper implementation when the validation scenarios above succeed without authorization bypasses, unsupported file acceptance, or missing audit trails.
