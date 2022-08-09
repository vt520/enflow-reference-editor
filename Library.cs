using Enflow;
using Enflow.Decisions;
using Enflow.Engine;
using Enflow.Outcomes;
using Enflow.Supplied;
using Reference_Enflow_Builder.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reference_Enflow_Builder {
    public class Library : Model {
        public static Library Instances { get; } = new Library();

        private Outcome? _SelectedOutcome = null;
        public Outcome? ReplacementOutcome {
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
        public static dynamic Programs { get; } = new {
            Default = new Program {
               Application = {
                    Fields = {
                        { "name", "String" },
                        { "address", "Address" },
                        { "income", "Float" },
                        { "residents", "Integer" }
                    }
                },

                Definitions = {
                    { "max_income", new Data {
                        Value = "100000.00", Format = "Float"
                    }},
                    { "min_income", new Data {
                        Value = "1.00", Format = "Float"
                    }},
                    { "large_household", new Data {
                        Value = "6", Format = "Integer"
                    }},
                    { "service_locations", new Table {
                        Format = "City",
                        Values = {
                            "San Diego, CA",
                            "Oakland, CA",
                            "Santa Cruz, CA",
                            "Watsonville, CA"
                        }
                    }}
                },

                Qualifications = new MatchesListItem {
                    From = $"{Program.Sections.Input}:address",
                    ComparedTo = $"{Program.Sections.Definition}:service_locations",
                    Yes = new GreaterThan {
                        From = $"{Program.Sections.Input}:income",
                        ComparedTo = $"{Program.Sections.Definition}:max_income",
                        Yes = new LessThan { // really GTEQ in Disguise
                            From = $"{Program.Sections.Input}:residents",
                            ComparedTo = $"{Program.Sections.Definition}:large_household",
                            Yes = new Reject(),
                            No = new Accept()
                        },
                        No = new GreaterThan {
                            From = $"{Program.Sections.Input}:income",
                            ComparedTo = $"{Program.Sections.Definition}:min_income",
                            Yes = new Accept(),
                            No = new Reject()
                        }
                    }
                }
            }
        };
        public Outcome DefaultOutcome { get => new GreaterThan(); }
        public static ProgramModel DefaultProgramModel { get; } = new ProgramModel { Program = Programs.Default.Clone() };
        public static dynamic Defaults { get; } = new {
            ProgramModel = new ProgramModel { Program = Programs.Default }
        };
        public static PropertyIndexer<string> DefaultOptions { get => Programs.Default.Qualifications.Options; }
    }
}
