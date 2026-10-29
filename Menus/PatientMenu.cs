using HospitalAppointmentEFCore.Helper;
using HospitalAppointmentEFCore.Models;
using HospitalAppointmentEFCore.Services;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentEFCore.Menus;

public class PatientMenu
{
    private readonly PatientService _service;
    public PatientMenu(PatientService service) => _service = service;

    public async Task ShowAsync()
    {
        while (true)
        {
            Console.WriteLine("\n=== PATIENT MANAGEMENT ===");
            Console.WriteLine("1. Add patient\n2. View all patients\n3. Find patient by ID\n4. Update patient\n5. Delete patient\n0. Back");
            Console.Write("Choice: ");
            if (!int.TryParse(Console.ReadLine(), out int choice)) { Console.WriteLine("Enter a number."); continue; }
            try
            {
                switch (choice)
                {
                    case 1:
                        var patient = new Patient
                        {
                            Name = ConsoleHelper.ReadRequired("Name: "),
                            Email = ConsoleHelper.ReadRequired("Email: "),
                            Age = ConsoleHelper.ReadAge()
                        };
                        Console.WriteLine($"Patient added. ID = {await _service.AddAsync(patient)}");
                        break;
                    case 2:
                        var patients = await _service.GetAllAsync();
                        foreach (var p in patients) Print(p);
                        if (patients.Count == 0) Console.WriteLine("No patients found.");
                        break;
                    case 3:
                        var found = await _service.GetByIdAsync(ConsoleHelper.ReadPositiveInt("Patient ID: "));
                        if (found is null) Console.WriteLine("Patient not found."); else Print(found);
                        break;
                    case 4:
                        int updateId = ConsoleHelper.ReadPositiveInt("Patient ID to update: ");
                        bool updated = await _service.UpdateAsync(updateId,
                            ConsoleHelper.ReadRequired("New name: "),
                            ConsoleHelper.ReadRequired("New email: "),
                            ConsoleHelper.ReadAge());
                        Console.WriteLine(updated ? "Patient updated." : "Patient not found.");
                        break;
                    case 5:
                        bool deleted = await _service.DeleteAsync(ConsoleHelper.ReadPositiveInt("Patient ID to delete: "));
                        Console.WriteLine(deleted ? "Patient deleted." : "Patient not found.");
                        break;
                    case 0: return;
                    default: Console.WriteLine("Choose 0 to 5."); break;
                }
            }
            catch (DbUpdateException)
            {
                Console.WriteLine("Database update failed. Check for duplicate email or existing appointments.");
            }
            catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    private static void Print(Patient p) =>
        Console.WriteLine($"ID: {p.PatientId} | Name: {p.Name} | Email: {p.Email} | Age: {p.Age}");
}
