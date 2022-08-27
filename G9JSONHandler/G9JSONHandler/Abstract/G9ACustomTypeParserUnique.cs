namespace G9JSONHandler.Abstract
{
    /// <summary>
    ///     An abstract class for defining a custom parser for a specified type.
    ///     <para />
    ///     For this type, per each member, a new instance is created and, after use, deleted (don't use it unless in mandatory
    ///     condition because it has a bad performance in terms of memory usage and speed).
    /// </summary>
    /// <typeparam name="TTargetType">Specifies a type that the parser reacts to.</typeparam>
    public abstract class G9ACustomTypeParserUnique<TTargetType> : G9ACustomTypeParser<TTargetType>
    {
    }
}