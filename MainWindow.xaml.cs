using Enflow;
using Enflow.Engine;
using Reference_Enflow_Builder.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Reference_Enflow_Builder {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window , INotifyPropertyChanged {
        public ProgramModel ProgramModel { get; set; } = new ProgramModel { 
            Program = Library.Programs.Default.Clone()
        };
        public MainWindow() {
            InitializeComponent();

            DataContext = ProgramModel;

        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Button.IsEnabled = false;
            if(ReplacementSelector.SelectedItem is Type type) {
                if(OutcomeTree.SelectedItem is Outcome outcome) {
                    if(!outcome.CompatibleOutcomes.Contains(type)) {
                        MessageBoxResult result = MessageBox.Show("Would you like to proceed?", "Outcome not fully compatible", MessageBoxButton.YesNo);
                        if (result == MessageBoxResult.No) return;
                    }
                    Outcome? replaceent = Activator.CreateInstance(type) as Outcome;

                    string path = OutcomeTree.SelectedValuePath;
                    if (replaceent is Outcome) {
                        Library.Instances.ReplacementOutcome = replaceent;
                        outcome.ReplaceWith(replaceent);
                        if (replaceent.IsRoot) {
                            OutcomeTree.GetBindingExpression(TreeViewItem.ItemsSourceProperty).UpdateTarget();
                            Source.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                        }
                    }
                    ReplacementSelector.SelectedItem = outcome.GetType();
                }
            }

        }

        private void ReplacementSelector_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            Button.IsEnabled = ReplacementSelector.SelectedItem is not null;
        }

        private void TextBox_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e) {

        }

        private TreeViewItem? FindChildItem(ItemsControl search_in, object search_for) {
            if (search_in is null) return null;
            if (search_for is null) return null;
            if (search_in.DataContext == search_for) return search_in as TreeViewItem;
            if (search_in is TreeViewItem view) {
                view.IsExpanded = true;

            }


            search_in.ApplyTemplate();
            TreeViewItem? subtree = null;
            for (int i = 0; i < search_in.Items.Count; i++) {
                subtree = (TreeViewItem)search_in.ItemContainerGenerator.ContainerFromIndex(i);
                if (subtree is not null) {
                    TreeViewItem? search = FindChildItem(subtree, search_for);
                    if(search is not null) return search;
                }
            }
            return null;
        }

        private void OutcomeTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
            if(e.NewValue == Library.Instances.ReplacementOutcome) {
                e.Handled = true;
                Library.Instances.ReplacementOutcome = null;
            }
            if(Library.Instances.ReplacementOutcome is not null) {
                TreeViewItem? new_Selection = FindChildItem(OutcomeTree, Library.Instances.ReplacementOutcome);
                if(new_Selection is not null) {
                    e.Handled = true;
                    new_Selection.IsSelected = true;
                }
            }
            if(e.NewValue is null) {

                SelectRootElement();
            }
        }

        private void SelectRootElement() {
            if (FindChildItem(OutcomeTree, ProgramModel.Qualifications) is TreeViewItem node) node.IsSelected = true;
        }
        private void OutcomeTree_Loaded(object sender, RoutedEventArgs e) {
            SelectRootElement();
        }


        private DependencyObject FindChildNamed(DependencyObject parent, string name) {
            if (parent is FrameworkElement element && element.Name == name) return parent; 
            for(int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++) {
                if(VisualTreeHelper.GetChild(parent, i) is FrameworkElement child) {
                    if (child.Name == name) return child;
                }
            }

            return null;
        }
        private void RemoveFromList_Click(object sender, RoutedEventArgs e) {
            if(e.Source is Button button) {
                _ = RelativeSource.TemplatedParent;
                if(VisualTreeHelper.GetParent(button) is FrameworkElement parent) {
                    if (FindChildNamed(parent, "TransientBox") is TextBox textBox) {
                        if(VisualTreeHelper.GetParent(parent) is FrameworkElement grandparent) {
                            if (FindChildNamed(grandparent, "TableEntriesList") is ListBox listBox) {
                                if (listBox.SelectedValue is string entry_Text) {
                                    if (button.DataContext is KeyValuePair<string, Data> index) {
                                        if (index.Value is Enflow.Table table) {
                                            table.RemoveEntry(entry_Text);
                                        }
                                    }

                                    textBox.Clear();
                                    textBox.SelectedText = entry_Text;
                                    textBox.Focus();
                                }
                            }
                        }
                        
                    }
                }
            }
        }

        private bool flagTextChanged = false;

        public event PropertyChangedEventHandler? PropertyChanged;

        private void TransientBox_TextChanged(object sender, TextChangedEventArgs e) {
            if (sender is TextBox text) {
                if (flagTextChanged == false) {
                    flagTextChanged = true;
                    if (BindingOperations.GetBinding(text, TextBox.TextProperty) is Binding binding) {
                        text.ClearValue(TextBox.TextProperty);
                        BindingOperations.SetBinding(text, TextBox.TextProperty, binding);
                    }
                }
            } else {
                flagTextChanged = false;
            }
        }

        private void DataGrid_AddingNewItem(object sender, AddingNewItemEventArgs e) {

        }

        private void DataGrid_Loaded(object sender, RoutedEventArgs e) {
            if(sender is DataGrid datagrid) {
                for(int i = 2; i < datagrid.Columns.Count; i++) {
                    datagrid.Columns[i].Visibility = Visibility.Collapsed;
                }
            }
        }
        private void AddToList_Click(object sender, RoutedEventArgs e) {
            if(sender is Button button && button.DataContext is ProgramModel model) {
                
                string entryName = NewFieldNameEntry.Text;
                string entryType = NewFieldTypeEntry.SelectedValue as string;
                model.Application.Fields.Add(entryName, entryType);
                ApplicationFieldsList.GetBindingExpression(ListBox.ItemsSourceProperty).UpdateTarget();
                ApplicationFieldsList.Items.Refresh();
            }

        }


        private void FieldNameEntry_TextChanged(object sender, TextChangedEventArgs e) {

        }

        private void FieldTypeEntry_SelectionChanged(object sender, SelectionChangedEventArgs e) {

        }

        private void RemoveEntry_Click(object sender, RoutedEventArgs e) {
            if (sender is Button button && button.DataContext is ProgramModel model) {

                string entryName = NewFieldNameEntry.Text;
                model.Application.Fields.Remove(entryName);
                ApplicationFieldsList.GetBindingExpression(ListBox.ItemsSourceProperty).UpdateTarget();
                ApplicationFieldsList.Items.Refresh();
            }
        }

        private void DefinitionList_SelectionChanged(object sender, SelectionChangedEventArgs e) {

        }

        private void RemoveItemDefinition_Click(object sender, RoutedEventArgs e) {
            if(e.OriginalSource is Button button && button.DataContext is KeyValuePair<string, Data> data_item) {
                ProgramModel.Definitions.Remove(data_item.Key);
            }
        }

        private void RemoveTableDefinition_Click(object sender, RoutedEventArgs e) {
            if (e.OriginalSource is Button button && button.DataContext is KeyValuePair<string, Enflow.Data> data_item) {
                ProgramModel.Definitions.Remove(data_item.Key);
            }

        }

        private void AddTableDefinition_Click(object sender, RoutedEventArgs e) {
            Enflow.Table table = new Enflow.Table {
                Format = "String",
                Entries = { }
            };
            ProgramModel.Definitions.Add(DefinitionName.Text, table);
            DefinitionList.GetBindingExpression(ListView.ItemsSourceProperty).UpdateTarget();
        }

        private void AddItemDefinition_Click(object sender, RoutedEventArgs e) {
            Data newData = new Data { Format = "String" };

            ProgramModel.Definitions.Add(DefinitionName.Text, newData) ;
            DefinitionList.GetBindingExpression(ListView.ItemsSourceProperty).UpdateTarget();
        }

        private void OptionValueSelector_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (e.OriginalSource is ComboBox combobox && combobox.DataContext is SuggestableIndexerItemView<string> item) {
                item.Value = (string)combobox.SelectedValue;

                if(combobox.Parent is Panel parent && parent.DataContext is not null) {

                }
            }

        }

        private void ResetInput_Click(object sender, RoutedEventArgs e) {
            ProgramModel.Input = null;
        }

        private void TestInput_Click(object sender, RoutedEventArgs e) {
            Dictionary<string, string> input_data  = ProgramModel.Input.InputDictionary;
            Result result = ProgramModel.Program.Process(input_data) as Result;
            ProcessResult.Text = (string)result;
        }
    }
}
