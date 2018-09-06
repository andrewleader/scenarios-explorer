using ScenariosExplorer.ApplicationSettings;
using ScenariosExplorer.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ScenariosExplorer.Models
{
    public class MapModel
    {
        private RepoInfo m_repo;

        public List<MapScenarioModel> Scenarios { get; set; }

        public List<MapProposalModel> Proposals { get; set; }

        public static async Task<MapModel> GetAsync(RepoInfo repo)
        {
            var contentService = await ContentService.GetAsync(repo);
            var yamlString = await contentService.GetFileContentsAsync("map.yaml");

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .IgnoreUnmatchedProperties()
                .Build();

            var answer = deserializer.Deserialize<MapModel>(yamlString);
            answer.m_repo = repo;
            return answer;
        }

        public MapScenarioModel FindScenario(string id)
        {
            return FindScenario(Scenarios, id);
        }

        private static MapScenarioModel FindScenario(List<MapScenarioModel> scenarios, string id)
        {
            foreach (var s in scenarios)
            {
                if (s.Id == id)
                {
                    return s;
                }

                if (s.Children != null)
                {
                    var answer = FindScenario(s.Children, id);
                    if (answer != null)
                    {
                        return answer;
                    }
                }
            }

            return null;
        }

        public async Task SaveAsync()
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .Build();

            var contentService = await ContentService.GetAsync(m_repo);
            using (var stream = File.Open(contentService.GetFilePath("map.yaml"), FileMode.Create))
            {
                using (var streamWriter = new StreamWriter(stream))
                {
                    serializer.Serialize(streamWriter, this);
                }
            }
        }
    }

    public class MapScenarioModel
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Summary { get; set; }

        public List<MapScenarioModel> Children { get; set; }

        public List<MapProposalExampleModel> ProposalExamples { get; set; }
    }

    public class MapProposalExampleModel
    {
        public string ProposalId { get; set; }
    }

    public class MapProposalModel
    {
        public string Id { get; set; }

        public string Title { get; set; }
    }
}
