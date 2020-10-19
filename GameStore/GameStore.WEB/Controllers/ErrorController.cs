using GameStore.Web.Util;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Web.Controllers
{
    [CustomController("Game")]
    public class ErrorController : Controller
    {
        [Route("error/{statusCode}")]
        public IActionResult HandleErrorHttpStatus(int statusCode)
        {
            ViewBag.ErrorMessage = statusCode switch
            {
                404 => "404! This page is not found",
                500 => "500! The server has encountered a situation it doesn't know how to handle",
                _ => "Oooops! Something went wrong",
            };
            return View();
        }
    }
}
