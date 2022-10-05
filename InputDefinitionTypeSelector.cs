using Enflow;
using Enflow.Engine;
using Reference_Enflow_Builder.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Reference_Enflow_Builder {
    public class InputDefinitionTypeSelector : OrganizedTypeModelFactory<InputDefinitionTypeSelector, InputTypeDefintionCommand> {
        public TypeProcessor? Processor {
            get => TypeProcessor.Processors[Type];
        }
        public InputDefinitionTypeSelector(Type type) : base(type) { }
        public InputDefinitionTypeSelector() : base(typeof(TypeProcessor), TypeProcessor.Processors.Keys.ToList(), false) { }
    }
    public class InputTypeDefintionCommand : ElementCommand<Type> {
        public InputTypeDefintionCommand(Type item) : base(item) {
        }
        public override void Execute(object? parameter) {
            if (Target is Button button) {
                button.DataContext = Item;
            }
            base.Execute(parameter);
        }
    }

}
