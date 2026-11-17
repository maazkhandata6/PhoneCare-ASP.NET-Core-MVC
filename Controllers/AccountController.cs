using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneCare.Data;
using PhoneCare.Models;
using PhoneCare.ViewModels;

namespace PhoneCare.Controllers;

public class AccountController : Controller
{
    private readonly PhoneCareContext _db;
    private readonly PasswordHasher<Customer> _hasher = new();

    public AccountController(PhoneCareContext db) => _db = db;

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _db.Customers.FirstOrDefaultAsync(x => x.Email == model.Email);
        if (user == null || _hasher.VerifyHashedPassword(user, user.PasswordHash, model.Password) == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError("", "Email or password is incorrect.");
            return View(model);
        }

        HttpContext.Session.SetInt32("UserId", user.Id);
        HttpContext.Session.SetString("UserName", user.FullName);
        HttpContext.Session.SetString("UserRole", user.Role);
        return user.Role == "Admin" ? RedirectToAction("Index", "Admin") : RedirectToAction("Index", "Customer");
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        if (await _db.Customers.AnyAsync(x => x.Email == model.Email))
        {
            ModelState.AddModelError(nameof(model.Email), "This email is already registered.");
            return View(model);
        }

        var customer = new Customer
        {
            FullName = model.FullName,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            Role = "Customer",
            CreatedAt = DateTime.UtcNow
        };
        customer.PasswordHash = _hasher.HashPassword(customer, model.Password);
        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();

        HttpContext.Session.SetInt32("UserId", customer.Id);
        HttpContext.Session.SetString("UserName", customer.FullName);
        HttpContext.Session.SetString("UserRole", customer.Role);
        return RedirectToAction("Index", "Customer");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}
