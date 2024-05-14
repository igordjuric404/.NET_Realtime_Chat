using System.Threading.Tasks;
using _NET_Realtime_Chat.Models;

namespace _NET_Realtime_Chat.Services
{
    public interface IUserService
    {
        Task<bool> RegisterUser(RegisterModel model);
        Task<string?> Authenticate(LoginModel model);
    }
}
