using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using HRMS.Application.Exceptions;
using HRMS.Core.Utilities;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Application.Features.Core.SystemSettings.Commands.UpdateSetting;

// 1. Command
/// <summary>
/// أمر تحديث إعداد نظام
/// </summary>
public record UpdateSettingCommand(string SettingKey, string SettingValue) : IRequest<bool>;

// 2. Validator
public class UpdateSettingValidator : AbstractValidator<UpdateSettingCommand>
{
    public UpdateSettingValidator()
    {
        RuleFor(x => x.SettingKey).NotEmpty().WithMessage("مفتاح الإعداد مطلوب");
        RuleFor(x => x.SettingValue).NotEmpty().WithMessage("قيمة الإعداد مطلوبة");
    }
}

// 3. Handler
public class UpdateSettingCommandHandler : IRequestHandler<UpdateSettingCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateSettingCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateSettingCommand request, CancellationToken cancellationToken)
    {
        // Fetch setting by key
        var setting = await _context.SystemSettings
            .FirstOrDefaultAsync(s => s.SettingKey == request.SettingKey, cancellationToken);
            
        if (setting == null)
            throw new NotFoundException($"Setting Key '{request.SettingKey}' not found.");

        // Validation Logic: Editable check
        // Validation Logic: Editable check
        if (setting.IsEditable == 0) // 0 means not editable
            throw new FluentValidation.ValidationException("لا يمكن تعديل هذا الإعداد (محمي من النظام).");

        // Update
        setting.SettingValue = request.SettingValue;
        
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
