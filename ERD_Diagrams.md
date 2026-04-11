# Hospital HRMS - Entity Relationship Diagrams (ERD)

This document provides a professional ERD blueprint for the Hospital HRMS Project. The diagrams are designed systematically module-by-module. I have chosen a syntax perfectly compatible with **Eraser.io**, incorporating proper primary keys (PK), foreign keys (FK), and standardized English terminology for a highly professional outcome.

## 1. HR Personnel Module (HR_PERSONNEL)
This is the central module, focusing on the `Employee` entity and its interconnected personal, professional, and financial dimensions.

```eraser
// Entities Definition
Employee [icon: user] {
  EmployeeId int PK
  EmployeeNumber string
  FullNameEn string
  NationalId string
  Gender string
  HireDate date
  DepartmentId int FK
  JobId int FK
  ManagerId int FK
  NationalityId int FK
  UserId string FK
}

EmployeeDocument [icon: file-text] {
  DocumentId int PK
  EmployeeId int FK
  DocTypeId int FK
  DocNumber string
  ExpiryDate date
}

Contract [icon: file-text] {
  ContractId int PK
  EmployeeId int FK
  StartDate date
  EndDate date
  BasicSalary decimal
}

ContractRenewal [icon: repeat] {
  RenewalId int PK
  ContractId int FK
  RenewalDate date
}

Dependent [icon: users] {
  DependentId int PK
  EmployeeId int FK
  Relationship string
  IsEligibleForInsurance bool
}

EmployeeQualification [icon: award] {
  QualificationId int PK
  EmployeeId int FK
  DegreeType string
  Major string
}

EmployeeExperience [icon: briefcase] {
  ExperienceId int PK
  EmployeeId int FK
  Company string
  JobTitle string
}

EmployeeCertification [icon: award] {
  CertificationId int PK
  EmployeeId int FK
  LicenseNumber string
  ExpiryDate date
}

EmployeeAddress [icon: map-pin] {
  AddressId int PK
  EmployeeId int FK
  CityId int FK
  AddressDetails string
}

EmergencyContact [icon: phone] {
  ContactId int PK
  EmployeeId int FK
  Name string
  Phone string
}

EmployeeBankAccount [icon: credit-card] {
  AccountId int PK
  EmployeeId int FK
  BankId int FK
  IBAN string
}

// Relationships
Employee 1 - * EmployeeDocument
Employee 1 - * Contract
Contract 1 - * ContractRenewal
Employee 1 - * Dependent
Employee 1 - * EmployeeQualification
Employee 1 - * EmployeeExperience
Employee 1 - * EmployeeCertification
Employee 1 - * EmployeeAddress
Employee 1 - * EmergencyContact
Employee 1 - * EmployeeBankAccount
Employee 1 - * Employee : manages
```

## 2. HR Core Module (HR_CORE)
This module acts as the backbone and lookup repository for the entire HRMS system.

```eraser
// Entities Definition
Country [icon: globe] {
  CountryId int PK
  CountryName string
}

City [icon: map] {
  CityId int PK
  CountryId int FK
  CityName string
}

Department [icon: grid] {
  DepartmentId int PK
  ParentId int FK
  DeptName string
}

Branch [icon: home] {
  BranchId int PK
  BranchName string
}

Job [icon: target] {
  JobId int PK
  JobTitle string
  JobGradeId int FK
}

JobGrade [icon: layers] {
  JobGradeId int PK
  GradeName string
  MinSalary decimal
  MaxSalary decimal
}

DocumentType [icon: file] {
  DocTypeId int PK
  DocTypeName string
}

Bank [icon: archive] {
  BankId int PK
  BankName string
}

SystemSetting [icon: settings] {
  SettingId int PK
  SettingKey string
  SettingValue string
}

AuditLog [icon: list] {
  LogId int PK
  UserId string
  Action string
  Timestamp datetime
}

// Relationships
Country 1 - * City
Department 1 - * Department : sub-department
JobGrade 1 - * Job
```

## 3. HR Attendance Module (HR_ATTENDANCE)
This module manages employee shifts, punch records, daily attendance tracking, and requests.

```eraser
// Entities Definition
ShiftType [icon: clock] {
  ShiftId int PK
  ShiftName string
  StartTime time
  EndTime time
  HoursCount decimal
}

RosterPeriod [icon: calendar] {
  PeriodId int PK
  Year int
  Month int
  IsLocked bool
}

EmployeeRoster [icon: calendar] {
  RosterId int PK
  EmployeeId int FK
  ShiftId int FK
  RosterDate date
  IsOffDay bool
}

RawPunchLog [icon: hash] {
  LogId int PK
  EmployeeId int FK
  DeviceId string
  PunchTime datetime
  PunchType string
}

DailyAttendance [icon: check-square] {
  AttendanceId int PK
  EmployeeId int FK
  AttendanceDate date
  ActualInTime datetime
  ActualOutTime datetime
  Status string
}

ShiftSwapRequest [icon: repeat] {
  RequestId int PK
  RequesterId int FK
  TargetEmployeeId int FK
  ShiftId int FK
  Status string
}

OvertimeRequest [icon: plus-circle] {
  RequestId int PK
  EmployeeId int FK
  AttendanceDate date
  HoursRequested decimal
  ApprovedHours decimal
  Status string
}

AttendancePolicy [icon: file-text] {
  PolicyId int PK
  DepartmentId int FK
  AllowedLateMinutes int
}

// Relationships
ShiftType 1 - * EmployeeRoster
Employee 1 - * EmployeeRoster
Employee 1 - * RawPunchLog
Employee 1 - * DailyAttendance
Employee 1 - * ShiftSwapRequest : requests
Employee 1 - * ShiftSwapRequest : targeted
Employee 1 - * OvertimeRequest
```

### Cross-Module Connections (The Big Picture)
*   **EmployeeRoster (Attendance)** links to `Employee` (Personnel) and `ShiftType` (Attendance).
*   **Employee (Personnel)** links to `Department` (Core), `Job` (Core), and `Country` (Core).
*   **EmployeeBankAccount (Personnel)** links to `Bank` (Core).
*   **EmployeeDocument (Personnel)** links to `DocumentType` (Core).
