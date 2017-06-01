using Microsoft.Diagnostics.EventFlow;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventFlowEventSourceAITest
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var pipeline = DiagnosticPipelineFactory.CreatePipeline("eventFlowConfig.json"))
            {
                var logger = new EventSourceLogger("Supplylogix-EventFlow-Tester");

                logger.LogDebug("Debug");
                logger.LogInfo("Info");
                logger.LogWarn("Warn");
                logger.LogError("Error", new Exception("BOOM"));

                Console.WriteLine("Press any key to exit");
                Console.ReadKey(intercept: true);
            }
        }

        public class EventSourceLogger : EventSource
        {
            public EventSourceLogger(string name)
                : base(name, EventSourceSettings.EtwSelfDescribingEventFormat) { }

            private const string LogMessageEventName = "LogMessage";

            [NonEvent]
            private void LogMessage(EventLevel level, string msg, Exception e = null)
            {
                Write(LogMessageEventName, new EventSourceOptions { Level = level }, new { Message = GetFinalMessage(msg, e) });
            }

            [NonEvent]
            public void LogDebug(string msg)
            {
                LogMessage(EventLevel.Verbose, msg);
            }

            [NonEvent]
            public void LogInfo(string msg)
            {
                LogMessage(EventLevel.Informational, msg);
            }

            [NonEvent]
            public void LogWarn(string msg)
            {
                LogMessage(EventLevel.Warning, msg);
            }

            [NonEvent]
            public void LogError(string msg, Exception e)
            {
                LogMessage(EventLevel.Error, msg, e);
            }

            protected string GetFinalMessage(string msg, Exception e)
            {
                return e == null ? msg : $"{msg} - {e.ToString()}";
            }
        }
    }
}
