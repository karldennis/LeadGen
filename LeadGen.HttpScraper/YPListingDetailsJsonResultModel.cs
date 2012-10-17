using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeadGen.Models
{
    public class YPListingDetailsJsonResultModel
    {
        public string Id { get; set; }
        public ListingDetailsResult ListingsDetailsResult { get; set; }   
    }

    public class ListingDetailsResult
    {
        public ListingDetails ListingsDetails { get; set; }
    }

    public class ListingDetails
    {
        public List<Actualdata> ListingDetail { get; set; }
        
    }

    public class Actualdata
    {
        public string Street { get; set; }
        public ExtraEmails ExtraEmails { get; set; }
        public ExtraWebsiteUrls ExtraWebsiteUrls { get; set; }
        
    }

    public class ExtraEmails
    {
        public List<string> ExtraEmail { get; set; }
    }

    public class ExtraWebsiteUrls
    {
        public List<string> CleanedUrls()
        {
            if (ExtraWebsiteUrl == null)
                return null;

            var cleanedUrls = new List<string>();
            foreach (var url in ExtraWebsiteUrl)
            {
                var urlDecoded = HttpUtility.UrlDecode(url);
                var cleanUrl = HttpUtility.ParseQueryString(new Uri(urlDecoded).Query)["dest"];

                cleanedUrls.Add(cleanUrl);
            }

            return cleanedUrls;
        }

        public List<string> ExtraWebsiteUrl { get; set; }
    }
   
}