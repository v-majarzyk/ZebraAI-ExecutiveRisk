using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Controls;

namespace ZebraAI.Desktop.Services
{
    public static class ColumnLayoutService
    {
        private class ColumnState
        {
            public string Header { get; set; } = "";
            public int DisplayIndex { get; set; }
            public double? Width { get; set; }
        }

        private class Store
        {
            public Dictionary<string, List<ColumnState>> Grids { get; set; } = new();
        }

        private static readonly string AppDir =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ZebraAI");
        private static readonly string StorePath = Path.Combine(AppDir, "columnLayout.json");

        public static void Save(DataGrid grid, string key)
        {
            try
            {
                Directory.CreateDirectory(AppDir);
                var store = Load();

                store.Grids[key] = grid.Columns
                    .OrderBy(c => c.DisplayIndex)
                    .Select(c => new ColumnState
                    {
                        Header = c.Header?.ToString() ?? "",
                        DisplayIndex = c.DisplayIndex,
                        Width = c.ActualWidth > 0 ? c.ActualWidth : null
                    })
                    .ToList();

                File.WriteAllText(StorePath, JsonSerializer.Serialize(store, new JsonSerializerOptions { WriteIndented = true }));
            }
            catch { /* never break UI on persistence */ }
        }

        public static void Restore(DataGrid grid, string key)
        {
            try
            {
                var store = Load();
                if (!store.Grids.TryGetValue(key, out var cols) || cols.Count == 0) return;

                var byHeader = grid.Columns.ToDictionary(c => c.Header?.ToString() ?? "", c => c);

                foreach (var s in cols)
                    if (byHeader.TryGetValue(s.Header, out var col))
                        col.DisplayIndex = s.DisplayIndex;

                foreach (var s in cols)
                    if (byHeader.TryGetValue(s.Header, out var col) && s.Width is > 0)
                        col.Width = s.Width.Value;
            }
            catch { /* ignore bad/old layouts */ }
        }

        private static Store Load()
        {
            try
            {
                if (File.Exists(StorePath))
                {
                    var json = File.ReadAllText(StorePath);
                    var s = JsonSerializer.Deserialize<Store>(json);
                    if (s != null) return s;
                }
            }
            catch { /* ignore parse errors */ }
            return new Store();
        }
    }
}
