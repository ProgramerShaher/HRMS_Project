# 🚀 دليل التثبيت الكامل - Oracle 23ai + APEX

## ✅ ما تم إنشاؤه

تم إنشاء **9 schemas** كاملة مع إعدادات APEX جاهزة:

1. **HR_CORE** - الأساس والإعدادات
2. **HR_PERSONNEL** - شؤون الموظفين
3. **HR_ATTENDANCE** - الحضور والانصراف
4. **HR_LEAVES** - الإجازات
5. **HR_PAYROLL** - الرواتب
6. **HR_RECRUITMENT** - التوظيف
7. **HR_PERFORMANCE** - الأداء
8. **HR_System_Admin** - إدارة النظام
9. **HR_System_PDB** - نظام قاعدة البيانات

---

## 📋 خطوات التنفيذ

### **1. إنشاء الـ Schemas (إلزامي)**

```bash
# من VS Code أو SQL Developer
# اتصل بـ SYS as SYSDBA على المنفذ 1522

sqlplus sys@localhost:1522/FREEPDB1 as sysdba
@G:\HRMS_Hospital\DB\Database\Oracle23ai\01_CREATE_SCHEMAS_23ai.sql
```

**النتيجة:** 9 schemas جاهزة مع صلاحيات متبادلة

---

### **2. إعداد APEX Workspaces (اختياري)**

```sql
@G:\HRMS_Hospital\DB\Database\Oracle23ai\05_SETUP_APEX_WORKSPACES.sql
```

**النتيجة:** 9 workspaces جاهزة للاستخدام

---

### **3. إنشاء الجداول**

استخدم السكربتات الموجودة في `Oracle_DDL` بعد تعديلها:

```sql
-- تأكد من تغيير XEPDB1 إلى FREEPDB1
@COMPLETE_INSTALL_PART1.sql  -- الجداول
@COMPLETE_INSTALL_PART2.sql  -- Foreign Keys
```

---

## 🔐 بيانات الاتصال

### **من VS Code (Oracle SQL Developer Extension):**

```json
{
  "host": "localhost",
  "port": 1522,
  "serviceName": "FREEPDB1",
  "username": "HR_CORE",
  "password": "Core@2026"
}
```

### **جميع كلمات المرور:**

| Schema | Password |
|:---|:---|
| HR_CORE | Core@2026 |
| HR_PERSONNEL | Personnel@2026 |
| HR_ATTENDANCE | Attend@2026 |
| HR_LEAVES | Leaves@2026 |
| HR_PAYROLL | Payroll@2026 |
| HR_RECRUITMENT | Recruit@2026 |
| HR_PERFORMANCE | Perform@2026 |
| HR_System_Admin | SysAdmin@2026 |
| HR_System_PDB | SysPDB@2026 |

---

## 🌐 الوصول إلى APEX

**URL:** `http://localhost:8080/ords`

**Workspaces:**
- HR_CORE → User: ADMIN | Pass: Admin@2026
- HR_PERSONNEL → User: HR_MANAGER | Pass: HRManager@2026
- وهكذا...

---

## 🎯 الخطوات التالية

1. ✅ الـ Schemas جاهزة
2. ⏳ إنشاء الجداول من السكربتات الموجودة
3. ⏳ إضافة JSON Duality Views (اختياري)
4. ⏳ بناء تطبيقات APEX

---

**كل شيء جاهز للبدء!** 🚀
