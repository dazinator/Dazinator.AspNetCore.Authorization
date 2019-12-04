using Dazinator.AspNetCore.Authorization.Caching;
using Dazinator.Extensions.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;
using Xunit;

namespace Dazinator.AspNetCore.Authorization.Tests
{
    public class MemoryCachingAuthorizationPolicyProviderTests
    {
        [Fact]
        public async Task GetDefaultPolicyAsync_WhenNoMatchedPolicy_ProviderCachesNullPolicy()
        {
            var cache = new MemoryCache(new MemoryCacheOptions
            {
               // SizeLimit = 1024
            });

            var options = new CachingAuthorizationPolicyProviderOptions()
            {
                Cache = cache,
                ConfigurePolicyCacheEntry = (name, entry) => entry.SetPriority(CacheItemPriority.NeverRemove)
            };

            var sut = new MemoryCachingAuthorizationPolicyProvider<TestPolicyProvider>(new TestPolicyProvider("Foo"), options);
            var policy = await sut.GetDefaultPolicyAsync();
            Assert.Null(policy);

            var cachedResult = options.Cache.Get<Task<AuthorizationPolicy>>(sut.DefaultPolicyCacheKey);
            Assert.Null(await cachedResult);
        }

        [Fact]
        public async Task GetPolicyAsync_WhenNoMatchedPolicy_ProviderCachesNullPolicy()
        {
            var cache = new MemoryCache(new MemoryCacheOptions
            {
               // SizeLimit = 1024
            });

            var options = new CachingAuthorizationPolicyProviderOptions()
            {
                Cache = cache,
                ConfigurePolicyCacheEntry = (name, entry) => entry.SetPriority(CacheItemPriority.NeverRemove)
            };
            var sut = new MemoryCachingAuthorizationPolicyProvider<TestPolicyProvider>(new TestPolicyProvider("Foo"), options);
            var policy = await sut.GetPolicyAsync("BAZ");
            Assert.Null(policy);

           
            var cachedResult = options.Cache.Get<Task<AuthorizationPolicy>>(sut.GetPolicyCacheKey("BAZ"));
            Assert.Null(await cachedResult);
        }

        [Fact]
        public async Task GetFallbackPolicyAsync_WhenNoMatchedPolicy_ProviderCachesNullPolicy()
        {
            var cache = new MemoryCache(new MemoryCacheOptions
            {
              //  SizeLimit = 1024
            });

            var options = new CachingAuthorizationPolicyProviderOptions()
            {
                Cache = cache,
                ConfigurePolicyCacheEntry = (name, entry) => entry.SetPriority(CacheItemPriority.NeverRemove)
            };
            var sut = new MemoryCachingAuthorizationPolicyProvider<TestPolicyProvider>(new TestPolicyProvider("Foo"), options);
            var policy = await sut.GetFallbackPolicyAsync();
            Assert.Null(policy);

            var cachedResult = options.Cache.Get<Task<AuthorizationPolicy>>(sut.FallbackPolicyCacheKey);
            Assert.Null(await cachedResult);
        }

        [Fact]
        public async Task GetPolicyAsync_WhenMatchedPolicy_ProviderReturnsPolicy()
        {
            var cache = new MemoryCache(new MemoryCacheOptions
            {
               // SizeLimit = 1024
            });

            var options = new CachingAuthorizationPolicyProviderOptions()
            {
                Cache = cache,
                ConfigurePolicyCacheEntry = (name, entry) => entry.SetPriority(CacheItemPriority.NeverRemove)
            };
            var sut = new MemoryCachingAuthorizationPolicyProvider<TestPolicyProvider>(new TestPolicyProvider("Foo"), options);
            var firstPolicyTask = sut.GetPolicyAsync("Foo"); // from first provider
            var firstPolicy = await firstPolicyTask;
            Assert.NotNull(firstPolicy);

            var cachedResult = options.Cache.Get<Task<AuthorizationPolicy>>(sut.GetPolicyCacheKey("Foo"));
            Assert.NotNull(await cachedResult);
            Assert.Same(cachedResult, firstPolicyTask);

            var secondPolicyTask =  sut.GetPolicyAsync("Bar"); // no policy found
            var secondPolicy = await secondPolicyTask;
            Assert.Null(secondPolicy);
         
            var cachedSecondResult = options.Cache.Get<Task<AuthorizationPolicy>>(sut.GetPolicyCacheKey("Bar"));
            Assert.Null(await cachedSecondResult);
            Assert.Same(cachedSecondResult, secondPolicyTask);        

        }

        [Fact]
        public async Task GetDefaultPolicyAsync_WhenMatchedPolicy_ProviderReturnsCachedPolicy()
        {
            var cache = new MemoryCache(new MemoryCacheOptions
            {
               // SizeLimit = 1024
            });

            var options = new CachingAuthorizationPolicyProviderOptions()
            {
                Cache = cache,
                ConfigurePolicyCacheEntry = (name, entry) => entry.SetPriority(CacheItemPriority.NeverRemove)
            };
            var sut = new MemoryCachingAuthorizationPolicyProvider<TestPolicyProvider>(new TestPolicyProvider("Foo", returnDefaultPolicy: true), options);
            var policy = await sut.GetDefaultPolicyAsync();
            Assert.NotNull(policy);
            Assert.Contains("Foo", policy.AuthenticationSchemes);

            var cachedResult = options.Cache.Get<Task<AuthorizationPolicy>>(sut.DefaultPolicyCacheKey);
            Assert.NotNull(await cachedResult);
            Assert.Same(await cachedResult, policy);
        }

        [Fact]
        public async Task GetFallbackPolicyAsync_WhenMatchedPolicy_ProviderReturnsPolicy()
        {
            var cache = new MemoryCache(new MemoryCacheOptions
            {
                //SizeLimit = 1024
            });

            var options = new CachingAuthorizationPolicyProviderOptions()
            {
                Cache = cache,
                ConfigurePolicyCacheEntry = (name, entry) => entry.SetPriority(CacheItemPriority.NeverRemove)
            };
            var sut = new MemoryCachingAuthorizationPolicyProvider<TestPolicyProvider>(new TestPolicyProvider("Bar", returnFallbackPolicy: true), options);
            var policy = await sut.GetFallbackPolicyAsync();
            Assert.NotNull(policy);
            Assert.Contains("Bar", policy.AuthenticationSchemes);

            var cachedResult = options.Cache.Get<Task<AuthorizationPolicy>>(sut.FallbackPolicyCacheKey);
            Assert.NotNull(await cachedResult);
            Assert.Same(await cachedResult, policy);
        }

    }

}
