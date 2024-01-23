using Microsoft.EntityFrameworkCore;
using prjFunShare_backend.Hubs;
using prjFunShare_backend.Models;

var builder = WebApplication.CreateBuilder(args);
//�Y�ɳq�T��
builder.Services.AddSignalR();
//�إ�Session
builder.Services.AddSession();
// �`�JEntity
builder.Services.AddDbContext<FUNShareContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("FUNShareConnection")));
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<FUNShareContext>(Options => Options.UseSqlServer(builder.Configuration.GetConnectionString("FUNShareConnection")));

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

//�ϥ�Session
app.UseSession();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=index}/{id?}");
app.MapHub<ChatHub>("/hubs/chat");//�o�y�O���w���|�A�ܭ��n�A�n��e�ݪ����|�@��
//���e�n�[
app.Run();

