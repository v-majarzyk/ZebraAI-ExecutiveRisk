namespace ZebraAI.Client;

public sealed class CaseRiskItem
{
    // Basic display
    public string CaseNumber { get; set; } = "";
    public string Title { get; set; } = "";
    public string Customer { get; set; } = "";
    public string Severity { get; set; } = "";
    public int    DaysOpen { get; set; }
    public string Reason { get; set; } = "";
    public string Link { get; set; } = "";

    // New: Support Area Path (for filtering/column)
    public string Sap { get; set; } = "";

    // Prompt-aware fields (mirrors your checklist/output)
    public bool   EngageNow { get; set; }               // Yes/No decision
    public DateTime? LastEngineerUpdateUtc { get; set; }
    public DateTime? LastDissatisfactionUtc { get; set; } // null = none in last 7 days
    public int LastContact { get; set; }             // Last Contact
    public string Summary { get; set; } = "";           // concise paragraph
}