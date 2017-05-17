using System;
using System.Security.Cryptography;
using System.Text;

namespace ControlAstro.Utils
{
    public class EncryptionTools
    {
        public static string MD5(string inText)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] outText = md5.ComputeHash(Encoding.Default.GetBytes(inText));
            return BitConverter.ToString(outText).Replace("-", "");
        }
    }
}
