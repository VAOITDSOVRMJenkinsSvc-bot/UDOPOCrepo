using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using UDO.LOB.Extensions;

namespace UDO.LOB.eMIS.Processors
{
    public class UDORankHelper
    {
        private const string settingskey = "udo_rankcode";
        private List<Entity> _ranks;

        // TODO: Caching to be handled
        private static MemoryCache _cache = new MemoryCache(settingskey);
        private static T AddOrGetFromCache<T>(string key, Func<T> init)
        {
            var newValue = new Lazy<T>(init);
            var now = DateTime.Now;

            var policy = new CacheItemPolicy()
            {
                Priority = CacheItemPriority.NotRemovable
            };

            var oldValue = _cache.AddOrGetExisting(key, newValue, policy) as Lazy<T>;
            try
            {
                return (oldValue ?? newValue).Value;
            }
            catch
            {
                _cache.Remove(key);
                throw;
            }
        }
        

        public UDORankHelper(string OrgName)
        {
            _ranks = AddOrGetFromCache<List<Entity>>(settingskey + "_" + OrgName, InitializeRanks);
        }

        public static string FindRank(string orgName, string branchcode, string paygrade, string payplan, string rankcode)
        {
            var helper = new UDORankHelper(orgName);
            var rank = helper.FindRank(branchcode,paygrade,payplan, rankcode);
            return rank;
        }

        public string FindRank(string branchcode, string paygrade, string payplan, string rankcode)
        {
            paygrade = paygrade.TrimStart('0');
            rankcode = rankcode.Replace('0', 'O') ?? string.Empty;

            if (String.IsNullOrEmpty(rankcode)) return string.Empty;

            Entity match = null;

            if (!string.IsNullOrEmpty(branchcode) && !string.IsNullOrEmpty(paygrade))
            {
                match = _ranks.FirstOrDefault((e) =>
                    String.Equals(e.GetAttributeValue<string>("udo_emis_branchcode"), branchcode) &&
                    String.Equals(e.GetAttributeValue<string>("udo_emis_paygradecode"), paygrade) &&
                    String.Equals(e.GetAttributeValue<string>("udo_emis_payplancode"), payplan) &&
                    String.Equals(e.GetAttributeValue<string>("udo_emis_rankcode"), rankcode)
                    );
            }

            if (match == null)
            {
                // Fallback level 1
                if (!string.IsNullOrEmpty(branchcode))
                {

                    match = _ranks.FirstOrDefault((e) =>
                        String.Equals(e.GetAttributeValue<string>("udo_emis_branchcode"), branchcode) &&
                        String.Equals(e.GetAttributeValue<string>("udo_emis_payplancode"), payplan) &&
                        String.Equals(e.GetAttributeValue<string>("udo_emis_rankcode"), rankcode)
                        );
                }

                if (match == null)
                {
                    // Fallback level 2
                    if (!string.IsNullOrEmpty(branchcode))
                    {
                        match = _ranks.FirstOrDefault((e) =>
                            String.Equals(e.GetAttributeValue<string>("udo_emis_branchcode"), branchcode) &&
                            String.Equals(e.GetAttributeValue<string>("udo_emis_rankcode"), rankcode)
                            );
                    }

                    if (match == null)
                    {
                        // Fallback level 3
                        match = _ranks.FirstOrDefault((e) =>
                            String.Equals(e.GetAttributeValue<string>("udo_emis_rankcode"), rankcode)
                            );

                        if (match == null)
                        {
                            // Fallback level 4 - no match, return rankcode.
                            return rankcode;
                        }
                    }
                }
            }

            // match is not null
            return match.GetAttributeValue<string>("udo_rank");
        }

        private List<Entity> InitializeRanks()
        {
            var fetch = "<fetch><entity name='udo_emisranks'><attribute name='udo_rank' /><order attribute='udo_emis_branchcode' descending='false' /><order attribute='udo_emis_payplancode' descending='false' /><order attribute='udo_emis_paygradecode' descending='false' /><order attribute='udo_emis_rankcode' descending='false' /><order attribute='udo_rank' descending='false' /><filter type='and'><condition attribute='statecode' operator='eq' value='0' /></filter><attribute name='udo_emis_rankcode' /><attribute name='udo_emis_payplancode' /><attribute name='udo_emis_paygradecode' /><attribute name='udo_emis_branchcode' /><attribute name='udo_emisranksid' /></entity></fetch>";

            var fe = new FetchExpression(fetch);
            EntityCollection results;

            using (CrmServiceClient OrgService = ConnectionCache.GetProxy())
            {
                results = OrgService.RetrieveMultiple(fe);
            }

            if (results.Entities.Count == 0) return new List<Entity>();
            return results.Entities.ToList();
        }
    }
}
