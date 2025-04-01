using MediatR;
using Stockas.Models.DTOS.User;

namespace Stockas.Application.Commands.User
{
    public class LoginUserCommand : IRequest<TokenResponseDTO>
    {
        public LoginDTO LoginUserDTO { get; set; }

        public LoginUserCommand(LoginDTO dto)
        {
            LoginUserDTO = dto;
        }
    }
}
