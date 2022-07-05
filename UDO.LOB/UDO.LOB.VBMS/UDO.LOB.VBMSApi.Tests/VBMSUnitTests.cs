using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UDO.LOB.VBMSApi.Tests
{
    using UDO.LOB.VBMS.Messages;
    using UDO.LOB.VBMS.Processors;
    using System.IO;
    using UDO.LOB.Core;

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void UploadDocTest()
        {

            UDOVBMSUploadDocumentAsyncRequest request = new UDOVBMSUploadDocumentAsyncRequest();
            request.Debug = true;
            request.MessageId = Guid.NewGuid().ToString();
            request.udo_filename = "Informal Claim Letter 001.pdf";
            request.udo_filenumber = "666551414";
            request.udo_doctypeid = Guid.Empty;
            request.udo_base64filecontents = GetFileContent();
            request.udo_subject = "UDO Informal Claim Letter";
            request.OrganizationName = "CRMEDEV";
            request.UserId = new Guid("0a90c445-88a2-e511-9418-00155d14f3d0");

            UDOVBMSUploadDocumentAsyncProcessor processor = new UDOVBMSUploadDocumentAsyncProcessor();
            string requestBody = JsonHelper.Serialize<UDOVBMSUploadDocumentAsyncRequest>(request);
            processor.Execute(request);
        }


        private string GetFileContent()
        {
            byte[] fileContent;

            string fileName = "Informal Claim Letter 05052019.pdf";
            string filePathFolder = @"D:\VBMS\";
            string filePath = $"{filePathFolder}{fileName}";

            //using (StreamReader sr = File.OpenText(filePath))
            //{
            //    string s;
            //    while ((s = sr.ReadLine()) != null)
            //    {
            //        Console.WriteLine(s);
            //    }

                
            //}

            fileContent = File.ReadAllBytes(filePath);

            return Convert.ToBase64String(fileContent);

            //return fileContent;
        }
    }
}
