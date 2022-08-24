using Enflow.Engine;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Reference_Enflow_Builder {
    public class OutcomeTypeConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            List<OutcomeTypeWrapper> results = new List<OutcomeTypeWrapper>();
            if(value is IEnumerable<Type> list) {
                foreach (Type type in list) {
                    results.Add(new OutcomeTypeWrapper(type));
                }
            }
            results.Sort((left, right) => left.Reference.Title.CompareTo(right.Reference.Title));
            return results;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
    public class OutcomeTypeWrapper {
        
        public OutcomeTypeWrapper(Type type) {
            Type = type;
            Reference = Activator.CreateInstance(type) as Outcome;
        }

        public Type Type { get; private set; }
        public Outcome? Reference { get; private set; }

    }
}
