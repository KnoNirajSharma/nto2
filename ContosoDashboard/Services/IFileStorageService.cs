namespace ContosoDashboard.Services;

public interface IFileStorageService
{
    Task<string> SaveAsync(Stream fileStream, string fileName, string containerName);
    Task<Stream> OpenReadAsync(string storedPath);
    Task DeleteAsync(string storedPath);
}
