using System.Windows.Input;

namespace ZebraAI.Desktop;

public sealed class RelayCommand : ICommand
{
    private readonly Func<object?, Task>? _async;
    private readonly Action<object?>? _sync;
    private readonly Predicate<object?>? _can;

    public RelayCommand(Func<object?, Task> execute, Predicate<object?>? canExecute = null)
    { _async = execute; _can = canExecute; }

    public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
    { _sync = execute; _can = canExecute; }

    public event EventHandler? CanExecuteChanged;
    public bool CanExecute(object? parameter) => _can?.Invoke(parameter) ?? true;
    public async void Execute(object? parameter)
    { if (_async != null) await _async(parameter); else _sync?.Invoke(parameter); }

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
