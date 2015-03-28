using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace DodgeDynasty.Shared.Security
{
	public class EncryptUtil
	{
		public static bool VerifyPassword(string inputPassword, byte[] storedPassword, byte[] storedSalt)
		{
			var encryptedPassword = EncryptPassword(inputPassword, storedSalt);
			return encryptedPassword.PasswordHash.SequenceEqual(storedPassword);
		}

		public static PasswordInfo EncryptPasswordForStorage(string password)
		{
			return EncryptPassword(password, GenerateSalt());
		}

		private static PasswordInfo EncryptPassword(string password, byte[] salt)
		{
			byte[] pBytes = Encoding.Unicode.GetBytes(password);
			Rfc2898DeriveBytes pGen = new Rfc2898DeriveBytes(pBytes, salt, 8745);

			RijndaelManaged rijndael = new RijndaelManaged();

			rijndael.Key = pGen.GetBytes(32);
			rijndael.IV = pGen.GetBytes(16);

			byte[] pHash;

			using (MemoryStream ms = new MemoryStream())
			{
				using (CryptoStream cs = new CryptoStream(ms, rijndael.CreateEncryptor(), CryptoStreamMode.Write))
				{
					cs.Write(pBytes, 0, pBytes.Length);
				}
				pHash = ms.ToArray();
			}

			return new PasswordInfo() { PasswordHash = pHash, Salt = salt };
		}

		private static byte[] GenerateSalt()
		{
			byte[] salt = new byte[8];
			RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
			rng.GetBytes(salt);
			return salt;
		}
	}
}