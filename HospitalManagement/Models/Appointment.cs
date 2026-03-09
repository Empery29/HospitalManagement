namespace HospitalManagement.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        public string? Description { get; set; }
        public string? PatientUsername { get; set; }
        public DateTime AppointmentDate { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public decimal ConsultationFee { get; set; }
        public string Status { get; set; } // Pending, Approved, Completed
    }
}
