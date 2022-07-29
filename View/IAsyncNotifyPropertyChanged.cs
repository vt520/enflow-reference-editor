namespace Reference_Enflow_Builder.View {
    public interface IAsyncNotifyPropertyChanged: IExposeNotifyPropertyChanged{
        void OnPropertyChangedAsync(string propertyName);
    }
}
