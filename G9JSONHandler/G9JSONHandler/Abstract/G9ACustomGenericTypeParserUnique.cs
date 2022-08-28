using System;

namespace G9JSONHandler.Abstract
{
    /// <summary>
    ///     An abstract class for defining a custom parser for a specified generic type.
    ///     <para />
    ///     For this type, per each member, a new instance is created and, after use, deleted (don't use it unless in mandatory
    ///     condition because it has a bad performance in terms of memory usage and speed).
    /// </summary>
    public abstract class G9ACustomGenericTypeParserUnique : G9ACustomGenericTypeParser
    {
        protected G9ACustomGenericTypeParserUnique(Type customGenericType) : base(customGenericType)
        {
        }
    }
}