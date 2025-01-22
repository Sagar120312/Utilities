using SingleSignOnUtility.DTOs;

namespace SingleSignOnUtility.Operation.Interface
{
    public interface IAppleSSOService
    {
        string GetAuthorizationUrl(string clientId, Uri redirectUri, string state);
        Task<AppleUserProfile> AuthenticateAsync(string clientId, string clientSecret, Uri redirectUri, string code);
    }
}
