using FluentValidation;
using test_crud.Models;

namespace test_crud.Validators
{
    public class RegisterValidator: AbstractValidator<RegisterDto>
    {
        public RegisterValidator()
        {
            RuleFor(user => user.Username)
                .MinimumLength(4)
                .MaximumLength(12)
                .NotEmpty();

            RuleFor(user => user.Password)
                .MinimumLength(4)
                .MaximumLength(12)
                .NotEmpty();
        }
    }
}
