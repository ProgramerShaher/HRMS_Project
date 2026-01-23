# 📊 دليل استخدام Dashboard Views في APEX

## 🎯 نظرة عامة

تم إنشاء **8 Views** جاهزة للاستخدام المباشر في APEX لبناء Dashboard احترافي.

---

## 📂 الملف
```
02_Core_Data/HR_CORE/Views/02_DASHBOARD_VIEWS.sql
```

---

## 🚀 كيفية الاستخدام

### الخطوة 1: تنصيب Views
```bash
sqlplus HR_CORE/Pwd_Core_123@FREEPDB1
@02_Core_Data/HR_CORE/Views/02_DASHBOARD_VIEWS.sql
```

### الخطوة 2: استخدامها في APEX
فقط اكتب:
```sql
SELECT * FROM HR_CORE.V_DASHBOARD_STATS
```

---

## 📊 الـ Views المتاحة

### 1. **V_DASHBOARD_STATS** - الإحصائيات الرئيسية

#### الاستخدام في APEX:
- **Badge List**
- **Value Cards**
- **KPI Region**

#### الاستعلام:
```sql
SELECT * FROM HR_CORE.V_DASHBOARD_STATS
```

#### النتيجة:
```
TOTAL_DEPARTMENTS: 12
TOTAL_JOBS: 14
TOTAL_BRANCHES: 4
TOTAL_BANKS: 6
TOTAL_CITIES: 8
TOTAL_COUNTRIES: 5
TOTAL_GRADES: 5
TOTAL_DOC_TYPES: 7
```

#### في APEX:
1. Create Region → **Badge List**
2. SQL Query: `SELECT * FROM HR_CORE.V_DASHBOARD_STATS`
3. Label: `عدد الأقسام`
4. Value: `TOTAL_DEPARTMENTS`

---

### 2. **V_NAVIGATION_CARDS** - بطاقات التنقل

#### الاستخدام في APEX:
- **Cards Region**
- **Navigation Menu**

#### الاستعلام:
```sql
SELECT * FROM HR_CORE.V_NAVIGATION_CARDS ORDER BY CARD_ORDER
```

#### الأعمدة:
- `TITLE` - العنوان
- `SUBTITLE` - الوصف
- `ICON` - الأيقونة (Font Awesome)
- `COLOR` - اللون
- `TARGET_PAGE` - الصفحة المستهدفة
- `CARD_ORDER` - الترتيب

#### في APEX:
1. Create Region → **Cards**
2. SQL Query: `SELECT * FROM HR_CORE.V_NAVIGATION_CARDS ORDER BY CARD_ORDER`
3. Attributes:
   - Title: `TITLE`
   - Body: `SUBTITLE`
   - Icon: `ICON`
   - Icon CSS Classes: `ICON`
   - Primary Key: `CARD_ORDER`

---

### 3. **V_JOBS_BY_DEPARTMENT** - توزيع الوظائف

#### الاستخدام في APEX:
- **Pie Chart**
- **Bar Chart**
- **Donut Chart**

#### الاستعلام:
```sql
SELECT * FROM HR_CORE.V_JOBS_BY_DEPARTMENT
```

#### في APEX:
1. Create Region → **Chart**
2. Chart Type: **Pie**
3. SQL Query: `SELECT * FROM HR_CORE.V_JOBS_BY_DEPARTMENT`
4. Label: `DEPARTMENT`
5. Value: `JOB_COUNT`

---

### 4. **V_BRANCHES_BY_CITY** - توزيع الفروع

#### الاستخدام في APEX:
- **Map**
- **Bar Chart**

#### الاستعلام:
```sql
SELECT * FROM HR_CORE.V_BRANCHES_BY_CITY
```

---

### 5. **V_SALARY_RANGES** - نطاقات الرواتب

#### الاستخدام في APEX:
- **Range Chart**
- **Interactive Report**

#### الاستعلام:
```sql
SELECT * FROM HR_CORE.V_SALARY_RANGES
```

#### في APEX:
1. Create Region → **Chart**
2. Chart Type: **Range**
3. SQL Query: `SELECT * FROM HR_CORE.V_SALARY_RANGES`
4. Label: `GRADE_NAME`
5. Low Value: `MIN_SALARY`
6. High Value: `MAX_SALARY`

---

### 6. **V_RECENT_ACTIVITY** - النشاط الأخير

#### الاستخدام في APEX:
- **Timeline**
- **Activity Feed**
- **List View**

#### الاستعلام:
```sql
SELECT * FROM HR_CORE.V_RECENT_ACTIVITY
```

#### في APEX:
1. Create Region → **Classic Report**
2. SQL Query: `SELECT * FROM HR_CORE.V_RECENT_ACTIVITY`
3. Template: **Timeline**

---

### 7. **V_DEPARTMENTS_TREE** - الهيكل التنظيمي

#### الاستخدام في APEX:
- **Tree View**
- **Org Chart**

#### الاستعلام:
```sql
SELECT * FROM HR_CORE.V_DEPARTMENTS_TREE
```

---

### 8. **V_QUICK_SEARCH** - البحث السريع

#### الاستخدام في APEX:
- **Search Bar**
- **Autocomplete**

#### الاستعلام:
```sql
SELECT * FROM HR_CORE.V_QUICK_SEARCH
WHERE UPPER(ITEM_NAME) LIKE '%' || UPPER(:P1_SEARCH) || '%'
```

---

## 🎨 مثال Dashboard كامل

### الهيكل المقترح:

```
┌─────────────────────────────────────────────────────────┐
│                    HRMS Dashboard                       │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  ┌──────┐  ┌──────┐  ┌──────┐  ┌──────┐              │
│  │  12  │  │  14  │  │   4  │  │   6  │              │
│  │أقسام │  │وظائف │  │فروع  │  │بنوك  │              │
│  └──────┘  └──────┘  └──────┘  └──────┘              │
│                                                         │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐   │
│  │   الأقسام   │  │   الوظائف   │  │   الفروع    │   │
│  │     📋      │  │     💼      │  │     📍      │   │
│  └─────────────┘  └─────────────┘  └─────────────┘   │
│                                                         │
│  ┌──────────────────────┐  ┌──────────────────────┐   │
│  │  توزيع الوظائف      │  │  النشاط الأخير      │   │
│  │  [Pie Chart]        │  │  [Timeline]         │   │
│  └──────────────────────┘  └──────────────────────┘   │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

### الكود في APEX:

#### Region 1: Stats (Badge List)
```sql
SELECT * FROM HR_CORE.V_DASHBOARD_STATS
```

#### Region 2: Navigation (Cards)
```sql
SELECT * FROM HR_CORE.V_NAVIGATION_CARDS ORDER BY CARD_ORDER
```

#### Region 3: Chart (Pie)
```sql
SELECT * FROM HR_CORE.V_JOBS_BY_DEPARTMENT
```

#### Region 4: Activity (Timeline)
```sql
SELECT * FROM HR_CORE.V_RECENT_ACTIVITY
```

---

## ✅ الخلاصة

- ✅ **8 Views** جاهزة
- ✅ استخدام مباشر في APEX
- ✅ لا حاجة لكتابة SQL معقد
- ✅ سهولة الصيانة

**ابدأ الآن ببناء Dashboard في APEX!** 🚀
