using System;
using System.Threading.Tasks;
using Cpm.Core;
using Cpm.Core.Services.Mailing;
using Cpm.Infrastructure.Data;
using Cpm.Web.Extensions;
using Cpm.Web.PageHelpers;
using Cpm.Web.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cpm.Web.Pages.Admin.Users
{
    public class EditModel : StatusAwarePageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        [BindProperty]
        public InputModel Input { get; set; }

        public EditModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                if (!User.CanManageUsers())
                {
                    return Forbid();
                }

                Input = new InputModel
                {
                    SendWelcomeEmail = true,
                };
                return Page();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            Input = new InputModel
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsSuspended = user.IsSuspended,
                Note = user.Note,
            };

            if (!User.CanManageUsers())
            {
                return Forbid();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Input.IsNew)
            {
                if (!User.CanManageUsers())
                {
                    return Forbid();
                }

                var newUser = new ApplicationUser
                {
                    Email = Input.Email,
                    UserName = Input.Email,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    Created = Clock.Now.ToUniversalTime(),
                    IsSuspended = Input.IsSuspended,
                    AreaUnit = "ha",
                    AllocationUnit = "kg",
                    Note = Input.Note,
                };

                var creationResult = await _userManager.CreateAsync(newUser);

                if (!creationResult.Succeeded)
                {
                    return DisplayErrors(creationResult);
                }

                var optionalEmailStatus = " and a welcome email was sent";

                if (Input.SendWelcomeEmail)
                {
                    var code = await _userManager.GeneratePasswordResetTokenAsync(newUser);
                    await _emailSender.SendInvitationEmail(newUser.AsEmailRecipient(), newUser.Id, code);
                }

                SaveStatus()
                    .Success()
                    .Text("User ")
                    .Strong(newUser.GetDefaultDisplayName())
                    .Text($" was created{optionalEmailStatus}.")
                    .Static();

                return RedirectToPage("./Index");
            }

            var existingUser = await _userManager.FindByIdAsync(Input.Id);

            if (existingUser == null)
            {
                throw new InvalidOperationException($"Could not find user id {Input.Id} to be updated.");
            }

            if (!User.CanManageUsers())
            {
                return Forbid();
            }

            existingUser.FirstName = Input.FirstName;
            existingUser.LastName = Input.LastName;
            existingUser.IsSuspended = Input.IsSuspended;
            existingUser.Note = Input.Note;

            var updatingResult = await _userManager.UpdateAsync(existingUser);

            if (!updatingResult.Succeeded)
            {
                return DisplayErrors(updatingResult);
            }

            SaveStatus()
                .Success()
                .Text("User ")
                .Link(existingUser.GetDefaultDisplayName(), Url.EditUserUrl(existingUser.Id))
                .Text(" was updated.")
                .Static();

            return RedirectToPage("./Index");
        }

        private IActionResult DisplayErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }
    }
}