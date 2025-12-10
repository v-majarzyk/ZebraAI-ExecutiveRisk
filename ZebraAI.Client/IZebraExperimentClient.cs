namespace ZebraAI.Client;

public interface IZebraExperimentClient
{
    Task<IReadOnlyList<CaseRiskItem>> GetExecRiskCasesAsync(string sap, CancellationToken ct = default);
}
