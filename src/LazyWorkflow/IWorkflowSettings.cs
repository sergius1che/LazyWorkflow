using System;

namespace Workflow
{
    public interface IWorkflowSettings
    {
        TimeSpan WorkerLifetime { get; }

        int WorkersCapacity { get; }

        TimeSpan CleanupPeriod { get; }
    }
}
