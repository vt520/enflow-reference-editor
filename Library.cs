using Enflow;
using Enflow.CSD;
using Enflow.Decisions;
using Enflow.Engine;
using Enflow.Outcomes;
using Enflow.Supplied;
using Enflow.Types;
using Enflow.Types.CSD;
using Enflow.Types.CSD.Accounts;
using Enflow.Types.CSD.Codes;
using Enflow.Types.CSD.Numbers;
using Enflow.Types.CSD.Text;
using Enflow.Types.Geographies;
using Enflow.Types.Numbers;
using Reference_Enflow_Builder.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
{
  "$type": "Program",
  "Caption": "Program",
  "Definitions": {
    "Yes": {
      "$type": "Data",
      "Format": "Choice",
      "Value": "Yes"
    },
    "No": {
      "$type": "Data",
      "Format": "Choice",
      "Value": "No"
    }
  },
  "Qualifications": {
    "$type": "Qualifications.Income.Offset.CAAMIOffset",
    "Caption": "Qualify using Area Median Income Data for California with Offset",
    "Rejected": {
      "$type": "Outcomes.Reject",
      "Caption": "Rejected Item"
    },
    "Qualified": {
      "$type": "Outcomes.AcceptToST",
      "Caption": "Approve with ServTraq",
      "SomeProp": "Something"
    },
    "HouseholdMembers": "Input:Household Members",
    "HouseholdIncome": "Input:Household Income",
    "Address": "Input:Service Address",
    "Multiplier": "2"
  },
  "Application": {
    "$type": "Application",
    "Fields": {
      "First Name": "Text",
      "Middle Initial": "Text",
      "Last Name": "Text",
      "Mailing Address": "Geographies.Address",
      "E-Mail Address": "CSD.Accounts.Email",
      "Household Members": "Numbers.Count",
      "Household Income": "Numbers.Decimal",
      "Service Address": "Geographies.Address",
      "Service Building Type": "CSD.Codes.Building",
      "Utility ID": "CSD.Codes.Utility",
      "Account Number": "CSD.Accounts.Number",
      "Bill Amount": "Numbers.Decimal",
      "Delinquent": "Choice",
      "Home Ownership Status": "Choice",
      "LIHEAP Qualified": "Choice",
      "Rebate Received": "Choice",
      "3CE Enrollment": "Choice",
      "Active Reduced Rate Program": "Choice",
      "Fixed Income": "Choice"
    }
  }
}
 */

namespace Reference_Enflow_Builder {
    public class Library : Model {
        public static string Name(System.Type type) {
            return type.EnflowName(true);
        }
        public static Library Instances { get; } = new Library();

        private Process? _SelectedOutcome = null;
        public Process? ReplacementOutcome {
            get {
                return _SelectedOutcome;
            }
            set {
                _SelectedOutcome = value;
                OnPropertyChangedAsync();
            }
        }
        public List<TypeProcessor> TypeProcessors {
            get => TypeProcessor.Processors.Values.ToList();
        }
        
        public static dynamic ProgramFragments = new {
            Application = new {
                Fields = new Enflow.Engine.FieldCollection {
                    {"First Name", Name(typeof(Text)) },
                    {"Middle Initial", Name(typeof(Text1)) },
                    {"Last Name", Name(typeof(Text)) },
                    {"Mailing Address", Name(typeof(Address))},
                    {"E-Mail Address", Name(typeof(Email))},

                    {"Household Members", Name(typeof(Count))},
                    {"Household Income", Name(typeof(Currency))},

                    {"Service Address", Name(typeof(Address)) },
                    {"Service Building Type", Name(typeof(Building))},
                    
                    {"Utility ID", Name(typeof(Utility))},
                    {"Account Number", Name(typeof(Enflow.Types.CSD.Accounts.Number)) },
                    {"Bill Amount", Name(typeof(Currency)) },
                    {"Delinquent", Name(typeof(Confirmation)) },
                    {"Home Ownership Status", Name(typeof(Confirmation))},

                    {"LIHEAP Qualified", Name(typeof(Confirmation))},
                    {"Rebate Received", Name(typeof(Confirmation))},
                    {"3CE Enrollment", Name(typeof(Confirmation))},
                    {"Active Reduced Rate Program", Name(typeof(Confirmation))},
                    {"Fixed Income", Name(typeof(Confirmation)) },
                }
            }
        };
        public static dynamic Programs { get; } = new {
            Empty = new Enflow.Program {
                Application = {
                    
                },
                Definitions = {

                },
                Process = new Reject()
            },
            Default  = new Enflow.Program {
                Application = {
                    Fields = ProgramFragments.Application.Fields
                },
                Definitions = {
                    {"Yes", new Data("Yes", "Choice") },
                    {"No", new Data("No", "Choice") },
                    {"maybe", new AddressBuilder() }
                },
                Process = new Enflow.Qualifications.Income.Offset.CaliforniaAMI {
                    Multiplier = "2",
                    Address = "Input:Service Address",
                    HouseholdIncome = "Input:Household Income",
                    HouseholdMembers = "Input:Household Members",
                    Rejected = new Enflow.Outcomes.Reject { Caption = "Rejected Item" },
                    Qualified = new Enflow.Outcomes.AcceptToST { Caption = "Approve with ServTraq" }
                }
            },
        };
        public Process DefaultOutcome { get => new GreaterThan(); }
        public static ProgramModel DefaultProgramModel { get; } = new ProgramModel { Program = Programs.Default.Clone() };
        public static dynamic Defaults { get; } = new {
            ProgramModel = new ProgramModel { Program = Programs.Default }
        };
        public static PropertyIndexer<string> DefaultOptions { get => Programs.Default.Qualifications.Options; }
    }
}
