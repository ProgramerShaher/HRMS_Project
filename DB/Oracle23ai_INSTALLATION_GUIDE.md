# 🚀 دليل تثبيت قاعدة البيانات على Oracle 23ai Free

## ✅ الترتيب الصحيح للتنفيذ

### **المرحلة 0: إنشاء المستخدمين (Schemas) - ⚠️ إلزامي**
```sql
00_CREATE_SCHEMAS_Oracle23ai.sql
```
- يقوم بإنشاء **7 مستخدمين** (HR_CORE, HR_PERSONNEL, HR_ATTENDANCE, HR_LEAVES, HR_PAYROLL, HR_RECRUITMENT, HR_PERFORMANCE)
- ✅ **يجب تنفيذه أولاً قبل أي شيء**
- يجب تشغيله بصلاحية **SYSDBA**

---

### **المرحلة 1: إنشاء الجداول (بدون Foreign Keys)**
```sql
COMPLETE_INSTALL_PART1.sql
```
- يقوم بإنشاء جميع الـ **75 جدول** بدون علاقات
- ✅ **تم تعديله ليعمل مع Oracle 23ai (FREEPDB1)**

---

### **المرحلة 2: إضافة المفاتيح الأجنبية**
```sql
COMPLETE_INSTALL_PART2.sql
```
- يضيف جميع العلاقات (Foreign Keys) بين الجداول
- ✅ **تم تعديله ليعمل مع Oracle 23ai (FREEPDB1)**

---

### **المرحلة 3: منح الصلاحيات**
```sql
10_GRANT_PERMISSIONS.sql
```
- يعطي الصلاحيات للمستخدمين للوصول إلى الجداول

---

## 📋 كيفية التنفيذ

### **الطريقة 1: من SQL Developer**

1. افتح SQL Developer
2. اتصل بـ **SYS as SYSDBA** باستخدام:
   - **Username:** `SYS`
   - **Password:** كلمة المرور التي أدخلتها عند تثبيت Oracle
   - **Role:** `SYSDBA`
   - **Connection Type:** `Basic`
   - **Hostname:** `localhost`
   - **Port:** `1521`
   - **Service Name:** `FREEPDB1` ⚠️ **مهم جداً**

3. نفذ الملفات بالترتيب:
   - افتح `COMPLETE_INSTALL_PART1.sql`
   - اضغط **F5** (Run Script)
   - انتظر حتى ينتهي
   - افتح `COMPLETE_INSTALL_PART2.sql`
   - اضغط **F5**
   - افتح `10_GRANT_PERMISSIONS.sql`
   - اضغط **F5**

---

### **الطريقة 2: من Command Line**

افتح **Command Prompt** كـ Administrator وشغل:

```cmd
cd G:\HRMS_Hospital\DB\Database\Oracle_DDL

sqlplus sys/your_password@FREEPDB1 as sysdba @00_CREATE_SCHEMAS_Oracle23ai.sql

sqlplus sys/your_password@FREEPDB1 as sysdba @COMPLETE_INSTALL_PART1.sql

sqlplus sys/your_password@FREEPDB1 as sysdba @COMPLETE_INSTALL_PART2.sql

sqlplus sys/your_password@FREEPDB1 as sysdba @10_GRANT_PERMISSIONS.sql
```

استبدل `your_password` بكلمة مرور SYS الخاصة بك.

---

## ⚠️ الفرق بين Oracle 23ai و Oracle XE

| البند | Oracle 23ai Free | Oracle XE |
|:---|:---|:---|
| **اسم PDB** | `FREEPDB1` | `XEPDB1` |
| **اسم الخدمة** | `OracleServiceFREE` | `OracleServiceXE` |
| **Listener** | `OracleOraDB23ai_home1TNSListener` | `OracleOraDB21Home1TNSListener` |

---

## 🔧 إعدادات الاتصال الصحيحة

### **لـ Oracle 23ai:**
```
Connection Type: Basic
Hostname: localhost
Port: 1521
Service Name: FREEPDB1  ← مهم جداً
Username: HR_CORE (أو أي مستخدم آخر)
Password: Pwd_Core_123
```

### **لـ Oracle XE:**
```
Connection Type: Basic
Hostname: localhost
Port: 1521
Service Name: XEPDB1  ← مهم جداً
Username: HR_CORE
Password: Pwd_Core_123
```

---

## 🎯 ملخص سريع

**إذا كنت تستخدم Oracle 23ai Free:**
1. ✅ الملفات جاهزة (تم تعديلها)
2. ✅ استخدم `FREEPDB1` في الاتصال
3. ✅ نفذ بالترتيب:
   - `00_CREATE_SCHEMAS_Oracle23ai.sql` ← **ابدأ هنا (إلزامي)**
   - `COMPLETE_INSTALL_PART1.sql`
   - `COMPLETE_INSTALL_PART2.sql`
   - `10_GRANT_PERMISSIONS.sql`

**إذا كنت تستخدم Oracle XE:**
- غيّر `FREEPDB1` إلى `XEPDB1` في السطر 6 من كل ملف

---

## 📝 ملاحظات مهمة

1. **تأكد من تشغيل الخدمات:**
   ```powershell
   # لـ Oracle 23ai
   net start OracleServiceFREE
   net start OracleOraDB23ai_home1TNSListener
   
   # لـ Oracle XE
   net start OracleServiceXE
   net start OracleOraDB21Home1TNSListener
   ```

2. **إذا واجهت خطأ "ORA-65096: invalid common user or role name":**
   - تأكد أنك متصل بـ `FREEPDB1` وليس `CDB$ROOT`

3. **للتحقق من نجاح التثبيت:**
   ```sql
   SELECT owner, COUNT(*) as table_count 
   FROM all_tables 
   WHERE owner LIKE 'HR_%' 
   GROUP BY owner
   ORDER BY owner;
   ```

---

## 🆘 استكشاف الأخطاء

### **خطأ: TNS:could not resolve the connect identifier**
- تأكد من استخدام `FREEPDB1` وليس `FREE` أو `XE`

### **خطأ: ORA-01017: invalid username/password**
- تأكد من الاتصال بـ `FREEPDB1` (PDB) وليس CDB

### **خطأ: ORA-12541: TNS:no listener**
- شغل الـ Listener:
  ```cmd
  lsnrctl start
  ```

---

## ✨ بعد التثبيت

بعد تنفيذ الملفات الثلاثة بنجاح، ستكون لديك:
- ✅ **75 جدول** موزعة على 7 مستخدمين (Schemas)
- ✅ جميع العلاقات (Foreign Keys) جاهزة
- ✅ الصلاحيات ممنوحة بشكل صحيح

يمكنك الآن البدء في:
- إدخال البيانات الأولية (`16_SEED_DATA.sql`)
- ربط التطبيق بقاعدة البيانات
- تطوير الـ API

---

**تم إعداد هذا الدليل خصيصاً لـ Oracle 23ai Free Edition** 🎉
