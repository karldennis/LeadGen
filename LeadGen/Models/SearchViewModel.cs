using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using LeadGen.Core;

namespace LeadGen.Models
{
    public class SearchViewModel
    {
        private List<Lead> _leads;

        [Required]
        public string Terms { get; set; }
        
        [Required]
        public int Radius { get; set; }

        [Required]
        public string Location { get; set; }

        public string Name { get; set; }

        public int ListingsFound { get; set; }

        public List<Lead> Leads
        {
            get { return _leads ?? (_leads = new List<Lead>()); }
            set { _leads = value; }
        }
    }
}