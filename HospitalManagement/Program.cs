using HospitalManagement.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

builder.Services.AddAuthorization();




builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if (!context.AppUsers.Any())
    {
        context.AppUsers.AddRange(
            new AppUser { Username = "admin", Password = "123", Role = "Admin" },
            new AppUser { Username = "doctor", Password = "123", Role = "Doctor" },
            new AppUser { Username = "patient", Password = "123", Role = "Patient" }
        );

        context.SaveChanges();
    }

    if (!context.Doctors.Any())
    {
        context.AppUsers.AddRange(
            new AppUser { Username = "doctor1", Password = "123", Role = "Doctor" },
            new AppUser { Username = "doctor2", Password = "123", Role = "Doctor" },
            new AppUser { Username = "doctor3", Password = "123", Role = "Doctor" }
        );
        context.Doctors.AddRange(
            new Doctor { FullName = "doctor1", Specialization = "Doctor", IsAvailable = true },
            new Doctor { FullName = "doctor2", Specialization = "Doctor", IsAvailable = true },
            new Doctor { FullName = "doctor3", Specialization = "Doctor", IsAvailable = true }
        );

        context.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
