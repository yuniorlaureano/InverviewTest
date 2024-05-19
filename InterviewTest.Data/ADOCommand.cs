﻿using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace InterviewTest.Data
{
    public interface IADOCommand
    {
        Task Execute(Func<SqlCommand, Task> executeCommand);
        Task ExecuteTransaction(Func<SqlCommand, Task> executeCommand);
        SqlParameter CreateParam<T>(string name, T value, SqlDbType type);
        string CreateFilter(SqlCommand command, params SqlFilterParam[] sqlFilterParams);
        string CreatePaging(int page = 1, int pageSize = 10)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            return @$"
                ORDER BY Id
                OFFSET {(page - 1) * pageSize} ROWS
                FETCH NEXT {pageSize} ROWS ONLY   
            ";
        }
    }

    public class ADOCommand : IADOCommand
    {
        public readonly ISqlFactory _sqlFactory;
        public ADOCommand(ISqlFactory sqlFactory)
        {
            _sqlFactory = sqlFactory;
        }

        public async Task Execute(Func<SqlCommand, Task> executeCommand)
        {
            using var connection = await _sqlFactory.GetConnection();
            using var command = connection.CreateCommand();
            await executeCommand(command);
        }

        public async Task ExecuteTransaction(Func<SqlCommand, Task> executeCommand)
        {
            using var connection = await _sqlFactory.GetConnection();
            using var transaction = (SqlTransaction)await connection.BeginTransactionAsync();
            using var command = connection.CreateCommand();

            command.Transaction = transaction;

            try
            {
                await executeCommand(command);
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                try
                {
                    await transaction.RollbackAsync();
                }
                catch (SqlException)
                {
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
    }

    public record SqlFilterParam(string Param, object Value, SqlDbType Type);
}