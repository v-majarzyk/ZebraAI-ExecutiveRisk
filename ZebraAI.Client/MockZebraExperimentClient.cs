using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ZebraAI.Client
{
    // NOTE: Do NOT redeclare IZebraExperimentClient here — it's already defined elsewhere in this project.
    // We just implement it.

    public sealed class MockZebraExperimentClient : IZebraExperimentClient
    {
        private readonly Random _rng = new(42);

        // Match the existing interface signature (includes CancellationToken with default)
        public Task<IReadOnlyList<CaseRiskItem>> GetExecRiskCasesAsync(string sap, CancellationToken ct = default)
        {
            // Fixed number per SAP so you can stress-test the UI
            const int perSap = 8;

            var list = new List<CaseRiskItem>(perSap);
            for (int i = 0; i < perSap; i++)
            {
                ct.ThrowIfCancellationRequested();

                var id = $"{_rng.Next(10, 999)}-{_rng.Next(100000, 999999)}";
                var sev = Pick("A","B","C");
                var days = _rng.Next(1, 60);
                var risk = Math.Round(_rng.NextDouble(), 2);
                var engage = risk > 0.7 || (sev == "A" && days > 25);

                list.Add(new CaseRiskItem
                {
                    CaseNumber = id,
                    Title = $"Signal {_rng.Next(1,16)} detected",
                    Customer = $"Customer {_rng.Next(1,50)}",
                    Severity = sev,
                    DaysOpen = days,
                    Reason = Pick(
                        "Moderate indicators",
                        "Long silence + high severity",
                        "Volatile sentiment + SLA risk",
                        "Awaiting customer data (justified)",
                        "Reopens + aging + negative sentiment"),
                    Link = $"https://contoso/case/{id}",
                    Sap = sap,
                    EngageNow = engage,
                    LastEngineerUpdateUtc = DateTime.UtcNow.AddDays(-_rng.Next(0, 14)),
                    LastDissatisfactionUtc = _rng.NextDouble() < 0.4 ? DateTime.UtcNow.AddDays(-_rng.Next(1, 7)) : null,
                    LastContact = _rng.Next(1, 72),
                    Summary = "Concise summary of case signals and latest actions."
                });
            }

            return Task.FromResult<IReadOnlyList<CaseRiskItem>>(list);
        }

        private string Pick(params string[] options) => options[_rng.Next(options.Length)];
    }
}
