using Terminal.Gui;

namespace TuiSecretary.Presentation.Widgets;

public interface IWidget
{
    string Name { get; }
    View CreateView();
    void Initialize();
    void Refresh();
    void Dispose();
}