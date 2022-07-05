using System.IO;

namespace VRM.Integration.Servicebus.Bgs.Services
{
    public static class ReadFile
    {
        /// <summary>
        /// Get a byte array containing the data from a file read from disk.
        /// </summary>
        /// <param name="filePath">The path used to read the file from disk.</param>
        /// <returns>Returns a byte array containing the file data from disk.</returns>
        public static byte[] GetFileData(string filePath)
        {
            byte[] buffer;

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            try
            {
                var length = (int)fileStream.Length;

                buffer = new byte[length];

                int count;

                var sum = 0;

                while ((count = fileStream.Read(buffer, sum, length - sum)) > 0)
                    sum += count;
            }
            finally
            {
                fileStream.Close();
            }

            return buffer;
        }
    }
}
