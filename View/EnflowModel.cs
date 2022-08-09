using Enflow;
using Enflow.Engine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reference_Enflow_Builder.View {
    public class ProgramModel : Model {
        private Program? _Program = null;
        public Program Program {
            get {
                if (_Program is null) return Program = new();
                return _Program;
            }
            set {
                if(_Program is not null) _Program.PropertyChanged -= _Program_PropertyChanged;
                _Program = value;
                if (_Program is not null) _Program.PropertyChanged += _Program_PropertyChanged;
                OnPropertyChangedAsync();
            }
        }

        private void _Program_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) {
            OnPropertyChangedAsync(nameof(Source));
        }

        private ObservableDictionary<string, Data>? _Definitions = null;
        [DerivedFromProperty(nameof(Program))]
        public ObservableDictionary<string, Data> Definitions {
            get {
                if(_Definitions != Program.Definitions) {
                    return Definitions = Program.Definitions; 
                }
                return _Definitions;
            }
            set {
                if (_Definitions is not null) _Definitions.CollectionChanged -= Definitions_CollectionChanged;
                _Definitions = value;
                if (_Definitions is not null) _Definitions.CollectionChanged += Definitions_CollectionChanged;
                OnPropertyChangedAsync();
            }
        }

        private void Definitions_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            OnPropertyChangedAsync(nameof(Definitions));
        }

        private QualificationsView? _Qualifications = null;
        [DerivedFromProperty(nameof(Program))]
        public Outcome Qualifications {
            get => Program.Qualifications;
            set {

                this.UnregisterChangeEventProxy(Qualifications);
                Program.Qualifications = value;
                this.RegisterChangeEventProxy(Qualifications);
                OnPropertyChangedAsync();
            }
        }
        private ApplicationView? _Application = null;
        [DerivedFromProperty(nameof(Program))]
        public ApplicationView Application {
            get {
                if (_Application is null || !_Application.Program.Equals(Program)) {
                    return Application = new() { Program = Program };
                }
                return _Application;
            }
            set {
                _Application= value;
                OnPropertyChangedAsync();
            }
        }

        //private SourceView? _Source = null;
        [DerivedFromProperty(nameof(Program))]
        public string? Source {
            get {
                return Program?.ToString();
            }
            set {
                try {
                    Program = (Program)value;
                } catch {

                }
            }
          /*  get {
                if (_Source is null || !_Source.Program.Equals(Program)) {
                    return Source = new() { Program = Program };
                }
                return _Source;
            }
            set {
                _Source = value;
                OnPropertyChangedAsync();
            }*/
        }

        private InputView? _Input = null;
        [DerivedFromProperty(nameof(Program))]
        public InputView Input {
            get {
                if (_Input is null || !_Input.Program.Equals(Program)) {
                    return Input = new() { Program = Program };
                }
                return _Input;
            }
            set {
                _Input = value;
                OnPropertyChangedAsync();
            }
        }
        public ProgramModel() {
            Definitions.CollectionChanged += Definitions_CollectionChanged;
            this.RegisterChangeEventProxy(Qualifications, null, nameof(Qualifications));
        }
        private List<DataTypeEntry> _DataTypes = null;
        public List<DataTypeEntry> DataTypes {
            get {
                if (_DataTypes is not null) return _DataTypes;
                List < DataTypeEntry > result = new() {
                    new DataTypeEntry {
                        Type = typeof(Data),
                        Name = "Data"
                    }
                };
                foreach (Type type in Data.Types) {
                    try {

                        if (Activator.CreateInstance(type) is Data source) {
                            result.Add(new DataTypeEntry { Name = source.Name, Type = type });
                        }
                    } catch {

                    }
                }
                DataTypes = result;
                return result;
            }
            set {
                _DataTypes = value;
                OnPropertyChangedAsync();
            }
        }
    }
    public class DataTypeEntry {
        public Type Type { get; set; }
        public string Name { get; set; }
    }
}
