using System.Security.Principal;
using Castle.DynamicProxy;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Globalization;
using Shorthand.Optimizely.AllowedParents.Attributes;

namespace Shorthand.Optimizely.AllowedParents.Services;

public class AllowedParentsContentTypeAvailabilityService : ContentTypeAvailabilityService {
    private readonly ContentTypeAvailabilityService _defaultContentTypeAvailabilityService;
    private readonly IContentLoader _contentLoader;

    public AllowedParentsContentTypeAvailabilityService(ContentTypeAvailabilityService defaultContentTypeAvailabilityService, IContentLoader contentLoader) {
        _defaultContentTypeAvailabilityService = defaultContentTypeAvailabilityService;
        _contentLoader = contentLoader;
    }

    public override AvailableSetting GetSetting(string contentTypeName) {
        return _defaultContentTypeAvailabilityService.GetSetting(contentTypeName);
    }

    public override bool IsAllowed(string parentContentTypeName, string childContentTypeName) {
        return _defaultContentTypeAvailabilityService.IsAllowed(parentContentTypeName, childContentTypeName);
    }

    public override IList<ContentType> ListAvailable(string contentTypeName, IPrincipal user) {
        return _defaultContentTypeAvailabilityService.ListAvailable(contentTypeName, user);
    }

    public override IList<ContentType> ListAvailable(IContent content, bool contentFolder, IPrincipal user) {
        var availableTypes = _defaultContentTypeAvailabilityService.ListAvailable(content, contentFolder, user);

        var targetType = ProxyUtil.GetUnproxiedType(content);
        if(targetType == null) {
            return availableTypes;
        }

        var filteredTypes = availableTypes.Where(contentType => {
            var modelType = contentType.ModelType;

            var hasDisallowAttribute = modelType.GetCustomAttributes(typeof(AllowNoParentsAttribute), true).Any();
            if(hasDisallowAttribute) {
                return false;
            }

            var allowAttribute = modelType.GetCustomAttributes(typeof(AllowedParentsAttribute), true)
                .OfType<AllowedParentsAttribute>()
                .FirstOrDefault();

            if(allowAttribute == null) {
                return true;
            }

            if(content is ContentFolder && allowAttribute.AlwaysAllowInFolders) {
                return true;
            }

            var allowedParents = allowAttribute.AllowedParents;
            if(allowedParents == null || allowedParents.Length == 0) {
                return true;
            }

            if(content is ContentAssetFolder contentAssetFolder) {
                if(CheckIfInContentAssetFolder(contentAssetFolder, allowedParents)) {
                    return true;
                }
            }

            return allowedParents.Contains(targetType);
        }).ToList();

        return filteredTypes!;
    }

    private bool CheckIfInContentAssetFolder(ContentFolder folder, Type[] allowedParents) {
        var rootFolder = GetContentAssetRootFolder(folder);

        if(_contentLoader.TryGet<IContent>(rootFolder.ContentOwnerID, ContentLanguage.PreferredCulture, out var ownerContent)) {
            var ownerTargetType = ProxyUtil.GetUnproxiedType(ownerContent);
            return allowedParents.Contains(ownerTargetType);
        }

        return false;
    }

    private ContentAssetFolder GetContentAssetRootFolder(ContentFolder folder) {
        while(folder.ParentLink != null) {
            if(!_contentLoader.TryGet<ContentAssetFolder>(folder.ParentLink, out var tmp)) {
                break;
            }

            folder = tmp;
        }

        return (ContentAssetFolder)folder;
    }
}
