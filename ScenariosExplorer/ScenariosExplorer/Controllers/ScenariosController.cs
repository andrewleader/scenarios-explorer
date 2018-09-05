using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ScenariosExplorer.ApplicationSettings;
using ScenariosExplorer.Models;

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
            return View(scenario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IAsyncResult> EditScenario(string id, ScenarioModel model)
        {
            return Redirect("/scenarios");
        }
    }
}