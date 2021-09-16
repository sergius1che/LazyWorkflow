using System.Threading.Tasks;

namespace Workflow
{
    public interface IWorkflowForkManagment<T>
    {
        Task PostAsync(T message);
    }
}
