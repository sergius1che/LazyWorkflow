using System;

namespace Workflow
{
    internal class CacheValue<T>
    {
        private readonly IWorker<T> _worker;
        private readonly TimeSpan _lifetime;

        private DateTime _expireDate;

        public CacheValue(IWorker<T> worker, TimeSpan lifetime)
        {
            _worker = worker;
            _lifetime = lifetime;
            _expireDate = DateTime.UtcNow + lifetime;
        }

        public IWorker<T> Value => _worker;

        public DateTime ExpireDate => _expireDate;

        public void Refresh()
        {
            _expireDate = DateTime.UtcNow + _lifetime;
        }
    }
}
