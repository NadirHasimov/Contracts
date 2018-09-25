using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AdyContracts.Models
{
    public class AttachmentViewModel
    {
        public string id { get; set; }
        public int typeId { get; set; }
        public string fileName { get; set; }
        public string filePath { get; set; }
        public string description { get; set; }
        public IEnumerable<uint> uids { get; set; }

        public string conntentType { get; set; }
        public string contractType { get; set; }
        public long FileSize { get; set; }
        public DateTime effectiveDate { get; set; }
        public DocumentFilterModel filterModel { get; set; }
        public List<AttachmentViewModel> listOfAttachments { get; set; }
    }
}