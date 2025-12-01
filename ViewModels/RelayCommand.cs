// ViewModels/RelayCommand.cs
using System;
using System.Windows.Input;

namespace ZebraAI_ExecutiveRisk.ViewModels
{
    // A command implementation that simply relays execution to a delegate.
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;

        // Fix CS8612: Mark the event handler as nullable.
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        // Fix CS8625: Use nullable types in parameters.
        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        // Fix CS8767: Match the nullable parameter type expected by ICommand.
        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        // Fix CS8767: Match the nullable parameter type expected by ICommand.
        public void Execute(object? parameter)
        {
            _execute(parameter);
        }
    }
}