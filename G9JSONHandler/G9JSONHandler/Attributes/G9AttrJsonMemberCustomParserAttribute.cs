using System;
using System.Linq;
using System.Reflection;
using G9AssemblyManagement;
using G9AssemblyManagement.DataType;
using G9AssemblyManagement.Enums;
using G9AssemblyManagement.Interfaces;
using G9JSONHandler.Enum;

namespace G9JSONHandler.Attributes
{
    /// <summary>
    ///     This attribute enables the definition of a custom parsing process for a member.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field,
        // ReSharper disable once RedundantAttributeUsageProperty
        AllowMultiple = false)]
    public class G9AttrJsonMemberCustomParserAttribute : Attribute
    {
        /// <summary>
        ///     Constructor for initializing parser methods.
        ///     <para />
        ///     This type of constructor is just used to specify one method for parsing, which can be the string to object method
        ///     or the object to string method.
        ///     <para />
        ///     Warning: this initializing is just used when you need to have a one-side parsing. For example, you just need to
        ///     parse the object to string or vice versa.
        ///     <para />
        ///     The string to object method is used to parse a string value to an object value that is a member of another object.
        ///     <para />
        ///     The object to string method is used to parse an object value (a member of another object) to a string value.
        /// </summary>
        /// <param name="isStringToObjectMethod">
        ///     Specifies whether the specified method is the string to object method or not.
        ///     <para />
        ///     If it's set "true", specifies that the method is the string to object method.
        ///     <para />
        ///     If it's set "false", specifies that the method is the object to string method.
        /// </param>
        /// <param name="targetObjectType">Specifies the object type that includes the parser method.</param>
        /// <param name="methodName">
        ///     Specifies a parser method name that must exist in the specified type.
        ///     <para />
        ///     If the parameter 'isStringToObjectMethod' is set to 'true,' the specified method must have two parameters (the
        ///     first parameter is 'string' and the second one is 'G9IMemberGetter'); in continuation, it must return an object
        ///     value that is parsed from the string value.
        ///     <para />
        ///     If the parameter 'isStringToObjectMethod' is set to 'false,' the specified method must have two parameters (the
        ///     first parameter is 'object' and the second one is 'G9IMemberGetter'); in continuation, it must return a string
        ///     value that is parsed from the object value.
        /// </param>
        public G9AttrJsonMemberCustomParserAttribute(bool isStringToObjectMethod,
            Type targetObjectType, string methodName)
        {
            if (targetObjectType == null)
                throw new ArgumentNullException(
                    nameof(targetObjectType),
                    $"The \"{nameof(targetObjectType)}\" used for the \"{nameof(G9AttrJsonMemberCustomParserAttribute)}\" argument can't be null.");

            if (string.IsNullOrEmpty(methodName))
                throw new ArgumentNullException(
                    nameof(methodName),
                    $"The \"{nameof(methodName)}\" used for the \"{nameof(G9AttrJsonMemberCustomParserAttribute)}\" argument can't be null.");

            PrepareParserMethod(isStringToObjectMethod, targetObjectType, methodName);

            ParserType = isStringToObjectMethod ? G9ECustomParserType.StringToObject : G9ECustomParserType.ObjectToJson;
        }

        /// <summary>
        ///     Constructor for initializing parser methods.
        /// </summary>
        /// <param name="targetObjectType">Specifies the object type that consists of parser methods.</param>
        /// <param name="stringToObjectMethodName">
        ///     Specifies a parser method name that must exist in the specified type.
        ///     <para />
        ///     The string to object method is used to parse a string value to an object value
        ///     that is a member of another object.
        ///     <para />
        ///     The specified method must have two parameters (the first parameter is 'string' and the second one is
        ///     'G9IMemberGetter'); in continuation, it must return an object value that is parsed from the string value.
        /// </param>
        /// <param name="objectToStringMethodName">
        ///     Specifies a parser method name that must exist in the specified type.
        ///     <para />
        ///     The object to string method is used to parse an object value (a member of
        ///     another object) to a string value.
        ///     <para />
        ///     The specified method must have two parameters (the first parameter is 'object' and the second one is
        ///     'G9IMemberGetter'); in continuation, it must return a string value that is parsed from the object value.
        /// </param>
        public G9AttrJsonMemberCustomParserAttribute(
            Type targetObjectType, string stringToObjectMethodName, string objectToStringMethodName)
        {
            if (targetObjectType == null)
                throw new ArgumentNullException(
                    nameof(targetObjectType),
                    $"The \"{nameof(targetObjectType)}\" used for the \"{nameof(G9AttrJsonMemberCustomParserAttribute)}\" argument can't be null.");

            if (string.IsNullOrEmpty(stringToObjectMethodName))
                throw new ArgumentNullException(
                    nameof(stringToObjectMethodName),
                    $"The \"{nameof(stringToObjectMethodName)}\" used for the \"{nameof(G9AttrJsonMemberCustomParserAttribute)}\" argument can't be null.");

            if (string.IsNullOrEmpty(objectToStringMethodName))
                throw new ArgumentNullException(
                    nameof(objectToStringMethodName),
                    $"The \"{nameof(objectToStringMethodName)}\" used for the \"{nameof(G9AttrJsonMemberCustomParserAttribute)}\" argument can't be null.");

            PrepareParserMethod(true, targetObjectType, stringToObjectMethodName);
            PrepareParserMethod(false, targetObjectType, objectToStringMethodName);
            ParserType = G9ECustomParserType.BothOfThem;
        }

        /// <summary>
        ///     Constructor for initializing parser methods.
        /// </summary>
        /// <param name="targetObjectTypeForStringToObjectMethod">
        ///     Specifies the object type that includes the parser method (for the string to object method).
        /// </param>
        /// <param name="stringToObjectMethodName">
        ///     Specifies a parser method name that must exist in the specified type.
        ///     <para />
        ///     The string to object method is used to parse a string value to an object value
        ///     that is a member of another object.
        ///     <para />
        ///     The specified method must have two parameters (the first parameter is 'string' and the second one is
        ///     'G9IMemberGetter'); in continuation, it must return an object value that is parsed from the string value.
        /// </param>
        /// <param name="targetObjectTypeForObjectToStringMethod">
        ///     Specifies the object type that includes the parser method (for the object to string method).
        /// </param>
        /// <param name="objectToStringMethodName">
        ///     Specifies a parser method name that must exist in the specified type.
        ///     <para />
        ///     The object to string method is used to parse an object value (a member of
        ///     another object) to a string value.
        ///     <para />
        ///     The specified method must have two parameters (the first parameter is 'object' and the second one is
        ///     'G9IMemberGetter'); in continuation, it must return a string value that is parsed from the object value.
        /// </param>
        public G9AttrJsonMemberCustomParserAttribute(
            Type targetObjectTypeForStringToObjectMethod, string stringToObjectMethodName,
            Type targetObjectTypeForObjectToStringMethod, string objectToStringMethodName)
        {
            if (targetObjectTypeForStringToObjectMethod == null)
                throw new ArgumentNullException(
                    nameof(targetObjectTypeForStringToObjectMethod),
                    $"The \"{nameof(targetObjectTypeForStringToObjectMethod)}\" used for the \"{nameof(G9AttrJsonMemberCustomParserAttribute)}\" argument can't be null.");

            if (string.IsNullOrEmpty(stringToObjectMethodName))
                throw new ArgumentNullException(
                    nameof(stringToObjectMethodName),
                    $"The \"{nameof(stringToObjectMethodName)}\" used for the \"{nameof(G9AttrJsonMemberCustomParserAttribute)}\" argument can't be null.");

            if (targetObjectTypeForObjectToStringMethod == null)
                throw new ArgumentNullException(
                    nameof(targetObjectTypeForObjectToStringMethod),
                    $"The \"{nameof(targetObjectTypeForObjectToStringMethod)}\" used for the \"{nameof(G9AttrJsonMemberCustomParserAttribute)}\" argument can't be null.");

            if (string.IsNullOrEmpty(objectToStringMethodName))

                throw new ArgumentNullException(
                    nameof(objectToStringMethodName),
                    $"The \"{nameof(objectToStringMethodName)}\" used for the \"{nameof(G9AttrJsonMemberCustomParserAttribute)}\" argument can't be null.");

            PrepareParserMethod(true, targetObjectTypeForStringToObjectMethod, stringToObjectMethodName);
            PrepareParserMethod(false, targetObjectTypeForObjectToStringMethod, objectToStringMethodName);
            ParserType = G9ECustomParserType.BothOfThem;
        }

        /// <summary>
        ///     Specifies the type of parser
        /// </summary>
        public G9ECustomParserType ParserType { get; }

        /// <summary>
        ///     Property to save specified method for parsing a string to an object
        /// </summary>
        public G9DtMethod StringToObjectMethod { private set; get; }

        /// <summary>
        ///     Property to save specified method for parsing an object to a string
        /// </summary>
        public G9DtMethod ObjectToStringMethod { private set; get; }

        /// <summary>
        ///     Method to specify one method for parsing, which can be the string to object method or the object to string method.
        /// </summary>
        /// <param name="isStringToObjectMethod">
        ///     Specifies whether the specified method is the string to object method or not.
        ///     <para />
        ///     If it's set "true", specifies that the method is the string to object method.
        ///     <para />
        ///     If it's set "false", specifies that the method is the object to string method.
        /// </param>
        /// <param name="targetObjectType">Specifies the object type that includes the parser method.</param>
        /// <param name="methodName">
        ///     Specifies a parser method name that must exist in the specified type.
        ///     <para />
        ///     If the parameter 'isStringToObjectMethod' is set to 'true,' the specified method must have two parameters (the
        ///     first parameter is 'string' and the second one is 'G9IMemberGetter'); in continuation, it must return an object
        ///     value that is parsed from the string value.
        ///     <para />
        ///     If the parameter 'isStringToObjectMethod' is set to 'false,' the specified method must have two parameters (the
        ///     first parameter is 'object' and the second one is 'G9IMemberGetter'); in continuation, it must return a string
        ///     value that is parsed from the object value.
        /// </param>
        private void PrepareParserMethod(bool isStringToObjectMethod,
            Type targetObjectType, string methodName)
        {
            // A temp variable specifying that the method with the specified name was found, but the validation of parameters isn't correct.
            var findButNotMatch = false;

            // Get access to method
            G9DtMethod parserMethod;
            if (targetObjectType.IsAbstract && targetObjectType.IsSealed)
                parserMethod = G9Assembly.ObjectAndReflectionTools.GetMethodsOfType(targetObjectType,
                        G9EAccessModifier.Public | G9EAccessModifier.StaticAndInstance,
                        s => s.Name == methodName && MethodValidation(isStringToObjectMethod, s, ref findButNotMatch))
                    .FirstOrDefault();
            else
                parserMethod = G9Assembly.ObjectAndReflectionTools.GetMethodsOfType(targetObjectType,
                        G9EAccessModifier.Public | G9EAccessModifier.StaticAndInstance,
                        s => s.Name == methodName && MethodValidation(isStringToObjectMethod, s, ref findButNotMatch),
                        true)
                    .FirstOrDefault();

            // Check access
            if (Equals(parserMethod, default(G9DtMethod)))
                if (findButNotMatch)
                    throw new ArgumentException(
                        $"In the specified type '{targetObjectType},' the specified parser method with the name '{methodName}' is not found.",
                        nameof(methodName));
                else
                    throw new ArgumentException(
                        $@"The specified method ({targetObjectType}.{methodName}) was found, but its parameters (or return type) are incorrect.
For the 'ObjectToString' method, the first parameter is 'string,' and the second one is 'G9IMemberGetter'; in continuation, it must return an object.
For the 'StringToObject' method, the first parameter is 'object,' and the second one is 'G9IMemberGetter'; in continuation, it must return a string.",
                        nameof(methodName));

            // Check parameters


            // Set the method
            if (isStringToObjectMethod)
                StringToObjectMethod = parserMethod;
            else
                ObjectToStringMethod = parserMethod;
        }

        /// <summary>
        ///     Method to check validation for parser method condition
        /// </summary>
        /// <param name="isStringToObjectMethod">
        ///     Specifies whether the specified method is the string to object method or not.
        ///     <para />
        ///     If it's set "true", specifies that the method is the string to object method.
        ///     <para />
        ///     If it's set "false", specifies that the method is the object to string method.
        /// </param>
        /// <param name="method">Specifies the method info for checking</param>
        /// <param name="findButNotMatch">
        ///     A ref parameter specifying that the method with the specified name was found, but the
        ///     validation of parameters isn't correct.
        /// </param>
        /// <returns>If method has a valid condition, return true</returns>
        // ReSharper disable once RedundantAssignment
        private static bool MethodValidation(bool isStringToObjectMethod, MethodInfo method, ref bool findButNotMatch)
        {
            findButNotMatch = true;

            var parameters = method.GetParameters();

            // For both parser types the parameters must be two
            if (parameters.Length != 2)
                return false;

            if (isStringToObjectMethod)
            {
                // For the "StringToObject" method, first parameter is string, second one is G9IMemberGetter, and return type is object
                if (parameters[0].ParameterType == typeof(string) &&
                    parameters[1].ParameterType == typeof(G9IMemberGetter) &&
                    method.ReturnType == typeof(object))
                    return true;
            }
            else
            {
                // For the "ObjectToString" method, first parameter is object, second one is G9IMemberGetter, and return type is string
                if (parameters[0].ParameterType == typeof(object) &&
                    parameters[1].ParameterType == typeof(G9IMemberGetter) &&
                    method.ReturnType == typeof(string))
                    return true;
            }

            return false;
        }
    }
}