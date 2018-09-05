using ScenariosExplorer.ApplicationSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScenariosExplorer.Models
{
    public class ScenarioModel : BaseContentsModel
    {
        public ScenarioModel(MapScenarioModel source, GitHubRepo repo, ScenarioModel parent)
            : base(source.Id, repo)
        {
            Title = source.Title;
            Summary = source.Summary;
            Parent = parent;

            if (source.Children != null)
            {
                foreach (var childScenario in source.Children)
                {
                    ChildrenScenarios.Add(new ScenarioModel(childScenario, repo, this));
                }
            }

            if (source.Proposals != null)
            {
                foreach (var proposal in source.Proposals)
                {
                    Proposals.Add(new ProposalModel(proposal, repo));
                }
            }
        }

        public string Title { get; set; }

        public string Summary { get; set; }

        public List<ScenarioModel> ChildrenScenarios { get; set; } = new List<ScenarioModel>();

        public List<ProposalModel> Proposals { get; set; } = new List<ProposalModel>();

        public ScenarioModel Parent { get; set; }

        public List<ScenarioModel> GetAncestors()
        {
            var answer = new List<ScenarioModel>();

            var ancestor = Parent;
            while (ancestor != null)
            {
                answer.Insert(0, ancestor);
                ancestor = ancestor.Parent;
            }

            return answer;
        }

        public ScenarioModel FindScenario(string id)
        {
            if (Id == id)
            {
                return this;
            }

            foreach (var child in ChildrenScenarios)
            {
                var answer = child.FindScenario(id);
                if (answer != null)
                {
                    return answer;
                }
            }

            return null;
        }

        public static async Task<ScenarioModel> GetAsync(GitHubRepo repo, string id)
        {
            var scenariosModel = await ScenariosModel.GetAsync(repo);
            foreach (var s in scenariosModel.Scenarios)
            {
                var answer = s.FindScenario(id);
                if (answer != null)
                {
                    return answer;
                }
            }

            return null;
        }
    }
}
