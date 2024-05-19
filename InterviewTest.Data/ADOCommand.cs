using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Text;

namespace InterviewTest.Data
{
    public interface IADOCommand
    {
        Task Execute(Func<SqlCommand, Task> executeCommand);
        Task ExecuteTransaction(Func<SqlCommand, Action, Task> executeCommand);
        SqlParameter CreateParam<T>(string name, T value, SqlDbType type);
        string CreateFilter(SqlCommand command, params SqlFilterParam[] sqlFilterParams);
        string CreateInFilter(SqlCommand command, params SqlFilterParam[] sqlFilterParams);
        string CreatePaging(int page = 1, int pageSize = 10);
        string GenerateInsertColumnsBody(params string[] fields);
        string GenerateUpdateColumnsBody(params string[] fields);
    }

    public class ADOCommand : IADOCommand
    {
        public readonly ISqlFactory _sqlFactory;
        private readonly ILogger<ADOCommand> _logger;
        private readonly IHostEnvironment _hostEnvironment;

        public ADOCommand(ISqlFactory sqlFactory, ILogger<ADOCommand> logger, IHostEnvironment hostEnvironment)
        {
            _sqlFactory = sqlFactory;
            _logger = logger;
            _hostEnvironment = hostEnvironment;
        }

        public async Task Execute(Func<SqlCommand, Task> executeCommand)
        {
            try
            {
                using var connection = await _sqlFactory.GetConnection();
                using var command = connection.CreateCommand();
                await executeCommand(command);
                LogCommand(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing sql command");
                throw;
            }
        }

        public async Task ExecuteTransaction(Func<SqlCommand, Action, Task> executeCommand)
        {
            using var connection = await _sqlFactory.GetConnection();
            using var transaction = (SqlTransaction)await connection.BeginTransactionAsync();
            using var command = connection.CreateCommand();

            command.Transaction = transaction;

            try
            {
                await executeCommand(command, () =>
                {
                    LogCommand(command);
                });
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing sql transaction");
                try
                {
                    await transaction.RollbackAsync();
                }
                catch (SqlException)
                {
                    _logger.LogError(ex, "Error rolling back sql transaction");
                    throw;
                }
            }
        }

        public SqlParameter CreateParam<T>(string name, T value, SqlDbType type)
        {
            return new SqlParameter()
            {
                ParameterName = name,
                Value = value,
                SqlDbType = type
            };
        }

        public string CreateFilter(SqlCommand command, params SqlFilterParam[] sqlFilterParams)
        {
            var where = new StringBuilder();
            foreach (var sqlFilterParam in sqlFilterParams)
            {
                if (sqlFilterParam.Value is not null)
                {
                    where.Append($"{sqlFilterParam.Param} = @{sqlFilterParam.Param} ");
                    where.Append(" AND ");
                    command.Parameters.Add(CreateParam($"@{sqlFilterParam.Param}", sqlFilterParam.Value, sqlFilterParam.Type));
                }
            }

            if (where.Length > 0)
            {
                where.Remove(where.Length - 4, 4);
                where.Insert(0, " WHERE ");
            }

            return where.ToString();
        }

        public string CreateInFilter(SqlCommand command, params SqlFilterParam[] sqlFilterParams)
        {
            var filter = new StringBuilder();
            filter.Append("(");
            for (int i = 0; i < sqlFilterParams.Length; i++)
            {
                if (sqlFilterParams[i].Value is not null)
                {
                    filter.Append($"@{sqlFilterParams[i].Param}{i}, ");
                    command.Parameters.Add(CreateParam($"@{sqlFilterParams[i].Param}{i}", sqlFilterParams[i].Value, sqlFilterParams[i].Type));
                }
            }

            if (filter.Length > 0)
            {
                filter.Remove(filter.Length - 2, 2);
            }
            filter.Append(")");

            return filter.ToString();
        }

        public string CreatePaging(int page = 1, int pageSize = 10)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            return @$"
                ORDER BY Id
                OFFSET {(page - 1) * pageSize} ROWS
                FETCH NEXT {pageSize} ROWS ONLY   
            ";
        }

        public string GenerateInsertColumnsBody(params string[] fields)
        {
            var lastFieldIndex = fields.Length - 1;
            var insertInto = new StringBuilder();
            var insertValue = new StringBuilder();

            for (int i = 0; i < fields.Length - 1; i++)
            {
                insertInto.Append($"{fields[i]},");
                insertValue.Append($"@{fields[i]},");
            }

            insertInto.Append($"{fields[lastFieldIndex]}");
            insertValue.Append($"@{fields[lastFieldIndex]}");

            return $"({insertInto}) VALUES ({insertValue})";
        }

        public string GenerateUpdateColumnsBody(params string[] fields)
        {
            var lastFieldIndex = fields.Length - 1;
            var udpate = new StringBuilder();

            for (int i = 0; i < fields.Length - 1; i++)
            {
                udpate.Append($"{fields[i]} = @{fields[i]},");
            }

            udpate.Append($"{fields[lastFieldIndex]} = @{fields[lastFieldIndex]}");

            return udpate.ToString();
        }

        private void LogCommand(SqlCommand command)
        {
            if (_hostEnvironment.IsDevelopment())
            {
                var query = command.CommandText;
                foreach (SqlParameter parameter in command.Parameters)
                {
                    query = query.Replace(parameter.ParameterName, parameter.Value?.ToString() ?? "NULL");
                }

                _logger.LogDebug("Executing SQL Command: {Query}", query);
            }           
        }
    }

    public record SqlFilterParam(string Param, object Value, SqlDbType Type);
}