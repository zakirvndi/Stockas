using MediatR;
using Stockas.Models.DTOS.User;

namespace Stockas.Application.Commands.User
{
    public class RefreshTokenCommand : IRequest<TokenResponseDTO>
    {
        public RefreshTokenDTO RefreshTokenRequestDTO { get; }

        public RefreshTokenCommand(RefreshTokenDTO dto)
        {
            RefreshTokenRequestDTO = dto;
        }
    }
}
