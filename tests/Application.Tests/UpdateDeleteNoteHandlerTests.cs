using FakeItEasy;
using TuiSecretary.Application.Handlers.Notes;
using TuiSecretary.Application.Commands.Notes;
using TuiSecretary.Domain.Entities;
using TuiSecretary.Domain.Interfaces;
using Xunit;

namespace TuiSecretary.Application.Tests;

public class UpdateDeleteNoteHandlerTests
{
    [Fact]
    public async Task UpdateNoteHandler_Should_UpdateTitle_WhenProvided()
    {
        // Arrange
        var unitOfWork = A.Fake<IUnitOfWork>();
        var notesRepository = A.Fake<IRepository<Note>>();
        var noteId = Guid.NewGuid();
        var existingNote = new Note("Old Title", "Content");
        
        A.CallTo(() => unitOfWork.Notes).Returns(notesRepository);
        A.CallTo(() => notesRepository.GetByIdAsync(noteId, A<CancellationToken>._))
            .Returns(existingNote);
        
        var handler = new UpdateNoteHandler(unitOfWork);
        var command = new UpdateNoteCommand(noteId, Title: "New Title");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Title", result.Title);
        Assert.Equal("Content", result.Content); // Unchanged
        
        A.CallTo(() => notesRepository.GetByIdAsync(noteId, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task UpdateNoteHandler_Should_UpdateContent_WhenProvided()
    {
        // Arrange
        var unitOfWork = A.Fake<IUnitOfWork>();
        var notesRepository = A.Fake<IRepository<Note>>();
        var noteId = Guid.NewGuid();
        var existingNote = new Note("Title", "Old Content");
        
        A.CallTo(() => unitOfWork.Notes).Returns(notesRepository);
        A.CallTo(() => notesRepository.GetByIdAsync(noteId, A<CancellationToken>._))
            .Returns(existingNote);
        
        var handler = new UpdateNoteHandler(unitOfWork);
        var command = new UpdateNoteCommand(noteId, Content: "New Content");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Title", result.Title); // Unchanged
        Assert.Equal("New Content", result.Content);
        
        A.CallTo(() => unitOfWork.SaveChangesAsync(A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task UpdateNoteHandler_Should_UpdateTags_WhenProvided()
    {
        // Arrange
        var unitOfWork = A.Fake<IUnitOfWork>();
        var notesRepository = A.Fake<IRepository<Note>>();
        var noteId = Guid.NewGuid();
        var existingNote = new Note("Title", "Content", new List<string> { "old-tag" });
        var newTags = new List<string> { "new-tag1", "new-tag2" };
        
        A.CallTo(() => unitOfWork.Notes).Returns(notesRepository);
        A.CallTo(() => notesRepository.GetByIdAsync(noteId, A<CancellationToken>._))
            .Returns(existingNote);
        
        var handler = new UpdateNoteHandler(unitOfWork);
        var command = new UpdateNoteCommand(noteId, Tags: newTags);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Tags.Count);
        Assert.Contains("new-tag1", result.Tags);
        Assert.Contains("new-tag2", result.Tags);
        Assert.DoesNotContain("old-tag", result.Tags);
        
        A.CallTo(() => unitOfWork.SaveChangesAsync(A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task UpdateNoteHandler_Should_ThrowException_WhenNoteNotFound()
    {
        // Arrange
        var unitOfWork = A.Fake<IUnitOfWork>();
        var notesRepository = A.Fake<IRepository<Note>>();
        var noteId = Guid.NewGuid();
        
        A.CallTo(() => unitOfWork.Notes).Returns(notesRepository);
        A.CallTo(() => notesRepository.GetByIdAsync(noteId, A<CancellationToken>._))
            .Returns((Note?)null);
        
        var handler = new UpdateNoteHandler(unitOfWork);
        var command = new UpdateNoteCommand(noteId, Title: "New Title");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await handler.Handle(command, CancellationToken.None));
        
        Assert.Contains($"Note with ID {noteId} not found", exception.Message);
        
        A.CallTo(() => unitOfWork.SaveChangesAsync(A<CancellationToken>._))
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task DeleteNoteHandler_Should_DeleteNote_WhenExists()
    {
        // Arrange
        var unitOfWork = A.Fake<IUnitOfWork>();
        var notesRepository = A.Fake<IRepository<Note>>();
        var noteId = Guid.NewGuid();
        var existingNote = new Note("Title", "Content");
        
        A.CallTo(() => unitOfWork.Notes).Returns(notesRepository);
        A.CallTo(() => notesRepository.GetByIdAsync(noteId, A<CancellationToken>._))
            .Returns(existingNote);
        
        var handler = new DeleteNoteHandler(unitOfWork);
        var command = new DeleteNoteCommand(noteId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(Mediator.Unit.Value, result);
        
        A.CallTo(() => notesRepository.GetByIdAsync(noteId, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => notesRepository.DeleteAsync(noteId, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task DeleteNoteHandler_Should_ThrowException_WhenNoteNotFound()
    {
        // Arrange
        var unitOfWork = A.Fake<IUnitOfWork>();
        var notesRepository = A.Fake<IRepository<Note>>();
        var noteId = Guid.NewGuid();
        
        A.CallTo(() => unitOfWork.Notes).Returns(notesRepository);
        A.CallTo(() => notesRepository.GetByIdAsync(noteId, A<CancellationToken>._))
            .Returns((Note?)null);
        
        var handler = new DeleteNoteHandler(unitOfWork);
        var command = new DeleteNoteCommand(noteId);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await handler.Handle(command, CancellationToken.None));
        
        Assert.Contains($"Note with ID {noteId} not found", exception.Message);
        
        A.CallTo(() => notesRepository.DeleteAsync(A<Guid>._, A<CancellationToken>._))
            .MustNotHaveHappened();
        A.CallTo(() => unitOfWork.SaveChangesAsync(A<CancellationToken>._))
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task UpdateNoteHandler_Should_OnlyUpdateProvidedFields()
    {
        // Arrange
        var unitOfWork = A.Fake<IUnitOfWork>();
        var notesRepository = A.Fake<IRepository<Note>>();
        var noteId = Guid.NewGuid();
        var existingNote = new Note("Original Title", "Original Content", new List<string> { "original-tag" });
        
        A.CallTo(() => unitOfWork.Notes).Returns(notesRepository);
        A.CallTo(() => notesRepository.GetByIdAsync(noteId, A<CancellationToken>._))
            .Returns(existingNote);
        
        var handler = new UpdateNoteHandler(unitOfWork);
        // Only update title, leave content and tags unchanged (null means don't update)
        var command = new UpdateNoteCommand(noteId, Title: "Updated Title", Content: null, Tags: null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Title", result.Title);
        Assert.Equal("Original Content", result.Content); // Should be unchanged
        Assert.Single(result.Tags); // Should be unchanged
        Assert.Contains("original-tag", result.Tags);
    }
}