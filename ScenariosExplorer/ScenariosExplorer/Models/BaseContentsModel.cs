using ScenariosExplorer.ApplicationSettings;
using ScenariosExplorer.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ScenariosExplorer.Models
{
    public class BaseContentsModel
    {
        public string Id { get; set; }

        public RepoInfo Repo { get; private set; }

        public BaseContentsModel(string identifier, RepoInfo repo)
        {
            Id = identifier;
            Repo = repo;
        }
        
        public async Task<string> GetContentsAsync()
        {
            if (Contents == null)
            {
                if (Id != null)
                {
                    try
                    {
                        Contents = await (await ContentService.GetAsync(Repo)).GetFileContentsAsync("files\\" + Id + ".md");
                    }
                    catch { }
                }
            }

            return Contents;
        }

        public static async Task SaveContentsAsync(string id, RepoInfo repo, string contents)
        {
            if (contents == null)
            {
                return;
            }

            var contentService = await ContentService.GetAsync(repo);
            File.WriteAllText(contentService.GetFilePath("files\\" + id + ".md"), contents);
        }

        public string Contents { get; set; }
    }
}
