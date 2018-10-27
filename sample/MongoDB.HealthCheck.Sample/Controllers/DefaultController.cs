using Microsoft.AspNetCore.Mvc;

namespace MongoDB.HealthCheck.Sample.Controllers
{
	[Route("")]
	public class DefaultController : Controller
	{
		[HttpGet]
		public IActionResult Get() => Redirect("healthz");
	}
}
