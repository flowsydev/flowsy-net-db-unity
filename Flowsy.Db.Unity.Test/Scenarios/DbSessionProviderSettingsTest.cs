using Flowsy.Db.Unity.Test.Mock;
using Flowsy.Db.Unity.Test.Mock.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Flowsy.Db.Unity.Test.Scenarios;

public class DbSessionProviderSettingsTest(ServiceHost host, ITestOutputHelper output)
{
    [Fact]
    public async Task Postgres_Settings_Should_Restore_Shopping_Cart_Context()
    {
        // Arrange
        var token = TestContext.Current.CancellationToken;
        await using var scope = host.CreateAsyncScope();
        var hub = scope.ServiceProvider.GetRequiredService<IDbConnectionHub>();
        await using var db = await hub.CreateSessionAsync(DbConnections.Postgres, DbConnectionUsage.Exclusive, cancellationToken: token);
        await db.ExecuteAsync("SET application_name TO 'shopping-cart'; SET search_path TO shopping, public", cancellationToken: token);
        var originalPath = await db.QuerySingleAsync<string>("SELECT current_setting('search_path')", cancellationToken: token);

        // Act
        var name = await db.WithSettingsAsync(
            [new("application_name", "cart's checkout"), new("search_path", "public, shopping")],
            async (session, ct) =>
            {
                (await session.QuerySingleAsync<string>("SELECT current_setting('search_path')", cancellationToken: ct))
                    .ShouldBe("public, shopping");
                return await session.QuerySingleAsync<string>("SELECT current_setting('application_name')", cancellationToken: ct);
            }, cancellationToken: token);

        // Assert
        name.ShouldBe("cart's checkout");
        (await db.QuerySingleAsync<string>("SELECT current_setting('application_name')", cancellationToken: token)).ShouldBe("shopping-cart");
        (await db.QuerySingleAsync<string>("SELECT current_setting('search_path')", cancellationToken: token)).ShouldBe(originalPath);
        output.WriteLine("PostgreSQL session settings restored successfully.");
    }

    [Fact]
    public async Task MySql_Settings_Should_Restore_Shopping_Cart_Context()
    {
        // Arrange
        var token = TestContext.Current.CancellationToken;
        await using var scope = host.CreateAsyncScope();
        var hub = scope.ServiceProvider.GetRequiredService<IDbConnectionHub>();
        await using var db = await hub.CreateSessionAsync(DbConnections.MySql, DbConnectionUsage.Exclusive, cancellationToken: token);
        db.Configuration.Provider.Family.ShouldBe(DbProviderFamily.MySql);
        await db.ExecuteAsync("SET SESSION sql_mode = 'ANSI_QUOTES'; SET SESSION time_zone = '+01:00'", cancellationToken: token);
        var originalMode = await db.QuerySingleAsync<string>("SELECT @@SESSION.sql_mode", cancellationToken: token);

        // Act
        await db.WithSettingsAsync([new("sql_mode", ""), new("time_zone", "+00:00")],
            async (session, ct) =>
            {
                (await session.QuerySingleAsync<string>("SELECT @@SESSION.sql_mode", cancellationToken: ct)).ShouldBe("");
                (await session.QuerySingleAsync<string>("SELECT @@SESSION.time_zone", cancellationToken: ct)).ShouldBe("+00:00");
            }, cancellationToken: token);

        // Assert
        (await db.QuerySingleAsync<string>("SELECT @@SESSION.sql_mode", cancellationToken: token)).ShouldBe(originalMode);
        (await db.QuerySingleAsync<string>("SELECT @@SESSION.time_zone", cancellationToken: token)).ShouldBe("+01:00");
        output.WriteLine("MySQL session settings restored successfully.");
    }
}
