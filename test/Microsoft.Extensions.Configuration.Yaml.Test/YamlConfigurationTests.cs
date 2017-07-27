using System;
using System.Globalization;
using System.IO;
using Xunit;

namespace Microsoft.Extensions.Configuration.Yaml.Test
{
    public class YamlConfigurationTests
    {
        private YamlConfigurationProvider LoadProvider(string yaml)
        {
            var p = new YamlConfigurationProvider(new YamlConfigurationSource { Optional = true });
            p.Load(TestStreamHelpers.StringToStream(yaml));
            return p;
        }

        [Fact]
        public void LoadKeyValuePairsFromValidYaml()
        {
            var yaml = @"
                firstname: test
                test.last.name: last.name
                residential.address:
                    street.name: Something street
                    zipcode: 12345";
            var yamlConfigurationProvider = LoadProvider(yaml);
            
            Assert.Equal("test", yamlConfigurationProvider.Get("firstname"));
            Assert.Equal("last.name", yamlConfigurationProvider.Get("test.last.name"));
            Assert.Equal("Something street", yamlConfigurationProvider.Get("residential.address:STREET.name"));
            Assert.Equal("12345", yamlConfigurationProvider.Get("residential.address:zipcode"));
        }

        [Fact]
        public void LoadMethodCanHandleEmptyValue()
        {
            var yaml = @"
                name:";
            var yamlConfigurationProvider = LoadProvider(yaml);
            Assert.Equal(string.Empty, yamlConfigurationProvider.Get("name"));
        }

        [Fact]
        public void LoadWithCulture()
        {
            var previousCulture = CultureInfo.CurrentCulture;

            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("fr-FR");

                var yaml = @"
                  number: 3.14";
                var yamlConfigurationProvider = LoadProvider(yaml);
                Assert.Equal("3.14", yamlConfigurationProvider.Get("number"));
            }
            finally
            {
                CultureInfo.CurrentCulture = previousCulture;
            }
        }

        [Fact]
        public void SupportAndIgnoreComments()
        {
            var yaml = @"
                # Comments
                name: test # More comments
                address:
                    street: Something street # Even more comments
                    zipcode: 12345";
            var yamlConfigurationProvider = LoadProvider(yaml);
            Assert.Equal("test", yamlConfigurationProvider.Get("name"));
            Assert.Equal("Something street", yamlConfigurationProvider.Get("address:street"));
            Assert.Equal("12345", yamlConfigurationProvider.Get("address:zipcode"));
        }

        [Fact]
        public void ThrowExceptionWhenPassingNullAsFilePath()
        {
            Assert.Throws<ArgumentException>(() => new ConfigurationBuilder().AddYamlFile(path: null));
        }

        [Fact]
        public void ThrowExceptionWhenPassingEmptyStringAsFilePath()
        {
            Assert.Throws<ArgumentException>(() => new ConfigurationBuilder().AddYamlFile(string.Empty));
        }

        [Fact]
        public void YamlConfiguration_Throws_On_Missing_Configuration_File()
        {
            var config = new ConfigurationBuilder().AddYamlFile("NotExistingConfig.yml", optional: false);
            var exception = Assert.Throws<FileNotFoundException>(() => config.Build());

            // Assert
            Assert.StartsWith("The configuration file 'NotExistingConfig.yml' was not found and is not optional. The physical path is '", exception.Message);
        }

        [Fact]
        public void YamlConfiguration_Does_Not_Throw_On_Optional_Configuration()
        {
            new ConfigurationBuilder().AddYamlFile("NotExistingConfig.yml", optional: true).Build();
        }
    }
}
