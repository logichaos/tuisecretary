# TUI Secretary

A cross-platform Terminal User Interface (TUI) application for managing calendar events, notes, todo lists, tasks, and task timers. Built with .NET 8.0, Terminal.Gui, and following Domain-Driven Design (DDD) and Clean Architecture principles.

## Features

- **📝 Notes Management**: Create, view, and organize notes with tags and favorites
- **📅 Calendar**: Navigate dates and manage calendar events  
- **✅ Todo Lists**: Create multiple todo lists with items and completion tracking
- **📋 Tasks**: Manage tasks with priority levels, due dates, and time tracking
- **⏱️ Task Timers**: Start/stop timers for tasks and track actual time spent
- **📊 Split Views**: Configurable layouts with horizontal and vertical splits
- **🎨 Grid Layout**: 2x2 grid layout for all four widgets simultaneously

## Architecture

The application follows **Clean Architecture** and **Domain-Driven Design** principles:

```
├── src/
│   ├── Domain/           # Core business logic and entities
│   ├── Application/      # CQRS commands/queries with Mediator pattern
│   ├── Infrastructure/   # External concerns (persistence, services)
│   └── Presentation/     # Terminal.Gui TUI application
└── tests/
    ├── Domain.Tests/     # Domain layer unit tests
    ├── Application.Tests/# Application layer unit tests (using FakeItEasy)
    ├── Infrastructure.Tests/ # Infrastructure layer unit tests
    └── Presentation.Tests/   # Presentation layer unit tests
```

### Key Technologies

- **.NET 8.0** - Modern C# runtime and SDK
- **Terminal.Gui v1.16.0** - Cross-platform TUI framework
- **Mediator Pattern** - CQRS implementation with martinothamar's Mediator
- **FakeItEasy** - Mocking framework for comprehensive unit testing
- **xUnit** - Testing framework
- **Dependency Injection** - Microsoft.Extensions.DependencyInjection

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- Terminal/Console that supports ANSI escape sequences

### Building the Application

```bash
# Clone the repository
git clone <repository-url>
cd tuisecretary

# Restore dependencies and build
dotnet restore
dotnet build

# Run tests
dotnet test

# Run the application
dotnet run --project src/Presentation/
```

## Usage

### Main Interface

The application displays a 2x2 grid layout with four widgets:

- **Top-left**: Notes widget
- **Top-right**: Calendar widget  
- **Bottom-left**: Todo Lists widget
- **Bottom-right**: Tasks widget

### Keyboard Shortcuts

- **F1** - Show help dialog
- **F5** - Refresh all widgets
- **Ctrl+T** - Toggle split orientation (for 2-widget layout)
- **Ctrl+Q** - Quit application
- **Tab** - Navigate between widgets
- **Shift+Tab** - Navigate between widgets (reverse)
- **Arrow Keys** - Navigate within widgets
- **h,j,k,l** - VIM-style navigation within widgets (left, down, up, right)
- **Enter** - Select/Activate
- **Esc** - Cancel/Back

### Widget Features

#### Notes Widget
- Add new notes with title and content
- Support for tags and favorites
- View all notes with creation dates

#### Calendar Widget
- Navigate between dates (Previous/Next/Today buttons)
- View events for selected date
- Add new calendar events

#### Todo Lists Widget
- Create multiple todo lists
- Add items to selected lists
- Track completion progress
- Visual completion indicators (✓/☐)

#### Tasks Widget
- Create tasks with priority levels and due dates
- Start/stop timers for time tracking
- Visual indicators for active timers (⏱️) and overdue tasks (⚠️)
- Status tracking (NotStarted, InProgress, Completed, etc.)

## Domain Models

### Core Entities

- **Note**: Title, content, tags, favorite status
- **CalendarEvent**: Title, description, start/end times, location, attendees
- **TodoList**: Name, description, color, collection of TodoItems
- **TodoItem**: Title, description, completion status, priority, due date
- **WorkTask**: Title, description, status, priority, dates, time tracking
- **TaskTimer**: Start/end times, duration tracking, notes

### Value Objects and Enums

- **Priority**: Low, Medium, High, Critical
- **TaskStatus**: NotStarted, InProgress, Completed, Cancelled, OnHold

## Testing Strategy

The project follows **Test-Driven Development (TDD)** principles with comprehensive test coverage:

- **Domain Tests**: Entity behavior, business rules, edge cases
- **Application Tests**: CQRS handlers with mocked dependencies (FakeItEasy)
- **Infrastructure Tests**: Repository implementations, data persistence
- **Presentation Tests**: UI component behavior

### Running Tests

```bash
# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --verbosity normal

# Run tests for specific project
dotnet test tests/Domain.Tests/
```

## Project Structure Details

### Domain Layer (`src/Domain/`)
- `Entities/`: Core business entities with encapsulated behavior
- `Enums/`: Domain-specific enumerations  
- `Interfaces/`: Repository and unit of work abstractions
- `ValueObjects/`: Immutable value objects (future expansion)

### Application Layer (`src/Application/`)
- `Commands/`: CQRS command definitions
- `Queries/`: CQRS query definitions  
- `Handlers/`: Command and query handlers
- `Common/`: Shared application services

### Infrastructure Layer (`src/Infrastructure/`)
- `Persistence/`: In-memory repository implementations
- `Services/`: External service integrations (future)

### Presentation Layer (`src/Presentation/`)
- `Views/`: Main application windows
- `Widgets/`: Reusable UI components for each domain area
- `Services/`: UI-specific services

## Configuration and Extensibility

The application is designed for extensibility:

- **New Widgets**: Implement `IWidget` interface
- **Persistence**: Replace `InMemoryRepository` with database implementations
- **UI Themes**: Extend Terminal.Gui color schemes
- **Additional Features**: Add new domain entities and corresponding CQRS operations

## Future Enhancements

- **Persistence**: SQLite or other database backends
- **Import/Export**: JSON/CSV data exchange
- **Notifications**: Due date reminders and alerts  
- **Search**: Full-text search across all content
- **Themes**: Customizable color schemes
- **Plugins**: Extensible widget system
- **Synchronization**: Cloud sync capabilities

## Contributing

1. Follow the existing architectural patterns
2. Maintain high test coverage
3. Use meaningful commit messages
4. Ensure all tests pass before submitting changes

## License

This project is licensed under the MIT License - see the LICENSE file for details.
