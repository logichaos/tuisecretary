using Terminal.Gui;
using TuiSecretary.Domain.Interfaces;
using TuiSecretary.Domain.Entities;

namespace TuiSecretary.Presentation.Widgets;

public class NotesWidget : BaseWidget
{
    private readonly IUnitOfWork _unitOfWork;
    private ListView? _listView;
    private List<Note> _notes = new();

    public override string Name => "Notes";

    public NotesWidget(IUnitOfWork unitOfWork)
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
            var note = new Note("New Note", "Enter your note content here...");
            await _unitOfWork.Notes.AddAsync(note);
            await RefreshAsync();
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
            _notes = (await _unitOfWork.Notes.GetAllAsync()).ToList();
            
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