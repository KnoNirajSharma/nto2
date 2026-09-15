using ContosoDashboard.Data;
using ContosoDashboard.Models;
using ContosoDashboard.Services;
using Microsoft.EntityFrameworkCore;

namespace ContosoDashboard.Tests.Services;

public class DocumentServiceUploadTests
{
    [Fact]
    public async Task UploadDocument_WithSupportedFile_StoresDocumentAndSetsPendingScan()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new ApplicationDbContext(options);
        context.Users.Add(new User
        {
            UserId = 10,
            Email = "uploader@contoso.com",
            DisplayName = "Uploader",
            Role = UserRole.Employee,
            Department = "Engineering"
        });
        await context.SaveChangesAsync();

        var storage = new InMemoryDocumentStorage();
        var service = new DocumentService(context, storage, new TestDocumentScanStatusService());

        var result = await service.UploadDocumentAsync(
            userId: 10,
            title: "Project brief",
            category: "project",
            description: "Quarterly planning document",
            projectId: null,
            tags: "planning,qp",
            fileName: "brief.pdf",
            contentType: "application/pdf",
            stream: new MemoryStream(new byte[] { 1, 2, 3, 4, 5 }));

        Assert.NotNull(result);
        Assert.Equal(DocumentStatus.PendingScan, result.Status);
        Assert.Equal("brief.pdf", result.FileName);
        Assert.Equal("Project brief", result.Title);
        Assert.Equal(1, await context.Documents.CountAsync());
    }

    [Fact]
    public async Task UploadDocument_WithUnsupportedExtension_ReturnsValidationError()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new ApplicationDbContext(options);
        context.Users.Add(new User
        {
            UserId = 11,
            Email = "bad@contoso.com",
            DisplayName = "Bad actor",
            Role = UserRole.Employee
        });
        await context.SaveChangesAsync();

        var storage = new InMemoryDocumentStorage();
        var service = new DocumentService(context, storage, new TestDocumentScanStatusService());

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.UploadDocumentAsync(
            userId: 11,
            title: "danger",
            category: "project",
            description: "bad upload",
            projectId: null,
            tags: "temp",
            fileName: "virus.exe",
            contentType: "application/x-msdownload",
            stream: new MemoryStream(new byte[] { 1, 2, 3 }))); 

        Assert.Contains("unsupported", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    private sealed class InMemoryDocumentStorage : IFileStorageService
    {
        public Task<string> SaveAsync(Stream fileStream, string fileName, string containerName)
        {
            var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + "_" + fileName);
            using var output = File.Create(path);
            fileStream.CopyTo(output);
            return Task.FromResult(path);
        }

        public Task<Stream> OpenReadAsync(string storedPath)
        {
            return Task.FromResult<Stream>(File.OpenRead(storedPath));
        }

        public Task DeleteAsync(string storedPath)
        {
            if (File.Exists(storedPath))
            {
                File.Delete(storedPath);
            }

            return Task.CompletedTask;
        }
    }

    private sealed class TestDocumentScanStatusService : IDocumentScanStatusService
    {
        public Task QueueScanAsync(Document document, string storedPath)
        {
            document.Status = DocumentStatus.PendingScan;
            document.SecurityScanStatus = SecurityScanStatus.Pending;
            return Task.CompletedTask;
        }

        public Task UpdateScanStatusAsync(Document document, SecurityScanStatus status, string? summary = null)
        {
            document.SecurityScanStatus = status;
            document.ScanCompletedAtUtc = DateTime.UtcNow;
            document.ScanResultSummary = summary;

            if (status == SecurityScanStatus.Clean)
            {
                document.Status = DocumentStatus.Approved;
            }

            return Task.CompletedTask;
        }
    }
}
