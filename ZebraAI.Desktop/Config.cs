using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ZebraAI.Desktop;

public sealed class AppConfig
{
    public List<string> SapOptions { get; set; } = new();
    public bool ShowOnlyEngageNow { get; set; } = false;

    public static AppConfig Load(string? path = null)
    {
        path ??= Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        if (!File.Exists(path)) return new AppConfig();

        string json = File.ReadAllText(path);
        var cfg = JsonSerializer.Deserialize<AppConfig>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        return cfg ?? new AppConfig();
    }
}
