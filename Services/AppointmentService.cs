using HospitalAppointmentEFCore.Models;
using HospitalAppointmentEFCore.Repositories;

namespace HospitalAppointmentEFCore.Services;

public class AppointmentService
{
    private readonly AppointmentRepository _repository;
    public AppointmentService(AppointmentRepository repository) => _repository = repository;

    public Task<int> AddAsync(Appointment appointment)
    {
        if (appointment.AppointmentDate <= DateTime.Now)
            throw new ArgumentException("Appointment date and time must be in the future.");
        appointment.Status = "Scheduled";
        return _repository.AddAsync(appointment);
    }

    public Task<List<Appointment>> GetAllAsync() => _repository.GetAllAsync();
    public Task<List<Appointment>> GetByDoctorAsync(int doctorId) =>
        _repository.GetByDoctorAsync(doctorId);

    public Task<bool> UpdateStatusAsync(int id, string status)
    {
        string[] validStatuses = ["Scheduled", "Completed", "Cancelled"];
        if (!validStatuses.Contains(status, StringComparer.OrdinalIgnoreCase))
            throw new ArgumentException("Status must be Scheduled, Completed, or Cancelled.");
        var normalized = validStatuses.First(s => s.Equals(status, StringComparison.OrdinalIgnoreCase));
        return _repository.UpdateStatusAsync(id, normalized);
    }

    public Task<bool> DeleteAsync(int id) => _repository.DeleteAsync(id);
}
