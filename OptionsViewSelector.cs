using Enflow;
using Reference_Enflow_Builder.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Reference_Enflow_Builder {
    public class OptionsViewSelector : DataTemplateSelector {
        public override DataTemplate SelectTemplate(object item, DependencyObject container) {
            FrameworkElement? element = container as FrameworkElement;
            if (item is SuggestableIndexerItemView<string> property) {
                return element.FindResource("SuggestableOption") as DataTemplate;
            }
            return element.FindResource("FreeOption") as DataTemplate;

            return base.SelectTemplate(item, container);
        }
    }
}
