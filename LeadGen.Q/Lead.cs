using System.Collections.Generic;
using System.Linq;

namespace LeadGen.Core
{
    public class Lead
    {
        private List<string> _emails;
        private List<string> _websites;
        private List<string> _contactUsUris;
        public string BaseClickUrl { get; set; }
        public string BusinessName { get; set; }
        
        public string Phone { get; set; }
        public string PrimaryCategory { get; set; }
        public string MoreInfoUrl { get; set; }
        public string YpListingId { get; set; }

        public List<string> Emails
        {
            get
            {
                if( _emails != null && _emails.Any())
                {
                    return _emails.Distinct().ToList();
                }

                return _emails;
            }
            set { _emails = value; }
        }

        public List<string> Websites
        {
            get
            {
                if (_websites != null && _websites.Any())
                {
                    return _websites.Distinct().ToList();
                }

                return _websites;
            }
            set { _websites = value; }
        }

        public bool ScrappedWebsite { get; set; }
        public bool DetailsScrapped { get; set; }

        public List<string> ContactUsUris
        {
            get { return _contactUsUris ?? ( _contactUsUris = new List<string>()); }
            set { _contactUsUris = value; }
        }

        public bool WebsiteScraped { get; set; }

        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

        public string Street { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }

        public string Password { get; set; }
    }

    public class LeadSearch
    {
        private List<Lead> _leads;
        private int _ypPagesScraped;
        public int UserId { get; set; }

        public string Terms { get; set; }
        public int Radius { get; set; }
        public string Location { get; set; }

        public int Id { get; set; }

        //this is dumb, why do I set this to 1, maybe older records in db I forget
        public int YpPagesScraped
        {
            get
            {
                if( _ypPagesScraped == 0 )
                {
                    _ypPagesScraped = 1;
                }
                return _ypPagesScraped;
            }
            set { _ypPagesScraped = value; }
        }

        public bool YpDone { get; set; }

        public List<Lead> Leads
        {
            get { return _leads ?? ( _leads = new List<Lead>()); }
            set { _leads = value; }
        }

        public string Name { get; set; }

        public string Status
        {
            get
            {
                if (WebsitesScraped())
                {
                    return "3 of 3";
                }

                if (DetailsScraped())
                {
                    return "2 of 3";
                }

                if( ListingsScraped() )
                {
                    return "1 of 3";
                }

                return "Not started";
            }
        }

        public bool Finished
        {
            get { return WebsitesScraped() && DetailsScraped() && ListingsScraped(); }
        }

        private bool ListingsScraped()
        {
            return Leads.Count == this.ListingsFound;
        }

        private bool DetailsScraped()
        {
            return Leads.Any() && Leads.All(@l => @l.DetailsScrapped);
        }

        private bool WebsitesScraped()
        {
            return Leads.Any() && Leads.All(@l => @l.WebsiteScraped);
        }

        public int ListingsFound { get; set; }
    }
}