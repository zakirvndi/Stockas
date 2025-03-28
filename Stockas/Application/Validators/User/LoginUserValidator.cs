using FluentValidation;
using Stockas.Models.DTOS.User;

namespace Stockas.Application.Validators.User
{
    public class LoginUserValidator : AbstractValidator<LoginDTO>
    {
        public LoginUserValidator()
        {
            RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email tidak boleh kosong.")
            .EmailAddress().WithMessage("Format email tidak valid.").
            OverridePropertyName("Email");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password tidak boleh kosong.")
                .OverridePropertyName("Password");
        }
    }
}
