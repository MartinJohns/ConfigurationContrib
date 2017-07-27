using System;
using System.ComponentModel;
using System.Reflection;

namespace Microsoft.Extensions.Configuration.ImmutableBinder
{
    internal static class TypeExtensions
    {
        /// <remarks>
        ///     Code adapted from https://github.com/aspnet/Configuration/blob/7761ff841962b18c1767769ac71fa12cb91240a1/src/Microsoft.Extensions.Configuration.Binder/ConfigurationBinder.cs#L495-L514
        /// </remarks>
        internal static Type FindOpenGenericInterface(this Type actual, Type expected)
        {
            var actualTypeInfo = actual.GetTypeInfo();
            if (actual.IsGenericType
                && actual.GetGenericTypeDefinition() == expected)
            {
                return actual;
            }

            var interfaces = actualTypeInfo.ImplementedInterfaces;
            foreach (var interfaceType in interfaces)
            {
                if (interfaceType.GetTypeInfo().IsGenericType
                    && interfaceType.GetGenericTypeDefinition() == expected)
                {
                    return interfaceType;
                }
            }

            return null;
        }

        /// <remarks>
        ///     Code adapted from https://github.com/aspnet/Configuration/blob/7761ff841962b18c1767769ac71fa12cb91240a1/src/Microsoft.Extensions.Configuration.Binder/ConfigurationBinder.cs#L447-L481
        /// </remarks>
        internal static bool TryConvertValue(this Type type, string value, out object result, out Exception error)
        {
            error = null;
            result = null;

            if (type == typeof(object))
            {
                result = value;
                return true;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (string.IsNullOrEmpty(value))
                {
                    return true;
                }

                return TryConvertValue(Nullable.GetUnderlyingType(type), value, out result, out error);
            }

            var converter = TypeDescriptor.GetConverter(type);
            if (converter.CanConvertFrom(typeof(string)))
            {
                try
                {
                    result = converter.ConvertFromInvariantString(value);
                }
                catch (Exception ex)
                {
                    error = new InvalidOperationException($"Failed to convert '{value}' to type '{type}'.", ex);
                }

                return true;
            }

            return false;
        }
    }
}
