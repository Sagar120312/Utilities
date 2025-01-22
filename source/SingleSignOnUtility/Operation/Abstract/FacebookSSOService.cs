using SingleSignOnUtility.DTOs;
using SingleSignOnUtility.Operation.Interface;
using System.Net.Http.Json;
using System.Text.Json;

namespace SingleSignOnUtility.Operation.Abstract
{
    public abstract class FacebookSSOService : IFacebookSSOService
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _redirectUri;

        protected FacebookSSOService(string clientId, string clientSecret, string redirectUri)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
            _redirectUri = redirectUri;
        }

        public Task<string> GetAuthorizationUrlAsync()
        {
            var url = $"https://www.facebook.com/v11.0/dialog/oauth" +
                      $"?client_id={_clientId}" +
                      $"&redirect_uri={_redirectUri}" +
                      $"&scope=email,public_profile";

            return Task.FromResult(url);
        }

        public async Task<string> ExchangeCodeForTokenAsync(string authorizationCode)
        {
            var httpClient = new HttpClient();
            var tokenUrl = $"https://graph.facebook.com/v11.0/oauth/access_token" +
                           $"?client_id={_clientId}" +
                           $"&redirect_uri={_redirectUri}" +
                           $"&client_secret={_clientSecret}" +
                           $"&code={authorizationCode}";

            var response = await httpClient.GetAsync(tokenUrl);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadFromJsonAsync<JsonElement>();
            return responseContent.GetProperty("access_token").GetString();
        }

        public async Task<FacebookUserProfile> GetUserInfoAsync(string accessToken)
        {
            using var httpClient = new HttpClient();

            var userInfoEndpoint = $"https://graph.facebook.com/me?fields=id,name,email,locale,picture&access_token={accessToken}";

            var response = await httpClient.GetAsync(userInfoEndpoint);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadFromJsonAsync<JsonElement>();

            var userInfo = new FacebookUserProfile
            {
                Id = responseContent.GetProperty("id").GetString(),
                Name = responseContent.GetProperty("name").GetString(),
                Email = responseContent.TryGetProperty("email", out var emailProp) ? emailProp.GetString() : null,
                Locale = responseContent.GetProperty("locale").GetString(),
                ProfilePictureUrl = responseContent.GetProperty("picture").GetProperty("data").GetProperty("url").GetString()
            };

            return userInfo;
        }
    }
}
