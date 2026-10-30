
using System.Text.Json;
using HospitalAppointmentEFCore.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentEFCore.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    // DbSets
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    // Table configurations
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.ToTable("Patients", table =>
                table.HasCheckConstraint(
                    "CK_Patients_Age",
                    "[Age] BETWEEN 0 AND 120"));

            entity.HasKey(p => p.PatientId);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
            entity.Property(p => p.Email).IsRequired().HasMaxLength(450);
            entity.Property(p => p.Age).IsRequired();
            entity.HasIndex(p => p.Email).IsUnique();
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.ToTable("Doctors");
            entity.HasKey(d => d.DoctorId);
            entity.Property(d => d.DoctorName)
                .IsRequired().HasMaxLength(100);
            entity.Property(d => d.Specialization)
                .IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(a => a.AppointmentId);
            entity.Property(a => a.AppointmentDate).IsRequired();
            entity.Property(a => a.Status)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("Scheduled");

            entity.HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // AuditLog configuration
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.ToTable("AuditLogs");

            entity.HasKey(a => a.AuditLogId);

            entity.Property(a => a.EntityName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(a => a.Action)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(a => a.PerformedBy)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(a => a.ActionDate)
                .IsRequired();
        });
    }

    // Automatically track changes
    public override int SaveChanges()
    {
        return SaveChangesAsync().GetAwaiter().GetResult();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        return SaveChangesAsync().GetAwaiter().GetResult();
    }

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        var auditEntries = ChangeTracker
            .Entries()
            .Where(entry =>
                entry.Entity is not AuditLog &&
                (entry.State == EntityState.Added ||
                 entry.State == EntityState.Modified ||
                 entry.State == EntityState.Deleted))
            .Select(entry =>
            {
                var primaryKey = entry.Metadata.FindPrimaryKey()!
                    .Properties.First();

                var oldValues = new Dictionary<string, object?>();
                var newValues = new Dictionary<string, object?>();

                if (entry.State == EntityState.Modified ||
                    entry.State == EntityState.Deleted)
                {
                    foreach (var property in entry.Properties)
                    {
                        oldValues[property.Metadata.Name] =
                            property.OriginalValue;
                    }
                }

                if (entry.State == EntityState.Added ||
                    entry.State == EntityState.Modified)
                {
                    foreach (var property in entry.Properties)
                    {
                        newValues[property.Metadata.Name] =
                            property.CurrentValue;
                    }
                }

                return new
                {
                    Entry = entry,
                    EntityName = entry.Metadata.ClrType.Name,
                    KeyName = primaryKey.Name,
                    OldValues = oldValues,
                    NewValues = newValues,
                    Action = entry.State.ToString()
                };
            })
            .ToList();

        // First save Patient / Doctor / Appointment changes
        var result = await base.SaveChangesAsync(cancellationToken);

        // Create audit records
        foreach (var item in auditEntries)
        {
            var entityId = item.Action == "Deleted"
                ? item.OldValues[item.KeyName]
                : item.Entry.Property(item.KeyName).CurrentValue;

            var auditLog = new AuditLog
            {
                EntityName = item.EntityName,
                EntityId = Convert.ToInt32(entityId),
                Action = item.Action,
                OldValues = item.OldValues.Count == 0
                    ? null
                    : JsonSerializer.Serialize(item.OldValues),
                NewValues = item.NewValues.Count == 0
                    ? null
                    : JsonSerializer.Serialize(item.NewValues),
                ActionDate = DateTime.UtcNow,
                PerformedBy = "System"
            };

            AuditLogs.Add(auditLog);
        }

        // Save AuditLog records
        if (auditEntries.Count > 0)
        {
            await base.SaveChangesAsync(cancellationToken);
        }

        return result;
    }
}
