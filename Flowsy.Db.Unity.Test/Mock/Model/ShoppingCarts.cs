namespace Flowsy.Db.Unity.Test.Mock.Model;

public enum ShoppingCartStatus
{
    Open,
    CheckedOut,
    Abandoned
}

public record ShoppingCartOverview(
    Guid ShoppingCartId, 
    int ItemCount, double TotalItemQuantity, decimal Total,
    ShoppingCartStatus Status,
    DateTimeOffset CreationInstant, DateTimeOffset? LastMutationInstant
    ) : IReadModel;

public record ShoppingCartDetail(
    Guid ShoppingCartId, 
    Guid UserAccountId, string UserAccountPrincipalName, string UserAccountEmail, string UserAccountNickname,
    IEnumerable<ShoppingCartItemDetail> Items,
    int ItemCount, double TotalItemQuantity, decimal Total,
    ShoppingCartStatus Status,
    DateTimeOffset CreationInstant, DateTimeOffset? LastMutationInstant
    ) : IReadModel;

public record ShoppingCartItemDetail(
    Guid ShoppingCartItemLineId, 
    Guid ProductId, string ProductSku, string ProductName, decimal ProductPrice, Currency ProductCurrency,
    double Quantity, decimal LineTotal,
    DateTimeOffset CreationInstant, DateTimeOffset? LastMutationInstant
    ) : IReadModel;