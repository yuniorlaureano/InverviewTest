using InterviewTest.Entity;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace InterviewTest.Data
{
    public interface IUserRepository
    {
        public Task<TemporalUser?> GetById(long id);
        Task<TemporalUser?> GetByEmail(string email);
        public Task<IEnumerable<TemporalUser>> Get(int page = 1, int pageSize = 10, byte? age = null, string? country = null);
        public Task Add(TemporalUser user);
        public Task Add(IEnumerable<TemporalUser> user);
        public Task Update(TemporalUser user);
        public Task Delete(long id);
    }

    public class UserRepository : IUserRepository
    {
        private readonly IADOCommand _adoCommand;

        public UserRepository(IADOCommand adoCommand)
        {
            _adoCommand = adoCommand;
        }

        public async Task<IEnumerable<TemporalUser>> Get(int page = 1, int pageSize = 10, byte? age = null, string? country = null)
        {
            var users = new List<TemporalUser>();
            await _adoCommand.Execute(async (command) =>
            {
                var filter = _adoCommand.CreateFilterWithCustomParameter(command,
                    new SqlFilterParam("USR.Age", age, SqlDbType.TinyInt, "Age"),
                    new SqlFilterParam("C.[Name]", country, SqlDbType.NVarChar, "Country")
                 );

                var paging = _adoCommand.CreatePaging(page, pageSize, orderBy: "C.Id");

                command.CommandText = @$"
                        SELECT 
                                USR.[Id]
                               ,USR.[FirstName]
                               ,USR.[LastName]
                               ,USR.[Email]
                               ,USR.[Password]
                               ,USR.[Age]
                               ,USR.[Date]
                               ,C.[Name] AS Country
                               ,P.[Name] AS Province
                               ,CT.[Name] AS City
                        FROM  [User] USR 
                                JOIN Country C ON USR.CountryId = C.Id
                                JOIN Province P ON USR.ProvinceId = P.Id AND C.Id = P.CountryId
                                JOIN City CT ON USR.CityId = CT.Id AND P.Id = CT.ProvinceId
                        {filter} {paging}";
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    users.Add(MapUserFromReader(reader));
                }
            });

            return users;
        }

        public async Task Add(TemporalUser user)
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

                    INSERT INTO [User] (
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

        public async Task Add(IEnumerable<TemporalUser> user)
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
                    nameof(TemporalUser.FirstName),
                    nameof(TemporalUser.LastName),
                    nameof(TemporalUser.Email),
                    nameof(TemporalUser.Age),
                    nameof(TemporalUser.Date),
                    nameof(TemporalUser.Country),
                    nameof(TemporalUser.Province),
                    nameof(TemporalUser.City)
                )}";

                var firstNameParameter = new SqlParameter(nameof(TemporalUser.FirstName), SqlDbType.NVarChar);
                var lastNameParameter = new SqlParameter(nameof(TemporalUser.LastName), SqlDbType.NVarChar);
                var emailParameter = new SqlParameter(nameof(TemporalUser.Email), SqlDbType.NVarChar);
                var ageParameter = new SqlParameter(nameof(TemporalUser.Age), SqlDbType.TinyInt);
                var dateParameter = new SqlParameter(nameof(TemporalUser.Date), SqlDbType.Date);
                var countryParameter = new SqlParameter(nameof(TemporalUser.Country), SqlDbType.NVarChar);
                var proviceParameter = new SqlParameter(nameof(TemporalUser.Province), SqlDbType.NVarChar);
                var cityParameter = new SqlParameter(nameof(TemporalUser.City), SqlDbType.NVarChar);

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

                    INSERT INTO [User] (
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
	                    SELECT 1 FROM [User] U1 WHERE U1.Email = T.Email
                    )
                ";

                await command.ExecuteNonQueryAsync();  
            });
        }

        public async Task Update(TemporalUser user)
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

                    UPDATE [User] SET 
                         [FirstName] = @FirstName
                        ,[LastName] = @LastName
                        ,[Email] = @Email
                        ,[Age] = @Age
                        ,[Date] = @Date
                        ,[CountryId] = @CountryId
                        ,[ProvinceId] = @ProvinceId
                        ,[CityId] = @CityId
                    WHERE {nameof(User.Id)} = @{nameof(User.Id)};
                ";

                command.Parameters.Add(_adoCommand.CreateParam("FirstName ", user.FirstName, SqlDbType.NVarChar));
                command.Parameters.Add(_adoCommand.CreateParam("LastName ", user.LastName, SqlDbType.NVarChar));
                command.Parameters.Add(_adoCommand.CreateParam("Email ", user.Email, SqlDbType.NVarChar));
                command.Parameters.Add(_adoCommand.CreateParam("Age ", user.Age, SqlDbType.TinyInt));
                command.Parameters.Add(_adoCommand.CreateParam("Date ", user.Date, SqlDbType.DateTime));
                command.Parameters.Add(_adoCommand.CreateParam("Country ", user.Country, SqlDbType.NVarChar));
                command.Parameters.Add(_adoCommand.CreateParam("Province ", user.Province, SqlDbType.NVarChar));
                command.Parameters.Add(_adoCommand.CreateParam("City ", user.Country, SqlDbType.NVarChar));
                command.Parameters.Add(_adoCommand.CreateParam("Id ", user.Id, SqlDbType.BigInt));

                await command.ExecuteNonQueryAsync();
            });
        }

        public async Task<TemporalUser?> GetById(long id)
        {
            TemporalUser? user = null;
            await _adoCommand.Execute(async (command) =>
            {
                command.Parameters.Add(_adoCommand.CreateParam($"@{nameof(User.Id)}", id, SqlDbType.BigInt));
                command.CommandText = @$"
                    SELECT
                         USR.[Id]
                        ,USR.[FirstName]
                        ,USR.[LastName]
                        ,USR.[Email]
                        ,USR.[Password]
                        ,USR.[Age]
                        ,USR.[Date]
                        ,C.[Name] AS Country
                        ,P.[Name] AS Province
                        ,CT.[Name] AS City
                    FROM  [User] USR 
                        JOIN Country C ON USR.CountryId = C.Id
                        JOIN Province P ON USR.ProvinceId = P.Id AND C.Id = P.CountryId
                        JOIN City CT ON USR.CityId = CT.Id AND P.Id = CT.ProvinceId
                    WHERE USR.{nameof(User.Id)} = @{nameof(User.Id)}";

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    user = MapUserFromReader(reader);
                }
            });

            return user;
        }

        public async Task<TemporalUser?> GetByEmail(string email)
        {
            TemporalUser? user = null;
            await _adoCommand.Execute(async (command) =>
            {
                command.Parameters.Add(_adoCommand.CreateParam($"@{nameof(User.Email)}", email, SqlDbType.NVarChar));
                command.CommandText = @$"
                        SELECT 
                         USR.[Id]
                        ,USR.[FirstName]
                        ,USR.[LastName]
                        ,USR.[Email]
                        ,USR.[Password]
                        ,USR.[Age]
                        ,USR.[Date]
                        ,C.[Name] AS Country
                        ,P.[Name] AS Province
                        ,CT.[Name] AS City
                    FROM  [User] USR 
                        JOIN Country C ON USR.CountryId = C.Id
                        JOIN Province P ON USR.ProvinceId = P.Id AND C.Id = P.CountryId
                        JOIN City CT ON USR.CityId = CT.Id AND P.Id = CT.ProvinceId
                    WHERE USR.{nameof(User.Email)} = @{nameof(User.Email)}";

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    user = MapUserFromReader(reader);
                    user.Password = reader.GetString(nameof(User.Password));
                }
            });

            return user;
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

        private void AddParamenters(SqlCommand command, TemporalUser user)
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

        private TemporalUser MapUserFromReader(SqlDataReader reader)
        {
            return new TemporalUser
            {
                Id = reader.GetInt64(nameof(TemporalUser.Id)),
                FirstName = reader.GetString(nameof(TemporalUser.FirstName)),
                LastName = reader.GetString(nameof(TemporalUser.LastName)),
                Email = reader.GetString(nameof(TemporalUser.Email)),
                Age = reader.GetByte(nameof(TemporalUser.Age)),
                Date = reader.GetDateTime(nameof(TemporalUser.Date)),
                Country = reader.GetString(nameof(TemporalUser.Country)),
                Province = reader.GetString(nameof(TemporalUser.Province)),
                City = reader.GetString(nameof(TemporalUser.City))
            };
        }
    }
}