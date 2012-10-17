using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeadGen.Models
{
    public class YellowPagesSearchListingsJsonResult
    {
        public SearchResult SearchResult { get; set; }
    }

    public class SearchResult
    {
        public MetaProperties MetaProperties { get; set; }
        public SearchListings SearchListings { get; set; }
    }

    public class SearchListings
    {
        public List<SearchListing> SearchListing { get; set; }
    }

    public class RelatedCategories
    {
        public List<RelatedCategory> RelatedCategory { get; set; }
    }

    public class RelatedCategory
    {
        public string Name { get; set; }
        public string Count { get; set; }
    }

    public class MetaProperties
    {
        public RelatedCategories RelatedCategories {get;set;}
    }

    public class SearchListing
    {
        public string BaseClickUrl { get; set; }
        public string BusinessName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string PrimaryCategory { get; set; }
        public string MoreInfoUrl { get; set; }
        public string ListingId { get; set; }
        public string Website { get; set; }
    }
}