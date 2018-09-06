using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ScenariosExplorer.ApplicationSettings;
using ScenariosExplorer.Models;
using ScenariosExplorer.Services;

namespace ScenariosExplorer.Controllers
{
    public class ProposalsController : Controller
    {
        [Route("proposals")]
        public async Task<IActionResult> ViewProposals()
        {
            return View(await ProposalsModel.GetAsync(AppSettings.DefaultGitHubRepo));
        }

        [Route("proposals/{id}")]
        public async Task<IActionResult> ViewProposal(string id)
        {
            var proposal = await ProposalModel.GetAsync(AppSettings.DefaultGitHubRepo, id);
            if (proposal == null)
            {
                return Redirect("/proposals");
            }

            await proposal.GetContentsAsync();
            return View(proposal);
        }

        [Route("proposals/create")]
        public async Task<IActionResult> CreateProposal()
        {
            this.ViewBag.Adding = true;
            this.ViewBag.FormAction = "CreateProposal";
            return View("EditProposal", new ProposalModel());
        }

        [Route("proposals/create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProposal(ProposalModel model)
        {
            var map = await MapModel.GetAsync(AppSettings.DefaultGitHubRepo);

            var id = Guid.NewGuid().ToString();
            if (map.Proposals == null)
            {
                map.Proposals = new List<MapProposalModel>();
            }
            map.Proposals.Add(new MapProposalModel()
            {
                Id = id,
                Title = model.Title
            });

            using (await LockingService.LockAsync())
            {
                await map.SaveAsync();

                await BaseContentsModel.SaveContentsAsync(id, AppSettings.DefaultGitHubRepo, model.Contents);

                await ChangesService.Get(AppSettings.DefaultGitHubRepo).PushChangesAsync();
            }

            dynamic parameters = new ExpandoObject();
            parameters.id = id;
            return RedirectToAction(nameof(ViewProposal), parameters);
        }

        [Route("proposals/{id}/edit")]
        public async Task<IActionResult> EditProposal(string id)
        {
            var proposal = await ProposalModel.GetAsync(AppSettings.DefaultGitHubRepo, id);
            if (proposal == null)
            {
                return Redirect("/proposals");
            }

            await proposal.GetContentsAsync();

            this.ViewBag.FormAction = "EditProposal";
            return View("EditProposal", proposal);
        }

        [Route("proposals/{id}/edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProposal(string id, ProposalModel model)
        {
            var map = await MapModel.GetAsync(AppSettings.DefaultGitHubRepo);

            var existing = map.Proposals?.FirstOrDefault(i => i.Id == id);
            if (existing != null)
            {
                existing.Title = model.Title;

                using (await LockingService.LockAsync())
                {
                    await map.SaveAsync();

                    await BaseContentsModel.SaveContentsAsync(id, AppSettings.DefaultGitHubRepo, model.Contents);

                    await ChangesService.Get(AppSettings.DefaultGitHubRepo).PushChangesAsync();
                }
            }

            dynamic parameters = new ExpandoObject();
            parameters.id = id;
            return RedirectToAction(nameof(ViewProposal), parameters);
        }

        [Route("scenarios/{scenarioId}/proposals/{proposalId}")]
        public async Task<IActionResult> ViewProposalExample(string scenarioId, string proposalId)
        {
            var scenario = await ScenarioModel.GetAsync(AppSettings.DefaultGitHubRepo, scenarioId);
            if (scenario == null)
            {
                return RedirectToAction(nameof(ScenariosController.Index), nameof(ScenariosController));
            }

            var proposalExample = scenario.Proposals?.FirstOrDefault(i => i.Info.Id == proposalId);
            if (proposalExample == null)
            {
                return RedirectToAction(nameof(ScenariosController.Index), nameof(ScenariosController));
            }

            await scenario.GetContentsAsync();
            await proposalExample.GetContentsAsync();
            return View(proposalExample);
        }

        [Route("scenarios/{scenarioId}/addproposalexample")]
        public async Task<IActionResult> AddProposalExample(string scenarioId)
        {
            var map = await MapModel.GetAsync(AppSettings.DefaultGitHubRepo);

            var model = new AddProposalExampleModel();

            model.Scenario = await ScenarioModel.GetAsync(AppSettings.DefaultGitHubRepo, scenarioId);
            if (model.Scenario == null)
            {
                return RedirectToAction(controllerName: nameof(ScenariosController), actionName: nameof(ScenariosController.Index));
            }
            await model.Scenario.GetContentsAsync();

            model.AvailableProposals = (map.Proposals ?? new List<MapProposalModel>()).Where(i => !model.Scenario.Proposals.Exists(x => x.ProposalId == i.Id)).ToList();

            this.ViewBag.Adding = true;
            this.ViewBag.FormAction = "AddProposalExample";

            return View("EditProposalExample", model);
        }

        [Route("scenarios/{scenarioId}/addproposalexample")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProposalExample(string scenarioId, AddProposalExampleModel model)
        {
            if (string.IsNullOrWhiteSpace(model.ProposalId))
            {
                throw new Exception("You must pick a proposal");
            }

            var map = await MapModel.GetAsync(AppSettings.DefaultGitHubRepo);
            var parent = map.FindScenario(scenarioId);
            if (parent != null)
            {
                if (parent.ProposalExamples == null)
                {
                    parent.ProposalExamples = new List<MapProposalExampleModel>();
                }

                var newProposal = new MapProposalExampleModel()
                {
                    ProposalId = model.ProposalId
                };

                parent.ProposalExamples.Add(newProposal);

                using (await LockingService.LockAsync())
                {
                    await map.SaveAsync();

                    await BaseContentsModel.SaveContentsAsync(parent.Id + "-" + model.ProposalId, AppSettings.DefaultGitHubRepo, model.Contents);

                    await ChangesService.Get(AppSettings.DefaultGitHubRepo).PushChangesAsync();
                }
            }

            dynamic parameters = new ExpandoObject();
            parameters.scenarioId = scenarioId;
            parameters.proposalId = model.ProposalId;
            return RedirectToAction(actionName: nameof(ViewProposalExample), routeValues: parameters);
        }

        [Route("scenarios/{scenarioId}/editproposalexample/{proposalId}")]
        public async Task<IActionResult> EditProposalExample(string scenarioId, string proposalId)
        {
            var scenario = await ScenarioModel.GetAsync(AppSettings.DefaultGitHubRepo, scenarioId);
            if (scenario == null)
            {
                return NotFound();
            }

            var proposalExample = scenario.Proposals?.FirstOrDefault(i => i.ProposalId == proposalId);
            if (proposalExample == null)
            {
                return NotFound();
            }

            await scenario.GetContentsAsync();
            await proposalExample.GetContentsAsync();

            var model = new AddProposalExampleModel();

            model.Scenario = scenario;
            model.ProposalId = proposalId;
            model.Contents = proposalExample.Contents;

            this.ViewBag.FormAction = "EditProposalExample";

            return View("EditProposalExample", model);
        }

        [Route("scenarios/{scenarioId}/editproposalexample/{proposalId}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProposalExample(string scenarioId, string proposalId, AddProposalExampleModel model)
        {
            var scenario = await ScenarioModel.GetAsync(AppSettings.DefaultGitHubRepo, scenarioId);
            if (scenario == null)
            {
                return NotFound();
            }

            var proposalExample = scenario.Proposals?.FirstOrDefault(i => i.ProposalId == proposalId);
            if (proposalExample == null)
            {
                return NotFound();
            }

            using (await LockingService.LockAsync())
            {
                await BaseContentsModel.SaveContentsAsync(proposalExample.Id, AppSettings.DefaultGitHubRepo, model.Contents);

                await ChangesService.Get(AppSettings.DefaultGitHubRepo).PushChangesAsync();
            }

            dynamic parameters = new ExpandoObject();
            parameters.scenarioId = scenarioId;
            parameters.proposalId = proposalId;
            return RedirectToAction(actionName: nameof(ViewProposalExample), routeValues: parameters);
        }
    }
}