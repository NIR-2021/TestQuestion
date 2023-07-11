using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TestQuestion.Models;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using System.Net.WebSockets;
using Microsoft.VisualBasic;

namespace TestQuestion.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public IConfiguration Iconfig { get; }

        public HomeController(ILogger<HomeController> logger, IConfiguration iconfig)
        {
            _logger = logger;
            Iconfig = iconfig;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            Console.WriteLine("Error caught at controller");
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> getSolution([FromBody] RequestModel rd)
        {
            string topic = rd.topic;
            string question = rd.question;
            Console.WriteLine("inside controller call " + topic);

            //topic = request.get
            var apiKey = Iconfig.GetValue<string>("APIKey");

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var jsnContent = new
            {
                prompt = question == null || question == "" ? $"what is {topic}" : $"{question}(search in the context of {topic}).",
                model = "text-davinci-003",
                max_tokens = 500
            };
            var requestData = new StringContent(JsonConvert.SerializeObject(jsnContent), Encoding.UTF8, "application/json");

            Console.WriteLine(await requestData.ReadAsStringAsync());

            var responseContent = await client.PostAsync("https://api.openai.com/v1/completions",requestData);
            
            var response = await responseContent.Content.ReadAsStringAsync();
            IDictionary<string,string> returnData = new Dictionary<string, string>();
            
            Console.WriteLine($"Response: {response}");
            try
            {
                Console.WriteLine("0");

                var data = JsonConvert.DeserializeObject<dynamic>(response);
                Console.WriteLine("1");

                var res = data!.choices[0].text;
                Console.WriteLine("2");
                returnData.Add("Result", res.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                returnData.Add("Error", ex.StackTrace!);

            }


            return Json(returnData);   
        }
    }
}