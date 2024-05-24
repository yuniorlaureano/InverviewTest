using InterviewTest.Common;
using InterviewTest.Data.Decorators;
using InterviewTest.Data.Interfaces;
using InterviewTest.Entity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Text;

namespace InterviewTest.Data
{
    public class CountryRepository : ICountryRepository
    {
        private readonly IADOCommand _adoCommand;
        private readonly IExecutionResultFactory _resultFactory;
        private readonly ILogger<CountryRepository> _logger;

        public CountryRepository(
            IADOCommand adoCommand,
            IExecutionResultFactory resultFactory,
            ILogger<CountryRepository> logger)
        {
            _adoCommand = adoCommand;
            _resultFactory = resultFactory;
            _logger = logger;
        }

        public async Task<ExecutionResult> AddAsync(Country country)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating country");
                return _resultFactory
                    .AsFailure()
                    .AddError("country", "Error creating country");
            }

            return _resultFactory.AsSuccessful();
        }

        public async Task<ExecutionResult<Country?>> GetByIdAsync(long id)
        {
            Country? country = null;
            try
            {
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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting country with Id '{id}'", id);
                return _resultFactory
                    .AsFailure<Country?>(null)
                    .AddError("country", $"Error getting country with Id '{id}'");
            }

            return _resultFactory
                .AsSuccessful(country);
        }

        public async Task<ExecutionResult<IEnumerable<Country>>> GetAsync(int page = 1, int pageSize = 10, string? name = null)
        {
            var Countrys = new List<Country>();
            try
            {
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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting country");
                return _resultFactory
                    .AsFailure<IEnumerable<Country>>(Countrys)
                    .AddError("country", "Error getting country");
            }
            return _resultFactory
                .AsSuccessful<IEnumerable<Country>>(Countrys);
        }

        public async Task<ExecutionResult> UpdateAsync(Country country)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating country");
                return _resultFactory.AsFailure()
                    .AddError("country", "Error updating country");
            }

            return _resultFactory.AsSuccessful();
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