using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LeadGen.Web.Controllers
{
    public class HomeController : ControllerBase
    {
        //
        // GET: /Landing/

        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "ScrapingJob");

            return View();
        }

    }
}
