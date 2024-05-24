using System.Security.Cryptography;
using System.Text;

namespace JustEmpire.Utils;

public static class Hasher
{
    public static string ToSha256(string inputString)
    {
        var crypt = SHA256.Create();
        string hash = String.Empty;
        byte[] crypto = crypt.ComputeHash(Encoding.ASCII.GetBytes(inputString));
        foreach (byte theByte in crypto)
        {
            hash += theByte.ToString("x2");
        }
        
        return hash;
    }
}