# Shorthand.Optimizely.AllowedParents

Attributes and services to stop blocks from being created anywhere. Built for
Optimizely 12 but could possibly be adapted to earlier versions quite easily.

## Installation

```sh
> dotnet add package RobotsTxtCore
````

## Usage

Add the service interceptor in your `Startup.cs` file like this.

```csharp
public void ConfigureServices(IServiceCollection services) {
    services.AddAllowedParents();
}
```

Then annotate your blocks like this.

```csharp
[ContentType(
    DisplayName = "Container block",
    Description = "Container that is the only place where SpecificBlock should be used.",
    GUID = "03d80863-d9d0-46b2-90a5-a90cc5bc6fd7")]
public class ContainerBlock : BlockData {
    [Display(
        Name = "Container",
        Description = "Container for specific blocks.",
        GroupName = SystemTabNames.Content,
        Order = 1)]
    [AllowedTypes(typeof(SpecificBlock))]
    public virtual ContentArea? Container { get; set; }
}

[SiteContentType(
    DisplayName = "Speific block",
    Description = "A specific block that only makes sense within a container block.",
    GUID = "
5f731183-dbf2-4c29-94c0-ed9ddb9de2c2")]
[AllowedParents(typeof(ContainerBlock))]
public class SellingPointBlock : BlockData {
    ...
}

```

Or if you have a block that should never be created except by code.

```csharp
[ContentType(
    DisplayName = "Footer block",
    GUID = "183eccc7-1e21-4501-a0cf-ba38f6828908")]
[AllowNoParents]
public class FooterBlock : BlockData {
    ...
}
```
