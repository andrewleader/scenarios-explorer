﻿using LibGit2Sharp;
using LibGit2Sharp.Handlers;
using ScenariosExplorer.ApplicationSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScenariosExplorer.Services
{
    public class ChangesService
    {
        public RepoInfo Repo { get; private set; }

        private ChangesService(RepoInfo repo)
        {
            Repo = repo;
        }

        public static ChangesService Get(RepoInfo repo)
        {
            return new ChangesService(repo);
        }

        public async Task PushChangesAsync()
        {
            try
            {
                using (var repository = new Repository(ContentService.GetRepoFolder(Repo)))
                {
                    var status = repository.RetrieveStatus();
                    var filePaths = status.Modified.Select(i => i.FilePath).Union(status.Untracked.Select(i => i.FilePath)).ToList();
                    if (!filePaths.Any())
                    {
                        return;
                    }
                    foreach (var p in filePaths)
                    {
                        repository.Index.Add(p);
                    }

                    repository.Commit("Changes", new Signature("scenarios-explorer", "scenarios-explorer@microsoft.com", DateTime.UtcNow), new Signature("scenarios-explorer", "scenarios-explorer@microsoft.com", DateTime.UtcNow));

                    var remote = repository.Network.Remotes["origin"];
                    repository.Network.Push(remote, pushRefSpec: "refs/heads/master", pushOptions: new PushOptions()
                    {
                        CredentialsProvider = new CredentialsHandler(CredentialsProvider)
                    });
                }
            }
            catch (LibGit2Sharp.NonFastForwardException)
            {
                // Means server has newer commits. Should be a niche case, just delete and drop their changes.
                await ContentService.DeleteCacheAsync(Repo);
            }
        }

        public static Credentials CredentialsProvider(string url, string urlFromUsername, SupportedCredentialTypes supportedCredentialTypes)
        {
            return new UsernamePasswordCredentials()
            {
                Username = "aleader@microsoft.com",
                Password = Startup.PersonalAccessToken
            };
        }
    }
}
