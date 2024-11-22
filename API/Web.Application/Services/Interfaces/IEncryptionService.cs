namespace Web.Application.Services.Interfaces
{
    public interface IEncryptionService
    {
        string Decrypt(string cipherText);

        string Encrypt(string plainText);
    }
}