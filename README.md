# ⚡ Zebra AI Executive Risk Dashboard

## 🎯 Project Goal
This application is a native Windows Presentation Foundation (WPF) tool designed to streamline and automate the manual daily process of identifying high-risk support cases requiring executive attention.

The goal of this Proof-of-Concept (POC) is to replace manual interaction with the Zebra AI web UI and subsequent data processing with a single, user-friendly desktop application.

## 💻 Technology Stack (Checkpoint 1)
* **Framework:** .NET 8
* **Language:** C#
* **Architecture:** Model-View-ViewModel (MVVM)
* **User Interface:** Windows Presentation Foundation (WPF)

## ✨ Key Features (POC Status)
* **Native Windows Interface:** Provides a clean, stable, and responsive graphical user interface (GUI).
* **Dynamic Query Building:** Allows users to select multiple criteria (Severity, Service Offerings) via multi-select listboxes.
* **Filter Generation:** Automatically translates user selections into a complete, syntactically correct JSON `Filter` string, ready for the downstream Zebra AI API call.
* **Data Grid Display:** Displays results in a structured `DataGrid` (currently populated with mock data).

## 🚀 Getting Started (To Run Locally)

### Prerequisites
1.  .NET 8 SDK (or later) installed.
2.  Visual Studio Code (or Visual Studio IDE).

### Execution Steps
1.  Clone the repository:
    ```bash
    git clone [https://github.com/v-majarzyk/ZebraAI-ExecutiveRisk.git](https://github.com/v-majarzyk/ZebraAI-ExecutiveRisk.git)
    cd ZebraAI-ExecutiveRisk
    ```
2.  Run the application from the terminal:
    ```bash
    dotnet run
    ```
    *(Note: This launches the application with mock data, demonstrating the filter building logic.)*

## 🛠️ Next Development Steps
The current stage (Checkpoint 1) is complete. The next phases involve **API Integration**:
1.  **API Handler:** Implement C# `HttpClient` to communicate with the internal Zebra AI API.
2.  **Data Serialization:** Handle JSON responses from the API, deserializing the data into the `CaseRiskItem` model.
3.  **Real-Time Data Loading:** Replace mock data in the `ExecuteSearch` command with the real, parsed data.