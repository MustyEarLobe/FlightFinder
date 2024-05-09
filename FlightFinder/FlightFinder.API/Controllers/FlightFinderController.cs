using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FlightFinder.Compatibility.Models;
using FlightFinder.Compatibility.Text;
using System.Linq;

namespace FlightFinder.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FlightFinderController
    {
        private readonly ILogger<FlightFinderController> _logger;

        public FlightFinderController(ILogger<FlightFinderController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public FlightFinderApiResponse GetFlightCount([FromBody] FlightFinderApiRequest request)
        {
            var response = request.GetNewResponse();

            if (Validate(request, response))
                Utilities.FindFlightCounts(request, response, _logger);

            return response;
        }

        private bool Validate(FlightFinderApiRequest request, FlightFinderApiResponse response)
        {
            if (request == null || !request.RequestItems.Any())
            {
                _logger.LogError(Messages.NoRequestItemInRequest);
                return false;
            }
            if (request.RequestItems.Any(r => r.Input.Length > 100))
            {
                _logger.LogError(Messages.TooManyCharInRequestItemInput);
                return false;
            }

            if (request.RequestItems.Any(r => string.IsNullOrEmpty(r.Input)))
            {
                _logger.LogError(Messages.NullInputRequest);
                return false;
            }

            return true;
        }
    }
}
