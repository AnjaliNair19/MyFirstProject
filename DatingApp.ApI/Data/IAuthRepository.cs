using System.Threading.Tasks;
using DatingApp.ApI.Models;

namespace DatingApp.ApI.Data
{
    public interface IAuthRepository
    {
         Task<User> Register(User  user,string Password);
         Task<User> Login(string Username,string Password);
         Task<bool> UserExists (string Username);
    }
}