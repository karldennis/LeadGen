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
        private ExtraWebsiteUrls _extraWebsiteUrls;
        private ExtraEmails _extraEmails;
        public string Street { get; set; }
        public ExtraEmails ExtraEmails
        {
            get { return _extraEmails ?? ( _extraEmails = new ExtraEmails()); }
            set { _extraEmails = value; }
        }

        public ExtraWebsiteUrls ExtraWebsiteUrls
        {
            get { return _extraWebsiteUrls ?? (_extraWebsiteUrls = new ExtraWebsiteUrls()); }
            set { _extraWebsiteUrls = value; }
        }
    }

    public class ExtraEmails
    {
        private List<string> _extraEmail;
        public List<string> ExtraEmail
        {
            get { return _extraEmail ?? ( _extraEmail = new List<string>()); }
            set { _extraEmail = value; }
        }
    }

    public class ExtraWebsiteUrls
    {
        private List<string> _extraWebsiteUrl;

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

        public List<string> ExtraWebsiteUrl
        {
            get { return _extraWebsiteUrl ?? ( _extraWebsiteUrl = new List<string>()); }
            set { _extraWebsiteUrl = value; }
        }
    }
   
}