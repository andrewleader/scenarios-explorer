using ScenariosExplorer.ApplicationSettings;
using ScenariosExplorer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScenariosExplorer.Models
{
    public class BaseContentsModel
    {
        public string Id { get; set; }

        public GitHubRepo Repo { get; private set; }

        public BaseContentsModel(string identifier, GitHubRepo repo)
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

        public string Contents { get; private set; }
    }
}
