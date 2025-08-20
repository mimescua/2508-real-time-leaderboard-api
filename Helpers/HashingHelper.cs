using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;

namespace SafeProjectName.Helpers;

public static class HashingHelper
{
	public static string HashPassword(string password)
	{
		byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
		byte[] salt = GenerateSalt(16);

		Argon2id argon2 = new Argon2id(passwordBytes)
		{
			Salt = salt,
			DegreeOfParallelism = 8,
			MemorySize = 65536,
			Iterations = 4
		};

		byte[] hashBytes = argon2.GetBytes(32);

		// Combine salt + hash and encode as Base64 for storage
		byte[] combined = new byte[salt.Length + hashBytes.Length];
		Buffer.BlockCopy(salt, 0, combined, 0, salt.Length);
		Buffer.BlockCopy(hashBytes, 0, combined, salt.Length, hashBytes.Length);

		return Convert.ToBase64String(combined);
	}

	public static bool VerifyPassword(string password, string storedHash)
	{
		byte[] combined = Convert.FromBase64String(storedHash);

		// Extract salt and hash
		byte[] salt = new byte[16];
		byte[] storedHashBytes = new byte[32];
		Buffer.BlockCopy(combined, 0, salt, 0, salt.Length);
		Buffer.BlockCopy(combined, salt.Length, storedHashBytes, 0, storedHashBytes.Length);

		// Hash the input password with the same salt
		byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
		Argon2id argon2 = new Argon2id(passwordBytes)
		{
			Salt = salt,
			DegreeOfParallelism = 8,
			MemorySize = 65536,
			Iterations = 4
		};

		byte[] computedHash = argon2.GetBytes(32);

		// Compare hashes securely
		return CryptographicOperations.FixedTimeEquals(computedHash, storedHashBytes);
	}

	private static byte[] GenerateSalt(int length)
	{
		byte[] salt = new byte[length];
		using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
		{
			rng.GetBytes(salt);
		}
		return salt;
	}
}
