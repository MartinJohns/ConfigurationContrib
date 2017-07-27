using System;
using System.Collections.Generic;
using Xunit;

namespace Microsoft.Extensions.Configuration.ImmutableBinder.Test
{
    public class ImmutableConfigurationBinderTests
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

        private class NestedOptions
        {
            public NestedOptions(string name, BasicOptions basic)
            {
                Name = name;
                Basic = basic;
            }

            public BasicOptions Basic { get; }

            public string Name { get; }
        }

        public class ComplexOptions
        {
            public ComplexOptions(string host, TimeSpan retentionTime, DateTime startDate, DateTime? endDate)
            {
                Host = host;
                RetentionTime = retentionTime;
                StartDate = startDate;
                EndDate = endDate;
            }

            public string Host { get; set; }

            public TimeSpan RetentionTime { get; set; }

            public DateTime StartDate { get; set; }

            public DateTime? EndDate { get; set; }
        }

        [Fact]
        public void CanBindBasicOptions()
        {
            // Arrange
            var dict = new Dictionary<string, string>
            {
                ["Section:Integer"] = "-2",
                ["Section:Boolean"] = "TRUe"
            };
            var configurationBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(dict);
            var config = configurationBuilder.Build();

            // Act
            var section = config.GetSection("Section");
            var options = section.ImmutableBind<BasicOptions>();

            // Assert
            Assert.NotNull(options);
            Assert.Equal(-2, options.Integer);
            Assert.True(options.Boolean);
        }


        [Fact]
        public void CanBindNestedOptions()
        {
            // Arrange
            var dict = new Dictionary<string, string>
            {
                ["Section:Basic:Integer"] = "-2",
                ["Section:Basic:Boolean"] = "TRUe",
                ["Section:Name"] = "Test"
            };
            var configurationBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(dict);
            var config = configurationBuilder.Build();

            // Act
            var section = config.GetSection("Section");
            var options = section.ImmutableBind<NestedOptions>();

            // Assert
            Assert.NotNull(options);
            Assert.Equal("Test", options.Name);
            Assert.NotNull(options.Basic);
            Assert.Equal(-2, options.Basic.Integer);
            Assert.True(options.Basic.Boolean);
        }

        [Fact]
        public void CanBindComplexOptions()
        {
            // Arrange
            var dict = new Dictionary<string, string>
            {
                ["Section:Host"] = "www.github.com",
                ["Section:RetentionTime"] = "00:20:00",
                ["Section:StartDate"] = "2017-07-27T00:06:00",
                ["Section:EndDate"] = ""
            };
            var configurationBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(dict);
            var config = configurationBuilder.Build();

            // Act
            var section = config.GetSection("Section");
            var options = section.ImmutableBind<ComplexOptions>();

            // Assert
            Assert.NotNull(options);
            Assert.Equal("www.github.com", options.Host);
            Assert.Equal(TimeSpan.FromMinutes(20), options.RetentionTime);
            Assert.Equal(DateTime.Parse("2017-07-27T00:06:00"), options.StartDate);
            Assert.Null(options.EndDate);
        }
    }
}
