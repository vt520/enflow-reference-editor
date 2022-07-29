using System.ComponentModel;

namespace Reference_Enflow_Builder.View {
    public interface IExposeNotifyPropertyChanged : INotifyPropertyChanged {
        void OnPropertyChanged(string propertyName);
    }
}
