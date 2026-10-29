using HospitalAppointmentEFCore.Helper;
using HospitalAppointmentEFCore.Models;
using HospitalAppointmentEFCore.Services;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentEFCore.Menus;

public class DoctorMenu
{
    private readonly DoctorService _service;
    public DoctorMenu(DoctorService service) => _service = service;

    public async Task ShowAsync()
    {
        while (true)
        {
            Console.WriteLine("\n=== DOCTOR MANAGEMENT ===");
            Console.WriteLine("1. Add doctor\n2. View all doctors\n3. Find doctor by ID\n4. Update doctor\n5. Delete doctor\n0. Back");
            Console.Write("Choice: ");
            if (!int.TryParse(Console.ReadLine(), out int choice)) { Console.WriteLine("Enter a number."); continue; }
            try
            {
                switch (choice)
                {
                    case 1:
                        var doctor = new Doctor
                        {
                            DoctorName = ConsoleHelper.ReadRequired("Doctor name: "),
                            Specialization = ConsoleHelper.ReadRequired("Specialization: ")
                        };
                        Console.WriteLine($"Doctor added. ID = {await _service.AddAsync(doctor)}");
                        break;
                    case 2:
                        var doctors = await _service.GetAllAsync();
                        foreach (var d in doctors) Print(d);
                        if (doctors.Count == 0) Console.WriteLine("No doctors found.");
                        break;
                    case 3:
                        var found = await _service.GetByIdAsync(ConsoleHelper.ReadPositiveInt("Doctor ID: "));
                        if (found is null) Console.WriteLine("Doctor not found."); else Print(found);
                        break;
                    case 4:
                        bool updated = await _service.UpdateAsync(
                            ConsoleHelper.ReadPositiveInt("Doctor ID to update: "),
                            ConsoleHelper.ReadRequired("New name: "),
                            ConsoleHelper.ReadRequired("New specialization: "));
                        Console.WriteLine(updated ? "Doctor updated." : "Doctor not found.");
                        break;
                    case 5:
                        bool deleted = await _service.DeleteAsync(ConsoleHelper.ReadPositiveInt("Doctor ID to delete: "));
                        Console.WriteLine(deleted ? "Doctor deleted." : "Doctor not found.");
                        break;
                    case 0: return;
                    default: Console.WriteLine("Choose 0 to 5."); break;
                }
            }
            catch (DbUpdateException)
            {
                Console.WriteLine("Database update failed. A doctor with appointments cannot be deleted.");
            }
            catch (ArgumentException ex) { Console.WriteLine(ex.Message); }
        }
    }

    private static void Print(Doctor d) =>
        Console.WriteLine($"ID: {d.DoctorId} | Name: {d.DoctorName} | Specialization: {d.Specialization}");
}
