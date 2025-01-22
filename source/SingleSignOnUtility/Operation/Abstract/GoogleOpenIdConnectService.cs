using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SingleSignOnUtility.DTOs;
using SingleSignOnUtility.Operation.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace SingleSignOnUtility.Operation.Abstract
{
    public class GoogleOpenIdConnectService : IGoogleOpenIdConnectService
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly Uri _redirectUri;
        private readonly string _issuer = "https://accounts.google.com";

        public GoogleOpenIdConnectService(string clientId, Uri redirectUri)
        {
            _clientId = clientId;
            //_clientSecret = clientSecret;
            _redirectUri = redirectUri;
        }

        /// <summary>
        /// Generates the Google authentication URL.
        /// </summary>
        public string GenerateAuthenticationUrl(out string state,out string nonce)
        {
            state = GenerateState();
            nonce = GenerateNonce();

            var authUrl = $"https://accounts.google.com/o/oauth2/v2/auth?" +
                          $"client_id={Uri.EscapeDataString(_clientId)}&" +
                          $"redirect_uri={Uri.EscapeDataString(_redirectUri.ToString())}&" +
                          $"response_type=id_token&" +
                          $"scope=openid%20profile%20email&" +
                          $"state={Uri.EscapeDataString(state)}&" +
                          $"nonce={Uri.EscapeDataString(nonce)}";

            return authUrl;
        }

        /// <summary>
        /// Validates and extracts user details from the ID token.
        /// </summary>
        public async Task<UserProfile> ValidateAndDecodeIdToken(string idToken, string nonce)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(idToken);

            // Validate token issuer, audience, and nonce
            var issuer = token.Issuer;
            var audience = token.Audiences.FirstOrDefault();
            var tokenNonce = token.Claims.FirstOrDefault(c => c.Type == "nonce")?.Value;

            if (issuer != _issuer || audience != _clientId || tokenNonce != nonce)
            {
                throw new InvalidOperationException("Invalid ID token.");
            }

            // Extract user information
            var email = token.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
            var name = token.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            var picture = token.Claims.FirstOrDefault(c => c.Type == "picture")?.Value;
            var locale = token.Claims.FirstOrDefault(c => c.Type == "locale")?.Value;

            return new UserProfile
            {
                Name = name,
                Email = email,
                Picture = picture,
                Locale = locale
            };
        }

        /// <summary>
        /// Generates a cryptographically random state.
        /// </summary>
        private static string GenerateState() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

        /// <summary>
        /// Generates a cryptographically random nonce.
        /// </summary>
        private static string GenerateNonce() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    }
}
