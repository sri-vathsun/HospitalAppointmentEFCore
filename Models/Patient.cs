namespace HospitalAppointmentEFCore.Models;

public class Patient
{
    public int PatientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Age { get; set; }
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
