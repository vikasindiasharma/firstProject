using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace dataEncrypt.core
{
    public class DecryptionFile
    {
        public void DecryptFile(string fileEncrypted, string password,string destinationFile)
        {

            byte[] bytesToBeDecrypted = File.ReadAllBytes(fileEncrypted);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesDecrypted = CoreEncryption.AES_Decrypt(bytesToBeDecrypted, passwordBytes);

            string file = fileEncrypted;
            File.WriteAllBytes(destinationFile, bytesDecrypted);
        }
    }
}
