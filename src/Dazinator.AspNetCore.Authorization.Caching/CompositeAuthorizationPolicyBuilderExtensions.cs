using Dazinator.AspNetCore.Authorization.Caching;
using Dazinator.Extensions.Authorization;
using Microsoft.AspNetCore.Authorization;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CompositeAuthorizationPolicyBuilderExtensions
    {
        public static void AddSingletonMemoryCachingPolicyProvider<TInnerProvider>(this CompositeAuthorizationPolicyOptionsBuilder builder, Action<CachingAuthorizationPolicyProviderOptionsBuilder> configure)
            where TInnerProvider : IAuthorizationPolicyProvider
        {
            var optionsBuilder = new CachingAuthorizationPolicyProviderOptionsBuilder();
            configure(optionsBuilder);

            builder.AddSingletonProvider<MemoryCachingAuthorizationPolicyProvider>((sp) =>
            {
                var options = optionsBuilder.Build(sp);
                var inner = sp.GetRequiredService<TInnerProvider>();
                return new MemoryCachingAuthorizationPolicyProvider(inner, options);
            });
        }
    }


}
