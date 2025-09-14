namespace Flowsy.Db.Unity.Test.Mock.Model;

public record ProductCategoryOverview(Guid ProductCategoryId, string Code, string Name) : IReadModel;

public record ProductCategoryDetail(
    Guid ProductCategoryId, string Code, string Name, string? Description,
    DateTimeOffset CreationInstant, DateTimeOffset? LastMutationInstant
    ) : IReadModel;

public record ProductOverview(
    Guid ProductId, string Sku, string Name, decimal Price, Currency Currency, Guid ProductCategoryId
    ) : IReadModel;

public record ProductDetail(
    Guid ProductId, string Sku, string Name, string? Description, decimal Price, Currency Currency, Guid ProductCategoryId,
    DateTimeOffset CreationInstant, DateTimeOffset? LastMutationInstant
    ) : IReadModel;