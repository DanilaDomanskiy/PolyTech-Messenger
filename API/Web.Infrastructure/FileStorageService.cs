using Microsoft.Extensions.Configuration;
using Web.Application.Interfaces;

namespace Web.Infrastructure
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _folderPath;

        public FileStorageService(IConfiguration configuration)
        {
            _folderPath = configuration["FileStorageSettings:UploadFolderPath"];
        }

        public async Task SaveFileAsync(byte[] file, string fileName)
        {
            var path = Path.Combine(_folderPath, fileName);
            using var memoryStream = new MemoryStream(file);
            using var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            await memoryStream.CopyToAsync(fileStream);
        }

        public void DeleteFile(string fileName)
        {
            var path = Path.Combine(_folderPath, fileName);

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public async Task<byte[]?> GetFile(string fileName)
        {
            var path = Path.Combine(_folderPath, fileName);

            if (!File.Exists(path))
            {
                return null;
            }

            return await File.ReadAllBytesAsync(path);
        }
    }
}