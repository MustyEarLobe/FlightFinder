using FlightFinder.Compatibility.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using FlightFinder.Compatibility.Text;


namespace FlightFinder.API
{
    public static class Utilities
    {
        public static FlightFinderApiResponse GetNewResponse(this FlightFinderApiRequest request)
        {
            var response = new FlightFinderApiResponse();
            response.ResponseItems = new List<FlightFinderApiResponseItem>();

            if (request.RequestItems == null) return response;

            foreach (var requestItem in request.RequestItems)
            {
                var responseItem = new FlightFinderApiResponseItem
                {
                    Input = requestItem.Input
                };

                response.ResponseItems.Add(responseItem);
            }

            return response;
        }

        public static void FindFlightCounts(FlightFinderApiRequest request, FlightFinderApiResponse response, ILogger logger)
        {
            foreach (var pair in request.PairRequestWithResponse(response))
            {
                FindFlightCount(pair, logger);
            }
        }

        private static void FindFlightCount(Tuple<FlightFinderApiRequestItem, FlightFinderApiResponseItem> pair, ILogger logger)
        {
            var charSetList = FindFlightLetterCounts(pair);

            if (charSetList.Count() != Constants.FlightChars.Count()) logger.LogError(Messages.SomethingWentWrongWithFindFlightLetterCounts);

            //The Item with the Least Char Count Dictates "Flight" Count
            pair.Item2.FlightCount = charSetList.OrderBy(c => c.Item2).First().Item2;
        }

        private static List<Tuple<char, int>> FindFlightLetterCounts(Tuple<FlightFinderApiRequestItem, FlightFinderApiResponseItem> pair)
        {
            if (pair.Item1.Input == null || pair.Item2.Input == null) {return new List<Tuple<char, int>>();}

            //Sanaitize, Update Data Type And Sort - allows batching
            var sortedInputByChar = pair.Item2.Input.ToUpper().Where(c => Constants.FlightChars.Contains(c)).OrderBy(c => c).ToArray();
            var charSetList = new List<Tuple<char, int>>();

            foreach (var c in Constants.FlightChars)
            {
                int startIndex = Array.IndexOf(sortedInputByChar, c);
                int endIndex = Array.LastIndexOf(sortedInputByChar, c);

                if (startIndex < 0 || endIndex < 0) { 
                    charSetList.Add(new Tuple<char, int>(c, 0));
                    continue;
                }

                var letters = sortedInputByChar.Skip(startIndex).Take(endIndex - startIndex + 1);

                charSetList.Add(new Tuple<char, int>(c, letters.Count()));
            }

            return charSetList;
        }

        public static List<Tuple<FlightFinderApiRequestItem, FlightFinderApiResponseItem>> PairRequestWithResponse(this FlightFinderApiRequest request, FlightFinderApiResponse response)
        {
            var pairs = new List<Tuple<FlightFinderApiRequestItem, FlightFinderApiResponseItem>>();

            foreach (var requestItem in request.RequestItems)
            {
                var responseItem = response.ResponseItems.FirstOrDefault(i => i.Input == requestItem.Input);

                if (responseItem == null) continue;                

                pairs.Add(Tuple.Create(requestItem, responseItem));
            }

            return pairs;
        }
    }
}
