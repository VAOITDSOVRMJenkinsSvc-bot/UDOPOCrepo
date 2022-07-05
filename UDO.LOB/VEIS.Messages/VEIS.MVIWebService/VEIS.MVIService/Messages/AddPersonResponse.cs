using System;

using System.Runtime.Serialization;
using VEIS.Core.Core;

namespace VEIS.Mvi.Messages
{

    [DataContract]
    public class AddPersonResponse : MessageBase
    {
        [DataMember]
        public bool ExceptionOccured { get; set; }

        [DataMember]
        public string Message { get; set; }

        /// <summary>
        /// Sets or gets the actual exception from MVI.
        /// </summary>
        [DataMember]
        public string RawMviExceptionMessage { get; set; }

        /// <summary>
        /// Sets or get the MVI Acknowlegment details that contain MVIerror codes and texts.
        /// </summary>
        [DataMember]
        public Acknowledgement Acknowledgement { get; set; }

    }
}
