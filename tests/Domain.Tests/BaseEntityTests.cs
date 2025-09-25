using TuiSecretary.Domain.Entities;
using Xunit;

namespace TuiSecretary.Domain.Tests;

public class BaseEntityTests
{
    private class TestEntity : BaseEntity
    {
        public string Name { get; private set; }

        public TestEntity(string name)
        {
            Name = name;
        }

        public void UpdateName(string name)
        {
            Name = name;
            SetUpdatedAt();
        }
    }

    [Fact]
    public void BaseEntity_Constructor_SetsCreatedAt()
    {
        // Act
        var entity = new TestEntity("Test");

        // Assert
        Assert.NotEqual(Guid.Empty, entity.Id);
        Assert.True(entity.CreatedAt <= DateTime.UtcNow);
        Assert.Null(entity.UpdatedAt);
    }

    [Fact]
    public void BaseEntity_Id_IsUnique()
    {
        // Act
        var entity1 = new TestEntity("Test1");
        var entity2 = new TestEntity("Test2");

        // Assert
        Assert.NotEqual(entity1.Id, entity2.Id);
    }

    [Fact]
    public void SetUpdatedAt_SetsUpdatedAtTimestamp()
    {
        // Arrange
        var entity = new TestEntity("Test");
        Thread.Sleep(10); // Ensure time difference

        // Act
        entity.UpdateName("Updated Test");

        // Assert
        Assert.NotNull(entity.UpdatedAt);
        Assert.True(entity.UpdatedAt > entity.CreatedAt);
    }

    [Fact]
    public void SetUpdatedAt_CalledMultipleTimes_UpdatesTimestamp()
    {
        // Arrange
        var entity = new TestEntity("Test");
        entity.UpdateName("First Update");
        var firstUpdateTime = entity.UpdatedAt;
        Thread.Sleep(10);

        // Act
        entity.UpdateName("Second Update");

        // Assert
        Assert.NotNull(entity.UpdatedAt);
        Assert.True(entity.UpdatedAt > firstUpdateTime);
    }

    [Fact]
    public void BaseEntity_CreatedAt_IsConsistent()
    {
        // Arrange
        var before = DateTime.UtcNow;

        // Act
        var entity = new TestEntity("Test");

        // Assert
        var after = DateTime.UtcNow;
        Assert.True(entity.CreatedAt >= before);
        Assert.True(entity.CreatedAt <= after);
    }
}