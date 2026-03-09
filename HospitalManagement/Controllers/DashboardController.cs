using HospitalManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HospitalManagement.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Doctor")]
        public IActionResult DoctorDashboard()
        {
            var pendingAppointments = _context.Appointments
                .Where(a => a.Status == "Pending")
                .ToList();

            return View(pendingAppointments);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminDashboard()
        {
            var totalDoctors = _context.Doctors.Count();
            var totalAppointments = _context.Appointments.Count();
            var pendingAppointments = _context.Appointments.Count(a => a.Status == "Pending");
            var pharmacyPending = _context.Appointments.Count(a => a.Status == "Completed");
            var todayResolvedAppointment = _context.Appointments.Count(a => a.Status == "Approved" && a.ResolvedDate.Value.Date == DateTime.Now.Date);

            var totalRevenue = _context.Appointments
                .Sum(c => (decimal?)c.ConsultationFee) ?? 0;

            var recentAppointments = _context.Appointments
                .OrderByDescending(a => a.AppointmentDate)
                .Take(5)
                .ToList();

            var model = new AdminDashboardViewModel
            {
                TotalDoctors = totalDoctors,
                TotalAppointments = totalAppointments,
                PendingAppointments = pendingAppointments,
                TotalRevenue = totalRevenue,
                RecentAppointments = recentAppointments,
                PharmacyPending = pharmacyPending,
                TotalResolvedAppointments = todayResolvedAppointment
            };

            return View(model);
        }
        [Authorize(Roles = "Patient")]
        public IActionResult PatientDashboard()
        {
            var model = new BookAppointmentViewModel
            {
                Doctors = _context.Doctors
                    .Where(d => d.IsAvailable)
                    .Select(d => new SelectListItem
                    {
                        Value = d.Id.ToString(),
                        Text = d.FullName + " - " + d.Specialization
                    })
                    .ToList(),

                AppointmentDate = DateTime.Now
                //MyAppointments = _context.Appointments
                //.Where(a => a.PatientId == currentUserId)
                //.ToList()
            };

            return View(model);
        }

        [Authorize(Roles = "Patient")]
        [HttpPost]
        public IActionResult BookAppointment(CreateAppointmentRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("PatientDashboard");
            }
            var exists = _context.Appointments.Any(a =>
    a.DoctorId == model.DoctorId &&
    a.AppointmentDate == model.AppointmentDate);

            if (exists)
            {
                TempData["Error"] = "Doctor already booked at this time.";
                return RedirectToAction("PatientDashboard");
            }

            var appointment = new Appointment
            {
                DoctorId = model.DoctorId,
                AppointmentDate = model.AppointmentDate,
                Status = "Pending",
                PatientUsername = User.FindFirst(ClaimTypes.Name)?.Value + $"({User.FindFirst(ClaimTypes.NameIdentifier)?.Value})"
            };

            _context.Appointments.Add(appointment);
            _context.SaveChanges();

            TempData["Success"] = "Appointment booked successfully!";
            return RedirectToAction("PatientDashboard");
        }

        public IActionResult PatientDescription(int id)
        {
            var appointment = _context.Appointments
                .Include(t => t.Doctor)
                .FirstOrDefault(p => p.Id == id);
            return View(appointment);
        }

        [HttpPost]
        public IActionResult CreateDescription(int id, string description)
        {
            var appointment = _context.Appointments.FirstOrDefault(p => p.Id == id);
            if (appointment != null)
            {
                appointment.Description = description;
                appointment.Status = "Completed";
                appointment.ConsultationFee = 5000;
                _context.SaveChanges();
                return RedirectToAction("DoctorDashboard");
            }
            return RedirectToAction("PatientDescription", new { id });
        }

        [Authorize(Roles = "Pharmacy")]
        public IActionResult PharmacyDashboard()
        {
            var pendingAppointments = _context.Appointments
              .Where(a => a.Status == "Completed" && a.Description != null)
              .ToList();

            return View(pendingAppointments);
        }
        [Authorize(Roles = "Pharmacy")]

        public IActionResult DescriptionPage(int id)
        {
            var patientDescription = _context.Appointments
                .Include(a => a.Doctor)
             .FirstOrDefault(a => a.Status == "Completed" && a.Description != null && a.Id == id);

            return View(patientDescription);
        }

        public IActionResult Approved(int id)
        {
            var appointment = _context.Appointments.FirstOrDefault(p => p.Id == id);
            if (appointment != null)
            {
                appointment.Status = "Approved";
                appointment.ResolvedDate = DateTime.UtcNow;
                _context.SaveChanges();
                return RedirectToAction("PharmacyDashboard");
            }
            return RedirectToAction("DescriptionPage", new { id });
        }
        public IActionResult PatientRegistration()
        {
            return View();
        }
        [HttpPost]
        public IActionResult PatientRegistration(RegistrationRequestModel model)
        { 
            if(model != null)
            {
                var user = new AppUser
                {
                    Username = model.Username,
                    Password = model.Password,
                    Phone = model.Phone,
                    Address = model.Address,
                    Email = model.Email,
                    Gender = model.Gender,
                    Role = "Patient",
                    CardId = "SRP" + new Random().Next(100, 9999).ToString(),
                };
                _context.AppUsers.Add(user);
                _context.SaveChanges();
                TempData["Success"] = "Registration successful! Please log in.";
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                if (role != null)
                {
                    if(role == "Admin")
                    return RedirectToAction("AdminDashboard", "Dashboard");
                }
                return RedirectToAction("Login", "Account");
            }
            return View();

        }
        [Authorize(Roles = "Admin")]
        public IActionResult DoctorRegistration()
        {
            return View();
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult DoctorRegistration(CreateDoctorRequestModel model)
        {
            if (model != null)
            {
                var user = new AppUser
                {
                    Username = model.Username,
                    Password = model.Password,
                    Phone = model.Phone,
                    Address = model.Address,
                    Email = model.Email,
                    Gender = model.Gender,
                    Role = "Doctor",
                    CardId = "SRP" + new Random().Next(100, 9999).ToString(),
                };
                _context.AppUsers.Add(user);

                var doctor = new Doctor
                {
                    FullName = model.Username,
                    Specialization = model.Specialization,
                    IsAvailable = true,
                    Email = model.Email,
                    PhoneNumber = model.Phone,
                    Gender = model.Gender
                };

                _context.Doctors.Add(doctor);
                _context.SaveChanges();
                TempData["Success"] = "Registration successful! Please log in.";
                return RedirectToAction("AdminDashboard", "Dashboard");
            }
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult PharmacistRegistration()
        {
            return View();
        }

        
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult PharmacistRegistration(RegistrationRequestModel model)
        {
            if (model != null)
            {
                var user = new AppUser
                {
                    Username = model.Username,
                    Password = model.Password,
                    Phone = model.Phone,
                    Address = model.Address,
                    Email = model.Email,
                    Gender = model.Gender,
                    Role = "Pharmacy",
                    CardId = "SRP" + new Random().Next(100, 9999).ToString(),
                };
                _context.AppUsers.Add(user);

                var pharmacy = new Pharmacy
                {
                    FullName = model.Username
                };

                _context.Pharmacies.Add(pharmacy);
                _context.SaveChanges();
                TempData["Success"] = "Registration successful! Please log in.";
                return RedirectToAction("AdminDashboard", "Dashboard");
            }
            return View();

        }

    }
}
