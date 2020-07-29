using System.Threading.Tasks;
using DatingApp.ApI.Data;
using DatingApp.ApI.DTOS;
using Microsoft.AspNetCore.Mvc;
using DatingApp.ApI.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Configuration;
using System;

namespace DatingApp.ApI.Controllers
{
     [Route("api/[controller]")]
     [ApiController]
    public class AuthController: ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repo,IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }

        [HttpPost ("register")]

        public async Task<IActionResult> Register(UserToRegister userForRegisterDto)
        {
            userForRegisterDto.username=userForRegisterDto.username.ToLower();
            if(await _repo.UserExists(userForRegisterDto.username))

            return BadRequest ("Username already exist");

            var UserTocreate =new User
            {
                Username = userForRegisterDto.username

            };
            var createduser = await _repo.Register(UserTocreate,userForRegisterDto.password);

            return StatusCode(201);


        }

        [HttpPost ("login")]

        public async Task<IActionResult> Login(UserToLogin usertologin)
        {
            var userFromRepo =await _repo.Login(usertologin.Username.ToLower(),usertologin.Password);

            if (userFromRepo == null)

            return Unauthorized();

            var claims= new[]
            {
                new Claim(ClaimTypes.NameIdentifier,userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username)
                //new Claim(ClaimTypes.Name,userFromRepo.)


            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            var cred = new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);
            var tokendescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials= cred
            };

            var tokenhandler = new JwtSecurityTokenHandler();
            var token = tokenhandler.CreateToken(tokendescriptor);
            return Ok( new {
                token= tokenhandler.WriteToken(token)
            });

            
        }
        
    }
}