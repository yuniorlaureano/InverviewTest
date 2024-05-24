using FluentAssertions;
using InterviewTest.Data;
using InterviewTest.Data.Decorators;
using InterviewTest.Data.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Data;

namespace InterviewTest.Test.Tests.Data
{
    public class ADOCommandTest
    {
        private IServiceProvider _services;

        public ADOCommandTest()
        {
            _services = DependencyBuilder.GetServices((collection, _) =>
            {
                collection.AddScoped<ISqlFactory, SqlFactory>();
                collection.AddScoped<IADOCommand, ADOCommand>();
            });
        }

        [Fact]
        public void Shuld_Return_Sql_Parameter_CreateParam()
        {
            //Arange
            var aDOCcommand = _services.GetRequiredService<IADOCommand>();

            //Act
            var parameter = aDOCcommand.CreateParam("Id", 43, SqlDbType.Int);

            //Assert
            parameter.Should().NotBeNull();
            parameter.Should().BeOfType<SqlParameter>();
            parameter.ParameterName.Should().Be("Id");
            parameter.Value.Should().Be(43);
            parameter.SqlDbType.Should().Be(SqlDbType.Int);
        }

        [Fact]
        public void Shuld_Return_Sql_WhereStatement_CreateFilter()
        {
            //Arange
            var aDOCcommand = _services.GetRequiredService<IADOCommand>();

            //Act
            var command = new InterviewTestDataBaseCommand();
            var whereFilter = aDOCcommand.CreateFilter(command,
                    new SqlFilterParam("Id", 22, SqlDbType.Int),
                    new SqlFilterParam("Name", "Dummy", SqlDbType.NVarChar)
                );

            //Assert
            whereFilter.Should().Be(" WHERE Id = @Id  AND Name = @Name ");

            command.Parameters[0].ParameterName.Should().Be("@Id");
            command.Parameters[0].Value.Should().Be(22);
            command.Parameters[0].SqlDbType.Should().Be(SqlDbType.Int);

            command.Parameters[1].ParameterName.Should().Be("@Name");
            command.Parameters[1].Value.Should().Be("Dummy");
            command.Parameters[1].SqlDbType.Should().Be(SqlDbType.NVarChar);
        }

        [Fact]
        public void Shuld_Return_Sql_WhereStatement_With_Custom_Name_CreateFilterWithCustomParameter()
        {
            //Arange
            var aDOCcommand = _services.GetRequiredService<IADOCommand>();

            //Act
            var command = new InterviewTestDataBaseCommand();
            var whereFilter = aDOCcommand.CreateFilterWithCustomParameter(command,
                    new SqlFilterParam("Id", 22, SqlDbType.Int, "ProductId"),
                    new SqlFilterParam("Name", "Dummy", SqlDbType.NVarChar, "ProductName")
                );

            //Assert
            whereFilter.Should().Be(" WHERE Id = @ProductId  AND Name = @ProductName ");

            command.Parameters[0].ParameterName.Should().Be("@ProductId");
            command.Parameters[0].Value.Should().Be(22);
            command.Parameters[0].SqlDbType.Should().Be(SqlDbType.Int);

            command.Parameters[1].ParameterName.Should().Be("@ProductName");
            command.Parameters[1].Value.Should().Be("Dummy");
            command.Parameters[1].SqlDbType.Should().Be(SqlDbType.NVarChar);
        }

        [Fact]
        public void Shuld_Return_Sql_InStatement_CreateInFilter()
        {
            //Arange
            var aDOCcommand = _services.GetRequiredService<IADOCommand>();
            var ids = new[] { 1, 2 };
            //Act
            var command = new InterviewTestDataBaseCommand();
            var whereFilter = aDOCcommand.CreateInFilter(command,
                ids.Select(x =>
                    new SqlFilterParam("Id", x, SqlDbType.Int)).ToArray()
                );

            //Assert
            whereFilter.Should().Be("IN (@Id0, @Id1)");
        }

        [Fact]
        public void Shuld_Return_Sql_Offset_Fetch_Statement_CreatePaging()
        {
            //Arange
            var page = 1;
            var pageSize = 10;
            var aDOCcommand = _services.GetRequiredService<IADOCommand>();

            //Act
            var whereFilter = aDOCcommand.CreatePaging(page, pageSize);

            //Assert
            string expected =
                $" ORDER BY Id " +
                $"OFFSET 0 ROWS " +
                $"FETCH NEXT 10 ROWS ONLY ";
            whereFilter.Should().Be(expected);
        }

        [Fact]
        public void Shuld_Return_Sql_Into_Values_Statement_GenerateInsertColumnsBody()
        {
            //Arange
            var aDOCcommand = _services.GetRequiredService<IADOCommand>();

            //Act
            var whereFilter = aDOCcommand.GenerateInsertColumnsBody(
                "Name",
                "LastName"
             );

            //Assert
            whereFilter.Should().Be("(Name,LastName) VALUES (@Name,@LastName)");
        }

        [Fact]
        public void Shuld_Return_Sql_Update_Statement_GenerateInsertColumnsBody()
        {
            //Arange
            var aDOCcommand = _services.GetRequiredService<IADOCommand>();

            //Act
            var whereFilter = aDOCcommand.GenerateUpdateColumnsBody(
                "Name",
                "LastName"
             );

            //Assert
            whereFilter.Should().Be("Name = @Name,LastName = @LastName");
        }

        [Fact]
        public async Task Shul_Execute_Sql_Statement_Execute()
        {
            //Arange
            var dbConnection = new Mock<IInterviewTestDataBaseConnection>();
            var dbCommand = new Mock<IInterviewTestDataBaseCommand>();
            var sqlFactory = new Mock<ISqlFactory>();

            dbCommand
                .Setup(x => x.ExecuteNonQueryAsync())
                .ReturnsAsync(45);

            dbCommand
                .Setup(x => x.Parameters)
                .Returns(new SqlCommand().Parameters);

            dbConnection
                .Setup(x => x.CreateCommand())
                .Returns(dbCommand.Object);

            sqlFactory
                .Setup(x => x.GetConnectionAsync())
                .ReturnsAsync(dbConnection.Object);

            var services = DependencyBuilder.GetServices((collection, _) =>
            {
                collection.AddScoped<ISqlFactory>((_) => sqlFactory.Object);
                collection.AddScoped<IADOCommand, ADOCommand>();
            });

            var aDOCcommand = services.GetRequiredService<IADOCommand>();

            //Act
            await aDOCcommand.ExecuteAsync(async (command) =>
            {
                await command.ExecuteNonQueryAsync();
                await Task.FromResult(0);
            });

            //Assert
            dbCommand.Verify(x => x.ExecuteNonQueryAsync(), Times.Once);
        }
    }
}
