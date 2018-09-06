using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScenariosExplorer.Services
{
    public class LockingService : IDisposable
    {
        private static List<LockingService> s_currentLocks = new List<LockingService>();

        public static async Task<LockingService> LockAsync()
        {
            var lockService = new LockingService();
            lock (s_currentLocks)
            {
                s_currentLocks.Add(lockService);
                if (s_currentLocks.Count == 1)
                {
                    lockService.Start();
                }
            }

            await lockService.StartTask;
            return lockService;
        }

        private TaskCompletionSource<bool> m_startTaskSource = new TaskCompletionSource<bool>();
        public Task StartTask => m_startTaskSource.Task;

        public void Dispose()
        {
            LockingService next = null;

            lock (s_currentLocks)
            {
                int index = s_currentLocks.IndexOf(this);
                if (index != -1)
                {
                    s_currentLocks.RemoveAt(index);
                    next = s_currentLocks.ElementAtOrDefault(index);
                }
            }

            if (next != null)
            {
                next.Start();
            }
        }

        private void Start()
        {
            m_startTaskSource.SetResult(true);
        }
    }
}
