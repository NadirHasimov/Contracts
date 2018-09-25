using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdyContracts.DomainModels
{
    public class ContractModel
    {
        public int id { get; set; }
        public int type { get; set; }
        public DateTime registrationDate { get; set; }
        public DateTime admissionDate { get; set; }
        public DateTime effectiveDate { get; set; }
        public string description { get; set; }
    }
}