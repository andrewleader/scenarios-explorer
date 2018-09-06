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
    public class ScenariosController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View(await ScenariosModel.GetAsync(AppSettings.DefaultGitHubRepo));
        }

        [Route("scenarios/{id}")]
        public async Task<IActionResult> ViewScenario(string id)
        {
            var scenario = await ScenarioModel.GetAsync(AppSettings.DefaultGitHubRepo, id);
            if (scenario == null)
            {
                return Redirect("/scenarios");
            }

            await scenario.GetContentsAsync();
            return View(scenario);
        }

        [Route("scenarios/{id}/edit")]
        public async Task<IActionResult> EditScenario(string id)
        {
            var scenario = await ScenarioModel.GetAsync(AppSettings.DefaultGitHubRepo, id);
            if (scenario == null)
            {
                return Redirect("/scenarios");
            }

            await scenario.GetContentsAsync();
            this.ViewBag.FormAction = "EditScenario";
            return View(scenario);
        }

        [Route("scenarios/{id}/edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditScenario(string id, ScenarioModel model)
        {
            var map = await MapModel.GetAsync(AppSettings.DefaultGitHubRepo);
            var mapScenario = map.FindScenario(id);
            if (mapScenario != null)
            {
                mapScenario.Title = model.Title;
                mapScenario.Summary = model.Summary;

                using (await LockingService.LockAsync())
                {
                    await map.SaveAsync();

                    await ScenarioModel.SaveContentsAsync(id, AppSettings.DefaultGitHubRepo, model.Contents);

                    await ChangesService.Get(AppSettings.DefaultGitHubRepo).PushChangesAsync();
                }
            }

            dynamic parameters = new ExpandoObject();
            parameters.id = id;
            return RedirectToAction(nameof(ViewScenario), parameters);
        }

        [Route("scenarios/{parentId}/addscenario")]
        public async Task<IActionResult> AddChildScenario(string parentId)
        {
            this.ViewBag.Adding = true;
            this.ViewBag.FormAction = "AddChildScenario";

            return View("EditScenario", new ScenarioModel());
        }

        [Route("scenarios/{parentId}/addscenario")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddChildScenario(string parentId, ScenarioModel model)
        {
            var map = await MapModel.GetAsync(AppSettings.DefaultGitHubRepo);
            var parent = map.FindScenario(parentId);
            if (parent != null)
            {
                if (parent.Children == null)
                {
                    parent.Children = new List<MapScenarioModel>();
                }

                var newScenario = new MapScenarioModel()
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = model.Title,
                    Summary = model.Summary
                };

                parent.Children.Add(newScenario);

                using (await LockingService.LockAsync())
                {
                    await map.SaveAsync();

                    await ScenarioModel.SaveContentsAsync(newScenario.Id, AppSettings.DefaultGitHubRepo, model.Contents);

                    await ChangesService.Get(AppSettings.DefaultGitHubRepo).PushChangesAsync();
                }

                dynamic parameters = new ExpandoObject();
                parameters.id = newScenario.Id;
                return RedirectToAction(nameof(ViewScenario), parameters);
            }

            dynamic failedParameters = new ExpandoObject();
            failedParameters.id = parentId;
            return RedirectToAction(nameof(ViewScenario), failedParameters);
        }
    }
}