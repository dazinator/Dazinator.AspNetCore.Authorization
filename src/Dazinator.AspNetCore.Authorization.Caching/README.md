## Features

Allows you decorate an `IAuthorizationPolicyProvider` with a Caching provider.
You can configure each cache item.

Usage in `startup.cs`:

```csharp

            services.AddSingleton<TestPolicyProvider>(sp => new TestPolicyProvider()); // Register your inner provider with DI

            services.AddCompositeAuthorizationPolicyProvider((builder) =>
            {
			    // Register a caching provider that wraps your inner provider but adds caching.
                builder.AddSingletonMemoryCachingPolicyProvider<TestPolicyProvider>((options) =>
                {
				    // Optionally configure the `IMemoryCache` otherwise shared `IMemoryCache` will be used which must be registered seperately with services.AddMemoryCache().
                    options.SetMemoryCacheFactory(sp => new MemoryCache(new MemoryCacheOptions
                    {
                        SizeLimit = 1024
                    })).SetConfigureCacheEntry((policyName, entry) =>
                    { 
					    // As policies are added to the cache, you can configure cache entry / expiry options here,
						// optionally taking advantage of the policyName to make caching decisions.
                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);
                    });
                });
				// Add more providers to the composite.. if the provider above returns a `NULL` policy, the next provider in the composite will be queried.
				// Could also wrap any one of this with Caching Provider.
                builder.AddSingletonProvider<TestPolicyProviderWithInnerProvider>((sp) => new TestPolicyProviderWithInnerProvider())
                       .AddSingletonProvider<DefaultAuthorizationPolicyProvider>();
            });   
    });

```