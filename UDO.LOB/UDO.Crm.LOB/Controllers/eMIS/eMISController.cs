namespace UDO.Crm.LOB.Controllers.eMIS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using VRM.Integration.Servicebus.Core;
    using VRM.Integration.UDO.eMIS.Messages;
    using VRM.Integration.UDO.eMIS.Processors;

    public class eMISController : ApiController
    {
        // GET api/eMIS
        public IMessageBase Get([FromBody]UDOgetMilitaryInformationRequest message)
        {
            try
            {
                //TODO: LogMessageReceipt(message);
                var processor = new UDOgetMilitaryInformationProcessor();
                return processor.Execute((UDOgetMilitaryInformationRequest)message);
            }
            catch (Exception ex)
            {
                UDOgetMilitaryInformationRequest msg = (UDOgetMilitaryInformationRequest)message;
                // LogHelper.LogError(msg.OrganizationName, msg.UserId, "UDOgetMilitaryInformationMessageHandler.HandleRequestResponse", ex);
                throw new Exception(string.Format("UDOgetMilitaryInformationMessageHandler Error: {0}", ex.Message), ex);
            }
        }
    }
}