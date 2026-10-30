
using HospitalAppointmentEFCore.Services;

namespace HospitalAppointmentEFCore.Menus;

public class AuditLogMenu
{
    private readonly AuditLogService _service;

    public AuditLogMenu(AuditLogService service)
    {
        _service = service;
    }

    public async Task ShowAsync()
    {
        while (true)
        {
            Console.WriteLine("\n===== AUDIT LOG MENU =====");
            Console.WriteLine("1. View All Audit Logs");
            Console.WriteLine("2. View Audit Log By ID");
            Console.WriteLine("3. View History By Entity");
            Console.WriteLine("4. View Logs By Action");
            Console.WriteLine("0. Back");
            Console.Write("Enter choice: ");

            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                Console.WriteLine("Enter a valid number.");
                continue;
            }

            try
            {
                switch (choice)
                {
                    case 1:
                        await DisplayAsync(
                            await _service.GetAllAsync());
                        break;

                    case 2:
                        Console.Write("Enter AuditLog ID: ");
                        if (int.TryParse(Console.ReadLine(), out int id))
                        {
                            var log = await _service.GetByIdAsync(id);

                            if (log == null)
                                Console.WriteLine("Audit log not found.");
                            else
                                DisplayLog(log);
                        }
                        else
                        {
                            Console.WriteLine("Invalid ID.");
                        }
                        break;

                    case 3:
                        Console.Write("Enter Entity Name (Patient/Doctor/Appointment): ");
                        string? entityName = Console.ReadLine();

                        Console.Write("Enter Entity ID: ");
                        if (int.TryParse(Console.ReadLine(), out int entityId))
                        {
                            var logs = await _service.GetByEntityAsync(
                                entityName ?? "", entityId);

                            await DisplayAsync(logs);
                        }
                        else
                        {
                            Console.WriteLine("Invalid ID.");
                        }
                        break;

                    case 4:
                        Console.Write("Enter Action (Added/Modified/Deleted): ");
                        string? action = Console.ReadLine();

                        var actionLogs = await _service.GetByActionAsync(
                            action ?? "");

                        await DisplayAsync(actionLogs);
                        break;

                    case 0:
                        return;

                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    private static Task DisplayAsync(
        List<Models.AuditLog> logs)
    {
        if (logs.Count == 0)
        {
            Console.WriteLine("No audit records found.");
            return Task.CompletedTask;
        }

        foreach (var log in logs)
        {
            DisplayLog(log);
        }

        return Task.CompletedTask;
    }

    private static void DisplayLog(Models.AuditLog log)
    {
        Console.WriteLine("\n------------------------------");
        Console.WriteLine($"Audit ID    : {log.AuditLogId}");
        Console.WriteLine($"Entity      : {log.EntityName}");
        Console.WriteLine($"Entity ID   : {log.EntityId}");
        Console.WriteLine($"Action      : {log.Action}");
        Console.WriteLine($"Old Values  : {log.OldValues ?? "N/A"}");
        Console.WriteLine($"New Values  : {log.NewValues ?? "N/A"}");
        Console.WriteLine($"Action Date : {log.ActionDate:dd-MM-yyyy HH:mm:ss} UTC");
        Console.WriteLine($"Performed By: {log.PerformedBy}");
        Console.WriteLine("------------------------------");
    }
}
