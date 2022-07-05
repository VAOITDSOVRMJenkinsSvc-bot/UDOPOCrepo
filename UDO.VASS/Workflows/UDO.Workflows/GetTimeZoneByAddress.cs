	using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace UDO.Workflows
{

    public class GetTimeZoneByAddress : CodeActivity
    {


        [Input("State")]
        public InArgument<string> stateName { get; set; }

        [Input("City")]
        public InArgument<string> cityName { get; set; }


        [Input("Zip")]
        public InArgument<string> zipCode { get; set; }

        [Output("TimeZone")]
        public OutArgument<string> timeZoneGenericName { get; set; }

        [Input("APIKey")]
        public InArgument<string> ApiKey { get; set; }


        protected override void Execute(CodeActivityContext executionContext)
        {
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();

            //Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            tracingService.Trace("Begin request");
            string statename = stateName.Get(executionContext);
            string cityname = cityName.Get(executionContext);
            string zipcode = zipCode.Get(executionContext);
            string placename = $"{cityname},{statename}";
            string bingMapsAPIKey = ApiKey.Get(executionContext);
            if (string.IsNullOrWhiteSpace(statename)&&string.IsNullOrWhiteSpace(cityname)&&string.IsNullOrWhiteSpace(zipcode))
            {
                timeZoneGenericName.Set(executionContext, "Not Available");
                return;
            }
            StringBuilder builder = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(zipcode))
            {
                builder.Append(zipcode);
                builder.Append(",");
            }
            if (!string.IsNullOrWhiteSpace(cityname) && !string.IsNullOrWhiteSpace(statename))
            {
                builder.Append(cityname);
                builder.Append(",");
            }
            if (!string.IsNullOrWhiteSpace(statename))
            {
                builder.Append(statename);
            }
           
           
            tracingService.Trace($"{placename}");
          
            string bingMapsUri = $"https://dev.virtualearth.net/REST/v1/TimeZone/?query={builder}&key={bingMapsAPIKey}";
            try
            {

                HttpClient request = new HttpClient();
                HttpResponseMessage response = request.GetAsync(bingMapsUri).GetAwaiter().GetResult();
                tracingService.Trace($"Bing api called and response recieved {response.StatusCode}");
                if (response.IsSuccessStatusCode)
                {
                    var timeZoneData = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(timeZoneData)))
                    {
                        // Deserialization from JSON  
                        DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(RootObject));
                        RootObject deserialziedData = (RootObject)deserializer.ReadObject(ms);
                        var timeZoneName = deserialziedData.resourceSets[0].resources[0].timeZoneAtLocation[0].timeZone[0].genericName;
                        var abbreviation = deserialziedData.resourceSets[0].resources[0].timeZoneAtLocation[0].timeZone[0].abbreviation;
                        string timeZone = $"{abbreviation}";
                        tracingService.Trace($"Time Zone :{abbreviation}");
                        timeZoneGenericName.Set(executionContext, timeZone);
                    }
                }
            }
          
            catch (Exception ex)
            {
                timeZoneGenericName.Set(executionContext, "Not Available");
                tracingService.Trace($"{ex.Message}");
            }


        }



        //Console.WriteLine($"Time Zone :{timeZoneName.Value} ");

    }

    [DataContract()]
    public class ConvertedTime
    {
        [IgnoreDataMember]
        public DateTime localTime { get; set; }
        [IgnoreDataMember]
        public string utcOffsetWithDst { get; set; }
        [DataMember(IsRequired = false)]
        public string timeZoneDisplayName { get; set; }
        [DataMember(IsRequired = false)]
        public string timeZoneDisplayAbbr { get; set; }

    }
    [DataContract]
    public class TimeZone
    {
        [DataMember(IsRequired = false)]
        public string genericName { get; set; }
        [DataMember(IsRequired = false)]
        public string abbreviation { get; set; }
        [DataMember(IsRequired = false)]
        public string ianaTimeZoneId { get; set; }
        [DataMember(IsRequired = false)]
        public string windowsTimeZoneId { get; set; }
        [DataMember(IsRequired = false)]
        public string utcOffset { get; set; }
        [DataMember(IsRequired = false)]
        public ConvertedTime convertedTime { get; set; }
    }
    [DataContract]
    public class TimeZoneAtLocation
    {
        [DataMember(IsRequired = false)]
        public string placeName { get; set; }
        [DataMember(IsRequired = false)]
        public List<TimeZone> timeZone { get; set; }
    }
    [DataContract(Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1", Name = "RESTTimeZone")]


    public class Resource
    {
        [DataMember(IsRequired = false)]
        public string __type { get; set; }
        [DataMember(IsRequired = false)]
        public List<TimeZoneAtLocation> timeZoneAtLocation { get; set; }
    }

    [DataContract(Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1", Name = "RESTTimeZone")]
    public class ResourceSet
    {
        [DataMember(IsRequired = false)]
        public int estimatedTotal { get; set; }
        [DataMember(IsRequired = false)]
        public List<Resource> resources { get; set; }
    }
    [DataContract]
    public class RootObject
    {
        [DataMember(IsRequired = false)]
        public string authenticationResultCode { get; set; }
        [DataMember(IsRequired = false)]
        public string brandLogoUri { get; set; }
        [DataMember(IsRequired = false)]
        public string copyright { get; set; }
        [DataMember(IsRequired = false)]
        public List<ResourceSet> resourceSets { get; set; }
        [DataMember(IsRequired = false)]
        public int statusCode { get; set; }
        [DataMember(IsRequired = false)]
        public string statusDescription { get; set; }
        [DataMember(IsRequired = false)]
        public string traceId { get; set; }
    }
}

