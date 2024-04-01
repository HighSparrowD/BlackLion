using System;
using System.Threading.Tasks;

namespace WebApi.Interfaces.Services
{
    public interface IAuthenticationService
    {
        string AuthenticateMachine(string appSecret);

        Task<string> AuthenticateUser(long userId, string appSecret);
    }
}