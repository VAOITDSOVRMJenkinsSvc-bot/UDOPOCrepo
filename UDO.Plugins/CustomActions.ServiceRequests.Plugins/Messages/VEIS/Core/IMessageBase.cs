using System.Runtime.Serialization;

namespace VEIS.Core.Core
{
    public interface IMessageBase : IExtensibleDataObject
    {
        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        string MessageId { get; set; }
    }
}