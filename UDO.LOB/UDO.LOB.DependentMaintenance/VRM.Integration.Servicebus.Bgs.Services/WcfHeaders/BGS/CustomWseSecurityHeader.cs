using System.ServiceModel.Channels;
using System.Xml;

namespace VRM.Integration.Servicebus.Bgs.Services
{
    internal class CustomWseSecurityHeader : MessageHeader
    {
        private readonly string _UserName;
        private readonly string _Password;
        private readonly string _ClientMachine;
        private readonly string _StnId;
        private readonly string _ApplicationId;

        public CustomWseSecurityHeader(string userName,
            string password,
            string clientMachine,
            string stnId,
            string applicationId)
        {
            _UserName = userName;
            _Password = password;
            _ClientMachine = clientMachine;
            _StnId = stnId;
            _ApplicationId = applicationId;
        }

        protected override void OnWriteStartHeader(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            writer.WriteStartElement(Prefix, Name, Namespace);

            WriteHeaderAttributes(writer, messageVersion);
        }

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer,
            MessageVersion messageVersion)
        {
            writer.WriteStartElement(Prefix, "UsernameToken", Namespace);
            writer.WriteElementString(Prefix, "Username", Namespace, _UserName);
            writer.WriteElementString(Prefix, "Password", Namespace, _Password);

            writer.WriteEndElement();

            writer.WriteStartElement(VaPrefix, "VaServiceHeaders", VaNamespace);
            writer.WriteElementString(VaPrefix, "CLIENT_MACHINE", VaNamespace, _ClientMachine);
            writer.WriteElementString(VaPrefix, "STN_ID", VaNamespace, _StnId);
            writer.WriteElementString(VaPrefix, "applicationName", VaNamespace, _ApplicationId);

            writer.WriteEndElement();
        }

        public string Prefix
        {
            get { return "wsse"; }
        }

        public string VaPrefix
        {
            get { return "vaws"; }
        }

        public override string Name
        {
            get { return "Security"; }
        }

        public override string Namespace
        {
            get { return "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"; }
        }

        public string VaNamespace
        {
            get { return "http://vbawebservices.vba.va.gov/vawss"; }
        }

        public override bool MustUnderstand
        {
            get { return false; }
        }
    }
}