using System;
using System.IO;
using System.Linq;
using System.Text;
using G9AssemblyManagement;
using G9AssemblyManagement.Enums;
using G9JSONHandler.Common;
using G9JSONHandler.Core;
using G9JSONHandler.DataType;

namespace G9JSONHandler
{
    /// <summary>
    ///     A pretty small library for working with JSON.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class G9JSON
    {
        /// <inheritdoc cref="G9JsonWriter.G9ObjectToJson" />
        public static string ObjectToJson(object objectItem, G9DtJsonWriterConfig writerConfig = null)
        {
            if (writerConfig == null)
                writerConfig = new G9DtJsonWriterConfig();

            return G9JsonWriter.G9ObjectToJson(objectItem, writerConfig);
        }

        /// <inheritdoc cref="G9JsonParser.G9JsonToObject{TType}" />
        public static TType JsonToObject<TType>(string json, G9DtJsonParserConfig parserConfig = null)
        {
            if (parserConfig == null)
                parserConfig = new G9DtJsonParserConfig();

            return G9JsonParser.G9JsonToObject<TType>(json, parserConfig);
        }

        /// <summary>
        ///     Converts a JSON string to a G9CDynamicObject.
        /// </summary>
        /// <param name="json">Specifies the JSON string for conversion.</param>
        /// <param name="parserConfig">
        ///     Specifies a custom configuration for the parsing process. If null, a default configuration is used.
        /// </param>
        /// <returns>A G9CDynamicObject that represents the JSON structure.</returns>
        public static G9CDynamicObject JsonToDynamicObject(string json, G9DtJsonParserConfig parserConfig = null)
        {
            if (parserConfig == null)
                parserConfig = new G9DtJsonParserConfig();

            return G9JsonParser.G9JsonToObject<G9CDynamicObject>(json, parserConfig);
        }

        /// <summary>
        ///     Converts a G9CDynamicObject to a JSON string.
        /// </summary>
        /// <param name="dynamicObject">The dynamic object to convert to JSON.</param>
        /// <param name="writerConfig">
        ///     Configuration for the JSON writer. If null, a default configuration is used.
        /// </param>
        /// <returns>A JSON string representation of the dynamic object.</returns>
        public static string DynamicObjectToJson(G9CDynamicObject dynamicObject, G9DtJsonWriterConfig writerConfig = null)
        {
            if (writerConfig == null)
                writerConfig = new G9DtJsonWriterConfig();

            return G9JsonWriter.G9DynamicObjectToJson(dynamicObject, writerConfig);
        }


        /// <summary>
        ///     Method to read a JSON from a file and convert it to the desired object.
        /// </summary>
        /// <typeparam name="TType">Specifies the type of object.</typeparam>
        /// <param name="filePath">Specifies a file path for reading.</param>
        /// <param name="parserConfig">
        ///     Specifies a custom config for the parsing process.
        /// </param>
        /// <returns>An object that is converted by JSON string.</returns>
        /// <param name="encoder">Specifies the desired encoding for reading (By default, it's UTF8).</param>
        /// <returns>An object that is converted by JSON string.</returns>
        public static TType JsonFileToObject<TType>(string filePath, G9DtJsonParserConfig parserConfig = null,
            Encoding encoder = default)
        {
            PathValidation(filePath, true);

            if (Equals(encoder, default(Encoding)))
                encoder = Encoding.UTF8;

            var jsonFileString = string.Empty;
            G9Assembly.InputOutputTools.WaitForAccessToFile(filePath, fs =>
            {
                var data = new byte[fs.Length];
                _ = fs.Read(data, 0, data.Length);
                jsonFileString = encoder.GetString(data);
            }, FileMode.Open, FileAccess.Read, FileShare.Read);

            return JsonToObject<TType>(jsonFileString, parserConfig);
        }

        /// <summary>
        ///     Method to save the created JSON from a parsed object to the desired file.
        /// </summary>
        /// <param name="objectItem">Specifies an object item for converting to JSON</param>
        /// <param name="filePath">Specifies a file path for reading.</param>
        /// <param name="parserConfig">Specifies a custom config for the parsing process.</param>
        /// <param name="encoder">Specifies the desired encoding for reading (By default, it's UTF8).</param>
        public static void ObjectToJsonFile(object objectItem, string filePath,
            G9DtJsonWriterConfig parserConfig = null, Encoding encoder = default)
        {
            PathValidation(filePath, false);

            if (Equals(encoder, default(Encoding)))
                encoder = Encoding.UTF8;

            var json = encoder.GetBytes(ObjectToJson(objectItem, parserConfig));

            G9Assembly.InputOutputTools.WaitForAccessToFile(filePath, fs => { fs.Write(json, 0, json.Length); },
                FileMode.Create, FileAccess.Write, FileShare.Write);
        }

        /// <summary>
        ///     Method to get total custom parser types that are existed and used.
        /// </summary>
        /// <returns>An array of existing custom parser types.</returns>
        public static Type[] GetTotalCustomParser()
        {
            Type PrepareType(object customParserOrType)
            {
                var type = customParserOrType.GetType();
                if (type.Name == "RuntimeType")
                    return (Type)customParserOrType;
                return type;
            }

            return G9CJsonCommon.CustomParserInstanceCollection.Select(s => PrepareType(s.Value)).ToArray();
        }


        /// <summary>
        ///     Path validation helper
        /// </summary>
        private static void PathValidation(string jsonFilePath, bool checkExsitance)
        {
            var result =
                G9Assembly.InputOutputTools.CheckFilePathValidation(jsonFilePath, true, checkExsitance, false);
            switch (result)
            {
                case G9EPatchCheckResult.PathNameIsIncorrect:
                    throw new ArgumentException(
                        $"The fixed value '{jsonFilePath}' in the specified parameter '{nameof(jsonFilePath)}' is incorrect regarding a directory path. The core can't use it as a directory path.",
                        nameof(jsonFilePath));
                case G9EPatchCheckResult.PathDriveIsIncorrect:
                    throw new ArgumentException(
                        $"The fixed value '{jsonFilePath}' in the specified parameter '{nameof(jsonFilePath)}' is incorrect regarding the directory drive. The specified drive doesn't exist.",
                        nameof(jsonFilePath));
                case G9EPatchCheckResult.PathExistenceIsIncorrect:
                    throw new ArgumentException(
                        $"The fixed value '{jsonFilePath}' in the specified parameter '{nameof(jsonFilePath)}' is incorrect regarding the path existence. The specified path doesn't exist.",
                        nameof(jsonFilePath));
                case G9EPatchCheckResult.Correct:
                default:
                    break;
            }
        }
    }
}