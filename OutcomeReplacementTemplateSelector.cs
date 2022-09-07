using Enflow;
using Enflow.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Reference_Enflow_Builder {
    public class OutcomeReplacementTemplateSelector : DataTemplateSelector {
        public override DataTemplate? SelectTemplate(object item, DependencyObject container) {
            FrameworkElement? element = container as FrameworkElement;
            if (element is not null && item is OutcomeTypeWrapper entry) {
                if(entry.Type.IsAssignableTo(typeof(ProcessResult))) return element?.FindResource("ResultOutcomeReplacement") as DataTemplate;

            }
            return element?.FindResource("GeneralOutcomeReplacement") as DataTemplate;
        }
    }
}
