using InterviewTest.Entity;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace InterviewTest.Data
{
    public interface ICountryRepository
    {
        public Task<Country?> GetByIdAsync(long id);
        public Task<IEnumerable<Country>> GetAsync(int page = 1, int pageSize = 10, string? name = null);
        public Task AddAsync(Country Country);
        public Task UpdateAsync(Country Country);
    }

    public class CountryRepository : ICountryRepository
    {
        private readonly IADOCommand _adoCommand;

        public CountryRepository(IADOCommand adoCommand)
        {
            _adoCommand = adoCommand;
        }

        public async Task AddAsync(Country country)
        {
            await _adoCommand.ExecuteAsync(async (command) =>
            {
                command.CommandText = @$"INSERT INTO [{nameof(Country)}]
                    {_adoCommand.GenerateInsertColumnsBody(
                       nameof(Country.Name)
                   )}";

                AddParamenters(command, country);
                await command.ExecuteNonQueryAsync();
            });
        }

        public async Task<Country?> GetByIdAsync(long id)
        {
            Country? country = null;
            await _adoCommand.ExecuteAsync(async (command) =>
            {
                command.Parameters.Add(_adoCommand.CreateParam($"@{nameof(Country.Id)}", id, SqlDbType.BigInt));
                command.CommandText = $"SELECT * FROM  [{nameof(Country)}] WHERE {nameof(Country.Id)} = @{nameof(Country.Id)}";

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    country = MapCountryFromReader(reader);
                }
            });

            return country;
        }

        public async Task<IEnumerable<Country>> GetAsync(int page = 1, int pageSize = 10, string? name = null)
        {
            var Countrys = new List<Country>();
            await _adoCommand.ExecuteAsync(async (command) =>
            {
                var filter = _adoCommand.CreateFilter(command,
                    new SqlFilterParam(nameof(Country.Name), name, SqlDbType.NVarChar)
                 );

                var paging = _adoCommand.CreatePaging(page, pageSize);

                command.CommandText = $"SELECT * FROM  [{nameof(Country)}] {filter} {paging}";
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    Countrys.Add(MapCountryFromReader(reader));
                }
            });

            return Countrys;
        }

        public async Task UpdateAsync(Country country)
        {
            await _adoCommand.ExecuteAsync(async (command) =>
            {
                command.CommandText = @$"UPDATE [{nameof(Country)}]
                   SET 
                       {_adoCommand.GenerateUpdateColumnsBody(
                           nameof(Country.Name)
                      )}                    
                   WHERE {nameof(Country.Id)} = @{nameof(Country.Id)}";

                AddParamenters(command, country);
                command.Parameters.Add(_adoCommand.CreateParam(nameof(Country.Id), country.Id, SqlDbType.BigInt));
                await command.ExecuteNonQueryAsync();
            });
        }

        private void AddParamenters(IInterviewTestDataBaseCommand command, Country country)
        {
            command.Parameters.Add(_adoCommand.CreateParam(nameof(Country.Name), country.Name, SqlDbType.NVarChar));
        }

        private Country MapCountryFromReader(SqlDataReader reader)
        {
            return new Country
            {
                Id = reader.GetInt64(nameof(Country.Id)),
                Name = reader.GetString(nameof(Country.Name))
            };
        }
    }
}