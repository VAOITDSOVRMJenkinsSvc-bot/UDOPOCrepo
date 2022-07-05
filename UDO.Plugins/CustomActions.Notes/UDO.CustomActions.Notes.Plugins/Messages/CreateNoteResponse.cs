using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace UDO.CustomActions.Notes.Plugins.Messages
{
    public class CreateNoteResponse
    {
        public bool ExceptionOccured { get; set; }
        public string ExceptionMessage { get; set; }

        public CreateNoteResponseInfo UDOCreateNoteInfo { get; set; }
    }

    public class CreateNoteResponseInfo
    {
        public string udo_ClaimId { get; set; }

        public string udo_DateTime { get; set; }

        public string udo_Note { get; set; }

        public string udo_ParticipantID { get; set; }

        public string udo_RO { get; set; }

        public string udo_SuspenseDate { get; set; }

        public string udo_Type { get; set; }

        public string udo_User { get; set; }

        public string udo_UserId { get; set; }

        public string udo_legacynoteid { get; set; }
    }
}