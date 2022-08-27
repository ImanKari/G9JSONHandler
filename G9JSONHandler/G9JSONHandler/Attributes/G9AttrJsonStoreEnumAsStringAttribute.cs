using System;
using System.Runtime.InteropServices;

namespace G9JSONHandler.Attributes
{
    /// <summary>
    ///     This attribute specifies that an "enum" value must convert as a string.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field,
        // ReSharper disable once RedundantAttributeUsageProperty
        AllowMultiple = false)]
    public class G9AttrJsonStoreEnumAsStringAttribute : Attribute
    {
    }
}