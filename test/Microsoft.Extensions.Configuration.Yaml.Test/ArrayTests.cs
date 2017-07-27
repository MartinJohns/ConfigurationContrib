using System.Linq;
using Xunit;

namespace Microsoft.Extensions.Configuration.Yaml.Test
{
    public class ArrayTest
    {
        [Fact]
        public void ArraysAreConvertedToKeyValuePairs()
        {
            var yaml = @"
                ip:
                    - 1.2.3.4
                    - 7.8.9.10
                    - 11.12.13.14";

            var yamlConfigurationProvider = new YamlConfigurationProvider(new YamlConfigurationSource());
            yamlConfigurationProvider.Load(TestStreamHelpers.StringToStream(yaml));

            Assert.Equal("1.2.3.4", yamlConfigurationProvider.Get("ip:0"));
            Assert.Equal("7.8.9.10", yamlConfigurationProvider.Get("ip:1"));
            Assert.Equal("11.12.13.14", yamlConfigurationProvider.Get("ip:2"));
        }

        [Fact]
        public void ArrayOfObjects()
        {
            var yaml = @"
                ip:
                    - address: 1.2.3.4
                      hidden: False
                    - address: 5.6.7.8
                      hidden: True";

            var yamlConfigurationProvider = new YamlConfigurationProvider(new YamlConfigurationSource());
            yamlConfigurationProvider.Load(TestStreamHelpers.StringToStream(yaml));

            Assert.Equal("1.2.3.4", yamlConfigurationProvider.Get("ip:0:address"));
            Assert.Equal("False", yamlConfigurationProvider.Get("ip:0:hidden"));
            Assert.Equal("5.6.7.8", yamlConfigurationProvider.Get("ip:1:address"));
            Assert.Equal("True", yamlConfigurationProvider.Get("ip:1:hidden"));
        }

        [Fact]
        public void NestedArrays()
        {
            var yaml = @"
                ip:
                  - - 1.2.3.4
                    - 5.6.7.8
                  - - 9.10.11.12
                    - 13.14.15.16";

            var yamlConfigurationProvider = new YamlConfigurationProvider(new YamlConfigurationSource());
            yamlConfigurationProvider.Load(TestStreamHelpers.StringToStream(yaml));

            Assert.Equal("1.2.3.4", yamlConfigurationProvider.Get("ip:0:0"));
            Assert.Equal("5.6.7.8", yamlConfigurationProvider.Get("ip:0:1"));
            Assert.Equal("9.10.11.12", yamlConfigurationProvider.Get("ip:1:0"));
            Assert.Equal("13.14.15.16", yamlConfigurationProvider.Get("ip:1:1"));
        }

        [Fact]
        public void ImplicitArrayItemReplacement()
        {
            var yaml1 = @"
                ip:
                    - 1.2.3.4
                    - 7.8.9.10
                    - 11.12.13.14";

            var yaml2 = @"
                ip:
                    - 15.16.17.18";

            var yamlConfigurationSource1 = new YamlConfigurationSource { FileProvider = TestStreamHelpers.StringToFileProvider(yaml1) };
            var yamlConfigurationSource2 = new YamlConfigurationSource { FileProvider = TestStreamHelpers.StringToFileProvider(yaml2) };

            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.Add(yamlConfigurationSource1);
            configurationBuilder.Add(yamlConfigurationSource2);
            var config = configurationBuilder.Build();

            Assert.Equal(3, config.GetSection("ip").GetChildren().Count());
            Assert.Equal("15.16.17.18", config["ip:0"]);
            Assert.Equal("7.8.9.10", config["ip:1"]);
            Assert.Equal("11.12.13.14", config["ip:2"]);
        }

        [Fact]
        public void ExplicitArrayReplacement()
        {
            var yaml1 = @"
                ip:
                    - 1.2.3.4
                    - 7.8.9.10
                    - 11.12.13.14";

            var yaml2 = @"
                ip:
                    1: 15.16.17.18";

            var yamlConfigurationSource1 = new YamlConfigurationSource { FileProvider = TestStreamHelpers.StringToFileProvider(yaml1) };
            var yamlConfigurationSource2 = new YamlConfigurationSource { FileProvider = TestStreamHelpers.StringToFileProvider(yaml2) };

            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.Add(yamlConfigurationSource1);
            configurationBuilder.Add(yamlConfigurationSource2);
            var config = configurationBuilder.Build();

            Assert.Equal(3, config.GetSection("ip").GetChildren().Count());
            Assert.Equal("1.2.3.4", config["ip:0"]);
            Assert.Equal("15.16.17.18", config["ip:1"]);
            Assert.Equal("11.12.13.14", config["ip:2"]);
        }

        [Fact]
        public void ArrayMerge()
        {
            var yaml1 = @"
                ip:
                    - 1.2.3.4
                    - 7.8.9.10
                    - 11.12.13.14";

            var yaml2 = @"
                ip: 
                    3: 15.16.17.18";

            var yamlConfigurationSource1 = new YamlConfigurationSource { FileProvider = TestStreamHelpers.StringToFileProvider(yaml1) };
            var yamlConfigurationSource2 = new YamlConfigurationSource { FileProvider = TestStreamHelpers.StringToFileProvider(yaml2) };

            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.Add(yamlConfigurationSource1);
            configurationBuilder.Add(yamlConfigurationSource2);
            var config = configurationBuilder.Build();

            Assert.Equal(4, config.GetSection("ip").GetChildren().Count());
            Assert.Equal("1.2.3.4", config["ip:0"]);
            Assert.Equal("7.8.9.10", config["ip:1"]);
            Assert.Equal("11.12.13.14", config["ip:2"]);
            Assert.Equal("15.16.17.18", config["ip:3"]);
        }

        [Fact]
        public void ArraysAreKeptInFileOrder()
        {
            var yaml = @"
                setting:
                    - b
                    - a
                    - 2";

            var yamlConfigurationSource = new YamlConfigurationSource { FileProvider = TestStreamHelpers.StringToFileProvider(yaml) };

            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.Add(yamlConfigurationSource);
            var config = configurationBuilder.Build();

            var configurationSection = config.GetSection("setting");
            var indexConfigurationSections = configurationSection.GetChildren().ToArray();

            Assert.Equal(3, indexConfigurationSections.Count());
            Assert.Equal("b", indexConfigurationSections[0].Value);
            Assert.Equal("a", indexConfigurationSections[1].Value);
            Assert.Equal("2", indexConfigurationSections[2].Value);
        }
    }
}
