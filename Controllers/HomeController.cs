using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneCare.Data;

namespace PhoneCare.Controllers;

public class HomeController : Controller
{
    private readonly PhoneCareContext _db;
    public HomeController(PhoneCareContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        ViewBag.ServiceCount = await _db.RepairServices.CountAsync(x => x.IsActive);
        ViewBag.BrandCount = await _db.PhoneBrands.CountAsync();
        return View();
    }

    public IActionResult Error() => View();
}
