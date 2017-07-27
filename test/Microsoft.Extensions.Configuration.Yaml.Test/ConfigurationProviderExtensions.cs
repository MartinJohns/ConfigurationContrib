using System;

namespace Microsoft.Extensions.Configuration.Yaml.Test
{
    /// <summary>
    ///     Adapter from: https://github.com/aspnet/Configuration/blob/b2609af11b7e1083d9bc4e6e21178297efcf798c/test/Microsoft.Extensions.Configuration.Test.Common/ConfigurationProviderExtensions.cs
    /// </summary>
    internal static class ConfigurationProviderExtensions
    {
        public static string Get(this IConfigurationProvider provider, string key)
        {
            string value;

            if (!provider.TryGet(key, out value))
            {
                throw new InvalidOperationException("Key not found");
            }

            return value;
        }
    }
}
