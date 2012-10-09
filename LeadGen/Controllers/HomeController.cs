using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HtmlAgilityPack;
using LeadGen.Models;
using Newtonsoft.Json;
using Raven.Client;

namespace LeadGen.Controllers
{
    public class HomeController : Controller
    {
        public IDocumentSession RavenSession { get; protected set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            RavenSession = MvcApplication.Store.OpenSession();
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.IsChildAction)
                return;
        }

        public ActionResult Index()
        {
            return View(new YPSearchListingsJsonResultModel());
        }

        [HttpPost]
        public ViewResult Index(SearchOptions postedModel)
        {
            if (!ModelState.IsValid)
                return View(new YPSearchListingsJsonResultModel() {SearchOptions = postedModel});

            

            var baseUrl =
                "http://api2.yp.com/listings/v1/search?searchloc={0}&term={1}&format=json&sort=distance&radius={2}&listingcount=50&key=fdf83365b5c35e6dd5b02bd93b49c84b";

            baseUrl = string.Format(baseUrl, postedModel.Location, postedModel.Terms, postedModel.Radius);

            var request = (HttpWebRequest)WebRequest.Create(baseUrl);

            request.UserAgent = "Lead Gen Client";

            // Get the response.
            WebResponse response = request.GetResponse();
            
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            
            reader.Close();
            response.Close();
            
            var result = JsonConvert.DeserializeObject<YPSearchListingsJsonResultModel>(responseFromServer);

            result.SearchOptions = postedModel;

            foreach (var business in result.SearchResult.SearchListings.SearchListing)
            {
                var details = GetDetails(business.ListingId);

                foreach (var detail in details.ListingsDetailsResult.ListingsDetails.ListingDetail)
                {
                    business.Email = string.Join(",", (detail.ExtraEmails ?? new ExtraEmails()).ExtraEmail ?? new List<string>());

                    var websiteUrls = (detail.ExtraWebsiteUrls ?? new ExtraWebsiteUrls()).CleanedUrls() ??new List<string>();
                    business.Website = string.Join(",", websiteUrls );

                    foreach( var url in websiteUrls)
                    {
                        var foundEmail = FindEmail(url);

                        if( string.IsNullOrWhiteSpace(business.Email) && !string.IsNullOrWhiteSpace(foundEmail))
                        {
                            business.Email = "*" + foundEmail;
                        }
                            
                    }
                }
            }

            RavenSession.SaveChanges();

            return View(result);
        }

        public string FindEmail(string url)
        {
            try
            {
                HtmlWeb hw = new HtmlWeb();
                HtmlDocument htmlDocument = hw.Load(url);

                 foreach(HtmlNode link in htmlDocument.DocumentNode.SelectNodes("//a[@href]"))
                 {
                     HtmlAttribute att = link.Attributes["href"];
                     if( att.Value.StartsWith("mailto"))
                     {
                         return att.Value.Replace("mailto:", "");
                     }   

                     if( att.Value.Contains("contact"))
                     {
                         HtmlWeb cuhw = new HtmlWeb();
                         HtmlDocument cudocument = hw.Load(url);

                         foreach (HtmlNode culink in cudocument.DocumentNode.SelectNodes("//a[@href]"))
                         {
                             HtmlAttribute cuatt = culink.Attributes["href"];
                             if (cuatt.Value.StartsWith("mailto"))
                             {
                                 return cuatt.Value.Replace("mailto:", "**");
                             }
                         }

                     }
                 }

                    return "";
            }
            catch (Exception)
            {
                return "";
            }

        }

        public YPListingDetailsJsonResultModel GetDetails(string id)
        {
            var key = "fdf83365b5c35e6dd5b02bd93b49c84b";
            var baseUrl = string.Format("http://api2.yp.com/listings/v1/details?listingid={0}&key={1}&format=json", id, key);

            var result = RavenSession.Load<YPListingDetailsJsonResultModel>().FirstOrDefault(m => m.Id == baseUrl);
            
            if (result == null)
            {
                var request = (HttpWebRequest) WebRequest.Create(baseUrl);

                request.UserAgent = "Lead Gen Client";

                // Get the response.
                WebResponse response = request.GetResponse();

                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();

                reader.Close();
                response.Close();

                result = JsonConvert.DeserializeObject<YPListingDetailsJsonResultModel>(responseFromServer);

                result.Id = baseUrl;

                RavenSession.Store(result);
                
            }
            
            
            return result;
        }


    }
}
