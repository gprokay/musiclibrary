using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Oauth2.v2.Data;
using Google.Apis.Services;
using Microsoft.IdentityModel.Tokens;
using MusicLibrary.DataAccess.Context;
using MusicLibrary.DataAccess.Entities;
using MusicLibrary.Model;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MusicLibrary.Services
{
    public static class PrincipalExtensions
    {
        public static int GetUserId(this IPrincipal principal)
        {
            var identity = principal?.Identity as ClaimsIdentity;
            if (identity == null)
            {
                throw new NotSupportedException();
            }

            return int.Parse(identity.FindFirst(ClaimTypes.Sid)?.Value);
        }

        public static User GetUser(this IPrincipal principal)
        {
            var identity = principal?.Identity as ClaimsIdentity;
            if (identity == null)
            {
                throw new NotSupportedException();
            }

            var name = identity.FindFirst(ClaimTypes.Name)?.Value;
            var email = identity.FindFirst(ClaimTypes.Email)?.Value;
            var id = int.Parse(identity.FindFirst(ClaimTypes.Sid)?.Value);

            return new User
            {
                Id = id,
                UserName = name,
                Email = email
            };
        }
    }

    public class GoogleTokenProviderService : IJwtTokenProviderService
    {
        private readonly string tokenSecret;
        private readonly string clientId;
        private readonly string clientSecret;
        private readonly MusicLibraryContext context;

        public GoogleTokenProviderService(string tokenSecret, string clientId, string clientSecret, MusicLibraryContext context)
        {
            this.tokenSecret = tokenSecret;
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            this.context = context;
        }

        public async Task<string> GetJwtTokenForAuthorizationCode(string authCode, CancellationToken cancellationToken)
        {
            var userInfo = await GetUserInfo(authCode, cancellationToken);
            var user = await context.UserRepository.GetUserByEmail(userInfo.Email);

            if (user == null)
            {
                return null;
            }

            return GetTokenFromUserInfo(user);
        }

        public User GetUserFromPrincipal(IPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }

            return principal.GetUser();
        }

        private async Task<Userinfoplus> GetUserInfo(string authCode, CancellationToken cancellationToken)
        {
            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret
                },
                Scopes = new[] { "profile", "email" },
            });

            var tokenRequest = new AuthorizationCodeTokenRequest();
            tokenRequest.ClientId = clientId;
            tokenRequest.ClientSecret = clientSecret;
            tokenRequest.Code = authCode;
            tokenRequest.RedirectUri = "http://localhost:4200";
            var tokenResponse = await tokenRequest.ExecuteAsync(flow.HttpClient, flow.TokenServerUrl, cancellationToken, flow.Clock);
            var cred = new UserCredential(flow, "user", tokenResponse);
            var oauthSerivce = new Google.Apis.Oauth2.v2.Oauth2Service(new BaseClientService.Initializer()
            {
                HttpClientInitializer = cred,
                ApplicationName = "App Name",
            });

            return await oauthSerivce.Userinfo.Get().ExecuteAsync();
        }

        private string GetTokenFromUserInfo(UserDo userInfo)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(tokenSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userInfo.UserName),
                    new Claim(ClaimTypes.Email, userInfo.Email),
                    new Claim(ClaimTypes.Sid, userInfo.Id.ToString())
                }),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Expires = DateTime.Now + TimeSpan.FromDays(365)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
