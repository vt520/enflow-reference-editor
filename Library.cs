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
        public static dynamic Programs { get; } = new {
            Empty = new Program {
                Application = {
                    Fields = {{"name", "String"}}
                },
                Definitions = {
                    {"approved applicants", new Table {
                        Format = "String",
                        Values = { }
                    }}
                }
            },
            Default  = (Program)
                "{\"$type\":\"Program\",\"Title\":\"Program\",\"Definitions\":{},\"Qualifications\":{\"$type\":\"Outcomes.Accept\",\"Title\":\"Accept Submission\"},\"Application\":{\"$type\":\"Application\",\"Fields\":{\"Service Address\":\"Address\",\"Medical Device\":\"Choice\",\"RRP\":\"Choice\",\"Utility\":\"Text\",\"Residence Address\":\"Address\",\"Bill Amount\":\"Number\",\"Bill Type\":\"Text\",\"Service Type\":\"Text\",\"Dwelling\":\"Text\",\"Electric Vehicle\":\"Choice\",\"3CE Enrollment\":\"Choice\",\"Water System\":\"Text\",\"Household Members\":\"Integer\",\"Household Income\":\"Number\",\"Rebate Recived\":\"Choice\",\"LIHEAP Qualified\":\"Choice\",\"Home Ownership Status\":\"Text\",\"Age\":\"Integer\",\"Fixed Income\":\"Choice\"}}}",
            Crap = new Program {
               Application = {
                    Fields = {
                        { "Full Name", "String" },
                        { "Address", "Address" },
                        { "Household Income", "Float" },
                        { "Household Members", "Integer" }
                    }
                },

                Definitions = {
                    { "Fibby", new Fibonacci() },
                    { "Maximum Income", new Data {
                        Value = "100000.00", Format = "Float"
                    }},
                    { "Minimum Income", new Data {
                        Value = "1.00", Format = "Float"
                    }},
                    { "Large Household Exemption", new Data {
                        Value = "6", Format = "Integer"
                    }},
                    { "Service Locations", new Table {
                        Format = "City",
                        Values = {
                            "San Diego, CA",
                            "Oakland, CA",
                            "Santa Cruz, CA",
                            "Watsonville, CA"
                        }
                    }}
                },

                Qualifications = new IsNotEmpty {
                    Title = "Ensure the Name Section is Completed",
                    From = $"{Program.Sections.Input}:Full Name",
                    Yes = new MatchesListItem {
                        Title = "Check if they are in the service area",
                        From = $"{Program.Sections.Input}:Address",
                        ComparedTo = $"{Program.Sections.Definition}:Service Locations",
                        Yes = new GreaterThan {
                            Title = "Test to see if they are under the maximum income",
                            From = $"{Program.Sections.Input}:Household Income",
                            ComparedTo = $"{Program.Sections.Definition}:Maximum Income",
                            Yes = new LessThan { // really GTEQ in Disguise
                                Title = "See if they are exempt because of # of residents",
                                From = $"{Program.Sections.Input}:Household Members",
                                ComparedTo = $"{Program.Sections.Definition}:Large Household Exemption",
                                Yes = new Reject(),
                                No = new Accept()
                            },
                            No = new GreaterThan {
                                Title = "Test if the application meets mimimum income requirements",
                                From = $"{Program.Sections.Input}:Household Income",
                                ComparedTo = $"{Program.Sections.Definition}:Minimum Income"
                            }
                        }
                    }
                }
            }
        };
        public Process DefaultOutcome { get => new GreaterThan(); }
        public static ProgramModel DefaultProgramModel { get; } = new ProgramModel { Program = Programs.Default.Clone() };
        public static dynamic Defaults { get; } = new {
            ProgramModel = new ProgramModel { Program = Programs.Default }
        };
        public static PropertyIndexer<string> DefaultOptions { get => Programs.Default.Qualifications.Options; }
    }
}
