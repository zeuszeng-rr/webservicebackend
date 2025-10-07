using Microsoft.AspNetCore.Mvc;

namespace WebServiceBackend.Api.Controllers
{
    [ApiController]
    [Route("/")]
    public class RootController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Web Service Backend is running!");
        }
    }
}
