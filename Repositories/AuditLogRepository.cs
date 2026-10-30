
using HospitalAppointmentEFCore.Data;
using HospitalAppointmentEFCore.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentEFCore.Repositories;

public class AuditLogRepository
{
    private readonly AppDbContext _context;

    public AuditLogRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<AuditLog>> GetAllAsync()
    {
        return await _context.AuditLogs
            .AsNoTracking()
            .OrderByDescending(a => a.ActionDate)
            .ToListAsync();
    }

    public async Task<AuditLog?> GetByIdAsync(int id)
    {
        return await _context.AuditLogs
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.AuditLogId == id);
    }

    public async Task<List<AuditLog>> GetByEntityAsync(
        string entityName,
        int entityId)
    {
        return await _context.AuditLogs
            .AsNoTracking()
            .Where(a =>
                a.EntityName == entityName &&
                a.EntityId == entityId)
            .OrderByDescending(a => a.ActionDate)
            .ToListAsync();
    }

    public async Task<List<AuditLog>> GetByActionAsync(
        string action)
    {
        return await _context.AuditLogs
            .AsNoTracking()
            .Where(a => a.Action == action)
            .OrderByDescending(a => a.ActionDate)
            .ToListAsync();
    }
}
