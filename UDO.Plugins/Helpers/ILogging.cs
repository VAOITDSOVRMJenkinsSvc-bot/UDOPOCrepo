using VA.UDO.Plugins.Helpers;

public interface ILogging
{
    void Trace(string message, int level = 4, bool logToAI = true);

    void AddCustomDimension(string key, string value, int count = 1);

    void AddCustomMetric(string key, double value, int count = 1);

    AppInsightsLogData LogData { get; set; }
}
