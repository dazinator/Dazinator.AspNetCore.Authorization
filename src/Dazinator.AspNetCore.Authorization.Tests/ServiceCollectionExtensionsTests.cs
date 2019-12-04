using Dazinator.Extensions.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace Dazinator.AspNetCore.Authorization.Tests
{
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void Can_Register_Composite_Provider()
        {
            var services = new ServiceCollection();
            services.AddCompositeAuthorizationPolicyProvider((builder) =>
            {
                builder.AddProvider<TestPolicyProvider>((a) => a.AsSingleton())
                       .AddProvider<TestPolicyProvider>((s) =>
                       {
                           s.AsSingleton((sp) => new TestPolicyProvider("Bar"));
                       });
            });

            var sp = services.BuildServiceProvider();
            var composite = sp.GetRequiredService<IAuthorizationPolicyProvider>();
            Assert.NotNull(composite);
            Assert.IsType<CompositeAuthorizationPolicyProvider>(composite);
        }

        [Fact]
        public void Can_Register_Composite_Provider_With_DefaultAuthroizationProvider()
        {
            var services = new ServiceCollection() as IServiceCollection;
            services.AddOptions();
            services.AddAuthorization();
            services.AddCompositeAuthorizationPolicyProvider((builder) =>
            {
                builder.AddProvider<TestPolicyProvider>((a) => a.AsSingleton())
                       .AddProvider<DefaultAuthorizationPolicyProvider>((a) => a.AsSingleton());

                //builder.AddSingletonProvider<TestPolicyProvider>()
                //       .AddSingletonProvider<DefaultAuthorizationPolicyProvider>();
            });

            var sp = services.BuildServiceProvider();
            var composite = sp.GetRequiredService<IAuthorizationPolicyProvider>();
            Assert.NotNull(composite);
            Assert.IsType<CompositeAuthorizationPolicyProvider>(composite);
        }

        [Fact]
        public void Can_Register_Composite_Provider_With_A_Provider_That_Has_Inner_Provider()
        {
            var services = new ServiceCollection() as IServiceCollection;
            services.AddOptions();
            services.AddAuthorization();
            services.AddCompositeAuthorizationPolicyProvider((builder) =>
            {
                builder.AddProvider<TestPolicyProviderWithInnerProvider>((a) => a.AsSingleton((sp) => new TestPolicyProviderWithInnerProvider()))
                       .AddProvider<DefaultAuthorizationPolicyProvider>((a) => a.AsSingleton());

                //builder.AddSingletonProvider<TestPolicyProviderWithInnerProvider>((sp) => new TestPolicyProviderWithInnerProvider())
                //       .AddSingletonProvider<DefaultAuthorizationPolicyProvider>();
            });

            var sp = services.BuildServiceProvider();
            var composite = sp.GetRequiredService<IAuthorizationPolicyProvider>();
            Assert.NotNull(composite);
            Assert.IsType<CompositeAuthorizationPolicyProvider>(composite);
        }


        [Fact]
        public void Can_Register_Composite_Provider_With_A_CachingInnerProvider()
        {
            var services = new ServiceCollection() as IServiceCollection;
            services.AddOptions();
            services.AddAuthorization();

           // services.AddSingleton<TestPolicyProvider>(sp => new TestPolicyProvider());

            services.AddCompositeAuthorizationPolicyProvider((builder) =>
            {
                builder.AddProvider<TestPolicyProvider>((a) =>
                {
                    a.AsSingleton((sp) => new TestPolicyProvider())
                     .AddCaching((cacheOptions) =>
                     {
                         cacheOptions.SetMemoryCacheFactory(sp => new MemoryCache(new MemoryCacheOptions
                         {
                             SizeLimit = 1024
                         })).SetConfigureCacheEntry((policyName, entry) =>
                         {
                             entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);
                         });
                     });
                })
                .AddProvider<DefaultAuthorizationPolicyProvider>((a) => a.AsSingleton());               

                //builder.AddSingletonProvider<TestPolicyProviderWithInnerProvider>((sp) => new TestPolicyProviderWithInnerProvider())
                //       .AddSingletonProvider<DefaultAuthorizationPolicyProvider>();
            });

            var sp = services.BuildServiceProvider();
            var composite = sp.GetRequiredService<IAuthorizationPolicyProvider>();
            Assert.NotNull(composite);
            Assert.IsType<CompositeAuthorizationPolicyProvider>(composite);
        }

    }

}
