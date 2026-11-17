using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneCare.Data;

namespace PhoneCare.Controllers;

public class CustomerController : Controller
{
    private readonly PhoneCareContext _db;
    public CustomerController(PhoneCareContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        if (HttpContext.Session.GetInt32("UserId") is not int id) return RedirectToAction("Login", "Account");
        if (HttpContext.Session.GetString("UserRole") == "Admin") return RedirectToAction("Index", "Admin");

        var requests = await _db.RepairRequests.Include(x => x.RepairService)
            .Where(x => x.CustomerId == id).OrderByDescending(x => x.CreatedAt).ToListAsync();
        ViewBag.Total = requests.Count;
        ViewBag.Active = requests.Count(x => x.Status != "Completed" && x.Status != "Rejected");
        ViewBag.Completed = requests.Count(x => x.Status == "Completed");
        return View(requests);
    }
}
