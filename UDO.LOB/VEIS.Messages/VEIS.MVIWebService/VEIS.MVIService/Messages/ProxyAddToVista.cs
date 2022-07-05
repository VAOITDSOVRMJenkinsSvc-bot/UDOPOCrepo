using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace VEIS.Mvi.Messages
{
    /// <summary>
    /// Encapsulates the information needed to make an Add Person to MVI request.
    /// </summary>

    [DataContract]
    public class ProxyAddToVistaRequest : Common.AddPersonRequest
    {
        /// <summary>
        /// Gets or sets the The MVI identifier for the person.
        /// </summary>
        /// <remarks>This is a required field.</remarks>
        [DataMember]
        public Identifier MviIdentifier { get; set; }

        /// <summary>
        /// Gets or sets a list of identifiers assoicated with the veteran.
        /// </summary>
        [DataMember]
        public Identifier VistaIdentifier { get; set; }

        /// <summary>
        /// Person's Family Name
        /// </summary>
        public string FamilyName { get; set; }

        /// <summary>
        /// Persons gender
        /// </summary>
        public GenderCode GenderCode { get; set; }

        /// <summary>
        /// Person's birth date.
        /// </summary>
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// Person's Social Security Number
        /// </summary>
        public Identifier SocialSecurityNumber { get; set; }

        /// <summary>
        /// Value indicating if the patient is service connected
        /// </summary>
        public bool PatientServiceConnected { get; set; }

        /// <summary>
        /// Value indicating if a patient is a veteran
        /// </summary>
        public bool PatientVeteran { get; set; }

        /// <summary>
        /// Patient Type.
        /// </summary>
        public int PatientType { get; set; }

        [DataMember(IsRequired = false)]
        public bool LogTiming { get; set; }

        [DataMember(IsRequired = false)]
        public bool LogSoap { get; set; }

        /// <summary>
        /// Creates an Add Person to MVI request with the minimum required data elements.
        /// </summary>
        /// <param name="organizationName">The name of the CRM organization.</param>
        /// <param name="requestor">The CRM user making the request.</param>
        /// <param name="mviIdentifier">The MVI identifier for the person.</param>
        /// <param name="vistaIdenitifers">The data of birth of the veteran.</param>
        /// <returns>A properly formated Proxy Add to Vista request.</returns>
        public static ProxyAddToVistaRequest CreateProxyAddToVistARequest(string organizationName, CrmUser requestor, Identifier mviIdentifier, Identifier vistaIdenitifers)
        {
            return new ProxyAddToVistaRequest
            {
                OrganizationName = organizationName,
                Requestor = requestor,
                VistaIdentifier = vistaIdenitifers,
            };
        }
    }

    /// <summary>
    /// Encapsulates the information returned from a Proxy Add to Vista request.
    /// </summary>

    [DataContract]
    public class ProxyAddToVistaResponse : Common.AddPersonResponse
    {
        /// <summary>
        /// Gets or sets the details of acknowledgment returned from MVI.
        /// </summary>
        public ProxyAddToVistaAcknowledgement ResponseDetail { get; set; }
    }

    /// <summary>
    /// Encapsulates acknowledgement information returned from the Proxy Add to VistA request.
    /// </summary>
    public class ProxyAddToVistaAcknowledgement : Common.Acknowledgement
    {
        /// <summary>
        /// Gets or sets the list of new VistA identifiers returned from MVI.
        /// </summary>
        public List<Identifier> VistaIdentifiers { get; set; }
    }
}
