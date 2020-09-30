using BlazorApp.Shared.Models;
using BlazorApp.Shared.Repository;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace AdoRepository
{
    public class UserDataRepository : IRepository<UserData>
    {
        private readonly string connectionString;

        public UserDataRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task Insert(UserData entity)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "INSERT INTO UserData (Email, PasswordHash) Values (@Email, @PasswordHash)";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;

                    // adding parameters
                    SqlParameter parameter = new SqlParameter
                    {
                        ParameterName = "@Email",
                        Value = entity.Email,
                        SqlDbType = SqlDbType.NVarChar,
                        Size = 320
                    };
                    command.Parameters.Add(parameter);

                    parameter = new SqlParameter
                    {
                        ParameterName = "@PasswordHash",
                        Value = entity.PasswordHash,
                        SqlDbType = SqlDbType.NVarChar,
                        Size = 72
                    };
                    command.Parameters.Add(parameter);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                    await connection.CloseAsync();
                }
            }
        }
    }
}
