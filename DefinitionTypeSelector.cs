using Enflow;
using Enflow.Engine;
using Reference_Enflow_Builder.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace Reference_Enflow_Builder
{
    public class DefinitionTypeSelector : OrganizedTypeModelFactory<DefinitionTypeSelector, ReplaceDefinitionType> {
        public TypeProcessor? Processor {
            get => TypeProcessor.Processors[Type];
        }
        public DefinitionTypeSelector(Type type) : base(type) { }
        public DefinitionTypeSelector() : base(typeof(TypeProcessor), TypeProcessor.Processors.Keys.ToList(), false) { }
    }
    public class ReplaceDefinitionType : ElementCommand<Type> {
        public ReplaceDefinitionType(Type item) : base(item) {
        }
        public override void Execute(object? parameter) {
            if (Target is Button button) {
                _ = button.DataContext;
                if (button.DataContext is KeyValuePair<string, Data> context) {
                    Dispatch = () => {
                        context.Value.Format = TypeProcessor.NameFromProcessorType(Item.EnflowName());
                    };
                }
            }
            base.Execute(parameter);
        }
    }

}
