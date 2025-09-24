using Terminal.Gui;
using TuiSecretary.Domain.Interfaces;
using TuiSecretary.Domain.Entities;

namespace TuiSecretary.Presentation.Widgets;

public class CalendarWidget : BaseWidget
{
    private readonly IUnitOfWork _unitOfWork;
    private Label? _monthYearLabel;
    private List<CalendarEvent> _events = new();
    private DateTime _selectedDate = DateTime.Today;
    private DateTime _currentMonth = DateTime.Today;
    private Label[,]? _dayLabels;
    private Label[,]? _eventIndicators;
    private View[,]? _calendarGrid;
    private const int GRID_ROWS = 6; // Maximum weeks in a month
    private const int GRID_COLS = 7; // Days of week
    private int _selectedRow = 0;
    private int _selectedCol = 0;

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

        // Month/Year display and navigation
        _monthYearLabel = new Label($"{_currentMonth:MMMM yyyy}")
        {
            X = 1,
            Y = 1,
            Width = Dim.Fill(2),
            TextAlignment = TextAlignment.Centered
        };

        var prevMonthButton = new Button("< Prev")
        {
            X = 1,
            Y = 2,
        };

        var nextMonthButton = new Button("Next >")
        {
            X = Pos.Right(prevMonthButton) + 1,
            Y = 2,
        };

        var todayButton = new Button("Today")
        {
            X = Pos.Right(nextMonthButton) + 1,
            Y = 2,
        };

        prevMonthButton.Clicked += () =>
        {
            _currentMonth = _currentMonth.AddMonths(-1);
            UpdateCalendarDisplay();
        };

        nextMonthButton.Clicked += () =>
        {
            _currentMonth = _currentMonth.AddMonths(1);
            UpdateCalendarDisplay();
        };

        todayButton.Clicked += () =>
        {
            _currentMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            _selectedDate = DateTime.Today;
            UpdateCalendarDisplay();
            UpdateSelectedPosition();
        };

        // Create calendar grid
        CreateCalendarGrid(view);

        // Add event button
        var addEventButton = new Button("Add Event")
        {
            X = 1,
            Y = Pos.Bottom(view) - 2,
        };

        addEventButton.Clicked += async () =>
        {
            var startDateTime = _selectedDate.Date.Add(new TimeSpan(9, 0, 0)); // 9 AM
            var endDateTime = startDateTime.AddHours(1); // 1 hour duration
            var calendarEvent = new CalendarEvent("New Event", startDateTime, endDateTime);
            await _unitOfWork.CalendarEvents.AddAsync(calendarEvent);
            await RefreshAsync();
        };

        view.Add(_monthYearLabel, prevMonthButton, nextMonthButton, todayButton, addEventButton);
        
        // Setup key handling for navigation
        view.KeyDown += HandleCalendarKeyPress;
        
        return view;
    }

    private void CreateCalendarGrid(View parentView)
    {
        // Day of week headers
        string[] dayHeaders = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
        for (int col = 0; col < 7; col++)
        {
            var header = new Label(dayHeaders[col])
            {
                X = 1 + col * 8,
                Y = 4,
                Width = 7,
                TextAlignment = TextAlignment.Centered
            };
            parentView.Add(header);
        }

        // Initialize calendar grid and label arrays
        _calendarGrid = new View[GRID_ROWS, GRID_COLS];
        _dayLabels = new Label[GRID_ROWS, GRID_COLS];
        _eventIndicators = new Label[GRID_ROWS, GRID_COLS];
        
        // Create calendar day cells
        for (int row = 0; row < GRID_ROWS; row++)
        {
            for (int col = 0; col < GRID_COLS; col++)
            {
                var dayView = new FrameView()
                {
                    X = 1 + col * 8,
                    Y = 5 + row * 3,
                    Width = 7,
                    Height = 3,
                    Border = new Border()
                    {
                        BorderStyle = BorderStyle.Single
                    }
                };

                var dayLabel = new Label("")
                {
                    X = 0,
                    Y = 0,
                    Width = Dim.Fill(),
                    Height = 1,
                    TextAlignment = TextAlignment.Centered
                };

                var eventIndicator = new Label("")
                {
                    X = 0,
                    Y = 1,
                    Width = Dim.Fill(),
                    Height = 1,
                    TextAlignment = TextAlignment.Centered
                };

                dayView.Add(dayLabel, eventIndicator);
                _calendarGrid[row, col] = dayView;
                _dayLabels[row, col] = dayLabel;
                _eventIndicators[row, col] = eventIndicator;
                parentView.Add(dayView);
            }
        }

        UpdateCalendarDisplay();
        UpdateSelectedPosition();
    }

    private void UpdateCalendarDisplay()
    {
        if (_monthYearLabel != null)
        {
            _monthYearLabel.Text = $"{_currentMonth:MMMM yyyy}";
        }

        if (_calendarGrid == null || _dayLabels == null || _eventIndicators == null) return;

        // Get first day of month and calculate starting position
        var firstDayOfMonth = new DateTime(_currentMonth.Year, _currentMonth.Month, 1);
        var firstDayOfWeek = (int)firstDayOfMonth.DayOfWeek;
        var daysInMonth = DateTime.DaysInMonth(_currentMonth.Year, _currentMonth.Month);

        // Clear all cells first
        for (int row = 0; row < GRID_ROWS; row++)
        {
            for (int col = 0; col < GRID_COLS; col++)
            {
                var dayView = _calendarGrid[row, col];
                var dayLabel = _dayLabels[row, col];
                var eventIndicator = _eventIndicators[row, col];
                
                dayLabel.Text = "";
                eventIndicator.Text = "";
                dayView.ColorScheme = Colors.Menu;
            }
        }

        // Fill in the days of the month
        for (int day = 1; day <= daysInMonth; day++)
        {
            int totalDays = firstDayOfWeek + day - 1;
            int row = totalDays / 7;
            int col = totalDays % 7;

            if (row < GRID_ROWS)
            {
                var dayView = _calendarGrid[row, col];
                var dayLabel = _dayLabels[row, col];
                var eventIndicator = _eventIndicators[row, col];
                
                dayLabel.Text = day.ToString();

                // Check for events on this day
                var currentDate = new DateTime(_currentMonth.Year, _currentMonth.Month, day);
                var dayEvents = _events.Where(e => e.IsOnDate(currentDate)).ToList();
                if (dayEvents.Any())
                {
                    eventIndicator.Text = $"({dayEvents.Count})";
                }

                // Highlight today
                if (currentDate.Date == DateTime.Today.Date)
                {
                    dayView.ColorScheme = Colors.Dialog;
                }

                // Highlight selected date
                if (currentDate.Date == _selectedDate.Date)
                {
                    dayView.ColorScheme = Colors.TopLevel;
                }
            }
        }
    }

    private void UpdateSelectedPosition()
    {
        if (_selectedDate.Year != _currentMonth.Year || _selectedDate.Month != _currentMonth.Month)
            return;

        var firstDayOfMonth = new DateTime(_currentMonth.Year, _currentMonth.Month, 1);
        var firstDayOfWeek = (int)firstDayOfMonth.DayOfWeek;
        var selectedDay = _selectedDate.Day;
        
        int totalDays = firstDayOfWeek + selectedDay - 1;
        _selectedRow = totalDays / 7;
        _selectedCol = totalDays % 7;
    }

    private void HandleCalendarKeyPress(View.KeyEventEventArgs e)
    {
        bool handled = true;
        
        switch (e.KeyEvent.Key)
        {
            case Key.CursorUp:
                MoveSelection(-7);
                break;
            case Key.CursorDown:
                MoveSelection(7);
                break;
            case Key.CursorLeft:
                MoveSelection(-1);
                break;
            case Key.CursorRight:
                MoveSelection(1);
                break;
            case Key.Enter:
                ShowDayDetailsPopup();
                break;
            default:
                handled = false;
                break;
        }

        if (handled)
        {
            e.Handled = true;
        }
    }

    private void MoveSelection(int dayOffset)
    {
        var newDate = _selectedDate.AddDays(dayOffset);
        
        // If we move to a different month, update the current month
        if (newDate.Month != _currentMonth.Month || newDate.Year != _currentMonth.Year)
        {
            _currentMonth = new DateTime(newDate.Year, newDate.Month, 1);
        }
        
        _selectedDate = newDate;
        UpdateCalendarDisplay();
        UpdateSelectedPosition();
    }

    private void ShowDayDetailsPopup()
    {
        var dayEvents = _events.Where(e => e.IsOnDate(_selectedDate)).OrderBy(e => e.StartDateTime).ToList();
        
        var popup = new Dialog($"Events for {_selectedDate:dddd, MMMM d, yyyy}")
        {
            Width = Dim.Percent(70),
            Height = Dim.Percent(70)
        };

        var eventsList = new ListView()
        {
            X = 1,
            Y = 1,
            Width = Dim.Fill(1),
            Height = Dim.Fill(3),
            AllowsMarking = false,
            AllowsMultipleSelection = false
        };

        var eventStrings = dayEvents.Any() 
            ? dayEvents.Select(e => $"{e.StartDateTime:HH:mm} - {e.Title}").ToList()
            : new List<string> { "No events for this day" };
        
        eventsList.SetSource(eventStrings);

        var addButton = new Button("Add Event")
        {
            X = 1,
            Y = Pos.Bottom(eventsList) + 1,
        };

        var closeButton = new Button("Close")
        {
            X = Pos.Right(addButton) + 2,
            Y = Pos.Bottom(eventsList) + 1,
        };

        addButton.Clicked += async () =>
        {
            var startDateTime = _selectedDate.Date.Add(new TimeSpan(9, 0, 0)); // 9 AM
            var endDateTime = startDateTime.AddHours(1); // 1 hour duration
            var calendarEvent = new CalendarEvent("New Event", startDateTime, endDateTime);
            await _unitOfWork.CalendarEvents.AddAsync(calendarEvent);
            await RefreshAsync();
            Terminal.Gui.Application.RequestStop(popup);
        };

        closeButton.Clicked += () =>
        {
            Terminal.Gui.Application.RequestStop(popup);
        };

        popup.Add(eventsList, addButton, closeButton);

        // Handle Esc key to close popup
        popup.KeyDown += (e) =>
        {
            if (e.KeyEvent.Key == Key.Esc)
            {
                Terminal.Gui.Application.RequestStop(popup);
                e.Handled = true;
            }
        };

        Terminal.Gui.Application.Run(popup);
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
            UpdateCalendarDisplay();
        }
        catch (Exception ex)
        {
            Terminal.Gui.Application.MainLoop.Invoke(() =>
            {
                MessageBox.ErrorQuery("Error", $"Failed to load events: {ex.Message}", "OK");
            });
        }
    }
}