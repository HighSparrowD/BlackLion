using System;
using System.Threading.Tasks;

namespace WebApi.Interfaces
{
    public interface IAuthenticationService
    {
        string AuthenticateMachine(string appSecret);

        Task<string> AuthenticateUser(long userId, string appSecret);
    }
}