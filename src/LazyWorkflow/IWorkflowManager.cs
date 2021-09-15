using System.Threading.Tasks;

namespace Workflow
{
    public interface IWorkflowManager<T>
    {
        Task PostAsync(T message);
    }
}
