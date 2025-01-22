using SingleSignOnUtility.DTOs;
using SingleSignOnUtility.Operation.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

namespace SingleSignOnUtility.Operation.Abstract
{
    public abstract class MicrosoftSSOService : IMicrosoftSSOService
    {
        private const string AuthorizationEndpoint = "https://login.microsoftonline.com/common/oauth2/v2.0/authorize";
        private const string TokenEndpoint = "https://login.microsoftonline.com/common/oauth2/v2.0/token";

        public string GetAuthorizationUrl(string clientId, Uri redirectUri, string state)
        {
            var scopes = "openid profile email";
            var queryParams = new Dictionary<string, string>
            {
                { "client_id", clientId },
                { "response_type", "code" },
                { "redirect_uri", redirectUri.ToString() },
                { "response_mode", "query" },
                { "scope", scopes },
                { "state", state }
            };

            var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
            return $"{AuthorizationEndpoint}?{queryString}";
        }

        public async Task<MicrosoftUserProfile> AuthenticateAsync(string clientId, string clientSecret, Uri redirectUri, string code)
        {
            var tokenResponse = await ExchangeCodeForTokensAsync(clientId, clientSecret, redirectUri, code);

            // Decode the ID token to extract user info
            var userInfo = DecodeIdToken(tokenResponse.IdToken);

            return userInfo;
        }

        private async Task<TokenResponse> ExchangeCodeForTokensAsync(string clientId, string clientSecret, Uri redirectUri, string code)
        {
            using var httpClient = new HttpClient();
            var requestContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "redirect_uri", redirectUri.ToString() },
                { "code", code },
                { "grant_type", "authorization_code" }
            });

            var response = await httpClient.PostAsync(TokenEndpoint, requestContent);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TokenResponse>(responseContent);
        }

        private MicrosoftUserProfile DecodeIdToken(string idToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(idToken);

            return new MicrosoftUserProfile
            {
                Username = token.Claims.FirstOrDefault(c => c.Type == "name")?.Value,
                Email = token.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value,
                Locale = token.Claims.FirstOrDefault(c => c.Type == "locale")?.Value
            };
        }

        private class TokenResponse
        {
            public string IdToken { get; set; }
            public string AccessToken { get; set; }
        }
    }
}
