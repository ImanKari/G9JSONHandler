using System;
using System.Collections;
using G9JSONHandler.Attributes;

namespace G9JSONHandler.Common
{

    /// <summary>
    ///     A common helper class for parsing and writing process
    /// </summary>
    internal static class G9CCommonHelper
    {
        /// <summary>
        ///     Method to check that a type is an array or collection or not
        /// </summary>
        /// <param name="type">Specifies a type to check</param>
        /// <returns>The result will be true if the type is an array or collection.</returns>
        public static bool IsEnumerableType(Type type)
        {
            return type.Name != nameof(String)
                   && type.GetInterface(nameof(IEnumerable)) != null;
        }
    }
}