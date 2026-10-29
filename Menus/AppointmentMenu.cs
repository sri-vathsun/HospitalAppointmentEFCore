using HospitalAppointmentEFCore.Helper;
using HospitalAppointmentEFCore.Models;
using HospitalAppointmentEFCore.Services;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentEFCore.Menus;

public class AppointmentMenu
{
    private readonly AppointmentService _service;
    private readonly PatientService _patients;
    private readonly DoctorService _doctors;

    public AppointmentMenu(AppointmentService service, PatientService patients, DoctorService doctors)
    {
        _service = service;
        _patients = patients;
        _doctors = doctors;
    }

    public async Task ShowAsync()
    {
        while (true)
        {
            Console.WriteLine("\n=== APPOINTMENT MANAGEMENT ===");
            Console.WriteLine("1. Book appointment\n2. View all appointments\n3. View appointments by doctor\n4. Update appointment status\n5. Delete appointment\n0. Back");
            Console.Write("Choice: ");
            if (!int.TryParse(Console.ReadLine(), out int choice)) { Console.WriteLine("Enter a number."); continue; }
            try
            {
                switch (choice)
                {
                    case 1:
                        await ShowPatientsAndDoctorsAsync();
                        var appointment = new Appointment
                        {
                            PatientId = ConsoleHelper.ReadPositiveInt("Patient ID: "),
                            DoctorId = ConsoleHelper.ReadPositiveInt("Doctor ID: "),
                            AppointmentDate = ConsoleHelper.ReadFutureDate()
                        };
                        Console.WriteLine($"Appointment booked. ID = {await _service.AddAsync(appointment)}");
                        break;
                    case 2:
                        var all = await _service.GetAllAsync();
                        foreach (var a in all) Print(a);
                        if (all.Count == 0) Console.WriteLine("No appointments found.");
                        break;
                    case 3:
                        int doctorId = ConsoleHelper.ReadPositiveInt("Doctor ID: ");
                        var byDoctor = await _service.GetByDoctorAsync(doctorId);
                        foreach (var a in byDoctor) Print(a);
                        if (byDoctor.Count == 0) Console.WriteLine("No appointments found for that doctor.");
                        break;
                    case 4:
                        int statusId = ConsoleHelper.ReadPositiveInt("Appointment ID: ");
                        string status = ConsoleHelper.ReadRequired("Status (Scheduled/Completed/Cancelled): ");
                        bool changed = await _service.UpdateStatusAsync(statusId, status);
                        Console.WriteLine(changed ? "Appointment status updated." : "Appointment not found.");
                        break;
                    case 5:
                        bool deleted = await _service.DeleteAsync(ConsoleHelper.ReadPositiveInt("Appointment ID to delete: "));
                        Console.WriteLine(deleted ? "Appointment deleted." : "Appointment not found.");
                        break;
                    case 0: return;
                    default: Console.WriteLine("Choose 0 to 5."); break;
                }
            }
            catch (DbUpdateException)
            {
                Console.WriteLine("Database update failed. Check the values and existing relationships.");
            }
            catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    private async Task ShowPatientsAndDoctorsAsync()
    {
        Console.WriteLine("\nPatients:");
        foreach (var p in await _patients.GetAllAsync())
            Console.WriteLine($"{p.PatientId} | {p.Name}");
        Console.WriteLine("\nDoctors:");
        foreach (var d in await _doctors.GetAllAsync())
            Console.WriteLine($"{d.DoctorId} | {d.DoctorName} | {d.Specialization}");
    }

    private static void Print(Appointment a) =>
        Console.WriteLine($"Appointment: {a.AppointmentId} | Patient: {a.Patient?.Name ?? a.PatientId.ToString()} | Doctor: {a.Doctor?.DoctorName ?? a.DoctorId.ToString()} | Date: {a.AppointmentDate:yyyy-MM-dd HH:mm} | Status: {a.Status}");
}
