using EsunBank.FinancialProducts.Service.Services;
using EsunBank.FinancialProducts.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EsunBank.FinancialProducts.Web.Controllers;

public sealed class LikeProductsController(ILikeProductService likeProductService) : Controller
{
    public async Task<IActionResult> Index(string? userId)
    {
        var result = await likeProductService.GetIndexAsync(userId);
        return View(MapIndex(result));
    }

    public async Task<IActionResult> Create()
    {
        var result = await likeProductService.BuildCreateFormAsync();
        return View(MapForm(result));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LikeProductFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Users = await GetUserViewModelsAsync();
            return View(model);
        }

        var result = await likeProductService.CreateAsync(MapInfo(model));
        if (!result.IsSuccess)
        {
            AddServiceErrors(result.Errors);
            model.Users = await GetUserViewModelsAsync();
            return View(model);
        }

        TempData["SuccessMessage"] = "喜好金融商品已新增。";
        return RedirectToAction(nameof(Details), new { id = result.Sn!.Value });
    }

    public async Task<IActionResult> Details(int id)
    {
        var result = await likeProductService.GetDetailAsync(id);
        return result is null ? NotFound() : View(MapListItem(result));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var result = await likeProductService.BuildEditFormAsync(id);
        return result is null ? NotFound() : View(MapForm(result));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, LikeProductFormViewModel model)
    {
        if (model.Sn is not null && model.Sn != id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            model.Sn = id;
            model.Users = await GetUserViewModelsAsync();
            return View(model);
        }

        var result = await likeProductService.UpdateAsync(id, MapInfo(model));
        if (!result.IsSuccess)
        {
            AddServiceErrors(result.Errors);
            model.Sn = id;
            model.Users = await GetUserViewModelsAsync();
            return View(model);
        }

        TempData["SuccessMessage"] = "喜好金融商品已更新。";
        return RedirectToAction(nameof(Details), new { id });
    }

    public async Task<IActionResult> Delete(int id)
    {
        var result = await likeProductService.GetDetailAsync(id);
        return result is null ? NotFound() : View(MapListItem(result));
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var isDeleted = await likeProductService.DeleteAsync(id);
        if (!isDeleted)
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = "喜好金融商品已刪除。";
        return RedirectToAction(nameof(Index));
    }

    private async Task<IReadOnlyList<UserOptionViewModel>> GetUserViewModelsAsync()
    {
        var users = await likeProductService.GetUsersAsync();
        return users.Select(MapUser).ToList();
    }

    private static LikeProductIndexViewModel MapIndex(LikeProductIndexResult result)
    {
        return new LikeProductIndexViewModel
        {
            SelectedUserId = result.SelectedUserId,
            Users = result.Users.Select(MapUser).ToList(),
            Items = result.Items.Select(MapListItem).ToList()
        };
    }

    private static LikeProductFormViewModel MapForm(LikeProductFormResult result)
    {
        return new LikeProductFormViewModel
        {
            Sn = result.Sn,
            UserId = result.UserId,
            ProductName = result.ProductName,
            Price = result.Price,
            FeeRate = result.FeeRate,
            Account = result.Account,
            PurchaseQuantity = result.PurchaseQuantity,
            Users = result.Users.Select(MapUser).ToList()
        };
    }

    private static UserOptionViewModel MapUser(UserOptionResult user)
    {
        return new UserOptionViewModel
        {
            UserId = user.UserId,
            UserName = user.UserName,
            Email = user.Email,
            Account = user.Account
        };
    }

    private static LikeProductListItemViewModel MapListItem(LikeProductResult item)
    {
        return new LikeProductListItemViewModel
        {
            Sn = item.Sn,
            UserId = item.UserId,
            UserName = item.UserName,
            Email = item.Email,
            DefaultAccount = item.DefaultAccount,
            DebitAccount = item.DebitAccount,
            ProductNo = item.ProductNo,
            ProductName = item.ProductName,
            Price = item.Price,
            FeeRate = item.FeeRate,
            PurchaseQuantity = item.PurchaseQuantity,
            TotalFee = item.TotalFee,
            TotalAmount = item.TotalAmount
        };
    }

    private static LikeProductInfo MapInfo(LikeProductFormViewModel model)
    {
        return new LikeProductInfo
        {
            UserId = model.UserId,
            ProductName = model.ProductName,
            Price = model.Price,
            FeeRate = model.FeeRate,
            Account = model.Account,
            PurchaseQuantity = model.PurchaseQuantity
        };
    }

    private void AddServiceErrors(IReadOnlyList<LikeProductValidationError> errors)
    {
        foreach (var error in errors)
        {
            ModelState.AddModelError(error.Field, error.Message);
        }
    }
}
