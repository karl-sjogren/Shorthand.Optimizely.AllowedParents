namespace Shorthand.Optimizely.AllowedParents.Attributes;

/// <summary>
/// Use this attribute to make a content type only createable from code.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class AllowNoParentsAttribute : Attribute {
}
