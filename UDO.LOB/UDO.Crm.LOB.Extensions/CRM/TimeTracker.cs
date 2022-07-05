using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UDO.LOB.Core;
using UDO.LOB.Extensions.Logging;
//using VRM.Integration.Servicebus.Logging.CRM.Util;
//using VRM.Integration.UDO.Common.Messages;

namespace UDO.LOB.Extensions 
{
    public class TimeMark
    {
        public TimeMark()
        {
            Start = 0;
            Stop = 0;
            Name = String.Empty;
            Description = String.Empty;
            IgnoreDuration = false;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public long Start { get; set; }
        public long Stop { get; set; }

        public bool IgnoreDuration { get; set; }

        public long Elapsed
        {
            get
            {
                if (Start == 0) return 0;
                if (Stop < Start) return -1;
                return Stop - Start;
            }
        }

        private static string ToString(string name, string description, long start, long stop, bool ignoreDuraction = false, bool timererror = false)
        {

            string label = String.IsNullOrEmpty(description) ? name : description;

            if (ignoreDuraction) return label;

            string duration;          //12345678901234567890
            var stopstr = string.Format("{0}ms", stop);
            var startstr = string.Format("{0}ms", start);

            if (timererror) duration = "timeerror";
            else if (stop < start)
            {
                duration = "running...";
                stopstr = "running";
            }
            else duration = String.Format("{0}ms", stop - start);

            return String.Format("{0:-14} [{1:-14}  {2:-14}] {3}", duration, startstr, stopstr, label);
        }

        public override string ToString()
        {
            return ToString(Name, Description, Start, Stop, IgnoreDuration);
        }

        public string ToString(Stopwatch timer)
        {
            if (Stop < Start)
            {
                var stop = timer.ElapsedMilliseconds;
                return ToString(Name, Description, Start, stop, IgnoreDuration, (stop < Start));
            }
            return ToString(Name, Description, Start, Stop, IgnoreDuration);
        }
    }

    public class TimeTracker : Stopwatch
    {
        private TimeMark lastMark { get; set; }

        public TimeTracker(bool start = true)
            : base()
        {
            if (start)
            {
                this.Restart();
            }
            Marks = new Dictionary<string, TimeMark>();
            MarkLog = new List<TimeMark>();
            lastMark = null;
        }

        private Dictionary<string, TimeMark> Marks { get; set; }

        private List<TimeMark> MarkLog { get; set; }

        public string MarkStart(string name, params object[] args)
        {
            name = string.Format(name, args);
            lastMark = Marks.ContainsKey(name) ? Marks[name] : new TimeMark() { Name = name };

            // start or restart mark
            lastMark.Start = ElapsedMilliseconds;

            if (!Marks.ContainsKey(name))
            {
                Marks.Add(name, lastMark);
            }

            MarkLog.Add(lastMark);
            return name;
        }

        public void MarkStop(string name, string description = null, params object[] objects)
        {
            TimeMark mark = null;
            if (Marks.ContainsKey(name))
            {
                mark = Marks[name];

                MarkStop(mark, description, objects);
            }
            else
            {
                long start = 0;
                if (lastMark == null)
                {
                    start = lastMark.Stop;
                    if (start == 0) start = lastMark.Start;
                }
                mark = new TimeMark { Name = name, Description = description, Start = start, Stop = ElapsedMilliseconds };
                MarkLog.Add(mark);
            }
            lastMark = mark;
        }

        private void MarkStop(TimeMark mark, string description, object[] objects)
        {
            mark.Stop = ElapsedMilliseconds;
            if (!String.IsNullOrEmpty(description))
            {
                mark.Description = String.Format(description, objects);
            }
            if (Marks.ContainsKey(mark.Name)) Marks.Remove(mark.Name);
        }

        public void Log(string text, params object[] args)
        {
            MarkLog.Add(new TimeMark { Description = String.Format(text, args), IgnoreDuration = true });
        }

        public void SetMarkDescription(string name, string description, params object[] args)
        {
            Marks[name].Description = string.Format(description, args);
        }

        public string GetDurationText(string name)
        {
            return Marks[name].ToString(this);
        }

        public string GetDurations(bool stopNow = false)
        {
            StringBuilder log = new StringBuilder("Timed Log:\r\n");
            foreach (var mark in MarkLog)
            {
                if (stopNow)
                {
                    log.AppendLine(mark.ToString(this));
                }
                else
                {
                    log.AppendLine(mark.ToString());
                }

            }
            if (stopNow) Stop();
            log.AppendFormat("Total Elapsed Time: {0}ms\r\n", ElapsedMilliseconds);

            return log.ToString();
        }

        public long ElapsedSinceMark(string name = "")
        {
            var mark = lastMark;
            if (!String.IsNullOrEmpty(name)) mark = Marks[name];
            var start = mark.Stop;
            if (start == 0) start = mark.Start;
            if (ElapsedMilliseconds > start)
                return ElapsedMilliseconds - start;
            return -1;
        }

        public long LogDurations(string orgName, bool debug, Guid userid, string method, bool stopNow = false, UDORelatedEntity relatedEntity = null)
        {
            // LogDebug (times for sections)
            // LogHelper.LogDebug(orgName, debug, userid, method, GetDurations(stopNow));
            // LogTiming
            try
            {

                var decTimer = Convert.ToDecimal(ElapsedMilliseconds);
                if (relatedEntity != null)
                {
                    //LogHelper.LogTiming(orgName, debug, userid, relatedEntity.RelatedEntityId,
                    //    relatedEntity.RelatedEntityName, relatedEntity.RelatedEntityFieldName, method, method, decTimer);
                }
                else
                {
                    //LogHelper.LogTiming(orgName, debug, userid, decTimer);
                }
            }
            catch (Exception e)
            {
                LogHelper.LogError(orgName, userid, "LogDurations", e.Message);
            }
            return ElapsedMilliseconds;
        }

    }
}
