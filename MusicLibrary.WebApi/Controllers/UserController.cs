using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicLibrary.Model;
using MusicLibrary.Services;

namespace MusicLibrary.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("user")]
    public class UserController : ControllerBase
    {
        private readonly IJwtTokenProviderService tokenProvider;

        public UserController(IJwtTokenProviderService tokenProvider)
        {
            this.tokenProvider = tokenProvider;
        }

        [HttpGet("me")]
        public User GetCurrentUser()
        {
            return tokenProvider.GetUserFromPrincipal(HttpContext.User);
        }
    }
}
