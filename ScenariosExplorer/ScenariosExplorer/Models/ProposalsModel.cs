using ScenariosExplorer.ApplicationSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScenariosExplorer.Models
{
    public class ProposalsModel
    {
        public List<ProposalModel> Proposals { get; set; } = new List<ProposalModel>();

        public static async Task<ProposalsModel> GetAsync(RepoInfo repo)
        {
            var mapModel = await MapModel.GetAsync(repo);

            var answer = new ProposalsModel();

            if (mapModel.Proposals != null)
            {
                foreach (var proposal in mapModel.Proposals)
                {
                    answer.Proposals.Add(new ProposalModel(proposal.Id, repo)
                    {
                        Title = proposal.Title
                    });
                }
            }

            return answer;
        }
    }
}
