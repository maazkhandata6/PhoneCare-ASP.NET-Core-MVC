using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PhoneCare.Models;

namespace PhoneCare.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(PhoneCareContext db)
    {
        if (!await db.PhoneBrands.AnyAsync())
        {
            db.PhoneBrands.AddRange(
                new PhoneBrand { Name = "Apple" },
                new PhoneBrand { Name = "Samsung" },
                new PhoneBrand { Name = "Google" },
                new PhoneBrand { Name = "Xiaomi" },
                new PhoneBrand { Name = "Oppo" },
                new PhoneBrand { Name = "Vivo" },
                new PhoneBrand { Name = "OnePlus" }
            );
        }

        if (!await db.RepairServices.AnyAsync())
        {
            db.RepairServices.AddRange(
                new RepairService { Name = "Screen Replacement", Description = "Cracked, broken or unresponsive display", BasePrice = 8500, EstimatedHours = 3, IsActive = true },
                new RepairService { Name = "Battery Replacement", Description = "Battery drains quickly or will not hold charge", BasePrice = 4500, EstimatedHours = 2, IsActive = true },
                new RepairService { Name = "Charging Port Repair", Description = "Loose, damaged or non-working charging port", BasePrice = 3000, EstimatedHours = 2, IsActive = true },
                new RepairService { Name = "Camera Repair", Description = "Blurry, damaged or non-working camera", BasePrice = 5000, EstimatedHours = 2, IsActive = true },
                new RepairService { Name = "Software Troubleshooting", Description = "Boot loops, software errors and performance issues", BasePrice = 1800, EstimatedHours = 1, IsActive = true },
                new RepairService { Name = "Water Damage Diagnosis", Description = "Inspection and diagnosis after liquid exposure", BasePrice = 2500, EstimatedHours = 4, IsActive = true }
            );
        }

        // Create an admin only when credentials are supplied
        // through environment variables.
        var adminEmail = Environment.GetEnvironmentVariable("PHONECARE_ADMIN_EMAIL");
        var adminPassword = Environment.GetEnvironmentVariable("PHONECARE_ADMIN_PASSWORD");

        if (!string.IsNullOrWhiteSpace(adminEmail) &&
            !string.IsNullOrWhiteSpace(adminPassword) &&
            !await db.Customers.AnyAsync(x => x.Email == adminEmail))
        {
            var hasher = new PasswordHasher<Customer>();

            var admin = new Customer
            {
                FullName = "PhoneCare Administrator",
                Email = adminEmail,
                PhoneNumber = "03000000000",
                Role = "Admin",
                CreatedAt = DateTime.UtcNow
            };

            admin.PasswordHash = hasher.HashPassword(admin, adminPassword);
            db.Customers.Add(admin);
        }
