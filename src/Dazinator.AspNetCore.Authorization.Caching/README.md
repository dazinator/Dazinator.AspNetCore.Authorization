## Features

Allows you decorate an `IAuthorizationPolicyProvider` with a Caching provider that caches the policies from
the underlying provider with a configurable cache.

Usage in `startup.cs`:

```csharp

			services.AddCompositeAuthorizationPolicyProvider((builder) =>
            {
                builder.AddProvider<TestPolicyProvider>((a) =>
                {
                    a.AsSingleton((sp) => new TestPolicyProvider()) // custom factory method
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
                .AddProvider<DefaultAuthorizationPolicyProvider>((a) => a.AsSingleton());  // add more providers to composite.             

            });   
```