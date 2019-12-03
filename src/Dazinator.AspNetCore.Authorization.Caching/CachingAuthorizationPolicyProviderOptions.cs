using Microsoft.Extensions.Caching.Memory;
using System;

namespace Dazinator.AspNetCore.Authorization.Caching
{
    public class CachingAuthorizationPolicyProviderOptions
    {
        /// <summary>
        /// An Action that Configures the cache entry for a cached policy. The Action takes the name of the policy to be cached and the cache entry to be configured as it's added to the cache.
        /// </summary> 
        public Action<string, ICacheEntry> ConfigurePolicyCacheEntry { get; set; }
        public IMemoryCache Cache { get; set; }

    }  

}
