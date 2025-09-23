using Terminal.Gui;

namespace TuiSecretary.Presentation.Widgets;

public abstract class BaseWidget : IWidget
{
    public abstract string Name { get; }
    
    protected View? _view;
    protected bool _isInitialized;

    public virtual View CreateView()
    {
        if (_view == null)
        {
            _view = CreateWidgetView();
            _view.Border.Title = Name;
        }
        return _view;
    }

    protected abstract View CreateWidgetView();

    public virtual void Initialize()
    {
        if (!_isInitialized)
        {
            InitializeWidget();
            _isInitialized = true;
        }
    }

    protected virtual void InitializeWidget() { }

    public abstract void Refresh();

    public virtual void Dispose()
    {
        _view?.Dispose();
        _view = null;
        _isInitialized = false;
    }
}