using FluentValidation;
using HRMS.Application.DTOs.Leaves;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Leaves;
using MediatR;
using AutoMapper;

namespace HRMS.Application.Features.Leaves.PublicHolidays.Commands.CreatePublicHoliday;

// 1. Command
/// <summary>
/// أمر إنشاء عطلة رسمية جديدة
/// </summary>
public record CreatePublicHolidayCommand(PublicHolidayDto Data) : IRequest<int>;

// 2. Validator
public class CreatePublicHolidayValidator : AbstractValidator<CreatePublicHolidayCommand>
{
    public CreatePublicHolidayValidator()
    {
        RuleFor(x => x.Data.HolidayNameAr).NotEmpty().WithMessage("اسم العطلة مطلوب")
            .MaximumLength(100).WithMessage("الاسم طويل جداً");
        
        RuleFor(x => x.Data.StartDate).NotEmpty().WithMessage("تاريخ البداية مطلوب");
        RuleFor(x => x.Data.EndDate).NotEmpty().WithMessage("تاريخ النهاية مطلوب")
            .GreaterThanOrEqualTo(x => x.Data.StartDate).WithMessage("تاريخ النهاية يجب أن يكون بعد البداية");
            
        RuleFor(x => x.Data.Year).GreaterThan((short)2000).WithMessage("السنة غير صحيحة");
    }
}

// 3. Handler
public class CreatePublicHolidayCommandHandler : IRequestHandler<CreatePublicHolidayCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreatePublicHolidayCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<int> Handle(CreatePublicHolidayCommand request, CancellationToken cancellationToken)
    {
        var holiday = _mapper.Map<PublicHoliday>(request.Data);
        _context.PublicHolidays.Add(holiday);
        await _context.SaveChangesAsync(cancellationToken);
        return holiday.HolidayId;
    }
}
