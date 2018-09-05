using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScenariosExplorer.ApplicationSettings
{
    public class GitHubRepo : IEquatable<GitHubRepo>
    {
        public string Owner { get; set; }
        public string RepositoryName { get; set; }
        public string Branch { get; set; }

        public bool Equals(GitHubRepo other)
        {
            return Owner.Equals(other.Owner) && RepositoryName.Equals(other.RepositoryName) && object.Equals(Branch, other.Branch);
        }
    }
}
