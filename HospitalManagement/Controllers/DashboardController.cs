using HospitalManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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

            var totalRevenue = _context.Consultations
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
                RecentAppointments = recentAppointments
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
                Status = "Pending"
            };

            _context.Appointments.Add(appointment);
            _context.SaveChanges();

            TempData["Success"] = "Appointment booked successfully!";
            return RedirectToAction("PatientDashboard");
        }
    }
}
