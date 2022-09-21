using System;
using G9AssemblyManagement.Interfaces;

namespace G9JSONHandler.Abstract
{
    /// <summary>
    ///     An abstract class for defining a custom parser for a specified generic type.
    /// </summary>
    public abstract class G9ACustomGenericTypeParser
    {
        #region Fields And Properties

        /// <summary>
        ///     Access to generic type
        /// </summary>
        public Type CustomGenericType { get; }

        #endregion

        #region Methods

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="customGenericType">Specifies a generic type for reacting.</param>
        protected G9ACustomGenericTypeParser(Type customGenericType)
        {
            CustomGenericType = customGenericType;
        }

        /// <summary>
        ///     Method to parse a generic object to a string.
        /// </summary>
        /// <param name="objectForParsing">Specifies a generic object for parsing to a string.</param>
        /// <param name="genericTypes">Specifies the type of generic parameters.</param>
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
        /// <returns>Parsed string value from specified generic object.</returns>
        public abstract string ObjectToString(object objectForParsing, Type[] genericTypes,
            G9IMemberGetter accessToObjectMember, Action<string> addCustomComment);

        /// <summary>
        ///     Method to parse a string to a generic object.
        /// </summary>
        /// <param name="stringForParsing">Specifies a string for parsing to a generic object.</param>
        /// <param name="genericTypes">Specifies the type of generic parameters.</param>
        /// <param name="accessToObjectMember">
        ///     An object consists of helpful information about a member (field or property) in the
        ///     main object.
        /// </param>
        /// <returns>Parsed generic object value from specified string.</returns>
        public abstract object StringToObject(string stringForParsing, Type[] genericTypes,
            G9IMemberGetter accessToObjectMember);

        #endregion
    }
}