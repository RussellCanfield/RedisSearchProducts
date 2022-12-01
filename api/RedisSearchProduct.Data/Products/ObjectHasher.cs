using System;
using System.Security.Cryptography;
using System.Text.Json;

namespace RedisSearchProduct.Data.Products
{
	public static class ObjectHasher
	{
		private static readonly SHA256 sha256;

		static ObjectHasher()
		{
			sha256 = SHA256.Create();
		}

		public static string Hash<T>(T obj)
		{
			using MemoryStream memoryStream = new MemoryStream();
			JsonSerializer.SerializeAsync(memoryStream, obj);
			return Convert.ToBase64String(sha256.ComputeHash(memoryStream.ToArray()));
		}

        public static string Hash(string value)
        {
            return Convert.ToBase64String(sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(value)));
        }
    }
}

