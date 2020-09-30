using BlazorApp.Server.Controllers;
using BlazorApp.Shared.BLL;
using BlazorApp.Shared.Models;
using BlazorApp.Shared.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace BlazorApp.Server.Tests
{
    public class UserDataControllerTests
    {
        [Theory]
        [InlineData(StatusCodes.Status201Created)]
        [InlineData(StatusCodes.Status400BadRequest)]
        [InlineData(StatusCodes.Status500InternalServerError)]
        public async Task PostTest(int expectedStatus)
        {
            var strongPassword = true;
            Mock<IRepository<UserData>> mockRepoistory = new Mock<IRepository<UserData>>();
            Mock<IPasswordStrengthValidator> mockValidator = new Mock<IPasswordStrengthValidator>();
            Mock<ILogger<UserDataController>> mockLogger = new Mock<ILogger<UserDataController>>();
            var controller = new UserDataController(mockLogger.Object, mockRepoistory.Object, mockValidator.Object);
            var email = "email@test.com";
            var password = "Password";
            var passwordHash = "";
            var request = new AddUserRequest { Email = email, Password = password };

            switch (expectedStatus)
            {
                case StatusCodes.Status201Created:
                    var userData = new UserData { Email = email, PasswordHash = passwordHash };
                    mockRepoistory.Setup(repository => repository.Insert(It.IsAny<UserData>())).Verifiable();
                    break;
                case StatusCodes.Status400BadRequest:
                    strongPassword = false;
                    break;
                case StatusCodes.Status500InternalServerError:
                    mockRepoistory.Setup(repository => repository.Insert(It.IsAny<UserData>())).Throws(new Exception("Already exists"));
                    break;
            }

            mockValidator.Setup(validator => validator.IsStrongPassword(password)).Returns(strongPassword).Verifiable();

            var result = await controller.Post(request);
            Assert.IsAssignableFrom<ActionResult<PostResponse>>(result);
            var objectResult = result.Result as ObjectResult;
            var postResponse = objectResult.Value as PostResponse;
            Assert.Equal(expectedStatus, objectResult.StatusCode);

            switch (expectedStatus)
            {
                case StatusCodes.Status201Created:                    
                    Assert.True(postResponse.Success);
                    Assert.Equal($"User {email} added", postResponse.Message);
                    break;
                case StatusCodes.Status400BadRequest:
                    Assert.False(postResponse.Success);
                    Assert.Equal("Invalid Password", postResponse.Message);
                    break;
                case StatusCodes.Status500InternalServerError:
                    Assert.False(postResponse.Success);
                    Assert.Equal("Already exists", postResponse.Message);
                    break;
            }
            
        }
    }
}
