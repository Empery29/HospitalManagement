namespace HospitalManagement.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Specialization { get; set; }
        public bool IsAvailable { get; set; }
    }
}
