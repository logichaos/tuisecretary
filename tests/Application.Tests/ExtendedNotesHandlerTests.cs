using FakeItEasy;
using TuiSecretary.Application.Handlers.Notes;
using TuiSecretary.Application.Commands.Notes;
using TuiSecretary.Application.Queries.Notes;
using TuiSecretary.Domain.Entities;
using TuiSecretary.Domain.Interfaces;
using Xunit;

namespace TuiSecretary.Application.Tests;

public class ExtendedNotesHandlerTests
{
    [Fact]
    public async System.Threading.Tasks.Task CreateNoteHandler_Should_HandleNullTagsList()
    {
        // Arrange
        var unitOfWork = A.Fake<IUnitOfWork>();
        var notesRepository = A.Fake<IRepository<Note>>();
        var expectedNote = new Note("Test Title", "Test Content");
        
        A.CallTo(() => unitOfWork.Notes).Returns(notesRepository);
        A.CallTo(() => notesRepository.AddAsync(A<Note>._, A<CancellationToken>._))
            .Returns(expectedNote);
        
        var handler = new CreateNoteHandler(unitOfWork);
        var command = new CreateNoteCommand("Test Title", "Test Content", null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Title", result.Title);
        Assert.Equal("Test Content", result.Content);
        Assert.Empty(result.Tags);
        
        A.CallTo(() => notesRepository.AddAsync(A<Note>._, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateNoteHandler_Should_HandleEmptyContent()
    {
        // Arrange
        var unitOfWork = A.Fake<IUnitOfWork>();
        var notesRepository = A.Fake<IRepository<Note>>();
        var expectedNote = new Note("Test Title", "");
        
        A.CallTo(() => unitOfWork.Notes).Returns(notesRepository);
        A.CallTo(() => notesRepository.AddAsync(A<Note>._, A<CancellationToken>._))
            .Returns(expectedNote);
        
        var handler = new CreateNoteHandler(unitOfWork);
        var command = new CreateNoteCommand("Test Title", "");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Title", result.Title);
        Assert.Equal("", result.Content);
        
        A.CallTo(() => notesRepository.AddAsync(A<Note>._, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateNoteHandler_Should_HandleTagsList()
    {
        // Arrange
        var unitOfWork = A.Fake<IUnitOfWork>();
        var notesRepository = A.Fake<IRepository<Note>>();
        var tags = new List<string> { "important", "work" };
        var expectedNote = new Note("Test Title", "Test Content", tags);
        
        A.CallTo(() => unitOfWork.Notes).Returns(notesRepository);
        A.CallTo(() => notesRepository.AddAsync(A<Note>._, A<CancellationToken>._))
            .Returns(expectedNote);
        
        var handler = new CreateNoteHandler(unitOfWork);
        var command = new CreateNoteCommand("Test Title", "Test Content", tags);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Title", result.Title);
        Assert.Equal("Test Content", result.Content);
        Assert.Equal(2, result.Tags.Count);
        Assert.Contains("important", result.Tags);
        Assert.Contains("work", result.Tags);
        
        A.CallTo(() => notesRepository.AddAsync(A<Note>._, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async System.Threading.Tasks.Task GetNoteByIdHandler_Should_ReturnNull_WhenNotExists()
    {
        // Arrange
        var unitOfWork = A.Fake<IUnitOfWork>();
        var notesRepository = A.Fake<IRepository<Note>>();
        var noteId = Guid.NewGuid();
        
        A.CallTo(() => unitOfWork.Notes).Returns(notesRepository);
        A.CallTo(() => notesRepository.GetByIdAsync(noteId, A<CancellationToken>._))
            .Returns((Note?)null);
        
        var handler = new GetNoteByIdHandler(unitOfWork);
        var query = new GetNoteByIdQuery(noteId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
        
        A.CallTo(() => notesRepository.GetByIdAsync(noteId, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async System.Threading.Tasks.Task GetAllNotesHandler_Should_ReturnEmptyList_WhenNoNotes()
    {
        // Arrange
        var unitOfWork = A.Fake<IUnitOfWork>();
        var notesRepository = A.Fake<IRepository<Note>>();
        var emptyNotesList = new List<Note>();
        
        A.CallTo(() => unitOfWork.Notes).Returns(notesRepository);
        A.CallTo(() => notesRepository.GetAllAsync(A<CancellationToken>._))
            .Returns(emptyNotesList);
        
        var handler = new GetAllNotesHandler(unitOfWork);
        var query = new GetAllNotesQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        
        A.CallTo(() => notesRepository.GetAllAsync(A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async System.Threading.Tasks.Task GetAllNotesHandler_Should_ReturnMultipleNotes()
    {
        // Arrange
        var unitOfWork = A.Fake<IUnitOfWork>();
        var notesRepository = A.Fake<IRepository<Note>>();
        var expectedNotes = new List<Note>
        {
            new Note("Note 1", "Content 1", new List<string> { "tag1" }),
            new Note("Note 2", "Content 2", new List<string> { "tag2" }),
            new Note("Note 3", "Content 3")
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
        Assert.Equal(3, result.Count());
        
        var resultList = result.ToList();
        Assert.Equal("Note 1", resultList[0].Title);
        Assert.Equal("Note 2", resultList[1].Title);
        Assert.Equal("Note 3", resultList[2].Title);
        
        A.CallTo(() => notesRepository.GetAllAsync(A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateNoteHandler_Should_HandleCancellationToken()
    {
        // Arrange
        var unitOfWork = A.Fake<IUnitOfWork>();
        var notesRepository = A.Fake<IRepository<Note>>();
        var expectedNote = new Note("Test Title", "Test Content");
        var cancellationToken = new CancellationToken();
        
        A.CallTo(() => unitOfWork.Notes).Returns(notesRepository);
        A.CallTo(() => notesRepository.AddAsync(A<Note>._, cancellationToken))
            .Returns(expectedNote);
        
        var handler = new CreateNoteHandler(unitOfWork);
        var command = new CreateNoteCommand("Test Title", "Test Content");

        // Act
        var result = await handler.Handle(command, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Title", result.Title);
        
        A.CallTo(() => notesRepository.AddAsync(A<Note>._, cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken))
            .MustHaveHappenedOnceExactly();
    }
}