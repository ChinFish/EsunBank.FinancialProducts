using System.ComponentModel.DataAnnotations;

namespace EsunBank.FinancialProducts.ViewModels;

public sealed class LikeProductFormViewModel
{
    public int? Sn { get; set; }

    [Display(Name = "使用者")]
    [Required(ErrorMessage = "請選擇使用者")]
    public string UserId { get; set; } = string.Empty;

    [Display(Name = "產品名稱")]
    [Required(ErrorMessage = "請輸入產品名稱")]
    [StringLength(100, ErrorMessage = "產品名稱不可超過 100 個字")]
    public string ProductName { get; set; } = string.Empty;

    [Display(Name = "產品價格")]
    [Range(1, 999999999, ErrorMessage = "產品價格需大於 0")]
    public decimal Price { get; set; }

    [Display(Name = "手續費率")]
    [Range(0, 1, ErrorMessage = "手續費率需介於 0 到 1 之間，例如 0.01 代表 1%")]
    public decimal FeeRate { get; set; }

    [Display(Name = "扣款帳號")]
    [Required(ErrorMessage = "請輸入扣款帳號")]
    [RegularExpression("^[0-9]{6,20}$", ErrorMessage = "扣款帳號需為 6 到 20 碼數字")]
    public string Account { get; set; } = string.Empty;

    [Display(Name = "購買數量")]
    [Range(1, 100000, ErrorMessage = "購買數量需大於 0")]
    public int PurchaseQuantity { get; set; } = 1;

    public IReadOnlyList<UserOptionViewModel> Users { get; set; } = [];
}
