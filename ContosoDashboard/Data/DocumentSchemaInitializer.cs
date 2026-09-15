using Microsoft.EntityFrameworkCore;

namespace ContosoDashboard.Data;

public static class DocumentSchemaInitializer
{
    public static void EnsureCreated(ApplicationDbContext context)
    {
        context.Database.ExecuteSqlRaw(@"
            CREATE TABLE IF NOT EXISTS Documents (
                DocumentId INTEGER NOT NULL CONSTRAINT PK_Documents PRIMARY KEY AUTOINCREMENT,
                Title TEXT NOT NULL,
                Category TEXT NOT NULL,
                Description TEXT NULL,
                FileName TEXT NOT NULL,
                ContentType TEXT NULL,
                StoragePath TEXT NOT NULL,
                FileSizeBytes INTEGER NOT NULL,
                UploadedByUserId INTEGER NOT NULL,
                ProjectId INTEGER NULL,
                Tags TEXT NOT NULL,
                UploadedDate TEXT NOT NULL,
                LastModifiedDate TEXT NULL,
                Status INTEGER NOT NULL,
                SecurityScanStatus INTEGER NOT NULL,
                ScanRequestedAtUtc TEXT NULL,
                ScanCompletedAtUtc TEXT NULL,
                ScanResultSummary TEXT NULL,
                CONSTRAINT FK_Documents_Users_UploadedByUserId FOREIGN KEY (UploadedByUserId) REFERENCES Users (UserId) ON DELETE RESTRICT,
                CONSTRAINT FK_Documents_Projects_ProjectId FOREIGN KEY (ProjectId) REFERENCES Projects (ProjectId) ON DELETE SET NULL
            );

            CREATE TABLE IF NOT EXISTS DocumentShares (
                DocumentShareId INTEGER NOT NULL CONSTRAINT PK_DocumentShares PRIMARY KEY AUTOINCREMENT,
                DocumentId INTEGER NOT NULL,
                SharedByUserId INTEGER NOT NULL,
                SharedWithUserId INTEGER NOT NULL,
                ProjectId INTEGER NULL,
                AccessLevel TEXT NOT NULL,
                SharedDate TEXT NOT NULL,
                NotificationSent INTEGER NOT NULL,
                CONSTRAINT FK_DocumentShares_Documents_DocumentId FOREIGN KEY (DocumentId) REFERENCES Documents (DocumentId) ON DELETE CASCADE,
                CONSTRAINT FK_DocumentShares_Users_SharedByUserId FOREIGN KEY (SharedByUserId) REFERENCES Users (UserId) ON DELETE RESTRICT,
                CONSTRAINT FK_DocumentShares_Users_SharedWithUserId FOREIGN KEY (SharedWithUserId) REFERENCES Users (UserId) ON DELETE RESTRICT
            );

            CREATE TABLE IF NOT EXISTS DocumentAuditLogs (
                DocumentAuditLogId INTEGER NOT NULL CONSTRAINT PK_DocumentAuditLogs PRIMARY KEY AUTOINCREMENT,
                DocumentId INTEGER NOT NULL,
                UserId INTEGER NOT NULL,
                EventType TEXT NOT NULL,
                EventDescription TEXT NULL,
                OccurredUtc TEXT NOT NULL,
                CONSTRAINT FK_DocumentAuditLogs_Documents_DocumentId FOREIGN KEY (DocumentId) REFERENCES Documents (DocumentId) ON DELETE CASCADE,
                CONSTRAINT FK_DocumentAuditLogs_Users_UserId FOREIGN KEY (UserId) REFERENCES Users (UserId) ON DELETE RESTRICT
            );

            CREATE INDEX IF NOT EXISTS IX_Documents_UploadedByUserId ON Documents (UploadedByUserId);
            CREATE INDEX IF NOT EXISTS IX_Documents_ProjectId ON Documents (ProjectId);
            CREATE INDEX IF NOT EXISTS IX_Documents_Status ON Documents (Status);
            CREATE INDEX IF NOT EXISTS IX_Documents_SecurityScanStatus ON Documents (SecurityScanStatus);
            CREATE INDEX IF NOT EXISTS IX_Documents_Title ON Documents (Title);
            CREATE INDEX IF NOT EXISTS IX_DocumentShares_DocumentId ON DocumentShares (DocumentId);
            CREATE INDEX IF NOT EXISTS IX_DocumentShares_SharedWithUserId ON DocumentShares (SharedWithUserId);
            CREATE INDEX IF NOT EXISTS IX_DocumentAuditLogs_DocumentId ON DocumentAuditLogs (DocumentId);
            CREATE INDEX IF NOT EXISTS IX_DocumentAuditLogs_OccurredUtc ON DocumentAuditLogs (OccurredUtc);
        ");
    }
}
