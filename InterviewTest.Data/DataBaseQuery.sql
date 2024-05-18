CREATE DATABASE InterviewTest;
GO
USE InterviewTest;
GO

CREATE TABLE [User] (
	Id bigint not null identity,
    FirstName nvarchar(50) not null,
    LastName nvarchar(50) not null,
    Age tinyint not null,
    [Date] DateTime,
    Country nvarchar(50),
    Province nvarchar(50),
    City nvarchar(50)

    CONSTRAINT PK_User_Id PRIMARY KEY(Id)
)