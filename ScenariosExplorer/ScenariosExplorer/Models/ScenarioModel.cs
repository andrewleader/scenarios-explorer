using ScenariosExplorer.ApplicationSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScenariosExplorer.Models
{
    public class ScenarioModel : BaseContentsModel
    {
        /// <summary>
        /// For editing purposes
        /// </summary>
        public ScenarioModel() : base(null, null) { }

        public ScenarioModel(MapScenarioModel source, MapModel map, RepoInfo repo, ScenarioModel parent)
            : base(source.Id, repo)
        {
            Title = source.Title;
            Summary = source.Summary;
            Parent = parent;

            if (source.Children != null)
            {
                foreach (var childScenario in source.Children)
                {
                    ChildrenScenarios.Add(new ScenarioModel(childScenario, map, repo, this));
                }
            }

            if (source.ProposalExamples != null)
            {
                foreach (var proposal in source.ProposalExamples)
                {
                    var p = new ProposalExampleModel(this, proposal, map, repo);
                    if (p.Info != null)
                    {
                        // Don't allow duplicate proposals
                        if (!Proposals.Any(i => i.ProposalId == p.ProposalId))
                        {
                            Proposals.Add(p);
                        }
                    }
                }
            }
        }

        public string Title { get; set; }

        public string Summary { get; set; }

        public List<ScenarioModel> ChildrenScenarios { get; set; } = new List<ScenarioModel>();

        public List<ProposalExampleModel> Proposals { get; set; } = new List<ProposalExampleModel>();

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

        public static async Task<ScenarioModel> GetAsync(RepoInfo repo, string id)
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
