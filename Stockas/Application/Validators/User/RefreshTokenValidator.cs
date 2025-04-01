using FluentValidation;
using Stockas.Models.DTOS.User;

namespace Stockas.Application.Validators.User
{
    public class RefreshTokenValidator : AbstractValidator<RefreshTokenDTO>
    {
        public RefreshTokenValidator()
        {
            RuleFor(x => x.AccessToken)
                .NotEmpty().WithMessage("Access token is required")
                .OverridePropertyName("AccessToken");

            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Refresh token is required")
                .OverridePropertyName("RefreshToken");
        }
    }
}
