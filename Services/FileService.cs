namespace StudentAdminPortal.Services;

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _env;

    public FileService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<(string relativePath, string originalName, long size, string contentType)> SaveFileAsync(IFormFile file, string subFolder)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File không hợp lệ.");
        }

        string originalName = Path.GetFileName(file.FileName);
        string extension = Path.GetExtension(originalName);
        string uniqueFileName = $"{Guid.NewGuid():N}_{DateTime.UtcNow:yyyyMMddHHmmss}{extension}";

        string uploadFolder = Path.Combine(_env.WebRootPath, "uploads", subFolder);
        if (!Directory.Exists(uploadFolder))
        {
            Directory.CreateDirectory(uploadFolder);
        }

        string fullPath = Path.Combine(uploadFolder, uniqueFileName);
        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        string relativePath = $"/uploads/{subFolder}/{uniqueFileName}";
        return (relativePath, originalName, file.Length, file.ContentType);
    }

    public void DeleteFile(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath)) return;

        string cleanRelative = relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        string fullPath = Path.Combine(_env.WebRootPath, cleanRelative);

        if (File.Exists(fullPath))
        {
            try
            {
                File.Delete(fullPath);
            }
            catch
            {
                // Ignore delete errors in demo
            }
        }
    }
}
