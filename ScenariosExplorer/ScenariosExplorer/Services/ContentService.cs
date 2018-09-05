using ScenariosExplorer.ApplicationSettings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ScenariosExplorer.Services
{
    public class ContentService
    {
        public GitHubRepo Repo { get; private set; }

        public ContentService(GitHubRepo repo)
        {
            Repo = repo;
        }

        public static async Task<ContentService> GetAsync(GitHubRepo repo)
        {
            if (!Directory.Exists(GetRepoFolder(repo)))
            {
                await CloningService.Current.CloneAsync(repo);
            }

            return new ContentService(repo);
        }

        public static async Task DeleteCacheAsync(GitHubRepo repo)
        {
            Directory.Delete(GetRepoFolder(repo));
        }

        public async Task<IEnumerable<string>> EnumerateFilesAsync(string directory)
        {
            return Directory.EnumerateFiles(GetRepoFolder(Repo) + "\\" + directory).Select(i => Path.GetFileNameWithoutExtension(i));
        }

        public async Task<string> GetFileContentsAsync(string path)
        {
            return await File.ReadAllTextAsync(GetRepoFolder(Repo) + "\\" + path);
        }

        public static string GetExtractDestinationFolder(GitHubRepo repo)
        {
            return Environment.GetEnvironmentVariable("LocalAppData") + $"\\Repos\\{repo.Owner}";
        }

        public static string GetRepoFolder(GitHubRepo repo)
        {
            return GetExtractDestinationFolder(repo) + $"\\{repo.RepositoryName}-{repo.Branch}";
        }
    }
}
