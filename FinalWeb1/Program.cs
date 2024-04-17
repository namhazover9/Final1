using FinalWeb1.DataAccess.Repository;
using FinalWeb1.DataAccess.Repository.IRepository;
using FinalWeb1.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using FinalWeb1.Utility;
using Microsoft.AspNetCore.Identity.UI.Services;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

// Facebook Authentication
builder.Services.AddAuthentication().AddFacebook(option =>
{
    option.AppId = "237135712725057";
    option.AppSecret = "9758c02180e149b61e8ab4fc0df8d734";
});
// Microsoft Authentication
builder.Services.AddAuthentication().AddMicrosoftAccount(option =>
{
    option.ClientId = "c7f5bbe4-6b68-4be2-b4af-f98e69fc5397";
    option.ClientSecret = "Efo8Q~WVI8LG~Srm6PZmL88Bnfq6KKDTBTP25duU";
});

builder.Services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache

// Add session support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(100); // You can set Time
    options.Cookie.HttpOnly = true; // This helps to prevent XSS - Cross-site scripting (XSS) is a type of security vulnerability typically found in web applications.
                                    // XSS attacks enable attackers to inject client-side scripts into web pages viewed by other users.
    options.Cookie.IsEssential = true; // make the session cookie essential
});

builder.Services.AddRazorPages();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession(); 
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();
