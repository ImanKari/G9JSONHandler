using System;
using System.Linq;
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
    }
}