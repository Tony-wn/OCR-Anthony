using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using OCR_Anthony.Models;
using OCR_Anthony.Services;

namespace OCR_Anthony.Controllers
{
    public class HomeController : Controller
    {
		public class HomeController : Controller
		{
			private readonly OcrService _ocrService;
			private readonly TranslationService _translationService;
			private readonly IWebHostEnvironment _env;

			public HomeController(OcrService ocrService, TranslationService translationService, IWebHostEnvironment env)
			{
				_ocrService = ocrService;
				_translationService = translationService;
				_env = env;
			}

			[HttpGet]
			public IActionResult Index()
			{
				return View();
			}

			[HttpPost]
			public async Task<IActionResult> Index(string imageUrl, IFormFile imageFile)
			{
				string imagePath = null;

				if (imageFile != null && imageFile.Length > 0)
				{
					var uploads = Path.Combine(_env.WebRootPath, "uploads");
					Directory.CreateDirectory(uploads);
					var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
					var filePath = Path.Combine(uploads, fileName);
					using (var stream = new FileStream(filePath, FileMode.Create))
					{
						await imageFile.CopyToAsync(stream);
					}

					imagePath = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";
				}
				else if (!string.IsNullOrEmpty(imageUrl))
				{
					imagePath = imageUrl;
				}

				if (string.IsNullOrEmpty(imagePath))
					return View();

				var text = await _ocrService.ExtractTextAsync(imagePath);
				var translated = await _translationService.TranslateAsync(text, "en");

				var model = new ResultadoOCR
				{
					ImagePath = imagePath,
					OriginalText = text,
					TranslatedText = translated
				};

				return View(model);
			}
		}

	}
}
