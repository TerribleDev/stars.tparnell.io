using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Octokit;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace stars.tparnell.io.Controllers
{
    public class ManageController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var token = this.HttpContext?.User?.Claims?.GithubTokenOrDefault();
            if(string.IsNullOrWhiteSpace(token))
            {
                return new ChallengeResult("GitHub", new Microsoft.AspNet.Http.Authentication.AuthenticationProperties() { RedirectUri = Url.Action("Manage") });
            }
            var client = new GitHubClient(new ProductHeaderValue("stars.tparnell.io"))
            {
                Credentials = new Credentials(token)
            };

            return View(await GetMissingStars(client));
        }

        public async Task<IActionResult> AddStars()
        {
            var token = this.HttpContext?.User?.Claims?.GithubTokenOrDefault();
            if(string.IsNullOrWhiteSpace(token))
            {
                return new ChallengeResult("GitHub", new Microsoft.AspNet.Http.Authentication.AuthenticationProperties() { RedirectUri = Url.Action("Manage") });
            }
            var client = new GitHubClient(new ProductHeaderValue("stars.tparnell.io"))
            {
                Credentials = new Credentials(token)
            };
            await StarRepos(client, await GetMissingStars(client));
            return this.RedirectToAction("Index");
        }

        private static Task StarRepos(GitHubClient client, IEnumerable<Repository> repos)
        {
            //TODO: Parallelize?
            return Task.WhenAll(repos.Select(a => client.Activity.Starring.StarRepo(a.Owner.Login, a.Name)));
        }

        private static async Task<IEnumerable<Repository>> GetMissingStars(GitHubClient client)
        {
            var currentUser = await client.User.Current();
            var repos = (await client.Repository.GetAllForCurrent()).Where(a => !a.Private).ToList();
            var stars = await client.Activity.Starring.GetAllForCurrent();
            return repos.Where(a => !stars.Any(b => string.Equals(a.Owner.Name, b.Owner.Name, StringComparison.OrdinalIgnoreCase) && string.Equals(a.FullName, b.FullName, StringComparison.OrdinalIgnoreCase)));
        }
    }
}