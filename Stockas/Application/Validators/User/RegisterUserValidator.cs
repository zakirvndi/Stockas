using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Stockas.Application.Commands.User;
using Stockas.Entities;

public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
    private readonly StockasContext _context;
    public RegisterUserValidator(StockasContext context)
    {
        _context = context;

        RuleFor(x => x.RegisterUserDTO.Name)
            .NotEmpty().WithMessage("Nama tidak boleh kosong.")
            .MinimumLength(3).WithMessage("Nama minimal memiliki 3 huruf.")
            .Matches(@"^[a-zA-Z\s'-]+$").WithMessage("Nama hanya boleh mengandung huruf dan spasi.")
            .OverridePropertyName("Name");

        RuleFor(x => x.RegisterUserDTO.Email)
            .NotEmpty().WithMessage("Email tidak boleh kosong.")
            .EmailAddress().WithMessage("Format email tidak valid.")
            .MustAsync(async (email, ct) =>
                !await context.Users.AnyAsync(u => u.Email == email, ct))
            .WithMessage("Email sudah terdaftar")
            .OverridePropertyName("Email");

        RuleFor(x => x.RegisterUserDTO.Password)
            .NotEmpty().WithMessage("Password tidak boleh kosong.")
            .MinimumLength(8).WithMessage("Password harus minimal 8 karakter.")
            .Matches(@"(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])")
            .WithMessage("Password harus mengandung huruf besar, huruf kecil, angka, dan karakter khusus.")
            .OverridePropertyName("Password");
    }
}