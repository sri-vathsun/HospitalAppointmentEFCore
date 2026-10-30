
using HospitalAppointmentEFCore.Models;
using HospitalAppointmentEFCore.Repositories;

namespace HospitalAppointmentEFCore.Services;

public class AuditLogService
{
	private readonly AuditLogRepository _repository;

	public AuditLogService(AuditLogRepository repository)
	{
		_repository = repository;
	}

	public async Task<List<AuditLog>> GetAllAsync()
	{
		return await _repository.GetAllAsync();
	}

	public async Task<AuditLog?> GetByIdAsync(int id)
	{
		if (id <= 0)
			throw new ArgumentException("Invalid AuditLog ID.");

		return await _repository.GetByIdAsync(id);
	}

	public async Task<List<AuditLog>> GetByEntityAsync(
		string entityName,
		int entityId)
	{
		if (string.IsNullOrWhiteSpace(entityName))
			throw new ArgumentException("Entity name is required.");

		if (entityId <= 0)
			throw new ArgumentException("Invalid entity ID.");

		return await _repository.GetByEntityAsync(
			entityName, entityId);
	}

	public async Task<List<AuditLog>> GetByActionAsync(
		string action)
	{
		var validActions = new[]
		{
			"Added",
			"Modified",
			"Deleted"
		};

		if (!validActions.Contains(action,
			StringComparer.OrdinalIgnoreCase))
		{
			throw new ArgumentException("Invalid audit action.");
		}

		return await _repository.GetByActionAsync(action);
	}
}
