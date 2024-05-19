
CREATE TABLE [User] (
	Id bigint not null identity,
    FirstName nvarchar(50) not null,
    LastName nvarchar(50) not null,
    Email nvarchar(50) not null,
    [Password] nvarchar(256) null,
    Age tinyint not null,
    [Date] DateTime,
    Country nvarchar(50),
    Province nvarchar(50),
    City nvarchar(50)

    CONSTRAINT PK_User_Id PRIMARY KEY(Id)
    
)
GO
CREATE UNIQUE NONCLUSTERED INDEX IX_User_Email ON [User](Email);

GO


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

