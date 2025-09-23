using Terminal.Gui;
using TuiSecretary.Domain.Interfaces;
using TuiSecretary.Domain.Entities;

namespace TuiSecretary.Presentation.Widgets;

public class TodoListWidget : BaseWidget
{
    private readonly IUnitOfWork _unitOfWork;
    private ListView? _listView;
    private ListView? _itemsView;
    private List<TodoList> _todoLists = new();
    private TodoList? _selectedTodoList;

    public override string Name => "Todo Lists";

    public TodoListWidget(IUnitOfWork unitOfWork)
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

        // Todo lists on the left
        var listsLabel = new Label("Lists:")
        {
            X = 1,
            Y = 1,
        };

        _listView = new ListView()
        {
            X = 1,
            Y = 2,
            Width = Dim.Percent(40),
            Height = Dim.Fill(3),
            AllowsMarking = false,
            AllowsMultipleSelection = false
        };

        _listView.SelectedItemChanged += (args) =>
        {
            var selectedIndex = args.Item;
            if (selectedIndex >= 0 && selectedIndex < _todoLists.Count)
            {
                _selectedTodoList = _todoLists[selectedIndex];
                RefreshItems();
            }
        };

        var addListButton = new Button("Add List")
        {
            X = 1,
            Y = Pos.Bottom(_listView),
        };

        addListButton.Clicked += async () =>
        {
            var todoList = new TodoList("New Todo List", "Description here...");
            await _unitOfWork.TodoLists.AddAsync(todoList);
            await RefreshAsync();
        };

        // Todo items on the right
        var itemsLabel = new Label("Items:")
        {
            X = Pos.Right(_listView) + 1,
            Y = 1,
        };

        _itemsView = new ListView()
        {
            X = Pos.Right(_listView) + 1,
            Y = 2,
            Width = Dim.Fill(1),
            Height = Dim.Fill(3),
            AllowsMarking = false,
            AllowsMultipleSelection = false
        };

        var addItemButton = new Button("Add Item")
        {
            X = Pos.Right(_listView) + 1,
            Y = Pos.Bottom(_itemsView),
        };

        addItemButton.Clicked += () =>
        {
            if (_selectedTodoList != null)
            {
                _selectedTodoList.AddItem("New Item", "Item description...");
                RefreshItems();
            }
            else
            {
                MessageBox.ErrorQuery("Error", "Please select a todo list first", "OK");
            }
        };

        view.Add(listsLabel, _listView, addListButton, itemsLabel, _itemsView, addItemButton);
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
            _todoLists = (await _unitOfWork.TodoLists.GetAllAsync()).ToList();
            
            Terminal.Gui.Application.MainLoop.Invoke(() =>
            {
                if (_listView != null)
                {
                    var listStrings = _todoLists.Select(tl => 
                        $"{tl.Name} ({tl.CompletedItemsCount}/{tl.TotalItemsCount})").ToList();
                    _listView.SetSource(listStrings);
                }
            });
        }
        catch (Exception ex)
        {
            Terminal.Gui.Application.MainLoop.Invoke(() =>
            {
                MessageBox.ErrorQuery("Error", $"Failed to load todo lists: {ex.Message}", "OK");
            });
        }
    }

    private void RefreshItems()
    {
        Terminal.Gui.Application.MainLoop.Invoke(() =>
        {
            if (_itemsView != null && _selectedTodoList != null)
            {
                var itemStrings = _selectedTodoList.Items.Select(item =>
                    $"{(item.IsCompleted ? "✓" : "☐")} {item.Title}").ToList();
                _itemsView.SetSource(itemStrings);
            }
        });
    }
}