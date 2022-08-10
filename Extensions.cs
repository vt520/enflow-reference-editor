using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Reference_Enflow_Builder {
    internal static class Extensions {
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
