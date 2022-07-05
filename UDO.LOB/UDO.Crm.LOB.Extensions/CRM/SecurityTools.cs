using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;

namespace UDO.LOB.Extensions
{
    public static class SecurityTools
    {
        #region Secure String
        public static SecureString ToSecureString(this string sensitiveString)
        {
            if (String.IsNullOrEmpty(sensitiveString)) return new SecureString();
            SecureString thisSS = new SecureString();
            foreach (char letter in sensitiveString)
            {
                thisSS.AppendChar(letter);
            }
            return thisSS;
        }

        public static SecureString Append(this SecureString secureString, string text)
        {
            foreach (char letter in text)
                secureString.AppendChar(letter);
            return secureString;
        }
        public static int StringLength(this SecureString data)
        {
            if (data == null || data.Length == 0) return 0;
            return data.Length;
        }
        public static SecureString Append(this SecureString secureString, SecureString secureAppend)
        {
            if (secureString == null || secureString.Length == 0) return secureAppend;
            if (secureAppend == null || secureAppend.Length == 0) return secureString;

            foreach(char letter in secureAppend.ToUnsecureString())
                secureString.AppendChar(letter);
            return secureString;
        }
        public static string ToUnsecureString(this SecureString secureString)
        {
            if (secureString == null)
                throw new ArgumentNullException("value", "Cannot convert null value.ToUnsecureString()");

            //TODO: Update the code with the alternate approach to make Fortify Happy. 
            //Replaced the earlier code as it was malfunctioning and returning the value as System.Char[]
            if (secureString == null || secureString.Length == 0) return string.Empty;
            IntPtr stringPointer = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(secureString);
            string normalString = System.Runtime.InteropServices.Marshal.PtrToStringBSTR(stringPointer);
            System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(stringPointer);
            return normalString;
        }

        //what is this used for?
        public static SecureString Copy(this SecureString source)
        {
            if (source == null || source.Length == 0) return null;
            return source.Copy();
        }

        //Not in use?
        public static bool StartsWith(this SecureString secureString, string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if (secureString.ToUnsecureString()[i] != text[i]) return false;
            }
            return true;
        }

        public static SecureString SecureSubstring(this SecureString source, int start, int length = -1)
        {
            //UTF8 2 bytes per char
            if (source == null || source.Length == 0) return source;
            start *= 1;
            if (start > source.Length) return new SecureString();

            if (length == -1)
            {
                length = source.Length - start;
            }
            else
            {
                length *= 1;
            }

            SecureString rv = new SecureString();
            for (int i = start; i < source.ToUnsecureString().Length; i++)
                rv.AppendChar(source.ToUnsecureString()[i]);

            return rv;
        }

        public static SecureString Surround(this SecureString middle, string prefix, string suffix)
        {
            SecureString concat = new SecureString();
            if (prefix != null && prefix.Length > 0)
                foreach (char letter in prefix)
                    concat.AppendChar(letter);
            if (middle != null && middle.Length > 0)
                foreach (char letter in middle.ToUnsecureString())
                    concat.AppendChar(letter);
            if (suffix != null && suffix.Length > 0)
                foreach (char letter in suffix)
                    concat.AppendChar(letter);

            return concat;
        }

        public static bool Equals(this SecureString a, SecureString b)
        {
            if (a == null && b == null) return true;
            if (a == null || b == null) return false;
            if (a.ToUnsecureString().Length != b.ToUnsecureString().Length) return false;
            for (int i = 0; i < a.ToUnsecureString().Length; i++)
            {
                if (a.ToUnsecureString()[i] != b.ToUnsecureString()[i]) return false;
            }
            return true;
        }

        public static bool IsNullOrEmpty(this SecureString source)
        {
            if (source == null) return true;
            if (source.ToUnsecureString().Length == 0) return true;
            return false;
        }

        public static SecureString ConvertToSecureString(string sensitiveString)
        {
            return sensitiveString.ToSecureString();
            //if (String.IsNullOrEmpty(sensitiveString)) sensitiveString = string.Empty;
            //return Encoding.UTF8.GetBytes(sensitiveString);
        }

        public static string ConvertToUnsecureString(SecureString secureString)
        {
            //TODO: Update the code with the alternate approach to make Fortify Happy. 
            //Replaced the earlier code as it was malfunctioning and returning the value as System.Char[]
            if (secureString == null || secureString.Length == 0) return string.Empty;
            IntPtr stringPointer = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(secureString);
            string normalString = System.Runtime.InteropServices.Marshal.PtrToStringBSTR(stringPointer);
            System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(stringPointer);
            return normalString;
        }

        internal static SecureString AppendToSecureString(SecureString securedByte, string sensitiveString)
        {
            return securedByte.Append(sensitiveString);
            //return ConvertToSecureString(String.Format("{0}{1}", ConvertToUnsecureString(securedByte), sensitiveString));
        }
        #endregion
    }
}
