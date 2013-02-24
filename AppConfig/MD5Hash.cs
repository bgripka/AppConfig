using System;
using System.EnterpriseServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace AppConfig
{
    [ComVisible(true)]
    public class MD5Hash
    {
        [ComVisible(true)]
        public string HashFile(string FileName)
        {
            var uri = new Uri(FileName);
            using (FileStream fileStream = new FileStream(uri.LocalPath, FileMode.Open, FileAccess.Read, FileShare.Read, 1024))
            {
                return HashStream(fileStream);
            }
        }

        [ComVisible(true)]
        public string HashBytes(byte[] Bytes)
        {
            return HashStream(new MemoryStream(Bytes));
        }

        [ComVisible(true)]
        public string HashString(string ValueToHash)
        {
            return HashStream(new MemoryStream(Encoding.ASCII.GetBytes(ValueToHash)));
        }

        public string HashPasswordWithSalt(string Password, string Salt)
        {
            return HashString(HashString(Password) + Salt);
        }

        public string HashStream(Stream stream)
        {
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            var md5HashBytes = md5.ComputeHash(stream);
            return Md5BytesToString(md5HashBytes);
        }

        private string Md5BytesToString(byte[] Bytes)
        {
            var rtn = new StringBuilder();
            foreach (byte b in Bytes)
                rtn.Append(b.ToString("x2", System.Globalization.CultureInfo.InvariantCulture));
            return rtn.ToString();
        }
    }
}
