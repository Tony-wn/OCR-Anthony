using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using OCR_Anthony.Models;
using OCR_Anthony.Services;

namespace OCR_Anthony.Controllers
{
    public class HomeController : Controller
    {
		private readonly VisionService _visionService;

		public HomeController(VisionService visionService)
		{
			_visionService = visionService;
		}

		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Index(string imageUrl)
		{
			var (descEs, descEn) = await _visionService.DescribeImageAsync(imageUrl);
			var model = new DescripcionImagen
			{
				UrlOrPath = imageUrl,
				DescripcionEs = descEs,
				DescripcionEn = descEn
			};

			return View(model);
		}
	}
}
