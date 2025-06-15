using Microsoft.AspNetCore.Mvc;

namespace visiom.Api.Search
{
    [ApiController]
    [Route("/api/[controller]")]
    public class SearchController : ControllerBase
    {

        [HttpGet]
        public IActionResult Search([FromQuery(Name = "q")] string query)
        {
            return Ok();
        }
    }
}
