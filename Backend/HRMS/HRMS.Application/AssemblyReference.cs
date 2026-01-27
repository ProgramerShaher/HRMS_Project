using System.Reflection;

namespace HRMS.Application;

/// <summary>
/// مرجع Assembly للتطبيق - يستخدم لتسجيل MediatR و AutoMapper
/// </summary>
public static class AssemblyReference
{
    public static Assembly Assembly => typeof(AssemblyReference).Assembly;
}
