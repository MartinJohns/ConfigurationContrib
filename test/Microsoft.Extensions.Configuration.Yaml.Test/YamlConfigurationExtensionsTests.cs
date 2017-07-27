using System;
using System.IO;
using Xunit;

namespace Microsoft.Extensions.Configuration.Yaml.Test
{
    public class YamlConfigurationExtensionsTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void AddJsonFile_ThrowsIfFilePathIsNullOrEmpty(string path)
        {
            // Arrange
            var configurationBuilder = new ConfigurationBuilder();

            // Act and Assert
            var ex = Assert.Throws<ArgumentException>(() => configurationBuilder.AddYamlFile(path));
            Assert.Equal("path", ex.ParamName);
            Assert.StartsWith("File path must be a non-empty string.", ex.Message);
        }

        [Fact]
        public void AddJsonFile_ThrowsIfFileDoesNotExistAtPath()
        {
            // Arrange
            var path = "file-does-not-exist.json";

            // Act and Assert
            var ex = Assert.Throws<FileNotFoundException>(() => new ConfigurationBuilder().AddYamlFile(path).Build());
            Assert.StartsWith(
                $"The configuration file '{path}' was not found and is not optional. The physical path is '",
                ex.Message);
        }
    }
}
