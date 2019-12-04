using Dazinator.AspNetCore.Authorization.Caching;
using Dazinator.Extensions.Authorization;
using Microsoft.AspNetCore.Authorization;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CompositeAuthorizationPolicyBuilderExtensions
    {
        public static void AddCaching<TInnerProvider>(this CompositeAuthorizationPolicyProviderOptionsBuilder<TInnerProvider> builder, Action<CachingAuthorizationPolicyProviderOptionsBuilder> configure)
            where TInnerProvider : class, IAuthorizationPolicyProvider
        {
            var optionsBuilder = new CachingAuthorizationPolicyProviderOptionsBuilder();
            configure(optionsBuilder);
            builder.CompositePolicyOptionsBulder.AddProvider<MemoryCachingAuthorizationPolicyProvider<TInnerProvider>>((s) =>
            {
                s.AsSingleton((sp) =>
                {
                    var options = optionsBuilder.Build(sp);
                    var inner = sp.GetRequiredService<TInnerProvider>();
                    return new MemoryCachingAuthorizationPolicyProvider<TInnerProvider>(inner, options);
                });
            });
            builder.DecoratorProviderType = typeof(MemoryCachingAuthorizationPolicyProvider<TInnerProvider>);


            //builder.AddSingletonProvider<MemoryCachingAuthorizationPolicyProvider<TInnerProvider>>((sp) =>
            //{
            //    var options = optionsBuilder.Build(sp);
            //    var inner = sp.GetRequiredService<TInnerProvider>();
            //    return new MemoryCachingAuthorizationPolicyProvider(inner, options);
            //});
        }
    }


}
