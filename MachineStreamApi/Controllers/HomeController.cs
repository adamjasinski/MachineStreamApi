using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace MachineStreamApi.Controllers
{
    [Route("/")]
    public class HomeController
    {
        public IActionResult Get()
        {
            return new ContentResult
            {
                Content =
                @"<p>Machine stream event API</p>"
                + "<p>Please use /api/machines endpoints with a HTTP REST client</p>",
                ContentType = "text/html"
            };
        }
    }
}
