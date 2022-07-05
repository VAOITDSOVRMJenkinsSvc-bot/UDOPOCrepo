using System.ServiceModel.Channels;
using System.Xml;

namespace VRM.Integration.Servicebus.Bgs.Services
{
    public class CustomVvaSecurityHeader : MessageHeader
    {
        public CustomVvaSecurityHeader(string username, string password)
        {
            Username = username;
            Password = password;
        }

        protected override void OnWriteStartHeader(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            writer.WriteStartElement("wsse", Name, Namespace);

            // Add the required Namespaces
            writer.WriteXmlnsAttribute("wsse", Namespace);

            writer.WriteXmlnsAttribute("wsu", SecUtilityNamespace);
        }

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            // UsernameToken Element
            writer.WriteStartElement("wsse", "UsernameToken", Namespace);

            // UserName Element
            writer.WriteAttributeString("wsu:Id", "UsernameToken-1");
            writer.WriteStartElement("wsse", "Username", Namespace);
            writer.WriteValue(Username);
            writer.WriteEndElement();

            // Password Element
            writer.WriteStartElement("wsse", "Password", Namespace);

            // Set the Password Type to Text
            writer.WriteAttributeString("Type", PasswordType);
            writer.WriteValue(Password);
            writer.WriteEndElement();

            // Add nonce and timestamp elements if required
            // End UsernameToken Element
            writer.WriteEndElement();
        }

        public override string Name
        {
            get { return "Security"; }
        }

        public override string Namespace
        {
            get { return "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"; }
        }

        public string SecUtilityNamespace
        {
            get { return "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"; }
        }

        public string PasswordType
        {
            get { return "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText"; }
        }

        public string Username { get; set; }
        public string Password { get; set; }
    }
}
