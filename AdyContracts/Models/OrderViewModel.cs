using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdyContracts.Models
{
    public class OrderViewModel
    {
        [Required(ErrorMessage = "Zəhmət olmazsa, sənədin qeydiyyat tarixini daxil edin.")]
        public DateTime RegistrationDate { get; set; }

        [Required(ErrorMessage = "Zəhmət olmazsa, qeyd hissəsini doldurun.")]
        public string Description { get; set; }

        public DateTime EffectiveDate { get; set; }

        public string ReceiverId { get; set; }

        public string TerminationDocNumber { get; set; }

        public string[] Paragraphs { get; set; }
        public string[] UpdatedDocuments { get; set; }
    }
}