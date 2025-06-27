using Bonfire.Domain.Dtos.Requests;
using FluentValidation;

namespace Bonfire.Application.Validators;

public class RegisterRequestDtoValidator: AbstractValidator<RegisterRequest> 
{
    public RegisterRequestDtoValidator()
    {
        RuleFor(registrationData => registrationData.NickName)
            .NotEmpty().WithMessage("Nickname cannot be empty.");
        RuleFor(registrationData => registrationData.Password)
            .NotEmpty().WithMessage("Password cannot be empty.");
    }
}