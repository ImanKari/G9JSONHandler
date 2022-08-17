using System;

namespace G9JSONHandler.Attributes
{
    /// <summary>
    ///     This attribute uses to add a note comment for a JSON member item
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field,
        // ReSharper disable once RedundantAttributeUsageProperty
        AllowMultiple = true)]
    public class G9AttrJsonCommentAttribute : Attribute
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="customNote">Custom note (comment for JSON member item)</param>
        /// <param name="isNonstandardComment">
        ///     Specifies that comment is a nonstandard type
        ///     <para />
        ///     Note: Indeed, JSON has no option for comments(notes)
        ///     <para />
        ///     In standard mode, this JSON library considers a custom member that consists of a key and value like the usual
        ///     member item ("#__CommentN__#": "Comment Data") and saves the comment note there.
        ///     <para />
        ///     In nonstandard mode, this JSON library saves comments notes between two signs ("/* Comment Data  /*").
        /// </param>
        public G9AttrJsonCommentAttribute(string customNote, bool isNonstandardComment = false)
        {
            if (string.IsNullOrEmpty(customNote))
                throw new ArgumentNullException(nameof(customNote),
                    $"The \"{nameof(customNote)}\" used for the \"{nameof(G9AttrJsonCommentAttribute)}\" argument can't be null.");
            CustomNote = customNote;
            IsNonstandardComment = isNonstandardComment;
        }

        /// <summary>
        ///     Specifies custom note for a member item
        /// </summary>
        public string CustomNote { get; }

        /// <summary>
        ///     Specifies that comment is a nonstandard type.
        /// </summary>
        public bool IsNonstandardComment { get; }
    }
}