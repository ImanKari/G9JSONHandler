using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using G9AssemblyManagement;
using G9AssemblyManagement.DataType;
using G9AssemblyManagement.Enums;
using G9AssemblyManagement.Interfaces;
using G9JSONHandler.Abstract;

namespace G9JSONHandler.Common
{
    internal static class G9CJsonCommon
    {
        /// <summary>
        ///     A collection for saving the Parser working process for use per each type
        /// </summary>
        public static readonly Dictionary<Type, G9DtTuple<Func<object, G9IMemberGetter, object>>>
            CustomParserCollection;

        /// <summary>
        ///     A collection for saving the instance of parsers
        /// </summary>
        public static readonly Dictionary<Type, object> CustomParserInstanceCollection;

        /// <summary>
        ///     Static constructor
        /// </summary>
        static G9CJsonCommon()
        {
            // Get total types inherited by specified abstract class
            var customTypeParsers = G9Assembly.TypeTools.GetInheritedTypesFromType(typeof(G9ACustomTypeParser<>));

            if (customTypeParsers.Count == 0)
                return;

            // Initialize
            CustomParserInstanceCollection = new Dictionary<Type, object>(customTypeParsers.Count);
            CustomParserCollection =
                new Dictionary<Type, G9DtTuple<Func<object, G9IMemberGetter, object>>>(customTypeParsers.Count);

            // The collection is completed by a loop
            // All parsers prepare for use.
            foreach (var parser in customTypeParsers)
            {
                // Get parser target type
                // ReSharper disable once PossibleNullReferenceException
                var genericType = parser.BaseType.GetGenericArguments().First();

                // Validation of repetitive parser
                if (CustomParserInstanceCollection.ContainsKey(genericType))
                    throw new Exception(
                        $@"The considered condition for each type's parser is just one number. But for type '{genericType.FullName}', there is more than one parser.
First parser for type '{genericType.FullName}': '{CustomParserInstanceCollection[genericType].GetType().FullName}'
Second parser for type '{genericType.FullName}': ''{parser.FullName}");


                G9DtTuple<Func<object, G9IMemberGetter, object>> access;

                if (parser.BaseType.IsGenericType &&
                    parser.BaseType.GetGenericTypeDefinition() == typeof(G9ACustomTypeParserUnique<>))
                {
                    CustomParserInstanceCollection.Add(genericType, parser);

                    access = new G9DtTuple<Func<object, G9IMemberGetter, object>>
                    {
                        Item1 = (o, m) =>
                        {
                            // Create instance of parser
                            var instance =
                                G9Assembly.InstanceTools.CreateInstanceFromType(
                                    (Type)CustomParserInstanceCollection[genericType]);

                            var methods = G9Assembly.ObjectAndReflectionTools.GetMethodsOfObject(instance,
                                G9EAccessModifier.StaticAndInstance | G9EAccessModifier.Public,
                                s => s.Name == nameof(G9ACustomTypeParser<object>.StringToObject) &&
                                     MethodValidation(s, genericType));

                            return methods[0].CallMethodWithResult<object>(o, m);
                        },
                        Item2 = (o, m) =>
                        {
                            // Create instance of parser
                            var instance =
                                G9Assembly.InstanceTools.CreateInstanceFromType(
                                    (Type)CustomParserInstanceCollection[genericType]);

                            var methods = G9Assembly.ObjectAndReflectionTools.GetMethodsOfObject(instance,
                                G9EAccessModifier.StaticAndInstance | G9EAccessModifier.Public,
                                s => s.Name == nameof(G9ACustomTypeParser<object>.ObjectToString) &&
                                     MethodValidation(s, genericType));

                            return methods[0].CallMethodWithResult<object>(o, m);
                        }
                    };
                }
                else
                {
                    // Add instance of parser
                    var instance = G9Assembly.InstanceTools.CreateInstanceFromType(parser);
                    CustomParserInstanceCollection.Add(genericType, instance);

                    var methods = G9Assembly.ObjectAndReflectionTools.GetMethodsOfObject(instance,
                        G9EAccessModifier.StaticAndInstance | G9EAccessModifier.Public,
                        s => MethodValidation(s, genericType)).OrderByDescending(s => s.MethodName).ToArray();

                    access = new G9DtTuple<Func<object, G9IMemberGetter, object>>
                    {
                        Item1 = (o, m) => methods[0].CallMethodWithResult<object>(o, m),
                        Item2 = (o, m) => methods[1].CallMethodWithResult<object>(o, m)
                    };
                }

                CustomParserCollection.Add(genericType, access);
            }
        }


        /// <summary>
        ///     Helper method to check validation of parser methods
        /// </summary>
        /// <param name="method">Access to method info</param>
        /// <param name="targetObjectType">Specifies the target type that parser write for that.</param>
        private static bool MethodValidation(MethodInfo method, Type targetObjectType)
        {
            if (method.Name == nameof(G9ACustomTypeParser<object>.StringToObject))
            {
                var parameters = method.GetParameters();

                // For both parser methods the parameters must be two
                if (parameters.Length != 2)
                    return false;

                // This parser method must have a return type like target type
                if (method.ReturnType != targetObjectType)
                    return false;

                // This parser method must have two parameter like below.
                return parameters[0].ParameterType == typeof(string) &&
                       parameters[1].ParameterType == typeof(G9IMemberGetter);
            }

            if (method.Name == nameof(G9ACustomTypeParser<object>.ObjectToString))
            {
                var parameters = method.GetParameters();

                // For both parser methods the parameters must be two
                if (parameters.Length != 2)
                    return false;

                // This parser method must have a return string type
                if (method.ReturnType != typeof(string))
                    return false;

                // This parser method must have two parameter like below.
                return parameters[0].ParameterType == targetObjectType &&
                       parameters[1].ParameterType == typeof(G9IMemberGetter);
            }

            return false;
        }
    }
}