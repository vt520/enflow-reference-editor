using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace Reference_Enflow_Builder.View {
    public class Model : IAsyncNotifyPropertyChanged {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            this.InvokeDependentProperies(propertyName);
        }
        
        public async void OnPropertyChangedAsync([CallerMemberName] string? propertyName = null) {
            await this.DispatchAction(
                () => {
                    OnPropertyChanged(propertyName);
                }
            );
        }
        protected virtual void Execute(ICommand command) {
            command.Execute(this);
        }
    }
}
