using Microsoft.Extensions.Configuration;
using RepositoryContacts;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Repository
{
    public class FinnhubRepository : IFinnhubRepository
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public FinnhubRepository(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<Dictionary<string, object>?> GetCompanyProfile(string stockSymbol)
        {
            HttpClient httpClient = _httpClientFactory.CreateClient();
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage()
            {
                RequestUri = new Uri($"https://finnhub.io/api/v1/stock/profile2?symbol={stockSymbol}&token={_configuration["FinnhubToken"]}"),
                Method = HttpMethod.Get
            };
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
            Stream stream = await httpResponseMessage.Content.ReadAsStreamAsync();
            StreamReader reader = new StreamReader(stream);
            string responseBody = await reader.ReadToEndAsync();

            Dictionary<string, object>? responseDictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);
            if (responseDictionary == null)
            {
                throw new InvalidOperationException("No response from Finnhub server");
            }
            if (responseDictionary.ContainsKey("error"))
            {
                throw new InvalidOperationException(Convert.ToString(responseDictionary["error"]));
            }
            return responseDictionary;
        }

        public async Task<Dictionary<string, object>?> GetStockPriceQuote(string stockSymbol)
        {
            HttpClient httpClient = _httpClientFactory.CreateClient();
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage()
            {
                RequestUri = new Uri($"https://finnhub.io/api/v1/quote?symbol={stockSymbol}&token={_configuration["FinnhubToken"]}"),
                Method = HttpMethod.Get
            };
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
            Stream stream = await httpResponseMessage.Content.ReadAsStreamAsync();
            StreamReader streamReader = new StreamReader(stream);
            string responseBody = await streamReader.ReadToEndAsync();
            Dictionary<string, object>? responseDictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);
            if (responseDictionary == null)
            {
                throw new InvalidOperationException("No response from Finnhub server");
            }
            if (responseDictionary.ContainsKey("error"))
            {
                throw new InvalidOperationException(Convert.ToString(responseDictionary["error"]));
            }
            return responseDictionary;
        }

        public async Task<List<Dictionary<string, string>>?> GetStocks()
        {
            HttpClient httpClient = _httpClientFactory.CreateClient();
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage()
            {
                RequestUri = new Uri($"https://finnhub.io/api/v1/stock/symbol?exchange=US&token={_configuration["FinnhubToken"]}"),
                Method = HttpMethod.Get
            };
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
            Stream stream = await httpResponseMessage.Content.ReadAsStreamAsync();
            StreamReader streamReader = new StreamReader(stream);
            string responseBody = await streamReader.ReadToEndAsync();
            List<Dictionary<string, string>>? responseDictionary = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(responseBody);
            if (responseDictionary == null)
                throw new InvalidOperationException("No response from server");
            return responseDictionary;
        }

        public async Task<Dictionary<string, object>?> SearchStocks(string stockSymbolToSearch)
        {
            HttpClient httpClient = _httpClientFactory.CreateClient();
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage()
            {
                RequestUri= new Uri($"https://finnhub.io/api/v1/search?q={stockSymbolToSearch}&token={_configuration["FinnhubToken"]}"),
                Method = HttpMethod.Get
            };
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
            Stream stream= await httpResponseMessage.Content.ReadAsStreamAsync();
            StreamReader streamReader = new StreamReader(stream);
            string responseBody = await streamReader.ReadToEndAsync();
            Dictionary<string, object>? responseDictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);
            if (responseDictionary == null)
            {
                throw new InvalidOperationException("No response from Finnhub server");
            }
            if (responseDictionary.ContainsKey("error"))
            {
                throw new InvalidOperationException(Convert.ToString(responseDictionary["error"]));
            }
            return responseDictionary;
        }
    }
}