namespace Namiono.Common
{
    public interface ICrypto
    {
        string GetHash(string text, string key);
    }
}