using FluentValidation;
using HRMS.Application.DTOs.Auth;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;

namespace HRMS.Application.Features.Auth.Commands.Register;

public class RegisterCommand : IRequest<Result<AuthResponse>>
{
    public string FullNameAr { get; set; } = string.Empty;
    public string FullNameEn { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int? EmployeeId { get; set; }
}

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponse>>
{
    private readonly IAuthService _authService;

    public RegisterCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Result<AuthResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(new RegisterRequest
        {
            FullNameAr = request.FullNameAr,
            FullNameEn = request.FullNameEn,
            UserName = request.UserName,
            Email = request.Email,
            Password = request.Password,
            ConfirmPassword = request.ConfirmPassword,
            PhoneNumber = request.PhoneNumber,
            EmployeeId = request.EmployeeId
        });

        if (!result.Succeeded)
        {
            return Result<AuthResponse>.Failure(result.Message);
        }

        return Result<AuthResponse>.Success(result.Data!, result.Message);
    }
}

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.FullNameAr).NotEmpty().MaximumLength(100);
        RuleFor(x => x.UserName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("Passwords do not match");
    }
}
