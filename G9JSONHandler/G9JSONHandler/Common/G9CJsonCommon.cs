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
        public static readonly Dictionary<Type, G9DtTuple<Func<object, Type, G9IMemberGetter, Action<string>, object>>>
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
            var customTypeParsers =
                // Custom type parser
                G9Assembly.TypeTools.GetInheritedTypesFromType(typeof(G9ACustomTypeParser<>))
                    .Concat(
                        // Custom generic type parser
                        G9Assembly.TypeTools.GetInheritedTypesFromType(typeof(G9ACustomGenericTypeParser))
                    ).ToArray();

            if (customTypeParsers.Length == 0)
                return;

            // Initialize
            CustomParserInstanceCollection = new Dictionary<Type, object>(customTypeParsers.Length);
            CustomParserCollection =
                new Dictionary<Type, G9DtTuple<Func<object, Type, G9IMemberGetter, Action<string>, object>>>(
                    customTypeParsers.Length);

            // The collection is completed by a loop
            // All parsers prepare for use.
            foreach (var parser in customTypeParsers)
            {
                // ReSharper disable once PossibleNullReferenceException
                Type genericTypeDefinition = null;
                Type targetTypeForParsing;

                // Get parser target type
                if (parser.BaseType == typeof(G9ACustomGenericTypeParser) ||
                    parser.BaseType == typeof(G9ACustomGenericTypeParserUnique))
                {
                    // Create instance of parser
                    var instance =
                        (G9ACustomGenericTypeParser)G9Assembly.InstanceTools.CreateInstanceFromType(parser);
                    targetTypeForParsing = instance.CustomGenericType;
                }
                else
                {
                    // ReSharper disable once PossibleNullReferenceException
                    genericTypeDefinition = parser.BaseType.GetGenericTypeDefinition();
                    targetTypeForParsing = parser.BaseType.GetGenericArguments().First();
                }

                // Validation of repetitive parser
                if (CustomParserInstanceCollection.ContainsKey(targetTypeForParsing))
                    throw new Exception(
                        $@"The considered condition for each type's parser is just one number. But for type '{targetTypeForParsing.FullName}', there is more than one parser.
First parser for type '{targetTypeForParsing.FullName}': '{CustomParserInstanceCollection[targetTypeForParsing].GetType().FullName}'
Second parser for type '{targetTypeForParsing.FullName}': ''{parser.FullName}");


                G9DtTuple<Func<object, Type, G9IMemberGetter, Action<string>, object>> access;

                if ((parser.BaseType.IsGenericType && genericTypeDefinition == typeof(G9ACustomTypeParserUnique<>)) ||
                    parser.BaseType == typeof(G9ACustomGenericTypeParserUnique))
                {
                    CustomParserInstanceCollection.Add(targetTypeForParsing, parser);

                    if (parser.BaseType.IsGenericType)
                        access = new G9DtTuple<Func<object, Type, G9IMemberGetter, Action<string>, object>>
                        {
                            Item1 = (o, t, m, a) =>
                            {
                                // Create instance of parser
                                var instance =
                                    G9Assembly.InstanceTools.CreateInstanceFromType(
                                        (Type)CustomParserInstanceCollection[targetTypeForParsing]);

                                var methods = G9Assembly.ObjectAndReflectionTools.GetMethodsOfObject(instance,
                                    G9EAccessModifier.StaticAndInstance | G9EAccessModifier.Public,
                                    s => s.Name == nameof(G9ACustomTypeParser<object>.StringToObject) &&
                                         MethodValidation(s, targetTypeForParsing));

                                return methods[0].CallMethodWithResult<object>(o, m);
                            },
                            Item2 = (o, t, m, a) =>
                            {
                                // Create instance of parser
                                var instance =
                                    G9Assembly.InstanceTools.CreateInstanceFromType(
                                        (Type)CustomParserInstanceCollection[targetTypeForParsing]);

                                var methods = G9Assembly.ObjectAndReflectionTools.GetMethodsOfObject(instance,
                                    G9EAccessModifier.StaticAndInstance | G9EAccessModifier.Public,
                                    s => s.Name == nameof(G9ACustomTypeParser<object>.ObjectToString) &&
                                         MethodValidation(s, targetTypeForParsing));

                                return methods[0].CallMethodWithResult<object>(o, m, a);
                            }
                        };
                    else
                        access = new G9DtTuple<Func<object, Type, G9IMemberGetter, Action<string>, object>>
                        {
                            Item1 = (o, t, m, a) =>
                            {
                                // Create instance of parser
                                var instance =
                                    G9Assembly.InstanceTools.CreateInstanceFromType(
                                        (Type)CustomParserInstanceCollection[targetTypeForParsing]);

                                var methods = G9Assembly.ObjectAndReflectionTools.GetMethodsOfObject(instance,
                                    G9EAccessModifier.StaticAndInstance | G9EAccessModifier.Public,
                                    s => s.Name == nameof(G9ACustomGenericTypeParser.StringToObject) &&
                                         MethodValidationForGenericTypes(s));

                                return methods[0].CallMethodWithResult<object>(o, t.GetGenericArguments(), m);
                            },
                            Item2 = (o, t, m, a) =>
                            {
                                // Create instance of parser
                                var instance =
                                    G9Assembly.InstanceTools.CreateInstanceFromType(
                                        (Type)CustomParserInstanceCollection[targetTypeForParsing]);

                                var methods = G9Assembly.ObjectAndReflectionTools.GetMethodsOfObject(instance,
                                    G9EAccessModifier.StaticAndInstance | G9EAccessModifier.Public,
                                    s => s.Name == nameof(G9ACustomGenericTypeParser.ObjectToString) &&
                                         MethodValidationForGenericTypes(s));

                                return methods[0].CallMethodWithResult<object>(o, t.GetGenericArguments(), m, a);
                            }
                        };
                }
                else
                {
                    // Add instance of parser
                    var instance = G9Assembly.InstanceTools.CreateInstanceFromType(parser);
                    CustomParserInstanceCollection.Add(targetTypeForParsing, instance);

                    if (parser.BaseType.IsGenericType)
                    {
                        var methods = G9Assembly.ObjectAndReflectionTools.GetMethodsOfObject(instance,
                                G9EAccessModifier.StaticAndInstance | G9EAccessModifier.Public,
                                s => MethodValidation(s, targetTypeForParsing)).OrderByDescending(s => s.MethodName)
                            .ToDictionary(s => s.MethodName);

                        access = new G9DtTuple<Func<object, Type, G9IMemberGetter, Action<string>, object>>
                        {
                            Item1 = (o, t, m, a) =>
                                methods[nameof(G9ACustomTypeParser<object>.StringToObject)]
                                    .CallMethodWithResult<object>(o, m),
                            Item2 = (o, t, m, a) =>
                                methods[nameof(G9ACustomTypeParser<object>.ObjectToString)]
                                    .CallMethodWithResult<object>(o, m, a)
                        };
                    }
                    else
                    {
                        var methods = G9Assembly.ObjectAndReflectionTools.GetMethodsOfObject(instance,
                                G9EAccessModifier.StaticAndInstance | G9EAccessModifier.Public,
                                MethodValidationForGenericTypes).OrderByDescending(s => s.MethodName)
                            .ToDictionary(s => s.MethodName);

                        access = new G9DtTuple<Func<object, Type, G9IMemberGetter, Action<string>, object>>
                        {
                            Item1 = (o, t, m, a) =>
                                methods[nameof(G9ACustomGenericTypeParser.StringToObject)]
                                    .CallMethodWithResult<object>(o, t.GetGenericArguments(), m),
                            Item2 = (o, t, m, a) =>
                                methods[nameof(G9ACustomGenericTypeParser.ObjectToString)]
                                    .CallMethodWithResult<object>(o, t.GetGenericArguments(), m, a)
                        };
                    }
                }

                CustomParserCollection.Add(targetTypeForParsing, access);
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
                if (parameters.Length != 3)
                    return false;

                // This parser method must have a return string type
                if (method.ReturnType != typeof(string))
                    return false;

                // This parser method must have two parameter like below.
                return parameters[0].ParameterType == targetObjectType &&
                       parameters[1].ParameterType == typeof(G9IMemberGetter) &&
                       parameters[2].ParameterType == typeof(Action<string>);
            }

            return false;
        }

        /// <summary>
        ///     Helper method to check validation of parser methods for generic types
        /// </summary>
        /// <param name="method">Access to method info</param>
        private static bool MethodValidationForGenericTypes(MethodInfo method)
        {
            if (method.Name == nameof(G9ACustomGenericTypeParser.StringToObject))
            {
                var parameters = method.GetParameters();

                // For both parser methods the parameters must be two
                if (parameters.Length != 3)
                    return false;

                // This parser method must have a return type like target type
                if (method.ReturnType != typeof(object))
                    return false;

                // This parser method must have two parameter like below.
                return parameters[0].ParameterType == typeof(string) &&
                       parameters[1].ParameterType == typeof(Type[]) &&
                       parameters[2].ParameterType == typeof(G9IMemberGetter);
            }

            if (method.Name == nameof(G9ACustomGenericTypeParser.ObjectToString))
            {
                var parameters = method.GetParameters();

                // For both parser methods the parameters must be two
                if (parameters.Length != 4)
                    return false;

                // This parser method must have a return string type
                if (method.ReturnType != typeof(string))
                    return false;

                // This parser method must have two parameter like below.
                return parameters[0].ParameterType == typeof(object) &&
                       parameters[1].ParameterType == typeof(Type[]) &&
                       parameters[2].ParameterType == typeof(G9IMemberGetter) &&
                       parameters[3].ParameterType == typeof(Action<string>);
            }

            return false;
        }
    }
}