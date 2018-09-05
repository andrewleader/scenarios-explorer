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
        public List<MapScenarioModel> Scenarios { get; set; }

        public static async Task<MapModel> GetAsync(GitHubRepo repo)
        {
            var contentService = await ContentService.GetAsync(repo);
            var yamlString = await contentService.GetFileContentsAsync("map.yaml");

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .Build();

            return deserializer.Deserialize<MapModel>(yamlString);
        }
    }

    public class MapScenarioModel
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Summary { get; set; }

        public List<MapScenarioModel> Children { get; set; }

        public List<MapProposalModel> Proposals { get; set; }
    }

    public class MapProposalModel
    {
        public string Id { get; set; }

        public string Title { get; set; }
    }
}
