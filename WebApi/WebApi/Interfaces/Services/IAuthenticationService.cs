using System.Threading.Tasks;
using WebApi.Models.Models.Authentication;

namespace WebApi.Interfaces.Services
{
    public interface IAuthenticationService
    {
        string AuthenticateMachine(string appSecret);

        Task<JwtResponse> AuthenticateUser(long userId, string appSecret);
    }
}