using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using WebApiDemo.Models;
using WebApiDemo.Services;

namespace WebApiDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DemoApiController : ControllerBase
    {
        private readonly IQuoteService _service;
        private readonly ILogger<DemoApiController> _logger;

        public DemoApiController(IQuoteService service, ILogger<DemoApiController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost]
        [SwaggerRequestExample(typeof(TopicsRequest), typeof(TopicsRequestExample))]
        public ActionResult<QuoteResponse> Post([FromBody] TopicsRequest request)
        {
            if (request?.Topics == null)
            {
                _logger.LogWarning("Bad request: missing topics");
                return BadRequest(new { error = "'topics' field is required and must be an object of topic -> numeric value" });
            }

            try
            {
                var response = _service.CalculateQuotes(request);

                if (response.Quotes.Count == 0) {
                    return Ok("No quote available");
                }
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error");
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}