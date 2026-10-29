using HospitalAppointmentEFCore.Models;
using HospitalAppointmentEFCore.Repositories;

namespace HospitalAppointmentEFCore.Services;

public class PatientService
{
    private readonly PatientRepository _repository;
    public PatientService(PatientRepository repository) => _repository = repository;

    public Task<int> AddAsync(Patient patient)
    {
        Validate(patient.Name, patient.Email, patient.Age);
        return _repository.AddAsync(patient);
    }

    public Task<List<Patient>> GetAllAsync() => _repository.GetAllAsync();
    public Task<Patient?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);

    public Task<bool> UpdateAsync(int id, string name, string email, int age)
    {
        Validate(name, email, age);
        return _repository.UpdateAsync(id, name.Trim(), email.Trim(), age);
    }

    public Task<bool> DeleteAsync(int id) => _repository.DeleteAsync(id);

    private static void Validate(string name, string email, int age)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.");
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new ArgumentException("Enter a valid email address.");
        if (age < 0 || age > 120) throw new ArgumentException("Age must be between 0 and 120.");
    }
}
