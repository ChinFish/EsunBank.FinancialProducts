using EsunBank.FinancialProducts.Repository.Repositories;

namespace EsunBank.FinancialProducts.Service.Services;

public sealed class LikeProductService(ILikeProductRepository repository) : ILikeProductService
{
    public async Task<IReadOnlyList<UserOptionResult>> GetUsersAsync()
    {
        var users = await repository.GetUsersAsync();
        return users.Select(MapUser).ToList();
    }

    public async Task<LikeProductIndexResult> GetIndexAsync(string? userId)
    {
        var users = await repository.GetUsersAsync();
        var items = await repository.GetListAsync(userId);

        return new LikeProductIndexResult
        {
            SelectedUserId = userId,
            Users = users.Select(MapUser).ToList(),
            Items = items.Select(MapListItem).ToList()
        };
    }

    public async Task<LikeProductFormResult> BuildCreateFormAsync()
    {
        var users = await GetUsersAsync();
        return new LikeProductFormResult
        {
            Users = users,
            UserId = users.FirstOrDefault()?.UserId ?? string.Empty,
            Account = users.FirstOrDefault()?.Account ?? string.Empty,
            PurchaseQuantity = 1
        };
    }

    public async Task<LikeProductFormResult?> BuildEditFormAsync(int sn)
    {
        var detail = await repository.GetDetailAsync(sn);
        if (detail is null)
        {
            return null;
        }

        return new LikeProductFormResult
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

    public async Task<LikeProductResult?> GetDetailAsync(int sn)
    {
        var detail = await repository.GetDetailAsync(sn);
        return detail is null ? null : MapListItem(detail);
    }

    public async Task<LikeProductSaveResult> CreateAsync(LikeProductInfo info)
    {
        var validation = await ValidateAsync(info);
        if (validation.Errors.Count > 0)
        {
            return LikeProductSaveResult.Failure(validation.Errors);
        }

        var sn = await repository.CreateAsync(validation.Command);
        return LikeProductSaveResult.Success(sn);
    }

    public async Task<LikeProductSaveResult> UpdateAsync(int sn, LikeProductInfo info)
    {
        var validation = await ValidateAsync(info);
        if (validation.Errors.Count > 0)
        {
            return LikeProductSaveResult.Failure(validation.Errors);
        }

        await repository.UpdateAsync(sn, validation.Command);
        return LikeProductSaveResult.Success(sn);
    }

    public async Task<bool> DeleteAsync(int sn)
    {
        var detail = await repository.GetDetailAsync(sn);
        if (detail is null)
        {
            return false;
        }

        await repository.DeleteAsync(sn);
        return true;
    }

    private static UserOptionResult MapUser(UserOptionDto user)
    {
        return new UserOptionResult
        {
            UserId = user.UserId,
            UserName = user.UserName,
            Email = user.Email,
            Account = user.Account
        };
    }

    private static LikeProductResult MapListItem(LikeProductDetailDto item)
    {
        return new LikeProductResult
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

    private static LikeProductCommand MapCommand(LikeProductInfo info)
    {
        return new LikeProductCommand
        {
            UserId = info.UserId?.Trim() ?? string.Empty,
            ProductName = info.ProductName?.Trim() ?? string.Empty,
            Price = info.Price,
            FeeRate = info.FeeRate,
            Account = info.Account?.Trim() ?? string.Empty,
            PurchaseQuantity = info.PurchaseQuantity
        };
    }

    private async Task<(LikeProductCommand Command, List<LikeProductValidationError> Errors)> ValidateAsync(
        LikeProductInfo info)
    {
        var command = MapCommand(info);
        var errors = new List<LikeProductValidationError>();

        await ValidateBusinessRulesAsync(command, errors);

        return (command, errors);
    }

    private async Task ValidateBusinessRulesAsync(
        LikeProductCommand command,
        List<LikeProductValidationError> errors)
    {
        var users = await repository.GetUsersAsync();
        if (!users.Any(user => user.UserId == command.UserId))
        {
            errors.Add(CreateError(nameof(LikeProductInfo.UserId), "選擇的使用者不存在"));
        }
    }

    private static LikeProductValidationError CreateError(string field, string message)
    {
        return new LikeProductValidationError
        {
            Field = field,
            Message = message
        };
    }
}
