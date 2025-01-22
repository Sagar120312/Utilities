using SingleSignOnUtility.DTOs;
using SingleSignOnUtility.Operation.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text.Json;

namespace SingleSignOnUtility.Operation.Abstract
{
    public abstract class AppleSSOService : IAppleSSOService
    {
        private const string AuthorizationEndpoint = "https://appleid.apple.com/auth/authorize";
        private const string TokenEndpoint = "https://appleid.apple.com/auth/token";

        public string GetAuthorizationUrl(string clientId, Uri redirectUri, string state)
        {
            var scopes = "openid email name";
            var queryParams = new Dictionary<string, string>
            {
                { "response_type", "code id_token" },
                { "response_mode", "form_post" },
                { "client_id", clientId },
                { "redirect_uri", redirectUri.ToString() },
                { "scope", scopes },
                { "state", state },
                { "nonce", GenerateNonce() } // Nonce for ID token validation
            };

            var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
            return $"{AuthorizationEndpoint}?{queryString}";
        }

        public async Task<AppleUserProfile> AuthenticateAsync(string clientId, string clientSecret, Uri redirectUri, string code)
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

        private AppleUserProfile DecodeIdToken(string idToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(idToken);

            return new AppleUserProfile
            {
                Name = token.Claims.FirstOrDefault(c => c.Type == "name")?.Value,
                Email = token.Claims.FirstOrDefault(c => c.Type == "email")?.Value,
                Locale = token.Claims.FirstOrDefault(c => c.Type == "locale")?.Value
            };
        }

        private string GenerateNonce()
        {
            using var rng = new RNGCryptoServiceProvider();
            var nonceBytes = new byte[32];
            rng.GetBytes(nonceBytes);
            return Convert.ToBase64String(nonceBytes);
        }

        private class TokenResponse
        {
            public string IdToken { get; set; }
            public string AccessToken { get; set; }
        }
    }

}
