using Enflow.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reference_Enflow_Builder.View {
    public class PropertyIndexerView<T> : Model, IList<Model>, INotifyCollectionChanged {

        public PropertyIndexerView(PropertyIndexer<T> indexer) {
            PropertyIndexer = indexer;
            foreach (string key in PropertyIndexer.Keys) {
                Model item = null;
                if(PropertyIndexer?.AllowableValues(key) is PropertySuggestions<T> suggestions) {
                    item = new SuggestableIndexerItemView<T>(PropertyIndexer, key, suggestions);
                } else {
                    item = new IndexerItemView<T>(PropertyIndexer, key);
                }
                if(item is not null) Items.Add(item);
            }
        }
        private PropertyIndexer<T>? _PropertyIndexer = null;
        private ObservableCollection<Model>? _Items = null;

        public Model this[int index] {
            get {
                return (Items as IList<Model>)[index];
            }
            set {
                (Items as IList<Model>)[index] = value;
            }
        }

        public PropertyIndexer<T>? PropertyIndexer {
            get { return _PropertyIndexer; }
            set {
                if(value is not null) {
                    value.PropertyChanged += PropertyIndexer_PropertyChanged;
                    value.CollectionChanged += PropertyIndexer_CollectionChanged;
                } if(_PropertyIndexer is not null) {
                    _PropertyIndexer.PropertyChanged -= PropertyIndexer_PropertyChanged;
                    _PropertyIndexer.CollectionChanged -= PropertyIndexer_CollectionChanged;
                }
                _PropertyIndexer = value;
                OnPropertyChangedAsync();
            }
        }

        public ObservableCollection<Model> Items {
            get {
                if(_Items is null) {
                    ObservableCollection<Model> value = new ();
                    Items = value;
                    return value;
                }
                return _Items;
            }
            set {
                _Items = value;
                OnPropertyChangedAsync();
            }
        }


        public int Count => (Items as ICollection<Model>).Count;

        public bool IsReadOnly => (Items as ICollection<Model>).IsReadOnly;

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        public void Add(Model item) {

        }

        public void Clear() {

        }

        public bool Contains(Model item) {
            return (Items as ICollection<Model>).Contains(item);
        }

        public void CopyTo(Model[] array, int arrayIndex) {
            (Items as ICollection<Model>).CopyTo(array, arrayIndex);
        }

        public IEnumerator<Model> GetEnumerator() {
            return (Items as IEnumerable<Model>).GetEnumerator();
        }

        public int IndexOf(Model item) {
            return (Items as IList<Model>).IndexOf(item);
        }

        public void Insert(int index, Model item) {
            
        }

        public bool Remove(Model item) {
            return false;
        }

        public void RemoveAt(int index) {
            
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return (Items as IEnumerable).GetEnumerator();
        }

        private void PropertyIndexer_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            CollectionChanged?.Invoke(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            
        }

        private void PropertyIndexer_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) {
            OnPropertyChanged(e.PropertyName);
        }
    }

    public class SuggestableIndexerItemView<T> : IndexerItemView<T> {
        private PropertySuggestions<T> suggestions;

        public SuggestableIndexerItemView(PropertyIndexer<T> propertyIndexer, string key, PropertySuggestions<T> suggestions) : base(propertyIndexer, key) {
            this.suggestions = suggestions;
        }
    }

    public class IndexerItemView<T> : Model {
        private PropertyIndexer<T> propertyIndexer;
        private string key;
        public string Property {
            get => key;
        }
        public T Value {
            get => propertyIndexer[Property];
            set {
                if (value.Equals(propertyIndexer[Property])) return;
                propertyIndexer.PropertyChanged -= PropertyIndexer_PropertyChanged;

                propertyIndexer[Property] = value;

                propertyIndexer.PropertyChanged += PropertyIndexer_PropertyChanged;
            }
        }

        private void PropertyIndexer_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) {
            OnPropertyChanged(nameof(Value)); // fix this
        }

        public IndexerItemView(PropertyIndexer<T> propertyIndexer, string key) {
            this.propertyIndexer = propertyIndexer;
            this.key = key;
        }
    }
}
