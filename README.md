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
