using System;
using System.Threading.Tasks;
using DatingApp.ApI.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.ApI.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository (DataContext context)
        {
         _context =context;
        }
        public async Task<User> Login(string Username, string Password)
        {
         var user=await _context.Users.FirstOrDefaultAsync(x => x.Username==Username);
         if (user == null)

             return null;

         if(!VerfiedPasswordHash(Password,user.PasswordHash,user.PasswordSalt))
         
             return null;
             return user;



        }

        private bool VerfiedPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using( var hmac= new System.Security.Cryptography.HMACSHA512(passwordSalt))
           {
                
                var ComputeHash=hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                for(int i=0;i<ComputeHash.Length;i++)
                {
                    if(ComputeHash[i]!=passwordHash[i]) return false;

                }
           }
           return true;
        }

        public async Task<User> Register(User user, string Password)
        {
            byte[] passwordHash,passwordSalt;
            Createpasswordhash(Password,out passwordHash,out passwordSalt);
            user.PasswordHash=passwordHash;
            user.PasswordSalt=passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;

        }

        private void Createpasswordhash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
           using( var hmac= new System.Security.Cryptography.HMACSHA512())
           {
                passwordSalt=hmac.Key;
                passwordHash=hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
           }
        }

        public async Task<bool> UserExists(string username)
        {
            if (await _context.Users.AnyAsync(x => x.Username == username))
                return true;

          return false;  
        }
    }
}