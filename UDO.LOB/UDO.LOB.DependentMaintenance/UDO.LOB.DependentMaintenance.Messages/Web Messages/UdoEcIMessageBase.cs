using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace UDO.LOB.DependentMaintenance.Messages
{
    public interface UdoEcIMessageBase : IExtensibleDataObject
    {
        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        string MessageId { get; set; }
    }
}
