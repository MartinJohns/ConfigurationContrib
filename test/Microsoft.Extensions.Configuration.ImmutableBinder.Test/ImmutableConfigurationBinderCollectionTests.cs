using System.Collections.Generic;
using Xunit;

namespace Microsoft.Extensions.Configuration.ImmutableBinder.Test
{
    public class ImmutableConfigurationBinderCollectionTests
    {
        private class BasicOptions
        {
            public BasicOptions(int integer, bool boolean)
            {
                Integer = integer;
                Boolean = boolean;
            }

            public int Integer { get; }

            public bool Boolean { get; }
        }

        private class DictionaryWithBasicOptions
        {
            public DictionaryWithBasicOptions(IReadOnlyDictionary<string, BasicOptions> entries)
            {
                Entries = entries;
            }

            public IReadOnlyDictionary<string, BasicOptions> Entries { get; }
        }

        private class PrimitiveCollectionOptions
        {
            public PrimitiveCollectionOptions(IReadOnlyCollection<string> entries)
            {
                Entries = entries;
            }

            public IReadOnlyCollection<string> Entries { get; }
        }

        private class PrimitiveDictionaryOptions
        {
            public PrimitiveDictionaryOptions(IReadOnlyDictionary<string, string> entries)
            {
                Entries = entries;
            }

            public IReadOnlyDictionary<string, string> Entries { get; }
        }

        [Fact]
        public void CanBindCollection()
        {
            // Arrange
            var dict = new Dictionary<string, string>
            {
                ["Section:Entries:0"] = "One",
                ["Section:Entries:1"] = "Two",
                ["Section:Entries:2"] = "Three"
            };

            var configurationBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(dict);
            var config = configurationBuilder.Build();

            // Act
            var section = config.GetSection("Section");
            var options = section.ImmutableBind<PrimitiveCollectionOptions>();

            // Assert
            Assert.NotNull(options);
            Assert.NotNull(options.Entries);
            Assert.Collection(
                options.Entries,
                e => Assert.Equal("One", e),
                e => Assert.Equal("Two", e),
                e => Assert.Equal("Three", e));
        }

        [Fact]
        public void CanBindEmptyCollection()
        {
            // Arrange
            var dict = new Dictionary<string, string>();

            var configurationBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(dict);
            var config = configurationBuilder.Build();

            // Act
            var section = config.GetSection("Section");
            var options = section.ImmutableBind<PrimitiveCollectionOptions>();

            // Assert
            Assert.NotNull(options);
            Assert.NotNull(options.Entries);
            Assert.Empty(options.Entries);
        }

        [Fact]
        public void CanBindDictionary()
        {
            // Arrange
            var dict = new Dictionary<string, string>
            {
                ["Section:Entries:one"] = "One",
                ["Section:Entries:two"] = "Two",
                ["Section:Entries:three"] = "Three"
            };

            var configurationBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(dict);
            var config = configurationBuilder.Build();

            // Act
            var section = config.GetSection("Section");
            var options = section.ImmutableBind<PrimitiveDictionaryOptions>();

            // Assert
            Assert.NotNull(options);
            Assert.NotNull(options.Entries);
            Assert.Equal(3, options.Entries.Count);
            Assert.Equal("One", options.Entries["one"]);
            Assert.Equal("Two", options.Entries["two"]);
            Assert.Equal("Three", options.Entries["three"]);
        }

        [Fact]
        public void CanBindEmptyDictionary()
        {
            // Arrange
            var dict = new Dictionary<string, string>();

            var configurationBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(dict);
            var config = configurationBuilder.Build();

            // Act
            var section = config.GetSection("Section");
            var options = section.ImmutableBind<PrimitiveDictionaryOptions>();

            // Assert
            Assert.NotNull(options);
            Assert.NotNull(options.Entries);
            Assert.Empty(options.Entries);
        }

        [Fact]
        public void CanBindDictionaryWithNestedOptions()
        {
            // Arrange
            var dict = new Dictionary<string, string>
            {
                ["Section:Entries:one:Integer"] = "-9",
                ["Section:Entries:one:Boolean"] = "trUe",
                ["Section:Entries:two:Integer"] = "-20",
                ["Section:Entries:two:Boolean"] = "tRuE",
            };

            var configurationBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(dict);
            var config = configurationBuilder.Build();

            // Act
            var section = config.GetSection("Section");
            var options = section.ImmutableBind<DictionaryWithBasicOptions>();

            // Assert
            Assert.NotNull(options);
            Assert.NotNull(options.Entries);
            Assert.Equal(2, options.Entries.Count);
            Assert.Equal(-9, options.Entries["one"].Integer);
            Assert.True(options.Entries["one"].Boolean);
            Assert.Equal(-20, options.Entries["two"].Integer);
            Assert.True(options.Entries["two"].Boolean);
        }

        [Fact]
        public void CanBindCollectionDirectly()
        {
            // Arrange
            var dict = new Dictionary<string, string>
            {
                ["Section:0"] = "one",
                ["Section:1"] = "two",
                ["Section:2"] = "three",
            };

            var configurationBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(dict);
            var config = configurationBuilder.Build();

            // Act
            var section = config.GetSection("Section");
            var options = section.ImmutableBind<IReadOnlyCollection<string>>();

            // Assert
            Assert.NotNull(options);
            Assert.Collection(
                options,
                e => Assert.Equal("one", e),
                e => Assert.Equal("two", e),
                e => Assert.Equal("three", e));
        }
    }
}
