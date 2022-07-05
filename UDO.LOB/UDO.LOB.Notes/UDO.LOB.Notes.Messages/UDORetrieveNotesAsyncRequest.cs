﻿using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using UDO.LOB.Core;
using VEIS.Messages.DevelopmentNotesService;


//using VIMT.DevelopmentNotesService.Messages;
//using VRM.Integration.Servicebus.Core;

namespace UDO.LOB.Notes.Messages
{
    //[Export(typeof(IMessageBase))]
    //[ExportMetadata("MessageType", MessageRegistry.UDORetrieveNotesAsyncRequest)]
    [DataContract]
    public class UDORetrieveNotesAsyncRequest : UDORetrieveNotesRequest
    {
        //[DataMember]
        //public VIMTfinddevfoundNoteMultipleResponse[] Notes { get; set; }
        [DataMember]
        public VEISfinddevnoteMultipleResponse[] Notes { get; set; }
        [DataMember]
        public int Start { get; set; }
        [DataMember]
        public int End { get; set; }
        [DataMember]
        public string CurrentPcrSSN { get; set; }
        [DataMember]
        public bool isRelatedPerson { get; set; }

        public UDORetrieveNotesAsyncRequest()
        {

        }

        public UDORetrieveNotesAsyncRequest(UDORetrieveNotesRequest source)
        {
            this.OrganizationName = source.OrganizationName;
            UserId = source.UserId;
            RelatedParentId = source.RelatedParentId;
            RelatedParentEntityName = source.RelatedParentEntityName;
            RelatedParentFieldName = source.RelatedParentFieldName;
            LogTiming = source.LogTiming;
            LogSoap = source.LogSoap;
            Debug = source.Debug;
            LegacyServiceHeaderInfo = source.LegacyServiceHeaderInfo;
            ptcpntId = source.ptcpntId;
            claimid = source.claimid;
            LoadSize = source.LoadSize;
            //this.owner = source.owner;
            OwnerId = source.OwnerId;
            OwnerType = source.OwnerType;
            // ref copy
            RelatedEntities = source.RelatedEntities;
            // value Copy
            // REM: None, this is previously commented code.
            //Todo: What is the purpose of this property?
            //UDORetrieveNotesRelatedEntitiesInfo = (UDORetrieveNotesRelatedEntitiesMultipleRequest[])source.UDORetrieveNotesRelatedEntitiesInfo.Clone();
        }
    }
}