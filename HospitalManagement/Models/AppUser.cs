namespace HospitalManagement.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }  // For demo (hash in real app)
        public string Role { get; set; }      // Admin, Doctor, Receptionist, Patient
    }
}
