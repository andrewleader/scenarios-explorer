using ScenariosExplorer.ApplicationSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScenariosExplorer.Models
{
    public class ScenariosModel
    {
        public List<ScenarioModel> Scenarios { get; } = new List<ScenarioModel>();

        public static async Task<ScenariosModel> GetAsync(RepoInfo repo)
        {
            var mapModel = await MapModel.GetAsync(repo);

            var answer = new ScenariosModel();

            foreach (var scenario in mapModel.Scenarios)
            {
                answer.Scenarios.Add(new ScenarioModel(scenario, mapModel, repo, null));
            }

            return answer;
        }
    }
}
