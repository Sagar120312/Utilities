using SingleSignOnUtility.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingleSignOnUtility.Operation.Interface
{
    public interface IFacebookSSOService
    {
        Task<string> GetAuthorizationUrlAsync();
        Task<string> ExchangeCodeForTokenAsync(string code);
        Task<FacebookUserProfile> GetUserInfoAsync(string accessToken);
    }
}
