using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;

// These namespaces are found in the Microsoft.Xrm.Sdk.dll assembly
// located in the SDK\bin folder of the SDK download.
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Client;
namespace MCSUtilities2011
{

    public class Initialize
    {
        private IOrganizationService _service;
        public IOrganizationService getService
        {
            get { return _service; }
        }
        private OrganizationServiceProxy _serviceProxy;
        public OrganizationServiceProxy getServiceProxy
        {
            get { return _serviceProxy; }
        }
        private Guid _initiatingUserId;
        public Guid setInitiatingUserId
        {
            set { _initiatingUserId = value; }
        }
        private Guid _currentCaseId;
        public Guid getCaseId
        {
            get { return _currentCaseId; }
        }
        private string _currentCaseNumber;
        public string getCurrentCaseNumber
        {
            get { return _currentCaseNumber; }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
        }
        private string _org;
        public string setOrg
        {
            set { _org = value; }
        }
        private string _server;
        public string setServer
        {
            set { _server = value; }
        }
        private MCSLogger _logger;
        public MCSLogger setlogger
        {
            set { _logger = value; }
        }

        public void ExecuteInit()
        {
            try
            {
                var serverConnect = new ServerConnection();
                var config = serverConnect.GetServerConfiguration(_org, _server);

                _serviceProxy = new OrganizationServiceProxy(config.OrganizationUri,
                                                                        config.HomeRealmUri,
                                                                        config.Credentials,
                                                                        config.DeviceCredentials);

                // This statement is required to enable early-bound type support.
                _serviceProxy.ServiceConfiguration.CurrentServiceEndpoint.Behaviors.Add(new ProxyTypesBehavior());
                if (_initiatingUserId != null)
                {
                    _serviceProxy.CallerId = _initiatingUserId;
                }
                _service = (IOrganizationService)_serviceProxy;
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                _logger.setMethod = "GetUserEntity";
                //_logger.WriteToFile(ex.Message);
                _errorMessage = "Error in ExecuteInit - see log files.";
            }
            catch (Exception ex)
            {
                _logger.setMethod = "ExecuteInit";
                //_logger.WriteToFile(ex.Message);
                _errorMessage = "Error in ExecuteInit - see log files.";
            }
        }
        public Entity GetUserEntity(out string initiatingUserName, out string initiatingUserInitials)
        {
            initiatingUserInitials = "unk";
            initiatingUserName = "unknown";

            try
            {

                ColumnSet attributes = new ColumnSet(new String[] { "businessunitid", "lastname", "firstname", "domainname", "fullname", "internalemailaddress", "new_initials" });

                // Retrieve the account and its name and ownerid attributes.
                Entity initiatingUserEntity = _service.Retrieve("systemuser", _initiatingUserId, attributes);


                string domainName = initiatingUserEntity["domainname"].ToString();

                int backslashIndex = domainName.IndexOf("\\");
                int usernameLength = (domainName.Length - 1) - backslashIndex;
                initiatingUserName = domainName.Substring(backslashIndex + 1, usernameLength);
                if (initiatingUserEntity.Attributes.Contains("new_initials"))
                {
                    initiatingUserInitials = initiatingUserEntity["new_initials"].ToString();
                }
                return initiatingUserEntity;
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                _logger.setMethod = "GetUserEntity";
                //_logger.WriteToFile(ex.Message);
                _errorMessage = "Error in GetUserEntity - see log files.";
                return null;
            }
            catch (Exception ex)
            {
                _logger.setMethod = "GetUserEntity";
                //_logger.WriteToFile(ex.Message);
                _errorMessage = "Error in GetUserEntity - see log files.";
                return null;
            }
        }
        public Entity GetCurrentObject(Guid objectId, string ObjectTypeName)
        {

            try
            {

                ColumnSet attributes = new ColumnSet(true);

                // Retrieve the account and its name and ownerid attributes.
                Entity currentObjectEntity = _service.Retrieve(ObjectTypeName, objectId, attributes);
                if (currentObjectEntity.Attributes.Contains("new_interpolcasenumberid"))
                {
                    EntityReference thisCase = (EntityReference)currentObjectEntity["new_interpolcasenumberid"];

                    _currentCaseId = thisCase.Id;
                    _currentCaseNumber = thisCase.Name;
                }
                return currentObjectEntity;
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                _logger.setMethod = "GetCurrentObject";
                if (ex.Message.Contains("Does Not Exist"))
                {
                    //_logger.WriteDebugMessage("waiting 10 seconds for:" + ex.Message);
                    System.Threading.Thread.Sleep(10000);
                    #region try again
                    try
                    {

                        ColumnSet attributes = new ColumnSet(true);

                        // Retrieve the account and its name and ownerid attributes.
                        Entity currentObjectEntity = _service.Retrieve(ObjectTypeName, objectId, attributes);
                        if (currentObjectEntity.Attributes.Contains("new_interpolcasenumberid"))
                        {
                            EntityReference thisCase = (EntityReference)currentObjectEntity["new_interpolcasenumberid"];

                            _currentCaseId = thisCase.Id;
                            _currentCaseNumber = thisCase.Name;
                        }
                        return currentObjectEntity;
                    }
                    catch (FaultException<OrganizationServiceFault> ex2)
                    {
                        _logger.setMethod = "GetCurrentObject Try 2";
                        //_logger.WriteToFile(ex2.Message);
                        _errorMessage = "Error in GetCurrentObject - see log files.";
                        return null;
                    }
                    catch (Exception ex2)
                    {
                        _logger.setMethod = "GetCurrentObject";
                        //_logger.WriteToFile(ex2.Message);
                        _errorMessage = "Error in GetCurrentObject - see log files.";
                        return null;
                    }
                    #endregion
                }
                else
                {

                    _logger.setMethod = "GetCurrentObject - Fault";
                    //_logger.WriteToFile(ex.Message);
                    _errorMessage = "Error in GetCurrentObject - see log files.";
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.setMethod = "GetCurrentObject- Ex";
                //_logger.WriteToFile(ex.Message);
                _errorMessage = "Error in GetCurrentObject - see log files.";
                return null;
            }
        }
       
    }
}
