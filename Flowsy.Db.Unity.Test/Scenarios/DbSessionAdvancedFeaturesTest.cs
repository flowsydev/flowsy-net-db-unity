using System.Data;
using System.Diagnostics;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Flowsy.Db.Unity.Postgres;
using Flowsy.Db.Unity.Conventions;
using Shouldly;

namespace Flowsy.Db.Unity.Test.Scenarios;

public class DbSessionAdvancedFeaturesTest
{
    [Fact]
    public void Convention_Type_Map_Should_Match_Constructors_Without_Reading_Past_Their_Parameters()
    {
        // Arrange
        var typeMap = new DbConventionTypeMap(
            typeof(ConstructorMappedProduct),
            new DbObjectConvention(DbCaseStyle.LowerSnakeCase));

        // Act
        var matchingConstructor = typeMap.FindConstructor(
            ["product_id", "name"],
            [typeof(int), typeof(string)]);
        var mismatchedConstructor = typeMap.FindConstructor(
            ["product_id"],
            [typeof(int), typeof(string)]);

        // Assert
        matchingConstructor.ShouldNotBeNull();
        mismatchedConstructor.ShouldBeNull();
    }

    private sealed record ConstructorMappedProduct(int ProductId, string Name);

    [Fact]
    public async Task Transaction_Helper_Should_Commit_Shopping_Cart_Changes()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await using var db = CreateSession(connection);
        await db.ExecuteAsync("CREATE TABLE shopping_cart (id INTEGER PRIMARY KEY, status TEXT NOT NULL)", cancellationToken: cancellationToken);

        // Act
        var id = await db.InTransactionAsync(async (session, cancellationToken) =>
        {
            await session.ExecuteAsync(
                "INSERT INTO shopping_cart (id, status) VALUES (@Id, @Status)",
                new { Id = 7, Status = "open" },
                cancellationToken);
            return 7;
        }, cancellationToken);

        // Assert
        id.ShouldBe(7);
        (await db.QuerySingleAsync<int>("SELECT COUNT(*) FROM shopping_cart", cancellationToken: cancellationToken)).ShouldBe(1);
    }

    [Fact]
    public async Task Transaction_Helper_Should_Roll_Back_Failed_Shopping_Cart_Changes()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await using var db = CreateSession(connection);
        await db.ExecuteAsync("CREATE TABLE shopping_cart (id INTEGER PRIMARY KEY)", cancellationToken: cancellationToken);

        // Act
        var exception = await Should.ThrowAsync<InvalidOperationException>(() =>
            db.InTransactionAsync(async (session, cancellationToken) =>
            {
                await session.ExecuteAsync("INSERT INTO shopping_cart (id) VALUES (1)", cancellationToken: cancellationToken);
                throw new InvalidOperationException("Simulated cart failure.");
            }, cancellationToken));

        // Assert
        exception.Message.ShouldContain("Simulated cart failure");
        db.InTransaction.ShouldBeFalse();
        (await db.QuerySingleAsync<int>("SELECT COUNT(*) FROM shopping_cart", cancellationToken: cancellationToken)).ShouldBe(0);
    }

    [Fact]
    public async Task Existing_Transaction_Helper_Should_Not_Complete_The_Outer_Transaction()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await using var db = CreateSession(connection);
        await db.ExecuteAsync("CREATE TABLE shopping_cart (id INTEGER PRIMARY KEY)", cancellationToken: cancellationToken);
        await db.BeginTransactionAsync(cancellationToken);

        // Act
        await db.InExistingOrNewTransactionAsync(
            (session, token) => session.ExecuteAsync("INSERT INTO shopping_cart (id) VALUES (1)", cancellationToken: token),
            cancellationToken);

        // Assert
        db.InTransaction.ShouldBeTrue();
        await db.RollbackTransactionAsync(cancellationToken);
        (await db.QuerySingleAsync<int>("SELECT COUNT(*) FROM shopping_cart", cancellationToken: cancellationToken)).ShouldBe(0);
    }

    [Fact]
    public async Task Native_Access_Should_Reuse_The_Session_Connection_And_Return_A_Result()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await using var db = CreateSession(connection);

        // Act
        var sameConnection = await db.WithConnectionAsync<SqliteConnection, bool>(
            (nativeConnection, transaction, _) =>
                Task.FromResult(ReferenceEquals(connection, nativeConnection) && transaction is null),
            cancellationToken);

        // Assert
        sameConnection.ShouldBeTrue();
        connection.State.ShouldBe(ConnectionState.Open);
    }

    [Fact]
    public async Task Streaming_Should_Read_All_Shopping_Carts_Progressively()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await using var db = CreateSession(connection);
        await db.ExecuteAsync("CREATE TABLE shopping_cart (id INTEGER PRIMARY KEY)", cancellationToken: cancellationToken);
        await db.ExecuteAsync("INSERT INTO shopping_cart (id) VALUES (1), (2), (3)", cancellationToken: cancellationToken);

        // Act
        var identifiers = new List<long>();
        await foreach (var identifier in db.StreamAsync<long>("SELECT id FROM shopping_cart ORDER BY id", cancellationToken: cancellationToken))
            identifiers.Add(identifier);

        // Assert
        identifiers.ShouldBe([1, 2, 3]);
    }

    [Fact]
    public async Task Query_Multiple_Callback_Should_Return_A_Result_And_Dispose_The_Reader()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await using var db = CreateSession(connection);
        SqlMapper.GridReader? capturedReader = null;

        // Act
        var result = await db.QueryMultipleAsync(
            "SELECT 2; SELECT 'open'",
            async (reader, _) =>
            {
                capturedReader = reader;
                var count = await reader.ReadSingleAsync<int>();
                var status = await reader.ReadSingleAsync<string>();
                return (count, status);
            }, cancellationToken: cancellationToken);

        // Assert
        result.ShouldBe((2, "open"));
        capturedReader.ShouldNotBeNull();
        capturedReader.IsConsumed.ShouldBeTrue();
    }

    [Fact]
    public async Task Write_Guard_Should_Require_A_Transaction_When_Enabled()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var connection = new SqliteConnection("Data Source=:memory:");
        var configuration = CreateConfiguration(requireTransactionForWrites: true);
        await using var db = new DbSession(connection, DbConnectionUsage.Exclusive, configuration);

        // Act
        var exception = await Should.ThrowAsync<InvalidOperationException>(
            () => db.ExecuteAsync("CREATE TABLE shopping_cart (id INTEGER PRIMARY KEY)", cancellationToken: cancellationToken));

        // Assert
        exception.Message.ShouldContain("requires an active transaction");
    }

    [Fact]
    public async Task Session_Settings_Should_Be_Cleaned_After_The_Callback()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await using var db = new DbSession(
            connection,
            DbConnectionUsage.Exclusive,
            CreateConfiguration(),
            sessionSettingFormatter: new SqliteTestSettingFormatter());

        // Act
        var enabledInsideScope = await db.WithSettingsAsync(
            [new DbSessionSetting("foreign_keys", true)],
            (session, token) => session.QuerySingleAsync<long>("PRAGMA foreign_keys", cancellationToken: token),
            cancellationToken);
        var enabledAfterScope = await db.QuerySingleAsync<long>("PRAGMA foreign_keys", cancellationToken: cancellationToken);

        // Assert
        enabledInsideScope.ShouldBe(1);
        enabledAfterScope.ShouldBe(0);
    }

    [Fact]
    public async Task Session_Settings_Should_Clean_Previously_Applied_Values_When_A_Later_Setting_Fails()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await using var db = new DbSession(
            connection,
            DbConnectionUsage.Exclusive,
            CreateConfiguration(),
            sessionSettingFormatter: new PartiallyFailingSqliteSettingFormatter());

        // Act
        await Should.ThrowAsync<SqliteException>(() => db.WithSettingsAsync(
            [new DbSessionSetting("foreign_keys", true), new DbSessionSetting("invalid_setting", true)],
            (_, _) => Task.CompletedTask,
            cancellationToken));
        var enabledAfterFailure = await db.QuerySingleAsync<long>(
            "PRAGMA foreign_keys",
            cancellationToken: cancellationToken);

        // Assert
        enabledAfterFailure.ShouldBe(0);
    }

    [Fact]
    public void Postgres_Extension_Should_Create_A_Connection_Through_The_Neutral_Provider_Model()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDatabases(options => options
            .UsePostgres(
                "ShoppingDatabase",
                "Host=localhost;Database=shopping;Username=test;Password=test")
            .AsDefault()
            .WithConventions()
            .WithDefault(DbCaseStyle.LowerSnakeCase));
        using var serviceProvider = services.BuildServiceProvider();
        var factory = serviceProvider.GetRequiredService<IDbConnectionFactory>();

        // Act
        using var connection = factory.GetConnection();

        // Assert
        connection.ShouldBeOfType<NpgsqlConnection>();
    }

    [Fact]
    public async Task Per_Call_Tag_Should_Be_Sanitized_In_The_Activity()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await using var db = CreateSession(connection);
        Activity? capturedActivity = null;
        using var listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == DbDiagnostics.InstrumentationName,
            Sample = static (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
            ActivityStopped = activity => capturedActivity = activity
        };
        ActivitySource.AddActivityListener(listener);

        // Act
        await db.QuerySingleAsync<int>(
            "SELECT @Value",
            new { Value = 1 },
            new DbSessionCallOptions { Tag = "shopping/cart ! 42" },
            cancellationToken);

        // Assert
        capturedActivity.ShouldNotBeNull();
        capturedActivity.GetTagItem("db.operation.tag").ShouldBe("shoppingcart42");
        capturedActivity.GetTagItem("db.statement").ShouldBeNull();
    }

    [Fact]
    public void Parameter_And_Enum_Mappings_Should_Be_Resolved_From_Configuration()
    {
        // Arrange
        var conventionBuilder = new DbConventionSetBuilder(CreateProvider());
        conventionBuilder
            .ForParameters(parameters => parameters
                .WithDefault(DbCaseStyle.LowerSnakeCase, "p_")
                .Map<CreateShoppingCartParameters, int>(x => x.ShoppingCartId, "p_cart_key"))
            .ForEnums(new DbEnumConvention(
                DbEnumValueFormat.Name,
                Mappings:
                [new DbEnumMapping<ShoppingCartStatus>(
                    null,
                    [(ShoppingCartStatus.AwaitingPayment, "waiting-payment")])]));
        var conventions = conventionBuilder.Build();

        // Act
        var descriptors = new DbParameterBuilder(
            conventions,
            new CreateShoppingCartParameters(15, ShoppingCartStatus.AwaitingPayment))
            .BuildDescriptors()
            .ToArray();
        conventions.Enums.Map(ShoppingCartStatus.AwaitingPayment, out _, out _, out var enumValue);

        // Assert
        descriptors.Single(x => x.RuntimeType == typeof(int)).Name.ShouldBe("p_cart_key");
        descriptors.Single(x => x.RuntimeType == typeof(ShoppingCartStatus)).Name.ShouldBe("p_status");
        enumValue.ShouldBe("waiting-payment");
    }

    [Fact]
    public void Postgres_Custom_Array_Type_Should_Append_The_Array_Suffix()
    {
        // Arrange
        var provider = new DbProviderDescriptor(DbProviderFamily.Postgres);

        // Act
        var arrayType = provider.FormatArrayType("shopping.shopping_cart_status");
        var missingType = provider.FormatArrayType(null);

        // Assert
        arrayType.ShouldBe("shopping.shopping_cart_status[]");
        missingType.ShouldBeNull();
    }

    [Fact]
    public async Task Global_Type_Handler_Should_Receive_The_Original_Value_And_Parse_The_Result()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        SqlMapper.AddTypeHandler(new ProductCodeTypeHandler());
        var configuration = CreateConfiguration();
        var descriptor = new DbParameterBuilder(
            configuration.Conventions,
            new { Code = new ProductCode("CART-42") })
            .BuildDescriptors()
            .Single();
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await using var db = new DbSession(connection, DbConnectionUsage.Exclusive, configuration);

        // Act
        var result = await db.QuerySingleAsync<ProductCode>(
            "SELECT @Code",
            new { Code = new ProductCode("CART-42") },
            cancellationToken);

        // Assert
        descriptor.UsesTypeHandler.ShouldBeTrue();
        descriptor.DatabaseType.ShouldBeNull();
        result.ShouldBe(new ProductCode("CART-42"));
    }

    [Fact]
    public async Task DateOnly_And_TimeOnly_Should_Round_Trip_With_The_Supported_Driver()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await using var db = CreateSession(connection);
        SqlMapper.AddTypeHandler(new DbDateOnlyTypeHandler());
        SqlMapper.AddTypeHandler(new DbTimeOnlyTypeHandler());
        var date = new DateOnly(2026, 8, 30);
        var time = new TimeOnly(14, 35, 12);

        // Act
        var result = await db.QuerySingleAsync<ModernTypesResult>(
            "SELECT @Date AS Date, @Time AS Time",
            new { Date = date, Time = time },
            cancellationToken);

        // Assert
        result.Date.ShouldBe(date);
        result.Time.ShouldBe(time);
    }

    private static DbSession CreateSession(SqliteConnection connection)
        => new(connection, DbConnectionUsage.Exclusive, CreateConfiguration());

    private static DbConnectionConfiguration CreateConfiguration(bool requireTransactionForWrites = false)
    {
        var configuration = new DbConnectionConfiguration(
            "ShoppingDatabase", "Data Source=:memory:", CreateProvider());
        if (!requireTransactionForWrites)
            return configuration;
        var builder = new DbConnectionConfigurationBuilderAccessor(
            "ShoppingDatabase", "Data Source=:memory:");
        return builder.Build(CreateProvider(), true);
    }

    private static DbProviderDescriptor CreateProvider()
        => new(DbProviderFamily.Sqlite, "Microsoft.Data.Sqlite", SqliteFactory.Instance);

    private sealed record CreateShoppingCartParameters(int ShoppingCartId, ShoppingCartStatus Status);
    private sealed record ModernTypesResult(DateOnly Date, TimeOnly Time);
    private readonly record struct ProductCode(string Value);
    private enum ShoppingCartStatus { Open, AwaitingPayment }

    private sealed class ProductCodeTypeHandler : SqlMapper.TypeHandler<ProductCode>
    {
        public override void SetValue(IDbDataParameter parameter, ProductCode value)
            => parameter.Value = value.Value;

        public override ProductCode Parse(object value) => new(Convert.ToString(value)!);
    }

    private sealed class SqliteTestSettingFormatter : IDbSessionSettingFormatter
    {
        public DbSessionSettingCommand Format(DbSessionSetting setting, DbConnectionConfiguration configuration)
            => new("PRAGMA foreign_keys = 1", "PRAGMA foreign_keys = 0");
    }


    private sealed class PartiallyFailingSqliteSettingFormatter : IDbSessionSettingFormatter
    {
        public DbSessionSettingCommand Format(DbSessionSetting setting, DbConnectionConfiguration configuration)
            => setting.Name == "foreign_keys"
                ? new("PRAGMA foreign_keys = 1", "PRAGMA foreign_keys = 0")
                : new("THIS IS NOT VALID SQL", "PRAGMA foreign_keys = 0");
    }

    private sealed class DbConnectionConfigurationBuilderAccessor
    {
        private readonly string _key;
        private readonly string _connectionString;

        public DbConnectionConfigurationBuilderAccessor(string key, string connectionString)
        {
            _key = key;
            _connectionString = connectionString;
        }

        public DbConnectionConfiguration Build(DbProviderDescriptor provider, bool guard)
        {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddDatabases(options => options
                .UseConnection(_key, _connectionString)
                .AsDefault()
                .WithProvider(provider.Family, provider.InvariantName!, provider.Factory!)
                .WithWriteTransactionGuard(guard));
            using var serviceProvider = services.BuildServiceProvider();
            return serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptionsMonitor<DbConnectionConfiguration>>().CurrentValue;
        }
    }
}
