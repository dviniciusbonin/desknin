using System.ComponentModel.DataAnnotations;
using DeskNin.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace DeskNin.Areas.Identity.Pages.Account;

public class ForgotPasswordModel(
    UserManager<IdentityUser> userManager,
    IAppSettingsService appSettingsService,
    IAppEmailSender appEmailSender,
    IEmailTemplateService emailTemplateService,
    ILogger<ForgotPasswordModel> logger) : PageModel
{
    private readonly UserManager<IdentityUser> _userManager = userManager;
    private readonly IAppSettingsService _appSettingsService = appSettingsService;
    private readonly IAppEmailSender _appEmailSender = appEmailSender;
    private readonly IEmailTemplateService _emailTemplateService = emailTemplateService;
    private readonly ILogger<ForgotPasswordModel> _logger = logger;

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public bool EmailSent { get; private set; }

    public sealed class InputModel
    {
        [Required(ErrorMessage = "The Email field is required.")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return Page();

        var user = await _userManager.FindByEmailAsync(Input.Email);
        if (user == null)
        {
            // Keep response neutral to avoid account enumeration.
            EmailSent = true;
            return Page();
        }

        if (!await _appSettingsService.IsEmailEnabledAsync(cancellationToken))
        {
            EmailSent = true;
            return Page();
        }

        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedCode = WebEncoders.Base64UrlEncode(System.Text.Encoding.UTF8.GetBytes(code));
        var callbackUrl = Url.Page(
            "/Account/ResetPassword",
            pageHandler: null,
            values: new { area = "Identity", code = encodedCode, email = user.Email },
            protocol: Request.Scheme);

        var body = _emailTemplateService.BuildForgotPasswordBody(callbackUrl ?? string.Empty);

        try
        {
            await _appEmailSender.SendEmailAsync(
                user.Email!,
                "DeskNin password reset",
                body,
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send forgot-password email for user {UserId}", user.Id);
        }

        EmailSent = true;
        return Page();
    }
}
