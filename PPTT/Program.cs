using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PPTT.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Configurar DbContext
builder.Services.AddDbContext<DBPPTTContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionSQL")));

// Register session services
builder.Services.AddDistributedMemoryCache(); 
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); 
});

// Register GlobalState as a singleton service
builder.Services.AddSingleton<GlobalState>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Configurar autenticación, autorización y sesión
app.UseAuthentication();
app.UseAuthorization();
app.UseSession(); // Ensure session middleware is used

app.MapRazorPages();

app.Run();
