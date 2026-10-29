using HospitalAppointmentEFCore.Data;
using HospitalAppointmentEFCore.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentEFCore.Repositories;

public class AppointmentRepository
{
    private readonly AppDbContext _db;
    public AppointmentRepository(AppDbContext db) => _db = db;

    public async Task<int> AddAsync(Appointment appointment)
    {
        if (!await _db.Patients.AnyAsync(p => p.PatientId == appointment.PatientId))
            throw new InvalidOperationException("Patient ID does not exist.");
        if (!await _db.Doctors.AnyAsync(d => d.DoctorId == appointment.DoctorId))
            throw new InvalidOperationException("Doctor ID does not exist.");

        _db.Appointments.Add(appointment);
        await _db.SaveChangesAsync();
        return appointment.AppointmentId;
    }

    public Task<List<Appointment>> GetAllAsync() =>
        _db.Appointments.AsNoTracking()
            .Include(a => a.Patient).Include(a => a.Doctor)
            .OrderBy(a => a.AppointmentDate).ToListAsync();

    public Task<List<Appointment>> GetByDoctorAsync(int doctorId) =>
        _db.Appointments.AsNoTracking()
            .Include(a => a.Patient).Include(a => a.Doctor)
            .Where(a => a.DoctorId == doctorId)
            .OrderBy(a => a.AppointmentDate).ToListAsync();

    public async Task<bool> UpdateStatusAsync(int id, string status)
    {
        var appointment = await _db.Appointments.FindAsync(id);
        if (appointment is null) return false;
        appointment.Status = status;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var appointment = await _db.Appointments.FindAsync(id);
        if (appointment is null) return false;
        _db.Appointments.Remove(appointment);
        await _db.SaveChangesAsync();
        return true;
    }
}
