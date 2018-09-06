using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ScenariosExplorer.ApplicationSettings
{
    public class RepoInfo : IEquatable<RepoInfo>
    {
        public string Url { get; set; }
        public string Branch { get; set; }

        private string m_urlHash;
        public string UrlHash()
        {
            if (m_urlHash == null)
            {
                using (var sha1 = new SHA1Managed())
                {
                    var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(Url));
                    var sb = new StringBuilder(hash.Length * 2);

                    foreach (byte b in hash)
                    {
                        // can be "x2" if you want lowercase
                        sb.Append(b.ToString("X2"));
                    }

                    m_urlHash = sb.ToString();
                }
            }

            return m_urlHash;
        }

        public bool Equals(RepoInfo other)
        {
            return Url.Equals(other.Url) && object.Equals(Branch, other.Branch);
        }
    }
}
