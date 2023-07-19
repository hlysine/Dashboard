using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Dashboard.Utilities;

public class RelayCommand<T> : ICommand
{
    #region Fields

    private readonly Action<T> execute;
    private readonly Predicate<T> canExecute;

    #endregion // Fields

    #region Constructors

    public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
    {
        this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
        this.canExecute = canExecute;
    }

    #endregion // Constructors

    #region ICommand Members

    [DebuggerStepThrough]
    public bool CanExecute(object parameter)
    {
        return canExecute?.Invoke((T)parameter) ?? true;
    }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public void Execute(object parameter)
    {
        execute((T)parameter);
    }

    #endregion // ICommand Members
}

public class RelayCommand : RelayCommand<object>
{
    public RelayCommand(Action execute, Func<bool> canExecute = null)
        : base(param => execute(), canExecute == null ? null : param => canExecute())
    {
    }
}
