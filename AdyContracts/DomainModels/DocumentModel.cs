using AdyContracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdyContracts.DomainModels
{
    public class DocumentModel
    {
        public string docNumber { get; set; }

        public string govRegNumber { get; set; }

        public int receivingAuthorityId { get; set; }
        public string receiver { get; set; }

        public int typeId { get; set; }
        public string type { get; set; }

        public bool status { get; set; }

        public DateTime registrationDate { get; set; }

        public DateTime effectiveDate { get; set; }

        public string description { get; set; }

        public AttachmentModel attachmentModel { get; set; }
        public string fileName { get; set; }
        public TemplateParameters templateParameters { get; set; }
        public string terminatedDocNumber { get; set; }

        public string[] changedOrders { get; set; }
        public string rbtnTerminationType { get; set; }

    }
}