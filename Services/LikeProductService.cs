using EsunBank.FinancialProducts.Repositories;
using EsunBank.FinancialProducts.ViewModels;

namespace EsunBank.FinancialProducts.Services;

public sealed class LikeProductService(ILikeProductRepository repository) : ILikeProductService
{
    public async Task<IReadOnlyList<UserOptionViewModel>> GetUsersAsync()
    {
        var users = await repository.GetUsersAsync();
        return users.Select(MapUser).ToList();
    }

    public async Task<LikeProductIndexViewModel> GetIndexAsync(string? userId)
    {
        var users = await repository.GetUsersAsync();
        var items = await repository.GetListAsync(userId);

        return new LikeProductIndexViewModel
        {
            SelectedUserId = userId,
            Users = users.Select(MapUser).ToList(),
            Items = items.Select(MapListItem).ToList()
        };
    }

    public async Task<LikeProductFormViewModel> BuildCreateFormAsync()
    {
        var users = await GetUsersAsync();
        return new LikeProductFormViewModel
        {
            Users = users,
            UserId = users.FirstOrDefault()?.UserId ?? string.Empty,
            Account = users.FirstOrDefault()?.Account ?? string.Empty,
            PurchaseQuantity = 1
        };
    }

    public async Task<LikeProductFormViewModel?> BuildEditFormAsync(int sn)
    {
        var detail = await repository.GetDetailAsync(sn);
        if (detail is null)
        {
            return null;
        }

        return new LikeProductFormViewModel
        {
            Sn = detail.Sn,
            UserId = detail.UserId,
            ProductName = detail.ProductName,
            Price = detail.Price,
            FeeRate = detail.FeeRate,
            Account = detail.DebitAccount,
            PurchaseQuantity = detail.PurchaseQuantity,
            Users = await GetUsersAsync()
        };
    }

    public async Task<LikeProductListItemViewModel?> GetDetailAsync(int sn)
    {
        var detail = await repository.GetDetailAsync(sn);
        return detail is null ? null : MapListItem(detail);
    }

    public Task<int> CreateAsync(LikeProductFormViewModel form)
    {
        return repository.CreateAsync(MapCommand(form));
    }

    public Task UpdateAsync(int sn, LikeProductFormViewModel form)
    {
        return repository.UpdateAsync(sn, MapCommand(form));
    }

    public Task DeleteAsync(int sn)
    {
        return repository.DeleteAsync(sn);
    }

    private static UserOptionViewModel MapUser(UserOptionDto user)
    {
        return new UserOptionViewModel
        {
            UserId = user.UserId,
            UserName = user.UserName,
            Email = user.Email,
            Account = user.Account
        };
    }

    private static LikeProductListItemViewModel MapListItem(LikeProductDetailDto item)
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

    private static LikeProductCommand MapCommand(LikeProductFormViewModel form)
    {
        return new LikeProductCommand
        {
            UserId = form.UserId.Trim(),
            ProductName = form.ProductName.Trim(),
            Price = form.Price,
            FeeRate = form.FeeRate,
            Account = form.Account.Trim(),
            PurchaseQuantity = form.PurchaseQuantity
        };
    }
}
