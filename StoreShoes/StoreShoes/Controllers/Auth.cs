using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreShoes.Model;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using StoreShoes.Model;

namespace GameStore1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Auth : Controller
    {
        private DbshoesContext _context;
        public Auth(DbshoesContext context)
        {
            _context = context;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Login == loginModel.Login);

            if (user == null || loginModel.Password != user.Password)
            {
                return Unauthorized("Неверный логин или пароль");
            }


            return Ok("Успешно");

        }
        [HttpPost("registration")]
        public async Task<IActionResult> Registration([FromBody] RegistrationModel registerModel)
        {
            var user = await _context.Users.AnyAsync(u => u.Login == registerModel.Login);
            if (user)
            {
                return Conflict("Пользователь существует");
            }

            var newUser = new User
            {
                Name = registerModel.Name,
                Login = registerModel.Login,
                Password = registerModel.Password,
                Role = "customer",
                Email = registerModel.Email
            };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok($"{newUser.Name}, успешно зарегестрировался(-лась)");
        }

        public class LoginModel
        {
            public string Login { get; set; }
            public string Password { get; set; }
        }

        public class RegistrationModel
        {
            [Required]
            public string Name { get; set; }
            [Required]
            public string Login { get; set; }

            [Required]
            [MinLength(3)]
            public string Password { get; set; }
            [Required]
            public string Email { get; set; }

        }

    }
}
