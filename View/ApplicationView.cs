using Enflow.Engine;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Reference_Enflow_Builder.View {
    public class ApplicationView : ProgramViewModel {
        public ApplicationView() {
            Program.Application.Fields.CollectionChanged += Fields_CollectionChanged;
        }
        [DerivedFromProperty(nameof(Program))]
        public FieldCollection Fields {
            get {
                return Program.Application.Fields;
            }
            set {
                if(value is not null) 
                    value.CollectionChanged += Fields_CollectionChanged;
                if(Program.Application.Fields is not null) {
                    Program.Application.Fields.CollectionChanged -= Fields_CollectionChanged;
                }
                Program.Application.Fields = value;
                OnPropertyChanged();
            }
        }

        public IList<string> FieldTypes {
            get => FieldCollection.RegisteredTypeNames;
        }
        private void Fields_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            OnPropertyChangedAsync(nameof(Fields));
        }
    }
}