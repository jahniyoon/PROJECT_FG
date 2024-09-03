using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class CryptoAES
{
    public static int[] aesKeySize = { 128, 192, 256 };
    public static int aesIVSize = 128;

    ICryptoTransform encrypter;
    ICryptoTransform decrypter;

    public void Create(string base64Key, string base64IV)
    {
        byte[] key = Convert.FromBase64String(base64Key);
        byte[] iv = Convert.FromBase64String(base64IV);

        RijndaelManaged rijndaelManaged = new RijndaelManaged();
        rijndaelManaged.KeySize = key.Length * 8;
        rijndaelManaged.BlockSize = aesIVSize;
        rijndaelManaged.Padding = PaddingMode.PKCS7;
        rijndaelManaged.Mode = CipherMode.CBC;

        rijndaelManaged.Key = key;
        rijndaelManaged.IV = iv;

        encrypter = rijndaelManaged.CreateEncryptor();
        decrypter = rijndaelManaged.CreateDecryptor();
    }

    public string Encrypt(string plainText)
    {
        using (MemoryStream memoryStream = new MemoryStream())
        {
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encrypter, CryptoStreamMode.Write))
            {
                byte[] byteData = Encoding.UTF8.GetBytes(plainText);
                cryptoStream.Write(byteData, 0, byteData.Length);
            }

            byte[] byteCrypto = memoryStream.ToArray();
            return Convert.ToBase64String(byteCrypto);
        }
    }

    public string Decrypt(string encryptData)
    {
        using (MemoryStream memoryStream = new MemoryStream())
        {
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decrypter, CryptoStreamMode.Write))
            {
                byte[] byteEncrpt = Convert.FromBase64String(encryptData);
                cryptoStream.Write(byteEncrpt, 0, byteEncrpt.Length);
            }

            byte[] byteCrypto = memoryStream.ToArray();
            return Encoding.UTF8.GetString(byteCrypto);
        }
    }
}