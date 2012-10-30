﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using LeadGen.Application;
using LeadGen.Core;
using LeadGen.Models;
using LeadGen.Q;

namespace LeadGen.Web.Controllers
{
    [Authorize]
    public class ScrapingJobController : ControllerBase
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
                                   YpListingId = listing.ListingId,
                                   Street = listing.Street,
                                   State = listing.State,
                                   City = listing.City,
                                   Zip = listing.Zip

                               };

                leads.Add(lead);
            }

            if (leads.Any())
            {
                postedModel.Leads = leads;
                postedModel.ListingsFound = Convert.ToInt32(searchResult.SearchResult.MetaProperties.totalAvailable);
            }

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
            var leadSearch = RavenSession.Load<Core.LeadSearch>("leadsearches/" + id);

            return View(leadSearch);
        }

        [HttpPost]
        public ActionResult AsCsv(int id, bool? onlyEmail)
        {
            var leadSearch = RavenSession.Load<Core.LeadSearch>("leadsearches/" + id);

            var csvLeads = new List<CsvLead>();

            var leads = leadSearch.Leads;

            if( onlyEmail.HasValue && onlyEmail == true )
            {
                leads = leads.Where(l => l.Emails.Any()).ToList();
            }

            foreach( var lead in leads )
            {
                var csvLead = new CsvLead()
                                  {
                                      Industry = lead.PrimaryCategory,
                                      Emails = string.Join("-", lead.Emails),
                                      Name = lead.BusinessName,
                                      Website = string.Join("-", lead.Websites),
                                      ContactUsUrls = string.Join("-", lead.ContactUsUris),
                                      Street = lead.Street,
                                      City = lead.City,
                                      State = lead.State,
                                      Zip = lead.Zip
                                  };

                csvLeads.Add(csvLead);
            }

            var csv = new CsvExport<CsvLead>(csvLeads);
        
            return File(csv.ExportToBytes(), "text/csv", string.Format("Export-{0}.csv", DateTime.Now.ToString("yyyyMMdd-HHmmss")));
        }

        public ActionResult JobStatus()
        {
            return View(new QueueManager().GetListingQMessages());
        }




}

    public class CsvLead
    {
        public string Industry { get; set; }
        public string Name { get; set; }
        public string Emails { get; set; }
        public string Website { get; set; }
        public string City { get; set; }
        public string ContactUsUrls { get; set; }
        public string Street { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
    }
}
