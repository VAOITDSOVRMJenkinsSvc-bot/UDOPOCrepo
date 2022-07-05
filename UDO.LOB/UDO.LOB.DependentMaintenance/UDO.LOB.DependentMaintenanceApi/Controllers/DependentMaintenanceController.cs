using System;
using System.Net;
using System.Web.Http;
using System.Web.Mvc;
using Swashbuckle.Swagger.Annotations;
using UDO.LOB.Core;
using UDO.LOB.Extensions.Logging;
using UDO.LOB.DependentMaintenance.Messages;
using UDO.LOB.DependentMaintenance.Processors;
using System.Diagnostics;

namespace UDO.LOB.DependentMaintenance.Controllers
{
	public class DependentMaintenanceController : ApiController
	{

		//One for each other processor 
		//Void return because it does it processing and records annotations
		#region Reg Processors
		[SwaggerOperation("AddDependentOrchestration")]
		[SwaggerResponse(HttpStatusCode.OK)]
		[SwaggerResponse(HttpStatusCode.NotFound)]
		[System.Web.Http.ActionName("AddDependentOrchestration")]
		public void AddDependentOrchestration([FromBody] AddDependentOrchestrationRequest request)
		{
			var gwatch = Stopwatch.StartNew();
			LogHelper.LogInfo($">> Entered {this.GetType().FullName}.AddDependentOrchestration");
			try
			{
				//New Processor 
				AddDependentOrchestrationProcessor processor = new AddDependentOrchestrationProcessor();
				processor.Execute(request);
			}
			catch (Exception ex)
			{
				Trace.TraceError(ex.ToString());
				LogHelper.LogError(request.OrganizationName, request.UserId, "AddDependentOrchestration", ex.Message);
				throw ex;
			}
			finally
			{
				//LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.AddDependentOrchestration ");
				LogHelper.LogTiming(request.MessageId, request.OrganizationName, request.LogTiming, request.UserId, request.RelatedParentId, request.RelatedParentEntityName, request.RelatedParentFieldName
					, $"<< Exited {this.GetType().FullName}.createClaims", null, gwatch.ElapsedMilliseconds);
			}
		}

		#endregion

		#region Plugin Processors
		//Tested
		[System.Web.Http.HttpPost]
		[SwaggerOperation("GetDependentInfo")]
		[SwaggerResponse(HttpStatusCode.OK)]
		[SwaggerResponse(HttpStatusCode.NotFound)]
		[System.Web.Http.ActionName("GetDependentInfo")]
		public IMessageBase GetDependentInfo([FromBody] GetDependentInfoRequest request)
		{
			LogHelper.LogInfo($">> Entered {this.GetType().FullName}.GetDependentInfo");
			try
			{
				//New Processor 
				GetDependentInfoProcessor processor = new GetDependentInfoProcessor();
				return processor.Execute(request);

			}
			catch (Exception ex)
			{
				Trace.TraceError(ex.ToString());
				throw ex;
			}
			finally
			{
				LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.GetDependentInfo ");
			}
		}

		//Tested
		[System.Web.Http.HttpPost]
		[SwaggerOperation("GetMaritalInfo")]
		[SwaggerResponse(HttpStatusCode.OK)]
		[SwaggerResponse(HttpStatusCode.NotFound)]
		[System.Web.Http.ActionName("GetMaritalInfo")]
		public IMessageBase GetMaritalInfo([FromBody] GetMaritalInfoRequest request)
		{
			LogHelper.LogInfo($">> Entered {this.GetType().FullName}.GetMaritalInfo");
			try
			{
				//New Processor 
				GetMaritalInfoProcessor processor = new GetMaritalInfoProcessor();
				return processor.Execute(request);

			}
			catch (Exception ex)
			{
				Trace.TraceError(ex.ToString());
				throw ex;
			}
			finally
			{
				LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.GetMaritalInfo ");
			}
		}

		//Tested 
		[System.Web.Http.HttpPost]
		[SwaggerOperation("GetSensitivityLevel")]
		[SwaggerResponse(HttpStatusCode.OK)]
		[SwaggerResponse(HttpStatusCode.NotFound)]
		[System.Web.Http.ActionName("GetSensitivityLevel")]
		public IMessageBase GetSensitivityLevel([FromBody] GetSensitivityLevelRequest request)
		{
			LogHelper.LogInfo($">> Entered {this.GetType().FullName}.GetSensitivityLevel");
			try
			{
				//New Processor 
				GetSensitivityLevelProcessor processor = new GetSensitivityLevelProcessor();
				return processor.Execute(request);

			}
			catch (Exception ex)
			{
				Trace.TraceError(ex.ToString());
				throw ex;
			}
			finally
			{
				LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.GetSensitivityLevel ");
			}
		}

		//Tested but GetVetInfo sometimes returns null corpbdb record 
		[System.Web.Http.HttpPost]
		[SwaggerOperation("GetVeteranInfo")]
		[SwaggerResponse(HttpStatusCode.OK)]
		[SwaggerResponse(HttpStatusCode.NotFound)]
		[System.Web.Http.ActionName("GetVeteranInfo")]
		public IMessageBase GetVeteranInfo([FromBody] GetVeteranInfoRequest request)
		{
			LogHelper.LogInfo($">> Entered {this.GetType().FullName}.GetVeteranInfo");
			try
			{
				//New Processor 
				GetVeteranInfoProcessor processor = new GetVeteranInfoProcessor();
				return processor.Execute(request);

			}
			catch (Exception ex)
			{
				Trace.TraceError(ex.ToString());
				throw ex;
			}
			finally
			{
				LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.GetVeteranInfo ");
			}
		}

		[System.Web.Http.HttpPost]
		[SwaggerOperation("SearchSchoolInfo")]
		[SwaggerResponse(HttpStatusCode.OK)]
		[SwaggerResponse(HttpStatusCode.NotFound)]
		[System.Web.Http.ActionName("SearchSchoolInfo")]
		public IMessageBase SearchSchoolInfo([FromBody] SearchSchoolInfoRequest request)
		{
			LogHelper.LogInfo($">> Entered {this.GetType().FullName}.SearchSchoolInfo");
			try
			{
				//New Processor 
				SearchSchoolInfoProcessor processor = new SearchSchoolInfoProcessor();
				return processor.Execute(request);

			}
			catch (Exception ex)
			{
				Trace.TraceError(ex.ToString());
				throw ex;
			}
			finally
			{
				LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.SearchSchoolInfo ");
			}
		}

		[System.Web.Http.HttpPost]
		[SwaggerOperation("GetSchoolInfo")]
		[SwaggerResponse(HttpStatusCode.OK)]
		[SwaggerResponse(HttpStatusCode.NotFound)]
		[System.Web.Http.ActionName("GetSchoolInfo")]
		public IMessageBase GetSchoolInfo([Bind(Include = "OrganizationName,UserId,LogTiming,LogSoap,Debug,LegacyServiceHeaderInfo,mcs_fullFacilityCode")] GetSchoolInfoRequest request)
		{
			LogHelper.LogInfo($">> Entered {this.GetType().FullName}.GetSchoolInfo");
			try
			{
				//New Processor 
				GetSchoolInfoProcessor processor = new GetSchoolInfoProcessor();
				return processor.Execute(request);

			}
			catch (Exception ex)
			{
				Trace.TraceError(ex.ToString());
				throw ex;
			}
			finally
			{
				LogHelper.LogInfo($"<< Exited {this.GetType().FullName}.GetSchoolInfo ");
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

		//// GET api/values
		//[SwaggerOperation("GetAll")]
		//public IEnumerable<string> Get()
		//{
		//    return new string[] { "value1", "value2" };
		//}

		//// GET api/values/5
		//[SwaggerOperation("GetById")]
		//[SwaggerResponse(HttpStatusCode.OK)]
		//[SwaggerResponse(HttpStatusCode.NotFound)]
		//public string Get(int id)
		//{
		//    return "value";
		//}

		//// POST api/values
		//[SwaggerOperation("Create")]
		//[SwaggerResponse(HttpStatusCode.Created)]
		//public void Post([FromBody]string value)
		//{
		//}

		//// PUT api/values/5
		//[SwaggerOperation("Update")]
		//[SwaggerResponse(HttpStatusCode.OK)]
		//[SwaggerResponse(HttpStatusCode.NotFound)]
		//public void Put(int id, [FromBody]string value)
		//{
		//}

		//// DELETE api/values/5
		//[SwaggerOperation("Delete")]
		//[SwaggerResponse(HttpStatusCode.OK)]
		//[SwaggerResponse(HttpStatusCode.NotFound)]
		//public void Delete(int id)
		//{
		//}
	}
}