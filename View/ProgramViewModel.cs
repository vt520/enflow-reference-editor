using Enflow;

namespace Reference_Enflow_Builder.View {
    public class ProgramViewModel : Model {
        private Program? _Program = null;
        public Program Program {
            get {
                if (_Program is null) return Program = new();
                return _Program;
            }
            set {
                _Program = value;
                OnPropertyChangedAsync();
            }
        }
    }
}