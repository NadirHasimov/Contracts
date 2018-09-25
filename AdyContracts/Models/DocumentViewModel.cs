using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdyContracts.Models
{
    public class DocumentViewModel
    {
        [Required]
        [Display(Name = "sənəd nömrəsi")]
        [Remote("CheckDocNumberExists", "Home", AdditionalFields = "id", ErrorMessage = "Bu nömrəli sənəd bazaya daxil edilib!")]
        public string id { get; set; }

        [Display(Name = "dövlət qeydiyyat nömrəsi")]
        public string govRegNumber { get; set; }

        [Required]
        [Display(Name = "qəbul edən")]
        public string ReceivingAuthorityId { get; set; }

        [Required]
        [Display(Name = "sənədin tipi")]
        public int type { get; set; }
        public string docType { get; set; }
        public string receiver { get; set; }
        public string fileName { get; set; }

        public bool status { get; set; }
        [Required]
        [Display(Name = "qeydiyyat tarixi")]
        public DateTime registrationDate { get; set; }

        public DateTime effectiveDate { get; set; }


        [Required]
        [AllowHtml]
        [Display(Name = "qeyd")]
        public string description { get; set; }

        [Required]
        [Display(Name = "attachment")]
        public HttpPostedFileBase file { get; set; }

        public HttpPostedFileBase pdfFile { get; set; }

        public string terminationContract { get; set; }

        public TemplateParameters templateParameters { get; set; }

        public AttachmentViewModel attachmentViewModel { get; set; }

        public OrderViewModel orderViewModel { get; set; }
        public string[] changedOrderDocs { get; set; }
        public string[] paragraphs { get; set; }
        public string rbtnTerminationType { get; set; }
        public string terminatedDocNumber { get; set; }
        public string paragraphText { get; set; }
        //public bool terminationStatus { get; set; }
    }

    public class TemplateParameters
    {
        [Required]
        [Display(Name = "müqavilənin adı")]
        [Remote("CheckDocNumberExists", "Home", ErrorMessage = "Bu nömrəli sənəd bazaya daxil edilib!")]
        public string documentName { get; set; }

        [Required]
        [Display(Name = "şirkətin adı")]
        public string companyName { get; set; }

        [Required]
        [Display(Name = "direktorun adı")]
        public string companyDirector { get; set; }

        [Required]
        [Display(Name = "ünvan")]
        public string companyAddress { get; set; }

        [Required]
        [Display(Name = "predmet")]
        public string companyPredmet { get; set; }
    }
}