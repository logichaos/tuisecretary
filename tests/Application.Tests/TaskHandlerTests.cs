using FakeItEasy;
using TuiSecretary.Application.Handlers.Tasks;
using TuiSecretary.Application.Commands.Tasks;
using TuiSecretary.Application.Queries.Tasks;
using TuiSecretary.Domain.Entities;
using TuiSecretary.Domain.Enums;
using TuiSecretary.Domain.Interfaces;
using Xunit;
using DomainPriority = TuiSecretary.Domain.Enums.Priority;

namespace TuiSecretary.Application.Tests;

public class TaskHandlerTests
{
    [Fact]
    public async Task CreateTaskHandler_Should_CreateTask_WithMinimalParameters()
    {
        // Arrange
        var unitOfWork = A.Fake<IUnitOfWork>();
        var tasksRepository = A.Fake<IRepository<WorkTask>>();
        var expectedTask = new WorkTask("Test Task");
        
        A.CallTo(() => unitOfWork.Tasks).Returns(tasksRepository);
        A.CallTo(() => tasksRepository.AddAsync(A<WorkTask>._, A<CancellationToken>._))
            .Returns(expectedTask);
        
        var handler = new CreateTaskHandler(unitOfWork);
        var command = new CreateTaskCommand("Test Task");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Task", result.Title);
        
        A.CallTo(() => tasksRepository.AddAsync(A<WorkTask>.That.Matches(t => 
            t.Title == "Test Task"), A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task CreateTaskHandler_Should_CreateTask_WithAllParameters()
    {
        // Arrange
        var unitOfWork = A.Fake<IUnitOfWork>();
        var tasksRepository = A.Fake<IRepository<WorkTask>>();
        var startDate = DateTime.Now;
        var dueDate = startDate.AddDays(7);
        var estimatedDuration = TimeSpan.FromHours(4);
        var expectedTask = new WorkTask("Test Task", "Description", DomainPriority.High, startDate, dueDate, estimatedDuration);
        
        A.CallTo(() => unitOfWork.Tasks).Returns(tasksRepository);
        A.CallTo(() => tasksRepository.AddAsync(A<WorkTask>._, A<CancellationToken>._))
            .Returns(expectedTask);
        
        var handler = new CreateTaskHandler(unitOfWork);
        var command = new CreateTaskCommand("Test Task", "Description", DomainPriority.High, startDate, dueDate, estimatedDuration);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Task", result.Title);
        Assert.Equal("Description", result.Description);
        Assert.Equal(DomainPriority.High, result.Priority);
        Assert.Equal(startDate, result.StartDate);
        Assert.Equal(dueDate, result.DueDate);
        
        A.CallTo(() => unitOfWork.SaveChangesAsync(A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task GetTaskByIdHandler_Should_ReturnTask_WhenExists()
    {
        // Arrange
        var unitOfWork = A.Fake<IUnitOfWork>();
        var tasksRepository = A.Fake<IRepository<WorkTask>>();
        var taskId = Guid.NewGuid();
        var expectedTask = new WorkTask("Test Task");
        
        A.CallTo(() => unitOfWork.Tasks).Returns(tasksRepository);
        A.CallTo(() => tasksRepository.GetByIdAsync(taskId, A<CancellationToken>._))
            .Returns(expectedTask);
        
        var handler = new GetTaskByIdHandler(unitOfWork);
        var query = new GetTaskByIdQuery(taskId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Task", result.Title);
        
        A.CallTo(() => tasksRepository.GetByIdAsync(taskId, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task GetTaskByIdHandler_Should_ReturnNull_WhenTaskNotExists()
    {
        // Arrange
        var unitOfWork = A.Fake<IUnitOfWork>();
        var tasksRepository = A.Fake<IRepository<WorkTask>>();
        var taskId = Guid.NewGuid();
        
        A.CallTo(() => unitOfWork.Tasks).Returns(tasksRepository);
        A.CallTo(() => tasksRepository.GetByIdAsync(taskId, A<CancellationToken>._))
            .Returns((WorkTask?)null);
        
        var handler = new GetTaskByIdHandler(unitOfWork);
        var query = new GetTaskByIdQuery(taskId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllTasksHandler_Should_ReturnAllTasks()
    {
        // Arrange
        var unitOfWork = A.Fake<IUnitOfWork>();
        var tasksRepository = A.Fake<IRepository<WorkTask>>();
        var expectedTasks = new List<WorkTask>
        {
            new WorkTask("Task 1"),
            new WorkTask("Task 2", "Description", DomainPriority.High)
        };
        
        A.CallTo(() => unitOfWork.Tasks).Returns(tasksRepository);
        A.CallTo(() => tasksRepository.GetAllAsync(A<CancellationToken>._))
            .Returns(expectedTasks);
        
        var handler = new GetAllTasksHandler(unitOfWork);
        var query = new GetAllTasksQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        
        A.CallTo(() => tasksRepository.GetAllAsync(A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task UpdateTaskHandler_Should_UpdateTitle_WhenProvided()
    {
        // Arrange
        var unitOfWork = A.Fake<IUnitOfWork>();
        var tasksRepository = A.Fake<IRepository<WorkTask>>();
        var taskId = Guid.NewGuid();
        var existingTask = new WorkTask("Old Title");
        
        A.CallTo(() => unitOfWork.Tasks).Returns(tasksRepository);
        A.CallTo(() => tasksRepository.GetByIdAsync(taskId, A<CancellationToken>._))
            .Returns(existingTask);
        
        var handler = new UpdateTaskHandler(unitOfWork);
        var command = new UpdateTaskCommand(taskId, Title: "New Title");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Title", result.Title);
        
        A.CallTo(() => unitOfWork.SaveChangesAsync(A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task UpdateTaskHandler_Should_UpdateStatus_WhenProvided()
    {
        // Arrange
        var unitOfWork = A.Fake<IUnitOfWork>();
        var tasksRepository = A.Fake<IRepository<WorkTask>>();
        var taskId = Guid.NewGuid();
        var existingTask = new WorkTask("Task");
        
        A.CallTo(() => unitOfWork.Tasks).Returns(tasksRepository);
        A.CallTo(() => tasksRepository.GetByIdAsync(taskId, A<CancellationToken>._))
            .Returns(existingTask);
        
        var handler = new UpdateTaskHandler(unitOfWork);
        var command = new UpdateTaskCommand(taskId, Status: TuiSecretary.Domain.Enums.TaskStatus.InProgress);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(TuiSecretary.Domain.Enums.TaskStatus.InProgress, result.Status);
        
        A.CallTo(() => unitOfWork.SaveChangesAsync(A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task UpdateTaskHandler_Should_ThrowException_WhenTaskNotFound()
    {
        // Arrange
        var unitOfWork = A.Fake<IUnitOfWork>();
        var tasksRepository = A.Fake<IRepository<WorkTask>>();
        var taskId = Guid.NewGuid();
        
        A.CallTo(() => unitOfWork.Tasks).Returns(tasksRepository);
        A.CallTo(() => tasksRepository.GetByIdAsync(taskId, A<CancellationToken>._))
            .Returns((WorkTask?)null);
        
        var handler = new UpdateTaskHandler(unitOfWork);
        var command = new UpdateTaskCommand(taskId, Title: "New Title");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await handler.Handle(command, CancellationToken.None));
        
        Assert.Contains($"Task with ID {taskId} not found", exception.Message);
    }

    [Fact]
    public async Task DeleteTaskHandler_Should_DeleteTask_WhenExists()
    {
        // Arrange
        var unitOfWork = A.Fake<IUnitOfWork>();
        var tasksRepository = A.Fake<IRepository<WorkTask>>();
        var taskId = Guid.NewGuid();
        var existingTask = new WorkTask("Task");
        
        A.CallTo(() => unitOfWork.Tasks).Returns(tasksRepository);
        A.CallTo(() => tasksRepository.GetByIdAsync(taskId, A<CancellationToken>._))
            .Returns(existingTask);
        
        var handler = new DeleteTaskHandler(unitOfWork);
        var command = new DeleteTaskCommand(taskId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(Mediator.Unit.Value, result);
        
        A.CallTo(() => tasksRepository.DeleteAsync(taskId, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task DeleteTaskHandler_Should_ThrowException_WhenTaskNotFound()
    {
        // Arrange
        var unitOfWork = A.Fake<IUnitOfWork>();
        var tasksRepository = A.Fake<IRepository<WorkTask>>();
        var taskId = Guid.NewGuid();
        
        A.CallTo(() => unitOfWork.Tasks).Returns(tasksRepository);
        A.CallTo(() => tasksRepository.GetByIdAsync(taskId, A<CancellationToken>._))
            .Returns((WorkTask?)null);
        
        var handler = new DeleteTaskHandler(unitOfWork);
        var command = new DeleteTaskCommand(taskId);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await handler.Handle(command, CancellationToken.None));
        
        Assert.Contains($"Task with ID {taskId} not found", exception.Message);
    }
}