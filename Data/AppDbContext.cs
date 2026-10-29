using HospitalAppointmentEFCore.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentEFCore.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Appointment> Appointments => Set<Appointment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.ToTable("Patients", table =>
                table.HasCheckConstraint("CK_Patients_Age", "[Age] BETWEEN 0 AND 120"));
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
            entity.Property(d => d.DoctorName).IsRequired().HasMaxLength(100);
            entity.Property(d => d.Specialization).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.ToTable("Appointments", table =>
                table.HasCheckConstraint(
                    "CK_Appointments_Status",
                    "[Status] IN ('Scheduled', 'Completed', 'Cancelled')"));
            entity.HasKey(a => a.AppointmentId);
            entity.Property(a => a.AppointmentDate).IsRequired();
            entity.Property(a => a.Status).IsRequired().HasMaxLength(20).HasDefaultValue("Scheduled");

            entity.HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
