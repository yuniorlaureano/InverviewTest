using InterviewTest.Entity;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace InterviewTest.Data
{
    public interface IUserRepository
    {
        public Task<User?> GetById(long id);
        Task<User?> GetByEmail(string email);
        public Task<IEnumerable<User>> Get(int page = 1, int pageSize = 10, byte? age = null, string? country = null);
        public Task Add(User user);
        public Task Add(IEnumerable<User> user);
        public Task Update(User user);
        public Task Delete(long id);
    }

    public class UserRepository : IUserRepository
    {
        private readonly IADOCommand _adoCommand;

        public UserRepository(IADOCommand adoCommand)
        {
            _adoCommand = adoCommand;
        }

        public async Task Add(User user)
        {
            await _adoCommand.Execute(async (command) =>
            {
                command.CommandText = @$"INSERT INTO [{nameof(User)}]
                    {_adoCommand.GenerateInsertColumnsBody(
                       nameof(User.FirstName),
                       nameof(User.LastName),
                       nameof(User.Email),
                       nameof(User.Password),
                       nameof(User.Age),
                       nameof(User.Date),
                       nameof(User.Country),
                       nameof(User.Province),
                       nameof(User.City)
                   )}";

                AddParamenters(command, user);
                command.Parameters.Add(_adoCommand.CreateParam(nameof(user.Password), user.Password, SqlDbType.NVarChar));
                await command.ExecuteNonQueryAsync();
            });
        }

        public async Task Add(IEnumerable<User> user)
        {
            await _adoCommand.ExecuteTransaction(async (command, onQuery) =>
            {
                command.CommandText = @$"INSERT INTO [{nameof(User)}]
                {_adoCommand.GenerateInsertColumnsBody(
                    nameof(User.FirstName),
                    nameof(User.LastName),
                    nameof(User.Email),
                    nameof(User.Age),
                    nameof(User.Date),
                    nameof(User.Country),
                    nameof(User.Province),
                    nameof(User.City)
                )}";

                var firstNameParameter = new SqlParameter(nameof(User.FirstName), SqlDbType.NVarChar);
                var lastNameParameter = new SqlParameter(nameof(User.LastName), SqlDbType.NVarChar);
                var emailParameter = new SqlParameter(nameof(User.Email), SqlDbType.NVarChar);
                var ageParameter = new SqlParameter(nameof(User.Age), SqlDbType.TinyInt);
                var dateParameter = new SqlParameter(nameof(User.Date), SqlDbType.Date);
                var countryParameter = new SqlParameter(nameof(User.Country), SqlDbType.NVarChar);
                var proviceParameter = new SqlParameter(nameof(User.Province), SqlDbType.NVarChar);
                var cityParameter = new SqlParameter(nameof(User.City), SqlDbType.NVarChar);

                command.Parameters.AddRange(new[]
                {
                    firstNameParameter,
                    lastNameParameter,
                    emailParameter,
                    ageParameter,
                    dateParameter,
                    countryParameter,
                    proviceParameter,
                    cityParameter
                });

                foreach (var item in user)
                {
                    firstNameParameter.Value = item.FirstName;
                    lastNameParameter.Value = item.LastName;
                    emailParameter.Value = item.Email;
                    ageParameter.Value = item.Age;
                    dateParameter.Value = item.Date;
                    countryParameter.Value = item.Country;
                    proviceParameter.Value = item.Province;
                    cityParameter.Value = item.City;
                    onQuery();
                    await command.ExecuteNonQueryAsync();
                }
            });
        }

        public async Task Delete(long id)
        {
            await _adoCommand.Execute(async (command) =>
            {
                command.Parameters.Add(_adoCommand.CreateParam($"@{nameof(User.Id)}", id, SqlDbType.BigInt));
                command.CommandText = $"DELETE FROM [{nameof(User)}] WHERE Id = @Id";
                await command.ExecuteNonQueryAsync();
            });
        }

        public async Task<User?> GetById(long id)
        {
            User? user = null;
            await _adoCommand.Execute(async (command) =>
            {
                command.Parameters.Add(_adoCommand.CreateParam($"@{nameof(User.Id)}", id, SqlDbType.BigInt));
                command.CommandText = $"SELECT * FROM  [{nameof(User)}] WHERE {nameof(User.Id)} = @{nameof(User.Id)}";

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    user = MapUserFromReader(reader);
                }
            });

            return user;
        }

        public async Task<User?> GetByEmail(string email)
        {
            User? user = null;
            await _adoCommand.Execute(async (command) =>
            {
                command.Parameters.Add(_adoCommand.CreateParam($"@{nameof(User.Email)}", email, SqlDbType.NVarChar));
                command.CommandText = $"SELECT * FROM  [{nameof(User)}] WHERE {nameof(User.Email)} = @{nameof(User.Email)}";

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    user = MapUserFromReader(reader);
                    user.Password = reader.GetString(nameof(User.Password));
                }
            });

            return user;
        }

        public async Task<IEnumerable<User>> Get(int page = 1, int pageSize = 10, byte? age = null, string? country = null)
        {
            var users = new List<User>();
            await _adoCommand.Execute(async (command) =>
            {
                var filter = _adoCommand.CreateFilter(command,
                    new SqlFilterParam(nameof(User.Age), age, SqlDbType.TinyInt),
                    new SqlFilterParam(nameof(User.Country), country, SqlDbType.NVarChar)
                 );

                var paging = _adoCommand.CreatePaging(page, pageSize);

                command.CommandText = $"SELECT * FROM  [{nameof(User)}] {filter} {paging}";
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    users.Add(MapUserFromReader(reader));
                }
            });

            return users;
        }

        public async Task Update(User user)
        {
            await _adoCommand.Execute(async (command) =>
            {
                command.CommandText = @$"UPDATE [{nameof(User)}]
                   SET 
                       {_adoCommand.GenerateUpdateColumnsBody(
                           nameof(User.FirstName),
                           nameof(User.LastName),
                           nameof(User.Age),
                           nameof(User.Email),
                           nameof(User.Date),
                           nameof(User.Country),
                           nameof(User.Province),
                           nameof(User.City)
                      )}                    
                   WHERE {nameof(User.Id)} = @{nameof(User.Id)}";

                    AddParamenters(command, user);
                    command.Parameters.Add(_adoCommand.CreateParam(nameof(user.Id), user.Id, SqlDbType.BigInt));
                    await command.ExecuteNonQueryAsync();
            });
        }

        private void AddParamenters(SqlCommand command, User user)
        {
            command.Parameters.Add(_adoCommand.CreateParam(nameof(user.FirstName), user.FirstName, SqlDbType.NVarChar));
            command.Parameters.Add(_adoCommand.CreateParam(nameof(user.LastName), user.LastName, SqlDbType.NVarChar));
            command.Parameters.Add(_adoCommand.CreateParam(nameof(user.Email), user.Email, SqlDbType.NVarChar));
            command.Parameters.Add(_adoCommand.CreateParam(nameof(user.Age), user.Age, SqlDbType.TinyInt));
            command.Parameters.Add(_adoCommand.CreateParam(nameof(user.Date), user.Date, SqlDbType.DateTime));
            command.Parameters.Add(_adoCommand.CreateParam(nameof(user.Country), user.Country, SqlDbType.NVarChar));
            command.Parameters.Add(_adoCommand.CreateParam(nameof(user.Province), user.Province, SqlDbType.NVarChar));
            command.Parameters.Add(_adoCommand.CreateParam(nameof(user.City), user.City, SqlDbType.NVarChar));
        }

        private User MapUserFromReader(SqlDataReader reader)
        {
            return new User
            {
                Id = reader.GetInt64(nameof(User.Id)),
                FirstName = reader.GetString(nameof(User.FirstName)),
                LastName = reader.GetString(nameof(User.LastName)),
                Email = reader.GetString(nameof(User.Email)),
                Age = reader.GetByte(nameof(User.Age)),
                Date = reader.GetDateTime(nameof(User.Date)),
                Country = reader.GetString(nameof(User.Country)),
                Province = reader.GetString(nameof(User.Province)),
                City = reader.GetString(nameof(User.City))
            };
        }
    }
}