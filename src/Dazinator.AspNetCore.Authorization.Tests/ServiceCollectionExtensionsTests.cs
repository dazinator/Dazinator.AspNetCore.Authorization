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
    }

}
