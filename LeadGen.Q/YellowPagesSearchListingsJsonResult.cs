using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeadGen.Models
{
    public class YellowPagesSearchListingsJsonResult
    {
        private SearchResult _searchResult;
        public SearchResult SearchResult
        {
            get { return _searchResult ?? (_searchResult = new SearchResult()); }
            set { _searchResult = value; }
        }
    }

    public class SearchResult
    {
        private MetaProperties _metaProperties;
        public MetaProperties MetaProperties
        {
            get { return _metaProperties ?? (_metaProperties = new MetaProperties()); }
            set { _metaProperties = value; }
        }

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
        public string requestId { get; set; }
        public string resultCode { get; set; }
        public string searchCity { get; set; }
        public string searchLat { get; set; }
        public string searchLon { get; set; }
        public string searchState { get; set; }
        public string searchType { get; set; }
        public string searchZip { get; set; }
        public int totalAvailable { get; set; }
        public string trackingRequestURL{ get; set; }
        public string ypcAttribution { get; set; }

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