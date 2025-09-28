namespace WebApiDemo.Models;

public class QuoteResponse
{
    public Dictionary<string, double> Quotes { get; set; } = new();
}
