using Enflow;
using Enflow.Engine;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Reference_Enflow_Builder {
    public class TypeNameToDisplayConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if(targetType.IsAssignableTo(typeof(string)) && value is string string_value) {
                try {
                    if (TypeProcessor.Instance[string_value] is TypeProcessor processor) {
                        return processor.DisplayName;
                    }
                } catch {

                }
            }
            return parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
