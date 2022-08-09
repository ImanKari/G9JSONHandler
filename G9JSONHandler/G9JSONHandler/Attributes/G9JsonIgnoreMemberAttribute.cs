using System;

namespace G9JSONHandler.Attributes
{
    /// <summary>
    ///     This attribute is used to ignore a member of an object for parsing to JSON (Also ignored while reading).
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field,
        // ReSharper disable once RedundantAttributeUsageProperty
        AllowMultiple = false,
        // ReSharper disable once RedundantAttributeUsageProperty
        Inherited = false)]
    public class G9JsonIgnoreMemberAttribute : Attribute
    {
    }
}