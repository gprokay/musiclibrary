using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace MusicLibrary.FrontEnd.Blazor
{
    public class AuthorizationPolicyProvider : IAuthorizationPolicyProvider
    {
        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return Task.FromResult(new AuthorizationPolicy(new IAuthorizationRequirement[] { new DefaultRequirment() }, new string[0]));
        }

        public Task<AuthorizationPolicy> GetFallbackPolicyAsync()
        {
            return Task.FromResult(new AuthorizationPolicy(new IAuthorizationRequirement[] { new DefaultRequirment() }, new string[0]));
        }

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            return Task.FromResult(new AuthorizationPolicy(new IAuthorizationRequirement[] { new DefaultRequirment() }, new string[0]));
        }
    }
}
