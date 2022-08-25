using System;

namespace G9JSONHandler.Attributes
{
    /// <summary>
    ///     This attribute is used to choose a custom name for a member of an object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field,
        // ReSharper disable once RedundantAttributeUsageProperty
        AllowMultiple = false)]
    public class G9AttrJsonMemberCustomNameAttribute : Attribute
    {
        public G9AttrJsonMemberCustomNameAttribute(string customName)
        {
            if (string.IsNullOrEmpty(customName))
                throw new ArgumentNullException(nameof(customName),
                    $"The \"{nameof(customName)}\" used for the \"{nameof(G9AttrJsonMemberCustomNameAttribute)}\" argument can't be null.");
            Name = customName;
        }

        public string Name { get; }
    }
}