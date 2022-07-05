namespace VRM.Integration.Servicebus.AddDependent.Messages
{
    public interface IDependentParticipant : IParticipant
    {
        DependentRelationship DependentRelationship { get; set; }
    }
}