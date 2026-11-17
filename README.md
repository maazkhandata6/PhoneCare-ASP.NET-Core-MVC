# PhoneCare — Extended ASP.NET Core MVC Semester Project
Stack: C#, ASP.NET Core MVC (.NET 8), EF Core 8, SQL Server 2022 Express, SSMS 22, Razor, Bootstrap 5.

## Run
1. Extract the ZIP and open PhoneCare.sln in Visual Studio 2022/2026.
2. Restore NuGet packages.
3. Ensure SQL Server Express instance `.\SQLEXPRESS` is running.
4. If your server instance differs, edit `PhoneCareConnection` in appsettings.json.
5. Press F5. EF Core EnsureCreated creates PhoneCareDB and starter data on first launch.


## Included
Landing page; customer registration/login; admin login/dashboard; customer dashboard/history; repair request intake and service base-price estimate; repair detail/ownership check; repair queue/status updates; service price management; customer directory/search; phone brand/model add; payment recording/register; operational report summary; responsive custom dashboard styling.

## Important database upgrade note
If you already ran the earlier starter package, `EnsureCreated()` will not add newly introduced tables to an existing database. For a fresh development setup, back up anything needed and drop the existing PhoneCareDB in SSMS, then run this version. For ongoing development, use EF Core migrations.

## Academic limitations
This is an extended academic project baseline, not a production commercial system. Build and test it locally before submission. Session-based authentication is simplified for classroom demonstration; production should use ASP.NET Core Identity, robust authorization, audit logs, rate limiting and secrets management. Payment records are manually entered; no real payment gateway is connected. Prices are illustrative PKR base estimates, not guaranteed final quotes. Phone catalog currently supports adding brands/models; edit/delete CRUD and more advanced diagnostic logic can be future enhancements.

 
