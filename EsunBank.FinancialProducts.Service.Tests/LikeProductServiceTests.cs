using EsunBank.FinancialProducts.Repository.Repositories;
using EsunBank.FinancialProducts.Service.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace EsunBank.FinancialProducts.Service.Tests;

public sealed class LikeProductServiceTests
{
    [Fact]
    public async Task GetIndexAsync_ShouldMapUsersAndItems()
    {
        var (service, repository) = CreateService();
        var users = new List<UserOptionDto> { CreateUser() };
        var items = new List<LikeProductDetailDto> { CreateDetail() };

        repository.Setup(x => x.GetUsersAsync()).ReturnsAsync(users);
        repository.Setup(x => x.GetListAsync("A1236456789")).ReturnsAsync(items);

        var result = await service.GetIndexAsync("A1236456789");

        result.SelectedUserId.Should().Be("A1236456789");
        result.Users.Should().ContainSingle().Which.Should().BeEquivalentTo(new
        {
            UserId = "A1236456789",
            UserName = "Wang Xiao Ming",
            Email = "test@email.com",
            Account = "1111999666"
        });
        result.Items.Should().ContainSingle().Which.Should().BeEquivalentTo(new
        {
            Sn = 10,
            UserId = "A1236456789",
            UserName = "Wang Xiao Ming",
            Email = "test@email.com",
            DefaultAccount = "1111999666",
            DebitAccount = "1111999666",
            ProductNo = 20,
            ProductName = "ESUN Fund",
            Price = 1000m,
            FeeRate = 0.01m,
            PurchaseQuantity = 2,
            TotalFee = 20m,
            TotalAmount = 2020m
        });
        repository.Verify(x => x.GetListAsync("A1236456789"), Times.Once);
    }

    [Fact]
    public async Task BuildCreateFormAsync_WhenUsersExist_ShouldUseFirstUserAsDefault()
    {
        var (service, repository) = CreateService();
        var users = new List<UserOptionDto>
        {
            CreateUser(),
            CreateUser(userId: "B2234567890", account: "2222888777")
        };

        repository.Setup(x => x.GetUsersAsync()).ReturnsAsync(users);

        var result = await service.BuildCreateFormAsync();

        result.UserId.Should().Be("A1236456789");
        result.Account.Should().Be("1111999666");
        result.PurchaseQuantity.Should().Be(1);
        result.Users.Should().HaveCount(2);
    }

    [Fact]
    public async Task BuildCreateFormAsync_WhenUsersDoNotExist_ShouldReturnEmptyDefaults()
    {
        var (service, repository) = CreateService();
        repository.Setup(x => x.GetUsersAsync()).ReturnsAsync([]);

        var result = await service.BuildCreateFormAsync();

        result.UserId.Should().BeEmpty();
        result.Account.Should().BeEmpty();
        result.PurchaseQuantity.Should().Be(1);
        result.Users.Should().BeEmpty();
    }

    [Fact]
    public async Task BuildEditFormAsync_WhenDetailDoesNotExist_ShouldReturnNull()
    {
        var (service, repository) = CreateService();
        repository.Setup(x => x.GetDetailAsync(10)).ReturnsAsync((LikeProductDetailDto?)null);

        var result = await service.BuildEditFormAsync(10);

        result.Should().BeNull();
    }

    [Fact]
    public async Task BuildEditFormAsync_WhenDetailExists_ShouldMapFormAndLoadUsers()
    {
        var (service, repository) = CreateService();
        repository.Setup(x => x.GetDetailAsync(10)).ReturnsAsync(CreateDetail());
        repository.Setup(x => x.GetUsersAsync()).ReturnsAsync([CreateUser()]);

        var result = await service.BuildEditFormAsync(10);

        result.Should().NotBeNull();
        result!.Should().BeEquivalentTo(new
        {
            Sn = 10,
            UserId = "A1236456789",
            ProductName = "ESUN Fund",
            Price = 1000m,
            FeeRate = 0.01m,
            Account = "1111999666",
            PurchaseQuantity = 2
        }, options => options.ExcludingMissingMembers());
        result!.Users.Should().ContainSingle();
    }

    [Fact]
    public async Task CreateAsync_ShouldTrimTextFieldsAndReturnCreatedSn()
    {
        var (service, repository) = CreateService();
        LikeProductCommand? capturedCommand = null;
        repository
            .Setup(x => x.CreateAsync(It.IsAny<LikeProductCommand>()))
            .Callback<LikeProductCommand>(command => capturedCommand = command)
            .ReturnsAsync(99);

        var result = await service.CreateAsync(new LikeProductInfo
        {
            UserId = " A1236456789 ",
            ProductName = " ESUN Fund ",
            Price = 1000m,
            FeeRate = 0.01m,
            Account = " 1111999666 ",
            PurchaseQuantity = 2
        });

        result.Should().Be(99);
        capturedCommand.Should().BeEquivalentTo(new
        {
            UserId = "A1236456789",
            ProductName = "ESUN Fund",
            Price = 1000m,
            FeeRate = 0.01m,
            Account = "1111999666",
            PurchaseQuantity = 2
        });
    }

    [Fact]
    public async Task UpdateAsync_ShouldPassSnAndTrimTextFields()
    {
        var (service, repository) = CreateService();
        LikeProductCommand? capturedCommand = null;
        repository
            .Setup(x => x.UpdateAsync(10, It.IsAny<LikeProductCommand>()))
            .Callback<int, LikeProductCommand>((_, command) => capturedCommand = command)
            .Returns(Task.CompletedTask);

        await service.UpdateAsync(10, new LikeProductInfo
        {
            UserId = " A1236456789 ",
            ProductName = " Updated Fund ",
            Price = 2000m,
            FeeRate = 0.02m,
            Account = " 1111999666 ",
            PurchaseQuantity = 3
        });

        capturedCommand.Should().BeEquivalentTo(new
        {
            UserId = "A1236456789",
            ProductName = "Updated Fund",
            Price = 2000m,
            FeeRate = 0.02m,
            Account = "1111999666",
            PurchaseQuantity = 3
        });
        repository.Verify(x => x.UpdateAsync(10, It.IsAny<LikeProductCommand>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenDetailDoesNotExist_ShouldReturnFalseAndNotDelete()
    {
        var (service, repository) = CreateService();
        repository.Setup(x => x.GetDetailAsync(10)).ReturnsAsync((LikeProductDetailDto?)null);

        var result = await service.DeleteAsync(10);

        result.Should().BeFalse();
        repository.Verify(x => x.DeleteAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenDetailExists_ShouldDeleteAndReturnTrue()
    {
        var (service, repository) = CreateService();
        repository.Setup(x => x.GetDetailAsync(10)).ReturnsAsync(CreateDetail());
        repository.Setup(x => x.DeleteAsync(10)).Returns(Task.CompletedTask);

        var result = await service.DeleteAsync(10);

        result.Should().BeTrue();
        repository.Verify(x => x.DeleteAsync(10), Times.Once);
    }

    private static (LikeProductService Service, Mock<ILikeProductRepository> Repository) CreateService()
    {
        var repository = new Mock<ILikeProductRepository>(MockBehavior.Strict);
        return (new LikeProductService(repository.Object), repository);
    }

    private static UserOptionDto CreateUser(
        string userId = "A1236456789",
        string userName = "Wang Xiao Ming",
        string email = "test@email.com",
        string account = "1111999666")
    {
        return new UserOptionDto
        {
            UserId = userId,
            UserName = userName,
            Email = email,
            Account = account
        };
    }

    private static LikeProductDetailDto CreateDetail()
    {
        return new LikeProductDetailDto
        {
            Sn = 10,
            UserId = "A1236456789",
            UserName = "Wang Xiao Ming",
            Email = "test@email.com",
            DefaultAccount = "1111999666",
            DebitAccount = "1111999666",
            ProductNo = 20,
            ProductName = "ESUN Fund",
            Price = 1000m,
            FeeRate = 0.01m,
            PurchaseQuantity = 2,
            TotalFee = 20m,
            TotalAmount = 2020m
        };
    }
}
