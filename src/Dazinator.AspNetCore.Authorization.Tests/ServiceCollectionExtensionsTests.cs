using Dazinator.Extensions.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
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
                builder.AddSingletonProvider<TestPolicyProvider>()
                       .AddSingletonProvider<TestPolicyProvider>((s) =>
                       {
                           return new TestPolicyProvider("Bar");
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
                builder.AddSingletonProvider<TestPolicyProvider>()
                       .AddSingletonProvider<DefaultAuthorizationPolicyProvider>();
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
                builder.AddSingletonProvider<TestPolicyProviderWithInnerProvider>((sp) => new TestPolicyProviderWithInnerProvider())
                       .AddSingletonProvider<DefaultAuthorizationPolicyProvider>();
            });

            var sp = services.BuildServiceProvider();
            var composite = sp.GetRequiredService<IAuthorizationPolicyProvider>();
            Assert.NotNull(composite);
            Assert.IsType<CompositeAuthorizationPolicyProvider>(composite);
        }
    }

}
