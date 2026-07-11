namespace BlogCSharp.Services
{
    public interface IFileService
    {
        Task<string> UploadImage(IFormFile file);
        void DeleteFile(string filePath);
    }
}