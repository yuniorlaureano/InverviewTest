using InterviewTest.Entity;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace InterviewTest.Data
{
    public interface IProvinceRepository
    {
        public Task<ProvinceDetail?> GetByIdAsync(long id);
        public Task<IEnumerable<ProvinceDetail>> GetAsync(int page = 1, int pageSize = 10, string? name = null, string? province = null);
        public Task AddAsync(Province Province);
        public Task UpdateAsync(Province Province);
    }

    public class ProvinceRepository : IProvinceRepository
    {
        private readonly IADOCommand _adoCommand;

        public ProvinceRepository(IADOCommand adoCommand)
        {
            _adoCommand = adoCommand;
        }

        public async Task AddAsync(Province province)
        {
            await _adoCommand.ExecuteAsync(async (command) =>
            {
                command.CommandText = @$"
                    INSERT INTO [{nameof(Province)}]
                    {_adoCommand.GenerateInsertColumnsBody(
                       nameof(Province.Name),
                       nameof(Province.CountryId)
                   )}";

                AddParamenters(command, province);
                await command.ExecuteNonQueryAsync();
            });
        }

        public async Task<ProvinceDetail?> GetByIdAsync(long id)
        {
            ProvinceDetail? province = null;
            await _adoCommand.ExecuteAsync(async (command) =>
            {
                command.Parameters.Add(_adoCommand.CreateParam($"@{nameof(Province.Id)}", id, SqlDbType.BigInt));
                command.CommandText = @$"SELECT P.Id, P.[Name], C.[Name] AS Country, P.[CountryId] 
                        FROM [{nameof(Province)}] P 
                        JOIN Country C ON P.CountryId = C.Id
                        WHERE P.{nameof(Province.Id)} = @{nameof(Province.Id)}";

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    province = MapProvinceFromReader(reader);
                }
            });

            return province;
        }

        public async Task<IEnumerable<ProvinceDetail>> GetAsync(int page = 1, int pageSize = 10, string? name = null, string? country = null)
        {
            var provinces = new List<ProvinceDetail>();
            await _adoCommand.ExecuteAsync(async (command) =>
            {
                var filter = _adoCommand.CreateFilterWithCustomParameter(command,
                    new SqlFilterParam("P.[Name]", name, SqlDbType.NVarChar, "ProvinceName"),
                    new SqlFilterParam("C.[Name]", country, SqlDbType.NVarChar, "CountryName")

                 );

                var paging = _adoCommand.CreatePaging(page, pageSize, "P.Id");

                command.CommandText =
                    @$"SELECT P.Id, P.[Name], C.[Name] AS Country, P.[CountryId] FROM [{nameof(Province)}] P
                        JOIN Country C ON P.CountryId = C.Id
                   {filter} {paging}";
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    provinces.Add(MapProvinceFromReader(reader));
                }
            });

            return provinces;
        }

        public async Task UpdateAsync(Province Province)
        {
            await _adoCommand.ExecuteAsync(async (command) =>
            {
                command.CommandText = @$"UPDATE [{nameof(Province)}]
                   SET 
                       {_adoCommand.GenerateUpdateColumnsBody(
                           nameof(Province.Name),
                           nameof(Province.CountryId)
                      )}                    
                   WHERE {nameof(Province.Id)} = @{nameof(Province.Id)}";

                AddParamenters(command, Province);
                command.Parameters.Add(_adoCommand.CreateParam(nameof(Province.Id), Province.Id, SqlDbType.BigInt));
                await command.ExecuteNonQueryAsync();
            });
        }

        private void AddParamenters(IInterviewTestDataBaseCommand command, Province Province)
        {
            command.Parameters.Add(_adoCommand.CreateParam(nameof(Province.Name), Province.Name, SqlDbType.NVarChar));
            command.Parameters.Add(_adoCommand.CreateParam(nameof(Province.CountryId), Province.CountryId, SqlDbType.BigInt));
        }

        private ProvinceDetail MapProvinceFromReader(SqlDataReader reader)
        {
            return new ProvinceDetail
            {
                Id = reader.GetInt64(nameof(ProvinceDetail.Id)),
                Name = reader.GetString(nameof(ProvinceDetail.Name)),
                Country = reader.GetString(nameof(ProvinceDetail.Country)),
                CountryId = reader.GetInt64(nameof(ProvinceDetail.CountryId))
            };
        }
    }
}