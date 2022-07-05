using System.Runtime.Serialization;

namespace VEIS.Mvi.Messages
{
    [DataContract]
    public class PatientAddress
    {
        [DataMember]
        public AddressUse Use { get; set; }

        [DataMember]
        public string StreetAddressLine { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string PostalCode { get; set; }

        [DataMember]
        public string Country { get; set; }

        [DataMember]
        public string State { get; set; }

        /// <summary>
        /// Creates a PersonAddress object representing a home address.
        /// </summary>
        /// <param name="addressLine">The home's street address.</param>
        /// <param name="city">The city for the address.</param>
        /// <param name="state">The state for the address.</param>
        /// <param name="postalCode">The postal code for the address.</param>
        /// <param name="country">The country for the address.</param>
        /// <returns>A properly formatted PatientAddress object.</returns>
        public static PatientAddress CreateHomeAddress(string addressLine, string city, string state, string postalCode, string country = "USA")
        {
            return new PatientAddress
            {
                Use = AddressUse.PrimaryHome,
                StreetAddressLine = addressLine,
                City = city,
                State = state,
                PostalCode = postalCode,
                Country = country
            };
        }

        /// <summary>
        /// Creates a PatientAddress object containing the minimum required information.
        /// </summary>
        /// <param name="use">The use of the address.</param>
        /// <param name="city">The city for the address.</param>
        /// <param name="state">The state for the address.</param>
        /// <returns>A properly formatted PatientAddress object.</returns>
        public static PatientAddress CreateMinimalAddress(AddressUse use, string city, string state)
        {
            return new PatientAddress
            {
                Use = use,
                City = city,
                State = state
            };
        }
    }
}
