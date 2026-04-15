using Microsoft.EntityFrameworkCore;
using TaskManager.API.Data;
using TaskManager.API.Models;
using TaskManager.API.Services;

namespace TaskManager.Tests;

public class TaskServiceTests
{
   private AppDbContext CreateInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetAllAsync_EmptyDb_ReturnsEmptyList()
    {
        using var db = CreateInMemoryDb();
        var service = new TaskService(db);

        var result = await service.GetAllAsync(1, 10, null);

        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task CreateAsync_ValidDto_ReturnsCreatedTask()
    {
        using var db = CreateInMemoryDb();
        var service = new TaskService(db);
        var dto = new CreateTaskDto { Title = "Test Gorevi", Description = "Aciklama" };

        var result = await service.CreateAsync(dto);

        Assert.NotNull(result);
        Assert.Equal("Test Gorevi", result.Title);
        Assert.False(result.IsCompleted);
        Assert.True(result.Id > 0);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsTask()
    {
        using var db = CreateInMemoryDb();
        var service = new TaskService(db);
        var created = await service.CreateAsync(new CreateTaskDto { Title = "Gorev 1" });

        var result = await service.GetByIdAsync(created.Id);

        Assert.NotNull(result);
        Assert.Equal(created.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        using var db = CreateInMemoryDb();
        var service = new TaskService(db);

        var result = await service.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_MarkAsCompleted_SetsCompletedAt()
    {
        using var db = CreateInMemoryDb();
        var service = new TaskService(db);
        var created = await service.CreateAsync(new CreateTaskDto { Title = "Gorev" });

        var updateDto = new UpdateTaskDto
        {
            Title = "Gorev",
            IsCompleted = true
        };

        var result = await service.UpdateAsync(created.Id, updateDto);

        Assert.NotNull(result);
        Assert.True(result.IsCompleted);
        Assert.NotNull(result.CompletedAt); 
    }

    [Fact]
    public async Task DeleteAsync_ExistingId_ReturnsTrueAndRemoves()
    {
        using var db = CreateInMemoryDb();
        var service = new TaskService(db);
        var created = await service.CreateAsync(new CreateTaskDto { Title = "Silinecek" });

        var deleted = await service.DeleteAsync(created.Id);
        var afterDelete = await service.GetByIdAsync(created.Id);

        Assert.True(deleted);
        Assert.Null(afterDelete);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingId_ReturnsFalse()
    {
        using var db = CreateInMemoryDb();
        var service = new TaskService(db);

        var result = await service.DeleteAsync(999);

        Assert.False(result);
    }
}