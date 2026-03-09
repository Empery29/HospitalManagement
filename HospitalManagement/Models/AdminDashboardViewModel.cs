namespace HospitalManagement.Models
{
    public class AdminDashboardViewModel
    {
       
            public int TotalDoctors { get; set; }
            public int TotalAppointments { get; set; }
            public int PendingAppointments { get; set; }
            public int PharmacyPending { get; set; }
            public int TotalResolvedAppointments { get; set; }
            public decimal TotalRevenue { get; set; }

            public List<Appointment> RecentAppointments { get; set; }
        
    }
}
