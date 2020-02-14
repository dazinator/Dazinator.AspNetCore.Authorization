using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;

namespace Dazinator.AspNetCore.Authorization.Caching
{
    public class MemoryCachingAuthorizationPolicyProvider<TInnerProvider> : IAuthorizationPolicyProvider
        where TInnerProvider: class, IAuthorizationPolicyProvider
    {

        private readonly TInnerProvider _innerProvider;
        private readonly CachingAuthorizationPolicyProviderOptions _options;

        public const string DefaultPolicyName = "DefaultPolicy";
        public const string FallbackPolicyName = "FallbackPolicy";

        public readonly string CacheKeyPrefix;
        public readonly string DefaultPolicyCacheKey;
        public readonly string FallbackPolicyCacheKey;


        public MemoryCachingAuthorizationPolicyProvider(TInnerProvider innerProvider, CachingAuthorizationPolicyProviderOptions options)
        {
            _innerProvider = innerProvider;
           // _cache = cache;
            _options = options;
            // Just in case we are using a shared cache with other providers we form a provider specific cache key prefix.
            CacheKeyPrefix = $"{_innerProvider.GetType().Name}:";
            DefaultPolicyCacheKey = $"{CacheKeyPrefix}{DefaultPolicyName}";
            FallbackPolicyCacheKey = $"{CacheKeyPrefix}{FallbackPolicyName}";
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            var cache = _options.Cache;
            var item = cache.GetOrCreate(DefaultPolicyCacheKey, (entry) =>
            {
                _options.ConfigurePolicyCacheEntry(DefaultPolicyName, entry);
                return _innerProvider.GetDefaultPolicyAsync();
            });
            return item;
        }

#if NETSTANDARD2_1
        public Task<AuthorizationPolicy> GetFallbackPolicyAsync()
        {
            var cache = _options.Cache;
            var item = cache.GetOrCreate(FallbackPolicyCacheKey, (entry) =>
            {
                _options.ConfigurePolicyCacheEntry(FallbackPolicyName, entry);
                return _innerProvider.GetFallbackPolicyAsync();
            });
            return item;
        }
#endif

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
           
            var cache = _options.Cache;
            var item = cache.GetOrCreate(GetPolicyCacheKey(policyName), (entry) =>
            {
                _options.ConfigurePolicyCacheEntry(policyName, entry);
                return _innerProvider.GetPolicyAsync(policyName);
            });
            return item;
        }

        public string GetPolicyCacheKey(string policyName)
        {
           return $"{CacheKeyPrefix}{policyName}";
        }
    }

}
