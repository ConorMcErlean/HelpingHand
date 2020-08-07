using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System;


/* 
A simple password hashing tool built with guidance from:
https://www.devtrends.co.uk/blog/hashing-encryption-and-random-in-asp.net-core

*/
namespace TagTool.Data.Security
{
    public static class Hasher
    {
        /* Increaseing the iterations improves security but will 
            make the process more computationally expensive. */
        const int ITERATIONS = 10000;

        public static string CalculateHash(string input)
        {
            // Salting will prevent password being guessed with a dictionary attack
            var salt = GenerateSalt(16);

            var bytes = KeyDerivation.Pbkdf2
            (input, salt, KeyDerivationPrf.HMACSHA512, ITERATIONS, 16);

            var Hashed = Convert.ToBase64String(salt) + ":" +
                Convert.ToBase64String(bytes);
            return Hashed;
        }// CalculateHash

        private static byte[] GenerateSalt(int length)
        {
            var salt = new byte [length];
            using (var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(salt);
            }
            return salt;
        }// GenerateSalt

        public static bool ValidateHash(string hash, string input)
        {
            try 
            {
                var parts = hash.Split(':');
                var salt  = Convert.FromBase64String(parts[0]);
                var bytes = KeyDerivation.Pbkdf2(input, salt, 
                    KeyDerivationPrf.HMACSHA512, ITERATIONS, 16);
                return parts[1].Equals(Convert.ToBase64String(bytes));
            }
            catch
            {
                return false;
            }
        }// ValidateHash
    }// Hasher
}