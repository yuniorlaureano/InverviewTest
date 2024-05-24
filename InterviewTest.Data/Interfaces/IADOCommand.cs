using InterviewTest.Data.Decorators;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InterviewTest.Data.Interfaces
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
}
