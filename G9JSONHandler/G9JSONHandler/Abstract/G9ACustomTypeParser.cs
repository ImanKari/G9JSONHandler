using System;
using G9AssemblyManagement.Interfaces;

namespace G9JSONHandler.Abstract
{
    /// <summary>
    ///     An abstract class for defining a custom parser for a specified type.
    /// </summary>
    /// <typeparam name="TTargetType">Specifies a type that the parser reacts to.</typeparam>
    public abstract class G9ACustomTypeParser<TTargetType>
    {
        /// <summary>
        ///     Method to parse an object to a string.
        /// </summary>
        /// <param name="objectForParsing">Specifies an object for parsing to a string.</param>
        /// <param name="accessToObjectMember">
        ///     An object consists of helpful information about a member (field or property) in the
        ///     main object.
        /// </param>
        /// <param name="addCustomComment">
        ///     A callback action that sets a comment for the specified member if needed.
        ///     <para />
        ///     Using that leads to making a comment before this member in the string structure.
        ///     <para />
        ///     Using of that is optional; it can be used several times or not used at all.
        /// </param>
        /// <returns>Parsed string value from specified object.</returns>
        public abstract string ObjectToString(TTargetType objectForParsing, G9IMemberGetter accessToObjectMember,
            Action<string> addCustomComment);

        /// <summary>
        ///     Method to parse a string to an object.
        /// </summary>
        /// <param name="stringForParsing">Specifies a string for parsing to an object.</param>
        /// <param name="accessToObjectMember">
        ///     An object consists of helpful information about a member (field or property) in the
        ///     main object.
        /// </param>
        /// <returns>Parsed object value from specified string.</returns>
        public abstract TTargetType StringToObject(string stringForParsing, G9IMemberGetter accessToObjectMember);
    }
}