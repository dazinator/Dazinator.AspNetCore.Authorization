[![Build Status](https://dev.azure.com/darrelltunnell/Public%20Projects/_apis/build/status/dazinator.Dazinator.AspNetCore.Authorization?branchName=master)](https://dev.azure.com/darrelltunnell/Public%20Projects/_build/latest?definitionId=10&branchName=master)

## Features

Allows you to build a composite `AuthorizationPolicyProvider` for your ASP.NET Core application.
This basically allows you to consolidate multiple `IAuthorizationPolicyProvider`s into a single one,
which will loop through the inner providers in order, to query each one to obtain the policy. 
If a NULL policy is returned by a provider, it will proceed to query the next provider in the list, until either the policy is returned, or NULL is returned.


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
I would recommend adhering to the following pattern so that your provider will as either part of the composite pattern above,
or could be registered on it's own with a specific fallback provider:


```

    public partial class CustomAuthorizationPolicyProvider : IAuthorizationPolicyProvider
    {
        public const string PolicyPrefix = "Custom:";
        private readonly IAuthorizationPolicyProvider _innerProvider;
        private readonly Task<AuthorizationPolicy> NullResult = Task.FromResult(default(AuthorizationPolicy));

        public PermissionsAuthorizationPolicyProvider(IAuthorizationPolicyProvider innerProvider)
        {
            _innerProvider = innerProvider; // could be null if being used as part of composite.
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            if(_innerProvider == null)
            {
                return NullResult;
            }
            return _innerProvider?.GetDefaultPolicyAsync();
        }

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (!policyName.StartsWith(CustomAuthorizationPolicyProvider.PolicyPrefix, StringComparison.OrdinalIgnoreCase))
            {
                if (_innerProvider == null)
                {
                    return NullResult;
                }
                return _innerProvider?.GetPolicyAsync();
            }

            // TODO: return your custom policy

           // var policy = new AuthorizationPolicyBuilder()
           //     .RequireClaim(CustomClaimTypes.Permission, permissionClaimValues)
           //     .Build();

            return Task.FromResult(policy);
        }
    }


```

By sticking to this pattern, consumers of your provider have the most flexibility. They can use it with any provider they want as a fallback, for example the Default provider:


```csharp

   services.AddSingleton<IAuthorizationPolicyProvider, CustomAuthorizationPolicyProvider>(sp=> {
                return new CustomAuthorizationPolicyProvider(new DefaultAuthorizationPolicyProvider(sp.GetRequiredService<IOptions<AuthorizationOptions>>()));
            });

```

Or they can use it as part of a `CompositePolicyProvider` as shown in above in this readme. The equivalent being:

```csharp

    services.AddCompositeAuthorizationPolicyProvider((builder) =>
    {
        builder.AddSingletonProvider<CustomAuthorizationPolicyProvider>()              
	           .AddSingletonProvider<DefaultAuthorizationPolicyProvider>();
    });

```

In either situation, your provider should work.