using System;

namespace Reference_Enflow_Builder.View {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class DerivedFromProperty : Attribute {
        private string _DependsOn;
        public string DependsOn { get => _DependsOn; }
        public DerivedFromProperty(string property) {
            _DependsOn = property;
        }
    }
}
