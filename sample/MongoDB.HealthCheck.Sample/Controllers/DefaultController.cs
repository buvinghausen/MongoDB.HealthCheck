using Microsoft.AspNetCore.Mvc;

namespace MongoDB.HealthCheck.Sample.Controllers
{
	[Route("")]
	public sealed class DefaultController : Controller
	{
		[HttpGet]
		public IActionResult Get() => Redirect("healthz");
	}
}
