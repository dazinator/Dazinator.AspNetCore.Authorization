using Dazinator.Extensions.Authorization;
using System.Threading.Tasks;
using Xunit;

namespace Dazinator.AspNetCore.Authorization.Tests
{
    public class CompositeAuthorizationPolicyProviderTests
    {
        [Fact]
        public async Task GetDefaultPolicyAsync_When_No_Matched_Policy_Provider_Returns_Null_Policy()
        {          
            var sut = new CompositeAuthorizationPolicyProvider(new TestPolicyProvider("Foo"), new TestPolicyProvider("Bar"));
            var policy = await sut.GetDefaultPolicyAsync();
            Assert.Null(policy);
        }

        [Fact]
        public async Task GetPolicyAsync_When_No_Matched_Policy_Provider_Returns_Null_Policy()
        {
            var sut = new CompositeAuthorizationPolicyProvider(new TestPolicyProvider("Foo"), new TestPolicyProvider("Bar"));
            var policy = await sut.GetPolicyAsync("BAZ");
            Assert.Null(policy);
        }

        [Fact]
        public async Task GetPolicyAsync_When_Matched_Policy_Provider_Returns_Policy()
        {
            var sut = new CompositeAuthorizationPolicyProvider(new TestPolicyProvider("Foo"), new TestPolicyProvider("Bar"));
            var firstPolicy = await sut.GetPolicyAsync("Foo"); // from first provider
            Assert.NotNull(firstPolicy);
            
            var secondPolicy = await sut.GetPolicyAsync("Bar"); // from second provider
            Assert.NotNull(secondPolicy);

            Assert.NotSame(firstPolicy, secondPolicy);
        }

        [Fact]
        public async Task GetDefaultPolicyAsync_When_Matched_Policy_Provider_Returns_Policy()
        {
            var sut = new CompositeAuthorizationPolicyProvider(new TestPolicyProvider("Foo"), 
                                                               new TestPolicyProvider("Bar", returnDefaultPolicy: true));
            var policy = await sut.GetDefaultPolicyAsync();
            Assert.NotNull(policy);
            Assert.Contains("Bar", policy.AuthenticationSchemes);
        }

    }

}
