namespace OCR_Anthony.Services 
{
	using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
	using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
	using System.Text;
	using System.Threading.Tasks;
	public class OcrService
	{
		private readonly string _key;
		private readonly string _endpoint;

		public OcrService(IConfiguration config)
		{
			_key = config["AzureVision:Key"];
			_endpoint = config["AzureVision:Endpoint"];
		}

		public async Task<string> ExtractTextAsync(string imageUrl)
		{
			var client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(_key))
			{
				Endpoint = _endpoint
			};

			var textHeaders = await client.ReadAsync(imageUrl);
			string operationId = textHeaders.OperationLocation.Split('/').Last();

			ReadOperationResult results;
			do
			{
				await Task.Delay(1000);
				results = await client.GetReadResultAsync(Guid.Parse(operationId));
			} while (results.Status == OperationStatusCodes.Running || results.Status == OperationStatusCodes.NotStarted);

			if (results.Status != OperationStatusCodes.Succeeded)
				return "No se pudo extraer texto.";

			var sb = new StringBuilder();
			var lines = results.AnalyzeResult.ReadResults.SelectMany(r => r.Lines);
			foreach (var line in lines)
			{
				sb.AppendLine(line.Text);
			}

			return sb.ToString();
		}
	}
}
