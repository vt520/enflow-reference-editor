namespace Reference_Enflow_Builder.View {
    public class FieldItem : Model {
        private string? _Name = null;
        public string? Name { 
            get => _Name;  
            set {
                _Name = value;
                OnPropertyChangedAsync();
            } 
        }
    }
}