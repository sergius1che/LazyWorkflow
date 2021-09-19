namespace Workflow.Lazy
{
    internal class LazyWorkflowSettings : WorkflowSettings, ILazyWorkflowSettings
    {
        public int LazyPeriodMs { get; set; }
    }
}
