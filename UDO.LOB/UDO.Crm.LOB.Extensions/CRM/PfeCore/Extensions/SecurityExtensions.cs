using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;


namespace Microsoft.Pfe.Xrm
{
    public static class SecurityExtensions
    {
        //TODO: Replace this value with your own entropy key
        private static byte[] additionalEntropy = Encoding.Unicode.GetBytes("replacewithyourentropykey");

        /// <summary>
        /// Converts a SecureString object to a plain-text string value
        /// </summary>
        /// <param name="value">The SecureString value</param>
        /// <returns>A plain-text string version of the SecureString value</returns>
        /// <remarks>
        /// Allocs unmanaged memory in process of converting to string. 
        /// Calls Marshal.ZeroFreeGlobalAllocUnicode to free unmanaged memory space for the ptr struct in finally { }
        /// </remarks>
        public static string ToUnsecureString(this SecureString value)
        {
            if (value == null)
                throw new ArgumentNullException("value", "Cannot convert null value.ToUnsecureString()");

            IntPtr ssAsIntPtr = IntPtr.Zero;

            try
            {
                // Fortify recommendation: copy data as unicode character array to a buffer in unmanaged space
                ssAsIntPtr = Marshal.SecureStringToGlobalAllocUnicode(value);

                char[] charArray = new char[value.Length];
                for (Int32 i = 0; i < value.Length; i++)
                {
                    // Multiply by 2 because Unicode chars are 2 bytes wide
                    char ch = (char)Marshal.ReadInt16(ssAsIntPtr, i * 2);
                    charArray[i] = ch;
                }

                return charArray.ToString();
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(ssAsIntPtr);
            }
        }

        /// <summary>
        /// Encrypts a SecureString value and returns it as a base64 string
        /// </summary>
        /// <param name="value">The SecureString value</param>
        /// <returns>An encrypted string value</returns>
        /// <remarks>
        /// Leverages DPAPI and assumes CurrentUser data protection scope
        /// Assumes that SecureString should not be disposed
        /// </remarks>
        /// <exception cref="CryptographicException">
        /// This method calls ProtectedData.Protect(). Callers of this method should handle potential cryptographic exceptions.
        /// </exception>
        public static string ToEncryptedString(this SecureString value)
        {
            if (value == null)
                throw new ArgumentNullException("value", "Cannot encrypt a null SecureString value");

            var encryptedValue = ProtectedData.Protect(Encoding.Unicode.GetBytes(value.ToUnsecureString()), SecurityExtensions.additionalEntropy, DataProtectionScope.CurrentUser);

            return Convert.ToBase64String(encryptedValue);
        }

        /// <summary>
        /// Converts a plain-text string value to a SecureString object
        /// </summary>
        /// <param name="value">The string value</param>
        /// <returns>A SecureString representation of the string value</returns>
        public static SecureString ToSecureString(this string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentNullException("value", "Cannot convert null value.ToSecureString()");
            
            var secureValue = new SecureString();

            value.ToCharArray()
                .ToList()
                .ForEach(c =>
                {
                    secureValue.AppendChar(c);
                });

            secureValue.MakeReadOnly();

            return secureValue;
        }

        /// <summary>
        /// Decrypts an encrypted string value and returns it as a SecureString
        /// </summary>
        /// <param name="value">The base64 encoded encrypted string value</param>
        /// <returns>The decrypted string value wrapped in a SecureString</returns>
        /// <remarks>
        /// Leverages DPAPI and assumes CurrentUser data protection scope
        /// </remarks>
        /// <exception cref="CryptographicException">
        /// This method calls ProtectedData.Unprotect(). Callers of this method should handle potential cryptographic exceptions.
        /// </exception>
        public static SecureString ToDecryptedSecureString(this string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("value", "Cannot decrypt a null (or empty) String value");
            }

            SecureString ss = new SecureString();

            byte[] byteArray = ProtectedData.Unprotect(Convert.FromBase64String(value), SecurityExtensions.additionalEntropy, DataProtectionScope.CurrentUser);
            char[] charArray = Encoding.Unicode.GetChars(byteArray);
            ss = charArray.ToString().ToSecureString();
            ss.MakeReadOnly();

            return ss;
        }
    }
}
