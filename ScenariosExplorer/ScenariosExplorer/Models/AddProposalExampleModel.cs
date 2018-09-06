using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScenariosExplorer.Models
{
    public class AddProposalExampleModel
    {
        public ScenarioModel Scenario { get; set; }

        public List<MapProposalModel> AvailableProposals { get; set; }

        public string ProposalId { get; set; }

        public string Contents { get; set; }
    }
}
