using System;
using System.Windows;
using System.Windows.Input;

namespace Reference_Enflow_Builder.View {
    public class ElementCommand<T> : ICommand, ITargetsFrameworkElement {
        private FrameworkElement? _Target = null;
        public FrameworkElement? Target {
            get => _Target;
            set {
                _Target = value;
                CanExecuteChanged?.Invoke(this, new());
            }
        }
        private Action? _Dispatch;
        public Action? Dispatch {
            set {
                _Dispatch = value;
                if(_Dispatch is not null) App.Current.Dispatcher.Invoke(value);
            }
        }
        public readonly T? Item = default(T);
        public ElementCommand(T item) {
            Item = item;
        }
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) {
            return Target is not null;
        }

        public virtual void Execute(object? parameter) {
            return;
        }
    }
}
