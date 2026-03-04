namespace HospitalManagement.Models
{
    public class Consultation
    {
      
            public int Id { get; set; }

            public int AppointmentId { get; set; }
            public Appointment Appointment { get; set; }

            public string Diagnosis { get; set; }
            public string Prescription { get; set; }

            public decimal ConsultationFee { get; set; }
        
    }
}
