using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;
using System.Threading.Tasks;

namespace Dazinator.AspNetCore.Authorization.Tests
{
    public class TestPolicyProvider : IAuthorizationPolicyProvider
    {
        private readonly string _policyPrefix;
        private readonly bool _returnDefaultPolicy;

        private readonly Lazy<AuthorizationPolicy> _defaultPolicy;


        public TestPolicyProvider(string policyPrefix = "Foo", bool returnDefaultPolicy = false)
        {
            _policyPrefix = policyPrefix;
            _returnDefaultPolicy = returnDefaultPolicy;
            _defaultPolicy = new Lazy<AuthorizationPolicy>(() => {
                if(returnDefaultPolicy)
                {
                    var policy = new AuthorizationPolicyBuilder(_policyPrefix);
                    policy.AddRequirements(new RolesAuthorizationRequirement(new[] { $"{_policyPrefix} Role" }));
                    return policy.Build();
                }
                return null;
            });
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return Task.FromResult(_defaultPolicy.Value);
        }

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith(_policyPrefix))
            {
                var policy = new AuthorizationPolicyBuilder(_policyPrefix);
                policy.AddRequirements(new RolesAuthorizationRequirement(new[] { $"{_policyPrefix} Role" }));
                return Task.FromResult(policy.Build());
            }
            return Task.FromResult(default(AuthorizationPolicy));
        }
    }

 }
