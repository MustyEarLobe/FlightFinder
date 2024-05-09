using FlightFinder.Compatibility.Models;
using FlightFinder.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;

namespace FlightFinder.Web.Controllers
{
    public class HomeController : Controller
    {

        private readonly string BaseUrl = "https://localhost:44346/FlightFinder";
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index([FromQuery]string inputStr)
        {
            var model = new HomeModel();

            validateInput(inputStr, model);

            if (!string.IsNullOrWhiteSpace(model.OutputMessage) || (inputStr == null))
                return View(model);

            var request = new FlightFinderApiRequest { RequestItems = new List<FlightFinderApiRequestItem> { new FlightFinderApiRequestItem { Input = inputStr } }};
            var requestString = JsonConvert.SerializeObject(request);

            try
            {
                using (var findFlightConnection = new HttpClient())
                {
                    findFlightConnection.DefaultRequestHeaders.Accept.Clear();
                    findFlightConnection.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));

                    HttpResponseMessage getData = await findFlightConnection.PostAsync(new Uri(BaseUrl), new StringContent(requestString, Encoding.UTF8, "application/json"));
                
                    if (getData.IsSuccessStatusCode)
                    {
                        string results = getData.Content.ReadAsStringAsync().Result;
                        var flightFinderApiResponse = JsonConvert.DeserializeObject<FlightFinderApiResponse>(results);

                        if (flightFinderApiResponse != null && flightFinderApiResponse.ResponseItems.Count == 1)
                        {
                            model.OutputMessage = OutputMessages.OutputCountMessage(flightFinderApiResponse.ResponseItems.First().FlightCount);
                        }
                        else
                            model.OutputMessage = OutputMessages.SomethingWentWrong(OutputMessages.SWRReasoning.NoResponse);
                    }
                    else
                        model.OutputMessage = OutputMessages.SomethingWentWrong(OutputMessages.SWRReasoning.NoSuccessStatusCode);
                }
            }
            catch (Exception e)
            {
                model.OutputMessage = e.Message;

            }
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #region Validation

        private void validateInput(string inputStr, HomeModel model)
        {
            if (inputStr?.Length > 100)
                model.OutputMessage = OutputMessages.inputTooLong; 
            
            //Add More If needed Here
        }

        #endregion

        #region Output Messages

        protected static class OutputMessages
        {
            private static readonly string somethingWentWrong = "Something Went Wrong With The API";
            public static readonly string inputTooLong = "Input String Too Long, Must Be Less Than 100 Charaters";
            
            public static class SWRReasoning
            {
                public static readonly string NoResponse = ", No Response.";
                public static readonly string NoSuccessStatusCode = ", No Success Status Code From API.";
            }


            public static string OutputCountMessage(int count)
            {
                return string.Format("You Entered {0} Flight{1} Above.", count, count == 1 ? string.Empty : "s");
            }

            public static string SomethingWentWrong(string reason)
            {
                return string.Concat(OutputMessages.somethingWentWrong, reason);
            }
        }

        #endregion
    }
}
