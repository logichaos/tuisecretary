using Terminal.Gui;
using TuiSecretary.Domain.Interfaces;
using TuiSecretary.Domain.Entities;

namespace TuiSecretary.Presentation.Widgets;

public class CalendarWidget : BaseWidget
{
    private readonly IUnitOfWork _unitOfWork;
    private ListView? _eventsListView;
    private Label? _dateLabel;
    private List<CalendarEvent> _events = new();
    private DateTime _selectedDate = DateTime.Today;

    public override string Name => "Calendar";

    public CalendarWidget(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    protected override View CreateWidgetView()
    {
        var view = new FrameView()
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
        };

        // Date display and navigation
        _dateLabel = new Label($"Date: {_selectedDate:yyyy-MM-dd}")
        {
            X = 1,
            Y = 1,
        };

        var prevDayButton = new Button("< Prev")
        {
            X = 1,
            Y = 2,
        };

        var nextDayButton = new Button("Next >")
        {
            X = Pos.Right(prevDayButton) + 1,
            Y = 2,
        };

        var todayButton = new Button("Today")
        {
            X = Pos.Right(nextDayButton) + 1,
            Y = 2,
        };

        prevDayButton.Clicked += () =>
        {
            _selectedDate = _selectedDate.AddDays(-1);
            UpdateDateDisplay();
            _ = RefreshEventsForDateAsync();
        };

        nextDayButton.Clicked += () =>
        {
            _selectedDate = _selectedDate.AddDays(1);
            UpdateDateDisplay();
            _ = RefreshEventsForDateAsync();
        };

        todayButton.Clicked += () =>
        {
            _selectedDate = DateTime.Today;
            UpdateDateDisplay();
            _ = RefreshEventsForDateAsync();
        };

        // Events list
        _eventsListView = new ListView()
        {
            X = 1,
            Y = 4,
            Width = Dim.Fill(1),
            Height = Dim.Fill(2),
            AllowsMarking = false,
            AllowsMultipleSelection = false
        };

        var addEventButton = new Button("Add Event")
        {
            X = 1,
            Y = Pos.Bottom(_eventsListView),
        };

        addEventButton.Clicked += async () =>
        {
            var startDateTime = _selectedDate.Date.Add(new TimeSpan(9, 0, 0)); // 9 AM
            var endDateTime = startDateTime.AddHours(1); // 1 hour duration
            var calendarEvent = new CalendarEvent("New Event", startDateTime, endDateTime);
            await _unitOfWork.CalendarEvents.AddAsync(calendarEvent);
            await RefreshAsync();
        };

        view.Add(_dateLabel, prevDayButton, nextDayButton, todayButton, _eventsListView, addEventButton);
        return view;
    }

    private void UpdateDateDisplay()
    {
        if (_dateLabel != null)
        {
            _dateLabel.Text = $"Date: {_selectedDate:yyyy-MM-dd} ({_selectedDate.DayOfWeek})";
        }
    }

    protected override void InitializeWidget()
    {
        _ = RefreshAsync();
    }

    public override void Refresh()
    {
        _ = RefreshAsync();
    }

    private async System.Threading.Tasks.Task RefreshAsync()
    {
        try
        {
            _events = (await _unitOfWork.CalendarEvents.GetAllAsync()).ToList();
            await RefreshEventsForDateAsync();
        }
        catch (Exception ex)
        {
            Terminal.Gui.Application.MainLoop.Invoke(() =>
            {
                MessageBox.ErrorQuery("Error", $"Failed to load events: {ex.Message}", "OK");
            });
        }
    }

    private async System.Threading.Tasks.Task RefreshEventsForDateAsync()
    {
        try
        {
            var dayEvents = _events.Where(e => e.IsOnDate(_selectedDate)).OrderBy(e => e.StartDateTime).ToList();
            
            Terminal.Gui.Application.MainLoop.Invoke(() =>
            {
                if (_eventsListView != null)
                {
                    var eventStrings = dayEvents.Any() 
                        ? dayEvents.Select(e => $"{e.StartDateTime:HH:mm} - {e.Title}").ToList()
                        : new List<string> { "No events for this day" };
                    _eventsListView.SetSource(eventStrings);
                }
            });
        }
        catch (Exception ex)
        {
            Terminal.Gui.Application.MainLoop.Invoke(() =>
            {
                MessageBox.ErrorQuery("Error", $"Failed to refresh events: {ex.Message}", "OK");
            });
        }
    }
}