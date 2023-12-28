using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace McQLib.Core
{
    /// <summary>
    /// 데이터를 암호화/복호화하는 기능이 구현된 클래스입니다.
    /// </summary>
    public static class Security
    {
        private static readonly string SecretKey = "McScience Inc. 5F Factory Q Series";
        /// <summary>
        /// 원본 데이터를 지정된 암호화 키를 사용하여 암호화합니다.
        /// </summary>
        /// <param name="source">원본 데이터를 포함하는 문자열입니다.</param>
        /// <param name="publicKey">암호화 키를 포함하는 문자열입니다.</param>
        /// <returns>암호화된 결과를 나타내는 문자열입니다.</returns>
        public static string Encrypt( this string source, string publicKey )
        {
            string key = publicKey.encryptSecret( SecretKey );

            RijndaelManaged rijndaelCipher = new RijndaelManaged();

            try
            {
                byte[] plainText = Encoding.Unicode.GetBytes( source );
                byte[] salt = Encoding.ASCII.GetBytes( key.Length.ToString() );
                PasswordDeriveBytes secretKey = new PasswordDeriveBytes( key, salt );

                ICryptoTransform encryptor = rijndaelCipher.CreateEncryptor( secretKey.GetBytes( 32 ), secretKey.GetBytes( 16 ) );
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream( memoryStream, encryptor, CryptoStreamMode.Write );
                cryptoStream.Write( plainText, 0, plainText.Length );
                cryptoStream.FlushFinalBlock();
                byte[] cipherBytes = memoryStream.ToArray();

                memoryStream.Close();
                cryptoStream.Close();

                return Convert.ToBase64String( cipherBytes );
            }
            catch
            {
                throw new QException( QExceptionType.IO_ENCRYPT_FAILED_ERROR );
            }
        }
        private static string encryptSecret( this string source, string publicKey )
        {
            string key = publicKey;

            RijndaelManaged rijndaelCipher = new RijndaelManaged();

            byte[] plainText = Encoding.Unicode.GetBytes( source );
            byte[] salt = Encoding.ASCII.GetBytes( key.Length.ToString() );
            PasswordDeriveBytes secretKey = new PasswordDeriveBytes( key, salt );

            ICryptoTransform encryptor = rijndaelCipher.CreateEncryptor( secretKey.GetBytes( 32 ), secretKey.GetBytes( 16 ) );
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream( memoryStream, encryptor, CryptoStreamMode.Write );
            cryptoStream.Write( plainText, 0, plainText.Length );
            cryptoStream.FlushFinalBlock();
            byte[] cipherBytes = memoryStream.ToArray();

            memoryStream.Close();
            cryptoStream.Close();

            return Convert.ToBase64String( cipherBytes );
        }
        /// <summary>
        /// 암호화 데이터를 지정된 복호화 키를 사용하여 복호화합니다.
        /// </summary>
        /// <param name="source">암호화 데이터를 포함하는 문자열입니다.</param>
        /// <param name="publicKey">복호화 키를 포함하는 문자열입니다.</param>
        /// <returns>복호화된 결과를 나타내는 문자열입니다.</returns>
        public static string Decrypt( this string source, string publicKey )
        {
            string key = publicKey.encryptSecret( SecretKey );

            RijndaelManaged rijndaelCipher = new RijndaelManaged();

            try
            {
                byte[] encryptedData = Convert.FromBase64String( source );
                byte[] salt = Encoding.ASCII.GetBytes( key.Length.ToString() );

                PasswordDeriveBytes secretKey = new PasswordDeriveBytes( key, salt );

                ICryptoTransform decryptor = rijndaelCipher.CreateDecryptor( secretKey.GetBytes( 32 ), secretKey.GetBytes( 16 ) );
                MemoryStream memoryStream = new MemoryStream( encryptedData );
                CryptoStream cryptoStream = new CryptoStream( memoryStream, decryptor, CryptoStreamMode.Read );

                byte[] plainText = new byte[encryptedData.Length];
                int decryptedCount = cryptoStream.Read( plainText, 0, plainText.Length );
                memoryStream.Close();
                cryptoStream.Close();

                return Encoding.Unicode.GetString( plainText, 0, decryptedCount );
            }
            catch
            {
                throw new QException( QExceptionType.IO_DECRYPT_FAILED_ERROR );
            }
        }
    }
}
