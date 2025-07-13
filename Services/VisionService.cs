namespace OCR_Anthony.Services 
{
	using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
	using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
	using System.Threading.Tasks;
	public class VisionService
	{
		private readonly string _key;
		private readonly string _endpoint;

		public VisionService(IConfiguration config)
		{
			_key = config["AzureVision:Key"];
			_endpoint = config["AzureVision:Endpoint"];
		}

		public async Task<(string, string)> DescribeImageAsync(string imageUrl)
		{
			var client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(_key))
			{
				Endpoint = _endpoint
			};

			var features = new List<VisualFeatureTypes?> { VisualFeatureTypes.Description };
			var result = await client.AnalyzeImageAsync(imageUrl, features);

			var description = result.Description.Captions.FirstOrDefault()?.Text ?? "No description found.";

			// Aquí traducimos al inglés manualmente o con una API
			string descriptionEn = await Translate(description, "en");

			return (description, descriptionEn);
		}

		// Simulación de traducción. Reemplazar por una API real si se desea.
		private Task<string> Translate(string text, string toLanguage)
		{
			var diccionario = new Dictionary<string, string>
		{
			{ "un grupo de personas en una playa", "a group of people on a beach" },
			{ "una persona montando una bicicleta", "a person riding a bike" }
		};

			return Task.FromResult(diccionario.ContainsKey(text.ToLower()) ? diccionario[text.ToLower()] : text);
		}
	}
}
