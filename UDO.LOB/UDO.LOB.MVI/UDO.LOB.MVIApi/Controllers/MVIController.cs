using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Swashbuckle.Swagger.Annotations;
using UDO.LOB.Core;
using UDO.LOB.Extensions;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.MVI.Messages;
using UDO.LOB.MVI.Processors;
using UDO.LOB.PersonSearch.Processors;

//CSDev
using System.Diagnostics;

//CSDev 


namespace UDO.LOB.MVI.Controllers
{
    public class MVIController : ApiController
    {
		//CSDev Sourced from POST api/MVI 
		//One for each other processor 

		#region Reg Processors
		[SwaggerOperation("AddPerson")]
		[SwaggerResponse(HttpStatusCode.OK)]
		[SwaggerResponse(HttpStatusCode.NotFound)]
		[ActionName("AddPerson")]
		public void AddPerson([FromBody]UDOAddPersonRequest request)
		{
			LogHelper.LogInfo($">> Entered {this.GetType().FullName}.AddPerson");
			try
			{
				//New Processor 
				UDOAddPersonProcessor processor = new UDOAddPersonProcessor();
				processor.Execute(request);

			}
			catch (Exception ex)
			{
				LogHelper.LogError(request.OrganizationName, request.UserId, "AddPerson", ex.ToString());
				throw ex;
			}
			finally
			{
				LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.AddPerson ");
			}
		}

		[SwaggerOperation("BIRLSandOtherSearch")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("BIRLSandOtherSearch")]
        public IMessageBase BIRLSandOtherSearch([FromBody]UDOBIRLSandOtherSearchRequest request)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.BIRLSandOtherSearch");
            try
            {
                //TODO: LogMessageReceipt(message);
                UDOBIRLSandOtherSearchProcessor processor = new UDOBIRLSandOtherSearchProcessor();
                return processor.Execute(request);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "BIRLSandOtherSearch", ex.ToString());
                throw ex;
            }
            finally
            {
                LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.BIRLSandOtherSearch");
            }
        }

		[SwaggerOperation("CombinedPersonSearch")]
		[SwaggerResponse(HttpStatusCode.OK)]
		[SwaggerResponse(HttpStatusCode.NotFound)]
		[ActionName("CombinedPersonSearch")]
		public IMessageBase CombinedPersonSearch([FromBody]UDOCombinedPersonSearchRequest request)
		{
			LogHelper.LogInfo($">> Entered {this.GetType().FullName}.CombinedPersonSearch");
			try
			{
				//New Processor 
				UDOCombinedPersonSearchProcessor processor = new UDOCombinedPersonSearchProcessor();
				return processor.Execute(request);

			}
			catch (Exception ex)
			{
				LogHelper.LogError(request.OrganizationName, request.UserId, "CombinedPersonSearch", ex.ToString());
				throw ex;
			}
			finally
			{
				LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.CombinedPersonSearch ");
			}
		}

		[SwaggerOperation("CombinedSelectedPerson")]
		[SwaggerResponse(HttpStatusCode.OK)]
		[SwaggerResponse(HttpStatusCode.NotFound)]
		[ActionName("CombinedSelectedPerson")]
		public IMessageBase CombinedSelectedPerson([FromBody]UDOCombinedSelectedPersonRequest request)
		{
			LogHelper.LogInfo($">> Entered {this.GetType().FullName}.CombinedSelectedPerson");
			try
			{
				//New Processor 
				UDOCombinedSelectedPersonProcessor processor = new UDOCombinedSelectedPersonProcessor();
				return processor.Execute(request);

			}
			catch (Exception ex)
			{
				LogHelper.LogError(request.OrganizationName, request.UserId, "CombinedSelectedPerson", ex.ToString());
				throw ex;
			}
			finally
			{
				LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.CombinedSelectedPerson ");
			}
		}

		[SwaggerOperation("CTIPersonSearch")]
		[SwaggerResponse(HttpStatusCode.OK)]
		[SwaggerResponse(HttpStatusCode.NotFound)]
		[ActionName("CTIPersonSearch")]
		public IMessageBase CTIPersonSearch([FromBody]UDOCTIPersonSearchRequest request)
		{
			LogHelper.LogInfo($">> Entered {this.GetType().FullName}.CTIPersonSearch");
			try
			{
				//New Processor 
				UDOCTIPersonSearchProcessor processor = new UDOCTIPersonSearchProcessor();
				return processor.Execute(request);

			}
			catch (Exception ex)
			{
				LogHelper.LogError(request.OrganizationName, request.UserId, "CTIPersonSearch", ex.ToString());
				throw ex;
			}
			finally
			{
				LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.CTIPersonSearch ");
			}
		}

		[SwaggerOperation("findVeteranInfo")]
		[SwaggerResponse(HttpStatusCode.OK)]
		[SwaggerResponse(HttpStatusCode.NotFound)]
		[ActionName("findVeteranInfo")]
		public IMessageBase findVeteranInfo([FromBody]UDOfindVeteranInfoRequest request)
		{
			LogHelper.LogInfo($">> Entered {this.GetType().FullName}.findVeteranInfo");
			try
			{
				//New Processor 
				UDOfindVeteranInfoProcessor processor = new UDOfindVeteranInfoProcessor();
				return processor.Execute(request);

			}
			catch (Exception ex)
			{
				LogHelper.LogError(request.OrganizationName, request.UserId, "findVeteranInfo", ex.ToString());
				throw ex;
			}
			finally
			{
				LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.findVeteranInfo ");
			}
		}

		[SwaggerOperation("UDOgetVeteranIdentifiers")]
		[SwaggerResponse(HttpStatusCode.OK)]
		[SwaggerResponse(HttpStatusCode.NotFound)]
		[ActionName("UDOgetVeteranIdentifiers")]
		public IMessageBase UDOgetVeteranIdentifiers([FromBody]UDOgetVeteranIdentifiersRequest request)
		{
			LogHelper.LogInfo($">> Entered {this.GetType().FullName}.UDOgetVeteranIdentifiers");
			try
			{
				//New Processor 
				UDOgetVeteranIdentifiersProcessor processor = new UDOgetVeteranIdentifiersProcessor();
				return processor.Execute(request);

			}
			catch (Exception ex)
			{
				LogHelper.LogError(request.OrganizationName, request.UserId, "UDOgetVeteranIdentifiers", ex.ToString());
				throw ex;
			}
			finally
			{
				LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.UDOgetVeteranIdentifiers ");
			}
		}

		[SwaggerOperation("PersonSearch")]
		[SwaggerResponse(HttpStatusCode.OK)]
		[SwaggerResponse(HttpStatusCode.NotFound)]
		[ActionName("PersonSearch")]
		public IMessageBase PersonSearch([FromBody]UDOPersonSearchRequest request)
		{
			LogHelper.LogInfo($">> Entered {this.GetType().FullName}.PersonSearch");
			try
			{
				//New Processor 
				UDOPersonSearchProcessor processor = new UDOPersonSearchProcessor();
				return processor.Execute(request);

			}
			catch (Exception ex)
			{
				LogHelper.LogError(request.OrganizationName, request.UserId, "PersonSearch", ex.ToString());
				throw ex;
			}
			finally
			{
				LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.PersonSearch ");
			}
		}

		[SwaggerOperation("SelectedPerson")]
		[SwaggerResponse(HttpStatusCode.OK)]
		[SwaggerResponse(HttpStatusCode.NotFound)]
		[ActionName("SelectedPerson")]
		public IMessageBase SelectedPerson([FromBody]UDOSelectedPersonRequest request)
		{
			LogHelper.LogInfo($">> Entered {this.GetType().FullName}.SelectedPerson");
			try
			{
				//New Processor 
				UDOSelectedPersonProcessor processor = new UDOSelectedPersonProcessor();
				return processor.Execute(request);

			}
			catch (Exception ex)
			{
				LogHelper.LogError(request.OrganizationName, request.UserId, "SelectedPerson", ex.ToString());
				throw ex;
			}
			finally
			{
				LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.SelectedPerson ");
			}
		}

		[SwaggerOperation("SimpleFindInCrm")]
		[SwaggerResponse(HttpStatusCode.OK)]
		[SwaggerResponse(HttpStatusCode.NotFound)]
		[ActionName("SimpleFindInCrm")]
		public IMessageBase SimpleFindInCrm([FromBody] UDOSelectedPersonRequest request)
		{
			LogHelper.LogInfo($">> Entered {this.GetType().FullName}.SelectedPerson");
			try
			{
				//New Processor 
				UDOSimpleFindInCrmProcessor processor = new UDOSimpleFindInCrmProcessor();
				return processor.Execute(request);

			}
			catch (Exception ex)
			{
				LogHelper.LogError(request.OrganizationName, request.UserId, "SelectedPerson", ex.ToString());
				throw ex;
			}
			finally
			{
				LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.SelectedPerson ");
			}
		}
		#endregion

		#region Faux Processors 
		[SwaggerOperation("SecondarySearch")]
		[SwaggerResponse(HttpStatusCode.OK)]
		[SwaggerResponse(HttpStatusCode.NotFound)]
		[ActionName("SecondarySearch")]
		public IMessageBase SecondarySearch([FromBody]UDOPersonSearchRequest request)
		{
			LogHelper.LogInfo($">> Entered {this.GetType().FullName}.SecondarySearch");
			try
			{
				//New Processor 
				SecondarySearchProcessor processor = new SecondarySearchProcessor();
				return processor.Execute(request);

			}
			catch (Exception ex)
			{
				LogHelper.LogError(request.OrganizationName, request.UserId, "SecondarySearch", ex.ToString());
				throw ex;
			}
			finally
			{
				LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.SecondarySearch ");
			}
		}

		[SwaggerOperation("HandleDupCorpRecord")]
		[SwaggerResponse(HttpStatusCode.OK)]
		[SwaggerResponse(HttpStatusCode.NotFound)]
		[ActionName("HandleDupCorpRecord")]
		public IMessageBase HandleDupCorpRecord([FromBody]UDOSelectedPersonResponse selectedPersonResponse, UDOSelectedPersonRequest selectedPersonRequest, PatientPerson originalPerson)
		{
			LogHelper.LogInfo($">> Entered {this.GetType().FullName}.HandleDupCorpRecord");
			try
			{
				//New Processor 
				HandleDupCorpRecordProcessor processor = new HandleDupCorpRecordProcessor();
				return processor.Execute(selectedPersonResponse, selectedPersonRequest, originalPerson);

			}
			catch (Exception ex)
			{
				LogHelper.LogError(selectedPersonRequest.OrganizationName, selectedPersonRequest.UserId, "HandleDupCorpRecord", ex.ToString());
				throw ex;
			}
			finally
			{
				LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.HandleDupCorpRecord ");
			}
		}

		[SwaggerOperation("CombinedSecondarySearch")]
		[SwaggerResponse(HttpStatusCode.OK)]
		[SwaggerResponse(HttpStatusCode.NotFound)]
		[ActionName("CombinedSecondarySearch")]
		public IMessageBase CombinedSecondarySearch([FromBody]UDOCombinedPersonSearchRequest request)
		{
			LogHelper.LogInfo($">> Entered {this.GetType().FullName}.CombinedSecondarySearch");
			try
			{
				//New Processor 
				CombinedSecondarySearchProcessor processor = new CombinedSecondarySearchProcessor();
				return processor.Execute(request);

			}
			catch (Exception ex)
			{
				LogHelper.LogError(request.OrganizationName, request.UserId, "CombinedSecondarySearch", ex.ToString());
				throw ex;
			}
			finally
			{
				LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.CombinedSecondarySearch ");
			}
		}

        [SwaggerOperation("OpenIDProofAsync")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [ActionName("OpenIDProofAsync")]
        public IMessageBase OpenIDProofAsync([FromBody]UDOOpenIDProofAsyncRequest request)
        {
            LogHelper.LogInfo($">> Entered {this.GetType().FullName}.OpenIDProofAsync");
            try
            {
                //New Processor 
                UDOOpenIDProofRequestProcessor processor = new UDOOpenIDProofRequestProcessor();
                return processor.Execute(request);

            }
            catch (Exception ex)
            {
                LogHelper.LogError(request.OrganizationName, request.UserId, "OpenIDProofAsync", ex.ToString());
                throw ex;
            }
            finally
            {
                LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.OpenIDProofAsync ");
            }
        }
		#endregion

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
