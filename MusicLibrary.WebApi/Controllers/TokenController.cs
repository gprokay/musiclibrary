using Microsoft.AspNetCore.Mvc;
using MusicLibrary.Services;
using System.Threading;
using System.Threading.Tasks;

namespace MusicLibrary.WebApi.Controllers
{
    [ApiController]
    [Route("token")]
    public class TokenController : ControllerBase
    {
        private readonly IJwtTokenProviderService tokenProvider;

        public TokenController(IJwtTokenProviderService tokenProvider)
        {
            this.tokenProvider = tokenProvider;
        }

        [HttpGet("google")]
        public async Task<string> GetTokenWithGoogleAccessCode(string authCode, CancellationToken cancellationToken)
        {
            var token = await tokenProvider.GetJwtTokenForAuthorizationCode(authCode, cancellationToken);

            if (string.IsNullOrEmpty(token))
            {
                HttpContext.Response.StatusCode = 401;
            }

            return token;
        }
    }
}
