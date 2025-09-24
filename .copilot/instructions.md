# TUI Secretary - Copilot Instructions

## Project Overview

TUI Secretary is a cross-platform Terminal User Interface (TUI) application for managing calendar events, notes, todo lists, tasks, and task timers. The project follows **Clean Architecture** and **Domain-Driven Design** principles with **Test-Driven Development (TDD)**.

## Architecture & Patterns

### Clean Architecture Layers
- **Domain** (`src/Domain/`): Core business logic, entities, value objects, and interfaces
- **Application** (`src/Application/`): CQRS commands/queries with Mediator pattern
- **Infrastructure** (`src/Infrastructure/`): Data persistence and external services
- **Presentation** (`src/Presentation/`): Terminal.Gui TUI components

### Key Design Patterns
- **CQRS** with Mediator pattern for command/query separation
- **Repository Pattern** with Unit of Work for data access
- **Dependency Injection** using Microsoft.Extensions.DependencyInjection
- **Widget Pattern** for UI components with IWidget interface

## Technology Stack

- **.NET 8.0** - Primary runtime and SDK
- **Terminal.Gui v1.16.0** - Cross-platform TUI framework
- **Mediator** - martinothamar's Mediator for CQRS implementation
- **xUnit** - Testing framework
- **FakeItEasy** - Mocking framework for unit tests
- **Microsoft.Extensions.DependencyInjection** - DI container

## Coding Standards

### Naming Conventions
- Use PascalCase for classes, methods, properties, and public fields
- Use camelCase for private fields with underscore prefix (`_fieldName`)
- Use PascalCase for namespaces following project structure
- Command/Query classes should end with "Command" or "Query"
- Handler classes should end with "Handler"

### File Organization
- One class per file
- Match file names to class names
- Group related functionality in appropriate folders
- Follow the established folder structure within each layer

### Entity Design
- Entities should inherit from `BaseEntity`
- Use private setters with public methods for state changes
- Include validation in entity methods (throw exceptions for invalid state)
- Call `SetUpdatedAt()` when modifying entity state
- Use `[Required]` and `[MaxLength]` attributes for validation

### CQRS Implementation
- Commands modify state, Queries read state
- Each command/query should have a corresponding handler
- Handlers should be thin and delegate to domain services or repositories
- Use the Mediator pattern for decoupling

## Testing Guidelines

### Test Structure
- Follow Arrange-Act-Assert pattern
- Use descriptive test method names describing the scenario
- One logical assertion per test
- Group related tests in the same test class

### Mocking
- Use FakeItEasy for creating test doubles
- Mock external dependencies and repositories
- Don't mock value objects or entities
- Verify behavior, not just state

### Test Categories
- **Domain Tests**: Entity behavior, business rules, edge cases
- **Application Tests**: CQRS handlers with mocked dependencies
- **Infrastructure Tests**: Repository implementations, data persistence
- **Presentation Tests**: UI component behavior

## UI Development Guidelines

### Widget Development
- Implement `IWidget` interface for new widgets
- Inherit from `BaseWidget` for common functionality
- Use Terminal.Gui views and controls
- Handle keyboard navigation and accessibility
- Support refresh functionality

### Layout Management
- Support multiple layout modes (1, 2, or 4 widgets)
- Use Terminal.Gui positioning (Pos, Dim) for responsive layouts
- Implement proper focus management
- Handle window resizing gracefully

### Keyboard Shortcuts
- Follow established patterns (F1=help, F5=refresh, Ctrl+Q=quit)
- Support both arrow keys and VIM-style (h,j,k,l) navigation
- Use Tab/Shift+Tab for widget navigation
- Document new shortcuts in help system

## Development Workflow

### Building and Testing
```bash
dotnet restore          # Restore dependencies
dotnet build           # Build solution
dotnet test            # Run all tests
dotnet run --project src/Presentation/  # Run application
```

### Git Workflow
- Use meaningful commit messages
- Follow conventional commit format when possible
- Keep commits atomic and focused
- Ensure all tests pass before committing

## Common Tasks

### Adding New Entities
1. Create entity class in `src/Domain/Entities/`
2. Add repository interface in `src/Domain/Interfaces/`
3. Implement repository in `src/Infrastructure/Persistence/`
4. Add to Unit of Work
5. Create corresponding CQRS operations
6. Write comprehensive tests

### Adding New Widgets
1. Create widget class inheriting from `BaseWidget`
2. Implement required interface methods
3. Add to MainWindow widget initialization
4. Update layout management if needed
5. Add keyboard shortcuts and help text
6. Write UI tests

### Adding CQRS Operations
1. Create command/query in appropriate Application folder
2. Create handler implementing IRequestHandler
3. Register handler in DI container
4. Add validation and error handling
5. Write handler tests with mocked dependencies

## Error Handling

- Use exceptions for exceptional cases, not control flow
- Validate inputs at application boundaries
- Use Result pattern for operations that can fail gracefully
- Log errors appropriately for debugging
- Provide meaningful error messages to users

## Performance Considerations

- Use async/await for I/O operations
- Implement proper disposal patterns for UI components
- Cache expensive operations when appropriate
- Consider memory usage in long-running TUI application
- Profile and optimize hot paths

## Dependencies and Extensions

### Current Architecture Supports
- In-memory persistence (current implementation)
- Extensible to database backends (SQLite, etc.)
- Plugin-based widget system
- Configurable themes and layouts
- Import/export functionality

### When Adding Dependencies
- Prefer packages that support .NET 8.0
- Consider cross-platform compatibility
- Evaluate impact on application startup time
- Update project files and documentation
- Ensure proper dependency injection setup

## Documentation

- Update README.md for significant changes
- Document public APIs with XML comments
- Include usage examples for complex features
- Update help system for new shortcuts or features
- Maintain architectural documentation for major changes