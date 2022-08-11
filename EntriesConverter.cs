using Reference_Enflow_Builder.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Reference_Enflow_Builder {
    
    [ValueConversion(typeof(string), typeof(EntryViewItem))]
    public class EntriesConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if(value is ObservableCollection<string> source_collection) {
                ObservableCollection<EntryViewItem> result_collection = new ObservableCollection<EntryViewItem>();
                foreach (string entry in source_collection) {
                    result_collection.Add(new EntryViewItem { 
                        Source = source_collection, 
                        Value = entry 
                    });
                }
                result_collection.CollectionChanged += (s, e) => {
                    switch (e.Action) {
                        case NotifyCollectionChangedAction.Add:
                            if (e.NewItems is null) return;
                            foreach(object item in e.NewItems) {
                                if (item is EntryViewItem entry) {
                                    entry.Source = source_collection;
                                }
                            }
                            break;
                        case NotifyCollectionChangedAction.Remove:
                            if(e.OldItems is null) return;
                            foreach(object item in e.OldItems) {
                                if (item is EntryViewItem entry) {
                                    if (entry.Value is null) continue;
                                    if (entry.Source is not null) entry.Source.Remove(entry.Value);
                                }
                            }
                            break;
                    }
                };
                return result_collection;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            if(value is ObservableCollection<EntryViewItem> observableValue) {
                if (observableValue.FirstOrDefault()?.Source is ObservableCollection<string> source_value) return source_value;
            }
            return value;
        }
    }
    public class EntryViewItem : Model {
        private ObservableCollection<string>? _Source = null;
        public ObservableCollection<string>? Source {
            get => _Source;
            set {
                if(Value is not null) value?.Add(Value);
                _Source = value;
                //OnPropertyChangedAsync();
            }
        }
        private string? _Value = null;
        public string? Value { 
            get => _Value; 
            set {
                if (value is not null) {
                    if (Source is not null) {
                        bool replace_item = _Value is not null && Source.Contains(_Value);
                        if (_Value is not null && Source.IndexOf(_Value) is int index && index > 0) {
                            Source[index] = value;
                        } else {
                            if (!Source.Contains(value)) {
                                Source.Add(value);
                            } 
                        }
                    }
                    if (_Value is null) _Value = value;
                    if (_Value != value) {
                        _Value = value;
                        OnPropertyChangedAsync();
                    }
                }
            } 
        }
    }
}
