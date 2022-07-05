using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace UDO.Crm.LOB.Controllers.Notes
{
    using Swashbuckle.Swagger.Annotations;
    using VRM.Integration.Servicebus.Core;
    using VRM.Integration.UDO.Notes.Messages;
    using VRM.Integration.UDO.Notes.Processors;

    public class NotesController : ApiController
    {
        [SwaggerOperation("getNotes")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IMessageBase Get(UDORetrieveNotesRequest message)
        {
            try
            {
               // TODO: LogMessageReceipt(message);
                var processor = new UDORetrieveNotesProcessor();
                return processor.Execute((UDORetrieveNotesRequest)message);
            }
            catch (Exception ex)
            {
                // UDORetrieveNotesRequest msg = (UDORetrieveNotesRequest)message;
                // LogHelper.LogError(msg.OrganizationName, "mcs_findNotes", msg.UserId, "UDORetrieveNotesMessageHandler.HandleRequestResponse", ex);
                throw new Exception(string.Format("UDORetrieveNotesMessageHandler Error: {0}", ex.Message), ex);
            }
        }

        [SwaggerOperation("createRatings")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        // POST api/notes
        public IMessageBase Post([FromBody]UDOCreateNoteRequest message)
        {
            try
            {
                // TODO: LogMessageReceipt(message);
                var processor = new UDOCreateNoteProcessor();
                return processor.Execute((UDOCreateNoteRequest)message);
            }
            catch (Exception ex)
            {
                var msg = message;
                // LogHelper.LogError(msg.OrganizationName, "mcs_CreateNote", msg.UserId, "UDOCreateNoteMessageHandler.HandleRequestResponse", ex);
                throw new Exception(string.Format("UDOCreateNoteMessageHandler Error: {0}", ex.Message), ex);
            }
        }

        [SwaggerOperation("updateRatings")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        // PUT api/notes
        public IMessageBase Put([FromBody]UDOUpdateNoteRequest message)
        {
            try
            {
                //TODO:  LogMessageReceipt(message);
                var processor = new UDOUpdateNoteProcessor();
                return processor.Execute((UDOUpdateNoteRequest)message);
            }
            catch (Exception ex)
            {
                // UDOUpdateNoteRequest msg = (UDOUpdateNoteRequest)message;
                // LogHelper.LogError(msg.OrganizationName, msg.UserId, "UDOUpdateNoteMessageHandler.HandleRequestResponse", ex);
                throw new Exception(string.Format("UDOUpdateNoteMessageHandler Error: {0}", ex.Message), ex);
            }
        }

        [SwaggerOperation("deleteRatings")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        // DELETE api/Notes
        public IMessageBase Delete(UDODeleteNoteRequest message)
        {
            try
            {
                //TODO:  LogMessageReceipt(message);
                var processor = new UDODeleteNoteProcessor();
                return processor.Execute((UDODeleteNoteRequest)message);
            }
            catch (Exception ex)
            {
                UDODeleteNoteRequest msg = (UDODeleteNoteRequest)message;
                // LogHelper.LogError(msg.OrganizationName, msg.UserId, "UDODeleteNoteRequestMessageHandler.HandleRequestResponse", ex);
                throw new Exception(string.Format("UDODeleteNoteRequestMessageHandler Error: {0}", ex.Message), ex);
            }
        }
    }
}