using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using VEIS.Core.Wcf;
using VEIS.Core.Messages;
using VEIS.Messages.ReportServerReportExecution;
using VEIS.ReportServerReportExecution.Api.Processors;
namespace VEIS.ReportServerReportExecution.UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        ////<http://vrm-crm/UDOSRO/userdefined/edit.aspx?etc=8&id=%7b2BA81618-D5C2-E411-80CD-00155D5564CC%7d>
        ////private static string _crmeOrg = "UDOSRO";
        private static Guid _UDOUserId = Guid.Parse("2BA81618-D5C2-E411-80CD-00155D5564CC");

        //<https://internalcrm.crmo.dev.crm.vrm.vba.va.gov/VRMUDOConfig/userdefined/edit.aspx?etc=8&id=%7b2BA81618-D5C2-E411-80CD-00155D5564CC%7d>

        private static string _UDOOrg = "UDODEV";//MUDOConfig";
                 [TestMethod]
        public void TestLRLoadReport()
        {
            try
            {
                SoapLog.Current.ClearLog();
                SoapLog.Current.Active = true;

                VEISLRLoadReportRequest request = new VEISLRLoadReportRequest()
                {
                    OrganizationName = _UDOOrg,
                    UserId = _UDOUserId,
                    LegacyServiceHeaderInfo = new LegacyHeaderInfo
                    {
                        ApplicationName = "CRMUD",
                        ClientMachine = "10.24.20.103",
                        LoginName = "281TUSER06",
                        StationNumber = "328"
                        // ApplicationName = "CRMUD",
                        //ClientMachine = "10.0.0.1",
                        //LoginName = "vacogrossj",
                        // LoginName = "281TUSER06",
                        //Password = "ANDREA11984#1",
                        // StationNumber = "317"
                    },
                    LogSoap = true,
                    LogTiming = true
                };

                VEISLRLoadReportGetDataProcessor processor = new VEISLRLoadReportGetDataProcessor();
                VEISLRLoadReportResponse response = (VEISLRLoadReportResponse)processor.Execute<VEISLRLoadReportResponse>(request);

                Assert.IsNotNull(response);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
        [TestMethod]
        public void TestrdrRender()
        {
            try
            {
                SoapLog.Current.ClearLog();
                SoapLog.Current.Active = true;

                VEISrdrRenderRequest request = new VEISrdrRenderRequest()
                {
                    OrganizationName = _UDOOrg,
                    UserId = _UDOUserId,
                    LegacyServiceHeaderInfo = new LegacyHeaderInfo
                    {
                        ApplicationName = "CRMUD",
                        ClientMachine = "10.24.20.103",
                        LoginName = "281TUSER06",
                        StationNumber = "328"
                        // ApplicationName = "CRMUD",
                        //ClientMachine = "10.0.0.1",
                        //LoginName = "vacogrossj",
                        // LoginName = "281TUSER06",
                        //Password = "ANDREA11984#1",
                        // StationNumber = "317"
                    },
                    LogSoap = true,
                    LogTiming = true
                };

                VEISrdrRenderGetDataProcessor processor = new VEISrdrRenderGetDataProcessor();
                VEISrdrRenderResponse response = (VEISrdrRenderResponse)processor.Execute<VEISrdrRenderResponse>(request);

                Assert.IsNotNull(response);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
        [TestMethod]
        public void TestSEPSetExecutionParameters()
        {
            try
            {
                SoapLog.Current.ClearLog();
                SoapLog.Current.Active = true;

                VEISSEPSetExecutionParametersRequest request = new VEISSEPSetExecutionParametersRequest()
                {
                    OrganizationName = _UDOOrg,
                    UserId = _UDOUserId,
                    LegacyServiceHeaderInfo = new LegacyHeaderInfo
                    {
                        ApplicationName = "CRMUD",
                        ClientMachine = "10.24.20.103",
                        LoginName = "281TUSER06",
                        StationNumber = "328"
                        // ApplicationName = "CRMUD",
                        //ClientMachine = "10.0.0.1",
                        //LoginName = "vacogrossj",
                        // LoginName = "281TUSER06",
                        //Password = "ANDREA11984#1",
                        // StationNumber = "317"
                    },
                    LogSoap = true,
                    LogTiming = true
                };

                VEISSEPSetExecutionParametersGetDataProcessor processor = new VEISSEPSetExecutionParametersGetDataProcessor();
                VEISSEPSetExecutionParametersResponse response = (VEISSEPSetExecutionParametersResponse)processor.Execute<VEISSEPSetExecutionParametersResponse>(request);

                Assert.IsNotNull(response);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

    }
}