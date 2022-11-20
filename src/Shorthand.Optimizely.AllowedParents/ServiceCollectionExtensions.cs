using EPiServer;
using EPiServer.DataAbstraction;
using Microsoft.Extensions.DependencyInjection;
using Shorthand.Optimizely.AllowedParents.Attributes.Services;

namespace Shorthand.Optimizely.AllowedParents;

public static class ServiceCollectionExtensions {
    public static IServiceCollection AddAllowedParents(this IServiceCollection services) {
        services.Intercept<ContentTypeAvailabilityService>((provider, defaultServoce) => new AllowedParentsContentTypeAvailabilityService(defaultServoce, provider.GetRequiredService<IContentLoader>()));
        return services;
    }
}
