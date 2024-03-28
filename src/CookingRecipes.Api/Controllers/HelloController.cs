using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CookingRecipes.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HelloController : ControllerBase
    {
        [HttpGet]
        public IActionResult Hello() => Ok("Hello!");
    }
}
