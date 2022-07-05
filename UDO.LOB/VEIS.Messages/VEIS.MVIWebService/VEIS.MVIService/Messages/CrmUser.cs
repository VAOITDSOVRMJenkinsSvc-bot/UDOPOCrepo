using System;
using System.Runtime.Serialization;

namespace VEIS.Mvi.Messages
{
    /// <summary>
    /// Encapsulates data about the CRM user that initiates the request.
    /// </summary>
    [DataContract]
    public class CrmUser
    {
        /// <summary>
        /// Gets or sets the user's CRM id.
        /// </summary>
        /// <remarks>This is a required field.</remarks>
        [DataMember]
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the user's last name.
        /// </summary>
        /// <remarks>This is a required field.</remarks>
        [DataMember]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the user's first name.
        /// </summary>
        /// <remarks>This is a required field.</remarks>
        [DataMember]
        public string FirstName { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public CrmUser()
        { }

        /// <summary>
        /// Creates a CrmUser object with required values.
        /// </summary>
        /// <param name="userId">The user's id in CRM.</param>
        /// <param name="lastName">The user's last name.</param>
        /// <param name="firstName">The user's first name.</param>
        /// <returns>A properly formatted CrmUser object.</returns>
        public static CrmUser GetCrmUser(Guid userId, string lastName, string firstName)
        {
            if (string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(firstName) ||
                userId == null || userId == Guid.Empty)
                throw new ArgumentException("Missing required parameters for creating a CrmUser object.");

            return new CrmUser
            {
                UserId = userId,
                LastName = lastName,
                FirstName = firstName
            };
        }
    }
}
