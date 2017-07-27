# Configuration Contrib

Contributions to the aspnet/Configuration libraries.

## Microsoft.Extensions.Configuration.ImmutableBinder

Related to the `Microsoft.Extensions.Configuration.Binder` project this project provides a way to bind configurations to an immutable type. In the current version the type must simply provide a single constructor that will be invoked, passing all arguments from the configuration.

Example:

```csharp
// Example immutable options class
public class ComplexOptions
{
    // No default constructor - only one accepting all values
    public ComplexOptions(
        string host,
        TimeSpan retentionTime,
        DateTime startDate,
        DateTime? endDate)
    {
        Host = host;
        RetentionTime = retentionTime;
        StartDate = startDate;
        EndDate = endDate;
    }

    // Using getter-only auto-properties
    public string Host { get; }
    public TimeSpan RetentionTime { get; }
    public DateTime StartDate { get; }
    public DateTime? EndDate { get; }
}

// Preparing some example data
var dict = new Dictionary<string, string>
{
    ["Section:Host"] = "www.github.com",
    ["Section:RetentionTime"] = "00:20:00",
    ["Section:StartDate"] = "2017-07-27T00:06:00",
    ["Section:EndDate"] = ""
};
var configurationBuilder = new ConfigurationBuilder().AddInMemoryCollection(dict);
var config = configurationBuilder.Build();

// Retrieve the options
var options = section.ImmutableBind<ComplexOptions>(config.GetSection("Section"));
```
