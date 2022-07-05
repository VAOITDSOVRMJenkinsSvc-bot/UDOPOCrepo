using System;
using MCSPlugins;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using UDO.Model;
using System.ServiceModel;
using System.Diagnostics;
using System.Threading;
using System.Text;
//using VRM.Integration.UDO.Common.Messages;
//using VRM.Integration.UDO.VetSnapShot.Messages;
using VRMRest;

namespace CustomActions.Plugins.Entities.VeteranSnapshot
{
    public class UDOGetVeteranSnapshotRunner : PluginRunner
    {
        #region Members

        private static readonly ColumnSet SnapShotColumn2 = new ColumnSet(
            "udo_idproofid",
            "udo_veteranid");

        private static readonly ColumnSet ContactColumns = new ColumnSet(
            "modifiedon",
            "udo_ssn",
            "contactid",
            "udo_participantid",
            "ownerid",
            "udo_filenumber",
            "udo_lastcalldatetime",
            "udo_lastcalltype",
            "udo_lastcallsubtype",
            "udo_phonenumber1",
            "udo_charactorofdischarge");

        private static readonly ColumnSet SojColumns = new ColumnSet("va_name");

        private Guid? _timeZoneId;
        private int? _localeId;
        private int? _timeZoneCode;
        private string soj = string.Empty;
        private string sojName = string.Empty;
        bool completed = false;
        //Guid _veteranId = new Guid();
        //Guid _vetSnapShot = new Guid();
        Guid _idproofid = new Guid();
        string _responseMessage = null;
        StringBuilder _logData = new StringBuilder();
        StringBuilder _statusOutput = new StringBuilder();
        StringBuilder _logStatusError = new StringBuilder();
        ITracingService tracer;
        //DateTime _modifiedDate;
        #endregion

        public UDOGetVeteranSnapshotRunner(IServiceProvider serviceProvider)
            : base(serviceProvider)
        { }

        public override Entity GetPrimaryEntity()
        {
            return PluginExecutionContext.InputParameters["Target"] as Entity;
        }

        public override Entity GetSecondaryEntity()
        {
            return PluginExecutionContext.InputParameters["Target"] as Entity;
        }

        public override string McsSettingsDebugField
        {
            get { return "udo_vetsnapshot"; }
        }

        internal void Execute()
        {
            ////Logger.WriteDebugMessage("Top of GetVeteranSnapshotRunner");
            tracer = base.TracingService;

            tracer.Trace("UDOGetVeteranSnahpShotRunner started");
            Trace("UDOGetVeteranSnahpShotRunner started");
            Logger.setMethod = "Execute";
            Stopwatch txnTimer = Stopwatch.StartNew();

            var inputParameters = PluginExecutionContext.InputParameters;

            var targetEntityRef = (EntityReference)inputParameters["ParentEntityReference"];

            try
            {
                _idproofid = targetEntityRef.Id;

                bool snapshotUpdated = false;

                ///Get the current state Veteran Snapshot record for the provided ID Proof
                
                ///Build the Fetch XML for Veteran Snapshot record
                #region Veteran Snapshot Fetch XML
                var veteranSnapShotFetchXml = String.Format(@"<fetch version='1.0' output-format='xml-platform' mapping='logical' no-lock='true' distinct='false'>
                                                                      <entity name='udo_veteransnapshot'>
                                                                        <attribute name='udo_veteransnapshotid' />
                                                                        <attribute name='createdon' />
                                                                        <attribute name='udo_ratingscomplete' />
                                                                        <attribute name='udo_paymentscompleted' />
                                                                        <attribute name='udo_contactupdatescomplete' />
                                                                        <attribute name='udo_claimscompleted' />
                                                                        <attribute name='udo_awardscompleted' />
                                                                        <attribute name='udo_appealscompleted' />
                                                                        <attribute name='udo_addresscomplete' />
                                                                        <attribute name='udo_integrationstatus' />
	                                                                    <attribute name='udo_lastname' />
                                                                        <attribute name='udo_firstname' />
                                                                        <attribute name='udo_ssn' />
                                                                        <attribute name='udo_filenumber' />
                                                                        <attribute name='udo_branchofservice' />
                                                                        <attribute name='udo_rank' />
                                                                        <attribute name='udo_soj' />
                                                                        <attribute name='udo_characterofdischarge' />
                                                                        <attribute name='udo_poa' />
                                                                        <attribute name='udo_birthdatestring' />
                                                                        <attribute name='udo_gender' />
                                                                        <attribute name='udo_dateofdeath' />
                                                                        <attribute name='udo_flashes' />
                                                                        <attribute name='udo_cfidstatus' />
                                                                        <attribute name='udo_cfidpersonorgname' />
                                                                        <attribute name='udo_sccombinedrating' />
                                                                        <attribute name='udo_nsccombineddegree' />
                                                                        <attribute name='udo_awardtype' />
                                                                        <attribute name='udo_paymentstatus' />
                                                                        <attribute name='udo_lastpaiddate' />
                                                                        <attribute name='udo_nextpaiddate' />
                                                                        <attribute name='udo_nextamount' />
                                                                        <attribute name='udo_amount' />
                                                                        <attribute name='udo_pendingclaims' />
                                                                        <attribute name='udo_pendingappeals' />
                                                                        <attribute name='udo_mailingaddress' />
                                                                        <attribute name='udo_lastcalldate' />
                                                                        <attribute name='udo_lastcalltime' />
                                                                        <attribute name='udo_type' />
                                                                        <attribute name='udo_subtype' />
                                                                        <attribute name='udo_integrationstatus' />
                                                                        <attribute name='udo_phonenumber' />
                                                                        <attribute name='udo_veteranid' />
                                                                        <filter type='and'>
                                                                          <condition attribute='udo_idproofid' operator='eq' value='{0}' />
                                                                        </filter>
                                                                      </entity>
                                                                    </fetch>", _idproofid);
                #endregion Veteran Snapshot Fetch XML

                var veteranSnapshotResponse = OrganizationService.RetrieveMultiple(new FetchExpression(veteranSnapShotFetchXml));

                udo_veteransnapshot currentSnapShot = new udo_veteransnapshot();
                udo_veteransnapshot updatedSnapShot = new udo_veteransnapshot();

                if (veteranSnapshotResponse.Entities.Count > 0)
                    currentSnapShot = veteranSnapshotResponse.Entities.FirstOrDefault().ToEntity<udo_veteransnapshot>();


                ///If Integration Status is = success this means refresh was clicked. How should we handle address update now?
                ///
                if (currentSnapShot.udo_integrationstatus == "Success")
                {
                    _logData.AppendLine("Integration Status = Success. Everything arleady Completed. Getting address updates.");


                    //Call Get address Custom Action to refersh in case of CADD
                    CallGetAddressAction(currentSnapShot.udo_VeteranID, currentSnapShot.ToEntityReference());

                    var updatedMailing = OrganizationService.Retrieve(udo_veteransnapshot.EntityLogicalName, currentSnapShot.Id, new ColumnSet("udo_mailingaddress")).ToEntity<udo_veteransnapshot>();

                    currentSnapShot.udo_mailingaddress = updatedMailing.udo_mailingaddress;

                    soj = currentSnapShot.GetAttributeValue<string>("udo_soj");

                    if(!String.IsNullOrEmpty(soj))
                    sojName = GetSoj(soj);

                    txnTimer.Stop();

                    ///Set Output Params

                    ////Logger.WriteDebugMessage(_logData.ToString());
                    tracer.Trace(_logData.ToString());
                    Trace(_logData.ToString());
                    //Logger.WriteTxnTimingMessage("VetSnapShot Custom Action", txnTimer.ElapsedMilliseconds);

                    if (!String.IsNullOrEmpty(sojName))
                        PluginExecutionContext.OutputParameters["SOJ"] = sojName;
                    else
                        PluginExecutionContext.OutputParameters["SOJ"] = soj;

                    PluginExecutionContext.OutputParameters["SnapShot"] = currentSnapShot.ToEntity<Entity>();
                    PluginExecutionContext.OutputParameters["VeteranSnapshotId"] = currentSnapShot.Id.ToString();
                    PluginExecutionContext.OutputParameters["ResponseMessage"] = _logData.ToString();    

                    //Exit
                    return;
                }

                ///check the flags for the current state to see if any data is needed
                ///if not we exit immediately 
                #region Check current state of snapshot
                bool ratingsComplete = false;
                bool awardsComplete = false;
                bool claimsComplete = false;
                bool appealsComplete = false;
                bool paymentsComplete = false;
                bool contactUpdatesComplete = false;
                bool addressComplete = false;

                //Ratings

                if (currentSnapShot.udo_ratingscomplete.HasValue)
                {
                    if (currentSnapShot.udo_ratingscomplete.Value)
                    {
                        _logData.AppendLine("Ratings Complete");
                        ratingsComplete = true;
                    }
                    else
                    {
                        _logData.AppendLine("Waiting on Ratings");
                    }
                }

                if (currentSnapShot.udo_awardscompleted != null)
                {
                    var awardInt = currentSnapShot.udo_awardscompleted.Value;
                    if (awardInt == 752280002)
                    {
                        _logData.AppendLine("Award Complete");
                        //complete, we're done
                        awardsComplete = true;
                    }
                    else
                    {
                        _logData.AppendLine("Waiting on Awards");
                    }
                    if (awardInt == 752280003)
                    {
                        //error - deal with it
                        _logStatusError.AppendLine("Awards Failed");
                        awardsComplete = true;
                        ////Logger.WriteDebugMessage("Award Error");
                    }
                }
                if (currentSnapShot.udo_claimscompleted != null)
                {
                    var claimsInt = currentSnapShot.udo_claimscompleted.Value;
                    if (claimsInt == 752280002)
                    {
                        //complete, we're done
                        _logData.AppendLine("Claim Complete");
                        claimsComplete = true;
                    }
                    else
                    {
                        _logData.AppendLine("Waiting on Claims");
                    }

                    if (claimsInt == 752280003)
                    {
                        //error - deal with it
                        _logStatusError.AppendLine("Claims Failed");
                        ////Logger.WriteDebugMessage("Claim Error");
                        claimsComplete = true;
                    }
                }
                if (currentSnapShot.udo_appealscompleted != null)
                {
                    var appealsInt = currentSnapShot.udo_appealscompleted.Value;
                    if (appealsInt == 752280002)
                    {
                        //complete, we're done
                        _logData.AppendLine("Appeals Complete");
                        appealsComplete = true;
                    }
                    else
                    {
                        _logData.AppendLine("Waiting on Appeals");
                    }
                    if (appealsInt == 752280003)
                    {
                        //error - deal with it
                        _logStatusError.AppendLine("Appeals Failed");
                        ////Logger.WriteDebugMessage("Appeals Error");
                        appealsComplete = true;
                    }
                }

                if (currentSnapShot.udo_paymentscompleted != null)
                {
                    var paymentsInt = currentSnapShot.udo_paymentscompleted.Value;
                    if (paymentsInt == 752280002)
                    {
                        //complete, we're done
                        _logData.AppendLine("Payments Complete");
                        paymentsComplete = true;
                    }
                    else
                    {
                        _logData.AppendLine("Waiting on Payments");
                    }
                    if (paymentsInt == 752280003)
                    {
                        //error - deal with it
                        _logStatusError.AppendLine("Payments Failed");
                        ////Logger.WriteDebugMessage("Appeals Error");
                        paymentsComplete = true;
                    }
                }

                if (currentSnapShot.udo_addresscomplete != null)
                {
                    if (currentSnapShot.udo_addresscomplete.Value)
                    {
                        _logData.AppendLine("Address Complete");
                        addressComplete = true;
                    }
                    else
                    {
                        _logData.AppendLine("Waiting on Address");
                    }
                }

                
                ///If Contact Data Complete = false then get contact data
                ///If contact was not modified within the last day then Get Veteran Updates

                if (currentSnapShot.udo_ContactUpdatesComplete.HasValue && currentSnapShot.udo_VeteranID != null)
                {
                    if (currentSnapShot.udo_ContactUpdatesComplete.Value)
                        contactUpdatesComplete = true;
                    else
                    {
                        ////Logger.WriteDebugMessage("Call GetContactAction");
                        tracer.Trace("Call GetContactAction");
                        Trace("Call GetContactAction");
                        //CallGetContactUpdatesAction(currentSnapShot.udo_VeteranID.Id);

                        var contact = OrganizationService.Retrieve(UDO.Model.Contact.EntityLogicalName, currentSnapShot.udo_VeteranID.Id, ContactColumns).ToEntity<UDO.Model.Contact>();


                        if (contact.udo_lastcalldatetime.HasValue)
                        {
                            RetrieveCurrentUsersSettings();
                            var response = RetrieveLocalTimeFromUTCTime(contact.udo_lastcalldatetime.Value);
                            updatedSnapShot.udo_LastCallDate = response.LocalTime.ToString("MM/dd/yyyy");
                            updatedSnapShot.udo_LastCallTime = response.LocalTime.ToShortTimeString();

                            currentSnapShot.udo_LastCallDate = response.LocalTime.ToString("MM/dd/yyyy");
                            currentSnapShot.udo_LastCallTime = response.LocalTime.ToShortTimeString();
                        }

                        ///Set fields needing to be updated on Snapshot
                        updatedSnapShot.udo_phonenumber = contact.udo_PhoneNumber1;
                        updatedSnapShot.udo_filenumber = contact.udo_FileNumber;
                        updatedSnapShot.udo_Type = contact.udo_lastcalltype;
                        updatedSnapShot.udo_SubType = contact.udo_lastcallsubtype;
                        updatedSnapShot.udo_characterofdischarge = contact.udo_CharactorofDischarge;
                        updatedSnapShot.udo_ContactUpdatesComplete = true;

                        if (!String.IsNullOrEmpty(contact.udo_ParticipantId))
                            updatedSnapShot.udo_ParticipantId = Int64.Parse(contact.udo_ParticipantId).ToString();

                        ///Set the fields in the previously returned snapshot results for returning back to UI
                        currentSnapShot.udo_Type = contact.udo_lastcalltype;
                        currentSnapShot.udo_SubType = contact.udo_lastcallsubtype;
                        currentSnapShot.udo_phonenumber = contact.udo_PhoneNumber1;

                        if (String.IsNullOrEmpty(currentSnapShot.udo_filenumber))
                            currentSnapShot.udo_filenumber = contact.udo_FileNumber;

                        //Indicate that snapshot needs to be updated
                        snapshotUpdated = true;
                        contactUpdatesComplete = true;
                    }
                }
                #endregion Check Current State of snapshot

                if ( ratingsComplete &&
                     awardsComplete &&
                     claimsComplete &&
                     appealsComplete &&
                     paymentsComplete &&
                     contactUpdatesComplete)
                {
                    updatedSnapShot.udo_integrationstatus = "Success";

                    if (String.IsNullOrEmpty(_logStatusError.ToString()))
                        currentSnapShot.udo_integrationstatus = "Success";
                    else
                        currentSnapShot.udo_integrationstatus = String.Format("Success {0} {1}", Environment.NewLine, _logStatusError.ToString());

                    //Indicate that snapshot needs to be updated
                    snapshotUpdated = true;

                    _logData.AppendLine("Everything Complete");
                }
                else if (String.IsNullOrEmpty(currentSnapShot.udo_integrationstatus))
                {
                    updatedSnapShot.udo_integrationstatus = "Not all data was retrieved";

                    //This is just for return to the UI to display the items waiting
                    if (!ratingsComplete)
                        _statusOutput.AppendLine("Waiting on Ratings");

                    if (!appealsComplete)
                        _statusOutput.AppendLine("Waiting on Appeals");

                    if (!awardsComplete)
                        _statusOutput.AppendLine("Waiting on Awards");

                    if (!claimsComplete)
                        _statusOutput.AppendLine("Waiting on Claims");

                    if (!paymentsComplete)
                        _statusOutput.AppendLine("Waiting on Payments");

                    currentSnapShot.udo_integrationstatus = _statusOutput.ToString();

                    snapshotUpdated = true;
                }
                else if (!ratingsComplete ||
                     !awardsComplete ||
                     !claimsComplete ||
                     !appealsComplete ||
                     !paymentsComplete )
                {
                    //This is just for return to the UI to display the items waiting
                    if (!ratingsComplete)
                        _statusOutput.AppendLine("Waiting on Ratings");

                    if (!appealsComplete)
                        _statusOutput.AppendLine("Waiting on Appeals");

                    if (!awardsComplete)
                        _statusOutput.AppendLine("Waiting on Awards");

                    if (!claimsComplete)
                        _statusOutput.AppendLine("Waiting on Claims");

                    if (!paymentsComplete)
                        _statusOutput.AppendLine("Waiting on Payments");

                    currentSnapShot.udo_integrationstatus = _statusOutput.ToString();
                }


               

                //Update the snapshot if needed

                if (snapshotUpdated)
                {
                    updatedSnapShot.Id = currentSnapShot.Id;

                    if (updatedSnapShot.Id != Guid.Empty)
                        OrganizationService.Update(updatedSnapShot);
                }

                soj = currentSnapShot.GetAttributeValue<string>("udo_soj");

                if (!string.IsNullOrEmpty(soj))
                    sojName = GetSoj(soj);


                txnTimer.Stop();

                ///Set Output Params for Custom Actions
                
                ////Logger.WriteDebugMessage(_logData.ToString());
                tracer.Trace(_logData.ToString());
                Trace(_logData.ToString());
                //Logger.WriteTxnTimingMessage("VetSnapShot Custom Action", txnTimer.ElapsedMilliseconds);

                if (!String.IsNullOrEmpty(sojName))
                    PluginExecutionContext.OutputParameters["SOJ"] = sojName;
                else
                    PluginExecutionContext.OutputParameters["SOJ"] = soj;

                PluginExecutionContext.OutputParameters["SnapShot"] = currentSnapShot.ToEntity<Entity>();
                PluginExecutionContext.OutputParameters["VeteranSnapshotId"] = currentSnapShot.Id.ToString();
                PluginExecutionContext.OutputParameters["ResponseMessage"] = _logData.ToString();                          
            }

            catch (FaultException<OrganizationServiceFault> ex)
            {
                PluginError = true;
                Logger.WriteToFile(string.Format("Error message - {0}. CorrelationId: {1}", ex.Message, PluginExecutionContext.CorrelationId));
                tracer.Trace(string.Format("Error message - {0}. CorrelationId: {1}", ex.Message, PluginExecutionContext.CorrelationId));
                Trace(string.Format("Error message - {0}. CorrelationId: {1}", ex.Message, PluginExecutionContext.CorrelationId));
                //throw new InvalidPluginExecutionException(McsSettings.getUnexpectedErrorMessage);
                _logData.AppendLine(ex.Message);
                _responseMessage = _logData.ToString();
                PluginExecutionContext.OutputParameters["Exception"] = true;
                PluginExecutionContext.OutputParameters["ResponseMessage"] = _responseMessage;
            }
            catch (Exception ex)
            {
                PluginError = true;
                Logger.WriteToFile(string.Format("Error message - {0}. CorrelationId: {1}", ex.Message, PluginExecutionContext.CorrelationId));
                tracer.Trace(string.Format("Error message - {0}. CorrelationId: {1}", ex.Message, PluginExecutionContext.CorrelationId));
                Trace(string.Format("Error message - {0}. CorrelationId: {1}", ex.Message, PluginExecutionContext.CorrelationId));

                _logData.AppendLine(ex.Message);
                _responseMessage = _logData.ToString();
                //throw new InvalidPluginExecutionException(McsSettings.getUnexpectedErrorMessage);
                PluginExecutionContext.OutputParameters["Exception"] = true;
                PluginExecutionContext.OutputParameters["ResponseMessage"] = _responseMessage;
            }
            finally
            {
                Trace("Entered Finally");
                SetupLogger();
                Trace("Set up logger done.");
                ExecuteFinally();
                Trace("Exit Finally");
            }
        }

        private string GetSoj(string sojCode)
        {
            var qe = new QueryExpression
            {
                EntityName = "va_regionaloffice",
                ColumnSet = SojColumns
            };

            var fe1 = new FilterExpression(LogicalOperator.And);
            fe1.AddCondition("statecode", ConditionOperator.Equal, 0);
            var fe2 = new FilterExpression(LogicalOperator.And);
            fe2.AddCondition("va_code", ConditionOperator.Equal, sojCode);
            qe.Criteria.AddFilter(fe1);
            qe.Criteria.AddFilter(fe2);

            var soj = OrganizationService.RetrieveMultiple(qe);

            if (soj.Entities.Count == 0) return null;

            var result = soj.Entities[0];
            return result.GetAttributeValue<string>("va_name");

        }     

        /// <summary>
        /// Retrieves the current users timezone code and locale id
        /// </summary>
        private void RetrieveCurrentUsersSettings()
        {
            var currentUserSettings = OrganizationService.RetrieveMultiple(
                new QueryExpression(UserSettings.EntityLogicalName)
                {
                    ColumnSet = new ColumnSet("localeid", "timezonecode"),
                    Criteria = new FilterExpression
                    {
                        Conditions =
                        {
                            new ConditionExpression("systemuserid", ConditionOperator.EqualUserId)
                        }
                    }
                }).Entities[0].ToEntity<UserSettings>();

            _localeId = currentUserSettings.LocaleId;
            _timeZoneCode = currentUserSettings.TimeZoneCode;
        }

        private LocalTimeFromUtcTimeResponse RetrieveLocalTimeFromUTCTime(DateTime utcTime)
        {
            if (!_timeZoneCode.HasValue)
                return null;
            ////Logger.WriteDebugMessage("_timeZoneCode:" + _timeZoneCode.Value);
            ////Logger.WriteDebugMessage("utcTime:" + utcTime.ToUniversalTime());
            var request = new LocalTimeFromUtcTimeRequest
            {
                TimeZoneCode = _timeZoneCode.Value,
                UtcTime = utcTime.ToUniversalTime()
            };

            var response = (LocalTimeFromUtcTimeResponse)OrganizationService.Execute(request);

            return response;

        }

        private Entity GetContact(Guid contactId)
        {
            return OrganizationService.Retrieve("contact", contactId, ContactColumns);
        }

        private OrganizationResponse CallGetContactUpdatesAction(Guid contactId)
        {
            var req = new OrganizationRequest("udo_GetVeteranUpdates");

            req["ParentEntityReference"] = new EntityReference("contact", contactId);
            return OrganizationService.Execute(req);
        }

        private void CallGetAddressAction(EntityReference contact, EntityReference snapShot)
        {
            var req = new OrganizationRequest("udo_GetAddresses");

            req["ParentEntityReference"] = contact;
            req["VeteranSnapshotReference"] = snapShot;

            OrganizationService.Execute(req);
        }
    }
}
