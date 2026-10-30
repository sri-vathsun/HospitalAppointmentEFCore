
namespace HospitalAppointmentEFCore.Models;

public class AuditLog
{
    public int AuditLogId { get; set; }

    public string EntityName { get; set; } = string.Empty;

    public int EntityId { get; set; }

    public string Action { get; set; } = string.Empty;

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public DateTime ActionDate { get; set; } = DateTime.Now;

    public string PerformedBy { get; set; } = "System";
}
