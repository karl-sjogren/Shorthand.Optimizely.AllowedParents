namespace Shorthand.Optimizely.AllowedParents.Attributes;

/// <summary>
/// Use this attribute to limit the allowed parents for a content type.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class AllowedParentsAttribute : Attribute {
    /// <summary>
    /// Limit which parent content types are allowed for this content type.
    /// </summary>
    /// <param name="parents">A list of types which should be allowed as parents.</param>
    public AllowedParentsAttribute(params Type[] parents) : this(false, parents) { }

    /// <summary>
    /// Limit which parent content types are allowed for this content type.
    /// </summary>
    /// <param name="alwaysAllowInFolders">Set this to true to allow creation in any folder, not just the local asset folder.</param>
    /// <param name="parents">A list of types which should be allowed as parents.</param>
    public AllowedParentsAttribute(bool alwaysAllowInFolders, params Type[] parents) {
        AlwaysAllowInFolders = alwaysAllowInFolders;
        AllowedParents = parents;
    }

    internal bool AlwaysAllowInFolders { get; }
    internal Type[] AllowedParents { get; }
}
