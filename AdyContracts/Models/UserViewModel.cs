using AdyContracts.DomainModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Web;
using System.Web.Mvc;
using AdyContracts.Resources;

namespace AdyContracts.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public bool confirmationStatus { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string firstName { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string lastName { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string gender { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public DateTime birthdate { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public int departId { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        [Display(Name = "Email address")]
        [EmailAddress(ErrorMessage = "Invalid email address!")]
        public string email { get; set; }

        public bool emailSendStatus { get; set; }

        public string department { get; set; }

        public int? roleId { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        [MinLength(6, ErrorMessage = "Minimum length is 6.")]
        [Display(Name = "password", ResourceType = typeof(Resource))]
        public string password { get; set; }
        [Required]
        [Remote("CheckUsernameExists", "User", ErrorMessage = "This username already exists.Choose another one.")]
        [RegularExpression(@"(\S)+", ErrorMessage = "White space is not allowed.")]
        [MinLength(5, ErrorMessage = "Minimum length is 5.")]
        [Display(Name = "username", ResourceType = typeof(Resource))]
        //[Display(Name = "username", ResourceType = typeof(Resource))]
        public string username { get; set; }

        [Required(ErrorMessage = "Confirm your password.")]
        [System.ComponentModel.DataAnnotations.CompareAttribute("password", ErrorMessage = "Password doesn't match.")]
        public string ConfirmPassowrd { get; set; }
        public IEnumerable<SelectListItem> departments { get; set; }
        public List<UserViewModel> userList { get; set; }
        public List<MenuModel> menus { get; set; }

        [Required(ErrorMessage = "Daxil edilməlidir!")]
        public string roleName { get; set; }
    }
}