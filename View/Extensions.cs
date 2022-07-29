using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Reference_Enflow_Builder.View {
    public static partial class Extensions {

        public static async Task DispatchAction(this INotifyPropertyChanged source, Action action) {
            await Resources.Dispatcher.InvokeAsync(() => { action(); }, DispatcherPriority.DataBind);
        }
        public static void InvokeDependentProperies(this IExposeNotifyPropertyChanged source, [CallerMemberName] string? propertyName = null) {
            if (propertyName is null) return;
            foreach (PropertyInfo property in source.GetDependentProperties(propertyName)) {
                source.OnPropertyChanged(property.Name);
            }
        }
        public static List<PropertyInfo> GetDependentProperties(this INotifyPropertyChanged source, string propertyName) {
            List<PropertyInfo> list = new List<PropertyInfo>(
               source.GetType().GetRuntimeProperties().Where(
                   propinfo => propinfo.GetCustomAttributes<DerivedFromProperty>().Count() > 0
               )
            );
            List<PropertyInfo> dependents = new List<PropertyInfo> { };
            foreach (PropertyInfo property in list) {
                List<DerivedFromProperty> dependencies = new List<DerivedFromProperty>(property.GetCustomAttributes<DerivedFromProperty>());
                foreach (DerivedFromProperty dependency in dependencies) {
                    if (propertyName.Equals(dependency.DependsOn)) dependents.Add(property);
                }
            }
            return dependents;
        }

        private static ConcurrentDictionary<SourceEntry, FlexibleBag<RecieverEntry>> _ChangeEventProxies = new();
        private static FlexibleBag<IExposeNotifyPropertyChanged> _ProxiedObjects = new(); 
        public static void RegisterChangeEventProxy(
            this IAsyncNotifyPropertyChanged receiver, 
            INotifyPropertyChanged source, 
            string? sourcePropertyName = null, 
            [CallerMemberName] string? receiverPropertyName = null) {
            if (receiverPropertyName is null) throw new ArgumentNullException($"{nameof(receiverPropertyName)} must not be null");
            if (source is null) return;
            RecieverEntry receiverEntry = (receiver, receiverPropertyName);
            SourceEntry sourceEntry = (source, sourcePropertyName);

            if (!_ChangeEventProxies.TryGetValue(sourceEntry, out FlexibleBag<RecieverEntry>? receivers)) {
                receivers = new();
                _ChangeEventProxies.TryAdd(sourceEntry, receivers);
            }

            if (!receivers.Contains(receiverEntry)) receivers.Add(receiverEntry);
            
            if(!_ProxiedObjects.Contains(receiver)) {
                _ProxiedObjects.Add(receiver);
                source.PropertyChanged += Child_PropertyChanged;
            }
        }

        public static void UnregisterChangeEventProxy(
            this IAsyncNotifyPropertyChanged receiver, 
            INotifyPropertyChanged source, 
            string? sourcePropertyName = null, 
            [CallerMemberName] string? receiverPropertyName = null) {
            if (receiver is null) throw new NullReferenceException($"{nameof(receiver)} is null");
            if (receiverPropertyName is null) throw new NullReferenceException($"{nameof(receiver)} is null");
            if (source is null) return;
            SourceEntry sourceEntry = (source, sourcePropertyName);
            RecieverEntry recieverEntry = (receiver, receiverPropertyName);

            if (_ChangeEventProxies.TryGetValue(sourceEntry, out FlexibleBag<RecieverEntry>? receivers)) {
                if (receivers is null) return;
                FlexibleBag<RecieverEntry> new_receivers  = receivers.Remove(recieverEntry);
                if(new_receivers.Count != receivers.Count) {
                    if (new_receivers.Count > 0) {
                        _ChangeEventProxies[sourceEntry] = new_receivers;
                    } else {
                        _ChangeEventProxies.TryRemove(sourceEntry, out _);
                    }
                }

                if (_ChangeEventProxies.Keys.Where(item => item.Source == receiver).Count() == 0) {
                    _ProxiedObjects = _ProxiedObjects.Remove(receiver);
                }
            }
        }
        private static void Child_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
            if (sender is INotifyPropertyChanged source) {
                List<SourceEntry> sourceEntries = new List<SourceEntry> {
                    (source, e.PropertyName),
                    (source, null)
                };
                foreach (SourceEntry sourceEntry in sourceEntries.Distinct()) {
                    if (_ChangeEventProxies.TryGetValue(sourceEntry, out FlexibleBag<RecieverEntry>? receivers)) {
                        if (receivers is null) return;
                        foreach (RecieverEntry receiverEntry in receivers) {
                            receiverEntry.Source.OnPropertyChanged(receiverEntry.PropertyName);
                        }
                    }
                }                
            }
        }
    }

    internal class ProxiedObjects : FlexibleBag<IExposeNotifyPropertyChanged> { }
    internal class PropertyEntry<T> : Tuple<T, string?> { 
        public T Source { get => this.Item1; }
        public string? PropertyName { get => this.Item2; }
        public PropertyEntry(T source, string? propertyName) : base(source, propertyName) { }

        public static implicit operator PropertyEntry<T>((T source, string propertyName) value) => new (value.source, value.propertyName);
        public override bool Equals([NotNullWhen(true)] object? obj) {
            if (obj is RecieverEntry recieverEntry) {
                return recieverEntry.GetHashCode() == GetHashCode();
            }
            return base.Equals(obj);
        }
        public override int GetHashCode() {
            int hashcode = base.GetHashCode();
            if(Source is not null) hashcode ^= Source.GetHashCode();
            return hashcode; 
        }

    }
    internal class RecieverEntry : PropertyEntry<IExposeNotifyPropertyChanged> {
        public RecieverEntry(IExposeNotifyPropertyChanged source, string? propertyName) : base(source, propertyName) {
            if (propertyName is null) throw new ArgumentNullException($"{nameof(propertyName)} cannot be null");
        }
        public new string PropertyName {
            get {
                if (base.PropertyName is null) throw new NullReferenceException($"{nameof(PropertyName)} is null");
                return base.PropertyName;
            }
        }
        public static implicit operator RecieverEntry((IExposeNotifyPropertyChanged source, string propertyName) entry) => new(entry.source, entry.propertyName);
    }
    internal class SourceEntry : PropertyEntry<INotifyPropertyChanged> {
        public SourceEntry(INotifyPropertyChanged source, string? propertyName) : base(source, propertyName) {
        }
        public static implicit operator SourceEntry((INotifyPropertyChanged source, string? propertyName) entry) => new(entry.source, entry.propertyName);
    }
}
