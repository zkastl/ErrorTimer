using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows.Input;

/// <summary>
/// Version 1.0/C#6 - Dated 28-Feb-2017
/// </summary>
namespace SlimBindableCommand
{
    [DataContract]
    public abstract class ProtoBind : INotifyPropertyChanged
    {
        private Dictionary<string, object> _backingFields;

        protected T ObservableGet<T>([CallerMemberName] string propertyName = null)
        {
            if (propertyName != null)
            {
                object value;
                if (_backingFields.TryGetValue(propertyName, out value))
                    return (T)value;
            }
            return default(T);
        }
        protected void ObservableSet<T>(T value, [CallerMemberName] string propertyName = null)
        {
            if (_backingFields == null)
                _backingFields = new Dictionary<string, object>();

            if (propertyName == null)
                throw new ArgumentNullException();

            if (!_backingFields.ContainsKey(propertyName))
                _backingFields.Add(propertyName, value);
            else
                _backingFields[propertyName] = value;

            RaisePropertyChangedEvent(propertyName);
        }
        protected void RaisePropertyChangedEvent(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public abstract class CommandBase : ICommand
    {
        public bool CanExecute(object parameter) { return true; }
        public event EventHandler CanExecuteChanged;
        public abstract void Execute(object parameter);
    }

    public class ProtoSyncCommand : CommandBase
    {
        private readonly Action _action;
        public ProtoSyncCommand(Action action) { _action = action; }
        public override void Execute(object parameter) { _action(); }
    }

    public class ProtoAsyncCommand : CommandBase
    {
        private readonly Func<Task> _action;
        public ProtoAsyncCommand(Func<Task> action) { _action = action; }
        public async override void Execute(object parameter) { await Run(parameter); }
        public Task Run(object parameter) { return _action(); }
    }
}