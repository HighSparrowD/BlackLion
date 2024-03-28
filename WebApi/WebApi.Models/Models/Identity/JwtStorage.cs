using WebApi.Models.Models.Authentication;
namespace WebApi.Models.Models.Identity;

public static class JwtStorage
{
    // TODO: Use cache, or something more secure
    private static readonly List<JwtSigningKey> _validTokenIds = new List<JwtSigningKey>();

    public static string SignToken()
    {
        var newId = Guid.NewGuid().ToString();

        var signKey = new JwtSigningKey
        {
            SignId = newId,
            UserId = null
        };

        _validTokenIds.RemoveAll(vk => string.IsNullOrEmpty(vk.UserId));
        _validTokenIds.Add(signKey);

        return newId;
    }

    public static string SignToken(string userId)
    {
        var newId = Guid.NewGuid().ToString();

        var signKey = new JwtSigningKey
        {
            SignId = newId,
            UserId = userId
        };

        _validTokenIds.RemoveAll(vk => vk.UserId.Equals(userId));
        _validTokenIds.Add(signKey);

        return newId;
    }

    public static bool IsTokenValid(string tokenId, string userId)
    {
        return _validTokenIds.Any(v => v.SignId == tokenId && v.UserId == userId);
    }
}
