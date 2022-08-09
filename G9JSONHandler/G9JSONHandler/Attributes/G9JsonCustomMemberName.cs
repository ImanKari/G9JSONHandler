using System;

namespace G9JSONHandler.Attributes
{
    /// <summary>
    ///     This attribute is used to choose a custom name for a member of an object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field,
        // ReSharper disable once RedundantAttributeUsageProperty
        AllowMultiple = false,
        // ReSharper disable once RedundantAttributeUsageProperty
        Inherited = false)]
    public sealed class G9JsonCustomMemberNameAttribute : Attribute
    {
        public G9JsonCustomMemberNameAttribute(string customName)
        {
            if (string.IsNullOrEmpty(customName))
                throw new ArgumentNullException(nameof(customName),
                    $"The \"{nameof(customName)}\" used for the \"{nameof(G9JsonCustomMemberNameAttribute)}\" argument can't be null.");
            Name = customName;
        }

        public string Name { get; }
    }
}