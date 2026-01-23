# 📘 دليل APEX - إخفاء حقول Audit

## 🎯 الهدف
إخفاء حقول Audit من Forms وعرضها فقط في Reports

---

## ✅ القاعدة الذهبية

### في Forms (الإدخال):
```
❌ إخفاء حقول Audit
✅ عرض الحقول الأساسية فقط
```

### في Interactive Reports (العرض):
```
✅ عرض جميع الحقول
✅ عرض حقول Audit للمراجعة
```

---

## 🔧 التطبيق في APEX

### 1. **إنشاء Form**

#### الخطوات:
1. Create Page → Form
2. اختر الجدول (مثل `COUNTRIES`)
3. APEX سيُنشئ Form مع **جميع** الحقول

#### إخفاء حقول Audit:
```
في Page Designer:
1. اختر الحقل (مثل CREATED_BY)
2. في Properties:
   - Type: Hidden
   أو
   - Server-side Condition:
     Type: Never
```

#### الحقول المخفية:
- `CREATED_BY`
- `CREATED_AT`
- `UPDATED_BY`
- `UPDATED_AT`
- `IS_DELETED`
- `VERSION_NO`

---

### 2. **إنشاء Interactive Report**

#### الخطوات:
1. Create Page → Interactive Report
2. اختر الجدول
3. **اعرض جميع الحقول** (بما فيها Audit)

#### SQL Query:
```sql
SELECT 
    COUNTRY_ID,
    COUNTRY_NAME_AR,
    COUNTRY_NAME_EN,
    ISO_CODE,
    -- حقول Audit (تظهر في Report)
    CREATED_BY,
    CREATED_AT,
    UPDATED_BY,
    UPDATED_AT,
    VERSION_NO
FROM HR_CORE.COUNTRIES
WHERE IS_DELETED = 0
ORDER BY COUNTRY_NAME_AR
```

---

## 📋 مثال عملي: Countries

### Form (الإدخال)

#### الحقول المعروضة:
```
┌─────────────────────────────┐
│ اسم الدولة (عربي)          │
│ [_____________________]     │
│                             │
│ اسم الدولة (إنجليزي)       │
│ [_____________________]     │
│                             │
│ رمز الدولة (ISO)            │
│ [__]                        │
│                             │
│ [حفظ] [إلغاء]               │
└─────────────────────────────┘
```

#### الحقول المخفية (تُملأ تلقائياً):
- `CREATED_BY` ← من Trigger
- `CREATED_AT` ← من Trigger
- `VERSION_NO` ← من Trigger
- `IS_DELETED` ← من Trigger

---

### Interactive Report (العرض)

```
┌────┬──────────┬────────────┬──────┬────────────┬────────────┬────────────┬────────────┬─────────┐
│ ID │ الاسم AR │ الاسم EN   │ ISO  │ أنشأ بواسطة│ تاريخ الإنشاء│ عدّل بواسطة│ تاريخ التعديل│ الإصدار│
├────┼──────────┼────────────┼──────┼────────────┼────────────┼────────────┼────────────┼─────────┤
│ 1  │ اليمن    │ Yemen      │ YE   │ ADMIN      │ 2026-01-20 │ HR_USER    │ 2026-01-20 │ 2       │
│ 2  │ السعودية│ Saudi      │ SA   │ ADMIN      │ 2026-01-20 │ -          │ -          │ 1       │
└────┴──────────┴────────────┴──────┴────────────┴────────────┴────────────┴────────────┴─────────┘
```

---

## 🎨 Best Practices

### 1. **تسمية واضحة للأعمدة**
```sql
SELECT 
    COUNTRY_NAME_AR AS "اسم الدولة",
    CREATED_BY AS "أنشأ بواسطة",
    CREATED_AT AS "تاريخ الإنشاء"
FROM HR_CORE.COUNTRIES
```

### 2. **تنسيق التواريخ**
```sql
SELECT 
    TO_CHAR(CREATED_AT, 'DD/MM/YYYY HH24:MI') AS "تاريخ الإنشاء"
FROM HR_CORE.COUNTRIES
```

### 3. **إخفاء IS_DELETED من Report**
```sql
-- عرض السجلات النشطة فقط
WHERE IS_DELETED = 0
```

### 4. **عرض VERSION_NO بشكل واضح**
```sql
SELECT 
    VERSION_NO AS "الإصدار",
    CASE 
        WHEN VERSION_NO = 1 THEN 'جديد'
        ELSE 'معدّل (' || VERSION_NO || ')'
    END AS "الحالة"
FROM HR_CORE.COUNTRIES
```

---

## 🔐 Security Context في APEX

### إعداد Application Context

#### في Application Settings:
```
1. Shared Components → Application Processes
2. Create Process:
   - Name: Set User Context
   - Point: On New Session
   - PL/SQL Code:
   
   BEGIN
       PKG_SECURITY_CONTEXT.SET_USER_INFO(
           p_username => :APP_USER
       );
   END;
```

#### في Page Process (قبل Submit):
```sql
BEGIN
    PKG_SECURITY_CONTEXT.SET_USER_INFO(
        p_username => :APP_USER
    );
END;
```

---

## ✅ الخلاصة

### في Forms:
- ❌ لا تعرض حقول Audit
- ✅ Triggers تملأها تلقائياً

### في Reports:
- ✅ اعرض جميع الحقول
- ✅ حقول Audit للمراجعة

### في APEX:
- ✅ استخدم Security Context
- ✅ اخفِ الحقول بـ Type: Hidden
- ✅ نسّق التواريخ بشكل جيد

---

**النظام جاهز للاستخدام!** ✅
