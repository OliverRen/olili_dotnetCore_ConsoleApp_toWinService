using System.Diagnostics;
using System.ServiceProcess;

namespace ConsoleApp
{
    partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            System.Diagnostics.EventLog.WriteEntry("test service","test start",EventLogEntryType.Information);
        }

        protected override void OnStop()
        {
            System.Diagnostics.EventLog.WriteEntry("test service", "test stop", EventLogEntryType.Information);
        }

        protected override void OnShutdown()
        {
            System.Diagnostics.EventLog.WriteEntry("test service", "test shutdown", EventLogEntryType.Information);
        }
    }
}
