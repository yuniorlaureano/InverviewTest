using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Text;

namespace InterviewTest.Data
{
    public interface IADOCommand
    {
        /// <summary>
        /// Allow the execution of a command against the database
        /// Takes care of openning the connection and handling it disposal
        /// When running in development log the commandText to the configured logger provider
        /// </summary>
        /// <param name="executeCommand">
        ///  Allow to build the command to be executed
        /// </param>
        /// <returns></returns>
        Task ExecuteAsync(Func<IInterviewTestDataBaseCommand, Task> executeCommand);

        /// <summary>
        /// Allow the execution of a command against the database using transaction
        /// Takes care of openning the connection and handling it disposal
        /// When running in development log the commandText to the configured logger provider
        /// </summary>
        /// <param name="executeCommand">
        ///  Allow to build the command to be executed
        /// </param>
        /// /// <param name="Action">
        ///  Allow the loggin of the commandText
        /// </param>
        /// <returns></returns>
        Task ExecuteTransactionAsync(Func<IInterviewTestDataBaseCommand, Action, Task> executeCommand);

        /// <summary>
        /// Create a sqlParameter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">Parameter name</param>
        /// <param name="value">Parameter value</param>
        /// <param name="type">Parameter data type</param>
        /// <returns>The built SqlParameter</returns>
        SqlParameter CreateParam<T>(string name, T value, SqlDbType type);

        /// <summary>
        /// Allow the creation of a where filter with a custom parameter name
        /// </summary>
        /// <param name="command">The command to attach the parameters to</param>
        /// <param name="sqlFilterParams">A type containing the information about the parameter to add to the filter</param>
        /// <returns>The built where filter</returns>
        string CreateFilterWithCustomParameter(IInterviewTestDataBaseCommand command, params SqlFilterParam[] sqlFilterParams);

        /// <summary>
        /// Allow the creation of a where filter
        /// </summary>
        /// <param name="command">The command to attach the parameters to</param>
        /// <param name="sqlFilterParams">A type containing the information about the parameter to add to the filter</param>
        /// <returns>The built where filter</returns>
        string CreateFilter(IInterviewTestDataBaseCommand command, params SqlFilterParam[] sqlFilterParams);

        /// <summary>
        /// Create a sql "in statement" of the supplied parameters
        /// </summary>
        /// <param name="command">The command to attach the parameters to</param>
        /// <param name="sqlFilterParams">A type containing the information about the parameter to add to the filter</param>
        /// <returns>An string like: "in(p1,p2,p3)"</returns>
        string CreateInFilter(IInterviewTestDataBaseCommand command, params SqlFilterParam[] sqlFilterParams);

        /// <summary>
        /// Create an sql pagin statement
        /// </summary>
        /// <param name="page">The page to filter, default to "1"</param>
        /// <param name="pageSize">The page size, default to 10</param>
        /// <param name="orderBy">The column to order by, default to Id</param>
        /// <returns>A sql statement like: 
        ///  <br/> ORDER BY {orderBy}
        ///  <br/> OFFSET {(page - 1) * pageSize} ROWS
        ///  <br/> FETCH NEXT {pageSize}
        ///  <br/> ROWS ONLY
        /// </returns>
        string CreatePaging(int page = 1, int pageSize = 10, string orderBy = "Id");

        /// <summary>
        /// Build the list of field inside the "INTO(fields)", and the "VALUES(@fields)"
        /// </summary>
        /// <param name="fields">The fields to add inside the INSERT clauses</param>
        /// <returns>A string like: "(fields) VALUES (@fields)"</returns>
        string GenerateInsertColumnsBody(params string[] fields);

        /// <summary>
        /// Build the list of field inside the "UPDATE SET fields = @fields"
        /// </summary>
        /// <param name="fields">The fields to add inside the INSERT clauses</param>
        /// <returns>A string like: "UPDATE SET fields = @fields"</returns>
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

        public async Task ExecuteAsync(Func<IInterviewTestDataBaseCommand, Task> executeCommand)
        {
            try
            {
                await using var connection = await _sqlFactory.GetConnectionAsync();
                await using var command = connection.CreateCommand();
                await executeCommand(command);
                LogCommand(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing sql command");
                throw;
            }
        }

        public async Task ExecuteTransactionAsync(Func<IInterviewTestDataBaseCommand, Action, Task> executeCommand)
        {
            await using var connection = await _sqlFactory.GetConnectionAsync();
            using var transaction = (SqlTransaction)await connection.BeginTransactionAsync();
            await using var command = connection.CreateCommand();

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

        public string CreateFilter(IInterviewTestDataBaseCommand command, params SqlFilterParam[] sqlFilterParams)
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
                where.Remove(where.Length - 5, 5);
                where.Insert(0, " WHERE ");
            }

            return where.ToString();
        }

        public string CreateFilterWithCustomParameter(IInterviewTestDataBaseCommand command, params SqlFilterParam[] sqlFilterParams)
        {
            var where = new StringBuilder();
            foreach (var sqlFilterParam in sqlFilterParams)
            {
                if (sqlFilterParam.Value is not null)
                {
                    where.Append($"{sqlFilterParam.Param} = @{sqlFilterParam.ParamName} ");
                    where.Append(" AND ");
                    command.Parameters.Add(CreateParam($"@{sqlFilterParam.ParamName}", sqlFilterParam.Value, sqlFilterParam.Type));
                }
            }

            if (where.Length > 0)
            {
                where.Remove(where.Length - 5, 5);
                where.Insert(0, " WHERE ");
            }

            return where.ToString();
        }

        public string CreateInFilter(IInterviewTestDataBaseCommand command, params SqlFilterParam[] sqlFilterParams)
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
                filter.Insert(0, "IN ");
            }
            filter.Append(")");

            return filter.ToString();
        }

        public string CreatePaging(int page = 1, int pageSize = 10, string orderBy = "Id")
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            return
                $" ORDER BY {orderBy} " +
                $"OFFSET {(page - 1) * pageSize} ROWS " +
                $"FETCH NEXT {pageSize} ROWS ONLY ";
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

        private void LogCommand(IInterviewTestDataBaseCommand command)
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

    /// <summary>
    /// Represent a parameter to be transformed into a SqlParameter
    /// </summary>
    /// <param name="Param">Parameter name</param>
    /// <param name="Value">Parameter value</param>
    /// <param name="Type">Paramete type</param>
    /// <param name="ParamName">A custom parameter name</param>
    public record SqlFilterParam(string Param, object Value, SqlDbType Type, string ParamName = "");
}