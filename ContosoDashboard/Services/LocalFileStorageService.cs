namespace ContosoDashboard.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _rootPath;

    public LocalFileStorageService()
    {
        _rootPath = Path.Combine(AppContext.BaseDirectory, "uploads");
        Directory.CreateDirectory(_rootPath);
    }

    public async Task<string> SaveAsync(Stream fileStream, string fileName, string containerName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("File name is required.", nameof(fileName));
        }

        var normalizedName = Path.GetFileName(fileName);
        var folderPath = Path.Combine(_rootPath, containerName);
        Directory.CreateDirectory(folderPath);

        var uniqueFileName = $"{Guid.NewGuid():N}_{normalizedName}";
        var fullPath = Path.Combine(folderPath, uniqueFileName);

        await using var outputStream = File.Create(fullPath);
        await fileStream.CopyToAsync(outputStream);

        return fullPath;
    }

    public async Task<Stream> OpenReadAsync(string storedPath)
    {
        if (string.IsNullOrWhiteSpace(storedPath) || !File.Exists(storedPath))
        {
            throw new FileNotFoundException("The requested document file was not found.", storedPath);
        }

        return await Task.FromResult<Stream>(File.OpenRead(storedPath));
    }

    public Task DeleteAsync(string storedPath)
    {
        if (!string.IsNullOrWhiteSpace(storedPath) && File.Exists(storedPath))
        {
            File.Delete(storedPath);
        }

        return Task.CompletedTask;
    }
}
