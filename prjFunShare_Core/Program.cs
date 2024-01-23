using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using prjFunShare_Core.Models;
using System;
using System.Text.Encodings.Web;
using prjFunShare_Core.Hubs;
using System.Text.Unicode;
using UAParser;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//加入singalR
builder.Services.AddSignalR();

//建立Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(40);
    options.Cookie.HttpOnly = true;
});
// 注入Entity
builder.Services.AddDbContext<FUNShareContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("FUNShareConnection")));
//// Add services
//builder.Services.AddSingleton<IParser>(Parser.GetDefault());

//允許Cookie
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential 
    // cookies is needed for a given request.
    options.CheckConsentNeeded = context => true;

    options.MinimumSameSitePolicy = SameSiteMode.None;
    options.ConsentCookieValue = "true";
});

//新增Cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        options.SlidingExpiration = true;
        options.AccessDeniedPath = "/Forbidden/";
    });

builder.Services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));

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

//使用Session
app.UseSession();
//使用Cookie
//app.UseCookiePolicy();
//新增Cookie
app.UseAuthentication();
app.UseAuthorization();

//app.MapRazorPages();
//app.MapDefaultControllerRoute();


app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//加入Hub
app.MapHub<ChatHub>("/chatHub");

app.Run();