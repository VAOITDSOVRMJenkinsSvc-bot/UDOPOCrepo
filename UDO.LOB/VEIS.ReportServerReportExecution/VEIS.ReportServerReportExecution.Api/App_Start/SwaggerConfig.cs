using System.Web.Http;
using Swashbuckle.Application;
using WebActivatorEx;
using VEIS.ReportServerReportExecution.Api;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace VEIS.ReportServerReportExecution.Api
{
	public class SwaggerConfig
	{
		public static void Register()
		{
			var thisAssembly = typeof(SwaggerConfig).Assembly; 
			GlobalConfiguration.Configuration
				.EnableSwagger(c => 
					{
						c.SingleApiVersion("v1", "VEIS.ReportServerReportWebService");
					});
		}
	}
}
