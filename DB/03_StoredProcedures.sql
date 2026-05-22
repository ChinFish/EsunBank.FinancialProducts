USE EsunBankFinancialProducts;
GO

CREATE OR ALTER PROCEDURE dbo.usp_User_GetOptions
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        UserId,
        UserName,
        Email,
        Account
    FROM dbo.Users
    ORDER BY UserName;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_LikeProduct_GetList
    @UserId NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        l.SN AS Sn,
        u.UserId,
        u.UserName,
        u.Email,
        u.Account AS DefaultAccount,
        l.Account AS DebitAccount,
        p.No AS ProductNo,
        p.ProductName,
        p.Price,
        p.FeeRate,
        l.PurchaseQuantity,
        l.TotalFee,
        l.TotalAmount
    FROM dbo.LikeLists l
    INNER JOIN dbo.Users u ON u.UserId = l.UserId
    INNER JOIN dbo.Products p ON p.No = l.ProductNo
    WHERE @UserId IS NULL OR l.UserId = @UserId
    ORDER BY l.SN DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_LikeProduct_GetDetail
    @Sn INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        l.SN AS Sn,
        u.UserId,
        u.UserName,
        u.Email,
        u.Account AS DefaultAccount,
        l.Account AS DebitAccount,
        p.No AS ProductNo,
        p.ProductName,
        p.Price,
        p.FeeRate,
        l.PurchaseQuantity,
        l.TotalFee,
        l.TotalAmount
    FROM dbo.LikeLists l
    INNER JOIN dbo.Users u ON u.UserId = l.UserId
    INNER JOIN dbo.Products p ON p.No = l.ProductNo
    WHERE l.SN = @Sn;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_LikeProduct_Create
    @UserId NVARCHAR(20),
    @ProductName NVARCHAR(100),
    @Price DECIMAL(18, 2),
    @FeeRate DECIMAL(9, 4),
    @Account NVARCHAR(20),
    @PurchaseQuantity INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE UserId = @UserId)
    BEGIN
        THROW 50001, 'User does not exist.', 1;
    END

    IF @Price <= 0 OR @FeeRate < 0 OR @FeeRate > 1 OR @PurchaseQuantity <= 0
    BEGIN
        THROW 50002, 'Invalid product price, fee rate, or purchase quantity.', 1;
    END

    DECLARE @ProductNo INT;
    DECLARE @Sn INT;
    DECLARE @BaseAmount DECIMAL(18, 2) = @Price * @PurchaseQuantity;
    DECLARE @TotalFee DECIMAL(18, 2) = @BaseAmount * @FeeRate;
    DECLARE @TotalAmount DECIMAL(18, 2) = @BaseAmount + @TotalFee;

    BEGIN TRANSACTION;

    INSERT INTO dbo.Products (ProductName, Price, FeeRate)
    VALUES (@ProductName, @Price, @FeeRate);

    SET @ProductNo = CAST(SCOPE_IDENTITY() AS INT);

    INSERT INTO dbo.LikeLists (UserId, ProductNo, PurchaseQuantity, Account, TotalFee, TotalAmount)
    VALUES (@UserId, @ProductNo, @PurchaseQuantity, @Account, @TotalFee, @TotalAmount);

    SET @Sn = CAST(SCOPE_IDENTITY() AS INT);

    COMMIT TRANSACTION;

    SELECT @Sn AS Sn;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_LikeProduct_Update
    @Sn INT,
    @UserId NVARCHAR(20),
    @ProductName NVARCHAR(100),
    @Price DECIMAL(18, 2),
    @FeeRate DECIMAL(9, 4),
    @Account NVARCHAR(20),
    @PurchaseQuantity INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE UserId = @UserId)
    BEGIN
        THROW 50001, 'User does not exist.', 1;
    END

    IF @Price <= 0 OR @FeeRate < 0 OR @FeeRate > 1 OR @PurchaseQuantity <= 0
    BEGIN
        THROW 50002, 'Invalid product price, fee rate, or purchase quantity.', 1;
    END

    DECLARE @ProductNo INT;
    DECLARE @BaseAmount DECIMAL(18, 2) = @Price * @PurchaseQuantity;
    DECLARE @TotalFee DECIMAL(18, 2) = @BaseAmount * @FeeRate;
    DECLARE @TotalAmount DECIMAL(18, 2) = @BaseAmount + @TotalFee;

    SELECT @ProductNo = ProductNo
    FROM dbo.LikeLists
    WHERE SN = @Sn;

    IF @ProductNo IS NULL
    BEGIN
        THROW 50003, 'Like product does not exist.', 1;
    END

    BEGIN TRANSACTION;

    UPDATE dbo.Products
    SET
        ProductName = @ProductName,
        Price = @Price,
        FeeRate = @FeeRate,
        UpdatedAt = SYSUTCDATETIME()
    WHERE No = @ProductNo;

    UPDATE dbo.LikeLists
    SET
        UserId = @UserId,
        PurchaseQuantity = @PurchaseQuantity,
        Account = @Account,
        TotalFee = @TotalFee,
        TotalAmount = @TotalAmount,
        UpdatedAt = SYSUTCDATETIME()
    WHERE SN = @Sn;

    COMMIT TRANSACTION;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_LikeProduct_Delete
    @Sn INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @ProductNo INT;

    SELECT @ProductNo = ProductNo
    FROM dbo.LikeLists
    WHERE SN = @Sn;

    IF @ProductNo IS NULL
    BEGIN
        RETURN;
    END

    BEGIN TRANSACTION;

    DELETE FROM dbo.LikeLists
    WHERE SN = @Sn;

    IF NOT EXISTS (SELECT 1 FROM dbo.LikeLists WHERE ProductNo = @ProductNo)
    BEGIN
        DELETE FROM dbo.Products
        WHERE No = @ProductNo;
    END

    COMMIT TRANSACTION;
END
GO
