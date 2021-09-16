using System;

namespace Workflow
{
    internal class WorkflowSettings : IWorkflowSettings
    {
        public TimeSpan WorkerLifetime { get; set; }
        public int WorkersCapacity { get; set; }
        public TimeSpan CleanupPeriod { get; set; }
    }
}
