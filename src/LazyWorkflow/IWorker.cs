using System.Threading.Tasks;

namespace Workflow
{
    public interface IWorker<T>
        where T : class
    {
        Task PostAsync(T message);
    }
}