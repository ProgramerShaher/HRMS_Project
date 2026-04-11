# Hospital HRMS - Use Case Diagrams (Mermaid Live Editor)

This document contains standard **Mermaid.js** syntax for the Use Case flowcharts. The text is strictly in English, using `flowchart TB` layout which natively renders perfectly in **Mermaid Live Editor** without any text-to-shape distortion.

---

## 1. HR Personnel Module

```mermaid
---
config:
  layout: elk
---
flowchart TB
    %% Actors
    Emp["Employee"]
    Mgr["Manager"]
    HR["HR Admin"]

    %% System Boundary
    subgraph ps [HR Personnel Module]
        direction TB
        UC1(("View Profile"))
        UC2(("Update Personal Info"))
        UC3(("Manage Dependents"))
        UC4(("Upload Documents"))
        UC5(("Add Qualifications"))
        UC6(("Manage Contracts"))
        UC7(("Approve Updates"))
    end

    %% Relationships
    Emp --> UC1
    Emp --> UC3
    Emp --> UC4
    Emp --> UC5

    Mgr --> UC1
    Mgr --> UC7

    HR --> UC1
    HR --> UC2
    HR --> UC6
    HR --> UC7
```

---

## 2. HR Core / Administration Module

```mermaid
---
config:
  layout: elk
---
flowchart TB
    %% Actors
    Admin["System Admin"]
    HRMgr["HR Manager"]

    %% System Boundary
    subgraph cs [HR Core Module]
        direction TB
        UC1(("Manage System Settings"))
        UC2(("Configure Branches & Depts"))
        UC3(("Manage Jobs & Grades"))
        UC4(("Manage Document Types"))
        UC5(("Review Audit Logs"))
        UC6(("Configure Workflows"))
    end

    %% Relationships
    Admin --> UC1
    Admin --> UC5
    Admin --> UC6

    HRMgr --> UC2
    HRMgr --> UC3
    HRMgr --> UC4
```

---

## 3. HR Attendance Module

```mermaid
---
config:
  layout: elk
---
flowchart TB
    %% Actors
    Emp["Employee"]
    Mgr["Direct Manager"]
    HRCtrl["HR Controller"]

    %% System Boundary
    subgraph ats [HR Attendance Module]
        direction TB
        UC1(("Record Punch In/Out"))
        UC2(("View Daily Attendance"))
        UC3(("Request Shift Swap"))
        UC4(("Request Overtime"))
        UC5(("Approve Requests"))
        UC6(("Manage Employee Roster"))
        UC7(("Configure Policies"))
    end

    %% Relationships
    Emp --> UC1
    Emp --> UC2
    Emp --> UC3
    Emp --> UC4

    Mgr --> UC2
    Mgr --> UC5

    HRCtrl --> UC2
    HRCtrl --> UC6
    HRCtrl --> UC7
```
