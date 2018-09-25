using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdyContracts.DomainModels
{
    public class CourtNotificationModel
    {
        public string Court_id { get; set; }
        public string Fullname { get; set; }
        public string Day { get; set; }
        public string Message { get; set; }
    }
}