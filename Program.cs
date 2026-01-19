using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebBanGiay.Models;

var builder = WebApplication.CreateBuilder(args);

// =====================
// SERVICES
// =====================
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<WebBanGiayContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Customer/Login";
        options.AccessDeniedPath = "/Customer/Login?denied=1";
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// =====================
// MIDDLEWARE
// =====================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

var contentTypeProvider = new FileExtensionContentTypeProvider();
contentTypeProvider.Mappings[".avif"] = "image/avif";
app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = contentTypeProvider
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// 🔥🔥🔥 QUAN TRỌNG NHẤT – THIẾU LÀ 404 HẾT 🔥🔥🔥
app.MapControllers();

// =====================
// ROUTES
// =====================

// 👉 ROOT "/" → CUSTOMER / TrangChu / Index
app.MapAreaControllerRoute(
    name: "customer_root",
    areaName: "Customer",
    pattern: "",
    defaults: new { controller = "TrangChu", action = "Index" }
);

// 👉 CUSTOMER AREA
app.MapAreaControllerRoute(
    name: "customer",
    areaName: "Customer",
    pattern: "Customer/{controller=TrangChu}/{action=Index}/{id?}"
);

// 👉 ADMIN AREA
app.MapAreaControllerRoute(
    name: "admin",
    areaName: "Admin",
    pattern: "Admin/{controller=Home}/{action=Index}/{id?}"
);

app.Run();
