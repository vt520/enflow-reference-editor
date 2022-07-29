using Enflow;
using Reference_Enflow_Builder.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Reference_Enflow_Builder {
    public partial class Resources : Model {
        public static readonly Resources Instance = new Resources();
        protected Resources() {
        }

        private Program? _Program = null;
        public Program Program {
            get {
                if (_Program is null) 
                    return Program = new Program();
                return _Program;
            }
            set {
                _Program = value;
                OnPropertyChangedAsync();
            }
        }

        [DerivedFromProperty(nameof(Program))]
        public object SourceView { get; set; }

        [DerivedFromProperty(nameof(Program))]
        public object DefinitionsView { get; set; }

        
        public static Dispatcher Dispatcher {
            get => App.Current.Dispatcher;
        }
    }
}
