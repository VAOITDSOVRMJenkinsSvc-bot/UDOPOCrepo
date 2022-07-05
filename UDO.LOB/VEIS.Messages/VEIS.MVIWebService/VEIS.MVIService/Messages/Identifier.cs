using System.Runtime.Serialization;
namespace VEIS.Mvi.Messages
{
    /// <summary>
    /// Represents a VA identifier string.
    /// </summary>
    [DataContract]
    public class Identifier
    {
        /// <summary>
        /// Gets or sets the identifier value.
        /// </summary>
        /// <remarks>This is a required field.</remarks>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the type of identifier.
        /// </summary>
        /// <remarks>This is a required field.</remarks>
        [DataMember]
        public IdentifierType Type { get; set; }

        /// <summary>
        /// Gets or sets the display name of the identifier.
        /// </summary>
        [DataMember]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the assigning authority of the identifier.
        /// </summary>
        [DataMember]
        public string AssigningAuthority { get; set; }

        /// <summary>
        /// Gets or sets the assigning facility of the identifier.
        /// </summary>
        [DataMember]
        public string AssigningFacility { get; set; }

        /// <summary>
        /// Creates an identifier object containing a social security number.
        /// </summary>
        /// <param name="ssn">The social security number.</param>
        /// <returns>A properly formatted identifier object.</returns>
        public static Identifier CreateSocialSecurityIdentifier(string ssn)
        {
            if (!string.IsNullOrEmpty(ssn))
                return null;

            return new Identifier
            {
                Id = ssn,
                Type = IdentifierType.SocialSecurityNumber
            };
        }

        /// <summary>
        /// Creates an identifier object containing a patient identifier.
        /// </summary>
        /// <param name="id">The id value.</param>
        /// <param name="assigningFacility">The identifier of the facility that assgined the id.</param>
        /// <returns>A properly formatted identifier object.</returns>
        /// <remarks>If id is null or empty, an identifier is returned with "PROXY_VISTA" in the id field.</remarks>
        public static Identifier CreatePatientIdentifier(string assigningFacility, string id = "")
        {
            if (string.IsNullOrEmpty(assigningFacility))
                return null;

            if (string.IsNullOrEmpty(id))
                id = "PROXY_VISTA";

            return new Identifier
            {
                Id = id,
                Type = IdentifierType.PatientIdentifier,
                AssigningFacility = assigningFacility,
                AssigningAuthority = "USVHA"
            };
        }

        /// <summary>
        /// Creates an identifier object containing a MVI identifier.
        /// </summary>
        /// <param name="id">The MVI id value.</param>
        /// <returns>A properly formatted identifier object.</returns>
        public static Identifier CreateMviIdentifier(string id)
        {
            if (!string.IsNullOrEmpty(id))
                return null;

            return new Identifier
            {
                Id = id,
                Type = IdentifierType.NationalIdentifier
            };
        }

        /// <summary>
        /// Creates an identifier object containing a EDIPI identifier.
        /// </summary>
        /// <param name="id">The edipi id value.</param>
        /// <returns>A properly formatted identifier object.</returns>
        public static Identifier CreateEdipiIdentifier(string id)
        {
            if (!string.IsNullOrEmpty(id))
                return null;

            return new Identifier
            {
                Id = id,
                Type = IdentifierType.NationalIdentifier,
                AssigningFacility = "200DOD",
                AssigningAuthority = "USDOD"
            };
        }

        /// <summary>
        /// Creates an identifier object containing a CORPDB identifier.
        /// </summary>
        /// <param name="id">The corpdb id value.</param>
        /// <returns>A properly formatted identifier object.</returns>
        public static Identifier CreateCorpDbIdentifier(string id)
        {
            if (!string.IsNullOrEmpty(id))
                return null;

            return new Identifier
            {
                Id = id,
                Type = IdentifierType.ParticipantNumber,
                AssigningFacility = "200CORP",
                AssigningAuthority = "USVBA"
            };
        }
    }
}
