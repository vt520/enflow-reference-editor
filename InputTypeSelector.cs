using Enflow;
using Enflow.Engine;
using Reference_Enflow_Builder.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Reference_Enflow_Builder {
    public class InputTypeSelector : OrganizedTypeModelFactory<InputTypeSelector, ReplaceInputType> {
        public TypeProcessor? Processor {
            get => TypeProcessor.Processors[Type];
        }
        public InputTypeSelector(Type type) : base(type) { }
        public InputTypeSelector() : base(typeof(TypeProcessor), TypeProcessor.Processors.Keys.ToList(), false) { }
    }
    public class ReplaceInputType : ElementCommand<Type> {
        public ReplaceInputType(Type item) : base(item) {
        }
        public override void Execute(object? parameter) {
            if (Target is Button button) {
                _ = button.DataContext;
                if (button.DataContext is KeyValuePair<string, string> context) {
                    Program current_program = MainWindow.Program;
                    
                    Dispatch = () => {
                        
                        current_program.Application.Fields[context.Key] = TypeProcessor.NameFromProcessorType(Item.EnflowName());
                    };
                }
            }
            base.Execute(parameter);
        }
    }

}
