using ContosoDashboard.Data;
using ContosoDashboard.Models;
using Microsoft.EntityFrameworkCore;

namespace ContosoDashboard.Services;

public interface IDocumentService
{
    Task<Document> UploadDocumentAsync(
        int userId,
        string title,
        string category,
        string description,
        int? projectId,
        string tags,
        string fileName,
        string contentType,
        Stream stream);

    Task<List<Document>> GetUserDocumentsAsync(int userId);
}

public class DocumentService : IDocumentService
{
    private const long MaxFileSizeBytes = 25 * 1024 * 1024;

    private readonly ApplicationDbContext _context;
    private readonly IFileStorageService _fileStorageService;
    private readonly IDocumentScanStatusService _scanStatusService;

    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf",
        ".doc",
        ".docx",
        ".xls",
        ".xlsx",
        ".ppt",
        ".pptx",
        ".png",
        ".jpg",
        ".jpeg",
        ".gif",
        ".txt",
        ".csv"
    };

    public DocumentService(
        ApplicationDbContext context,
        IFileStorageService fileStorageService,
        IDocumentScanStatusService scanStatusService)
    {
        _context = context;
        _fileStorageService = fileStorageService;
        _scanStatusService = scanStatusService;
    }

    public async Task<Document> UploadDocumentAsync(
        int userId,
        string title,
        string category,
        string description,
        int? projectId,
        string tags,
        string fileName,
        string contentType,
        Stream stream)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new InvalidOperationException("Document title is required.");
        }

        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new InvalidOperationException("Document file name is required.");
        }

        var extension = Path.GetExtension(fileName);
        if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException("Unsupported file type. Please upload a valid PDF, Office, image, or text document.");
        }

        if (stream.Length <= 0 || stream.Length > MaxFileSizeBytes)
        {
            throw new InvalidOperationException("The file exceeds the 25 MB maximum upload size.");
        }

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException("The uploading user could not be found.");
        }

        if (projectId.HasValue)
        {
            var projectExists = await _context.Projects.AnyAsync(p => p.ProjectId == projectId.Value);
            if (!projectExists)
            {
                throw new InvalidOperationException("Project not found.");
            }
        }

        var storedPath = await _fileStorageService.SaveAsync(stream, fileName, "documents");

        var document = new Document
        {
            Title = title.Trim(),
            Category = category.Trim(),
            Description = description?.Trim(),
            FileName = Path.GetFileName(fileName),
            ContentType = contentType,
            StoragePath = storedPath,
            FileSizeBytes = stream.Length,
            UploadedByUserId = userId,
            ProjectId = projectId,
            Tags = tags ?? string.Empty,
            UploadedDate = DateTime.UtcNow,
            LastModifiedDate = DateTime.UtcNow,
            Status = DocumentStatus.PendingScan,
            SecurityScanStatus = SecurityScanStatus.Pending
        };

        _context.Documents.Add(document);
        await _context.SaveChangesAsync();

        await _scanStatusService.QueueScanAsync(document, storedPath);
        await _context.SaveChangesAsync();

        return document;
    }

    public async Task<List<Document>> GetUserDocumentsAsync(int userId)
    {
        return await _context.Documents
            .Where(d => d.UploadedByUserId == userId)
            .OrderByDescending(d => d.UploadedDate)
            .ToListAsync();
    }
}
