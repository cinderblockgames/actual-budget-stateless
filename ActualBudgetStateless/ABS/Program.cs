using ABS.Configuration;
using ABS.Jobs;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;

var builder = WebApplication.CreateBuilder(args);

// Load mounted assets.
StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    foreach (var type in Dependencies.GetAllFilters())
    {
        options.Filters.Add(type);
    }
});
Dependencies.Load(builder.Services);

// Add session.
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".ActualStateless.Session";
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
#if !DEBUG
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
#endif
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Bank sync every hour (if enabled).
var bankSync = app.Services.BuildJob<BankSyncJob>(TimeSpan.FromHours(1));

app.Run();

Console.WriteLine("Shutting down; please wait.");
bankSync.Stop().Wait();
Console.WriteLine("Shutdown complete.");