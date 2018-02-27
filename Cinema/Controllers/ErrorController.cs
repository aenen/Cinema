using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using System.Web.UI;

namespace Cinema.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
    public class ErrorController : Controller
    {
        public ActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View();
        }
        

        public ActionResult BadRequest()
        {
            Response.StatusCode = 400;
            return View();
        }
    }
}