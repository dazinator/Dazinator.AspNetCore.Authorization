using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dazinator.Extensions.Authorization
{

    public class CompositeAuthorizationPolicyProviderOptionsBuilder<TProvider>
         where TProvider : class, IAuthorizationPolicyProvider
    {
        private CompositeAuthorizationPolicyOptionsBuilder _compositeAuthorizationPolicyOptionsBuilder;

        public CompositeAuthorizationPolicyProviderOptionsBuilder(CompositeAuthorizationPolicyOptionsBuilder compositeAuthorizationPolicyOptionsBuilder)
        {
            _compositeAuthorizationPolicyOptionsBuilder = compositeAuthorizationPolicyOptionsBuilder;
            DecoratorProviderType = typeof(TProvider); // default to decorated type (i.e no decorator used).
        }

        public Type DecoratorProviderType { get; set; }

        public CompositeAuthorizationPolicyOptionsBuilder CompositePolicyOptionsBulder { get { return _compositeAuthorizationPolicyOptionsBuilder; } }

        public CompositeAuthorizationPolicyProviderOptionsBuilder<TProvider> AsSingleton(Func<IServiceProvider, TProvider> factory)
        {
            _compositeAuthorizationPolicyOptionsBuilder.Services.AddSingleton<TProvider>(factory);
            return this;

        }

        public CompositeAuthorizationPolicyProviderOptionsBuilder<TProvider> AsSingleton()
        {
            _compositeAuthorizationPolicyOptionsBuilder.Services.AddSingleton<TProvider>();
            return this;
        }

        internal void Build()
        {
            var providersList = _compositeAuthorizationPolicyOptionsBuilder.Providers;
            if (!providersList.Contains(DecoratorProviderType))
            {
                providersList.Add(DecoratorProviderType);
            }
        }
    }

    public class CompositeAuthorizationPolicyOptionsBuilder
    {
        private readonly IServiceCollection _services;
        private readonly List<Type> _providers;

        public CompositeAuthorizationPolicyOptionsBuilder(IServiceCollection services)
        {
            _services = services;
            _providers = new List<Type>();
        }
        ///// <summary>
        ///// Register the provider concrete type as a singleton for DI purposes, and adds it to the composite.
        ///// </summary>
        ///// <typeparam name="TProvider"></typeparam>
        ///// <param name="factory"></param>
        ///// <returns></returns>
        //public CompositeAuthorizationPolicyOptionsBuilder AddSingletonProvider<TProvider>(Func<IServiceProvider, TProvider> factory)
        //    where TProvider : class, IAuthorizationPolicyProvider
        //{
        //    Services.AddSingleton<TProvider>(factory);
        //    return AddProvider<TProvider>();
        //}

        //public CompositeAuthorizationPolicyOptionsBuilder AddSingletonProvider<TProvider>()
        //    where TProvider : class, IAuthorizationPolicyProvider
        //{
        //    Services.AddSingleton<TProvider>();
        //    return AddProvider<TProvider>();
        //}       

        /// <summary>
        /// Adds the provider's type to the composite, but does not register that type with DI. You must register is with DI seperately with the appropriate lifetime etc.
        /// </summary>
        /// <typeparam name="TProvider"></typeparam>
        /// <returns></returns>
        public CompositeAuthorizationPolicyOptionsBuilder AddProvider<TProvider>(Action<CompositeAuthorizationPolicyProviderOptionsBuilder<TProvider>> providerOptions = null)
            where TProvider : class, IAuthorizationPolicyProvider
        {
            var opts = new CompositeAuthorizationPolicyProviderOptionsBuilder<TProvider>(this);
            providerOptions?.Invoke(opts);
            opts.Build();
            return this;
        }
        internal List<Type> Providers => _providers;

        internal IServiceCollection Services => _services;

        /// <summary>
        /// Builds the required DI service registration for the Composite provider.
        /// </summary>
        public void Build()
        {
            Services.AddSingleton<IAuthorizationPolicyProvider, CompositeAuthorizationPolicyProvider>((sp) =>
            {
                var innerProviders = new List<IAuthorizationPolicyProvider>();
                foreach (var item in Providers)
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
