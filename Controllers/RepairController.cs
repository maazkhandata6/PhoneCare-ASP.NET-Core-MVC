using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PhoneCare.Data;
using PhoneCare.Models;

namespace PhoneCare.Controllers;

public class RepairController : Controller
{
    private readonly PhoneCareContext _db;
    public RepairController(PhoneCareContext db) => _db = db;

    private async Task LoadServices() =>
        ViewBag.RepairServiceId = new SelectList(await _db.RepairServices.Where(x => x.IsActive).OrderBy(x => x.Name).ToListAsync(), "Id", "Name");

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        if (HttpContext.Session.GetInt32("UserId") == null) return RedirectToAction("Login", "Account");
        await LoadServices();
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RepairRequest model)
    {
        var customerId = HttpContext.Session.GetInt32("UserId");
        if (customerId == null) return RedirectToAction("Login", "Account");
        ModelState.Remove(nameof(model.Customer));
        ModelState.Remove(nameof(model.RepairService));
        if (!ModelState.IsValid) { await LoadServices(); return View(model); }

        var service = await _db.RepairServices.FirstOrDefaultAsync(x => x.Id == model.RepairServiceId && x.IsActive);
        if (service == null) { ModelState.AddModelError(nameof(model.RepairServiceId), "Please select a valid service."); await LoadServices(); return View(model); }

        model.CustomerId = customerId.Value;
        model.EstimatedCost = service.BasePrice;
        model.Status = "Submitted";
        model.CreatedAt = DateTime.UtcNow;
        _db.RepairRequests.Add(model);
        await _db.SaveChangesAsync();
        TempData["Success"] = $"Repair request #{model.Id} submitted. Estimated cost: PKR {model.EstimatedCost:N0}.";
        return RedirectToAction("Index", "Customer");
    }

    public async Task<IActionResult> Details(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToAction("Login", "Account");
        var item = await _db.RepairRequests.Include(x => x.Customer).Include(x => x.RepairService).FirstOrDefaultAsync(x => x.Id == id);
        if (item == null) return NotFound();
        if (HttpContext.Session.GetString("UserRole") != "Admin" && item.CustomerId != userId) return Forbid();
        return View(item);
    }
}
