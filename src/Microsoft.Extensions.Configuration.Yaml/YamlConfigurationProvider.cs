using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.Core;

namespace Microsoft.Extensions.Configuration.Yaml
{
    /// <summary>
    ///     A JSON file based <see cref="FileConfigurationProvider"/>.
    /// </summary>
    public class YamlConfigurationProvider : FileConfigurationProvider
    {
        /// <summary>
        ///     Initializes a new instance with the specified source.
        /// </summary>
        /// <param name="source">
        ///     The source settings.
        /// </param>
        public YamlConfigurationProvider(YamlConfigurationSource source)
            : base(source)
        {
        }
        
        /// <summary>
        ///     Loads the YAML data from a stream.
        /// </summary>
        /// <param name="stream">
        ///     The stream to read.
        /// </param>
        public override void Load(Stream stream)
        {
            var parser = new YamlConfigurationFileParser();
            try
            {
                Data = parser.Parse(stream);
            }
            catch (YamlException ex)
            {
                string errorLine = string.Empty;
                if (stream.CanSeek)
                {
                    stream.Seek(0, SeekOrigin.Begin);

                    using (var streamReader = new StreamReader(stream))
                    {
                        var fileContent = ReadLines(streamReader);
                        errorLine = RetrieveErrorContext(ex, fileContent);
                    }
                }

                throw new FormatException(
                    "Could not parse the YAML file. " +
                    $"Error on line number '{ex.Start.Line}': '{errorLine}'.", ex);
            }
        }

        private static string RetrieveErrorContext(YamlException ex, IEnumerable<string> fileContent)
        {
            var possibleLineContent = fileContent.Skip(ex.Start.Line - 1).FirstOrDefault();
            return possibleLineContent ?? string.Empty;
        }

        private static IEnumerable<string> ReadLines(StreamReader streamReader)
        {
            string line;
            do
            {
                line = streamReader.ReadLine();
                yield return line;
            } while (line != null);
        }
    }
}
