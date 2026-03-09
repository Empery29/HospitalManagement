namespace HospitalManagement.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        public string CardId { get; set; }  // Unique identifier for patients
        public string Username { get; set; }
        public string Password { get; set; }  // For demo (hash in real app)
        public string Role { get; set; }      // Admin, Doctor, Receptionist, Patient
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
    }
}
