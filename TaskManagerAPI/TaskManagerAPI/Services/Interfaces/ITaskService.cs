using TaskManager.API.Models;

namespace TaskManager.API.Services.Interfaces;

// 
public interface ITaskService
{

    // Verileri listeleyen method imza.
    Task<PagedResult<TaskItem>> GetAllAsync(int page, int pageSize, bool? isCompleted);
    // ID değerine göre veriyi getiren method imza.
    Task<TaskItem?> GetByIdAsync(int id);
    // Request isteğine göre veritabanına kayıt işlemi yapan method imzası.
    Task<TaskItem> CreateAsync(CreateTaskDto dto);
    // Requeste göre ID'li veriyi bulup veriyi güncelleyen method imzası.
    Task<TaskItem?> UpdateAsync(int id, UpdateTaskDto dto);
    // Silme method imzası.
    Task<bool> DeleteAsync(int id);
}
