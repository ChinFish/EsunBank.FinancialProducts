# AGENT 規範

本檔案定義 AI agent 在本專案中的工作方式。請優先遵守本檔案，其次才參考一般習慣或外部範例。

## 專案定位

- 這是玉山金控面試 take-home 專案：金融商品喜好紀錄系統。
- 技術棧為 ASP.NET Core 8 MVC、Razor Views、Bootstrap、SQL Server、Dapper。
- 資料庫存取必須走 Stored Procedure，SQL scripts 放在 `DB/`。
- 專案目標不是過度工程化，而是清楚展示可維護的 MVC 分層、資料驗證、安全性與 CRUD 流程。

## 溝通原則

- 一律使用繁體中文回覆。
- 回覆要直接、具體、可執行，避免空泛建議。
- 修改前先讀相關程式碼與 README，不要憑印象改。
- 若發現需求、README、程式碼或 SQL 互相矛盾，先指出矛盾與建議處理方向。
- 不要自動記錄或外洩面試公司細節、題目內容、連線字串、帳號或其他敏感資訊。

## 架構邊界

本專案架構以圖中的分層資料流為準：

```text
APP / Web
  -> Parameter DTO
  -> Controller
  -> Info DTO
  -> Service
  -> Condition DTO
  -> Repository
  -> Database / API

Database / API
  -> DataModel DTO
  -> Repository
  -> ResultModel DTO
  -> Service
  -> ViewModel DTO
  -> Controller
  -> APP / Web
```

- `Controllers/`：處理 HTTP flow、ModelState、Redirect、TempData、NotFound/BadRequest，並負責接收 Parameter DTO 或表單 ViewModel。
- `Services/`：負責商業流程、資料轉換與組裝。Controller 不應直接知道 Repository 的資料模型。
- `Repositories/`：只負責 Dapper 與 Stored Procedure 呼叫，不放 UI 或商業流程。Repository 對外回傳的資料應包成明確 DTO。
- `ViewModels/`：對應畫面顯示或表單需求，屬於 Controller 回傳給 View 的 ViewModel DTO。
- `Models/`：保留 domain/data shape，不直接綁定 Razor form，也不要讓 View 直接依賴資料庫模型。
- `Data/`：資料庫連線工廠與資料存取基礎設施。
- `Views/`：Razor 畫面與 Bootstrap 呈現，不放資料庫查詢或複雜商業邏輯。
- `DB/`：DDL、seed data、Stored Procedures。任何 schema 或 SP 變更都要同步更新此資料夾。
- `Common/` 若未來新增，只能放跨 Controller、Service、Repository 都合理共用的純工具、常數或基礎型別；不要把業務流程塞進 Common。

## DTO 流向規範

- 外部輸入到 Controller：使用 Parameter DTO 或表單 ViewModel，處理 model binding 與基本驗證。
- Controller 到 Service：轉成 Info DTO 或 service command，不要把 HTTP context、TempData、ViewBag 傳入 Service。
- Service 到 Repository：轉成 Condition DTO 或 repository command，只包含查詢或異動資料需要的條件。
- Repository 到 Service：回傳 DataModel DTO 或 repository DTO，代表 Stored Procedure 查出的資料形狀。
- Service 到 Controller/View：轉成 ResultModel DTO 或 ViewModel DTO，避免 View 直接依賴 Repository DTO。
- 不要跨層共用同一個 DTO 來省 mapping。若 DTO 用途不同，即使欄位相似也應分開命名與轉換。
- Mapping 應放在 Service 或鄰近該層的 private method，保持 Controller 薄、Repository 專注資料存取。

## 開發規範

- 維持現有 C# 風格：file-scoped namespace、nullable enable、async/await、primary constructor 可沿用。
- 優先使用 interface 注入既有服務，例如 `ILikeProductService`、`ILikeProductRepository`。
- 不要在 Controller 直接呼叫 Dapper、SQL 或 connection factory。
- 不要用字串拼接 SQL。所有資料庫操作都應以 Dapper 參數化呼叫 Stored Procedure。
- 新增資料異動時，Stored Procedure 需負責必要 transaction 與資料完整性檢查。
- 金額、數量、費率等規則要同時考慮前端表單驗證、ViewModel DataAnnotations、SQL constraints/SP 防護。
- Razor POST action 必須使用 `[ValidateAntiForgeryToken]`。
- 對使用者輸入的字串，在 service mapping 階段維持 trim 行為。
- 不要引入大型框架、ORM 或前端 build pipeline，除非需求明確要求且能說明必要性。

## 資料庫規範

- SQL scripts 執行順序固定為：
  1. `DB/01_CreateDatabaseAndTables.sql`
  2. `DB/02_SeedData.sql`
  3. `DB/03_StoredProcedures.sql`
- 若新增或修改 Stored Procedure，Repository method 名稱與 SP 名稱應保持可追蹤。
- 預設資料庫為 `EsunBankFinancialProducts`。
- 開發環境預設 connection string 位於 `appsettings.json`，不要提交個人帳密或正式環境連線資訊。

## 驗證流程

完成修改後至少執行：

```powershell
dotnet restore
dotnet build
```

若修改資料庫或 CRUD 流程，還要視情況執行：

```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -C -i "DB\01_CreateDatabaseAndTables.sql"
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -C -i "DB\02_SeedData.sql"
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -C -i "DB\03_StoredProcedures.sql"
dotnet run
```

手動檢查至少包含：

- 喜好商品清單可正常顯示。
- 新增、查看、修改、刪除流程可完成。
- 表單驗證錯誤會正常顯示。
- 不合法 id 會回傳合理的 NotFound 或 BadRequest。
- 金額、手續費與扣款總額計算符合 README 假設。

## Git 與檔案管理

- 修改前先看 `git status --short`，避免覆蓋使用者未提交變更。
- 不要 revert 使用者既有變更，除非使用者明確要求。
- 不要提交 `bin/`、`obj/`、`.idea/`、個人輸出檔、面試題目 PDF 或敏感資料。
- Commit message 建議使用 Conventional Commits，例如：
  - `feat: add favorite product edit flow`
  - `fix: validate debit account format`
  - `docs: update database setup instructions`

## 面試交付取向

- 任何修改都要能向面試官說明「為什麼這樣設計」。
- 優先保留簡潔、清楚、可 demo 的實作，不做與題目無關的架構展示。
- 若需要做 trade-off，請在回覆中明確說明：
  - 做了什麼。
  - 為什麼這樣做。
  - 如何驗證。
  - 還有哪些限制或可改進處。
