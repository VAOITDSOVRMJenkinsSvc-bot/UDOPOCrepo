namespace UDO.LOB.Core
{
    public interface ILegacyHeaderInfo
    {
        string StationNumber { get; set; }
        string LoginName { get; set; }
        string ApplicationName { get; set; }
        string ClientMachine { get; set; }
    }
}