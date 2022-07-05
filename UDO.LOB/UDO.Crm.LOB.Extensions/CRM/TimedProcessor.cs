using System;
using System.Collections.Generic;
using UDO.LOB.Core;
using UDO.LOB.Core.Interfaces;
//using VRM.Integration.Servicebus.Core;
//using VRM.Integration.UDO.Common.Interfaces;
//using VRM.Integration.UDO.Common.Messages;

namespace UDO.LOB.Extensions
{
    public class TimedProcessor : ITimedProcessor
    {
        public string CurrentMethod { get; set; }
        public TimeTracker Timer { get; set; }
        public Stack<string> MethodHistory { get; set; }

        public TimedProcessor()
        {
            Timer = new TimeTracker();
            MethodHistory = new Stack<string>();
        }

        public void StopTimer(UDORequestBase request)
        {
            #region Stop Timer

            Timer.LogDurations(request.OrganizationName, request.Debug, request.UserId, CurrentMethod, true, new UDORelatedEntity
            {
                RelatedEntityId = request.RelatedParentId,
                RelatedEntityFieldName = request.RelatedParentFieldName,
                RelatedEntityName = request.RelatedParentEntityName
            });
            MethodHistory.Clear();
            #endregion
        }

        public string LogStartOfMethod(string name, params object[] args)
        {
            CurrentMethod = Timer.MarkStart(name, args);
            MethodHistory.Push(CurrentMethod);
            return CurrentMethod;
        }

        public string LogEndOfMethod(string description = null, params object[] args)
        {
            var name = MethodHistory.Pop();
            Timer.MarkStop(name, description, args);
            if (MethodHistory.Count == 0)
            {
                CurrentMethod = null;
            }
            else
            {
                CurrentMethod = MethodHistory.Peek();
            }
            return CurrentMethod;
        }
    }
}