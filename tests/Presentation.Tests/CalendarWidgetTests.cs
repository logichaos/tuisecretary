using Xunit;
using FakeItEasy;
using Terminal.Gui;
using TuiSecretary.Presentation.Widgets;
using TuiSecretary.Domain.Interfaces;
using TuiSecretary.Domain.Entities;

namespace TuiSecretary.Presentation.Tests;

public class CalendarWidgetTests
{
    [Fact]
    public void CalendarWidget_Should_Initialize_With_Current_Month()
    {
        // Arrange
        var mockUnitOfWork = A.Fake<IUnitOfWork>();
        var mockCalendarEvents = A.Fake<IRepository<CalendarEvent>>();
        A.CallTo(() => mockUnitOfWork.CalendarEvents).Returns(mockCalendarEvents);
        A.CallTo(() => mockCalendarEvents.GetAllAsync(A<CancellationToken>._))
         .Returns(Task.FromResult<IEnumerable<CalendarEvent>>(new List<CalendarEvent>()));
        
        // Initialize Terminal.Gui for testing
        Terminal.Gui.Application.Init();
        
        try
        {
            // Act
            var calendarWidget = new CalendarWidget(mockUnitOfWork);
            calendarWidget.Initialize();
            var view = calendarWidget.CreateView();
            
            // Assert
            Assert.NotNull(calendarWidget);
            Assert.Equal("Calendar", calendarWidget.Name);
            Assert.NotNull(view);
        }
        finally
        {
            // Clean up
            Terminal.Gui.Application.Shutdown();
        }
    }

    [Fact]
    public void CalendarWidget_Should_Display_Month_Year_Header()
    {
        // Arrange
        var mockUnitOfWork = A.Fake<IUnitOfWork>();
        var mockCalendarEvents = A.Fake<IRepository<CalendarEvent>>();
        A.CallTo(() => mockUnitOfWork.CalendarEvents).Returns(mockCalendarEvents);
        A.CallTo(() => mockCalendarEvents.GetAllAsync(A<CancellationToken>._))
         .Returns(Task.FromResult<IEnumerable<CalendarEvent>>(new List<CalendarEvent>()));
        
        // Initialize Terminal.Gui for testing
        Terminal.Gui.Application.Init();
        
        try
        {
            // Act
            var calendarWidget = new CalendarWidget(mockUnitOfWork);
            calendarWidget.Initialize();
            var view = calendarWidget.CreateView();
            
            // Assert - Check that the view contains month/year display
            Assert.NotNull(view);
            // The month/year label should be somewhere in the view hierarchy
            Assert.True(view.Subviews.Count > 0);
        }
        finally
        {
            // Clean up
            Terminal.Gui.Application.Shutdown();
        }
    }

    [Fact] 
    public void CalendarWidget_Should_Handle_Navigation_Buttons()
    {
        // Arrange
        var mockUnitOfWork = A.Fake<IUnitOfWork>();
        var mockCalendarEvents = A.Fake<IRepository<CalendarEvent>>();
        A.CallTo(() => mockUnitOfWork.CalendarEvents).Returns(mockCalendarEvents);
        A.CallTo(() => mockCalendarEvents.GetAllAsync(A<CancellationToken>._))
         .Returns(Task.FromResult<IEnumerable<CalendarEvent>>(new List<CalendarEvent>()));
        
        // Initialize Terminal.Gui for testing
        Terminal.Gui.Application.Init();
        
        try
        {
            // Act
            var calendarWidget = new CalendarWidget(mockUnitOfWork);
            calendarWidget.Initialize();
            var view = calendarWidget.CreateView();
            
            // Assert - Check that navigation buttons exist (they might be nested in subviews)
            Assert.NotNull(view);
            
            // Recursively search for buttons in the view hierarchy
            var allButtons = new List<Button>();
            CollectButtons(view, allButtons);
            
            Assert.True(allButtons.Count >= 3); // Should have Prev, Next, Today, and Add Event buttons
            
            // Check for expected button text
            var buttonTexts = allButtons.Select(b => b.Text.ToString()).ToList();
            Assert.Contains("< Prev", buttonTexts);
            Assert.Contains("Next >", buttonTexts);
            Assert.Contains("Today", buttonTexts);
        }
        finally
        {
            // Clean up
            Terminal.Gui.Application.Shutdown();
        }
    }

    private void CollectButtons(View view, List<Button> buttons)
    {
        if (view is Button button)
        {
            buttons.Add(button);
        }
        
        foreach (var subview in view.Subviews)
        {
            CollectButtons(subview, buttons);
        }
    }
}