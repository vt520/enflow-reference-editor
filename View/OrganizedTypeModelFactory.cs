using Enflow;
using Enflow.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Printing;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Reference_Enflow_Builder.View {
    public interface ICommandable {
        public ICommand? Command { get; set; }
    }
    
    public class OrganizedTypeModelFactory<T, C> : Model, ICommandable, IEnumerable<T?> where T:OrganizedTypeModelFactory<T, C>  where C:ElementCommand<Type> {
        private string? _Title;
        private string? _Description;
        
        public virtual string? Title {
            get {
                if (Type is null) return null;
                if (_Title is null) {
                    Title = Type.GetDisplayName();
                }
                return _Title;
            }
            set {
                _Title = value;
                OnPropertyChanged();
            }
        }

        public virtual string? Description {
            get {
                if (Type is null) return null;
                if(_Description is null) {
                    Description = Type.GetDescription();
                }
                return _Description;
            }
            set {
                _Description = value;
                OnPropertyChanged();
            }
        }

        private C? _Command = null;
        public C? Command {
            get => _Command;
            set {
                _Command = value;
                OnPropertyChanged();
            }
        }
        private Type? _Type = null;

        public virtual Type? Type {
            get => _Type;
            set {
                _Type = value;
                if (value is not null) {
                    C? command = null;
                    if(value.IsCreatable()) command = Activator.CreateInstance(typeof(C), new object[] { value }) as C;
                    Command = command;
                } else {
                    Command = null;
                }
                OnPropertyChanged();
            }
        }

        public virtual bool IsEnabled { get => true; }
        public OrganizedTypeModelFactory(Type? root_type, List<Type>? child_types = null, bool recurse_to_public = false) {
            if (root_type is null) root_type = typeof(object);
            Type = root_type;
            if (child_types is null) return;
            
            IsRoot = true;

            Dictionary<Type, T> unorganized_children = new() {{ root_type, (T)this}};
            Stack<Type> type_stack = new(child_types);
            while(type_stack.TryPop(out Type? test_type)) {
                if (unorganized_children.ContainsKey(test_type)) continue;
                if (Activator.CreateInstance(typeof(T), new object[] { test_type }) is T new_element) {
                    unorganized_children.Add(test_type, new_element);
                }
                //Type? parent_type = recurse_to_public ? test_type.GetCreatableBaseType() : test_type.BaseType;
                Type parent_type = test_type.GetLogicalParent(recurse_to_public);
                if (parent_type is null || !parent_type.IsAssignableTo(root_type)) parent_type = root_type;
                if (parent_type == root_type) continue;
                if (type_stack.Contains(parent_type) || unorganized_children.ContainsKey(parent_type)) continue;
                type_stack.Push(parent_type);
            }

            foreach(Type child_type in unorganized_children.Keys) {
                T? child_element = unorganized_children[child_type];
                Type? parent_type = child_type.GetLogicalParent(recurse_to_public);//recurse_to_public ? child_type.GetCreatableBaseType() : child_type.BaseType;

                if (parent_type is null || !parent_type.IsAssignableTo(root_type)) parent_type = root_type;
                T? parent_element = unorganized_children[parent_type];
                if (parent_element is null) parent_element = this as T;
                if (parent_element == child_element) continue;
                child_element.Parent = parent_element;
                Debug.WriteLine($"{child_element.Type.Name} is a child of {parent_element.Type.Name}");
                parent_element!.Children.Add(child_element);
            }
            return;
        }
        private T? _Parent;
        public T? Parent {
            get => _Parent;
            set {
                _Parent = value;
                OnPropertyChanged();
            }
        }

        public OrganizedTypeModelFactory(List<Type> types) : this(null, types) {
        }

        public IEnumerator<T> GetEnumerator() {
            return ((IEnumerable<T>)Children).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable)Children).GetEnumerator();
        }

        private ObservableCollection<T?>? _Children;
        public ObservableCollection<T?> Children {
            get {
                if (_Children is null) Children = new();
                return _Children!;
            }
            set {
                if (_Children is not null) _Children.CollectionChanged -= Children_CollectionChanged;
                _Children = value;
                if(_Children is not null) _Children.CollectionChanged += Children_CollectionChanged;
                OnPropertyChanged();
            }
        }
        private bool _IsHeading = false;
        public bool IsHeading {
            get => _IsHeading;
            protected set {
                _IsHeading = value;
                OnPropertyChanged();
            }
        }
        private T? _Self = null;
        protected T? Self {
            get {
                if(_Self is null && Type is not null) {
                    _Self = Activator.CreateInstance(typeof(T), new object[] { Type }) as T;
                    if (_Self is not null) _Self.IsHeading = true;
                }
                return _Self;
            }
        }
        private bool _IsRoot = false;
        public bool IsRoot {
            get => _IsRoot;
            protected set {
                _IsRoot = value;
                OnPropertyChanged();
            }
        }

        ICommand? ICommandable.Command { 
            get => Command; 
            set  {
                if (value is C command) {
                    Command = command;
                } else {
                    throw new ArgumentException();
                }
            } 
        }

        private void Children_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            if(Children.Count > 0) {
                if (IsRoot) return;
                if (Self is not null && !Children.Contains(Self)) {
                    if (Command is null) return;
                    Children.Insert(0, Self);
                    Children.Insert(1, null);
                }
            }
        }
    }
}
