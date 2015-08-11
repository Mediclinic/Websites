using System;
using System.Data;
using System.Security.Cryptography;
using System.IO;
using System.Text;


public class SimpleAES
{
    // DO NOT CHANGE THESE KEYS
    private static byte[] invoices_key             = { 208, 123, 217, 19, 11, 24, 26, 85, 45, 114, 184, 27, 162, 37, 112, 222, 209, 241, 24, 175, 144, 173, 53, 196, 29, 24, 26, 17, 218, 131, 236, 53 };
    private static byte[] invoices_vector          = { 146, 64, 191, 111, 23, 3, 113, 119, 231, 121, 221, 112, 79, 32, 114, 156 };
    private static byte[] email_unsubscribe_key    = { 203, 62, 17, 128, 131, 236, 54, 99, 217, 19, 11, 24, 37, 85, 45, 14, 184, 127, 162, 37, 144, 173, 153, 97, 129, 24, 112, 222, 219, 241, 24, 175 };
    private static byte[] email_unsubscribe_vector = { 32, 115, 176, 46, 164, 91, 21, 83, 2, 131, 119, 231, 11, 221, 112, 79 };


    private ICryptoTransform encryptor, decryptor;
    private UTF8Encoding encoder;

    public enum KeyType { Invoices, EmailUnsubscribe }
    private byte[] key    = null;
    private byte[] vector = null;






    public SimpleAES(KeyType keyType)
    {
        SetKeys(keyType);

        RijndaelManaged rm = new RijndaelManaged();
        encryptor = rm.CreateEncryptor(key, vector);
        decryptor = rm.CreateDecryptor(key, vector);
        encoder = new UTF8Encoding();
    }
    private void SetKeys(KeyType keyType)
    {
        if (keyType == KeyType.Invoices)
        {
            this.key = SimpleAES.invoices_key;
            this.vector = SimpleAES.invoices_vector;
        }
        else if (keyType == KeyType.EmailUnsubscribe)
        {
            this.key = SimpleAES.email_unsubscribe_key;
            this.vector = SimpleAES.email_unsubscribe_vector;
        }
        else
            throw new Exception("Unknown encryption type");
    }


    public static string Encrypt(KeyType keyType, string unencrypted)
    {
        SimpleAES aes = new SimpleAES(keyType);
        return aes._Encrypt(unencrypted);
    }
    public static string Decrypt(KeyType keyType, string encrypted)
    {
        SimpleAES aes = new SimpleAES(keyType);
        try
        {
            return aes._Decrypt(encrypted);
        }
        catch(Exception)
        {
            return null;
        }
    }


    protected string _Encrypt(string unencrypted)
    {
        return Convert.ToBase64String(Encrypt(encoder.GetBytes(unencrypted)));
    }

    protected string _Decrypt(string encrypted)
    {
        return encoder.GetString(Decrypt(Convert.FromBase64String(encrypted)));
    }

    protected byte[] Encrypt(byte[] buffer)
    {
        return Transform(buffer, encryptor);
    }

    protected byte[] Decrypt(byte[] buffer)
    {
        return Transform(buffer, decryptor);
    }

    protected byte[] Transform(byte[] buffer, ICryptoTransform transform)
    {
        MemoryStream stream = new MemoryStream();
        using (CryptoStream cs = new CryptoStream(stream, transform, CryptoStreamMode.Write))
        {
            cs.Write(buffer, 0, buffer.Length);
        }
        return stream.ToArray();
    }
}

/*
public class SimpleAES2
{
    // Change these keys
    private byte[] Key = { 232, 123, 217, 19, 11, 24, 26, 85, 45, 114, 184, 27, 162, 37, 112, 222, 209, 241, 24, 175, 144, 173, 53, 196, 29, 24, 26, 17, 218, 131, 53, 209 };
    private byte[] Vector = { 114, 146, 64, 191, 111, 23, 3, 113, 119, 231, 121, 252, 112, 79, 32, 114 };
    private ICryptoTransform EncryptorTransform, DecryptorTransform;
    private UTF8Encoding UTFEncoder;

    public SimpleAES2()
    {
        RijndaelManaged rm = new RijndaelManaged();
        EncryptorTransform = rm.CreateEncryptor(this.Key, this.Vector);
        DecryptorTransform = rm.CreateDecryptor(this.Key, this.Vector);
        UTFEncoder = new UTF8Encoding();
    }


    public static string Encrypt(string unencrypted)
    {
        SimpleAES2 aes = new SimpleAES2();
        return aes.EncryptToString(unencrypted);
    }
    public static string Decrypt(string encrypted)
    {
        SimpleAES2 aes = new SimpleAES2();
        try
        {
            return aes.DecryptString(encrypted);
        }
        catch (Exception)
        {
            return null;
        }
    }



    /// -------------- Two Utility Methods (not used but may be useful) -----------
    /// Generates an encryption key.
    static public byte[] GenerateEncryptionKey()
    {
        //Generate a Key.
        RijndaelManaged rm = new RijndaelManaged();
        rm.GenerateKey();
        return rm.Key;
    }

    /// Generates a unique encryption vector
    static public byte[] GenerateEncryptionVector()
    {
        //Generate a Vector
        RijndaelManaged rm = new RijndaelManaged();
        rm.GenerateIV();
        return rm.IV;
    }


    /// ----------- The commonly used methods ------------------------------    
    /// Encrypt some text and return a string suitable for passing in a URL.
    public string EncryptToString(string TextValue)
    {
        return ByteArrToString(EncryptToBytes(TextValue));
    }

    /// Encrypt some text and return an encrypted byte array.
    public byte[] EncryptToBytes(string TextValue)
    {
        //Translates our text value into a byte array.
        Byte[] bytes = UTFEncoder.GetBytes(TextValue);

        //Used to stream the data in and out of the CryptoStream.
        MemoryStream memoryStream = new MemoryStream();

        //
        // We will have to write the unencrypted bytes to the stream,
        // then read the encrypted result back from the stream.
        //
        #region Write the decrypted value to the encryption stream
        CryptoStream cs = new CryptoStream(memoryStream, EncryptorTransform, CryptoStreamMode.Write);
        cs.Write(bytes, 0, bytes.Length);
        cs.FlushFinalBlock();
        #endregion

        #region Read encrypted value back out of the stream
        memoryStream.Position = 0;
        byte[] encrypted = new byte[memoryStream.Length];
        memoryStream.Read(encrypted, 0, encrypted.Length);
        #endregion

        //Clean up.
        cs.Close();
        memoryStream.Close();

        return encrypted;
    }

    /// The other side: Decryption methods
    public string DecryptString(string EncryptedString)
    {
        return DecryptFromBytes(StrToByteArray(EncryptedString));
    }

    /// Decryption when working with byte arrays.    
    public string DecryptFromBytes(byte[] EncryptedValue)
    {
        #region Write the encrypted value to the decryption stream
        MemoryStream encryptedStream = new MemoryStream();
        CryptoStream decryptStream = new CryptoStream(encryptedStream, DecryptorTransform, CryptoStreamMode.Write);
        decryptStream.Write(EncryptedValue, 0, EncryptedValue.Length);
        decryptStream.FlushFinalBlock();
        #endregion

        #region Read the decrypted value from the stream.
        encryptedStream.Position = 0;
        Byte[] decryptedBytes = new Byte[encryptedStream.Length];
        encryptedStream.Read(decryptedBytes, 0, decryptedBytes.Length);
        encryptedStream.Close();
        #endregion
        return UTFEncoder.GetString(decryptedBytes);
    }

    /// Convert a string to a byte array.  NOTE: Normally we'd create a Byte Array from a string using an ASCII encoding (like so).
    //      System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
    //      return encoding.GetBytes(str);
    // However, this results in character values that cannot be passed in a URL.  So, instead, I just
    // lay out all of the byte values in a long string of numbers (three per - must pad numbers less than 100).
    public byte[] StrToByteArray(string str)
    {
        if (str.Length == 0)
            throw new Exception("Invalid string value in StrToByteArray");

        byte val;
        byte[] byteArr = new byte[str.Length / 3];
        int i = 0;
        int j = 0;
        do
        {
            val = byte.Parse(str.Substring(i, 3));
            byteArr[j++] = val;
            i += 3;
        }
        while (i < str.Length);
        return byteArr;
    }

    // Same comment as above.  Normally the conversion would use an ASCII encoding in the other direction:
    //      System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
    //      return enc.GetString(byteArr);    
    public string ByteArrToString(byte[] byteArr)
    {
        byte val;
        string tempStr = "";
        for (int i = 0; i <= byteArr.GetUpperBound(0); i++)
        {
            val = byteArr[i];
            if (val < (byte)10)
                tempStr += "00" + val.ToString();
            else if (val < (byte)100)
                tempStr += "0" + val.ToString();
            else
                tempStr += val.ToString();
        }
        return tempStr;
    }
}

public class SimpleAES3
{

    public static string Encrypt(string clearText)
    {
        string EncryptionKey = "MAKV2SPBNI99212";
        byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(clearBytes, 0, clearBytes.Length);
                    cs.Close();
                }
                clearText = Convert.ToBase64String(ms.ToArray());
            }
        }
        return clearText;
    }

    public static string Decrypt(string cipherText)
    {
        string EncryptionKey = "MAKV2SPBNI99212";
        cipherText = cipherText.Replace(" ", "+");
        byte[] cipherBytes = Convert.FromBase64String(cipherText);
        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(cipherBytes, 0, cipherBytes.Length);
                    cs.Close();
                }
                cipherText = Encoding.Unicode.GetString(ms.ToArray());
            }
        }
        return cipherText;
    }

}
*/