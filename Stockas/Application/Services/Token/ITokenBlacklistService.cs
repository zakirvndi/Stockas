namespace Stockas.Application.Services.Token
{
    public interface ITokenBlacklistService
    {
        Task BlacklistTokenAsync(string token, TimeSpan expiry);
        Task<bool> IsTokenBlacklistedAsync(string token);
    }
}
