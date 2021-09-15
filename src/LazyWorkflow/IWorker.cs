using System.Threading.Tasks;

namespace Workflow
{
    public interface IWorker<T>
    {
        Task PostAsync(T message);
    }
}