using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Cpm.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace Cpm.Infrastructure.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "First name")]
        [MaxLength(100)]
        [Required]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        [MaxLength(100)]
        [Required]
        public string LastName { get; set; }

        public DateTime? LastLogin { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Activated { get; set; }

        public string AreaUnit { get; set; }

        public string AllocationUnit { get; set; }

        public bool IsSuspended { get; set; }

        public bool IsApplicationAdmin { get; set; }

        public string Note { get; set; }

        // Navigation

        public ICollection<SiteUserPermission> SitePermissions { get; set; }

        public ApplicationUser()
        {
            SitePermissions = new List<SiteUserPermission>();
        }

        public string GetDefaultDisplayName()
        {
            return string.IsNullOrWhiteSpace(FullName)
                ? string.IsNullOrWhiteSpace(Email)
                    ? "?"
                    : Email
                : FullName;
        }

        public string GetDefaultShortName()
        {
            return string.IsNullOrWhiteSpace(FirstName)
                ? string.IsNullOrWhiteSpace(Email)
                    ? "?"
                    : Email
                : FirstName;
        }

        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}
