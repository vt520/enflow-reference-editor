using Enflow;
using Enflow.Engine;

namespace Reference_Enflow_Builder.View
{
    public class DefinitionsView : ProgramViewModel {
        public ObservableDictionary<string, Data> Entries {
            get => Program.Definitions;
            set {
                if(Entries is ObservableDictionary<string, Data>)
                    Entries.CollectionChanged -= Entries_CollectionChanged;
                Program.Definitions = value;
                if (Entries is ObservableDictionary<string, Data>) 
                    Entries.CollectionChanged += Entries_CollectionChanged;
            }
        }
        public DefinitionsView() {
            Entries.CollectionChanged += Entries_CollectionChanged;
        }

        private void Entries_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            OnPropertyChangedAsync(nameof(Entries));
        }
    }
}