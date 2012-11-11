using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LeadGen.Core;

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

        public ActionResult Help()
        {
            return View();
        }

        public ActionResult Lol()
        {
            var se = RavenSession.Load<LeadSearch>().Where(@ls => @ls.FindListingsDuration != 0);

            var thing = se.Average(ls => ls.FindListingsDuration);
            var thing2 = se.Average(ls => ls.FindListingDetailsDuration);
            var thing3 = se.Average(ls => ls.ScrapeWebsitesForContactInformationDuration);
            
            return View();
        }

    }
}
