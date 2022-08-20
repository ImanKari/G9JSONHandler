namespace G9JSONHandler.Enum
{
    /// <summary>
    ///     Enum specifies that parser has what methods.
    /// </summary>
    public enum G9ECustomParserType : byte
    {
        /// <summary>
        ///     Specifies that parser just includes the 'String To Object' method.
        /// </summary>
        StringToObject,

        /// <summary>
        ///     Specifies that parser just includes the 'Object To Json' method.
        /// </summary>
        ObjectToJson,

        /// <summary>
        ///     Specifies that parser consists of both methods ("Json To Object", "Object To Json").
        /// </summary>
        BothOfThem
    }
}