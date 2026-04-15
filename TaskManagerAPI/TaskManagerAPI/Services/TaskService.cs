using Microsoft.EntityFrameworkCore;
using TaskManager.API.Data;
using TaskManager.API.Models;
using TaskManager.API.Services.Interfaces;

namespace TaskManager.API.Services;

// Tüm iş mantığı burada. DbContext Scoped olduğundan servis de Scoped register edilmeli

public class TaskService : ITaskService
{
    private readonly AppDbContext _db;

    public TaskService(AppDbContext db)
    {
        _db = db;
    }

    // Oluşturma tarihine göre veriyi listeleyen method.
    public async Task<PagedResult<TaskItem>> GetAllAsync(int page, int pageSize, bool? isCompleted)
    {
        var query = _db.Tasks.AsQueryable();

        if (isCompleted.HasValue)
            query = query.Where(t => t.IsCompleted == isCompleted.Value);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<TaskItem>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    // ID değerine göre veriyi bulup getiren method.
    public async Task<TaskItem?> GetByIdAsync(int id)
    {
        return await _db.Tasks.FindAsync(id);
    }

    // Request sonucunu veritabanına kaydeden method.
    public async Task<TaskItem> CreateAsync(CreateTaskDto dto)
    {
        var task = new TaskItem
        {
            Title = dto.Title.Trim(),
            Description = dto.Description?.Trim(),
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();
        return task;
    }

    // Güncelleme
    public async Task<TaskItem?> UpdateAsync(int id, UpdateTaskDto dto)
    {
        var task = await _db.Tasks.FindAsync(id);
        if (task is null) return null;

        task.Title = dto.Title.Trim();
        task.Description = dto.Description?.Trim();

        if (!task.IsCompleted && dto.IsCompleted)
            task.CompletedAt = DateTime.UtcNow;
        else if (task.IsCompleted && !dto.IsCompleted)
            task.CompletedAt = null;

        task.IsCompleted = dto.IsCompleted;

        await _db.SaveChangesAsync();
        return task;
    }

    // Silme
    public async Task<bool> DeleteAsync(int id)
    {
        var task = await _db.Tasks.FindAsync(id);
        if (task is null) return false;

        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync();
        return true;
    }
}
