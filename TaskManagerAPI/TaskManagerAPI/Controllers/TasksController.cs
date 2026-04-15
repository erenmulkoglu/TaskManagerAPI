using Microsoft.AspNetCore.Mvc;
using TaskManager.API.Models;
using TaskManager.API.Services.Interfaces;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    /// Görevleri sayfalı ve filtrelenmiş getirir.
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<TaskItem>>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? isCompleted = null)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);
        var result = await _taskService.GetAllAsync(page, pageSize, isCompleted);
        return Ok(ApiResponse<PagedResult<TaskItem>>.Ok(result));
    }

    // Gönderilen ID parametresine göre görevi getiren endpoint.
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<TaskItem>>> GetById(int id)
    {
        var task = await _taskService.GetByIdAsync(id);
        if (task is null)
            return NotFound(ApiResponse<TaskItem>.Fail($"ID={id} olan görev bulunamadı."));
        return Ok(ApiResponse<TaskItem>.Ok(task));
    }

    // Request isteğine göre veriyi alıp db'ye yansıtan endpoint.
    [HttpPost]
    public async Task<ActionResult<ApiResponse<TaskItem>>> Create([FromBody] CreateTaskDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var created = await _taskService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id },
            ApiResponse<TaskItem>.Ok(created, "Görev oluşturuldu."));
    }

    // Gönderilen ID parametresine göre ilgili kaydı bulup güncelleyen endpoint.
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<TaskItem>>> Update(int id, [FromBody] UpdateTaskDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var updated = await _taskService.UpdateAsync(id, dto);
        if (updated is null)
            return NotFound(ApiResponse<TaskItem>.Fail($"ID={id} olan görev bulunamadı."));
        return Ok(ApiResponse<TaskItem>.Ok(updated, "Görev güncellendi."));
    }

    // Gönderilen ID değerine göre o ID'li kaydı bulup veritabanından silen endpoint.
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        var deleted = await _taskService.DeleteAsync(id);
        if (!deleted)
            return NotFound(ApiResponse<object>.Fail($"ID={id} olan görev bulunamadı."));
        return Ok(ApiResponse<object>.Ok(null, "Görev silindi."));
    }
}