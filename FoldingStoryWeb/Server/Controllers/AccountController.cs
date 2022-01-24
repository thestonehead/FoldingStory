using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoldingStoryWeb.Server.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        [HttpGet("login")]
        public IActionResult GetLogin(string? returnUrl = null, [FromQuery]string? provider = null)
        {
         //   string provider = "Microsoft";

            // Request a redirect to the external login provider.
            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = returnUrl ?? "/",
            };
            return new ChallengeResult(provider, authenticationProperties);
        }

        [HttpGet("signin_microsoft")]
        public async Task<IActionResult> OnGetCallbackAsync()
        {
            var msUser = this.User.Identities.First();
            if (msUser.IsAuthenticated)
            {
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    RedirectUri = this.Request.Host.Value
                };
                await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(msUser),
                authProperties);
            }
            return LocalRedirect("/");
        }

        [HttpGet("externalSignIn")]
        public async Task<IActionResult> OnGetExternalSignInCallbackAsync()
        {
            var extUser = this.User.Identities.First();
            if (extUser.IsAuthenticated)
            {
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    RedirectUri = this.Request.Host.Value
                };
                await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(extUser),
                authProperties);
            }
            return LocalRedirect("/");
        }

        [HttpGet("identity")]
        public IEnumerable<KeyValuePair<string,string>>? GetIdentity()
        {
            if (this.User.Identity?.IsAuthenticated != true)
                return null;
            return this.User.Claims.Select(t => new KeyValuePair<string, string>(t.Type, t.Value));
        }

        [HttpGet("signOut")]
        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync();

            return LocalRedirect("/");
        }
    }
}
