using FluentValidation;

namespace DelegasiAPI.Models.Validators
{
    public class SampleModelValidator: AbstractValidator<SampleModel>
    {
        public SampleModelValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();
        }
    }
}
