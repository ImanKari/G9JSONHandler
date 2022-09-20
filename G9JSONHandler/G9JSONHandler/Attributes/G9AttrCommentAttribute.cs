using System;

namespace G9JSONHandler.Attributes
{
    /// <summary>
    ///     This attribute adds a custom note (comment) for a member item.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field,
        // ReSharper disable once RedundantAttributeUsageProperty
        AllowMultiple = true)]
    public class G9AttrCommentAttribute : Attribute
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="customNote">Specifies a custom note (comment) for an object member.</param>
        public G9AttrCommentAttribute(string customNote)
        {
            if (string.IsNullOrEmpty(customNote))
                throw new ArgumentNullException(nameof(customNote),
                    $"The \"{nameof(customNote)}\" used for the \"{nameof(G9AttrCommentAttribute)}\" argument can't be null.");
            CustomNote = customNote;
        }

        /// <summary>
        ///     Specifies custom note for a member item
        /// </summary>
        public string CustomNote { get; }
    }
}