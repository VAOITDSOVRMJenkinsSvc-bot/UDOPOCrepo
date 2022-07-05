using System.Collections.Generic;
using UDO.LOB.Core;
using UDO.LOB.Core.Interfaces;

namespace UDO.LOB.Extensions 
{
    public interface ITimedProcessor
    {
        string CurrentMethod { get; set; }
        TimeTracker Timer { get; set; }
        Stack<string> MethodHistory { get; set; }

        // Methods
        void StopTimer(UDORequestBase request);
        string LogStartOfMethod(string name, params object[] args);
        string LogEndOfMethod(string description = null, params object[] args);
    }
}