using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScenariosExplorer.ApplicationSettings;

namespace ScenariosExplorer.Models
{
    public class ProposalModel : BaseContentsModel
    {
        public ProposalModel() : base(null, null) { }

        public ProposalModel(string identifier, RepoInfo repo) : base(identifier, repo)
        {
        }

        public string Title { get; set; }

        public static async Task<ProposalModel> GetAsync(RepoInfo repo, string id)
        {
            var proposalsModel = await ProposalsModel.GetAsync(repo);
            return proposalsModel.Proposals?.FirstOrDefault(i => i.Id == id);
        }
    }
}
