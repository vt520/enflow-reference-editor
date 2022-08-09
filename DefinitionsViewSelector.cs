using Enflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Reference_Enflow_Builder {
    public class DefinitionsViewSelector : DataTemplateSelector {
        public override DataTemplate SelectTemplate(object item, DependencyObject container) {
            FrameworkElement? element = container as FrameworkElement;
            if(element is not null && item is KeyValuePair<string, Data> entry) {
                if (entry.Value is Table) {
                    return element.FindResource("ComplexDefinitionView") as DataTemplate;
                } else if (entry.Value is SuppliedData) {
                    return element.FindResource("SuppliedDefinitionView") as DataTemplate;
                } else {
                    return element.FindResource("SimpleDefinitionView") as DataTemplate;
                }
                
            }
            return base.SelectTemplate(item, container);
        }
    }
}
