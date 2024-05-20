using InterviewTest.Entity;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace InterviewTest.Data
{
    public interface IUserTRepository
    {
        public Task<User1?> GetById(long id);
        Task<User1?> GetByEmail(string email);
        public Task<IEnumerable<User1>> Get(int page = 1, int pageSize = 10, byte? age = null, string? country = null);
        public Task Add(User user);
        public Task Add(IEnumerable<User> user);
        public Task Update(User user);
        public Task Delete(long id);
    }

    public class UserTRepository : IUserTRepository
    {
        private readonly IADOCommand _adoCommand;

        public UserTRepository(IADOCommand adoCommand)
        {
            _adoCommand = adoCommand;
        }

        public async Task<IEnumerable<User1>> Get(int page = 1, int pageSize = 10, byte? age = null, string? country = null)
        {
            var users = new List<User1>();
            await _adoCommand.Execute(async (command) =>
            {
                var filter = _adoCommand.CreateFilter1(command,
                    new SqlFilterParam(nameof(User.Age), age, SqlDbType.TinyInt, "nameof(User.Age)"),
                    new SqlFilterParam($"C.[Name]", country, SqlDbType.NVarChar, "Country")
                 );

                var paging = _adoCommand.CreatePaging(page, pageSize);

                command.CommandText = $"SELECT USR.* FROM  User1 USR JOIN Country C ON USR.CountryId = C.Id {filter} {paging}";
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    users.Add(MapUserFromReader(reader));
                }
            });

            return users;
        }

        public async Task Add(User user)
        {
            await _adoCommand.ExecuteTransaction(async (command, onQuery) =>
            {
                command.CommandText = @"
                    DECLARE 
	                    @CountryId BIGINT,
	                    @ProvinceId BIGINT,
	                    @CityId BIGINT;

                    SELECT TOP 1 @CountryId = Id  FROM Country WHERE [Name] = @Country;
                    IF @CountryId IS NULL
                    BEGIN
	                    INSERT INTO Country([Name]) VALUES(@Country)
	                    SET @CountryId = SCOPE_IDENTITY()
                    END

                    SELECT TOP 1 @ProvinceId = Id  FROM Province WHERE [Name] = @Province AND CountryId = @CountryId;
                    IF @ProvinceId IS NULL
                    BEGIN
	                    INSERT INTO Province([Name], CountryId) VALUES(@Province, @CountryId)
	                    SET @ProvinceId = SCOPE_IDENTITY()
                    END

                    SELECT TOP 1 @CityId = Id  FROM City WHERE [Name] = @City AND ProvinceId = @ProvinceId;
                    IF @CityId IS NULL
                    BEGIN
	                    INSERT INTO City([Name], ProvinceId) VALUES(@City, @ProvinceId)
	                    SET @CityId = SCOPE_IDENTITY()
                    END

                    INSERT INTO [User1] (
                         [FirstName]
                        ,[LastName]
                        ,[Email]
                        ,[Password]
                        ,[Age]
                        ,[Date]
                        ,[CountryId]
                        ,[ProvinceId]
                        ,[CityId] )
                    VALUES (
	                     @FirstName, 
	                     @LastName, 
	                     @Email, 
	                     @Password, 
	                     @Age, 
	                     @Date, 
	                     @CountryId,
	                     @ProvinceId,
	                     @CityId
                    )
                ";

                command.Parameters.Add(_adoCommand.CreateParam("FirstName ", user.FirstName, SqlDbType.NVarChar));
                command.Parameters.Add(_adoCommand.CreateParam("LastName ", user.LastName, SqlDbType.NVarChar));
                command.Parameters.Add(_adoCommand.CreateParam("Email ", user.Email, SqlDbType.NVarChar));
                command.Parameters.Add(_adoCommand.CreateParam("Password ", user.Password, SqlDbType.NVarChar));
                command.Parameters.Add(_adoCommand.CreateParam("Age ", user.Age, SqlDbType.TinyInt));
                command.Parameters.Add(_adoCommand.CreateParam("Date ", user.Date, SqlDbType.DateTime));
                command.Parameters.Add(_adoCommand.CreateParam("Country ", user.Country, SqlDbType.NVarChar));
                command.Parameters.Add(_adoCommand.CreateParam("Province ", user.Province, SqlDbType.NVarChar));
                command.Parameters.Add(_adoCommand.CreateParam("City ", user.Country, SqlDbType.NVarChar));

                await command.ExecuteNonQueryAsync();
            });
        }

        public async Task Add(IEnumerable<User> user)
        {
            await _adoCommand.ExecuteTransaction(async (command, onQuery) =>
            {
                command.CommandText = @"
                    CREATE TABLE #TempUser (
	                    Id bigint not null identity,
                        FirstName nvarchar(50) not null,
                        LastName nvarchar(50) not null,
                        Email nvarchar(50) not null,
                        [Password] nvarchar(256) null,
                        Age tinyint not null,
                        [Date] DateTime,
                        Country nvarchar(50),
                        Province nvarchar(50),
                        City nvarchar(50)
                    );
                ";

                await command.ExecuteNonQueryAsync();

                command.CommandText = @$"INSERT INTO #TempUser
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

                command.Parameters.Clear();
                command.CommandText = @"
                    INSERT INTO Country ([Name])
	                    SELECT DISTINCT Country FROM #TempUser WHERE Country NOT IN (SELECT [Name] from Country)

                    INSERT INTO Province ([Name], CountryId)
	                    SELECT DISTINCT T.Province, C.Id 
	                    FROM #TempUser T JOIN Country C ON T.Country = C.[Name]
	                    WHERE T.Province NOT IN (SELECT [Name] from Province)

                    INSERT INTO City ([Name], ProvinceId)
	                    SELECT DISTINCT City, P.Id
	                    FROM #TempUser T JOIN Province P ON P.[Name] = T.Province
	                    WHERE City NOT IN (SELECT [Name] from City)

                    INSERT INTO [User1] (
	                     [FirstName]
                        ,[LastName]
                        ,[Email]
                        ,[Password]
                        ,[Age]
                        ,[Date]
                        ,[CountryId]
                        ,[ProvinceId]
                        ,[CityId] )
                    SELECT DISTINCT 
	                    T.FirstName, 
	                    T.LastName, 
	                    T.Email, 
	                    T.[Password], 
	                    T.Age, 
	                    T.[Date], 
	                    C.Id,
	                    P.Id,
	                    CT.Id
                    FROM #TempUser T 
		                    JOIN Country C ON C.[Name] = T.Country
		                    JOIN Province P ON P.[Name] = T.Province AND P.CountryId = C.Id
		                    JOIN City CT ON CT.[Name] = T.City AND CT.ProvinceId = P.Id
                    WHERE NOT EXISTS(
	                    SELECT 1 FROM [User1] U1 WHERE U1.Email = T.Email
                    )
                ";

                await command.ExecuteNonQueryAsync();  
            });
        }

        public async Task Update(User user)
        {
            await _adoCommand.ExecuteTransaction(async (command, onQuery) =>
            {
                command.CommandText = @$"
                    DECLARE 
	                    @CountryId BIGINT,
	                    @ProvinceId BIGINT,
	                    @CityId BIGINT;

                    SELECT TOP 1 @CountryId = Id  FROM Country WHERE [Name] = @Country;
                    IF @CountryId IS NULL
                    BEGIN
	                    INSERT INTO Country([Name]) VALUES(@Country)
	                    SET @CountryId = SCOPE_IDENTITY()
                    END

                    SELECT TOP 1 @ProvinceId = Id  FROM Province WHERE [Name] = @Province AND CountryId = @CountryId;
                    IF @ProvinceId IS NULL
                    BEGIN
	                    INSERT INTO Province([Name], CountryId) VALUES(@Province, @CountryId)
	                    SET @ProvinceId = SCOPE_IDENTITY()
                    END

                    SELECT TOP 1 @CityId = Id  FROM City WHERE [Name] = @City AND ProvinceId = @ProvinceId;
                    IF @CityId IS NULL
                    BEGIN
	                    INSERT INTO City([Name], ProvinceId) VALUES(@City, @ProvinceId)
	                    SET @CityId = SCOPE_IDENTITY()
                    END

                    UPDATE [User1] SET 
                         [FirstName] = @FirstName
                        ,[LastName] = @LastName
                        ,[Email] = @Email
                        ,[Password] = @Password
                        ,[Age] = @Age
                        ,[Date] = @Date
                        ,[CountryId] = @CountryId
                        ,[ProvinceId] = @ProvinceId
                        ,[CityId] = @CityId
                    WHERE {nameof(User1.Id)} = @{nameof(User1.Id)}"";
                ";

                command.Parameters.Add(_adoCommand.CreateParam("FirstName ", user.FirstName, SqlDbType.NVarChar));
                command.Parameters.Add(_adoCommand.CreateParam("LastName ", user.LastName, SqlDbType.NVarChar));
                command.Parameters.Add(_adoCommand.CreateParam("Email ", user.Email, SqlDbType.NVarChar));
                command.Parameters.Add(_adoCommand.CreateParam("Password ", user.Password, SqlDbType.NVarChar));
                command.Parameters.Add(_adoCommand.CreateParam("Age ", user.Age, SqlDbType.TinyInt));
                command.Parameters.Add(_adoCommand.CreateParam("Date ", user.Date, SqlDbType.DateTime));
                command.Parameters.Add(_adoCommand.CreateParam("Country ", user.Country, SqlDbType.NVarChar));
                command.Parameters.Add(_adoCommand.CreateParam("Province ", user.Province, SqlDbType.NVarChar));
                command.Parameters.Add(_adoCommand.CreateParam("City ", user.Country, SqlDbType.NVarChar));
                command.Parameters.Add(_adoCommand.CreateParam("Id ", user.Id, SqlDbType.BigInt));

                await command.ExecuteNonQueryAsync();
            });
        }

        public async Task<User1?> GetById(long id)
        {
            User1? user = null;
            await _adoCommand.Execute(async (command) =>
            {
                command.Parameters.Add(_adoCommand.CreateParam($"@{nameof(User1.Id)}", id, SqlDbType.BigInt));
                command.CommandText = $"SELECT * FROM  [{nameof(User1)}] WHERE {nameof(User1.Id)} = @{nameof(User1.Id)}";

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    user = MapUserFromReader(reader);
                }
            });

            return user;
        }

        public async Task<User1?> GetByEmail(string email)
        {
            User1? user = null;
            await _adoCommand.Execute(async (command) =>
            {
                command.Parameters.Add(_adoCommand.CreateParam($"@{nameof(User1.Email)}", email, SqlDbType.NVarChar));
                command.CommandText = $"SELECT * FROM  [{nameof(User1)}] WHERE {nameof(User1.Email)} = @{nameof(User1.Email)}";

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    user = MapUserFromReader(reader);
                    user.Password = reader.GetString(nameof(User1.Password));
                }
            });

            return user;
        }

        public async Task Delete(long id)
        {
            await _adoCommand.Execute(async (command) =>
            {
                command.Parameters.Add(_adoCommand.CreateParam($"@{nameof(User1.Id)}", id, SqlDbType.BigInt));
                command.CommandText = $"DELETE FROM [{nameof(User1)}] WHERE Id = @Id";
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

        private User1 MapUserFromReader(SqlDataReader reader)
        {
            return new User1
            {
                Id = reader.GetInt64(nameof(User1.Id)),
                FirstName = reader.GetString(nameof(User1.FirstName)),
                LastName = reader.GetString(nameof(User1.LastName)),
                Email = reader.GetString(nameof(User1.Email)),
                Age = reader.GetByte(nameof(User1.Age)),
                Date = reader.GetDateTime(nameof(User1.Date)),

                CountryId = reader.GetInt64(nameof(User1.CountryId)),
                ProvinceId = reader.GetInt64(nameof(User1.ProvinceId)),
                CityId = reader.GetInt64(nameof(User1.CityId))
            };
        }
    }
}