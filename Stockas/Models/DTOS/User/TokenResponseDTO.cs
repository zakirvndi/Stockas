namespace Stockas.Models.DTOS.User
{
    public class TokenResponseDTO
    {
        public string Token { get; }
        public string RefreshToken { get; }
        public UserBriefResponseDTO User { get; }

        public TokenResponseDTO(string token, string refreshToken, UserBriefResponseDTO user)
        {
            Token = token;
            RefreshToken = refreshToken;
            User = user;
        }
    }

    public class UserBriefResponseDTO
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}