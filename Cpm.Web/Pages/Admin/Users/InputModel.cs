using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Cpm.Web.Pages.Admin.Users
{
    public class InputModel
    {
        public string Id { get; set; }

        [Display(Name = "Email")]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "First name")]
        [MaxLength(100)]
        [Required]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        [MaxLength(100)]
        [Required]
        public string LastName { get; set; }

        public string Note { get; set; }

        [DisplayName("Is suspended?")]
        public bool IsSuspended { get; set; }

        [DisplayName("Send welcome email?")]
        public bool SendWelcomeEmail { get; set; }

        public bool IsNew => String.IsNullOrEmpty(Id);
    }
}