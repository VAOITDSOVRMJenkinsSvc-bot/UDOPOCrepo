using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRM.Integration.UDO.MVI.Messages
{
    public class UDORelatedEntity
    {
        public string RelatedEntityName { get; set; }
        public Guid RelatedEntityId { get; set; }
        public string RelatedEntityFieldName { get; set; }
    }
}
