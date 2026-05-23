# 金融商品喜好紀錄系統

ASP.NET Core MVC 實作的金融商品喜好紀錄系統。使用者可以依會員建立、查詢、檢視、修改與刪除喜好的金融商品，系統會依產品價格、購買數量與手續費率計算手續費與預計扣款總金額。

本專案重點放在清楚可維護的 MVC 分層、Stored Procedure 資料存取、基本表單驗證與可 demo 的 CRUD 流程。

## 功能範圍

- 依使用者篩選喜好金融商品清單。
- 新增喜好金融商品。
- 查看單筆喜好金融商品明細。
- 修改喜好金融商品與扣款資訊。
- 刪除喜好金融商品。
- 表單驗證與錯誤訊息顯示。
- 資料異動透過 Stored Procedure 處理，新增、修改、刪除流程包含多資料表異動與 transaction。

## 技術棧

- .NET 8 / ASP.NET Core MVC
- Razor Views
- Bootstrap
- SQL Server / LocalDB
- Dapper
- SQL Server Stored Procedure

## 專案結構

```text
EsunBank.FinancialProducts/
├── DB/               # DDL、seed data、Stored Procedures
├── EsunBank.FinancialProducts.Web/
│   ├── Controllers/  # HTTP flow、ModelState、Redirect、NotFound/BadRequest
│   ├── Models/       # MVC error/domain data shape
│   ├── ViewModels/   # Razor Views 使用的顯示與表單模型
│   ├── Views/        # Razor 畫面
│   └── wwwroot/      # CSS、JavaScript 與前端套件
├── EsunBank.FinancialProducts.Service/
│   └── Services/     # 商業流程、資料 trim、service DTO mapping
├── EsunBank.FinancialProducts.Repository/
│   ├── Data/         # SQL connection factory
│   └── Repositories/ # Dapper + Stored Procedure 呼叫
├── EsunBank.FinancialProducts.Common/
│   └── EsunBank.FinancialProducts.Common.csproj
└── EsunBank.FinancialProducts.Service.Tests/
    └── LikeProductServiceTests.cs
```

## 架構說明

資料流採用 Controller、Service、Repository 分層：

```text
Browser
  -> Controller
  -> Service
  -> Repository
  -> Stored Procedure
  -> SQL Server
```

- `Controllers` 負責 HTTP request/response、ModelState、TempData 與頁面導向。
- `Services` 負責應用流程、資料 trim、service DTO mapping，不依賴 MVC ViewModel。
- `Repositories` 只負責透過 Dapper 呼叫 Stored Procedure，不放畫面或商業流程。
- `Common` 保留給跨展示層、業務層與資料層都合理共用的型別或工具；目前沒有必要的共用邏輯，因此不放 placeholder 程式碼。
- `DB` 保存可重建資料庫的 SQL scripts，包含 schema、seed data 與 Stored Procedures。
- 專案依賴方向固定為 `Web -> Service -> Repository`，Web 不直接引用 Repository；各層可引用 `Common`。

部署語意上，本專案可對應為：

```text
Browser
  -> Web Server: Kestrel / IIS Express / IIS
  -> Application Server: ASP.NET Core MVC application
  -> Database Server: SQL Server / LocalDB
```

take-home 範圍以可編譯、可 demo 的 MVC 分層與資料庫 CRUD 為主，因此未額外加入 Docker Compose 或 IIS 部署檔。

## 資料庫設計

主要資料表：

- `Users`：使用者基本資料與預設帳號。
- `Products`：金融商品名稱、價格與手續費率。
- `LikeLists`：使用者喜好的金融商品、購買數量、扣款帳號、手續費與總扣款金額。

主要 Stored Procedures：

- `dbo.usp_User_GetOptions`
- `dbo.usp_LikeProduct_GetList`
- `dbo.usp_LikeProduct_GetDetail`
- `dbo.usp_LikeProduct_Create`
- `dbo.usp_LikeProduct_Update`
- `dbo.usp_LikeProduct_Delete`

## 金額計算規則

- `FeeRate` 使用小數表示，例如 `0.01` 代表 1%。
- `BaseAmount = Price * PurchaseQuantity`
- `TotalFee = BaseAmount * FeeRate`
- `TotalAmount = BaseAmount + TotalFee`

新增與修改時，金額由 Stored Procedure 重新計算，避免只依賴前端或 MVC 表單輸入。

## 執行環境

請先安裝：

- .NET 8 SDK
- SQL Server LocalDB 或 SQL Server
- `sqlcmd`

預設連線字串位於 `EsunBank.FinancialProducts.Web/appsettings.json`：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=EsunBankFinancialProducts;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

如果不是使用 LocalDB，請同步調整 SQL script 的 `-S` 參數與 `appsettings.json` 的 connection string。

## 資料庫初始化

在專案根目錄依序執行：

```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -C -i "DB\01_CreateDatabaseAndTables.sql"
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -C -i "DB\02_SeedData.sql"
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -C -i "DB\03_StoredProcedures.sql"
```

注意：`01_CreateDatabaseAndTables.sql` 會重建資料表，既有測試資料會被清除。

## 啟動專案

```powershell
dotnet restore .\EsunBank.FinancialProducts.slnx
dotnet build .\EsunBank.FinancialProducts.slnx
dotnet run --project .\EsunBank.FinancialProducts.Web\EsunBank.FinancialProducts.Web.csproj
```

啟動後開啟終端機顯示的 localhost URL。預設首頁為喜好金融商品清單。

## 驗證方式

建議手動確認以下流程：

- 清單頁可顯示 seed data。
- 可依使用者篩選喜好商品。
- 新增喜好商品後會導向明細頁。
- 修改產品名稱、價格、費率、數量與扣款帳號後，手續費與總扣款金額會重新計算。
- 刪除喜好商品後，清單不再顯示該筆資料。
- 空白產品名稱、不合法扣款帳號、非正數價格或數量會顯示表單驗證錯誤。
- 不存在的明細、修改或刪除 id 會回傳合理的 NotFound。

## 安全性與資料完整性

- POST actions 使用 `[ValidateAntiForgeryToken]`。
- Razor 預設 HTML encoding，降低 XSS 風險。
- 表單使用 DataAnnotations 驗證。
- Repository 透過 Dapper 參數化呼叫 Stored Procedure，不拼接 SQL 字串。
- SQL table constraints 與 Stored Procedure 會再次檢查金額、費率、數量與扣款帳號格式。
- Stored Procedure 使用 transaction 處理 Products 與 LikeLists 的一致性。

## 實作取捨

- 為了符合題目要求與保持範圍單純，本專案使用 Dapper + Stored Procedure，未引入 Entity Framework。
- 每筆喜好清單建立一筆商品資料，修改喜好資料時也同步更新該商品資料，用來展示多表 transaction。
- 刪除喜好清單時，若商品未被其他喜好清單使用，會一併刪除商品資料。
- 目前以 take-home demo 為主，尚未加入登入授權、自動化測試與 production 等級的錯誤處理。
