# 金融商品喜好紀錄系統

ASP.NET Core MVC take-home implementation for a financial product favorite list system.

## 技術選擇

- C# / ASP.NET Core 8 MVC
- Razor Pages style MVC Views + Bootstrap
- SQL Server
- Dapper
- Stored Procedures for all database access
- Three-layer style:
  - `Controllers`: HTTP flow and model state handling
  - `Services`: business flow and ViewModel mapping
  - `Repositories`: Dapper + Stored Procedure calls
  - `DB`: DDL, seed data, and Stored Procedures

## 需求覆蓋

- 新增喜好金融商品
- 查詢喜好金融商品清單
- 查看喜好金融商品詳細資料
- 修改喜好金融商品
- 刪除喜好金融商品
- 使用 SQL Server Stored Procedure 存取資料
- 新增 / 修改 / 刪除流程使用 Transaction 處理多資料表異動
- 表單驗證、Anti-Forgery Token、參數化 Stored Procedure 呼叫
- DDL / DML / Stored Procedures 放在 `DB` 資料夾

## 重要假設

- `FeeRate` 使用小數表示，例如 `0.01` 代表 1%，`0.1` 代表 10%。
- `TotalFee = Price * PurchaseQuantity * FeeRate`。
- `TotalAmount = Price * PurchaseQuantity + TotalFee`，也就是預計扣款總金額包含手續費。
- 每筆喜好清單會建立一筆商品資料，更新喜好資料時也會同步更新該筆商品資料，藉此展示多表 Transaction。
- 刪除喜好清單時，若商品未被其他喜好清單使用，也會一併刪除商品資料。

## 資料庫初始化

請依序執行以下 SQL scripts：

```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -C -i "DB\01_CreateDatabaseAndTables.sql"
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -C -i "DB\02_SeedData.sql"
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -C -i "DB\03_StoredProcedures.sql"
```

若使用完整 SQL Server，請調整 `-S` 的 server name，並同步修改 `appsettings.json`：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=EsunBankFinancialProducts;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

## 執行方式

```powershell
dotnet restore
dotnet build
dotnet run
```

瀏覽器開啟終端機顯示的 localhost URL。

## 專案結構

```text
EsunBank.FinancialProducts/
├── Controllers/
│   └── LikeProductsController.cs
├── Data/
│   ├── ISqlConnectionFactory.cs
│   └── SqlConnectionFactory.cs
├── DB/
│   ├── 01_CreateDatabaseAndTables.sql
│   ├── 02_SeedData.sql
│   └── 03_StoredProcedures.sql
├── Repositories/
├── Services/
├── ViewModels/
└── Views/LikeProducts/
```

## 安全性說明

- Controller POST actions 使用 `[ValidateAntiForgeryToken]`。
- Razor 預設會進行 HTML encoding，降低 XSS 風險。
- 表單欄位使用 DataAnnotations 驗證。
- Repository 使用 Dapper 參數化呼叫 Stored Procedure，避免字串拼接 SQL。
- SQL table constraints 與 Stored Procedure 內部檢查會再次保護金額、費率與數量。

## Codex CLI 建議工作流

1. 先跑 `dotnet build`，確認程式碼可編譯。
2. 執行 `DB` scripts 建立資料庫。
3. 跑 `dotnet run`，手動測試新增、查詢、修改、刪除。
4. 每完成一個小段落就 commit，例如 `feat: scaffold mvc favorite product system`。
5. 送出前確認不要 commit 面試題目 PDF。
