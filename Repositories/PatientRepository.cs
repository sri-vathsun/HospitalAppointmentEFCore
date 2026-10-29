using HospitalAppointmentEFCore.Data;
using HospitalAppointmentEFCore.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentEFCore.Repositories;

public class PatientRepository
{
    private readonly AppDbContext _db;
    public PatientRepository(AppDbContext db) => _db = db;

    public async Task<int> AddAsync(Patient patient)
    {
        _db.Patients.Add(patient);
        await _db.SaveChangesAsync();
        return patient.PatientId;
    }

    public Task<List<Patient>> GetAllAsync() =>
        _db.Patients.AsNoTracking().OrderBy(p => p.PatientId).ToListAsync();

    public Task<Patient?> GetByIdAsync(int id) =>
        _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.PatientId == id);

    public async Task<bool> UpdateAsync(int id, string name, string email, int age)
    {
        var patient = await _db.Patients.FindAsync(id);
        if (patient is null) return false;
        patient.Name = name;
        patient.Email = email;
        patient.Age = age;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var patient = await _db.Patients.FindAsync(id);
        if (patient is null) return false;
        _db.Patients.Remove(patient);
        await _db.SaveChangesAsync();
        return true;
    }
}
