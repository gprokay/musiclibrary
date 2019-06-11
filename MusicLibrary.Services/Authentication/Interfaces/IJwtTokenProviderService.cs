using MusicLibrary.Model;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace MusicLibrary.Services
{
    public interface IJwtTokenProviderService
    {
        Task<string> GetJwtTokenForAuthorizationCode(string authCode, CancellationToken cancellationToken);

        User GetUserFromPrincipal(IPrincipal principal);
    }
}
