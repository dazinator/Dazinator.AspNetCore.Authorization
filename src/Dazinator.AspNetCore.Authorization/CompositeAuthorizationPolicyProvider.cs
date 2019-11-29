using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace Dazinator.Extensions.Authorization
{
    public class CompositeAuthorizationPolicyProvider : IAuthorizationPolicyProvider
    {

        private readonly IAuthorizationPolicyProvider[] _innerProviders;

        public CompositeAuthorizationPolicyProvider(params IAuthorizationPolicyProvider[] innerProviders)
        {
            _innerProviders = innerProviders;
        }

        public async Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            foreach (var item in _innerProviders)
            {
                var policy = await item.GetDefaultPolicyAsync();
                if (policy != null)
                {
                    return policy;
                }
            }

            return null;
        }

        public async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            foreach (var item in _innerProviders)
            {
                var policy = await item.GetPolicyAsync(policyName);
                if (policy != null)
                {
                    return policy;
                }
            }

            return null;
        }
    }
}
