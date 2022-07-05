namespace UDO.LOB.DependentMaintenance.Messages
{
    public interface IDependentParticipant : IParticipant
    {
        DependentRelationship DependentRelationship { get; set; }
    }
}