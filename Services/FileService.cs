using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace BlogCSharp.Services;

public class FileService : IFileService
{
    private readonly string _uploadFolderPath;

    public FileService(IWebHostEnvironment webHostEnvironment)
    {
        _uploadFolderPath = Path.Combine(webHostEnvironment.WebRootPath, "uploads");
        if (!Directory.Exists(_uploadFolderPath))
        {
            Directory.CreateDirectory(_uploadFolderPath);
        }
    }

    public async Task<string> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("上传的文件不能为空");
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
        {
            throw new ArgumentException("只支持jpg、jpeg、png、gif格式的图片");
        }

        var maxFileSize = 5 * 1024 * 1024;
        if (file.Length > maxFileSize)
        {
            throw new ArgumentException("图片大小不能超过5MB");
        }

        var fileName = Guid.NewGuid().ToString() + extension;
        var filePath = Path.Combine(_uploadFolderPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return $"/uploads/{fileName}";
    }

    public void DeleteFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}