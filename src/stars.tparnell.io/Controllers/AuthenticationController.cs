using System.Threading.Tasks;
using Microsoft.AspNet.Authentication.Cookies;
using Microsoft.AspNet.Http.Authentication;
using Microsoft.AspNet.Mvc;

namespace stars.tparnell.io.Controllers
{
    public class AuthenticationController : Controller
    {
        [HttpGet("~/signin")]
        [HttpGet("~/")]
        public IActionResult SignIn()
        {
            if(this.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }
            return View("SignIn", HttpContext.GetExternalProviders());
        }

        [HttpPost("~/signin")]
        [HttpPost("~/")]
        public IActionResult SignIn([FromForm] string provider, string ReturnUrl)
        {
            // Note: the "provider" parameter corresponds to the external
            // authentication provider choosen by the user agent.

            if(string.IsNullOrWhiteSpace(provider) || !HttpContext.IsProviderSupported(provider))
            {
                return HttpBadRequest();
            }

            // Instruct the middleware corresponding to the requested external identity
            // provider to redirect the user agent to its own authorization endpoint.
            // Note: the authenticationScheme parameter must match the value configured in Startup.cs
            return new ChallengeResult(provider, new AuthenticationProperties { RedirectUri = string.IsNullOrWhiteSpace(ReturnUrl) ? "~/Manage" : ReturnUrl });
        }

        [HttpGet("~/signout")]
        [HttpPost("~/signout")]
        public async Task<IActionResult> SignOut()
        {
            // Instruct the cookies middleware to delete the local cookie created
            // when the user agent is redirected from the external identity provider
            // after a successful authentication flow (e.g Google or Facebook).
            await HttpContext.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("SignIn");
        }
    }
}