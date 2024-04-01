using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApi.Interfaces.Services
{
    public interface IAdminService
    {
        Task StartInDebug(List<long> userIds);
    }
}
