using Microsoft.Extensions.Logging;
using System.Text.Json;
using WebApiDemo.Models;
using Newtonsoft.Json;

namespace WebApiDemo.Services;

public class QuoteService : IQuoteService
{
    private readonly IWebHostEnvironment _env;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<QuoteService> _logger;
    private Dictionary<string, List<string>> _providers;

    public QuoteService(IWebHostEnvironment env,
        IHttpContextAccessor httpContextAccessor,
        ILogger<QuoteService> logger)
    {
        _providers = new Dictionary<string, List<string>>();
        _env = env;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        ReadConfig();
    }


    public QuoteResponse CalculateQuotes(TopicsRequest request)
    {
        if (request.Topics == null || !request.Topics.Any())
            throw new ArgumentException("'topics' must contain at least one topic with numeric value");

        var lowercaseRequest = request.Topics.ToDictionary(kv => kv.Key.Trim().ToLowerInvariant(), kv => kv.Value);

        // Validate numeric values (non-negative)
        foreach (var kv in lowercaseRequest)
        {
            if (double.IsNaN(kv.Value) || double.IsInfinity(kv.Value) || kv.Value < 0)
                throw new ArgumentException($"value for topic '{kv.Key}' must be a non-negative number");
        }

        // Take top 3 topics by value
        var top3 = lowercaseRequest.OrderByDescending(kv => kv.Value).Take(3).ToList();
        if (top3.Count == 0)
            return new QuoteResponse();

        var rankMap = new Dictionary<string, int>();
        for (int i = 0; i < top3.Count; i++)
            rankMap[top3[i].Key] = i; 

        var quotes = new Dictionary<string, double>();

        foreach (var provider in _providers)
        {
            var matches = provider.Value.Where(t => rankMap.ContainsKey(t)).ToList();
            if (matches.Count == 0) continue;

            double quote = 0.0;
            if (matches.Count >= 2)
            {
                double total = matches.Sum(t => lowercaseRequest[t]);
                quote = 0.10 * total;
            }
            else
            {
                var topic = matches[0];
                var rank = rankMap[topic];
                var val = lowercaseRequest[topic];
                double pct = rank switch
                {
                    0 => 0.20,
                    1 => 0.25,
                    2 => 0.30,
                    _ => 0.0
                };
                quote = pct * val;
            }

            if (quote > 0)
                quotes[provider.Key] = Math.Round(quote, 2);
        }

        _logger.LogInformation("Computed quotes: {@Quotes}", quotes);
        return new QuoteResponse { Quotes = quotes };
    }

    private void ReadConfig() {
       

        var configPath =  Path.Combine(_env.ContentRootPath, "config", "config.json");
        if (!File.Exists(configPath))
            throw new FileNotFoundException("Provider config not found", configPath);

        var json = File.ReadAllText(configPath);
        var config = JsonConvert.DeserializeObject<ConfigObject>(json);
                
        if (config==null)
            throw new InvalidDataException("config.json must contain 'provider_topics'");

        foreach (var kvp in config.ProviderTopics)
        {
            var topicsStr = kvp.Value;
            var topics = topicsStr.Split('+', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                                  .Select(t => t.ToLowerInvariant())
                                  .ToList();
            _providers[kvp.Key] = topics;
        }
    }
}
