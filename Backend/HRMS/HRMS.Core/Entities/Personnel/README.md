# 📦 Domain Entities - HR_PERSONNEL Schema

## ✅ الكيانات المنشأة

تم إنشاء جميع كيانات HR_PERSONNEL بنجاح مع التوثيق الكامل بالعربية.

---

## 📋 قائمة الكيانات

### **1. Employee** (الموظفين)
**المسار:** `HRMS.Core/Entities/Personnel/Employee.cs`  
**الجدول:** `HR_PERSONNEL.EMPLOYEES`

**الوصف:** الكيان الرئيسي في النظام، يمثل الموظف وجميع بياناته الأساسية.

**الحقول الرئيسية:**
- `EmployeeId` (int) - المعرف الفريد
- `EmployeeNumber` (string) - الرقم الوظيفي (Unique)
- `FullNameEn` (string) - الاسم الكامل بالإنجليزية
- `NationalId` (string) - رقم الهوية
- `NationalityId` (FK) - الجنسية
- `JobId` (FK) - الوظيفة
- `DeptId` (FK) - القسم
- `ManagerId` (FK) - المدير المباشر

**العلاقات:**
- `Documents` → الوثائق الرسمية
- `Contracts` → العقود
- `Dependents` → التابعين
- `Qualifications` → المؤهلات
- `Experiences` → الخبرات
- `Addresses` → العناوين
- `EmergencyContacts` → جهات الاتصال للطوارئ
- `BankAccounts` → الحسابات البنكية

---

### **2. EmployeeDocument** (وثائق الموظف)
**المسار:** `HRMS.Core/Entities/Personnel/EmployeeDocument.cs`  
**الجدول:** `HR_PERSONNEL.EMPLOYEE_DOCUMENTS`

**الوصف:** تخزين وثائق الموظف مثل الهوية، الجواز، الرخص.

**الحقول الرئيسية:**
- `DocTypeId` (FK) - نوع الوثيقة
- `DocNumber` (string) - رقم الوثيقة
- `ExpiryDate` (DateTime) - تاريخ الانتهاء
- `AttachmentPath` (string) - رابط الملف المرفق

---

### **3. Contract** (العقود)
**المسار:** `HRMS.Core/Entities/Personnel/Contract.cs`  
**الجدول:** `HR_PERSONNEL.CONTRACTS`

**الوصف:** تفاصيل العقد الوظيفي والراتب الأساسي والبدلات.

**الحقول الرئيسية:**
- `StartDate`, `EndDate` - فترة العقد
- `BasicSalary` (decimal) - الراتب الأساسي
- `HousingAllowance`, `TransportAllowance` - البدلات
- `IsRenewable` (byte) - قابلية التجديد

**العلاقات:**
- `Renewals` → سجل تجديدات العقد

---

### **4. ContractRenewal** (تجديدات العقود)
**المسار:** `HRMS.Core/Entities/Personnel/ContractRenewal.cs`  
**الجدول:** `HR_PERSONNEL.CONTRACT_RENEWALS`

**الوصف:** أرشيف لتجديدات العقود السابقة.

---

### **5. Dependent** (التابعين)
**المسار:** `HRMS.Core/Entities/Personnel/Dependent.cs`  
**الجدول:** `HR_PERSONNEL.DEPENDENTS`

**الوصف:** أفراد عائلة الموظف (المرافقين).

**الحقول الرئيسية:**
- `NameAr` - الاسم
- `Relationship` - العلاقة
- `IsEligibleForTicket` - استحقاق التذاكر
- `IsEligibleForInsurance` - استحقاق التأمين

---

### **6. EmployeeQualification** (المؤهلات)
**المسار:** `HRMS.Core/Entities/Personnel/EmployeeQualification.cs`  
**الجدول:** `HR_PERSONNEL.EMPLOYEE_QUALIFICATIONS`

**الوصف:** الدرجات العلمية والتخصصات.

**الحقول الرئيسية:**
- `DegreeType` - الدرجة (بكالوريوس، إلخ)
- `MajorAr` - التخصص
- `UniversityAr` - الجامعة
- `GraduationYear` - سنة التخرج

---

### **7. EmployeeExperience** (الخبرات)
**المسار:** `HRMS.Core/Entities/Personnel/EmployeeExperience.cs`  
**الجدول:** `HR_PERSONNEL.EMPLOYEE_EXPERIENCES`

**الوصف:** التاريخ الوظيفي السابق.

---

### **8. EmployeeCertification** (الشهادات المهنية)
**المسار:** `HRMS.Core/Entities/Personnel/EmployeeCertification.cs`  
**الجدول:** `HR_PERSONNEL.EMPLOYEE_CERTIFICATIONS`

**الوصف:** الشهادات الاحترافية والتراخيص الطبية (مثل الهيئة السعودية).

---

### **9. EmployeeAddress** (العناوين)
**المسار:** `HRMS.Core/Entities/Personnel/EmployeeAddress.cs`  
**الجدول:** `HR_PERSONNEL.EMPLOYEE_ADDRESSES`

**الوصف:** عناوين السكن (الحالي والدائم).

---

### **10. EmergencyContact** (جهات الطوارئ)
**المسار:** `HRMS.Core/Entities/Personnel/EmergencyContact.cs`  
**الجدول:** `HR_PERSONNEL.EMERGENCY_CONTACTS`

**الوصف:** أشخاص للتواصل معهم في الحالات الطارئة.

---

### **11. EmployeeBankAccount** (الحسابات البنكية)
**المسار:** `HRMS.Core/Entities/Personnel/EmployeeBankAccount.cs`  
**الجدول:** `HR_PERSONNEL.EMPLOYEE_BANK_ACCOUNTS`

**الوصف:** تفاصيل الحساب البنكي لتحويل الراتب.

**الحقول الرئيسية:**
- `BankId` (FK) - البنك
- `AccountNumber`, `IBAN` - أرقام الحساب
- `IsPrimary` - هل هو الحساب الرئيسي

---

## 📊 ملاحظات التصميم

1. **الشمولية**: تم تغطية جميع جداول المخطط `HR_PERSONNEL`.
2. **العلاقات**: تم ربط جميع الكيانات الفرعية بالموظف `Employee`.
3. **أنواع البيانات**: تم استخدام `decimal` للرواتب و `DateTime` للتواريخ.
4. **التحقق**: إضافة `[Required]` و `[MaxLength]` ورسائل خطأ بالعربية.
5. **التوثيق**: شرح كامل لكل كلاس وخاصية باستخدام XML Comments.
