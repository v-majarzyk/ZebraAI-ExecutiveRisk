// ViewModels/MainViewModel.cs
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq; 
using System.Windows.Input;
using ZebraAI_ExecutiveRisk.Models;
using ZebraAI_ExecutiveRisk.ViewModels;

namespace ZebraAI_ExecutiveRisk.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
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

            // 1. Build the API Filter String (Best Practice: Call dedicated method)
            string severityFilter = BuildSeverityFilter();
            
            // 2. Build the API Filter String (Service Offerings - Placeholder for next step)
            string offeringFilter = "ServiceOfferingLevelTwo eq 'Unified Support Base'"; // Hardcoded placeholder

            // 3. Combine the Filters (This is the string sent to the API/used for mock)
            string combinedFilter = $"({severityFilter}) AND ({offeringFilter})";

            // 4. MOCK DATA (Using the combined filter in the title for verification)
            string criteria = $"Filter: {combinedFilter}";

            CasesList.Add(new CaseRiskItem 
            { 
                CaseNumber = $"CS-001A", 
                Title = $"High Risk Escalation Candidate ({criteria})", 
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

        // Implements the filter: (CurrentSeverity eq '1' or CurrentSeverity eq 'A' or ...)
        private string BuildSeverityFilter()
        {
            const string FieldName = "CurrentSeverity";
            
            // 1. Get the list of selected severity values (e.g., ["1", "A"])
            var selectedSeverities = SeverityOptions.Where(s => s.IsSelected).Select(s => s.Value).ToList();

            if (!selectedSeverities.Any())
            {
                // Return a filter that results in no cases found if nothing is selected
                return $"{FieldName} eq 'NONE_SELECTED'";
            }

            // 2. Build the individual query clauses (e.g., "CurrentSeverity eq '1'")
            var clauses = selectedSeverities.Select(severity => 
                $"{FieldName} eq '{severity}'"
            );

            // 3. Combine the clauses with " or " and wrap in parentheses
            return string.Join(" or ", clauses);
        }
    }
}