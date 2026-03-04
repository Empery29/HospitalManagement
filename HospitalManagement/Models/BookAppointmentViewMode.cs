namespace HospitalManagement.Models
{
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class BookAppointmentViewModel
    {
        public int DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        

        public List<SelectListItem> Doctors { get; set; }
        public List<Appointment> MyAppointments { get; set; }
    }

    public class CreateAppointmentRequestModel
    {
        public int DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }


     
    }
}
