using System.ComponentModel.DataAnnotations;

namespace TaskManager.API.Models;

// Yeni görev oluşturmak için kullanılan DTO, API kontratımızı etkilemez
public class CreateTaskDto
{
    [Required(ErrorMessage = "Başlık zorunludur.")]
    [MaxLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir.")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }
}

// Mevcut görevi güncellemek için kullanılan DTO
public class UpdateTaskDto
{
    [Required(ErrorMessage = "Başlık zorunludur.")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    public bool IsCompleted { get; set; }
}
