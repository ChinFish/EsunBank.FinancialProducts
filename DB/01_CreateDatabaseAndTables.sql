IF DB_ID(N'EsunBankFinancialProducts') IS NULL
BEGIN
    CREATE DATABASE EsunBankFinancialProducts;
END
GO

USE EsunBankFinancialProducts;
GO

IF OBJECT_ID(N'dbo.LikeLists', N'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.LikeLists;
END
GO

IF OBJECT_ID(N'dbo.Products', N'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.Products;
END
GO

IF OBJECT_ID(N'dbo.Users', N'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.Users;
END
GO

CREATE TABLE dbo.Users
(
    UserId NVARCHAR(20) NOT NULL,
    UserName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(256) NOT NULL,
    Account NVARCHAR(20) NOT NULL,
    CONSTRAINT PK_Users PRIMARY KEY (UserId),
    CONSTRAINT CK_Users_Account_Numeric CHECK (Account NOT LIKE '%[^0-9]%'),
    CONSTRAINT CK_Users_Account_Length CHECK (LEN(Account) BETWEEN 6 AND 20),
    CONSTRAINT CK_Users_Email_NotBlank CHECK (LEN(LTRIM(RTRIM(Email))) > 0)
);
GO

CREATE TABLE dbo.Products
(
    No INT IDENTITY(1,1) NOT NULL,
    ProductName NVARCHAR(100) NOT NULL,
    Price DECIMAL(18, 2) NOT NULL,
    FeeRate DECIMAL(9, 4) NOT NULL,
    CreatedAt DATETIME2(0) NOT NULL CONSTRAINT DF_Products_CreatedAt DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2(0) NULL,
    CONSTRAINT PK_Products PRIMARY KEY (No),
    CONSTRAINT CK_Products_ProductName_NotBlank CHECK (LEN(LTRIM(RTRIM(ProductName))) > 0),
    CONSTRAINT CK_Products_Price_Positive CHECK (Price > 0),
    CONSTRAINT CK_Products_FeeRate_Range CHECK (FeeRate >= 0 AND FeeRate <= 1)
);
GO

CREATE TABLE dbo.LikeLists
(
    SN INT IDENTITY(1,1) NOT NULL,
    UserId NVARCHAR(20) NOT NULL,
    ProductNo INT NOT NULL,
    PurchaseQuantity INT NOT NULL,
    Account NVARCHAR(20) NOT NULL,
    TotalFee DECIMAL(18, 2) NOT NULL,
    TotalAmount DECIMAL(18, 2) NOT NULL,
    CreatedAt DATETIME2(0) NOT NULL CONSTRAINT DF_LikeLists_CreatedAt DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2(0) NULL,
    CONSTRAINT PK_LikeLists PRIMARY KEY (SN),
    CONSTRAINT FK_LikeLists_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_LikeLists_Products FOREIGN KEY (ProductNo) REFERENCES dbo.Products(No),
    CONSTRAINT CK_LikeLists_Quantity_Positive CHECK (PurchaseQuantity > 0),
    CONSTRAINT CK_LikeLists_Account_Numeric CHECK (Account NOT LIKE '%[^0-9]%'),
    CONSTRAINT CK_LikeLists_Account_Length CHECK (LEN(Account) BETWEEN 6 AND 20),
    CONSTRAINT CK_LikeLists_TotalFee_NonNegative CHECK (TotalFee >= 0),
    CONSTRAINT CK_LikeLists_TotalAmount_Positive CHECK (TotalAmount > 0)
);
GO

CREATE INDEX IX_LikeLists_UserId ON dbo.LikeLists(UserId);
CREATE INDEX IX_LikeLists_ProductNo ON dbo.LikeLists(ProductNo);
GO
