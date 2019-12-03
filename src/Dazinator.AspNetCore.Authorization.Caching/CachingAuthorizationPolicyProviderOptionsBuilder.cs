using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dazinator.AspNetCore.Authorization.Caching
{
    public class CachingAuthorizationPolicyProviderOptionsBuilder
    {

        protected Action<string, ICacheEntry> ConfigureCacheEntry { get; set; }

        protected Func<IServiceProvider, IMemoryCache> MemoryCacheFactory { get; set; }

        protected Action<string, ICacheEntry> DefaultConfigureCacheEntry = new Action<string, ICacheEntry>((policyName, entry) =>
        {
            entry.SetPriority(CacheItemPriority.NeverRemove);

        });

        public CachingAuthorizationPolicyProviderOptionsBuilder SetConfigureCacheEntry(Action<string, ICacheEntry> configure)
        {
            ConfigureCacheEntry = configure;
            return this;
        }

        public CachingAuthorizationPolicyProviderOptionsBuilder SetMemoryCacheFactory(Func<IServiceProvider, IMemoryCache> memoryCacheFactory)
        {
            MemoryCacheFactory = memoryCacheFactory;
            return this;
        }

        public CachingAuthorizationPolicyProviderOptions Build(IServiceProvider sp)
        {
            var options = new CachingAuthorizationPolicyProviderOptions();
            options.Cache = MemoryCacheFactory?.Invoke(sp) ?? sp.GetRequiredService<IMemoryCache>();
            options.ConfigurePolicyCacheEntry = this.ConfigureCacheEntry ?? DefaultConfigureCacheEntry;
            return options;
        }

    }
}
