namespace OCR_Anthony.Services
{
	using System.Net.Http.Headers;
	using System.Text;
	using System.Text.Json;
	public class TranslationService
	{
		private readonly string _endpoint;
		private readonly string _key;
		private readonly string _region;
		private readonly HttpClient _httpClient;

		public TranslationService(IConfiguration config)
		{
			_endpoint = config["AzureTranslator:Endpoint"];
			_key = config["AzureTranslator:Key"];
			_region = config["AzureTranslator:Region"];
			_httpClient = new HttpClient();
		}

		public async Task<string> TranslateAsync(string text, string toLang)
		{
			var url = $"{_endpoint}/translate?api-version=3.0&to={toLang}";
			_httpClient.DefaultRequestHeaders.Clear();
			_httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _key);
			_httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Region", _region);
			_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			var body = new[] { new { Text = text } };
			var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

			var response = await _httpClient.PostAsync(url, content);
			var responseBody = await response.Content.ReadAsStringAsync();

			using var doc = JsonDocument.Parse(responseBody);
			return doc.RootElement[0].GetProperty("translations")[0].GetProperty("text").GetString();
		}
	}
}
