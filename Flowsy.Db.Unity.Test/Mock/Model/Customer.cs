namespace Flowsy.Db.Unity.Test.Mock.Model;

public record Customer(
    int CustomerId, string Name, string Email, DateTimeOffset CreatedAt, DateTimeOffset? ModifiedAt
    ) : IReadModel;