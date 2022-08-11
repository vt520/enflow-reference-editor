using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Reference_Enflow_Builder.View {
    public class InputView : ProgramViewModel {
        public InputView() {

        }
        private ObservableCollection<InputItem>? _Items = null;
        public ObservableCollection<InputItem>? Items {
            get {
                if(_Items is null) {
                    if(Program is not null) {
                        ObservableCollection<InputItem> new_items = new();
                        Items = new_items;
                        foreach (KeyValuePair<string, string> field in Program.Application.Fields) {
                            new_items.Add(new InputItem { Name = field.Key, Format = field.Value });
                        }
                        return new_items;
                    }
                    return null;
                }
                return _Items;
            }
            set {
                if(_Items is not null) _Items.CollectionChanged -= Items_CollectionChanged;
                _Items = value;
                if (_Items is not null) _Items.CollectionChanged += Items_CollectionChanged;
                OnPropertyChangedAsync();
            }
        }

        private void Items_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            OnPropertyChangedAsync(nameof(Items));
        }

        public Dictionary<string, string?> InputDictionary {
            get {
                Dictionary<string, string?> buffer = new() { };
                if(Items is not null) {
                    foreach (InputItem item in Items) {
                        if (item.Name is null) continue;
                        buffer.Add(item.Name, item.Value);
                    }
                }
                return buffer;
            }
        }

    }
    public class InputItem : Model {
        private string? _Name = null;
        public string? Name {
            get => _Name;
            set {
                _Name = value;
                OnPropertyChangedAsync();
            }
        }
        private string? _Format = null;
        public string? Format {
            get => _Format;
            set {
                _Format = value;
            }
        }

        private string? _Value = null;
        public string? Value {
            get => _Value;
            set {
                _Value = value;
                OnPropertyChangedAsync();
            }
        }
    }
}