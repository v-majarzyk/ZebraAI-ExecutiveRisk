using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Win32;
using ZebraAI.Client;
using ZebraAI.Desktop; // AppConfig

namespace ZebraAI.Desktop.ViewModels
{
    // Simple checkbox row for the SAP filter list
    public sealed class SapFilterItem : INotifyPropertyChanged
    {
        public string Name { get; }
        private bool _isSelected = true;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value) return;
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public SapFilterItem(string name) => Name = name;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? n = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }

    public sealed class MainViewModel : INotifyPropertyChanged
    {
        private readonly IZebraExperimentClient _client;
        private readonly string _configPath;
        private readonly AppConfig _config;

        public ObservableCollection<CaseRiskItem> Results { get; } = new();
        public ICollectionView ResultsView { get; }

        public ObservableCollection<SapFilterItem> SapItems { get; } = new();
        public ICollectionView SapItemsView { get; }

        private HashSet<string> _selectedSap = new(StringComparer.OrdinalIgnoreCase);

        private bool _isSapFilterOpen;
        public bool IsSapFilterOpen
        {
            get => _isSapFilterOpen;
            set
            {
                if (_isSapFilterOpen == value) return;
                _isSapFilterOpen = value;
                OnPropertyChanged();
            }
        }

        private string _sapFilterSearch = "";
        public string SapFilterSearch
        {
            get => _sapFilterSearch;
            set
            {
                if (_sapFilterSearch == value) return;
                _sapFilterSearch = value;
                // Narrows the left list only; selection state persists
                SapItemsView.Refresh();
                OnPropertyChanged();
                UpdateExportEnabled();
            }
        }

        private bool _showOnlyEngageNow;
        public bool ShowOnlyEngageNow
        {
            get => _showOnlyEngageNow;
            set
            {
                if (_showOnlyEngageNow == value) return;
                _showOnlyEngageNow = value;
                ResultsView.Refresh();
                OnPropertyChanged();
                UpdateExportEnabled();
            }
        }

        // Commands
        public ICommand RunCommand { get; }
        public ICommand ExportCsvCommand { get; }
        public ICommand SapSelectAllCmd { get; }
        public ICommand SapSelectNoneCmd { get; }

        public MainViewModel(IZebraExperimentClient client)
        {
            _client = client;

            _configPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            _config = AppConfig.Load(_configPath);

            ResultsView = CollectionViewSource.GetDefaultView(Results);
            ResultsView.Filter = ResultsPredicate;

            SapItemsView = CollectionViewSource.GetDefaultView(SapItems);
            SapItemsView.Filter = o =>
            {
                if (o is not SapFilterItem sfi) return false;
                if (string.IsNullOrWhiteSpace(SapFilterSearch)) return true;
                return sfi.Name.IndexOf(SapFilterSearch, StringComparison.OrdinalIgnoreCase) >= 0;
            };

            RunCommand       = new RelayCommand(async _ => await RunAsync(), _ => true);

            // Enable only when the CURRENT VIEW has any rows
            ExportCsvCommand = new RelayCommand(_ => ExportCsv(), _ => HasAnyVisibleRows());

            // These update the selection and immediately refresh results
            SapSelectAllCmd  = new RelayCommand(_ => { SetAllSap(true);  ApplySapFilter(); });
            SapSelectNoneCmd = new RelayCommand(_ => { SetAllSap(false); ApplySapFilter(); });

            _showOnlyEngageNow = _config.ShowOnlyEngageNow;

            // Keep the SAP list in sync when results change
            ((INotifyCollectionChanged)Results).CollectionChanged += (_, __) =>
            {
                RebuildSapList();
                UpdateExportEnabled();
            };
        }

        private bool ResultsPredicate(object? obj)
        {
            if (obj is not CaseRiskItem r) return false;

            if (_selectedSap.Count > 0 && !string.IsNullOrWhiteSpace(r.Sap))
            {
                if (!_selectedSap.Contains(r.Sap)) return false;
            }

            if (ShowOnlyEngageNow && !r.EngageNow) return false;

            return true;
        }

        // Live-apply when a checkbox is toggled
        private void HookSapItem(SapFilterItem item)
        {
            item.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(SapFilterItem.IsSelected))
                {
                    ApplySapFilter();
                }
            };
        }

        private void RebuildSapList()
        {
            var distinct = Results
                .Select(r => r.Sap ?? "")
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(s => s, StringComparer.OrdinalIgnoreCase)
                .ToList();

            // Preserve current selections where possible
            var remembered = SapItems.ToDictionary(s => s.Name, s => s.IsSelected, StringComparer.OrdinalIgnoreCase);

            SapItems.Clear();
            foreach (var s in distinct)
            {
                var sel = remembered.TryGetValue(s, out var on) ? on : true;
                var item = new SapFilterItem(s) { IsSelected = sel };
                HookSapItem(item);
                SapItems.Add(item);
            }

            _selectedSap = SapItems.Where(x => x.IsSelected)
                                   .Select(x => x.Name)
                                   .ToHashSet(StringComparer.OrdinalIgnoreCase);

            SapItemsView.Refresh();
            ResultsView.Refresh();
            UpdateExportEnabled();
        }

        private void SetAllSap(bool value)
        {
            foreach (var s in SapItems) s.IsSelected = value;
            // ApplySapFilter called by the command wrapper after bulk set
        }

        private void ApplySapFilter()
        {
            _selectedSap = SapItems.Where(x => x.IsSelected)
                                   .Select(x => x.Name)
                                   .ToHashSet(StringComparer.OrdinalIgnoreCase);
            ResultsView.Refresh();
            UpdateExportEnabled();
        }

        private async Task RunAsync()
        {
            Results.Clear();

            var saps = (_config.SapOptions?.Any() == true)
                ? _config.SapOptions
                : new System.Collections.Generic.List<string>
                {
                    "Compute/Windows/Kernel",
                    "Azure/Storage/Blobs",
                    "Office/Outlook/Mailflow",
                    "Security/Identity/Auth"
                };

            foreach (var sap in saps)
            {
                var items = await _client.GetExecRiskCasesAsync(sap);
                foreach (var it in items)
                {
                    it.Sap = sap; // ensure populated even if backend omits
                    Results.Add(it);
                }
            }

            RebuildSapList();
            UpdateExportEnabled();
        }

        private static string Esc(string s) => "\"" + (s?.Replace("\"", "\"\"") ?? "") + "\"";
        private static string Dt(DateTime? d) => d.HasValue ? d.Value.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture) : "";

        private void ExportCsv()
        {
            var dlg = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                FileName = $"zebra-results-{DateTime.UtcNow:yyyyMMdd-HHmm}.csv"
            };
            if (dlg.ShowDialog() != true) return;

            var sep = ",";
            var sb = new StringBuilder();

            // Header
            sb.AppendLine(string.Join(sep, new[]
            {
                "Case","SAP","Title","EngageNow","Customer","Severity","DaysOpen","Reason",
                "LastEngineerUpdateUtc","LastDissatisfactionUtc","LastContact","Summary","Link"
            }));

            // Export ONLY currently visible rows
            foreach (CaseRiskItem r in ResultsView.Cast<CaseRiskItem>())
            {
                var row = new[]
                {
                    Esc(r.CaseNumber),
                    Esc(r.Sap),
                    Esc(r.Title),
                    r.EngageNow ? "Yes" : "No",
                    Esc(r.Customer),
                    Esc(r.Severity),
                    r.DaysOpen.ToString(CultureInfo.InvariantCulture),
                    Esc(r.Reason),
                    Dt(r.LastEngineerUpdateUtc),
                    Dt(r.LastDissatisfactionUtc),
                    r.LastContact.ToString(CultureInfo.InvariantCulture),
                    Esc(r.Summary),
                    Esc(r.Link)
                };
                sb.AppendLine(string.Join(sep, row));
            }

            // UTF-8 with BOM for Excel-friendliness
            var utf8Bom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);
            File.WriteAllText(dlg.FileName, sb.ToString(), utf8Bom);
            System.Windows.MessageBox.Show($"Saved:\n{dlg.FileName}", "Export CSV",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }

        private bool HasAnyVisibleRows()
        {
            // Cheap and cheerful check against the filtered view
            return ResultsView.Cast<object>().Any();
        }

        private void UpdateExportEnabled()
        {
            if (ExportCsvCommand is RelayCommand rc)
                rc.RaiseCanExecuteChanged();
            else
                CommandManager.InvalidateRequerySuggested();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? n = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }
}
