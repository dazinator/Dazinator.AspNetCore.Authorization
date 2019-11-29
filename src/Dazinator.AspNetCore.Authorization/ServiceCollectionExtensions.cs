using Dazinator.Extensions.Authorization;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCompositeAuthorizationPolicyProvider(this IServiceCollection services, Action<CompositeAuthorizationPolicyOptionsBuilder> configure)
        {
            var builder = new CompositeAuthorizationPolicyOptionsBuilder(services);
            configure?.Invoke(builder);
            builder.Build();
            return services;

        }
    }
}
