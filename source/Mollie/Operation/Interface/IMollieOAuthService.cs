using Mollie.DTOs;
using Mollie.DTOs.Enum;

namespace Mollie.Operation.Interface
{
    public interface IMollieOAuthService
    {
        string GenerateOAuthUrl(string state, string userScope);
        Task<ResponseDto> GetAccessToken(string refreshToken, AuthDataDto authData, OAuthAccessGrantType grantType);
    }
}
