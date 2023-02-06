using FluentValidation;
using test_crud.Entities;
using test_crud.Models;

namespace test_crud.Validators
{
    public class UpdateTaskValidator: AbstractValidator<UpdateTaskDto>
    {
        public UpdateTaskValidator()
        {
            RuleFor(v => v.Name)
                .MinimumLength(4)
                .MaximumLength(12);

            RuleFor(v => v.TaskStatus)
                .IsInEnum();
        }
    }
}
