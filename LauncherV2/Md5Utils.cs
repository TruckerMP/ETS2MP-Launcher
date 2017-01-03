using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace LauncherV2
{
	internal class Md5Utils
	{
		public static bool FileNotMatched(string file, string fileMd5)
		{
			bool flag = File.Exists(file);
			bool result;
			if (flag)
			{
				bool flag2 = Md5Utils.GetMd5HashFromFile(file) == fileMd5;
				if (flag2)
				{
					result = false;
					return result;
				}
				File.Delete(file);
			}
			result = true;
			return result;
		}

		public static string GetMd5HashFromFile(string fileName)
		{
			string result;
			using (FileStream fileStream = File.OpenRead(fileName))
			{
				MD5 mD = new MD5CryptoServiceProvider();
				StringBuilder stringBuilder = new StringBuilder();
				byte[] array = mD.ComputeHash(fileStream);
				for (int i = 0; i < array.Length; i++)
				{
					byte b = array[i];
					stringBuilder.Append(b.ToString("x2"));
				}
				result = stringBuilder.ToString();
			}
			return result;
		}
	}
}
