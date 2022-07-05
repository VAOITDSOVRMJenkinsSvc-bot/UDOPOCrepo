
namespace UDO.LOB.Notes.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Hosting;
    using UDO.LOB.Core;
    using UDO.LOB.Extensions;
    using UDO.LOB.Extensions.Logging;
    using UDO.LOB.Notes.Messages;
    using UDO.LOB.Notes.Processors;
    using Swashbuckle.Swagger.Annotations;
    using System.Threading.Tasks;

    public class NotesController : ApiController
    {
        // POST api/notes/getNotes
        [SwaggerOperation("getNotes")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("getNotes")]
        public IMessageBase RetrieveNotes(UDORetrieveNotesRequest request)
        {
            var gwatch = Stopwatch.StartNew();
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.getNotes for UDORetrieveNotesRequest {request.MessageId}");
            try
            {
                var retrieveNotes = Task.Factory.StartNew(() =>
                {
                    UDORetrieveNotesProcessor processor = new UDORetrieveNotesProcessor();

                    processor.Execute(request);
                });

                //Task.WaitAll(retrieveNotes);
                return new UDORetrieveNotesResponse();

            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "RetrieveNotes", ex.ToString());
                throw ex;
            }
            finally
            {
                LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.LogTiming, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName
                    , $"<< Exited {this.GetType().FullName}.RetrieveNotes", null, gwatch.ElapsedMilliseconds);
            }
        }

        // POST api/notes/createNotes
        [SwaggerOperation("createNotes")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("createNotes")]
        public IMessageBase CreateNote([FromBody] UDOCreateNoteRequest request)
        {
            var gwatch = Stopwatch.StartNew();
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.createNotes ");
            try
            {
                // TODO: LogMessageReceipt(message);
                UDOCreateNoteProcessor processor = new UDOCreateNoteProcessor();
                return processor.Execute(request);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "CreateNote", ex.ToString());
                throw ex;
            }
            finally
            {
                //LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.createNotes ");
                LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.LogTiming, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName
                    , $"<< Exited {this.GetType().FullName}.CreateNote", null, gwatch.ElapsedMilliseconds);

            }
        }

        // POST api/notes/updateNote
        [SwaggerOperation("updateNote")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("updateNote")]
        public IMessageBase UpdateNote([FromBody] UDOUpdateNoteRequest request)
        {
            var gwatch = Stopwatch.StartNew();
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.updateNote ");
            try
            {
                UDOUpdateNoteProcessor processor = new UDOUpdateNoteProcessor();
                return processor.Execute(request);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "UpdateNote", ex.ToString());
                throw ex;
            }
            finally
            {
                LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.LogTiming, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName
                    , $"<< Exited {this.GetType().FullName}.UpdateNote", null, gwatch.ElapsedMilliseconds);
            }
        }

        // POST api/notes/deleteNote
        [SwaggerOperation("deleteNote")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("deleteNote")]
        public IMessageBase UDODeleteNote(UDODeleteNoteRequest request)
        {
            var gwatch = Stopwatch.StartNew();
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.deleteNote ");
            try
            {
                UDODeleteNoteProcessor processor = new UDODeleteNoteProcessor();
                return processor.Execute(request);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "UDODeleteNote", ex.ToString());
                throw ex;
            }
            finally
            {
                LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.LogTiming, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName
                    , $"<< Exited {this.GetType().FullName}.UDODeleteNote", null, gwatch.ElapsedMilliseconds);
            }
        }
        #region Ping Controller
        [System.Web.Http.HttpGet]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/ping")]
        [SwaggerOperation("ping")]
        [SwaggerResponse(HttpStatusCode.OK)]
        public IHttpActionResult ping()
        {
            return Ok("pong");
        }

        #endregion
    }
}