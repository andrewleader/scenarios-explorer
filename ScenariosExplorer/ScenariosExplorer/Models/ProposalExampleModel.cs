using ScenariosExplorer.ApplicationSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScenariosExplorer.Models
{
    public class ProposalExampleModel : BaseContentsModel
    {
        /// <summary>
        /// Used for editing from ASP.NET
        /// </summary>
        public ProposalExampleModel() : base(null, null) { }

        public ProposalExampleModel(ScenarioModel parentScenario, MapProposalExampleModel source, MapModel map, RepoInfo repo)
            : base(parentScenario.Id + "-" + source.ProposalId, repo)
        {
            ParentScenario = parentScenario;
            ProposalId = source.ProposalId;
            Info = map.Proposals?.FirstOrDefault(i => i.Id == source.ProposalId);
        }

        public string ProposalId { get; set; }

        public MapProposalModel Info { get; set; }

        public ScenarioModel ParentScenario { get; set; }
    }
}
