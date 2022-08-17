using System;
using G9AssemblyManagement.Interfaces;

namespace G9JSONHandler.Attributes
{
    /// <summary>
    ///     This attribute enables the definition of a custom parsing process for a member.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field,
        // ReSharper disable once RedundantAttributeUsageProperty
        AllowMultiple = false)]
    public class G9AttrJsonCustomMemberParsingProcessAttribute : Attribute
    {
        public G9AttrJsonCustomMemberParsingProcessAttribute(
            Func<G9IObjectMember, string> customParserForMemberValueToJson,
            Func<string, G9IObjectMember> customParserForJsonToMemberValue)
        {
            CustomParserForMemberValueToJson = customParserForMemberValueToJson ?? throw new ArgumentNullException(
                nameof(customParserForMemberValueToJson),
                $"The \"{nameof(customParserForMemberValueToJson)}\" used for the \"{nameof(G9AttrJsonCustomMemberParsingProcessAttribute)}\" argument can't be null.");
            CustomParserForJsonToMemberValue = customParserForJsonToMemberValue ?? throw new ArgumentNullException(
                nameof(customParserForJsonToMemberValue),
                $"The \"{nameof(customParserForJsonToMemberValue)}\" used for the \"{nameof(G9AttrJsonCustomMemberParsingProcessAttribute)}\" argument can't be null.");
        }

        public Func<G9IObjectMember, string> CustomParserForMemberValueToJson { get; }

        public Func<string, G9IObjectMember> CustomParserForJsonToMemberValue { get; }
    }
}