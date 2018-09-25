using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdyContracts.Models
{
    public class DocumentFilterModel
    {
        public string description { get; set; }
        public string docNumber { get; set; }
        public string regGovNumber { get; set; }
        public DateTime? registrationDate1 { get; set; }
        public DateTime? registrationDate2 { get; set; }
        public DateTime? effectiveDate1 { get; set; }
        public DateTime? effectiveDate2 { get; set; }
        public string date { get; set; }
        public int[] docTypes { get; set; }
        public int[] receivers { get; set; }
        public int searchType { get; set; }
        public int exactSame { get; set; }
        public int searchOrder { get; set; }
        public int descendingOrder { get; set; }
        public int? status { get; set; }
    }
}