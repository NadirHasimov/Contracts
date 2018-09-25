using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdyContracts.Models
{
    public class ParagraphModel
    {
        public string orderNumber { get; set; }
        public string paragraphNumber { get; set; }
        public string paragraphText { get; set; }
        public string parent { get; set; }
    }
}