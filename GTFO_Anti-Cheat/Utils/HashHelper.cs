using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Hikaria.GTFO_Anti_Cheat.Utils
{
    public static class HashHelper
    {
        public static string GetHashString(this object obj, HashType hashType)
        {
            return HashBytes(Object2Bytes(obj), hashType);
        }

        private static string HashBytes(byte[] buf, HashType hashType)
        {
            byte[] hashBytes = HashData(buf, hashType);
            return ByteArrayToHexString(hashBytes);
        }

        public static string HashFile(string fileName, HashType hashType)
        {
            if (!File.Exists(fileName))
            {
                return string.Empty;
            }

            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            byte[] hashBytes = HashData(fs, hashType);
            fs.Close();
            return ByteArrayToHexString(hashBytes);
        }

        private static byte[] HashData(Stream stream, HashType hashType)
        {
            System.Security.Cryptography.HashAlgorithm algorithm;

            if (hashType == HashType.SHA_1)
            {
                algorithm = System.Security.Cryptography.SHA1.Create();
            }
            else
            {
                algorithm = System.Security.Cryptography.MD5.Create();
            }

            return algorithm.ComputeHash(stream);
        }

        private static byte[] HashData(byte[] buf, HashType hashType)
        {
            System.Security.Cryptography.HashAlgorithm algorithm;

            if (hashType == HashType.SHA_1)
            {
                algorithm = System.Security.Cryptography.SHA1.Create();
            }
            else
            {
                algorithm = System.Security.Cryptography.MD5.Create();
            }

            return algorithm.ComputeHash(buf);
        }

        private static string ByteArrayToHexString(byte[] buf)
        {
            return BitConverter.ToString(buf).Replace("-", "");
        }

        [Flags]
        public enum HashType
        {
            SHA_1,
            MD5
        }

        public static byte[] Object2Bytes(object obj)
        {
            byte[] buff;
            using (MemoryStream ms = new MemoryStream())
            {
                IFormatter iFormatter = new BinaryFormatter();
                iFormatter.Serialize(ms, obj);
                buff = ms.GetBuffer();
            }
            return buff;
        }

        public static object Bytes2Object(byte[] buff)
        {
            object obj;
            using (MemoryStream ms = new MemoryStream(buff))
            {
                IFormatter iFormatter = new BinaryFormatter();
                obj = iFormatter.Deserialize(ms);
            }
            return obj;
        }

    }
}
