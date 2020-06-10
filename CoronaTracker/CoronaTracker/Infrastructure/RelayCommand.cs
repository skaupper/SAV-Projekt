using System;
using System.Windows.Input;

namespace CoronaTracker.Infrastructure
{
    /// <summary>
    /// Handels the command binding of all buttons. Provides the viewModels with an abstracted command binding functionality.
    /// </summary>
    class RelayCommand : ICommand
    {
        #region Fields
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;
        #endregion Fields

        #region Constructors
        /// <summary>
        /// CTOR if only the execute method was specified
        /// </summary>
        /// <param name="execute">method which sould be carried out</param>
        public RelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// CTOR if the execute and canexecute methods are specified
        /// </summary>
        /// <param name="execute">method which sould be carried out</param>
        /// <param name="canExecute">method which checks if the action ban be executed</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }
        #endregion Constructors

        #region ICommand
        /// <summary>
        /// EventHandler that needs to be implemented for teh ICommand interface. It will add or remove a event
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Executes the specified canexecute method. If none was specifed it returns true per default
        /// </summary>
        /// <param name="parameter">parameter for the specified method</param>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        /// <summary>
        /// Executes the specified method
        /// </summary>
        /// <param name="parameter">parameter for the specified method</param>
        public void Execute(object parameter)
        {
            _execute(parameter);
        }
        #endregion ICommand
    }
}
