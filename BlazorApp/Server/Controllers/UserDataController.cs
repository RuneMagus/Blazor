using BlazorApp.Shared.BLL;
using BlazorApp.Shared.Models;
using BlazorApp.Shared.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Mime;
using System.Threading.Tasks;

namespace BlazorApp.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserDataController : ControllerBase
    {
        private readonly IRepository<UserData> repository;
        private readonly IPasswordStrengthValidator passwordStrengthValidator;
        private readonly ILogger<UserDataController> logger;

        public UserDataController(ILogger<UserDataController> logger,
            IRepository<UserData> repository,
            IPasswordStrengthValidator passwordStrengthValidator)
        {
            this.logger = logger;
            this.repository = repository;
            this.passwordStrengthValidator = passwordStrengthValidator;
        }

        // POST <UserDataController>
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] 
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PostResponse>> Post([FromBody] AddUserRequest value)
        {
            try
            {
                if (passwordStrengthValidator.IsStrongPassword(value.Password))
                {
                    var passwordHash = BCrypt.Net.BCrypt.HashPassword(value.Password);
                    var userData = new UserData { Email = value.Email, PasswordHash = passwordHash };
                    await repository.Insert(userData);
                    return StatusCode(StatusCodes.Status201Created, new PostResponse { Success = true, Message = $"User {value.Email} added" });
                }
                else
                {
                    return BadRequest(new PostResponse { Success = false, Message = "Invalid Password" });
                }
            }
            catch (Exception exception)
            {
                // ToDo: make the error messages more end user friendly
                return StatusCode(StatusCodes.Status500InternalServerError, new PostResponse { Success = false, Message = exception.Message });
            }
        }
    }
}
