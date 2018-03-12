using System;
using System.Text;
#if UNITY_WSA || UNITY_WSA_8_1 || UNITY_WSA_10_0
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
#else
using System.Security.Cryptography;
#endif
namespace PubNubMessaging.Core
{
    #region "PubnubCrypto"
    public class PubnubCrypto: PubnubCryptoBase
    {
        public PubnubCrypto (string cipher_key)
            : base (cipher_key)
        {
        }
#if UNITY_WSA || UNITY_WSA_8_1 || UNITY_WSA_10_0
        protected override string ComputeHashRaw(string input)
        {
            Sha256Digest algorithm = new Sha256Digest();
            Byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            Byte[] bufferBytes = new byte[algorithm.GetDigestSize()];
            algorithm.BlockUpdate(inputBytes, 0, inputBytes.Length);
            algorithm.DoFinal(bufferBytes, 0);
            return BitConverter.ToString(bufferBytes);
        }

        protected override string EncryptOrDecrypt(bool type, string plainStr)
        {
            //Demo params
            string keyString = GetEncryptionKey();

            string input = plainStr;
            byte[] inputBytes;
            byte[] iv = System.Text.Encoding.UTF8.GetBytes("0123456789012345");
            byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(keyString);

            //Set up
            AesEngine engine = new AesEngine();
            CbcBlockCipher blockCipher = new CbcBlockCipher(engine); //CBC
            PaddedBufferedBlockCipher cipher = new PaddedBufferedBlockCipher(blockCipher); //Default scheme is PKCS5/PKCS7
            KeyParameter keyParam = new KeyParameter(keyBytes);
            ParametersWithIV keyParamWithIV = new ParametersWithIV(keyParam, iv, 0, iv.Length);

            if (type)
            {
                // Encrypt
                input = EncodeNonAsciiCharacters(input);
                inputBytes = Encoding.UTF8.GetBytes(input);
                cipher.Init(true, keyParamWithIV);
                byte[] outputBytes = new byte[cipher.GetOutputSize(inputBytes.Length)];
                int length = cipher.ProcessBytes(inputBytes, outputBytes, 0);
                cipher.DoFinal(outputBytes, length); //Do the final block
                string encryptedInput = Convert.ToBase64String(outputBytes);

                return encryptedInput;
            }
            else
            {
                try
                {
                    //Decrypt
                    inputBytes = Convert.FromBase64CharArray(input.ToCharArray(), 0, input.Length);
                    cipher.Init(false, keyParamWithIV);
                    byte[] encryptedBytes = new byte[cipher.GetOutputSize(inputBytes.Length)];
                    int encryptLength = cipher.ProcessBytes(inputBytes, encryptedBytes, 0);
                    int numOfOutputBytes = cipher.DoFinal(encryptedBytes, encryptLength); //Do the final block
                                                                                          //string actualInput = Encoding.UTF8.GetString(encryptedBytes, 0, encryptedBytes.Length);
                    int len = Array.IndexOf(encryptedBytes, (byte)0);
                    len = (len == -1) ? encryptedBytes.Length : len;
                    string actualInput = Encoding.UTF8.GetString(encryptedBytes, 0, len);
                    return actualInput;

                }
                catch (Exception ex)
                {
#if (ENABLE_PUBNUB_LOGGING)
                    LoggingMethod.WriteToLog(string.Format("DateTime {0} Decrypt Error. {1}", DateTime.Now.ToString(), ex.ToString()), LoggingMethod.LevelVerbose);
#endif
                    throw ex;
                    //LoggingMethod.WriteToLog(string.Format("DateTime {0} Decrypt Error. {1}", DateTime.Now.ToString(), ex.ToString()), LoggingMethod.LevelVerbose);
                    //return "**DECRYPT ERROR**";
                }
            }
        }
#else
        protected override string ComputeHashRaw (string input)
        {
#if (SILVERLIGHT || WINDOWS_PHONE || MONOTOUCH || __IOS__ || MONODROID || __ANDROID__ || UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_IOS || UNITY_ANDROID || UNITY_5 || UNITY_WEBGL)
            HashAlgorithm algorithm = new System.Security.Cryptography.SHA256Managed ();
#else
            HashAlgorithm algorithm = new SHA256CryptoServiceProvider ();
#endif

            Byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes (input);
            Byte[] hashedBytes = algorithm.ComputeHash (inputBytes);
            return BitConverter.ToString (hashedBytes);
        }

        protected override string EncryptOrDecrypt (bool type, string plainStr)
        {
            {
                RijndaelManaged aesEncryption = new RijndaelManaged ();
                aesEncryption.KeySize = 256;
                aesEncryption.BlockSize = 128;
                //Mode CBC
                aesEncryption.Mode = CipherMode.CBC;
                //padding
                aesEncryption.Padding = PaddingMode.PKCS7;
                //get ASCII bytes of the string
                aesEncryption.IV = System.Text.Encoding.ASCII.GetBytes ("0123456789012345");
                aesEncryption.Key = System.Text.Encoding.ASCII.GetBytes (GetEncryptionKey ());

                if (type) {
                    ICryptoTransform crypto = aesEncryption.CreateEncryptor ();
                    plainStr = EncodeNonAsciiCharacters (plainStr);
                    byte[] plainText = Encoding.UTF8.GetBytes (plainStr);

                    //encrypt
                    byte[] cipherText = crypto.TransformFinalBlock (plainText, 0, plainText.Length);
                    return Convert.ToBase64String (cipherText);
                } else {
                    try {
                        ICryptoTransform decrypto = aesEncryption.CreateDecryptor ();
                        //decode
                        byte[] decryptedBytes = Convert.FromBase64CharArray (plainStr.ToCharArray (), 0, plainStr.Length);

                        //decrypt
                        string decrypted = System.Text.Encoding.UTF8.GetString (decrypto.TransformFinalBlock (decryptedBytes, 0, decryptedBytes.Length));

                        return decrypted;
                    } catch (Exception ex) {
#if (ENABLE_PUBNUB_LOGGING)
                        LoggingMethod.WriteToLog (string.Format ("DateTime {0} Decrypt Error. {1}", DateTime.Now.ToString (), ex.ToString ()), 
                            LoggingMethod.LevelInfo);
#endif
                        throw ex;
                    }
                }
            }
        }

#endif

    }
    #endregion
}

