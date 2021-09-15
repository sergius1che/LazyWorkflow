using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workflow
{
    public interface IWorkerFactory<T>
    {
        IWorker<T> CreateWorker();
    }
}
