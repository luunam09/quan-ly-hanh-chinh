namespace StudentAdminPortal.Services;

public interface IFileService
{
    Task<(string relativePath, string originalName, long size, string contentType)> SaveFileAsync(IFormFile file, string subFolder);
    void DeleteFile(string relativePath);
}
