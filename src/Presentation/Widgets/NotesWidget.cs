using Terminal.Gui;
using TuiSecretary.Domain.Entities;
using TuiSecretary.Presentation.Services;

namespace TuiSecretary.Presentation.Widgets;

public class NotesWidget : BaseWidget
{
    private readonly ICachedApiClient _apiClient;
    private ListView? _listView;
    private List<Note> _notes = new();

    public override string Name => "Notes";

    public NotesWidget(ICachedApiClient apiClient)
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
            Height = Dim.Fill(2),
            AllowsMarking = false,
            AllowsMultipleSelection = false
        };

        var addButton = new Button("Add Note")
        {
            X = 1,
            Y = Pos.Bottom(_listView),
        };

        addButton.Clicked += async () =>
        {
            try
            {
                await _apiClient.CreateNoteAsync("New Note", "Enter your note content here...", new List<string>());
                await RefreshAsync();
            }
            catch (Exception ex)
            {
                MessageBox.ErrorQuery("Error", $"Failed to create note: {ex.Message}", "OK");
            }
        };

        view.Add(_listView, addButton);
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
            _notes = (await _apiClient.GetNotesAsync()).ToList();
            
            Terminal.Gui.Application.MainLoop.Invoke(() =>
            {
                if (_listView != null)
                {
                    _listView.SetSource(_notes.Select(n => $"{n.Title} - {n.CreatedAt:yyyy-MM-dd}").ToList());
                }
            });
        }
        catch (Exception ex)
        {
            Terminal.Gui.Application.MainLoop.Invoke(() =>
            {
                MessageBox.ErrorQuery("Error", $"Failed to load notes: {ex.Message}", "OK");
            });
        }
    }
}