using Enflow;
using Enflow.Engine;
using Reference_Enflow_Builder.View;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static Enflow.Engine.Services;

namespace Reference_Enflow_Builder {
    public class ProcessTypeSelector : OrganizedTypeModelFactory<ProcessTypeSelector, ProcessTypeSelectorCommand> {
        public ProcessTypeSelector() : base(typeof(Process), Services.Index.GetVisibleProcesses, false) {

        }
        public ProcessTypeSelector(Type type) : base(type){

        }
    }
    public class ProcessTypeSelectorCommand : ElementCommand<Type> {
       



        public ProcessTypeSelectorCommand(Type item) : base(item) {
            
        }
        public override void Execute(object? parameter) {
            if(Target is Button button && button.DataContext is Process current) {
                if(!current.CompatibleOutcomes.Contains(Item)) {
                    MessageBoxResult result = MessageBox.Show("Cannot copy child Outcomes to new object, Would you like to proceed?", "Data Loss Warning", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.No) return;
                }
                if (Activator.CreateInstance(Item!) is Process replacement_outcome) {
                    if(button.FindAncestor(typeof(TreeView)) is TreeView tree) {
                        var bef = tree.SelectedValuePath;
                        current.ReplaceWith(replacement_outcome);
                        var aft = tree.SelectedValuePath;
                        if (replacement_outcome.IsRoot) {
                            tree.GetBindingExpression(TreeViewItem.ItemsSourceProperty).UpdateTarget();
                        } else {
                            
                        }
                    }
                }
            }
            base.Execute(parameter);
        }
    }
}
