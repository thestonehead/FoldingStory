using Microsoft.AspNetCore.Mvc;

namespace FoldingStoryWeb.Server.Infrastructure
{
    public static class AuthExtensions
    {

        public static string? GetUserId(this ControllerBase controller)
        {
            return controller.User.Claims.FirstOrDefault(t => t.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        }

        public static string? GetUsername(this ControllerBase controller)
        {
            return controller.User.Claims.FirstOrDefault(t => t.Type == System.Security.Claims.ClaimTypes.Name)?.Value;
        }
    }
}
