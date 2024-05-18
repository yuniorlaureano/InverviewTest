using InterviewTest.Entity;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace InterviewTest.Data
{
    //ToDo: Add serilog 
    //ToDo: Add fluent validation
    //ToDo: Add pagination
    //ToDo: Add authentication
    //ToDo: Add the inventory part
    //ToDo: Try to may things a little more generic
    public interface IUserRepository
    {
        public Task<User?> Get(long id);
        public Task<IEnumerable<User>> Get(byte? age, string? country);
        public Task Add(User user);
        public Task Add(IEnumerable<User> user);
        public Task Update(User user);
        public Task Delete(long id);
    }
    public class UserRepository : IUserRepository
    {
        private readonly ISqlFactory _sqlFactory;
        public UserRepository(ISqlFactory sqlFactory)
        {
            _sqlFactory = sqlFactory;
        }

        public async Task Add(User user)
        {
            using var connection = await _sqlFactory.GetConnection();

            using var command = connection.CreateCommand();
            command.CommandText = @$"INSERT INTO [{nameof(User)}](
                   {nameof(User.FirstName)},
                   {nameof(User.LastName)},
                   {nameof(User.Age)},
                   {nameof(User.Date)},
                   {nameof(User.Country)},
                   {nameof(User.Province)},
                   {nameof(User.City)}
                ) VALUES(
                   @{nameof(User.FirstName)},
                   @{nameof(User.LastName)},
                   @{nameof(User.Age)},
                   @{nameof(User.Date)},
                   @{nameof(User.Country)},
                   @{nameof(User.Province)},
                   @{nameof(User.City)}
                )";

            AddParamenters(command, user);

            await command.ExecuteNonQueryAsync();
        }

        public async Task Add(IEnumerable<User> user)
        {
            using var connection = await _sqlFactory.GetConnection();
            using SqlTransaction transaction = (SqlTransaction)await connection.BeginTransactionAsync();

            using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = @$"INSERT INTO [{nameof(User)}](
               {nameof(User.FirstName)},
               {nameof(User.LastName)},
               {nameof(User.Age)},
               {nameof(User.Date)},
               {nameof(User.Country)},
               {nameof(User.Province)},
               {nameof(User.City)}
            ) VALUES(
               @{nameof(User.FirstName)},
               @{nameof(User.LastName)},
               @{nameof(User.Age)},
               @{nameof(User.Date)},
               @{nameof(User.Country)},
               @{nameof(User.Province)},
               @{nameof(User.City)}
            )";

            var firstNameParameter = new SqlParameter(nameof(User.FirstName), SqlDbType.NVarChar);
            var lastNameParameter = new SqlParameter(nameof(User.LastName), SqlDbType.NVarChar);
            var ageParameter = new SqlParameter(nameof(User.Age), SqlDbType.TinyInt);
            var dateParameter = new SqlParameter(nameof(User.Date), SqlDbType.Date);
            var countryParameter = new SqlParameter(nameof(User.Country), SqlDbType.NVarChar);
            var proviceParameter = new SqlParameter(nameof(User.Province), SqlDbType.NVarChar);
            var cityParameter = new SqlParameter(nameof(User.City), SqlDbType.NVarChar);

            command.Parameters.AddRange(new[]
            {
                firstNameParameter,
                lastNameParameter,
                ageParameter,
                dateParameter,
                countryParameter,
                proviceParameter,
                cityParameter
            });

            try
            {
                foreach (var item in user)
                {
                    firstNameParameter.Value = item.FirstName;
                    lastNameParameter.Value = item.LastName;
                    ageParameter.Value = item.Age;
                    dateParameter.Value = item.Date;
                    countryParameter.Value = item.Country;
                    proviceParameter.Value = item.Province;
                    cityParameter.Value = item.City;

                    await command.ExecuteNonQueryAsync();
                }
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                try
                {
                    await transaction.RollbackAsync();
                }
                catch (SqlException)
                {
                    //todo log this and all the other
                }
            }

            await command.ExecuteNonQueryAsync();
        }

        public async Task Delete(long id)
        {
            using var connection = await _sqlFactory.GetConnection();
            using var command = connection.CreateCommand();
            command.Parameters.Add(_sqlFactory.CreateParam($"@{nameof(User.Id)}", id, SqlDbType.BigInt));
            command.CommandText = $"DELETE FROM [{nameof(User)}] WHERE Id = @Id";

            await command.ExecuteNonQueryAsync();
        }

        public async Task<User?> Get(long id)
        {
            using var connection = await _sqlFactory.GetConnection();
            using var command = connection.CreateCommand();
            command.Parameters.Add(_sqlFactory.CreateParam($"@{nameof(User.Id)}", id, SqlDbType.BigInt));
            command.CommandText = $"SELECT * FROM  [{nameof(User)}] WHERE {nameof(User.Id)} = @{nameof(User.Id)}";

            using var reader = await command.ExecuteReaderAsync();
            User? user = null;
            while (await reader.ReadAsync())
            {
                user = MapUserFromReader(reader);
            }

            return user;
        }

        public async Task<IEnumerable<User>> Get(byte? age, string? country)
        {
            using var connection = await _sqlFactory.GetConnection();
            using var command = connection.CreateCommand();

            var where = new StringBuilder();
            if (age is not null)
            {
                where.Append($"{nameof(User.Age)} = @{nameof(User.Age)} ");
                where.Append(" AND ");
                command.Parameters.Add(_sqlFactory.CreateParam($"@{nameof(User.Age)}", age, SqlDbType.TinyInt));
            }

            if (country is not null)
            {
                where.Append($"{nameof(User.Country)} = @{nameof(User.Country)} ");
                where.Append(" AND ");
                command.Parameters.Add(_sqlFactory.CreateParam($"@{nameof(User.Country)}", country, SqlDbType.NVarChar));
            }

            if (where.Length > 0)
            {
                where.Remove(where.Length - 4, 4);
                where.Insert(0, "WHERE ");
            }

            command.CommandText = @$"
                SELECT * FROM  [{nameof(User)}] {where.ToString()}";

            using var reader = await command.ExecuteReaderAsync();
            var users = new List<User>();
            while (await reader.ReadAsync())
            {
                users.Add(MapUserFromReader(reader));
            }

            return users;
        }

        public async Task Update(User user)
        {
            using var connection = await _sqlFactory.GetConnection();
            using var command = connection.CreateCommand();
            command.CommandText = @$"UPDATE [{nameof(User)}]
               SET {nameof(User.FirstName)} = @{nameof(User.FirstName)},
                   {nameof(User.LastName)} = @{nameof(User.LastName)},
                   {nameof(User.Age)} = @{nameof(User.Age)},
                   {nameof(User.Date)} = @{nameof(User.Date)},
                   {nameof(User.Country)} = @{nameof(User.Country)},
                   {nameof(User.Province)} = @{nameof(User.Province)},
                   {nameof(User.City)} = @{nameof(User.City)}
               WHERE {nameof(User.Id)} = @{nameof(User.Id)}";

            AddParamenters(command, user);
            command.Parameters.Add(_sqlFactory.CreateParam(nameof(user.Id), user.Id, SqlDbType.BigInt));
            await command.ExecuteNonQueryAsync();
        }

        private void AddParamenters(SqlCommand command, User user)
        {
            command.Parameters.Add(_sqlFactory.CreateParam(nameof(user.FirstName), user.FirstName, SqlDbType.NVarChar));
            command.Parameters.Add(_sqlFactory.CreateParam(nameof(user.LastName), user.LastName, SqlDbType.NVarChar));
            command.Parameters.Add(_sqlFactory.CreateParam(nameof(user.Age), user.Age, SqlDbType.TinyInt));
            command.Parameters.Add(_sqlFactory.CreateParam(nameof(user.Date), user.Date, SqlDbType.DateTime));
            command.Parameters.Add(_sqlFactory.CreateParam(nameof(user.Country), user.Country, SqlDbType.NVarChar));
            command.Parameters.Add(_sqlFactory.CreateParam(nameof(user.Province), user.Province, SqlDbType.NVarChar));
            command.Parameters.Add(_sqlFactory.CreateParam(nameof(user.City), user.City, SqlDbType.NVarChar));
        }

        private User MapUserFromReader(SqlDataReader reader)
        {
            return new User
            {
                Id = reader.GetInt64(nameof(User.Id)),
                FirstName = reader.GetString(nameof(User.FirstName)),
                LastName = reader.GetString(nameof(User.LastName)),
                Age = reader.GetByte(nameof(User.Age)),
                Date = reader.GetDateTime(nameof(User.Date)),
                Country = reader.GetString(nameof(User.Country)),
                Province = reader.GetString(nameof(User.Province)),
                City = reader.GetString(nameof(User.City))
            };
        }
    }
}