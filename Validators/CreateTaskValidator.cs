using FluentValidation;
using test_crud.Models;

namespace test_crud.Validators
{
    public class CreateTaskValidator: AbstractValidator<CreateTaskDto>
    {
        public CreateTaskValidator()
        {
            RuleFor(v => v.Name)
                .MinimumLength(6)
                .MaximumLength(50)
                .NotEmpty();
        }
    }
}
