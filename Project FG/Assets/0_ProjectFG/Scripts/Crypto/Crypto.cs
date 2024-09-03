using System.Collections.Generic;
using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Diagnostics;

public class Crypto
{
    static Dictionary<string, CryptoAES> aesManages = new Dictionary<string, CryptoAES>();

    static void CreateAESManage(string base64Key, string base64IV)
    {
        CryptoAES aesManage = new CryptoAES();
        aesManage.Create(base64Key, base64IV);
        aesManages.Add(base64Key, aesManage);
    }

    public static string EncryptAESbyBase64Key(string plainText, string base64Key, string base64IV)
    {
        if (aesManages.ContainsKey(base64Key) == false)
        {
            CreateAESManage(base64Key, base64IV);
        }

        return aesManages[base64Key].Encrypt(plainText);
    }


    public static string DecryptAESByBase64Key(string encryptData, string base64Key, string base64IV, bool instead = false)
    {
        if (aesManages.ContainsKey(base64Key) == false)
        {          
            return string.Empty;
        }

        return aesManages[base64Key].Decrypt(encryptData);
    }

    public static string SimpleDecryptAES(string encryptData, string base64Key, string base64IV)
    {
        CryptoAES aesManage = new CryptoAES();
        aesManage.Create(base64Key, base64IV);
        UnityEngine.Debug.Log($"{encryptData}, base : {base64Key}, IV : {base64IV}");
        return aesManage.Decrypt(encryptData);
    }

    public static string EncodingBase64(string plainText)
    {
        Byte[] strByte = Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(strByte);
    }

    public static string DecodingBase64(string base64PlainText)
    {
        Byte[] strByte = Convert.FromBase64String(base64PlainText);
        return Encoding.UTF8.GetString(strByte);
    }
}