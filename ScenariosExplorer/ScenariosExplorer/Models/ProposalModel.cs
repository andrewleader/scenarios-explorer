using ScenariosExplorer.ApplicationSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScenariosExplorer.Models
{
    public class ProposalModel : BaseContentsModel
    {
        public ProposalModel(MapProposalModel source, GitHubRepo repo)
            : base(source.Id, repo)
        {
            Title = source.Title;
        }

        public string Title { get; set; }
    }
}
