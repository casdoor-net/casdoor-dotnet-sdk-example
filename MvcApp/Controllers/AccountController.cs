using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MvcApp.Controllers;

public class AccountController : Controller
{
    // Ends the session of this app. The user stays signed in to Casdoor,
    // so signing in again doesn't ask for the password.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SignOutUser()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(SignedOut));
    }

    [AllowAnonymous]
    public IActionResult SignedOut()
    {
        return View();
    }
}
