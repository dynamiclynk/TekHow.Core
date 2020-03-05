using TekHow.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TekHow.Core.Extensions
{
    /// <summary>
    /// System.Type Extensions
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Creates a default instance of the type.
        /// </summary>
        /// <param name="t">Type extension reference.</param>
        /// <returns>System.Object.</returns>
        /// <remarks>
        /// If the type is a value type or primitive, it will return a type that is
        /// that primitive or value type initialized to zero.
        /// 
        /// If the type is a complex type, and the type contains a default constructor
        /// it will return a default instance of that type. If the type does not have
        /// a default constructor, it will return null.
        /// </remarks>
        public static object CreateDefault(this Type t)
        {
            return ConvertAny.DefaultByType(t);
        }

        /// <summary>
        /// Determines whether this type can convert to the specified type.
        /// </summary>
        /// <param name="t">Type extension reference.</param>
        /// <param name="convertTo">The type to convert to.</param>
        /// <returns><c>true</c> if this instance can convert to the specified type; otherwise, <c>false</c>.</returns>
        public static bool CanConvertTo(this Type t, Type convertTo)
        {
            return ConvertAny.CanConvert(t, convertTo);
        }
    }
}
