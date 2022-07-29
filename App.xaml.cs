using Enflow.Engine;
using Enflow.Outcomes;
using Reference_Enflow_Builder.View;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Reference_Enflow_Builder {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        public App() {
            
        }

        private void ProgramModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) {
            Debug.WriteLine($"Property Change: {sender?.GetType()} : {e.PropertyName}");
        }
    }
}
