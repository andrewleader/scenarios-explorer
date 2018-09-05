using ScenariosExplorer.ApplicationSettings;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ScenariosExplorer.Services
{

    public class CloningService
    {
        public static CloningService Current { get; private set; } = new CloningService();

        private List<CloningRequest> m_queue = new List<CloningRequest>();

        public Task CloneAsync(GitHubRepo repo)
        {
            lock (m_queue)
            {
                var matches = m_queue.Where(i => i.Repo.Equals(repo)).Take(2).ToArray();

                // If there's already another pending, merge into that
                if (matches.Length > 1)
                {
                    return matches.Last().Task;
                }

                // Otherwise, if there's only one currently executing, add a new one that waits on the current one
                if (matches.Length == 1)
                {
                    m_queue.Add(new CloningRequest(repo, m_queue, m_queue.First()));
                    return m_queue.Last().Task;
                }

                // Otherwise, none currently executing, add and execute now
                m_queue.Add(new CloningRequest(repo, m_queue));
                return m_queue.First().Task;
            }
        }

        private class CloningRequest
        {
            public Task Task { get; set; }
            private List<CloningRequest> m_queue;
            public GitHubRepo Repo { get; private set; }

            public CloningRequest(GitHubRepo repo, List<CloningRequest> queue)
            {
                Repo = repo;
                m_queue = queue;

                Task = ActuallyCloneAsync();
            }

            public CloningRequest(GitHubRepo repo, List<CloningRequest> queue, CloningRequest before)
            {
                Repo = repo;
                m_queue = queue;

                Task = WaitAndCloneAsync(before.Task);
            }

            private async Task WaitAndCloneAsync(Task taskToWaitFor)
            {
                try
                {
                    await taskToWaitFor;
                }
                catch { }

                await ActuallyCloneAsync();
            }

            private Task ActuallyCloneAsync()
            {
                var task = ActuallyCloneAsyncHelper();

                return task;
            }

            private string CreateGitHubZipUrl()
            {
                return $"https://github.com/{Repo.Owner}/{Repo.RepositoryName}/archive/{Repo.Branch}.zip";
            }

            private async Task ActuallyCloneAsyncHelper()
            {
                try
                {
                    string tempFile = Path.GetTempFileName();
                    using (var client = new HttpClient())
                    {
                        using (var stream = await client.GetStreamAsync(CreateGitHubZipUrl()))
                        {
                            using (var fileStream = File.OpenWrite(tempFile))
                            {
                                await stream.CopyToAsync(fileStream);
                            }
                        }
                    }

                    ZipFile.ExtractToDirectory(tempFile, ContentService.GetExtractDestinationFolder(Repo), overwriteFiles: true);
                    File.Delete(tempFile);
                }
                finally
                {
                    lock (m_queue)
                    {
                        m_queue.Remove(this);
                    }
                }
            }
        }
    }
}
