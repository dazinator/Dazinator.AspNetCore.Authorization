using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;
using System.Threading.Tasks;

namespace Dazinator.AspNetCore.Authorization.Tests
{
    public class TestPolicyProviderWithInnerProvider : IAuthorizationPolicyProvider
    {
        public readonly string PolicyPrefix = "WithInner:";

        private readonly Task<AuthorizationPolicy> NullResult = Task.FromResult(default(AuthorizationPolicy));

        private readonly IAuthorizationPolicyProvider _innerProvider;


        public TestPolicyProviderWithInnerProvider(IAuthorizationPolicyProvider inner = null)
        {
            _innerProvider = inner;
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            if (_innerProvider == null)
            {
                return NullResult;
            }
            return _innerProvider?.GetDefaultPolicyAsync();
        }

#if NETCOREAPP3_1
        public Task<AuthorizationPolicy> GetFallbackPolicyAsync()
        {
            if (_innerProvider == null)
            {
                return NullResult;
            }
            return _innerProvider?.GetFallbackPolicyAsync();
        }
#endif


        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (!policyName.StartsWith(PolicyPrefix, StringComparison.OrdinalIgnoreCase))
            {
                if (_innerProvider == null)
                {
                    return NullResult;
                }
                return _innerProvider?.GetPolicyAsync(policyName);
            }


            var policy = new AuthorizationPolicyBuilder(PolicyPrefix);
            policy.AddRequirements(new RolesAuthorizationRequirement(new[] { $"{PolicyPrefix} Role" }));
            return Task.FromResult(policy.Build());

        }
    }


}
