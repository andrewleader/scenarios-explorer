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
        public RepoInfo Repo { get; private set; }

        public ContentService(RepoInfo repo)
        {
            Repo = repo;
        }

        public static async Task<ContentService> GetAsync(RepoInfo repo)
        {
            if (!Directory.Exists(GetRepoFolder(repo)))
            {
                await CloningService.Current.CloneAsync(repo);
            }

            return new ContentService(repo);
        }

        public static async Task DeleteCacheAsync(RepoInfo repo)
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
            return await File.ReadAllTextAsync(GetFilePath(path));
        }

        public string GetFilePath(string subpath)
        {
            return GetRepoFolder(Repo) + "\\" + subpath;
        }

        public static string GetExtractDestinationFolder(RepoInfo repo)
        {
            string answer = Environment.GetEnvironmentVariable("LocalAppData") + $"\\Repos\\{repo.UrlHash()}";
            return answer;
        }

        public static string GetRepoFolder(RepoInfo repo)
        {
            return GetExtractDestinationFolder(repo) + $"\\{repo.Branch}";
        }
    }
}
