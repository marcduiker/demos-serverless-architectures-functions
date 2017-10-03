using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;

namespace ImageAnalysisApp
{
    public class ComputerVisionHandler
    {
        protected const string Region = "westeurope";
        protected const string ApiKey = "";
        protected const string BaseCognitiveServicesVisionUrlV1 = ".api.cognitive.microsoft.com/vision/v1.0/";
        protected string ApiEndPoint;
        private readonly HttpClient _httpClient;

        public ComputerVisionHandler()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", $"{ ApiKey }");

            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["visualFeatures"] = "Description, Tags, Color, Faces";
            queryString["language"] = "en";
            ApiEndPoint = $"https://{ Region }{ BaseCognitiveServicesVisionUrlV1 }analyze?{ queryString }";

        }

        public async Task<JToken> AnalyzeImage(byte[] image)
        {
            using (var content = new ByteArrayContent(image))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                HttpResponseMessage response = await _httpClient.PostAsync(ApiEndPoint, content);
                Task<string> stringResponse = response.Content.ReadAsStringAsync();

                return JToken.Parse(stringResponse.Result);
            }
        }
    }
}
