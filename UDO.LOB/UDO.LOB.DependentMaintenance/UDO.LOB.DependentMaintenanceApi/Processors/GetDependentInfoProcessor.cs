using System.Collections.Generic;
using System.Linq;
using CuttingEdge.Conditions;
//using VRM.Integration.Servicebus.Bgs.Messages;
//using VRM.Integration.Servicebus.Core;
using VRM.Integration.Servicebus.Bgs.Services.ClaimantWebServiceReference;
using System;
using System.Globalization;
using UDO.LOB.DependentMaintenance;
using VRM.Integration.Servicebus.AddDependent.Util;
using UDO.LOB.DependentMaintenance.Messages;
using UDO.LOB.Core;
using UDO.LOB.Extensions.Logging;
using VEIS.Core.Wcf;

//CSdev
//namespace VRM.Integration.Servicebus.Bgs
namespace UDO.LOB.DependentMaintenance.Processors
{
    public class GetDependentInfoProcessor
    {
        public IMessageBase Execute(GetDependentInfoRequest message)
        {
			//CSDev
			//Logger.Instance.Debug("Calling GetDependentInfoProcessor");
			LogHelper.LogInfo("Calling GetDependentInfoProcessor");

			//CSDEv Adding back in Soap Logs
			VEIS.Core.Wcf.SoapLog.Current.Active = true;

            
            var response = new GetDependentInfoResponse();

            Condition.Requires(message.crme_OrganizationName, "message.crme_OrganizationName").IsNotNullOrEmpty();
            LogHelper.LogInfo("Organization Name: " + message.crme_OrganizationName);

            var crmConnection = ConnectToCrmHelper.ConnectToCrm(message.crme_OrganizationName);
            LogHelper.LogInfo("Done with ConnectToCrmHelper");
            var dependents = new List<GetDependentInfoMultipleResponse>();

            string participantId;

            if (string.IsNullOrEmpty(message.crme_ParticipantId))
            {
                Condition.Requires(message.crme_SSN, "message.crme_SSN").IsNotNullOrEmpty();

                participantId = Utils.GetVeteranParticipantId(message.crme_OrganizationName, 
                    message.crme_SSN,
                    crmConnection,
                    message.crme_UserId);
            }
            else
            {
                participantId = message.crme_ParticipantId;
            }

            Condition.Requires(participantId, "participantId").IsNotNullOrEmpty();

            shrinq6Person[] persons = Utils.GetAllRelationships(message.crme_OrganizationName,
                participantId,
                crmConnection,
                message.crme_UserId);

            //if (persons != null)
            //{
            //    foreach (shrinq6Person person in persons.Where(p => p.relationshipType != null && (p.relationshipType.Equals("Child") ||
            //                                                        p.relationshipType.Equals("Spouse"))))
            //    {
            //        var dependentInfo = Utils.MapShrink6PersonToGetDependentInfoMultipleResponse(person);
            //        dependents.Add(dependentInfo);
            //    }
            //}


            if(persons != null)
            {
                foreach (shrinq6Person person in persons.Where(p => p.relationshipType != null && (p.relationshipType.Equals("Child") ||
                                                                    p.relationshipType.Equals("Spouse"))))
                {

                    var dependentInfo = Utils.MapShrink6PersonToGetDependentInfoMultipleResponse(person);

                    if (person.relationshipType.Equals("Child"))
                    {
                        var depParticipantId = long.Parse(person.ptcpntId);
                        var childBirthInfo = Utils.GetBirthInformation(message.crme_OrganizationName, depParticipantId, crmConnection, message.crme_UserId);

                        dependentInfo.crme_ChildPlaceofBirthCity = childBirthInfo.birthCityNm;
                        dependentInfo.crme_ChildPlaceofBirthState = childBirthInfo.birthStateCd;

                        var childAddressInfo = Utils.GetAddressInformation(message.crme_OrganizationName, depParticipantId, crmConnection, message.crme_UserId);

                        dependentInfo.crme_Address1 = childAddressInfo.addrsOneTxt;
                        dependentInfo.crme_Address2 = childAddressInfo.addrsTwoTxt;
                        dependentInfo.crme_Address3 = childAddressInfo.addrsThreeTxt;
                        dependentInfo.crme_City = childAddressInfo.cityNm;
                        dependentInfo.crme_State = childAddressInfo.postalCd;
                        dependentInfo.crme_Zip = childAddressInfo.zipPrefixNbr;
                        dependentInfo.crme_Country = childAddressInfo.cntryNm;

                        if(childAddressInfo.addrsOneTxt == null)
                        {
                            dependentInfo.crme_ChildLiveswithVet = true;
                        }
                        else
                        {
                            dependentInfo.crme_ChildLiveswithVet = false;
                        }

                        //CRMUDO-2360 Commenting this piece as connecting to phonewebservice from BGS is throwing an exception
                        //var childPhoneInfo = Utils.GetPhoneInformation(message.crme_OrganizationName, depParticipantId, crmConnection, message.crme_UserId);
                        //if (childPhoneInfo != null)
                        //{ 
                        //    foreach (var phone in childPhoneInfo)
                        //    {
                        //        if (phone.compId.phoneTypeNm == "Daytime")
                        //        {
                        //            dependentInfo.crme_ChildPrimaryPhone = phone.phoneNbr.ToString();
                        //        }
                        //    }
                        //}

                    }
                    else //this means spouse
                    {
                        var depParticipantId = long.Parse(person.ptcpntId);
                        var spouseAddressInfo = Utils.GetAddressInformation(message.crme_OrganizationName, depParticipantId, crmConnection, message.crme_UserId);

                        dependentInfo.crme_Address1 = spouseAddressInfo.addrsOneTxt;
                        dependentInfo.crme_Address2 = spouseAddressInfo.addrsTwoTxt;
                        dependentInfo.crme_Address3 = spouseAddressInfo.addrsThreeTxt;
                        dependentInfo.crme_City = spouseAddressInfo.cityNm;
                        dependentInfo.crme_State = spouseAddressInfo.postalCd;
                        dependentInfo.crme_Zip = spouseAddressInfo.zipPrefixNbr;
                        dependentInfo.crme_Country = spouseAddressInfo.cntryNm;

                        if (spouseAddressInfo.addrsOneTxt == null)
                        {
                            dependentInfo.crme_LiveswithSpouse = true;
                        }
                        else
                        {
                            dependentInfo.crme_LiveswithSpouse = false;
                        }
                    }
                    dependents.Add(dependentInfo);
                }

            }
            

            response.GetDependentInfo = dependents.ToArray();

			//CSDEv Readding Soap Log
			response.SoapLog = VEIS.Core.Wcf.SoapLog.Current.Log;
			VEIS.Core.Wcf.SoapLog.Current.Active = false;
			VEIS.Core.Wcf.SoapLog.Current.ClearLog();

			return response;
        }
    }
}
