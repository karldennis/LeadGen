using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LeadGen.Models
{
    public class SearchOptions
    {
        [Required]
        public string Terms { get; set; }
        
        [Required]
        public int Radius { get; set; }

        [Required]
        public string Location { get; set; }
    }
}