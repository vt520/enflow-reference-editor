using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Reference_Enflow_Builder {
    internal static class Extensions {
        public static void DisplayFor(this ContextMenu menu, UIElement element) {
            element.Focus();
            menu.PlacementTarget = element;
            menu.IsOpen = true;
        }
        
        public static bool IsCreatable(this Type type) {
            return type.GetConstructor(Type.EmptyTypes) is not null;
        }
        public static Type GetCreatableBaseType(this Type type) {
            Type? parent_type = type.BaseType;
            if (parent_type is null) parent_type = typeof(object);
            while(parent_type is not null && !parent_type.IsCreatable()) {
                parent_type = parent_type.BaseType;
            }
            return parent_type!;
        }

        public static DependencyObject? FindAncestor(this DependencyObject source, Type seach_type) {
            DependencyObject current = source;
            DependencyObject? parent = null;
            while ((parent = VisualTreeHelper.GetParent(current)) is not null) {
                if (seach_type.IsAssignableFrom(parent.GetType())) return parent as DependencyObject;
                current = parent;
            }
            return default;
        }
        public static T? FindAncestor<T>(this DependencyObject source) where T : DependencyObject {
            return source.FindAncestor(typeof(T)) as T;
        }
    }
}
