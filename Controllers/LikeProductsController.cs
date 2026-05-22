using EsunBank.FinancialProducts.Services;
using EsunBank.FinancialProducts.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EsunBank.FinancialProducts.Controllers;

public sealed class LikeProductsController(ILikeProductService likeProductService) : Controller
{
    public async Task<IActionResult> Index(string? userId)
    {
        var model = await likeProductService.GetIndexAsync(userId);
        return View(model);
    }

    public async Task<IActionResult> Create()
    {
        var model = await likeProductService.BuildCreateFormAsync();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LikeProductFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Users = await likeProductService.GetUsersAsync();
            return View(model);
        }

        var sn = await likeProductService.CreateAsync(model);
        TempData["SuccessMessage"] = "喜好金融商品已新增。";
        return RedirectToAction(nameof(Details), new { id = sn });
    }

    public async Task<IActionResult> Details(int id)
    {
        var model = await likeProductService.GetDetailAsync(id);
        return model is null ? NotFound() : View(model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var model = await likeProductService.BuildEditFormAsync(id);
        return model is null ? NotFound() : View(model);
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
            model.Users = await likeProductService.GetUsersAsync();
            return View(model);
        }

        await likeProductService.UpdateAsync(id, model);
        TempData["SuccessMessage"] = "喜好金融商品已更新。";
        return RedirectToAction(nameof(Details), new { id });
    }

    public async Task<IActionResult> Delete(int id)
    {
        var model = await likeProductService.GetDetailAsync(id);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await likeProductService.DeleteAsync(id);
        TempData["SuccessMessage"] = "喜好金融商品已刪除。";
        return RedirectToAction(nameof(Index));
    }
}
