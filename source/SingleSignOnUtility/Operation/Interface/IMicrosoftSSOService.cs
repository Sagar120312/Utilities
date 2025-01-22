using SingleSignOnUtility.DTOs;

namespace SingleSignOnUtility.Operation.Interface
{
    public interface IMicrosoftSSOService
    {
        string GetAuthorizationUrl(string clientId, Uri redirectUri, string state);
        Task<MicrosoftUserProfile> AuthenticateAsync(string clientId, string clientSecret, Uri redirectUri, string code);
    }
}
