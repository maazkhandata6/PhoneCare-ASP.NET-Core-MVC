using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneCare.Data;
using PhoneCare.Models;

namespace PhoneCare.Controllers;

public class AdminController : Controller
{
    private readonly PhoneCareContext _db;
    public AdminController(PhoneCareContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        if (HttpContext.Session.GetString("UserRole") != "Admin") return RedirectToAction("Login", "Account");
        var requests = await _db.RepairRequests.Include(x => x.Customer).Include(x => x.RepairService)
            .OrderByDescending(x => x.CreatedAt).Take(10).ToListAsync();
        ViewBag.CustomerCount = await _db.Customers.CountAsync(x => x.Role == "Customer");
        ViewBag.RequestCount = await _db.RepairRequests.CountAsync();
        ViewBag.PendingCount = await _db.RepairRequests.CountAsync(x => x.Status == "Submitted" || x.Status == "Diagnosing");
        ViewBag.CompletedCount = await _db.RepairRequests.CountAsync(x => x.Status == "Completed");
        ViewBag.Revenue = await _db.RepairRequests.Where(x => x.Status == "Completed").SumAsync(x => (decimal?)x.EstimatedCost) ?? 0;
        return View(requests);
    }

    public async Task<IActionResult> Requests(string? status)
    {
        if (HttpContext.Session.GetString("UserRole") != "Admin") return RedirectToAction("Login", "Account");
        var query = _db.RepairRequests.Include(x => x.Customer).Include(x => x.RepairService).AsQueryable();
        if (!string.IsNullOrWhiteSpace(status)) query = query.Where(x => x.Status == status);
        ViewBag.Status = status;
        return View(await query.OrderByDescending(x => x.CreatedAt).ToListAsync());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, string status)
    {
        if (HttpContext.Session.GetString("UserRole") != "Admin") return Forbid();
        var allowed = new[] { "Submitted", "Diagnosing", "Awaiting Approval", "In Repair", "Completed", "Rejected" };
        if (!allowed.Contains(status)) return BadRequest();
        var request = await _db.RepairRequests.FindAsync(id);
        if (request == null) return NotFound();
        request.Status = status;
        request.CompletedAt = status == "Completed" ? DateTime.UtcNow : null;
        await _db.SaveChangesAsync();
        TempData["Success"] = $"Request #{id} status updated.";
        return RedirectToAction("Requests");
    }

    public async Task<IActionResult> Services()
    {
        if (HttpContext.Session.GetString("UserRole") != "Admin") return RedirectToAction("Login", "Account");
        return View(await _db.RepairServices.OrderBy(x => x.Name).ToListAsync());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdatePrice(int id, decimal basePrice)
    {
        if (HttpContext.Session.GetString("UserRole") != "Admin") return Forbid();
        if (basePrice < 0 || basePrice > 1000000) return BadRequest();
        var service = await _db.RepairServices.FindAsync(id);
        if (service == null) return NotFound();
        service.BasePrice = basePrice;
        await _db.SaveChangesAsync();
        TempData["Success"] = "Service price updated.";
        return RedirectToAction("Services");
    }

    public async Task<IActionResult> Customers(string? q) {
        if (HttpContext.Session.GetString("UserRole") != "Admin") return RedirectToAction("Login","Account");
        var query = _db.Customers.Where(x => x.Role == "Customer").AsQueryable();
        if (!string.IsNullOrWhiteSpace(q)) query = query.Where(x => x.FullName.Contains(q) || x.Email.Contains(q) || x.PhoneNumber.Contains(q));
        ViewBag.Query=q; return View(await query.OrderBy(x=>x.FullName).ToListAsync());
    }
    public async Task<IActionResult> Brands() {
        if (HttpContext.Session.GetString("UserRole") != "Admin") return RedirectToAction("Login","Account");
        return View(await _db.PhoneBrands.Include(x=>x.Models).OrderBy(x=>x.Name).ToListAsync());
    }
    [HttpPost,ValidateAntiForgeryToken]
    public async Task<IActionResult> AddBrand(string name) {
        if (HttpContext.Session.GetString("UserRole") != "Admin") return Forbid();
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length>60) return BadRequest();
        if (!await _db.PhoneBrands.AnyAsync(x=>x.Name==name.Trim())) _db.PhoneBrands.Add(new PhoneBrand{Name=name.Trim()});
        await _db.SaveChangesAsync(); TempData["Success"]="Phone brand saved."; return RedirectToAction("Brands");
    }
    [HttpPost,ValidateAntiForgeryToken]
    public async Task<IActionResult> AddModel(int phoneBrandId,string name) {
        if (HttpContext.Session.GetString("UserRole") != "Admin") return Forbid();
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length>80) return BadRequest();
        if (!await _db.PhoneBrands.AnyAsync(x=>x.Id==phoneBrandId)) return NotFound();
        _db.PhoneModels.Add(new PhoneModel{PhoneBrandId=phoneBrandId,Name=name.Trim()});
        await _db.SaveChangesAsync(); TempData["Success"]="Phone model added."; return RedirectToAction("Brands");
    }
    public async Task<IActionResult> Payments() {
        if (HttpContext.Session.GetString("UserRole") != "Admin") return RedirectToAction("Login","Account");
        return View(await _db.Payments.Include(x=>x.RepairRequest).ThenInclude(x=>x!.Customer).OrderByDescending(x=>x.PaidAt).ToListAsync());
    }
    [HttpPost,ValidateAntiForgeryToken]
    public async Task<IActionResult> RecordPayment(int repairRequestId,decimal amount,string method) {
        if (HttpContext.Session.GetString("UserRole") != "Admin") return Forbid();
        if (amount<=0 || amount>10000000 || !new[]{"Cash","Bank Transfer","Card","Other"}.Contains(method)) return BadRequest();
        var repair=await _db.RepairRequests.FindAsync(repairRequestId); if(repair==null)return NotFound();
        _db.Payments.Add(new Payment{RepairRequestId=repairRequestId,Amount=amount,Method=method,Status="Paid",Reference="PC-"+repairRequestId+"-"+DateTime.UtcNow.ToString("yyyyMMddHHmm"),PaidAt=DateTime.UtcNow});
        await _db.SaveChangesAsync(); TempData["Success"]="Payment recorded."; return RedirectToAction("Payments");
    }
    public async Task<IActionResult> Reports() {
        if (HttpContext.Session.GetString("UserRole") != "Admin") return RedirectToAction("Login","Account");
        ViewBag.Total=await _db.RepairRequests.CountAsync();
        ViewBag.Completed=await _db.RepairRequests.CountAsync(x=>x.Status=="Completed");
        ViewBag.Open=await _db.RepairRequests.CountAsync(x=>x.Status!="Completed" && x.Status!="Rejected");
        ViewBag.Paid=await _db.Payments.Where(x=>x.Status=="Paid").SumAsync(x=>(decimal?)x.Amount) ?? 0;
        return View();
    }
}
