# UI Columns (Current – Risk removed)

Case                -> Property: CaseNumber        (string)    // unique id
SAP                 -> Property: SAP               (string)    // service/application path
Title               -> Property: Title             (string)    // short summary
Engage Now          -> Property: EngageNow         (bool)      // TRUE/FALSE
Customer            -> Property: Customer          (string)
Severity            -> Property: Severity          (string)    // A/B/C
Days Open           -> Property: DaysOpen          (int)
Reason              -> Property: Reason            (string)

Last Eng Update (UTC)       -> Property: LastEngUpdateUtc   (DateTimeOffset?)
Last Dissatisfaction (UTC)  -> Property: LastDissatUtc      (DateTimeOffset?)
Last Contact (UTC)          -> Property: LastContactUtc     (DateTimeOffset?)

Summary             -> Property: Summary           (string)
Link                -> Property: Link              (string)    // URL
