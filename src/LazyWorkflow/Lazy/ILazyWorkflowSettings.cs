namespace Workflow.Lazy
{
    public interface ILazyWorkflowSettings : IWorkflowSettings
    {
        int LazyPeriodMs { get; }
    }
}