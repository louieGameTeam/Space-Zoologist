using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto;
using System.IO;
using System;


public class Encryption : MonoBehaviour
{
    public string Encrypt(string id) {
        //Create a UnicodeEncoder to convert between byte array and string.
            UnicodeEncoding ByteConverter = new UnicodeEncoding();

            //Create byte arrays to hold original, encrypted, and decrypted data.
            byte[] dataToEncrypt = ByteConverter.GetBytes(id);
            byte[] encryptedData;

            RSACryptoServiceProvider publicRSA = ImportPublicKey(System.IO.File.ReadAllText("./Assets/Resources/Backend/publicKey.txt"));
            encryptedData = RSAEncrypt(dataToEncrypt, publicRSA.ExportParameters(false), false);
            return Convert.ToBase64String(encryptedData);
    }

    /// <summary>
    /// Import OpenSSH PEM public key string into MS RSACryptoServiceProvider
    /// </summary>
    /// <param name="pem"></param>
    /// <returns></returns>
    public static RSACryptoServiceProvider ImportPublicKey(string pem) {
        PemReader pr = new PemReader(new StringReader(pem));
        AsymmetricKeyParameter publicKey = (AsymmetricKeyParameter)pr.ReadObject();
        RSAParameters rsaParams = DotNetUtilities.ToRSAParameters((RsaKeyParameters)publicKey);

        RSACryptoServiceProvider csp = new RSACryptoServiceProvider();// cspParams);
        csp.ImportParameters(rsaParams);
        return csp;
    }

    public static byte[] RSAEncrypt(byte[] DataToEncrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
    {
        try
        {
            byte[] encryptedData;
            //Create a new instance of RSACryptoServiceProvider.
            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
            {

                //Import the RSA Key information. This only needs
                //toinclude the public key information.
                RSA.ImportParameters(RSAKeyInfo);

                //Encrypt the passed byte array and specify OAEP padding.  
                //OAEP padding is only available on Microsoft Windows XP or
                //later.  
                encryptedData = RSA.Encrypt(DataToEncrypt, DoOAEPPadding);
            }
            return encryptedData;
        }
        //Catch and display a CryptographicException  
        //to the console.
        catch (CryptographicException e)
        {
            Debug.Log(e.Message);

            return null;
        }
    }   
}
