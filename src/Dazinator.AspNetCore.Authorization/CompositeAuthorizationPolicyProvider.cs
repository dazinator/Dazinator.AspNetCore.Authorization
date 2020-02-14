using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace Dazinator.Extensions.Authorization
{
    public class CompositeAuthorizationPolicyProvider : IAuthorizationPolicyProvider
    {

        private readonly IAuthorizationPolicyProvider[] _innerProviders;
        private readonly Task<AuthorizationPolicy> NullResult = Task.FromResult(default(AuthorizationPolicy));

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

            return await NullResult;
        }

#if NETSTANDARD2_1
        public async Task<AuthorizationPolicy> GetFallbackPolicyAsync()
        {
            foreach (var item in _innerProviders)
            {
                var policy = await item.GetFallbackPolicyAsync();
                if (policy != null)
                {
                    return policy;
                }
            }

            return await NullResult;
        }
#endif

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

            return await NullResult;
        }
    }
    
}
