using FakeItEasy;
using TuiSecretary.Application.Handlers.Notes;
using TuiSecretary.Application.Commands.Notes;
using TuiSecretary.Application.Queries.Notes;
using TuiSecretary.Domain.Entities;
using TuiSecretary.Domain.Interfaces;
using Xunit;

namespace TuiSecretary.Application.Tests;

public class NotesHandlerTests
{
    [Fact]
    public async Task CreateNoteHandler_Should_CreateNote_And_SaveChanges()
    {
        // Arrange
        var unitOfWork = A.Fake<IUnitOfWork>();
        var notesRepository = A.Fake<IRepository<Note>>();
        var expectedNote = new Note("Test Title", "Test Content");
        
        A.CallTo(() => unitOfWork.Notes).Returns(notesRepository);
        A.CallTo(() => notesRepository.AddAsync(A<Note>._, A<CancellationToken>._))
            .Returns(expectedNote);
        
        var handler = new CreateNoteHandler(unitOfWork);
        var command = new CreateNoteCommand("Test Title", "Test Content");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Title", result.Title);
        Assert.Equal("Test Content", result.Content);
        
        A.CallTo(() => notesRepository.AddAsync(A<Note>.That.Matches(n => 
            n.Title == "Test Title" && n.Content == "Test Content"), A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task GetNoteByIdHandler_Should_ReturnNote_WhenExists()
    {
        // Arrange
        var unitOfWork = A.Fake<IUnitOfWork>();
        var notesRepository = A.Fake<IRepository<Note>>();
        var noteId = Guid.NewGuid();
        var expectedNote = new Note("Test Title", "Test Content");
        
        A.CallTo(() => unitOfWork.Notes).Returns(notesRepository);
        A.CallTo(() => notesRepository.GetByIdAsync(noteId, A<CancellationToken>._))
            .Returns(expectedNote);
        
        var handler = new GetNoteByIdHandler(unitOfWork);
        var query = new GetNoteByIdQuery(noteId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Title", result.Title);
        Assert.Equal("Test Content", result.Content);
        
        A.CallTo(() => notesRepository.GetByIdAsync(noteId, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task GetAllNotesHandler_Should_ReturnAllNotes()
    {
        // Arrange
        var unitOfWork = A.Fake<IUnitOfWork>();
        var notesRepository = A.Fake<IRepository<Note>>();
        var expectedNotes = new List<Note>
        {
            new Note("Note 1", "Content 1"),
            new Note("Note 2", "Content 2")
        };
        
        A.CallTo(() => unitOfWork.Notes).Returns(notesRepository);
        A.CallTo(() => notesRepository.GetAllAsync(A<CancellationToken>._))
            .Returns(expectedNotes);
        
        var handler = new GetAllNotesHandler(unitOfWork);
        var query = new GetAllNotesQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        
        A.CallTo(() => notesRepository.GetAllAsync(A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }
}