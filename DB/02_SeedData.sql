USE EsunBankFinancialProducts;
GO

DELETE FROM dbo.LikeLists;
DELETE FROM dbo.Products;
DELETE FROM dbo.Users;
GO

INSERT INTO dbo.Users (UserId, UserName, Email, Account)
VALUES
    (N'A1236456789', N'Wang Xiao Ming', N'test@email.com', N'1111999666'),
    (N'B2234567890', N'Irene Chen', N'irene.chen@example.com', N'2222888777'),
    (N'C3234567890', N'Hao Lin', N'hao.lin@example.com', N'3333777666');
GO

DECLARE @ProductNo INT;

INSERT INTO dbo.Products (ProductName, Price, FeeRate)
VALUES (N'ESUN Global Income Fund', 10000.00, 0.0100);

SET @ProductNo = SCOPE_IDENTITY();

INSERT INTO dbo.LikeLists (UserId, ProductNo, PurchaseQuantity, Account, TotalFee, TotalAmount)
VALUES
(
    N'A1236456789',
    @ProductNo,
    3,
    N'1111999666',
    10000.00 * 3 * 0.0100,
    10000.00 * 3 + (10000.00 * 3 * 0.0100)
);
GO
