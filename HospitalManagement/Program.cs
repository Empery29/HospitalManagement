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
            new AppUser { Username = "admin", Password = "123", CardId="SRP123", Role = "Admin",  Email = "yy@yahoo.com", Phone = "08012345678", Gender = "Male", Address = "Ikeja, Lagos" },
            new AppUser { Username = "doctor", Password = "123", CardId = "SRP234", Role = "Doctor", Gender = "Male", Phone = "1234567890", Email = "W@yahoo.com",  Address = "Idimu, Lagos" },
            new AppUser { Username = "patient", Password = "123", CardId = "SRP345", Role = "Patient", Email = "mike@yahoo.com", Phone = "08012345678", Gender = "Male", Address = "Ikola Rd, Lagos" }
        );

        context.SaveChanges();
    }

    if (!context.Doctors.Any())
    {
        context.AppUsers.AddRange(
            new AppUser { Username = "Dr.Olaoye", Password = "123", CardId = "SRP456", Role = "Doctor", Email = "Olaoye@yahoo.com", Phone = "08012345678", Gender = "Male", Address = "Ebute Metta, Lagos" },
            new AppUser { Username = "Dr.Nuru", Password = "123", CardId = "SRP567", Role = "Doctor",  Email = "Nuru@yahoo.com", Phone = "08012345678", Gender = "Male", Address = "Idimu Rd, Lagos" },
            new AppUser { Username = "Dr. Yussuff", Password = "123", CardId = "SRP678", Role = "Doctor", Email = "yusuf@yahoo.com", Phone = "08012345678", Gender = "Male", Address = "Ikola Rd, Lagos" }
        );
        context.Doctors.AddRange(
            new Doctor { FullName = "Dr.Olaoye", Specialization = "Surgeon", IsAvailable = true, Email = "mike@yahoo.com", PhoneNumber = "08012345678", Gender = "Male" },
            new Doctor { FullName = "Dr.Nuru", Specialization = "Orthopedic", IsAvailable = true, Email = "mike@yahoo.com", PhoneNumber = "08012345678", Gender = "Male" },
            new Doctor { FullName = "Dr.Yussuff", Specialization = "Dermatologist", IsAvailable = true, Email = "mike@yahoo.com", PhoneNumber = "08012345678", Gender = "Male" }
        );

        context.SaveChanges();
    }

    if (!context.Pharmacies.Any())
    {
        context.AppUsers.AddRange(
            new AppUser { Username = "pharmacy1", Password = "123", CardId = "789", Role = "Pharmacy", Email = "yyyyy@yahoo.com", Phone = "08012345678", Gender = "Male", Address = "Ikotaa, Lagos" },
            new AppUser { Username = "pharmacy2", Password = "123", CardId = "SRP996", Role = "Pharmacy", Email = "olo@yahoo.com", Phone = "08012349998", Gender = "Male", Address = " V.I, Lagos" },
            new AppUser { Username = "pharmacy3", Password = "123", CardId = "SRP890", Role = "Pharmacy", Email = "sdfg@yahoo.com", Phone = "08012300078", Gender = "Male", Address = " Agege, Lagos" }
        );
        context.Pharmacies.AddRange(
            new Pharmacy { FullName = "pharmacy1" },
            new Pharmacy { FullName = "pharmacy2" },
            new Pharmacy { FullName = "pharmacy3" }
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
