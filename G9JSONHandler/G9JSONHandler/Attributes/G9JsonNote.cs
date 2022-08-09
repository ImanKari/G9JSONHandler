using System;

namespace G9JSONHandler.Attributes
{
    /// <summary>
    ///     This attribute uses to add a note comment for a JSON member item
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field,
        // ReSharper disable once RedundantAttributeUsageProperty
        AllowMultiple = true,
        // ReSharper disable once RedundantAttributeUsageProperty
        Inherited = false)]
    public class G9JsonComment : Attribute
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="customNote">Custom note (comment for JSON member item)</param>
        public G9JsonComment(string customNote)
        {
            if (string.IsNullOrEmpty(customNote))
                throw new ArgumentNullException(nameof(customNote),
                    $"The \"{nameof(customNote)}\" used for the \"{nameof(G9JsonComment)}\" argument can't be null.");
            CustomNot = customNote;
        }

        /// <summary>
        ///     Save note
        /// </summary>
        public string CustomNot { get; }
    }
}