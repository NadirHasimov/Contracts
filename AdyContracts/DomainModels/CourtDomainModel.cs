using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdyContracts.DomainModels
{
    public class CourtDomainModel
    {
        public int id { get; set; }

        public int userId { get; set; }

        public DateTime trialDate { get; set; }

        public string location { get; set; }

        public string time { get; set; }

        public string fullname { get; set; }
    }
}