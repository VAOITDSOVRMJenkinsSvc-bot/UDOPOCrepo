using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using UDO.LOB.Core;
using UDO.LOB.Extensions.Logging;

namespace UDO.LOB.Extensions
{
    public class ApiCatalogManager
    {
        public static ApiCatalog LoadApiSettings()
        {
            // Read json config file and load into api settings
            ApiCatalog apiCatalog = new ApiCatalog();

            try
            {
                string filePath = HostingEnvironment.MapPath("~/bin/App_Data/ApiCatalog.json");
                // LogHelper.LogInfo($"File Path: {filePath}");

                if (File.Exists(filePath))
                {
                    // LogHelper.LogInfo($"File Exists: {filePath}");
                    using (FileStream fs = File.OpenRead(filePath))
                    {
                        DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings()
                        {
                            KnownTypes = new Collection<Type>
                            {
                                typeof(Api),
                                typeof(ApiCatalog)
                            }
                        };

                        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ApiCatalog), settings);
                        apiCatalog = (ApiCatalog)serializer.ReadObject(fs);
                    }
                }
                else
                {
                    LogHelper.LogInfo($"File DOES NOT Exist: {filePath}");
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError("", Guid.Empty, $"#Error in LoadApiSettings: ", ex);
            }

            return apiCatalog;

        }

    }
}
