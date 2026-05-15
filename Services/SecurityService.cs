using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace ApartmanYonetimSistemi.Services;

public class SecurityService
{
    private const int IterationCount = 100000; 
    private const int SaltSize = 128 / 8;     
    private const int HashSize = 256 / 8;     

    
    public (string Hash, string Salt) HashPassword(string password)
    {
        
        byte[] saltBytes = RandomNumberGenerator.GetBytes(SaltSize);
        string salt = Convert.ToBase64String(saltBytes);

        
        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: saltBytes,
            prf: KeyDerivationPrf.HMACSHA256, 
            iterationCount: IterationCount,
            numBytesRequested: HashSize));

        return (hashed, salt);
    }

    
    public bool VerifyPassword(string password, string storedHash, string storedSalt)
    {
        try
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedHash) || string.IsNullOrEmpty(storedSalt))
                return false;

            byte[] saltBytes = Convert.FromBase64String(storedSalt);

            
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: IterationCount,
                numBytesRequested: HashSize));

            return hashed == storedHash;
        }
        catch
        {
            
            return false;
        }
    }
}