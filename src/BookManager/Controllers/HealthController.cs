using Microsoft.AspNetCore.Mvc;

namespace BookManager.Controllers
{
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Health()
        {
            return Ok();
        }
    }
}
