using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dazinator.Extensions.Authorization
{
    public class CompositeAuthorizationPolicyOptionsBuilder
    {
        private readonly IServiceCollection _services;
        private readonly List<Type> _providers;


        public CompositeAuthorizationPolicyOptionsBuilder(IServiceCollection services)
        {
            _services = services;
            _providers = new List<Type>();
        }
        /// <summary>
        /// Register the provider concrete type as a singleton for DI purposes, and adds it to the composite.
        /// </summary>
        /// <typeparam name="TProvider"></typeparam>
        /// <param name="factory"></param>
        /// <returns></returns>
        public CompositeAuthorizationPolicyOptionsBuilder AddSingletonProvider<TProvider>(Func<IServiceProvider, TProvider> factory)
            where TProvider : class, IAuthorizationPolicyProvider
        {
            _services.AddSingleton<TProvider>(factory);
            return AddProvider<TProvider>();
        }

        public CompositeAuthorizationPolicyOptionsBuilder AddSingletonProvider<TProvider>()
            where TProvider : class, IAuthorizationPolicyProvider
        {
            _services.AddSingleton<TProvider>();
            return AddProvider<TProvider>();
        }

        /// <summary>
        /// Adds the provider's type to the composite, but does not register that type with DI. You must register is with DI seperately with the appropriate lifetime etc.
        /// </summary>
        /// <typeparam name="TProvider"></typeparam>
        /// <returns></returns>
        public CompositeAuthorizationPolicyOptionsBuilder AddProvider<TProvider>()
            where TProvider : class, IAuthorizationPolicyProvider
        {
            var providerType = typeof(TProvider);
            if (!_providers.Contains(providerType))
            {
                _providers.Add(providerType);
            }
            return this;
        }
        protected List<Type> Providers { get; set; }

        /// <summary>
        /// Builds the required DI service registration for the Composite provider.
        /// </summary>
        public void Build()
        {
            _services.AddSingleton<IAuthorizationPolicyProvider, CompositeAuthorizationPolicyProvider>((sp) =>
            {
                var innerProviders = new List<IAuthorizationPolicyProvider>();
                foreach (var item in _providers)
                {
                    var providerInstances = sp.GetServices(item).Cast<IAuthorizationPolicyProvider>();
                    innerProviders.AddRange(providerInstances);                   
                }
                var composite = new CompositeAuthorizationPolicyProvider(innerProviders.ToArray());
                return composite;
            });
        }

    }
}
