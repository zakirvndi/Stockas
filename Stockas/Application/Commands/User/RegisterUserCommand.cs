using MediatR;
using Stockas.Models.DTOS.User;

namespace Stockas.Application.Commands.User
{
    public class RegisterUserCommand : IRequest<TokenResponseDTO>
    {
        public RegisterDTO RegisterUserDTO { get; }

        public RegisterUserCommand(RegisterDTO dto)
        {
            RegisterUserDTO = dto;
        }
    }
}
