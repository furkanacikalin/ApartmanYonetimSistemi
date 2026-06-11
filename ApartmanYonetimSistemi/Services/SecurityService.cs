using System;
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
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Parola boş bırakılamaz.", nameof(password));

        // Kriptografik olarak güvenli rastgele salt (tuz) üretimi
        byte[] saltBytes = RandomNumberGenerator.GetBytes(SaltSize);
        string salt = Convert.ToBase64String(saltBytes);

        // PBKDF2 ile şifre hashleme
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
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrEmpty(storedHash) || string.IsNullOrEmpty(storedSalt))
                return false;

            byte[] saltBytes = Convert.FromBase64String(storedSalt);

            // Gelen parolayı mevcut tuzla tekrar hashliyoruz
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: IterationCount,
                numBytesRequested: HashSize));

            // Klasik "==" yerine "FixedTimeEquals" kullanarak Timing Attack (Zamanlama Saldırıları) riskini sıfırlıyoruz.
            // Bu metot, karakterler ne olursa olsun her zaman tüm diziyi tarar ve işlem süresini sabit tutar.
            byte[] hashedBytes = Convert.FromBase64String(hashed);
            byte[] storedHashBytes = Convert.FromBase64String(storedHash);

            return CryptographicOperations.FixedTimeEquals(hashedBytes, storedHashBytes);
        }
        catch
        {
            // Hata durumunda siber saldırgana bilgi sızdırmamak adına sessizce false dönüyoruz
            return false;
        }
    }
}