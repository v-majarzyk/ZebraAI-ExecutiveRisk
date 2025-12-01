// ViewModels/MainViewModel.cs
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq; 
using System.Windows.Input;
using ZebraAI_ExecutiveRisk.Models;
using ZebraAI_ExecutiveRisk.ViewModels;

namespace ZebraAI_ExecutiveRisk
{
    // The MainViewModel must be within the namespace block.
    public class MainViewModel : BaseViewModel
    {
        // --- STATIC BASE FILTER CLAUSES ---
        // MOVED HERE: This array is now correctly defined inside the class.
        private static readonly string[] StaticRequiredFilters = new[]
        {
            "CaseState eq 'Open'",
            "CaseStatus ne 'Duplicate'",
            "CaseStatus ne 'Resolved'",
            "ServiceOfferingLevelOne eq 'Enterprise Support'"
        };
        
        // -------------------------------------------------------------
        // INPUT PROPERTIES
        // -------------------------------------------------------------

        // Service Offering Level 2 Selection (Uses SelectableItem for Multi-Select)
        public ObservableCollection<SelectableItem<string>> ServiceOfferingLevel2Options { get; } = new ObservableCollection<SelectableItem<string>>
        {
            new SelectableItem<string>("Unified", true), 
            new SelectableItem<string>("CSD")
        };

        // Severity Selection (Uses SelectableItem for Multi-Select)
        public ObservableCollection<SelectableItem<string>> SeverityOptions { get; } = new ObservableCollection<SelectableItem<string>>
        {
            new SelectableItem<string>("1", true), 
            new SelectableItem<string>("A", true), 
            new SelectableItem<string>("B"),
            new SelectableItem<string>("C")
        };
        
        // -------------------------------------------------------------
        // OUTPUT/COMMAND PROPERTIES
        // -------------------------------------------------------------
        
        public ObservableCollection<CaseRiskItem> CasesList { get; set; } = new ObservableCollection<CaseRiskItem>();
        public ICommand SearchCommand { get; }

        public MainViewModel()
        {
            SearchCommand = new RelayCommand(ExecuteSearch);
            ExecuteSearch(null); 
        }

        // --- EXECUTION WORKFLOW ---
        private void ExecuteSearch(object? parameter)
        {
            CasesList.Clear();

            // 1. Build the full filter string 
            string severityFilter = BuildSeverityFilter();
            string offeringFilter = BuildServiceOfferingFilter(); 
            string fullFilter = BuildFullQueryFilter(severityFilter, offeringFilter);
            
            // NOTE: The 'fullFilter' variable holds the final query string, 
            // ready to be sent to the API in the next development phase.

            // 2. MOCK DATA 
            CasesList.Add(new CaseRiskItem 
            { 
                CaseNumber = $"CS-001A", 
                Title = "High Risk Escalation Candidate (Verified)",
                Customer = "Alpha Corp", 
                Severity = SeverityOptions.Where(s => s.IsSelected).Select(s => s.Value).FirstOrDefault() ?? "N/A",
                DaysOpen = 12,
                Reason = "Recent Dissatisfaction",
                EngageNow = true,
                Sap = "Server/Deployment",
                Link = "https://mocklink/001A",
                LastDissatisfactionUtc = DateTime.UtcNow.AddDays(-3),
                Summary = "This case shows recent customer dissatisfaction and prolonged open time."
            });

            CasesList.Add(new CaseRiskItem 
            { 
                CaseNumber = $"CS-002B", 
                Title = "Standard Review Case",
                Customer = "Beta Solutions", 
                Severity = "B",
                DaysOpen = 4,
                Reason = "Engineer Update Only",
                EngageNow = false,
                Sap = "Client/Billing",
                Link = "https://mocklink/002B",
                LastDissatisfactionUtc = null,
                Summary = "Standard case with low risk, no recent negative indicators."
            });
        }
        // --- END EXECUTION WORKFLOW ---


        // --- DEDICATED FILTER BUILDER METHODS ---

        // 1. Builds the core severity filter segment.
        private string BuildSeverityFilter()
        {
            const string FieldName = "CurrentSeverity";
            
            var selectedSeverities = SeverityOptions.Where(s => s.IsSelected).Select(s => s.Value).ToList();

            if (!selectedSeverities.Any())
            {
                return $"{FieldName} eq 'NONE_SELECTED'";
            }

            var clauses = selectedSeverities.Select(severity => 
                $"{FieldName} eq '{severity}'"
            );

            return string.Join(" or ", clauses);
        }

        // 2. Builds the core service offering filter segment.
        private string BuildServiceOfferingFilter()
        {
            const string FieldName = "ServiceOfferingLevelTwo";
            
            var selectedOfferings = ServiceOfferingLevel2Options.Where(s => s.IsSelected).Select(s => s.Value).ToList();

            if (!selectedOfferings.Any())
            {
                return $"{FieldName} eq 'NONE_SELECTED'";
            }

            var clauses = selectedOfferings.Select(offering => 
                $"{FieldName} eq '{offering}'"
            );

            return string.Join(" or ", clauses);
        }

        // 3. Combines all static and dynamic filters into one string.
        private string BuildFullQueryFilter(string severityFilter, string offeringFilter)
        {
            var filterParts = new List<string>(StaticRequiredFilters);

            // Add dynamic filters, wrapping the OR clauses in parentheses
            if (!string.IsNullOrEmpty(severityFilter))
            {
                filterParts.Add($"({severityFilter})");
            }
            if (!string.IsNullOrEmpty(offeringFilter))
            {
                filterParts.Add($"({offeringFilter})");
            }

            // Join all parts with the mandatory ' and ' operator
            return string.Join(" and ", filterParts);
        }
    }
}