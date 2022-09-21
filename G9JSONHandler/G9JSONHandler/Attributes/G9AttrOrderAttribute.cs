using System;

namespace G9JSONHandler.Attributes
{
    /// <summary>
    ///     Attribute for specifying the order of members in an object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field,
        // ReSharper disable once RedundantAttributeUsageProperty
        AllowMultiple = false)]
    public class G9AttrOrderAttribute : Attribute
    {
        /// <summary>
        ///     Specifies the order number
        /// </summary>
        public readonly uint OrderNumber;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="orderNumber">Specifies the order number</param>
        public G9AttrOrderAttribute(uint orderNumber)
        {
            OrderNumber = orderNumber;
        }
    }
}