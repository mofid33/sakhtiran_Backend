using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlace.API.Controllers.Panel
{
    public class FallBack : Controller
    {
        [HttpGet]   
        public IActionResult Index()
        {
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(),
            "wwwroot", "index.html"), "text/HTML");
        }

    }
}