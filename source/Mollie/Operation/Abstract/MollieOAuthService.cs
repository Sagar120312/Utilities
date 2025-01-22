using Mollie.DTOs;
using Mollie.DTOs.Enum;
using Mollie.Operation.Interface;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace Mollie.Operation.Abstract
{
    public class MollieOAuthService : IMollieOAuthService
    {
        private static readonly HttpClient client = new HttpClient();
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _redirectUri;

        public MollieOAuthService(string clientId, string clientSecret, string redirectUri)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
            _redirectUri = redirectUri;
        }
        public string GenerateOAuthUrl(string state, string userScope)
        {
            try
            {
                if (string.IsNullOrEmpty(_clientId))
                {
                    throw new ArgumentException("Client ID cannot be null or empty.", nameof(_clientId));
                }

                if (string.IsNullOrEmpty(state))
                {
                    throw new ArgumentException("State cannot be null or empty.", nameof(state));
                }

                var baseUrl = "https://www.mollie.com/oauth2/authorize";
                var responseType = "code";

                // Use user-provided scope or default scope if not provided
                var scope = string.IsNullOrWhiteSpace(userScope)
                    ? "customers.read customers.write mandates.read mandates.write " +
                      "subscriptions.read subscriptions.write profiles.read profiles.write " +
                      "payments.read payments.write refunds.read refunds.write"
                    : userScope;

                // Construct the query string
                var query = HttpUtility.ParseQueryString(string.Empty);
                query["client_id"] = _clientId;
                query["response_type"] = responseType;
                query["scope"] = scope;
                query["state"] = state;

                return $"{baseUrl}?{query}";
            }
            catch (Exception ex)
            {
                // Log or handle the error as required
                Console.WriteLine($"Error generating OAuth URL: {ex.Message}");
                throw new InvalidOperationException("Failed to generate OAuth URL.", ex);
            }
        }
        public async Task<ResponseDto> GetAccessToken(string refreshToken, AuthDataDto authData, OAuthAccessGrantType grantType = OAuthAccessGrantType.authorization_code)
        {
            ResponseDto response = new ResponseDto();
            try
            {
                HttpClient client = new HttpClient();

                if (grantType != OAuthAccessGrantType.authorization_code && string.IsNullOrEmpty(refreshToken))
                {
                    response.Message = "Auth token missing";
                    response.Success = false;
                    return response;
                }
                // Prepare the Basic Authorization Header
                var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                string jsonBody = "";
                // Prepare the body parameters
                if (grantType == OAuthAccessGrantType.authorization_code)
                {
                    var requestBody = new
                    {
                        grant_type = "authorization_code",
                        code = authData.AuthCode,
                        redirect_uri = authData.RedirectUri
                    };
                    jsonBody = JsonConvert.SerializeObject(requestBody);

                }
                else
                {
                    var requestBody = new
                    {
                        grant_type = "refresh_token",
                        refresh_token = refreshToken,
                        redirect_uri = _redirectUri
                    };
                    jsonBody = JsonConvert.SerializeObject(requestBody);

                }

                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                // Make the POST request
                var responseData = await client.PostAsync("https://api.mollie.com/oauth2/tokens", content);

                if (responseData.IsSuccessStatusCode)
                {
                    var responseContent = await responseData.Content.ReadAsStringAsync();
                    var tokenResponse = JsonConvert.DeserializeObject<MollieTokenResponseDto>(responseContent);
                    response.Success = true;
                    if (grantType == OAuthAccessGrantType.refresh_token)// Return the access token JSON response
                    {
                        response.Data = tokenResponse;
                    }
                }
                else
                {
                    var errorResponse = await responseData.Content.ReadAsStringAsync();
                    response.Message = $"Error exchanging authorization code: {responseData.StatusCode} - {errorResponse}";
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Success = false;
            }
            return response;
        }
    }
}
