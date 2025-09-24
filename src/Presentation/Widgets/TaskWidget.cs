using Terminal.Gui;
using TuiSecretary.Domain.Entities;
using TuiSecretary.Domain.Enums;
using TuiSecretary.Presentation.Services;

namespace TuiSecretary.Presentation.Widgets;

public class TaskWidget : BaseWidget
{
    private readonly ICachedApiClient _apiClient;
    private ListView? _listView;
    private List<WorkTask> _tasks = new();

    public override string Name => "Tasks";

    public TaskWidget(ICachedApiClient apiClient)
    {
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
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

        _listView = new ListView()
        {
            X = 1,
            Y = 1,
            Width = Dim.Fill(1),
            Height = Dim.Fill(3),
            AllowsMarking = false,
            AllowsMultipleSelection = false
        };

        var addTaskButton = new Button("Add Task")
        {
            X = 1,
            Y = Pos.Bottom(_listView),
        };

        var startTimerButton = new Button("Start Timer")
        {
            X = Pos.Right(addTaskButton) + 1,
            Y = Pos.Bottom(_listView),
        };

        var stopTimerButton = new Button("Stop Timer")
        {
            X = Pos.Right(startTimerButton) + 1,
            Y = Pos.Bottom(_listView),
        };

        addTaskButton.Clicked += async () =>
        {
            // TODO: Implement create task API endpoint
            MessageBox.Query("Info", "Creating tasks via API not yet implemented", "OK");
            // var task = new WorkTask(
            //     "New Task", 
            //     "Task description...", 
            //     Priority.Medium, 
            //     DateTime.Now, 
            //     DateTime.Now.AddDays(7)
            // );
            // await _apiClient.CreateTaskAsync(task);
            // await RefreshAsync();
        };

        startTimerButton.Clicked += async () =>
        {
            var selectedIndex = _listView?.SelectedItem ?? -1;
            if (selectedIndex >= 0 && selectedIndex < _tasks.Count)
            {
                var task = _tasks[selectedIndex];
                try
                {
                    task.StartTimer();
                    // TODO: Implement update task API endpoint
                    MessageBox.Query("Info", "Updating tasks via API not yet implemented", "OK");
                    // await _apiClient.UpdateTaskAsync(task);
                    // await RefreshAsync();
                    // MessageBox.Query("Timer", $"Timer started for task: {task.Title}", "OK");
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.ErrorQuery("Error", ex.Message, "OK");
                }
            }
            else
            {
                MessageBox.ErrorQuery("Error", "Please select a task first", "OK");
            }
        };

        stopTimerButton.Clicked += async () =>
        {
            var selectedIndex = _listView?.SelectedItem ?? -1;
            if (selectedIndex >= 0 && selectedIndex < _tasks.Count)
            {
                var task = _tasks[selectedIndex];
                if (task.HasActiveTimer)
                {
                    task.StopActiveTimer();
                    // TODO: Implement update task API endpoint
                    MessageBox.Query("Info", "Updating tasks via API not yet implemented", "OK");
                    // await _apiClient.UpdateTaskAsync(task);
                    // await RefreshAsync();
                    // MessageBox.Query("Timer", $"Timer stopped for task: {task.Title}", "OK");
                }
                else
                {
                    MessageBox.ErrorQuery("Error", "No active timer for this task", "OK");
                }
            }
            else
            {
                MessageBox.ErrorQuery("Error", "Please select a task first", "OK");
            }
        };

        view.Add(_listView, addTaskButton, startTimerButton, stopTimerButton);
        return view;
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
            _tasks = (await _apiClient.GetTasksAsync()).ToList();
            
            Terminal.Gui.Application.MainLoop.Invoke(() =>
            {
                if (_listView != null)
                {
                    var taskStrings = _tasks.Select(t => 
                    {
                        var status = t.Status.ToString();
                        var timer = t.HasActiveTimer ? " ⏱️" : "";
                        var overdue = t.IsOverdue ? " ⚠️" : "";
                        return $"{status} - {t.Title}{timer}{overdue}";
                    }).ToList();
                    _listView.SetSource(taskStrings);
                }
            });
        }
        catch (Exception ex)
        {
            Terminal.Gui.Application.MainLoop.Invoke(() =>
            {
                MessageBox.ErrorQuery("Error", $"Failed to load tasks: {ex.Message}", "OK");
            });
        }
    }
}