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
            try
            {
                DeleteReadOnlyDirectory(GetRepoFolder(repo));
            }
            catch { }
        }
        
        /// <summary>
         /// Recursively deletes a directory as well as any subdirectories and files. If the files are read-only, they are flagged as normal and then deleted.
         /// </summary>
         /// <param name="directory">The name of the directory to remove.</param>
        private static void DeleteReadOnlyDirectory(string directory)
        {
            foreach (var subdirectory in Directory.EnumerateDirectories(directory))
            {
                DeleteReadOnlyDirectory(subdirectory);
            }
            foreach (var fileName in Directory.EnumerateFiles(directory))
            {
                var fileInfo = new FileInfo(fileName);
                fileInfo.Attributes = FileAttributes.Normal;
                fileInfo.Delete();
            }
            Directory.Delete(directory);
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
