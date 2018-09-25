using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AdyContracts.Models
{
    public class CourtViewModel
    {
        [Required(ErrorMessage = "Daxil edilməlidir!")]
        public int userId { get; set; }

        [Required(ErrorMessage = "Daxil edilməlidir!")]
        public DateTime trialDate { get; set; }

        [Required(ErrorMessage = "Daxil edilməlidir!")]
        public string location { get; set; }

        [Required(ErrorMessage = "Daxil edilməlidir!")]
        public string time { get; set; }

        public string fullname { get; set; }

        public List<CourtViewModel> CourtList{ get; set; }
    }
}