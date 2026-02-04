using FluentValidation;
using HRMS.Application.DTOs.Auth;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;

namespace HRMS.Application.Features.Auth.Commands.Login;

public class LoginCommand : IRequest<Result<AuthResponse>>
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly IAuthService _authService;

    public LoginCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(new LoginRequest 
        { 
            UserName = request.UserName, 
            Password = request.Password 
        });

        if (!result.Succeeded)
        {
            return Result<AuthResponse>.Failure(result.Message);
        }

        return Result<AuthResponse>.Success(result.Data!, result.Message);
    }
}

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(p => p.UserName).NotEmpty().WithMessage("اسم المستخدم مطلوب");
        RuleFor(p => p.Password).NotEmpty().WithMessage("كلمة المرور مطلوبة");
    }
}
