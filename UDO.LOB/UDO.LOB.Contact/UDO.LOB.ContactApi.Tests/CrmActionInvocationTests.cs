using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using UDO.LOB.Extensions;

namespace UDO.LOB.ContactApi.Tests
{
    [TestClass]
    public class CrmActionInvocationTests
    {
        private string ORGANIZATION_NAME = "XXX";
        private Guid contactId = new Guid("88CDCB40-DC66-E511-8E5B-00155D14D88F");
        private CrmServiceClient OrgServiceProxy;

        [TestInitialize]
        public void Init()
        {
            OrgServiceProxy = ConnectionCache.GetProxy();

            QueryByAttribute query = new QueryByAttribute("contact");
            query.AddAttributeValue("udo_ssn", "333313333");
            query.ColumnSet = new ColumnSet(new string[] { "contactid", "udo_edipi"});

            EntityCollection ec = OrgServiceProxy.RetrieveMultiple(query);
            if (ec.Entities.Count > 0)
            {
                contactId = ec.Entities[0].Id;
            }

        }

        [TestMethod]
        public void TestMethod1()
        {
        }

        [TestMethod]
        public void InvokeCrmAction()
        {
            string actionName = "udo_contactsetcontactebenefits";

            // Invoke Crm Action and read response

            try
            {
                OrganizationRequest req = new OrganizationRequest();
                req.RequestName = actionName;
                req.Parameters.Add("ParentEntityReference", new EntityReference("contact", contactId));
                req.Parameters.Add("EDIPI", "UNK");
                req.Parameters.Add("LogTimer", true);
                req.Parameters.Add("Debug", true);

                #region connect to CRM
                try
                {
                    if (OrgServiceProxy == null)
                    {
                        OrgServiceProxy = ConnectionCache.GetProxy();
                    }
                }
                catch (Exception connectException)
                {
                    Console.WriteLine(connectException.Message);
                }
                #endregion

                OrganizationResponse resp = OrgServiceProxy.Execute(req);

                if(resp != null)
                {
                    Console.WriteLine(resp.Results);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (OrgServiceProxy != null)
                {
                    OrgServiceProxy.Dispose();
                }
            }
        }
    }
}
