using Dazinator.Extensions.Authorization;
using System.Threading.Tasks;
using Xunit;

namespace Dazinator.AspNetCore.Authorization.Tests
{
    public class CompositeAuthorizationPolicyProviderTests
    {
        [Fact]
        public async Task GetDefaultPolicyAsync_WhenNoMatchedPolicy_ProviderReturnsNullPolicy()
        {          
            var sut = new CompositeAuthorizationPolicyProvider(new TestPolicyProvider("Foo"), new TestPolicyProvider("Bar"));
            var policy = await sut.GetDefaultPolicyAsync();
            Assert.Null(policy);
        }

        [Fact]
        public async Task GetPolicyAsync_WhenNoMatchedPolicy_ProviderReturnsNullPolicy()
        {
            var sut = new CompositeAuthorizationPolicyProvider(new TestPolicyProvider("Foo"), new TestPolicyProvider("Bar"));
            var policy = await sut.GetPolicyAsync("BAZ");
            Assert.Null(policy);
        }

        [Fact]
        public async Task GetFallbackPolicyAsync_WhenNoMatchedPolicy_ProviderReturnsNullPolicy()
        {
            var sut = new CompositeAuthorizationPolicyProvider(new TestPolicyProvider("Foo"), new TestPolicyProvider("Bar"));
            var policy = await sut.GetFallbackPolicyAsync();
            Assert.Null(policy);
        }

        [Fact]
        public async Task GetPolicyAsync_WhenMatchedPolicy_ProviderReturnsPolicy()
        {
            var sut = new CompositeAuthorizationPolicyProvider(new TestPolicyProvider("Foo"), new TestPolicyProvider("Bar"));
            var firstPolicy = await sut.GetPolicyAsync("Foo"); // from first provider
            Assert.NotNull(firstPolicy);
            
            var secondPolicy = await sut.GetPolicyAsync("Bar"); // from second provider
            Assert.NotNull(secondPolicy);

            Assert.NotSame(firstPolicy, secondPolicy);
        }

        [Fact]
        public async Task GetDefaultPolicyAsync_WhenMatchedPolicy_ProviderReturnsPolicy()
        {
            var sut = new CompositeAuthorizationPolicyProvider(new TestPolicyProvider("Foo"), 
                                                               new TestPolicyProvider("Bar", returnDefaultPolicy: true));
            var policy = await sut.GetDefaultPolicyAsync();
            Assert.NotNull(policy);
            Assert.Contains("Bar", policy.AuthenticationSchemes);
        }

        [Fact]
        public async Task GetFallbackPolicyAsync_WhenMatchedPolicy_ProviderReturnsPolicy()
        {
            var sut = new CompositeAuthorizationPolicyProvider(new TestPolicyProvider("Foo"),
                                                               new TestPolicyProvider("Bar", returnFallbackPolicy: true));
            var policy = await sut.GetFallbackPolicyAsync();
            Assert.NotNull(policy);
            Assert.Contains("Bar", policy.AuthenticationSchemes);
        }

    }

}
