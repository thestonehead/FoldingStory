using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;

namespace FoldingStoryWeb.Client.AuthProviders
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient httpClient;

        public AuthStateProvider(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var response = await httpClient.GetAsync("/api/account/identity");

            if (response?.IsSuccessStatusCode != true || response.Content.Headers.ContentLength == 0)
            {
                var problem = await response.Content.ReadAsStringAsync();
                var anonymous = new ClaimsIdentity();
                return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(anonymous)));
            }
            //var stuff = await response.Content.ReadAsStringAsync();


            using var streamResponse = await response.Content.ReadAsStreamAsync();
            var claims = JsonSerializer.Deserialize<ClaimDto[]>(streamResponse);
            var claimsIdentity = new ClaimsIdentity("Server");
            foreach (var c in claims)
            {
                claimsIdentity.AddClaim(new Claim(c.Key, c.Value));
            }
            return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(claimsIdentity)));
        }

        class ClaimDto
        {
            [System.Text.Json.Serialization.JsonPropertyName("key")]
            public string Key { get; set; }
            [System.Text.Json.Serialization.JsonPropertyName("value")]
            public string Value { get; set; }
        }
    }
}
