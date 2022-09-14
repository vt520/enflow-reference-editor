using System.Windows;

namespace Reference_Enflow_Builder.View {
    public interface ITargetsFrameworkElement {
        public abstract FrameworkElement? Target { get; set; }
    }
}
