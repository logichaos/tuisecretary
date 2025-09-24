using Terminal.Gui;
using TuiSecretary.Presentation.Widgets;
using TuiSecretary.Domain.Interfaces;

namespace TuiSecretary.Presentation.Views;

public class MainWindow : Window
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly List<IWidget> _widgets = new();
    private View? _currentLayout;
    private bool _isVerticalSplit = true;

    public MainWindow(IUnitOfWork unitOfWork) : base("TUI Secretary")
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        
        X = 0;
        Y = 1; // Leave room for menu
        Width = Dim.Fill();
        Height = Dim.Fill();
        
        InitializeWidgets();
        CreateLayout();
        SetupKeyBindings();
    }

    private void InitializeWidgets()
    {
        // Create all available widgets
        _widgets.Add(new NotesWidget(_unitOfWork));
        _widgets.Add(new CalendarWidget(_unitOfWork));
        _widgets.Add(new TodoListWidget(_unitOfWork));
        _widgets.Add(new TaskWidget(_unitOfWork));
        
        // Initialize widgets
        foreach (var widget in _widgets)
        {
            widget.Initialize();
        }
    }

    private void CreateLayout()
    {
        RemoveAll();
        _currentLayout?.Dispose();

        if (_widgets.Count == 0) return;

        if (_widgets.Count == 1)
        {
            // Single widget layout
            var widget = _widgets[0];
            var view = widget.CreateView();
            view.Width = Dim.Fill();
            view.Height = Dim.Fill();
            Add(view);
            _currentLayout = view;
        }
        else if (_widgets.Count == 2)
        {
            // Split layout with first two widgets
            CreateSplitLayout(_widgets[0], _widgets[1]);
        }
        else
        {
            // Grid layout for 4 widgets (2x2)
            CreateGridLayout();
        }
    }

    private void CreateGridLayout()
    {
        var containerView = new View()
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };

        // Top-left: Notes
        if (_widgets.Count > 0)
        {
            var view = _widgets[0].CreateView();
            view.X = 0;
            view.Y = 0;
            view.Width = Dim.Percent(50);
            view.Height = Dim.Percent(50);
            containerView.Add(view);
        }

        // Top-right: Calendar
        if (_widgets.Count > 1)
        {
            var view = _widgets[1].CreateView();
            view.X = Pos.Percent(50);
            view.Y = 0;
            view.Width = Dim.Fill();
            view.Height = Dim.Percent(50);
            containerView.Add(view);
        }

        // Bottom-left: Todo Lists
        if (_widgets.Count > 2)
        {
            var view = _widgets[2].CreateView();
            view.X = 0;
            view.Y = Pos.Percent(50);
            view.Width = Dim.Percent(50);
            view.Height = Dim.Fill();
            containerView.Add(view);
        }

        // Bottom-right: Tasks
        if (_widgets.Count > 3)
        {
            var view = _widgets[3].CreateView();
            view.X = Pos.Percent(50);
            view.Y = Pos.Percent(50);
            view.Width = Dim.Fill();
            view.Height = Dim.Fill();
            containerView.Add(view);
        }

        Add(containerView);
        _currentLayout = containerView;
    }

    private void CreateSplitLayout(IWidget leftWidget, IWidget rightWidget)
    {
        var leftView = leftWidget.CreateView();
        var rightView = rightWidget.CreateView();

        if (_isVerticalSplit)
        {
            // Vertical split (side by side)
            leftView.X = 0;
            leftView.Y = 0;
            leftView.Width = Dim.Percent(50);
            leftView.Height = Dim.Fill();

            rightView.X = Pos.Right(leftView);
            rightView.Y = 0;
            rightView.Width = Dim.Fill();
            rightView.Height = Dim.Fill();
        }
        else
        {
            // Horizontal split (top and bottom)
            leftView.X = 0;
            leftView.Y = 0;
            leftView.Width = Dim.Fill();
            leftView.Height = Dim.Percent(50);

            rightView.X = 0;
            rightView.Y = Pos.Bottom(leftView);
            rightView.Width = Dim.Fill();
            rightView.Height = Dim.Fill();
        }

        Add(leftView, rightView);
        _currentLayout = new View();
        _currentLayout.Add(leftView, rightView);
    }

    private void SetupKeyBindings()
    {
        // Override ProcessKey to handle custom key bindings
        KeyDown += (e) =>
        {
            switch (e.KeyEvent.Key)
            {
                case Key.F5:
                    RefreshAllWidgets();
                    e.Handled = true;
                    break;
                case Key.CtrlMask | Key.T:
                    ToggleSplitOrientation();
                    e.Handled = true;
                    break;
                case Key.F1:
                    ShowHelp();
                    e.Handled = true;
                    break;
                case Key.CtrlMask | Key.Q:
                    Terminal.Gui.Application.RequestStop();
                    e.Handled = true;
                    break;
                // VIM navigation keys for within-widget navigation
                case Key.h:
                    HandleVimNavigation(Key.CursorLeft);
                    e.Handled = true;
                    break;
                case Key.j:
                    HandleVimNavigation(Key.CursorDown);
                    e.Handled = true;
                    break;
                case Key.k:
                    HandleVimNavigation(Key.CursorUp);
                    e.Handled = true;
                    break;
                case Key.l:
                    HandleVimNavigation(Key.CursorRight);
                    e.Handled = true;
                    break;
            }
        };
    }

    private void HandleVimNavigation(Key arrowKey)
    {
        // Get the currently focused view
        var focused = Terminal.Gui.Application.Top.MostFocused;
        if (focused == null) return;

        // Find the widget container that contains the focused view
        var widgetContainer = FindWidgetContainer(focused);
        if (widgetContainer == null) return;

        // For h,j,k,l navigation, we want to move focus between elements within the widget
        // Similar to how Tab/Shift+Tab works, but using directional logic
        switch (arrowKey)
        {
            case Key.CursorLeft: // h key
            case Key.CursorUp:   // k key
                // Move focus to previous focusable element (like Shift+Tab)
                MoveFocusWithinWidget(widgetContainer, reverse: true);
                break;
            case Key.CursorRight: // l key  
            case Key.CursorDown:  // j key
                // Move focus to next focusable element (like Tab)
                MoveFocusWithinWidget(widgetContainer, reverse: false);
                break;
        }
    }

    private View? FindWidgetContainer(View focused)
    {
        // Walk up the view hierarchy to find the widget's root FrameView
        var current = focused;
        while (current != null)
        {
            // Widget containers are typically FrameView instances that are direct children of the main layout
            if (current is FrameView frameView && current.SuperView != null)
            {
                // Check if this FrameView is a widget container (has a border title)
                if (!string.IsNullOrEmpty(frameView.Border?.Title?.ToString()))
                {
                    return frameView;
                }
            }
            current = current.SuperView;
        }
        return null;
    }

    private void MoveFocusWithinWidget(View widgetContainer, bool reverse)
    {
        // Get all focusable views within the widget container
        var focusableViews = GetFocusableViews(widgetContainer);
        if (focusableViews.Count <= 1) return;

        // Find the currently focused view in the list
        var currentFocused = Terminal.Gui.Application.Top.MostFocused;
        var currentIndex = focusableViews.IndexOf(currentFocused);
        
        if (currentIndex == -1) 
        {
            // If current focused view is not in the list, focus the first one
            focusableViews[0].SetFocus();
            return;
        }

        // Calculate next index
        int nextIndex;
        if (reverse)
        {
            nextIndex = currentIndex == 0 ? focusableViews.Count - 1 : currentIndex - 1;
        }
        else
        {
            nextIndex = currentIndex == focusableViews.Count - 1 ? 0 : currentIndex + 1;
        }

        // Set focus to the next view
        focusableViews[nextIndex].SetFocus();
    }

    private List<View> GetFocusableViews(View container)
    {
        var focusableViews = new List<View>();
        CollectFocusableViews(container, focusableViews);
        return focusableViews;
    }

    private void CollectFocusableViews(View view, List<View> focusableViews)
    {
        // Add this view if it can be focused (and is not the container itself)
        if (view.CanFocus && view.Visible && view.Enabled)
        {
            focusableViews.Add(view);
        }

        // Recursively check subviews
        foreach (View subview in view.Subviews)
        {
            CollectFocusableViews(subview, focusableViews);
        }
    }

    private void RefreshAllWidgets()
    {
        foreach (var widget in _widgets)
        {
            widget.Refresh();
        }
    }

    private void ToggleSplitOrientation()
    {
        _isVerticalSplit = !_isVerticalSplit;
        CreateLayout();
    }

    private void ShowHelp()
    {
        var helpText = @"TUI Secretary - Keyboard Shortcuts

F1          - Show this help
F5          - Refresh all widgets
Ctrl+T      - Toggle split orientation for 2 widgets
Ctrl+Q      - Quit application

Layout:
- 1 widget: Full screen
- 2 widgets: Split view (toggle with Ctrl+T)  
- 4 widgets: Grid view (2x2)
  Top-left: Notes, Top-right: Calendar
  Bottom-left: Todo Lists, Bottom-right: Tasks

Navigation:
Tab         - Move between widgets
Shift+Tab   - Move between widgets (reverse)
Arrow Keys  - Navigate within widgets
h,j,k,l     - VIM-style navigation within widgets (left,down,up,right)
Enter       - Select/Action
Esc         - Cancel/Back

Widget Features:
- Notes: Add notes with tags
- Calendar: Navigate dates, add events
- Todo Lists: Manage lists and items
- Tasks: Start/stop timers, track progress";

        MessageBox.Query("Help", helpText, "OK");
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            foreach (var widget in _widgets)
            {
                widget.Dispose();
            }
            _widgets.Clear();
            _currentLayout?.Dispose();
        }
        base.Dispose(disposing);
    }
}