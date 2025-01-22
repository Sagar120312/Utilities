using Microsoft.IdentityModel.Tokens;
using PdfUtility.Operation.Abstract;
using PdfUtility.Operation.Interface;
using SingleSignOnUtility.Operation.Abstract;
using System;
using System.IO;

namespace PdfUtilityTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var clientId = "<your-client-id>";
                var clientSecret = "<your-client-secret>";
                var redirectUri = new Uri("http://localhost:4200/");

                var utility = new GoogleOpenIdConnectService(clientId, redirectUri);
                string state, nonce;
                var authUrl = utility.GenerateAuthenticationUrl(out state, out nonce);
                Console.WriteLine("-----------------Auth Url : " + authUrl);
                Console.WriteLine("-----------------Auth state : " + state);
                Console.WriteLine("-----------------Auth nonce : " + nonce);


                var idToken = "";
                var response  = utility.ValidateAndDecodeIdToken(idToken, nonce);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
