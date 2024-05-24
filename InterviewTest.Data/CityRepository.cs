using InterviewTest.Data.Decorators;
using InterviewTest.Data.Interfaces;
using InterviewTest.Entity;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace InterviewTest.Data
{
    public class CityRepository : ICityRepository
    {
        private readonly IADOCommand _adoCommand;

        public CityRepository(IADOCommand adoCommand)
        {
            _adoCommand = adoCommand;
        }

        public async Task AddAsync(City city)
        {
            await _adoCommand.ExecuteAsync(async (command) =>
            {
                command.CommandText = @$"
                    INSERT INTO [{nameof(City)}]
                    {_adoCommand.GenerateInsertColumnsBody(
                       nameof(City.Name),
                       nameof(City.ProvinceId)
                   )}";

                AddParamenters(command, city);
                await command.ExecuteNonQueryAsync();
            });
        }

        public async Task<CityDetail?> GetByIdAsync(long id)
        {
            CityDetail? city = null;
            await _adoCommand.ExecuteAsync(async (command) =>
            {
                command.Parameters.Add(_adoCommand.CreateParam($"@{nameof(City.Id)}", id, SqlDbType.BigInt));
                command.CommandText = @$"SELECT CT.Id, CT.[Name], P.[Name] AS Province, CT.[ProvinceId] 
                        FROM [{nameof(City)}] CT 
                        JOIN Province P ON CT.ProvinceId = P.Id
                        WHERE CT.{nameof(City.Id)} = @{nameof(City.Id)}";

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    city = MapCityFromReader(reader);
                }
            });

            return city;
        }

        public async Task<IEnumerable<CityDetail>> GetAsync(int page = 1, int pageSize = 10, string? name = null, string? province = null)
        {
            var Citys = new List<CityDetail>();
            await _adoCommand.ExecuteAsync(async (command) =>
            {
                var filter = _adoCommand.CreateFilterWithCustomParameter(command,
                    new SqlFilterParam("CT.[Name]", name, SqlDbType.NVarChar, "CityName"),
                    new SqlFilterParam("P.[Name]", province, SqlDbType.NVarChar, "ProvinceName")

                 );

                var paging = _adoCommand.CreatePaging(page, pageSize, "P.Id");

                command.CommandText =
                    @$"SELECT CT.Id, CT.[Name], P.[Name] AS Province, CT.[ProvinceId] 
                        FROM [{nameof(City)}] CT 
                        JOIN Province P ON CT.ProvinceId = P.Id
                   {filter} {paging}";
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    Citys.Add(MapCityFromReader(reader));
                }
            });

            return Citys;
        }

        public async Task UpdateAsync(City City)
        {
            await _adoCommand.ExecuteAsync(async (command) =>
            {
                command.CommandText = @$"UPDATE [{nameof(City)}]
                   SET 
                       {_adoCommand.GenerateUpdateColumnsBody(
                           nameof(City.Name),
                           nameof(City.ProvinceId)
                      )}                    
                   WHERE {nameof(City.Id)} = @{nameof(City.Id)}";

                AddParamenters(command, City);
                command.Parameters.Add(_adoCommand.CreateParam(nameof(City.Id), City.Id, SqlDbType.BigInt));
                await command.ExecuteNonQueryAsync();
            });
        }

        private void AddParamenters(IInterviewTestDataBaseCommand command, City city)
        {
            command.Parameters.Add(_adoCommand.CreateParam(nameof(City.Name), city.Name, SqlDbType.NVarChar));
            command.Parameters.Add(_adoCommand.CreateParam(nameof(City.ProvinceId), city.ProvinceId, SqlDbType.BigInt));
        }

        private CityDetail MapCityFromReader(SqlDataReader reader)
        {
            return new CityDetail
            {
                Id = reader.GetInt64(nameof(CityDetail.Id)),
                Name = reader.GetString(nameof(CityDetail.Name)),
                Province = reader.GetString(nameof(CityDetail.Province)),
                ProvinceId = reader.GetInt64(nameof(CityDetail.ProvinceId))
            };
        }
    }
}