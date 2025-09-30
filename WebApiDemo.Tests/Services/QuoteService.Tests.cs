using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using WebApiDemo.Services;
using WebApiDemo.Models;

public class QuoteServiceTests
{
    private readonly Mock<IWebHostEnvironment> _envMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly Mock<ILogger<QuoteService>> _loggerMock;
    private readonly string _configPath;
    private readonly string _configDir;

    public QuoteServiceTests()
    {
        _envMock = new Mock<IWebHostEnvironment>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _loggerMock = new Mock<ILogger<QuoteService>>();
        _configDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_configDir);
        _configPath = Path.Combine(_configDir, "config.json");
        _envMock.Setup(e => e.ContentRootPath).Returns(_configDir);
    }

    private void WriteConfig(Dictionary<string, string> providerTopics)
    {
        var configObj = new ConfigObject { ProviderTopics = providerTopics };
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(configObj);
        Directory.CreateDirectory(Path.Combine(_configDir, "config"));
        File.WriteAllText(Path.Combine(_configDir, "config", "config.json"), json);
    }

    [Fact]
    public void CalculateQuotes_ReturnsExpectedQuotes_ForValidInput()
    {
        WriteConfig(new Dictionary<string, string>
        {
            { "ProviderA", "topic1+topic2" },
            { "ProviderB", "topic2+topic3" }
        });

        var service = new QuoteService(_envMock.Object, _httpContextAccessorMock.Object, _loggerMock.Object);

        var request = new TopicsRequest
        {
            Topics = new Dictionary<string, double>
            {
                { "Topic1", 100 },
                { "Topic2", 50 },
                { "Topic3", 30 }
            }
        };

        var result = service.CalculateQuotes(request);

        Assert.Equal(2, result.Quotes.Count);
        Assert.Equal(15.0, result.Quotes["ProviderA"]);
        Assert.Equal(8.0, result.Quotes["ProviderB"]);
    }

    [Fact]
    public void CalculateQuotes_ThrowsArgumentException_WhenTopicsIsNullOrEmpty()
    {
        WriteConfig(new Dictionary<string, string> { { "ProviderA", "topic1" } });
        var service = new QuoteService(_envMock.Object, _httpContextAccessorMock.Object, _loggerMock.Object);

        var request = new TopicsRequest { Topics = null };
        Assert.Throws<ArgumentException>(() => service.CalculateQuotes(request));

        request.Topics = new Dictionary<string, double>();
        Assert.Throws<ArgumentException>(() => service.CalculateQuotes(request));
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(-1)]
    public void CalculateQuotes_ThrowsArgumentException_ForInvalidTopicValues(double value)
    {
        WriteConfig(new Dictionary<string, string> { { "ProviderA", "topic1" } });
        var service = new QuoteService(_envMock.Object, _httpContextAccessorMock.Object, _loggerMock.Object);

        var request = new TopicsRequest
        {
            Topics = new Dictionary<string, double> { { "Topic1", value } }
        };

        Assert.Throws<ArgumentException>(() => service.CalculateQuotes(request));
    }

    [Fact]
    public void CalculateQuotes_ReturnsEmptyQuotes_WhenNoProviderMatches()
    {
        WriteConfig(new Dictionary<string, string> { { "ProviderA", "topicX" } });
        var service = new QuoteService(_envMock.Object, _httpContextAccessorMock.Object, _loggerMock.Object);

        var request = new TopicsRequest
        {
            Topics = new Dictionary<string, double> { { "Topic1", 10 } }
        };

        var result = service.CalculateQuotes(request);
        Assert.Empty(result.Quotes);
    }

    [Fact]
    public void CalculateQuotes_CorrectlyCalculatesSingleMatchQuote()
    {
        WriteConfig(new Dictionary<string, string> { { "ProviderA", "topic1" } });
        var service = new QuoteService(_envMock.Object, _httpContextAccessorMock.Object, _loggerMock.Object);

        var request = new TopicsRequest
        {
            Topics = new Dictionary<string, double>
            {
                { "Topic1", 100 },
                { "Topic2", 50 }
            }
        };

        var result = service.CalculateQuotes(request);
        Assert.Single(result.Quotes);
        Assert.Equal(20.0, result.Quotes["ProviderA"]);
    }

    [Fact]
    public void Constructor_ThrowsFileNotFoundException_WhenConfigMissing()
    {
        var configDir = Path.Combine(_configDir, "config");
        if (Directory.Exists(configDir))
            Directory.Delete(configDir, true);

        Assert.Throws<FileNotFoundException>(() =>
            new QuoteService(_envMock.Object, _httpContextAccessorMock.Object, _loggerMock.Object));
    }


    [Fact]
    public void ReadConfig_LoadsProvidersCorrectly()
    {
        var providerTopics = new Dictionary<string, string>
        {
            { "ProviderA", "topic1+topic2" },
            { "ProviderB", "topic3" }
        };
        WriteConfig(providerTopics);

        var service = new QuoteService(_envMock.Object, _httpContextAccessorMock.Object, _loggerMock.Object);

        var providersField = typeof(QuoteService).GetField("_providers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var providers = (Dictionary<string, List<string>>)providersField.GetValue(service);

        Assert.Equal(2, providers.Count);
        Assert.Contains("topic1", providers["ProviderA"]);
        Assert.Contains("topic2", providers["ProviderA"]);
        Assert.Contains("topic3", providers["ProviderB"]);
    }
}