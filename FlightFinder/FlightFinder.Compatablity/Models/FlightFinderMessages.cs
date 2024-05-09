using System;
using System.Collections.Generic;

namespace FlightFinder.Compatibility.Models
{

    public class FlightFinderApiRequest
    {
        public List<FlightFinderApiRequestItem> RequestItems { get; set; }
    }
    public class FlightFinderApiRequestItem
    {
        public string Input { get; set; }
    }
    public class FlightFinderApiResponse
    {
        public List<FlightFinderApiResponseItem> ResponseItems { get; set; }
    }

    public class FlightFinderApiResponseItem
    {
        public string Input { get; set; }
        public int FlightCount { get; set; }
    }
}
