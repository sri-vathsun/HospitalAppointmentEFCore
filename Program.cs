```csharp
using HospitalAppointmentEFCore.Data;
using HospitalAppointmentEFCore.Menus;
using HospitalAppointmentEFCore.Repositories;
using HospitalAppointmentEFCore.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("DefaultConnection is missing from appsettings.json.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Repository registrations
builder.Services.AddScoped<PatientRepository>();
builder.Services.AddScoped<DoctorRepository>();
builder.Services.AddScoped<AppointmentRepository>();
builder.Services.AddScoped<AuditLogRepository>(); // NEW

// Service registrations
builder.Services.AddScoped<PatientService>();
builder.Services.AddScoped<DoctorService>();
builder.Services.AddScoped<AppointmentService>();
builder.Services.AddScoped<AuditLogService>(); // NEW

// Menu registrations
builder.Services.AddScoped<PatientMenu>();
builder.Services.AddScoped<DoctorMenu>();
builder.Services.AddScoped<AppointmentMenu>();
builder.Services.AddScoped<AuditLogMenu>(); // NEW

using var host = builder.Build();
using var scope = host.Services.CreateScope();

var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
await db.Database.MigrateAsync();

var patientMenu = scope.ServiceProvider.GetRequiredService<PatientMenu>();
var doctorMenu = scope.ServiceProvider.GetRequiredService<DoctorMenu>();
var appointmentMenu = scope.ServiceProvider.GetRequiredService<AppointmentMenu>();
var auditLogMenu = scope.ServiceProvider.GetRequiredService<AuditLogMenu>(); // NEW

while (true)
{
    Console.WriteLine("\n===== HOSPITAL APPOINTMENT MANAGEMENT =====");
    Console.WriteLine("1. Patient management");
    Console.WriteLine("2. Doctor management");
    Console.WriteLine("3. Appointment management");
    Console.WriteLine("4. Audit log management"); // NEW
    Console.WriteLine("0. Exit");
    Console.Write("Enter choice: ");

    if (!int.TryParse(Console.ReadLine(), out int choice))
    {
        Console.WriteLine("Enter a valid number.");
        continue;
    }

    switch (choice)
    {
        case 1: await patientMenu.ShowAsync(); break;
        case 2: await doctorMenu.ShowAsync(); break;
        case 3: await appointmentMenu.ShowAsync(); break;
        case 4: await auditLogMenu.ShowAsync(); break; // NEW
        case 0: return;
        default: Console.WriteLine("Choose 0 to 4."); break;
    }
}
```