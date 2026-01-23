# دليل الربط مع Oracle Database 21c

## الخطوة 1: التحقق من تشغيل Oracle
افتح Command Prompt وأدخل:
```cmd
lsnrctl status
```
يجب أن ترى معلومات الـ Listener تعمل.

## الخطوة 2: معرفة معلومات الاتصال

### للحصول على Service Name:
افتح SQL*Plus كـ SYSDBA:
```cmd
sqlplus / as sysdba
```
ثم نفذ:
```sql
SELECT name FROM v$database;
SELECT name FROM v$pdbs;
```

## الخطوة 3: إنشاء الاتصال في VS Code

1. في VS Code، افتح لوحة Oracle SQL Developer (من الشريط الجانبي)
2. اضغط على زر **"Create Connection"**
3. املأ البيانات التالية:

### معلومات الاتصال الأساسية:
- **Connection Name**: `HR_System_Local`
- **Authentication Type**: `Default`
- **Username**: `SYSTEM` (أو `SYS` إذا كنت تريد صلاحيات كاملة)
- **Password**: كلمة المرور التي أنشأتها عند تثبيت Oracle
- **Save Password**: ✓ (اختياري)

### تفاصيل الاتصال (Details Tab):
- **Hostname**: `localhost`
- **Port**: `1521`
- **Type**: `Service Name`
- **Service Name**: 
  - إذا كنت تستخدم Oracle XE: `XEPDB1`
  - إذا كنت تستخدم Oracle Standard: `ORCLPDB` أو `ORCL`

4. اضغط **Test** للتحقق من الاتصال
5. إذا نجح، اضغط **Connect**

## الخطوة 4: تنفيذ السكربتات

بعد الاتصال الناجح:

### أ) إنشاء المستخدمين (Schemas):
1. افتح الملف: `Database\Oracle_DDL\00_Setup_Schemas.sql`
2. اضغط F5 أو زر Run
3. انتظر حتى يكتمل التنفيذ

### ب) إنشاء الجداول:
نفذ الملفات بالترتيب:
1. `01_HR_CORE.sql`
2. `02_HR_PERSONNEL.sql`
3. `03_HR_ATTENDANCE.sql`
4. `04_HR_LEAVES.sql`
5. `05_HR_PAYROLL.sql`
6. `06_HR_RECRUITMENT.sql`
7. `07_HR_PERFORMANCE.sql`

### ج) إنشاء الـ PL/SQL Packages:
1. `PLSQL\01_PKG_EMP_MANAGER_SPEC.sql`
2. `PLSQL\02_PKG_EMP_MANAGER_BODY.sql`

## استكشاف الأخطاء الشائعة:

### خطأ: ORA-12541 (TNS:no listener)
الحل: تأكد من تشغيل Oracle Listener:
```cmd
lsnrctl start
```

### خطأ: ORA-01017 (invalid username/password)
الحل: تأكد من كلمة المرور، أو أعد تعيينها:
```sql
ALTER USER SYSTEM IDENTIFIED BY new_password;
```

### خطأ: ORA-12514 (TNS:listener does not currently know of service)
الحل: تحقق من اسم الخدمة (Service Name) الصحيح باستخدام:
```sql
SELECT name FROM v$pdbs;
```

## ملاحظات مهمة:
- إذا كنت تستخدم Oracle XE، فالـ Service Name عادة يكون `XEPDB1`
- إذا كنت تستخدم Oracle Standard/Enterprise، فعادة يكون `ORCLPDB` أو `ORCL`
- يمكنك إنشاء اتصالات متعددة (واحد لكل Schema: HR_CORE, HR_PERSONNEL, إلخ)
