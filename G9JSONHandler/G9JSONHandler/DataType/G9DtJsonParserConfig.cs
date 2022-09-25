using System.Reflection;
using G9AssemblyManagement;
using G9AssemblyManagement.Enums;

namespace G9JSONHandler.DataType
{
    /// <summary>
    ///     The data type for setting the parser config.
    /// </summary>
    public class G9DtJsonParserConfig
    {
        /// <summary>
        ///     Specifies which modifiers will include in the searching process
        /// </summary>
        public readonly BindingFlags AccessibleModifiers;

        /// <summary>
        ///     Specifies that if in the parsing process a mismatch occurs, the exception (mismatch) on the member that has it must
        ///     be ignored or not.
        /// </summary>
        public readonly bool IgnoreMismatching;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="accessibleModifiers">
        ///     Specifies which modifiers will include in the searching process
        ///     <para />
        ///     By default, it's set to "BindingFlags.Instance | BindingFlags.Public"
        /// </param>
        /// <param name="ignoreMismatching">
        ///     Specifies that if in the parsing process a mismatch occurs, the exception (mismatch) on the member that has it must
        ///     be ignored or not.
        /// </param>
        public G9DtJsonParserConfig(BindingFlags accessibleModifiers = BindingFlags.Instance | BindingFlags.Public,
            bool ignoreMismatching = false)
        {
            AccessibleModifiers = accessibleModifiers;
            IgnoreMismatching = ignoreMismatching;
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="accessibleModifiers">
        ///     Specifies which modifiers will include in the searching process
        /// </param>
        /// <param name="ignoreMismatching">
        ///     Specifies that if in the parsing process a mismatch occurs, the exception (mismatch) on the member that has it must
        ///     be ignored or not.
        /// </param>
        public G9DtJsonParserConfig(G9EAccessModifier accessibleModifiers, bool ignoreMismatching = false)
        {
            AccessibleModifiers = G9Assembly.ReflectionTools.CreateCustomModifier(accessibleModifiers);
            IgnoreMismatching = ignoreMismatching;
        }
    }
}