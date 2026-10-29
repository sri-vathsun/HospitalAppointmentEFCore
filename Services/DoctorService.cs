using HospitalAppointmentEFCore.Models;
using HospitalAppointmentEFCore.Repositories;

namespace HospitalAppointmentEFCore.Services;

public class DoctorService
{
    private readonly DoctorRepository _repository;
    public DoctorService(DoctorRepository repository) => _repository = repository;

    public Task<int> AddAsync(Doctor doctor)
    {
        if (string.IsNullOrWhiteSpace(doctor.DoctorName))
            throw new ArgumentException("Doctor name is required.");
        if (string.IsNullOrWhiteSpace(doctor.Specialization))
            throw new ArgumentException("Specialization is required.");
        doctor.DoctorName = doctor.DoctorName.Trim();
        doctor.Specialization = doctor.Specialization.Trim();
        return _repository.AddAsync(doctor);
    }

    public Task<List<Doctor>> GetAllAsync() => _repository.GetAllAsync();
    public Task<Doctor?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);

    public Task<bool> UpdateAsync(int id, string name, string specialization)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(specialization))
            throw new ArgumentException("Doctor name and specialization are required.");
        return _repository.UpdateAsync(id, name.Trim(), specialization.Trim());
    }

    public Task<bool> DeleteAsync(int id) => _repository.DeleteAsync(id);
}
