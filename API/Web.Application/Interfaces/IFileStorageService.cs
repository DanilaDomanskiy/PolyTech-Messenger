namespace Web.Application.Interfaces
{
    public interface IFileStorageService
    {
        void DeleteFile(string fileName);

        Task<byte[]?> GetFile(string fileName);

        Task SaveFileAsync(byte[] file, string filePath);
    }
}