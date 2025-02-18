using System.Security.Cryptography;
using System.Text;

public static class HashUtility
{
    // Generates a SHA-256 hash for a given projected integer value
    public static string GenerateHash(ProjectedInt32Value value)
    {
        int actualValue = value.GetValue();
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(actualValue.ToString()));
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                // Convert byte to hex string
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }

    // Verifies if the hash matches the ProjectedInt32Value
    public static bool VerifyHash(ProjectedInt32Value value, string hash)
    {
        string computedHash = GenerateHash(value);
        return computedHash.Equals(hash, StringComparison.OrdinalIgnoreCase);
    }
}
