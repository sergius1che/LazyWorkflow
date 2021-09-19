using System.Threading.Tasks;

namespace Workflow
{
    public interface IWorkflowForkManagment<T>
        where T : class
    {
        Task PostAsync(T message);
    }
}
