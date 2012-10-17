using System.IO;
using System.Net;
using LeadGen.Models;
using Newtonsoft.Json;

namespace LeadGen.Http
{
    public class YellowPagesApi
    {
        

        public YPListingDetailsJsonResultModel GetDetails(string id)
        {
            var key = "fdf83365b5c35e6dd5b02bd93b49c84b";
            var baseUrl = string.Format("http://api2.yp.com/listings/v1/details?listingid={0}&key={1}&format=json", id, key);

            //var result = RavenSession.Load<YPListingDetailsJsonResultModel>().FirstOrDefault(m => m.Id == baseUrl);
            YPListingDetailsJsonResultModel result = null;
            if (result == null)
            {
                var request = (HttpWebRequest)WebRequest.Create(baseUrl);

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

                //RavenSession.Store(result);

            }


            return result;
        }

        public YellowPagesSearchListingsJsonResult SearchListings(string location, string terms, int radius)
        {
            var baseUrl =
                "http://api2.yp.com/listings/v1/search?searchloc={0}&term={1}&format=json&sort=distance&radius={2}&listingcount=50&key=fdf83365b5c35e6dd5b02bd93b49c84b";

            baseUrl = string.Format(baseUrl, location, terms, radius);

            var request = (HttpWebRequest)WebRequest.Create(baseUrl);

            request.UserAgent = "Lead Gen Client";

            // Get the response.
            WebResponse response = request.GetResponse();

            var dataStream = response.GetResponseStream();
            var reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();

            reader.Close();
            response.Close();

            var result = JsonConvert.DeserializeObject<YellowPagesSearchListingsJsonResult>(responseFromServer);

            //foreach (var business in result.SearchResult.SearchListings.SearchListing)
            //{
                //var details = GetDetails(business.ListingId);

                //foreach (var detail in details.ListingsDetailsResult.ListingsDetails.ListingDetail)
                //{
                //    business.Email = string.Join(",", (detail.ExtraEmails ?? new ExtraEmails()).ExtraEmail ?? new List<string>());

                //    var websiteUrls = (detail.ExtraWebsiteUrls ?? new ExtraWebsiteUrls()).CleanedUrls() ?? new List<string>();
                //    business.Website = string.Join(",", websiteUrls);

                    //foreach (var url in websiteUrls)
                    //{
                    //    var foundEmail = ScrapePageForEmail(url);

                    //    if (string.IsNullOrWhiteSpace(business.Email) && !string.IsNullOrWhiteSpace(foundEmail))
                    //    {
                    //        business.Email = "*" + foundEmail;
                    //    }

                    //}
              //  }
            //}

            return result;
        }
    }
}
