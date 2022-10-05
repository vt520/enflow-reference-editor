using Enflow;
using Enflow.Engine;
using Reference_Enflow_Builder.View;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
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
using static Enflow.Engine.Services;
using static Enflow.CSD_Extensions;
using Enflow.Supplied.Sums;
using System.Threading;
using System.Diagnostics;

namespace Reference_Enflow_Builder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private static Program? _Program = null;
        public static Program Program {
            get {
                if (_Program is null) Program = null!;
                return _Program!;
            }
            set {
                _Program = value;
                if (_Program is null) _Program = Library.Programs.Default;
                if(ProgramModel is not null) ProgramModel.Program = _Program ;
            }
        }
        public static ProgramModel ProgramModel {
            get;
            set;
        } = new ProgramModel { Program = MainWindow.Program }; 
        public MainWindow() {
            Stopwatch s = new();
            s.Start();
            Universe.InitializeDomain();
            Program = (Program)@"{""$type"":""Program"",""Caption"":""Program"",""Definitions"":{""Yes"":{""$type"":""Engine.Data"",""Format"":""Choice"",""Value"":""Yes""},""No"":{""$type"":""Engine.Data"",""Format"":""Choice"",""Value"":""No""},""maybe"":{""$type"":""CSD.AddressBuilder"",""Location"":""Input:Address1"",""Detail"":""Input:Address2"",""City"":""Input:CSD City"",""State"":""Input:CSD State"",""Zip"":""Input:Zip""},""Expiration Date"":{""$type"":""Item"",""Format"":""Date""}},""Process"":{""$type"":""Qualifications.Income.Offset.CaliforniaAMI"",""Caption"":""Qualify using Area Median Income Data for California with Offset"",""Rejected"":{""$type"":""Outcomes.Reject"",""Caption"":""Rejected Item""},""Qualified"":{""$type"":""Filters.Dates.Before"",""Caption"":""Filters.Dates.Before"",""Yes"":{""$type"":""Outcomes.Accept"",""Caption"":""Accept Submission""},""Value"":""Definitions:Expiration Date""},""HouseholdMembers"":""Input:Household Members"",""HouseholdIncome"":""Input:Household Income"",""Address"":""Definitions:maybe"",""Multiplier"":""2""},""Application"":{""$type"":""Application"",""Fields"":{""First Name"":""Text"",""Middle Initial"":""CSD.Text.Text1"",""Last Name"":""Text"",""Mailing Address"":""Geographies.Address"",""E-Mail Address"":""CSD.Accounts.Email"",""Household Members"":""Numbers.Count"",""Household Income"":""CSD.Numbers.Currency"",""Service Address"":""Geographies.Address"",""Service Building Type"":""CSD.Codes.Building"",""Utility ID"":""CSD.Codes.Utility"",""Account Number"":""CSD.Accounts.Number"",""Bill Amount"":""CSD.Numbers.Currency"",""Delinquent"":""CSD.Confirmation"",""Home Ownership Status"":""CSD.Confirmation"",""LIHEAP Qualified"":""CSD.Confirmation"",""Rebate Received"":""CSD.Confirmation"",""3CE Enrollment"":""CSD.Confirmation"",""Active Reduced Rate Program"":""CSD.Confirmation"",""Fixed Income"":""CSD.Confirmation"",""Address1"":""CSD.Locations.Address1"",""Address2"":""CSD.Locations.Address2"",""CSD State"":""CSD.Locations.State"",""CSD City"":""CSD.Locations.City"",""Zip"":""Geographies.Zipcode""}}}";
            s.Stop();
            //Type foo = typeof(Enflow.Types.Class1);
            InitializeComponent();

            ProgramModel.OpTime = s.ElapsedMilliseconds;
            //string version = $"{Services.Version.Revision} {Services.VersionOf(this).Revision} (boot time {s.ElapsedMilliseconds}ms)";
            //Windows.ApplicationModel.Package.Current.Id.Version;
            DataContext = ProgramModel;
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


        private DependencyObject? FindChildNamed(DependencyObject parent, string name) {
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
                Type? entry_type = NewInputType.DataContext as Type;
                if (entry_type is null) return;
                string entry_name = NewFieldNameEntry.Text;

                model.Application.Fields.Add(entry_name, entry_type.EnflowName(true));
/*                string entryName = NewFieldNameEntry.Text;
                string? entryType = NewFieldTypeEntry.SelectedValue as string;
                model.Application.Fields.Add(entryName, entryType);*/
                ApplicationFieldsList.GetBindingExpression(ListBox.ItemsSourceProperty).UpdateTarget();
                ApplicationFieldsList.Items.Refresh();
                NewFieldNameEntry.Text = "";
            }

        }


        private void FieldNameEntry_TextChanged(object sender, TextChangedEventArgs e) {

        }

        private void FieldTypeEntry_SelectionChanged(object sender, SelectionChangedEventArgs e) {

        }

        private void RemoveEntry_Click(object sender, RoutedEventArgs e) {
            if (sender is Button button && button.Parent is Panel parent) {
                string? entryName = (parent.FindName("FieldNameEntry") as TextBlock)?.Text;
                ProgramModel.Application.Fields.Remove(entryName);
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
            if (e.OriginalSource is Button button && button.DataContext is KeyValuePair<string, Data> data_item) {
                ProgramModel.Definitions.Remove(data_item.Key);
            }

        }

        private void AddTableDefinition_Click(object sender, RoutedEventArgs e) {
            Enflow.Table table = new Enflow.Table {
                Format = "String",
                Values = { }
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
            Stopwatch s = new Stopwatch();
            s.Start();
            Dictionary<string, string?>? input_data  = ProgramModel.Input?.InputDictionary;
            Program program = ProgramModel.Program;
            ProcessResult? result = program.ProcessResult(input_data) as ProcessResult;
            ProcessResult.Text = (string)result;
            s.Stop();
            ProcessResultText.Text = (string)result;
            ProgramModel.OpTime = s.ElapsedMilliseconds;
        }

        private void AddDefinitionObject_Click(object sender, RoutedEventArgs e) {
            if(DefinitionTypeBtn.DataContext is Type selected_entry) {
                if(Activator.CreateInstance(selected_entry) is Data new_entry) {
                    IEnumerable source = DefinitionList.ItemsSource;
                    //DefinitionList.ItemsSource = null;
                    try {
                        if (new_entry.Format is null) {
                            new_entry.Format = "Empty";
                            new_entry.Value = "";
                        }

                        ProgramModel.Definitions.Add(DefinitionName.Text, new_entry);
                        DefinitionName.Text = "";
                    } catch (Exception evt) {

                    }

                    //DefinitionList.ItemsSource = source;
                    DefinitionList.GetBindingExpression(ListView.ItemsSourceProperty).UpdateTarget();
                }


            }

        }

        private void DefinitionName_TextChanged(object sender, TextChangedEventArgs e) {
            if(sender is TextBox textbox) {
                UpdateDefinitionAdderEnabled();
            }

        }

        private void UpdateDefinitionAdderEnabled() {
            bool enabled = (DefinitionName.Text != "") && (!ProgramModel.Definitions.ContainsKey(DefinitionName.Text));
            AddDefinitionObject.IsEnabled = enabled ;
        }
        private void DataTypeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if(sender is ComboBox combobox) {
                UpdateDefinitionAdderEnabled();
            }
        }

        private void ProcessOutcome_Click(object sender, RoutedEventArgs e) {
            try {
                ProcessResult result = (ProcessResult)ProcessResultText.Text;
                if (result.IsValid && result.IsSignatureValid()) {
                    ResultMachineState.Text = result.Complete();
                } else if(!result.IsValid) {
                    ResultMachineState.Text = "Object constructed in an invalid manner";
                } else {
                    if (result.IsSigned()) {
                        ResultMachineState.Text = "Validation Failed, object signature error";
                    } else {
                        ResultMachineState.Text = "A Valid Seal is Required For Processing";
                    }
                }
            } catch (Exception ex) {
                ResultMachineState.Text = ex.ToString();
            }
        }

        private void NewFieldNameEntry_TextChanged(object sender, TextChangedEventArgs e) {

            if (sender is TextBox textbox && textbox.DataContext is ProgramModel model) {

                string entryName = NewFieldNameEntry.Text;
                AddNewField.IsEnabled = NewFieldNameEntry.Text != String.Empty &&  !model.Application.Fields.ContainsKey(entryName);

            }
        }


        private void Title_Click(object sender, RoutedEventArgs e) {
            if(sender is Button button && button.ContextMenu is ContextMenu menu) {
                menu.DisplayFor(button);
            }
        }

        private void Replacement_LostFocus(object sender, RoutedEventArgs e) {
            
            if(sender is ComboBox combo) {
                if (FocusManager.GetFocusedElement(this) is ComboBoxItem item) {
             
                    if(ItemsControl.ItemsControlFromItemContainer(item) is ComboBox parent) {
                        if (parent == sender) return;
                    }
                }

                if (combo.Parent is Grid grid && grid.FindName("Title") is Button button) {
                    button.Visibility = Visibility.Visible;
                    combo.Visibility = Visibility.Hidden;
                }
            }
        }
        private void CompileSource_Click(object sender, RoutedEventArgs e) {
            Stopwatch s = new();
            s.Start();
            try {
                ProgramModel.Program = (Program)Source.Text;
                s.Stop();
            } catch (Exception ex) {
                Source.Text = ex.ToString();
            }
            ProgramModel.OpTime = s.ElapsedMilliseconds;
        }

        private void RefreshSource_Click(object sender, RoutedEventArgs e) {
            Stopwatch s = new();
            s.Start();
            string source = ProgramModel.Program;
            s.Stop();
            Source.Text = source;
            ProgramModel.OpTime = s.ElapsedMilliseconds;
        }

        private void CompleteResponse_Click(object sender, RoutedEventArgs e) {
            ProcessResultText.Text = ResultMachineState.Text;
            ProcessOutcome_Click(sender, e);
        }

        private void TypeSelector_Click(object sender, RoutedEventArgs e) {
            if(sender is FrameworkElement element) {
                
                if(element.ContextMenu is ContextMenu menu) {
                    menu.DisplayFor(element);
                }
            }
            //TypeSelector.PlacementTarget = sender as UIElement;
            //TypeSelector.IsOpen = true;
            /*button.ContextMenu.PlacementTarget = button;
button.ContextMenu.IsOpen = true;
return;
*/
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e) {
            if(sender is ContextMenu menu) {
                if(menu.PlacementTarget is FrameworkElement element) {
                    foreach(IEnumerable item in menu.Items.OfType<IEnumerable>() ) {
                        if(item is Model) UpdateCommandTargets(item, element);
                    }
                }
            }
        }
        private void UpdateCommandTargets(IEnumerable target, FrameworkElement element) {
            if (target is null) return;
            if(target is ICommandable commandable) {
                if(commandable.Command is ElementCommand<Type> command) {
                    command.Target = element;
                }
                foreach(object item in target) {
                    if(item is IEnumerable enumerable) UpdateCommandTargets(enumerable, element);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            if (sender is FrameworkElement element) {

                if (element.ContextMenu is ContextMenu menu) {
                    menu.DisplayFor(element);
                }
            }
        }

        private void DefinitionTypeBtn_Click(object sender, RoutedEventArgs e) {
            if (sender is Button button && button.ContextMenu is ContextMenu menu) {
                menu.DisplayFor(button);
            }
        }
    }
}
