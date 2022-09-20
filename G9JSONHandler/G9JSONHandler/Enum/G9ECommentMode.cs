namespace G9JSONHandler.Enum
{
    /// <summary>
    ///     Enum for specifying how comments must be written in JSON.
    /// </summary>
    public enum G9ECommentMode : byte
    {
        /// <summary>
        ///     Specifies that comment is a standard type
        ///     <para />
        ///     Note: Indeed, JSON has no option for comments(notes)
        ///     <para />
        ///     In standard mode, this JSON library considers a custom member that consists of a key and value like the usual
        ///     member item ("#__CommentN__#": "Comment Data") and saves the comment note there.
        ///     <para />
        ///     In nonstandard mode, this JSON library saves comments notes between two signs ("/* Comment Data  /*").
        /// </summary>
        StandardMode,

        /// <summary>
        ///     Specifies that comment is a nonstandard type
        ///     <para />
        ///     Note: Indeed, JSON has no option for comments(notes)
        ///     <para />
        ///     In standard mode, this JSON library considers a custom member that consists of a key and value like the usual
        ///     member item ("#__CommentN__#": "Comment Data") and saves the comment note there.
        ///     <para />
        ///     In nonstandard mode, this JSON library saves comments notes between two signs ("/* Comment Data  /*").
        /// </summary>
        NonstandardMode
    }
}