
CREATE TABLE [Product] (
	Id bigint not null identity,
    Code nvarchar(50) not null,
    [Name] nvarchar(50) not null,
    Price decimal(15,2) not null,
    [Description] nvarchar(50) null

    CONSTRAINT PK_Product_Id PRIMARY KEY(Id)
    
)
GO
CREATE UNIQUE NONCLUSTERED INDEX IX_Product_Code ON [Product](Code);
GO

CREATE TABLE [Stock] (
	Id bigint not null identity,
    TransactionType tinyint not null,
    [Description] nvarchar(200) not null,
    [Date] datetime not null

    CONSTRAINT PK_Stock_Id PRIMARY KEY(Id)
    
)
GO

CREATE TABLE [StockDetail] (
	Id bigint not null identity,
    Quantity int not null,
    ProductId bigint not null,
    StockId bigint not null

    CONSTRAINT PK_StockDetail_Id PRIMARY KEY(Id),
    CONSTRAINT FK_StockDetail_Product_ProductId FOREIGN KEY(ProductId) REFERENCES Product(Id),
    CONSTRAINT FK_StockDetail_Stock_StockId FOREIGN KEY(StockId) REFERENCES Stock(Id) ON DELETE CASCADE
    
)
GO

CREATE TABLE [User] (
	Id bigint not null identity,
    FirstName nvarchar(50) not null,
    LastName nvarchar(50) not null,
    Email nvarchar(50) not null,
    [Password] nvarchar(256) null,
    Age tinyint not null,
    [Date] DateTime,

    CountryId bigint,
    ProvinceId bigint,
    CityId bigint
)
GO
CREATE UNIQUE NONCLUSTERED INDEX IX_User_Email ON [User](Email);

CREATE TABLE Country (
	Id bigint primary key identity,
	[Name] nvarchar(50),
)
GO
CREATE TABLE Province (
	Id bigint primary key identity,
	[Name] nvarchar(50),
	CountryId bigint not null

	CONSTRAINT FK_Province_Country_CountryId FOREIGN KEY(CountryId) REFERENCES Country(Id)
)
GO
CREATE TABLE City (
	Id bigint primary key identity,
	[Name] nvarchar(50),
	ProvinceId bigint not null,

	CONSTRAINT FK_City_Province_ProvinceId FOREIGN KEY(ProvinceId) REFERENCES Province(Id)
)
GO