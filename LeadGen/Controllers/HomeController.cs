using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using LeadGen.Application;
using LeadGen.Core;
using LeadGen.Http;
using LeadGen.Models;
using LeadGen.Q;
using Raven.Client;

namespace LeadGen.Controllers
{
    [Authorize]
    public class HomeController : ControllerBase
    {
        

        public ActionResult Index()
        {
            return View(new SearchViewModel());
        }

        [HttpPost]
        public ViewResult Index(SearchViewModel postedModel)
        {
            if (!ModelState.IsValid)
                return View(postedModel);

            var searchResult = new YellowPagesApi().SearchListings(postedModel.Location, postedModel.Terms,postedModel.Radius, 1);

            var leads = new List<Lead>();
            foreach (var listing in searchResult.SearchResult.SearchListings.SearchListing)
            {
                var lead = new Lead()
                               {
                                   BusinessName = listing.BusinessName,
                                   BaseClickUrl = listing.BaseClickUrl,
                                   MoreInfoUrl = listing.MoreInfoUrl,
                                   Phone = listing.Phone,
                                   PrimaryCategory = listing.PrimaryCategory,
                                   YpListingId = listing.ListingId

                               };

                leads.Add(lead);
            }
            postedModel.Leads = leads;
            postedModel.ListingsFound = searchResult.SearchResult.MetaProperties.totalAvailable;

            //RavenSession.SaveChanges();

            return View(postedModel);
        }

        [HttpPost]
        public ActionResult CreateJob(SearchViewModel postedModel)
        {
            var name = postedModel.Name;
            if(string.IsNullOrWhiteSpace(postedModel.Name))
            {
                name = postedModel.Terms + postedModel.Location + DateTime.Now;
            }

            var leadSearch = new LeadSearch()
                          {
                              Location = postedModel.Location,
                              Radius = postedModel.Radius,
                              Terms = postedModel.Terms,
                              UserId = CurrentUser.Id,
                              Name = name,
                              ListingsFound = postedModel.ListingsFound
                          };

            RavenSession.Store(leadSearch);
            RavenSession.SaveChanges();

            new QueueManager().AddListingSearch(leadSearch.Id);

            return RedirectToAction("Searches");
        }

        public ActionResult Searches()
        {
            var searches = RavenSession.Query<LeadSearch>().Where(ls=>ls.UserId == CurrentUser.Id);

            return View(searches);
        }

        public ActionResult LeadSearch(int id)
        {
            var leadSearch = RavenSession.Load<Core.LeadSearch>("leadsearches/67");

            return View(leadSearch);
        }

        public ActionResult AsCsv()
        {
            var leadSearch = RavenSession.Load<Core.LeadSearch>("leadsearches/67");

            var csv = new CsvExport<Lead>(leadSearch.Leads);
        
            return File(csv.ExportToBytes(), "text/csv", string.Format("Export-{0}.csv", DateTime.Now.ToString("yyyyMMdd-HHmmss")));
        }

        public ActionResult JobStatus()
        {
            return View(new QueueManager().GetListingQMessages());
        }




}
}
