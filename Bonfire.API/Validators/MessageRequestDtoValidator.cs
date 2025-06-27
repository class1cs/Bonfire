using Bonfire.Domain.Dtos.Requests;
using Bonfire.Domain.Entities;
using FluentValidation;

namespace Bonfire.Application.Validators;

public class MessageRequestDtoValidator : AbstractValidator<MessageRequest> 
{
    public MessageRequestDtoValidator()
    {
        RuleFor(message => message.Text)
            .NotEmpty().WithMessage("Message text cannot be empty.");
    }
}