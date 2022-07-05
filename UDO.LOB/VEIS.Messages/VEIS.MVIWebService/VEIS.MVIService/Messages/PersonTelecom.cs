using System.Runtime.Serialization;

namespace VEIS.Mvi.Messages
{
    /// <summary>
    /// Encapsulates the data needed for a telecommunications number or address.
    /// </summary>
    [DataContract]
    public class PersonTelecom
    {
        /// <summary>
        /// Gets or sets the usage for the telecom address.
        /// </summary>
        /// <remarks>This is a required field.</remarks>
        [DataMember]
        public TelecomUse Use { get; set; }

        /// <summary>
        /// Gets or sets the telecommunications number or address.
        /// </summary>
        /// <remarks>This is a required field.</remarks>
        [DataMember]
        public string TelecomNumberOrAddress { get; set; }

        /// <summary>
        /// Creates a person telecom object representing a home phone number.
        /// </summary>
        /// <param name="phoneNumber">The phone number string.</param>
        /// <returns>A properly formated PersonTelecom object.</returns>
        public static PersonTelecom CreateHomePhone(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                return null;

            return new PersonTelecom
            {
                Use = TelecomUse.PrimaryHome,
                TelecomNumberOrAddress = phoneNumber
            };
        }
    }

}
