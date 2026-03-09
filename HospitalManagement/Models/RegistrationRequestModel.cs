namespace HospitalManagement.Models
{
    public class RegistrationRequestModel
    {
        public string Username { get; set; }
        public string Password { get; set; }  // For demo (hash in real app)
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
    }

    public class CreateDoctorRequestModel : RegistrationRequestModel
    {
        public string Specialization { get; set; }
    }
}
