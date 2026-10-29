# HospitalAppointmentEFCore

Console-based EF Core project using SQL Server, repositories, services, dependency injection, and async/await.

## Prerequisites
- .NET 10 SDK
- SQL Server / SQL Server Express
- `dotnet-ef` tool (install with `dotnet tool install --global dotnet-ef` if needed)

## Before running
1. Open `appsettings.json`.
2. Change `Server=.\\SQLEXPRESS` if your SQL Server instance has a different name.
3. Run these commands from the folder containing the `.csproj` file:

```powershell
dotnet restore
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run
```

The app also calls `Database.MigrateAsync()` on startup, so after the first migration and database update, future migrations can be applied at startup.

## Git commits
Make one commit after each tested milestone, not one commit for the entire project:

```powershell
git init
git add HospitalAppointmentEFCore.csproj appsettings.json
git commit -m "chore: initialize hospital appointment project"

git add Models/Patient.cs Models/Doctor.cs Models/Appointment.cs
git commit -m "feat: add hospital entities"

git add Data/AppDbContext.cs
git commit -m "feat: configure entity relationships"

git add Repositories Services
git commit -m "feat: add repository and service layers"

git add Helper Menus Program.cs
git commit -m "feat: add console menus and operations"
```

For a learning exercise, it is even better to add one feature at a time and commit it after testing. The commits above are examples; they are not already created.
