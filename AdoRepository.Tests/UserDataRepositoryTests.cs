using BlazorApp.Shared.Models;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AdoRepository.Tests
{
    public class UserDataRepositoryTests
    {
        [Fact]
        public async Task FirstInsertShouldPassSecondWithSameEmailShouldFailTest()
        {
            var databaseName = "BlazorTest";
            using (var database = new TestDatabase(databaseName))
            {
                var connectionString = $"Server=(local)\\SQLEXPRESS;Integrated Security=SSPI;Initial Catalog={databaseName};MultipleActiveResultSets=True";
                var repository = new UserDataRepository(connectionString);
                var expected = new UserData { Email = "email", PasswordHash = "hash" };
                await repository.Insert(expected);
                var users = database.GetUserData(connectionString);
                var actual = users.Single();
                Assert.Equal(expected.Email, actual.Email);
                Assert.Equal(expected.PasswordHash, actual.PasswordHash);
                Func<Task> action = () => repository.Insert(new UserData { Email = "email", PasswordHash = "new hash" });
                var exception = await Assert.ThrowsAsync<SqlException>(action);
                Assert.Equal(@"Violation of UNIQUE KEY constraint 'UC_User'. Cannot insert duplicate key in object 'dbo.UserData'. The duplicate key value is (email).
The statement has been terminated.", exception.Message);
            }
        }

        [Fact]
        public async Task AllowsSecondInsertWithDifferentEmailTest()
        {
            var databaseName = "BlazorTest";
            using (var database = new TestDatabase(databaseName))
            {
                var connectionString = $"Server=(local)\\SQLEXPRESS;Integrated Security=SSPI;Initial Catalog={databaseName};MultipleActiveResultSets=True";
                var repository = new UserDataRepository(connectionString);
                var expected = new UserData { Email = "email", PasswordHash = "hash" };
                var expected2 = new UserData { Email = "email2", PasswordHash = "hash" };
                await repository.Insert(expected);
                await repository.Insert(expected2);
                var users = database.GetUserData(connectionString);
                Assert.Equal(2, users.Count);
                Assert.Equal(expected.Email, users[0].Email);
                Assert.Equal(expected.PasswordHash, users[0].PasswordHash);
                Assert.Equal(expected2.Email, users[1].Email);
                Assert.Equal(expected2.PasswordHash, users[1].PasswordHash);
            }
        }
    }
}
