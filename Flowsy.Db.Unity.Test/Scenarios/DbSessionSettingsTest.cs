using System.Data;
using Microsoft.Data.Sqlite;
using Shouldly;

namespace Flowsy.Db.Unity.Test.Scenarios;

public class DbSessionSettingsTest
{
    [Fact]
    public async Task Session_Settings_Should_Restore_Non_Default_Values()
    {
        // Arrange
        var token = TestContext.Current.CancellationToken;
        await using var db = CreateSession();
        await db.ExecuteAsync("PRAGMA foreign_keys = 1; PRAGMA busy_timeout = 7000", cancellationToken: token);

        // Act
        await db.WithSettingsAsync(
            [new("foreign_keys", false), new("busy_timeout", 1500)],
            async (session, ct) =>
            {
                (await session.QuerySingleAsync<long>("PRAGMA foreign_keys", cancellationToken: ct)).ShouldBe(0);
                (await session.QuerySingleAsync<long>("PRAGMA busy_timeout", cancellationToken: ct)).ShouldBe(1500);
            }, cancellationToken: token);

        // Assert
        (await db.QuerySingleAsync<long>("PRAGMA foreign_keys", cancellationToken: token)).ShouldBe(1);
        (await db.QuerySingleAsync<long>("PRAGMA busy_timeout", cancellationToken: token)).ShouldBe(7000);
    }

    [Fact]
    public async Task Nested_And_Repeated_Settings_Should_Restore_Each_Previous_Value()
    {
        // Arrange
        var token = TestContext.Current.CancellationToken;
        await using var db = CreateSession();
        await db.ExecuteAsync("PRAGMA busy_timeout = 7000", cancellationToken: token);

        // Act
        await db.WithSettingsAsync([new("busy_timeout", 4000), new("busy_timeout", 3000)],
            async (session, ct) =>
            {
                await session.WithSettingsAsync([new("busy_timeout", 1000)],
                    async (nested, nestedToken) =>
                        (await nested.QuerySingleAsync<long>("PRAGMA busy_timeout", cancellationToken: nestedToken)).ShouldBe(1000), cancellationToken: ct);
                (await session.QuerySingleAsync<long>("PRAGMA busy_timeout", cancellationToken: ct)).ShouldBe(3000);
            }, cancellationToken: token);

        // Assert
        (await db.QuerySingleAsync<long>("PRAGMA busy_timeout", cancellationToken: token)).ShouldBe(7000);
    }

    [Fact]
    public async Task Session_Settings_Should_Restore_Values_When_The_Callback_Fails()
    {
        // Arrange
        var token = TestContext.Current.CancellationToken;
        await using var db = CreateSession();
        await db.ExecuteAsync("PRAGMA busy_timeout = 7000", cancellationToken: token);
        var original = new InvalidOperationException("Shopping cart processing failed.");

        // Act
        var exception = await Should.ThrowAsync<InvalidOperationException>(() => db.WithSettingsAsync(
            [new("busy_timeout", 1500)], (_, _) => Task.FromException(original), cancellationToken: token));

        // Assert
        exception.ShouldBeSameAs(original);
        (await db.QuerySingleAsync<long>("PRAGMA busy_timeout", cancellationToken: token)).ShouldBe(7000);
    }

    [Fact]
    public async Task Session_Settings_Should_Restore_Values_After_Cancellation()
    {
        // Arrange
        var token = TestContext.Current.CancellationToken;
        using var cancellation = CancellationTokenSource.CreateLinkedTokenSource(token);
        await using var db = CreateSession();
        await db.ExecuteAsync("PRAGMA busy_timeout = 7000", cancellationToken: token);

        // Act
        await Should.ThrowAsync<OperationCanceledException>(() => db.WithSettingsAsync(
            [new("busy_timeout", 1500)], (_, ct) =>
            {
                cancellation.Cancel();
                ct.ThrowIfCancellationRequested();
                return Task.CompletedTask;
            }, cancellation.Token));

        // Assert
        (await db.QuerySingleAsync<long>("PRAGMA busy_timeout", cancellationToken: token)).ShouldBe(7000);
    }

    [Fact]
    public async Task Session_Settings_Should_Restore_Applied_Values_When_A_Later_Read_Fails()
    {
        // Arrange
        var token = TestContext.Current.CancellationToken;
        await using var db = CreateSession(new FailingSettingFormatter(failRead: true));
        await db.ExecuteAsync("PRAGMA foreign_keys = 1", cancellationToken: token);
        var callbackRan = false;

        // Act
        await Should.ThrowAsync<SqliteException>(() => db.WithSettingsAsync(
            [new("foreign_keys", false), new("busy_timeout", 1500)], (_, _) =>
            {
                callbackRan = true;
                return Task.CompletedTask;
            }, cancellationToken: token));

        // Assert
        callbackRan.ShouldBeFalse();
        (await db.QuerySingleAsync<long>("PRAGMA foreign_keys", cancellationToken: token)).ShouldBe(1);
    }

    [Fact]
    public async Task Session_Settings_Should_Continue_Cleanup_After_A_Restoration_Fails()
    {
        // Arrange
        var token = TestContext.Current.CancellationToken;
        await using var db = CreateSession(new FailingSettingFormatter());
        await db.ExecuteAsync("PRAGMA foreign_keys = 1", cancellationToken: token);

        // Act
        await Should.ThrowAsync<SqliteException>(() => db.WithSettingsAsync(
            [new("foreign_keys", false), new("busy_timeout", 1500)], (_, _) => Task.CompletedTask, cancellationToken: token));

        // Assert
        (await db.QuerySingleAsync<long>("PRAGMA foreign_keys", cancellationToken: token)).ShouldBe(1);
    }

    [Fact]
    public async Task Session_Settings_Should_Preserve_Work_And_Cleanup_Errors()
    {
        // Arrange
        var token = TestContext.Current.CancellationToken;
        await using var db = CreateSession(new FailingSettingFormatter());
        await db.ExecuteAsync("PRAGMA foreign_keys = 1", cancellationToken: token);
        var original = new InvalidOperationException("Shopping cart processing failed.");

        // Act
        var exception = await Should.ThrowAsync<AggregateException>(() => db.WithSettingsAsync(
            [new("foreign_keys", false), new("busy_timeout", 1500)], (_, _) => Task.FromException(original), cancellationToken: token));

        // Assert
        exception.InnerExceptions.Count.ShouldBe(2);
        exception.InnerExceptions[0].ShouldBeSameAs(original);
        exception.InnerExceptions[1].ShouldBeOfType<SqliteException>();
        (await db.QuerySingleAsync<long>("PRAGMA foreign_keys", cancellationToken: token)).ShouldBe(1);
    }

    [Fact]
    public async Task Session_Settings_Should_Report_All_Cleanup_Errors()
    {
        // Arrange
        var token = TestContext.Current.CancellationToken;
        await using var db = CreateSession(new FailingSettingFormatter(failAllRestorations: true));

        // Act
        var exception = await Should.ThrowAsync<AggregateException>(() => db.WithSettingsAsync(
            [new("foreign_keys", false), new("busy_timeout", 1500)], (_, _) => Task.CompletedTask, cancellationToken: token));

        // Assert
        exception.InnerExceptions.Count.ShouldBe(2);
        exception.InnerExceptions.ShouldAllBe(error => error is SqliteException);
    }

    [Theory]
    [InlineData(DbProviderFamily.Postgres, "application_name", "SELECT current_setting('application_name')")]
    [InlineData(DbProviderFamily.MySql, "time_zone", "SELECT @@SESSION.time_zone")]
    [InlineData(DbProviderFamily.SqlServer, "lock_timeout", "SELECT lock_timeout FROM sys.dm_exec_sessions WHERE session_id = @@SPID")]
    [InlineData(DbProviderFamily.Oracle, "current_schema", "SELECT SYS_CONTEXT('USERENV', 'CURRENT_SCHEMA') FROM DUAL")]
    [InlineData(DbProviderFamily.Db2, "current_schema", "VALUES CURRENT SCHEMA")]
    public void Supported_Providers_Should_Capture_And_Restore_Current_Settings(
        DbProviderFamily family, string name, string readStatement)
    {
        // Arrange
        var configuration = new DbConnectionConfiguration("ShoppingDatabase", "test", new DbProviderDescriptor(family));
        var formatter = new DbSessionSettingFormatter();
        object initial = family == DbProviderFamily.SqlServer ? 7000 : "SHOPPING";
        object scoped = family == DbProviderFamily.SqlServer ? 1500 : "CART";

        // Act
        var command = formatter.Format(new(name.ToUpperInvariant(), scoped), configuration);
        var restore = command.RestoreStatementFactory!(initial);

        // Assert
        command.ReadStatement.ShouldBe(readStatement);
        restore.ShouldBe(formatter.Format(new(name, initial), configuration).ApplyStatement);
        restore.ShouldNotBe(command.ApplyStatement);
    }

    [Theory]
    [InlineData("LOW", "SET deadlock_priority LOW")]
    [InlineData("normal", "SET deadlock_priority NORMAL")]
    [InlineData("HIGH", "SET deadlock_priority HIGH")]
    [InlineData("-5", "SET deadlock_priority -5")]
    public void SqlServer_Deadlock_Priority_Should_Use_Valid_Keywords_Or_Numbers(string value, string statement)
    {
        // Arrange
        var configuration = new DbConnectionConfiguration("ShoppingDatabase", "test", new(DbProviderFamily.SqlServer));

        // Act
        var command = new DbSessionSettingFormatter().Format(new("deadlock_priority", value), configuration);

        // Assert
        command.ApplyStatement.ShouldBe(statement);
    }

    [Theory]
    [InlineData(DbRoutineType.StoredProcedure, false, "CALL shopping.sp_cart(@p_id)")]
    [InlineData(DbRoutineType.StoredFunction, false, "VALUES shopping.fn_cart(@p_id)")]
    [InlineData(DbRoutineType.StoredFunction, true, "SELECT * FROM TABLE(shopping.fn_cart(@p_id)) AS routine_result")]
    public void Db2_Should_Format_Shopping_Cart_Routines(DbRoutineType routineType, bool returnsTable, string statement)
    {
        // Arrange
        var provider = new DbProviderDescriptor(DbProviderFamily.Db2);
        var routine = routineType == DbRoutineType.StoredProcedure ? "shopping.sp_cart" : "shopping.fn_cart";
        var parameter = new DbParameterDescriptor(provider, "p_id", typeof(int), DbType.Int32);

        // Act
        var result = provider.FormatRoutineCall(routine, routineType, returnsTable: returnsTable, parameters: [parameter]);

        // Assert
        result.ShouldBe(statement);
        provider.SupportsSchemas.ShouldBeTrue();
        provider.SupportsRoutineType(routineType).ShouldBeTrue();
        provider.RoutineCanReturnTable(routineType).ShouldBeTrue();
        provider.FormatNamedParameter("p_id", "@p_id").ShouldBe("p_id => @p_id");
    }

    [Fact]
    public void Provider_Family_Values_Should_Remain_Stable()
    {
        // Arrange / Act / Assert
        ((int)DbProviderFamily.Generic).ShouldBe(0);
        ((int)DbProviderFamily.Postgres).ShouldBe(1);
        ((int)DbProviderFamily.MySql).ShouldBe(2);
        ((int)DbProviderFamily.SqlServer).ShouldBe(3);
        ((int)DbProviderFamily.Oracle).ShouldBe(4);
        ((int)DbProviderFamily.Sqlite).ShouldBe(5);
        ((int)DbProviderFamily.Db2).ShouldBe(6);
    }

    private static DbSession CreateSession(IDbSessionSettingFormatter? formatter = null)
        => new(new SqliteConnection("Data Source=:memory:"), DbConnectionUsage.Exclusive,
            new DbConnectionConfiguration("ShoppingDatabase", "Data Source=:memory:",
                new(DbProviderFamily.Sqlite, "Microsoft.Data.Sqlite", SqliteFactory.Instance)),
            sessionSettingFormatter: formatter);

    private sealed class FailingSettingFormatter(bool failRead = false, bool failAllRestorations = false) : IDbSessionSettingFormatter
    {
        public DbSessionSettingCommand Format(DbSessionSetting setting, DbConnectionConfiguration configuration)
        {
            var command = new DbSessionSettingFormatter().Format(setting, configuration);
            if (setting.Name != "busy_timeout" && !failAllRestorations)
                return command;
            return failRead
                ? command with { ReadStatement = "SELECT value FROM missing_setting" }
                : command with { RestoreStatementFactory = _ => "INVALID SQL" };
        }
    }
}
