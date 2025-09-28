using WebApiDemo.Models;

namespace WebApiDemo.Services;

public interface IQuoteService
{
    QuoteResponse CalculateQuotes(TopicsRequest request);
}
