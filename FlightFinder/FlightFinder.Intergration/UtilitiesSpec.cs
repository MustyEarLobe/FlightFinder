using FlightFinder.API;
using FlightFinder.Compatibility.Models;
using Microsoft.Extensions.Logging;

namespace FlightFinder.Intergration
{    
    [TestFixture]
    public class UtilitiesSpec
    {
        FlightFinderApiRequest _request = new FlightFinderApiRequest { RequestItems = new List<FlightFinderApiRequestItem> { new FlightFinderApiRequestItem { Input = "FlightFlight" }, new FlightFinderApiRequestItem { Input = "Fl" }, new FlightFinderApiRequestItem { Input = "FFFLLLIIIGHTGHTGHT" } } };


        [Test]
        public void GetNewResponseShouldCreateResponseWithResponseItemsForEachRequestItems()
        {
            var response = _request.GetNewResponse();

            Assert.That(_request.RequestItems.Count(), Is.EqualTo(response.ResponseItems.Count()));

            foreach (var item in response.ResponseItems)
            {
                var matchedRequestItem = _request.RequestItems.Where(x => x.Input == item.Input);
                Assert.That(matchedRequestItem, Is.Not.EqualTo(null));
                Assert.That(matchedRequestItem.Count() == 1);
            }
        }

        [Test]
        public void PairRequestWithResponseShouldPairEachResponseItemsForAllRequestItems()
        {
            var response = _request.GetNewResponse();

            var pairs = _request.PairRequestWithResponse(response);

            Assert.That(_request.RequestItems.Count(), Is.EqualTo(pairs.Count()));
            Assert.That(response.ResponseItems.Count(), Is.EqualTo(pairs.Count()));


            foreach (var pair in pairs)
            {
                Assert.That(_request.RequestItems.Contains(pair.Item1));
                Assert.That(response.ResponseItems.Contains(pair.Item2));
            }
        }

        [Test]
        public void FindFlightCountShouldUpdateResponseItemsCorrectlyWithOutError()
        {
            using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger logger = factory.CreateLogger("Test");
            var response = _request.GetNewResponse();

            Utilities.FindFlightCounts(_request, response, logger);

            Assert.That(response.ResponseItems[0].FlightCount == 2);
            Assert.That(response.ResponseItems[1].FlightCount == 0);
            Assert.That(response.ResponseItems[2].FlightCount == 3);

            foreach (var item in response.ResponseItems)
            {
                var matchedRequestItem = _request.RequestItems.Where(x => x.Input == item.Input);
                Assert.That(matchedRequestItem, Is.Not.EqualTo(null));
                Assert.That(matchedRequestItem.Count() == 1);
            }
        }
    }
}
