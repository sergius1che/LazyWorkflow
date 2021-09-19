namespace Workflow
{
    public interface IWorkerFactory<T>
        where T : class
    {
        IWorker<T> CreateWorker();
    }
}
