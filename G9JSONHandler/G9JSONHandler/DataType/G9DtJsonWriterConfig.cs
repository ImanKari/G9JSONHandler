using System.Reflection;
using G9AssemblyManagement;
using G9AssemblyManagement.Enums;
using G9JSONHandler.Enum;

namespace G9JSONHandler.DataType
{
    /// <summary>
    ///     The data type for setting the writer config.
    /// </summary>
    public class G9DtJsonWriterConfig
    {
        /// <summary>
        ///     Specifies which modifiers will include in the searching process
        /// </summary>
        public readonly BindingFlags AccessibleModifiers;

        /// <summary>
        ///     Specifies the type of comments in JSON structure.
        ///     <para />
        ///     Note: Indeed, JSON has no option for comments(notes)
        ///     <para />
        ///     In standard mode, this JSON library considers a custom member that consists of a key and value like the usual
        ///     member item ("#__CommentN__#": "Comment Data") and saves the comment note there.
        ///     <para />
        ///     In nonstandard mode, this JSON library saves comments notes between two signs ("/* Comment Data  /*").
        /// </summary>
        public readonly G9ECommentMode CommentMode;

        /// <summary>
        ///     Specifies that JSON result must be formatted or not
        /// </summary>
        public readonly bool IsFormatted;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="accessibleModifiers">
        ///     Specifies which modifiers will include in the searching process
        ///     <para />
        ///     By default, it's set to "BindingFlags.Instance | BindingFlags.Public"
        /// </param>
        /// <param name="isFormatted">Specifies that JSON result must be formatted or not.</param>
        /// <param name="commentMode">
        ///     Specifies the type of comments in JSON structure.
        ///     <para />
        ///     Note: Indeed, JSON has no option for comments(notes)
        ///     <para />
        ///     In standard mode, this JSON library considers a custom member that consists of a key and value like the usual
        ///     member item ("#__CommentN__#": "Comment Data") and saves the comment note there.
        ///     <para />
        ///     In nonstandard mode, this JSON library saves comments notes between two signs ("/* Comment Data  /*").
        /// </param>
        public G9DtJsonWriterConfig(BindingFlags accessibleModifiers = BindingFlags.Instance | BindingFlags.Public,
            bool isFormatted = false, G9ECommentMode commentMode = G9ECommentMode.StandardMode)
        {
            CommentMode = commentMode;
            IsFormatted = isFormatted;
            AccessibleModifiers = accessibleModifiers;
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="accessibleModifiers">Specifies which modifiers will include in the searching process</param>
        /// <param name="isFormatted">Specifies that JSON result must be formatted or not.</param>
        /// <param name="commentMode">
        ///     Specifies the type of comments in JSON structure.
        ///     <para />
        ///     Note: Indeed, JSON has no option for comments(notes)
        ///     <para />
        ///     In standard mode, this JSON library considers a custom member that consists of a key and value like the usual
        ///     member item ("#__CommentN__#": "Comment Data") and saves the comment note there.
        ///     <para />
        ///     In nonstandard mode, this JSON library saves comments notes between two signs ("/* Comment Data  /*").
        /// </param>
        public G9DtJsonWriterConfig(G9EAccessModifier accessibleModifiers, bool isFormatted = false,
            G9ECommentMode commentMode = G9ECommentMode.StandardMode)
        {
            CommentMode = commentMode;
            IsFormatted = isFormatted;
            AccessibleModifiers = G9Assembly.ObjectAndReflectionTools.CreateCustomModifier(accessibleModifiers);
        }
    }
}