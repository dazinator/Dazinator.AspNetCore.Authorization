[![Build Status](https://dev.azure.com/darrelltunnell/Public%20Projects/_apis/build/status/dazinator.Dazinator.AspNetCore.Authorization?branchName=master)](https://dev.azure.com/darrelltunnell/Public%20Projects/_build/latest?definitionId=10&branchName=master)

## Features

Allows you to build a composite `AuthorizationPolicyProvider` for your ASP.NET Core application.
This basically allows you to consolidate multiple `IAuthorizationPolicyProvider`s into a single one,
which will loop through the inner providers in order, to query each one to obtain the policy. 
If a NULL policy is returned by a provider, it will proceed to query the next provider in the list, until either the policy is returned, or the last NULL is returned.


Usage in `startup.cs`:

```csharp

    services.AddCompositeAuthorizationPolicyProvider((builder) =>
    {
        builder.AddSingletonProvider<PermissionPolicyProvider>()
               .AddSingletonProvider<TestPolicyProvider>((s) =>
               {
                   return new TestPolicyProvider("Bar");
               })
	           .AddSingletonProvider<DefaultAuthorizationPolicyProvider>(); // Asp.net default provider.
    });


```

See the tests for more information regarding usage.

## Note to Provider Authors

Are you writing a custom AuthorizationPolicyProvider that others might user?
I would recommend adhering to the following pattern so that your provider will behave well as either part of a composite provider,
or just registered on it's own but with a specific desired fallback provider:


```csharp

    public class CustomAuthorizationPolicyProvider : IAuthorizationPolicyProvider
    {
        public const string PolicyPrefix = "Custom:";
        private readonly IAuthorizationPolicyProvider _innerProvider;
        private readonly Task<AuthorizationPolicy> NullResult = Task.FromResult(default(AuthorizationPolicy));

        public PermissionsAuthorizationPolicyProvider(IAuthorizationPolicyProvider innerProvider = null)
        {
            _innerProvider = innerProvider;
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            if (_innerProvider == null)
            {
                return NullResult;
            }
            return _innerProvider?.GetDefaultPolicyAsync();
        }

#if NETCOREAPP3_0
        public Task<AuthorizationPolicy> GetFallbackPolicyAsync()
        {
            if (_innerProvider == null)
            {
                return NullResult;
            }
            return _innerProvider?.GetFallbackPolicyAsync();
        }
#endif

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (!policyName.StartsWith(PolicyPrefix, StringComparison.OrdinalIgnoreCase))
            {
                if (_innerProvider == null)
                {
                    return NullResult;
                }
                return _innerProvider?.GetPolicyAsync(policyName);
            }

            // TODO: Build policy but also think about caching!
            // var policy = new AuthorizationPolicyBuilder()
            //     .RequireClaim(CustomClaimTypes.Foo, permissionClaimValues)
            //     .Build();

            return Task.FromResult(policy);
        }
    } 


```

By sticking to this pattern, consumers of your provider will have the most flexibility. They can either register it with any provider they'd like as the fallback, for example the Default provider:


```csharp

   services.AddSingleton<IAuthorizationPolicyProvider, CustomAuthorizationPolicyProvider>(sp=> {
                return new CustomAuthorizationPolicyProvider(new DefaultAuthorizationPolicyProvider(sp.GetRequiredService<IOptions<AuthorizationOptions>>()));
            });

```

Or they can use it as part of a `CompositePolicyProvider` (as provided in this repository) and shown in above in this readme. The equivalent in that case would be:

```csharp

    services.AddCompositeAuthorizationPolicyProvider((builder) =>
    {
        builder.AddSingletonProvider<CustomAuthorizationPolicyProvider>()              
	           .AddSingletonProvider<DefaultAuthorizationPolicyProvider>();
    });

```

In either situation, your provider should work.

As a bonus, here is an example of the extension method you might ship with your custom provider, so consumers can add it directly when not wishing to add it using the composite pattern provided by this repo:

```csharp

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMyCustomAuthorisationPolicyProvider(this IServiceCollection services, Func<IServiceProvider, IAuthorizationPolicyProvider> innerProviderFactory = null)
        {
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionsAuthorizationPolicyProvider>(sp =>
            {
                return new PermissionsAuthorizationPolicyProvider(innerProviderFactory?.Invoke(sp));
            });
           
            return services;
        }
    }
    
```
