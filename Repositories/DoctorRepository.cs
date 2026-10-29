using HospitalAppointmentEFCore.Data;
using HospitalAppointmentEFCore.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentEFCore.Repositories;

public class DoctorRepository
{
    private readonly AppDbContext _db;
    public DoctorRepository(AppDbContext db) => _db = db;

    public async Task<int> AddAsync(Doctor doctor)
    {
        _db.Doctors.Add(doctor);
        await _db.SaveChangesAsync();
        return doctor.DoctorId;
    }

    public Task<List<Doctor>> GetAllAsync() =>
        _db.Doctors.AsNoTracking().OrderBy(d => d.DoctorId).ToListAsync();

    public Task<Doctor?> GetByIdAsync(int id) =>
        _db.Doctors.AsNoTracking().FirstOrDefaultAsync(d => d.DoctorId == id);

    public async Task<bool> UpdateAsync(int id, string name, string specialization)
    {
        var doctor = await _db.Doctors.FindAsync(id);
        if (doctor is null) return false;
        doctor.DoctorName = name;
        doctor.Specialization = specialization;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var doctor = await _db.Doctors.FindAsync(id);
        if (doctor is null) return false;
        _db.Doctors.Remove(doctor);
        await _db.SaveChangesAsync();
        return true;
    }
}
