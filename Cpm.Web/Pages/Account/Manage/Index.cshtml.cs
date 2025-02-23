using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cpm.Core.Services;
using Cpm.Core.Services.Mailing;
using Cpm.Core.ViewModels;
using Cpm.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Cpm.Web.PageHelpers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cpm.Web.Pages.Account.Manage
{
    public class IndexModel : StatusAwarePageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserPreferences _userPreferences;
        private readonly IUserPreferencesFactory _userPreferencesFactory;
        private readonly IEmailSender _emailSender;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            IUserPreferences userPreferences,
            IUserPreferencesFactory userPreferencesFactory,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userPreferences = userPreferences;
            _userPreferencesFactory = userPreferencesFactory;
            _emailSender = emailSender;
        }

        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "First name")]
            [MaxLength(100)]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last name")]
            [MaxLength(100)]
            public string LastName { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Required]
            [Display(Name = "Preferred area unit")]
            public string AreaUnit { get; set; }

            [Required]
            [Display(Name = "Preferred allocation unit")]
            public string AllocationUnit { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            Username = user.UserName;

            Input = new InputModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                AreaUnit = user.AreaUnit,
                AllocationUnit = user.AllocationUnit,
            };

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

            AvailableAreaUnits = _userPreferencesFactory.AvailableAreaUnits()
                .Select(ToSelectListItem);

            AvailableAllocationUnits = _userPreferencesFactory.AvailableAllocationUnits()
                .Select(ToSelectListItem);

            return Page();
        }

        public IEnumerable<SelectListItem> AvailableAllocationUnits { get; set; }

        private static SelectListItem ToSelectListItem(OptionVm x)
        {
            return new SelectListItem
            {
                Value = x.Value,
                Text = x.Description,
            };
        }

        public IEnumerable<SelectListItem> AvailableAreaUnits { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (Input.Email != user.Email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, Input.Email);
                if (!setEmailResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting email for user with ID '{user.Id}'.");
                }
            }

            if (Input.PhoneNumber != user.PhoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting phone number for user with ID '{user.Id}'.");
                }
            }

            if (Input.FirstName != user.FirstName || Input.LastName != user.LastName)
            {
                user.FirstName = Input.FirstName;
                user.LastName = Input.LastName;
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred updating real names for user with ID '{user.Id}'.");
                }
            }

            if (Input.AreaUnit != user.AreaUnit)
            {
                user.AreaUnit = Input.AreaUnit;
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred while updating area unit for user with ID '{user.Id}'.");
                }
            }

            if (Input.AllocationUnit != user.AllocationUnit)
            {
                user.AllocationUnit = Input.AllocationUnit;
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred while updating allocation unit for user with ID '{user.Id}'.");
                }
            }

            SaveStatus()
                .Success()
                .Text("Your profile has been updated")
                .Dismissible();

            return RedirectToPage();
        }
    }
}
