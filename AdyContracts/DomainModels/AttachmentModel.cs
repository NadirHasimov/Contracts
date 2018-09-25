using AdyContracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdyContracts.DomainModels
{
    public class AttachmentModel
    {
        public string docNumber { get; set; }
        public string id { get; set; }
        public int typeId { get; set; }
        public string fileName { get; set; }
        public string filePath { get; set; }
        public string description { get; set; }
        public string conntentType { get; set; }
        public string contractType { get; set; }
        public string opreationsColumn { get; set; }
        public long FileSize { get; set; }
        public DateTime effectiveDate { get; set; }
        public string date { get; set; }

        public DocumentFilterModel filterModel { get; set; }
    }
}