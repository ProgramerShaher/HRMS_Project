<div align="center">

<img src="https://www.google.com/search?q=https://cdn-icons-png.flaticon.com/512/3063/3063176.png" alt="Logo" width="160" height="160" />

🏥 Hospital Pulse ERP

Next-Gen Healthcare Management System

نظام تخطيط موارد المؤسسات الذكي لإدارة المستشفيات والقطاع الطبي

<!-- Badges Section -->

<p>
<a href="https://dotnet.microsoft.com/">
<img src="https://www.google.com/search?q=https://img.shields.io/badge/Backend-.NET%25208.0-512bd4%3Fstyle%3Dfor-the-badge%26logo%3D.net%26logoColor%3Dwhite" />
</a>
<a href="https://angular.io/">
<img src="https://www.google.com/search?q=https://img.shields.io/badge/Frontend-Angular%252017-dd0031%3Fstyle%3Dfor-the-badge%26logo%3Dangular%26logoColor%3Dwhite" />
</a>
<a href="https://www.microsoft.com/sql-server">
<img src="https://www.google.com/search?q=https://img.shields.io/badge/Database-SQL%2520Server%25202022-CC2927%3Fstyle%3Dfor-the-badge%26logo%3Dmicrosoft-sql-server%26logoColor%3Dwhite" />
</a>
</p>
<p>
<img src="https://www.google.com/search?q=https://img.shields.io/badge/Architecture-Clean%2520Architecture-blue%3Fstyle%3Dflat-square%26logo%3Dcsharp%26logoColor%3Dwhite" />
<img src="https://www.google.com/search?q=https://img.shields.io/badge/Pattern-CQRS%2520%252B%2520Mediator-green%3Fstyle%3Dflat-square" />
<img src="https://www.google.com/search?q=https://img.shields.io/badge/Security-JWT%2520%252B%2520RBAC-orange%3Fstyle%3Dflat-square" />
<img src="https://www.google.com/search?q=https://img.shields.io/badge/Status-Production%2520Ready-success%3Fstyle%3Dflat-square" />
</p>

<h3>"حيث تلتقي الرعاية الطبية بالدقة الإدارية."</h3>
<p>
نظام ERP متكامل يدير رحلة الطبيب والموظف من "الإعلان الوظيفي" وحتى "التقاعد"، بدقة مالية 100% مع مراعاة خصوصية العمل الطبي.
</p>

<p>
<a href="#-نظرة-عامة-overview">🌟 نظرة عامة</a> •
<a href="#-الهيكلية-system-architecture">🏗️ الهيكلية</a> •
<a href="#-الوحدات-الأساسية-modules">💎 الوحدات</a> •
<a href="#-سيناريوهات-التكامل-integration">🔥 التكامل السحري</a> •
<a href="#-كيفية-التشغيل-installation">🚀 التشغيل</a>
</p>
</div>

🌟 نظرة عامة (Overview)

Hospital Pulse ERP ليس مجرد برنامج شؤون موظفين تقليدي؛ هو عقل إلكتروني مصمم خصيصاً لمواجهة التحديات الفريدة لإدارة المستشفيات التي تعمل على مدار الساعة (24/7).

يعالج النظام التعقيدات الخاصة بالكادر الطبي مثل:

🚑 نظام المناوبات المتغيرة (Shift Rostering): إدارة الورديات الصباحية والمسائية والسهر للأطباء والممرضين.

📜 التراخيص الطبية: مراقبة تواريخ انتهاء تراخيص الهيئة الطبية وتأمين الأخطاء المهنية وتنبيه الإدارة آلياً.

💸 الرواتب المعقدة: احتساب بدلات العدوى، الندرة، التفرغ، والمناوبات الإضافية بشكل ديناميكي.

🔄 التكامل الآلي: التحول من "إدخال البيانات" اليدوي إلى "تدفق البيانات" الآلي بين الأقسام.

🏗️ الهيكلية الهندسية (System Architecture)

تم بناء النظام وفقاً لأرقى معايير هندسة البرمجيات العالمية (Clean Architecture) لضمان الاستقرار، الأمان، وقابلية التوسع.

graph TD
    User((👨‍⚕️ User/Admin)) --> UI[💻 Angular 17 SPA]
    UI --> API[🛡️ ASP.NET Core 8 Web API]
    
    subgraph "Backend Core (The Brain)"
      API --> App[Application Layer (CQRS + MediatR)]
      App --> Domain[Domain Layer (Entities + Logic)]
      App --> Infra[Infrastructure (EF Core & Services)]
    end
    
    Infra --> DB[(🗄️ SQL Server)]
    
    style API fill:#512bd4,stroke:#fff,stroke-width:2px,color:#fff
    style UI fill:#dd0031,stroke:#fff,stroke-width:2px,color:#fff
    style DB fill:#CC2927,stroke:#fff,stroke-width:2px,color:#fff
    style App fill:#098a00,stroke:#fff,stroke-width:1px,color:#fff


🛠️ الترسانة التقنية (Tech Stack)

تم اختيار أدوات التطوير بعناية فائقة لتوفير تجربة مستخدم سلسة وأداء خادم قوي:

المجال

التقنيات والأدوات

الوصف

Backend

.NET 8

إطار عمل قوي وسريع جداً للتعامل مع العمليات الحسابية المعقدة.

Database



قاعدة بيانات علائقية لإدارة ملايين السجلات الطبية والمالية.

Frontend

v17

واجهة مستخدم تفاعلية (SPA) توفر تجربة مستخدم سلسة للأطباء.

Patterns

CQRS, Mediator, Repository

أنماط تصميم تضمن فصل المسؤوليات وسهولة الصيانة.

Validation

FluentValidation

قواعد تحقق صارمة لضمان دقة البيانات المدخلة.

Libraries

AutoMapper, Serilog, JWT

أدوات مساعدة للأمان والتحويل والتسجيل.

💎 الوحدات الأساسية (Modules Breakdown)

نظام شامل يغطي 5 محاور رئيسية، تعمل بتناغم تام:

1️⃣ وحدة التوظيف الطبي (Medical Recruitment) 🩺

بوابة المستشفى لاستقطاب الكفاءات.

إدارة الشواغر: نشر وظائف للأطباء الاستشاريين، المقيمين، والممرضين.

فلترة المرشحين: تتبع المتقدمين ومراحل المقابلات الفنية.

عروض العمل الذكية: إنشاء عقود تتضمن البدلات الطبية المعقدة.

✨ Automated Onboarding: تحويل المرشح لموظف بضغطة زر واحدة.

2️⃣ شؤون الموظفين (Core HR) 🗂️

السجل الرقمي الكامل.

السجل الطبي: متابعة تواريخ انتهاء تراخيص الهيئة الطبية.

العقود: إدارة عقود الـ Locum والدوام الكامل.

الهيكل التنظيمي: إدارة الأقسام الطبية، التمريض، والمختبرات.

3️⃣ الحضور والمناوبات (Attendance & Rostering) ⏰

إدارة وقت الكادر الطبي بدقة.

جداول المناوبات (Rosters): دعم الورديات المعقدة (شفتات ليلية، مناوبات 12 ساعة).

التكامل الحي: سحب بيانات البصمة لحظياً.

✨ الربط المالي: تحويل دقائق التأخير والإضافي إلى قيود مالية ترحل للرواتب فوراً.

4️⃣ إدارة الأداء (Performance) 📈

الجودة هي المعيار.

KPIs طبية: تقييم الأطباء بناءً على معايير فنية (عدد العمليات، نسبة النجاح) وسلوكية.

دورات التقييم: تقييم ربع سنوي وسنوي.

✨ الجزاءات: ربط المخالفات الإدارية بالخصومات المالية مباشرة.

5️⃣ محرك الرواتب (Payroll Engine) 💰

دقة مالية 100%.

المعالجة بضغطة زر: حساب رواتب آلاف الموظفين في ثوانٍ.

المعادلة الذكية:

(الأساسي + بدل ندرة + بدل تفرغ + مناوبات) - (الغياب + التأخير + الضرائب + السلف)

التقارير: إصدار كشوفات الرواتب (Payslips) وتقارير البنوك (WPS).

🔥 سيناريوهات التكامل الذكي (ERP Integration)

هنا يكمن سحر النظام، حيث تتحدث الوحدات مع بعضها البعض دون تدخل بشري:

<div align="center">

الحدث (Trigger)

الوحدة المصدر

الإجراء الآلي (Auto-Action)

الوحدة الهدف

قبول عرض العمل

Recruitment

⚙️ إنشاء ملف موظف + هيكل راتب

Core HR + Payroll

إغلاق شهر الحضور

Attendance

⚙️ حساب خصم التأخير والإضافي

Payroll

اعتماد مخالفة

Performance

⚙️ إنشاء قيد مالي (Deduction)

Payroll

ترقية طبيب

Core HR

⚙️ تحديث البدلات والأساسي

Payroll

</div>

📸 لقطات من النظام (Screenshots)

<table width="100%">
<tr>
<th width="50%">لوحة القيادة (Dashboard)</th>
<th width="50%">مسير الرواتب (Payroll Run)</th>
</tr>
<tr>
<td><img src="https://www.google.com/search?q=https://via.placeholder.com/600x350/0d6efd/ffffff%3Ftext%3DDoctor%2BDashboard%2BUI" alt="Dashboard" width="100%"></td>
<td><img src="https://www.google.com/search?q=https://via.placeholder.com/600x350/198754/ffffff%3Ftext%3DPayroll%2BEngine%2BProcessing" alt="Payroll" width="100%"></td>
</tr>
<tr>
<th>دورة التوظيف (Recruitment Flow)</th>
<th>جداول المناوبات (Shift Roster)</th>
</tr>
<tr>
<td><img src="https://www.google.com/search?q=https://via.placeholder.com/600x350/ffc107/000000%3Ftext%3DRecruitment%2BWorkflow" alt="Recruitment" width="100%"></td>
<td><img src="https://www.google.com/search?q=https://via.placeholder.com/600x350/dc3545/ffffff%3Ftext%3DShift%2BManagement" alt="Roster" width="100%"></td>
</tr>
</table>

🚀 كيفية التشغيل (Installation)

اتبع الخطوات التالية لتشغيل النظام في بيئة التطوير الخاصة بك:

1️⃣ المتطلبات المسبقة

.NET 8.0 SDK

Node.js (LTS Version)

SQL Server

2️⃣ إعداد قاعدة البيانات

# قم بتحديث نص الاتصال في appsettings.json أولاً
Update-Database -Context HRMSDbContext


3️⃣ تشغيل الواجهة الخلفية (API)

cd HRMS.API
dotnet run
# سيعمل السيرفر على: https://localhost:5001


4️⃣ تشغيل الواجهة الأمامية (Client)

cd HRMS.Client
npm install
ng serve
# افتح المتصفح على: http://localhost:4200


🤝 المساهمة (Contributing)

نرحب بمساهماتكم! إذا كان لديك اقتراح لتطوير النظام:

قم بعمل Fork للمشروع.

أنشئ فرعاً جديداً (git checkout -b feature/AmazingFeature).

قم بعمل Commit لتغييراتك (git commit -m 'Add some AmazingFeature').

ارفع التغييرات (git push origin feature/AmazingFeature).

افتح Pull Request.

<div align="center">





<b>تم التطوير بواسطة شاهر اليعري ❤️ شغف لخدمة القطاع الطبي</b>





<span>جميع الحقوق محفوظة © 2026</span>







<a href="#">
<img src="https://www.google.com/search?q=https://img.shields.io/badge/Contact-Email-red%3Fstyle%3Dfor-the-badge%26logo%3Dgmail%26logoColor%3Dwhite" />
</a>
<a href="#">
<img src="https://www.google.com/search?q=https://img.shields.io/badge/Connect-LinkedIn-blue%3Fstyle%3Dfor-the-badge%26logo%3Dlinkedin%26logoColor%3Dwhite" />
</a>
</div>
