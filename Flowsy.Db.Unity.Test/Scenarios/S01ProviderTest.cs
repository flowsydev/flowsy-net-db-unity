using System.Data.Common;
using Flowsy.Core;
using Flowsy.Db.Unity.Conventions;
using Flowsy.Db.Unity.Test.Extensions;
using Flowsy.Db.Unity.Test.Mock.Model;
using MySql.Data.MySqlClient;
using Npgsql;
using Shouldly;
using Xunit.Abstractions;
using Xunit.Extensions.Ordering;

namespace Flowsy.Db.Unity.Test.Scenarios;

[Collection(Collections.ConventionSet), Order(1)]
public class S01ProviderTest
{
    private readonly GlobalModel _globalModel;
    private readonly ITestOutputHelper _output;

    public S01ProviderTest(GlobalModel globalModel, ITestOutputHelper output)
    {
        _globalModel = globalModel;
        _output = output;
    }

    [Theory, Order(1)]
    [InlineData(DbProviderFamily.Postgres)]
    [InlineData(DbProviderFamily.MySql)]
    public void T01_Should_Create_Postgres_Provider(DbProviderFamily providerFamily)
    {
        // Arrange
        DbProviderDescriptor? provider = null;
        Exception? exception = null;
        
        // Act
        try
        {
            var invariantName = providerFamily switch
            {
                DbProviderFamily.Postgres => "Npgsql",
                DbProviderFamily.MySql => "MySql.Data.MySqlClient",
                _ => throw new ArgumentOutOfRangeException(nameof(providerFamily), providerFamily, null)
            };
            DbProviderFactory factoryInstance = providerFamily switch
            {
                DbProviderFamily.Postgres => NpgsqlFactory.Instance,
                DbProviderFamily.MySql => MySqlClientFactory.Instance,
                _ => throw new ArgumentOutOfRangeException(nameof(providerFamily), providerFamily, null)
            };
            provider = new DbProviderDescriptor(providerFamily, invariantName, factoryInstance);
            
            _output.WriteHeader(provider.GetType().Name);
            _output.WriteLine("Family: {0}", provider.Family);
            _output.WriteLine("InvariantName: {0}", provider.InvariantName);
            _output.WriteLine("Factory: {0}", provider.Factory?.GetType().Name);
            _output.WriteLine("ObjectSeparator: {0}", provider.ObjectSeparator);
        }
        catch (Exception ex)
        {
            exception = ex;
        }
        
        // Assert
        exception.ShouldBeNull();
        provider.ShouldNotBeNull();

        switch (providerFamily)
        {
            case DbProviderFamily.Postgres:
            {
                provider.Family.ShouldBe(DbProviderFamily.Postgres);
                provider.InvariantName.ShouldBe("Npgsql");
                provider.Factory.ShouldBeAssignableTo<NpgsqlFactory>();
                break;
            }
            
            case DbProviderFamily.MySql:
            {
                provider.Family.ShouldBe(DbProviderFamily.MySql);
                provider.InvariantName.ShouldBe("MySql.Data.MySqlClient");
                provider.Factory.ShouldBeAssignableTo<MySqlClientFactory>();
                break;
            }
        }
        
        _globalModel.Providers[providerFamily] = provider;
    }
    
    [Theory, Order(3)]
    [InlineData(DbProviderFamily.Postgres, "car_rental.quoting.quote_inquiry")]
    [InlineData(DbProviderFamily.MySql, "car_rental.globalization.country")]
    public void T03_Should_Parse_ObjectName(DbProviderFamily providerFamily, string objectName)
    {
        // Arrange
        _globalModel.Providers.TryGetValue(providerFamily, out var provider);
        provider.ShouldNotBeNull();

        DbFullyQualifiedName? fullyQualifiedName = null;
        Exception? exception = null;

        // Act
        try
        {
            fullyQualifiedName = provider.ParseObjectName(objectName);
            _output.WriteFullPath(fullyQualifiedName);
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        // Assert
        exception.ShouldBeNull();
        fullyQualifiedName.ShouldNotBeNull();
    }

    public static object[][] GetRoutines() =>
    [
        [
            DbProviderFamily.Postgres,
            new ValueTuple<string, DbRoutineType?, bool, object?>[]
            {
                new ("crm.cst_create", DbRoutineType.StoredFunction, false, new
                {
                    CustomerId = Guid.NewGuid(),
                    Name = "John Doe",
                    Email = "john.doe@example.com",
                    PreferredCurrency = Currency.Usd,
                    PreferredCurrencies = new [] { Currency.Usd, Currency.Eur },
                }),
                new ("crm.cst_remove", DbRoutineType.StoredProcedure, false, new
                {
                    CustomerId = Guid.NewGuid(),
                }),
                new ("crm.cst_get_by_filter", DbRoutineType.StoredFunction, true, new
                {
                    SearchTerm = "@example.com",
                    PageSize = 100,
                }),
            }
        ],
        [
            DbProviderFamily.MySql,
            new ValueTuple<string, DbRoutineType?, bool, object?>[]
            {
                new ("cst_create", DbRoutineType.StoredProcedure, false, new
                {
                    CustomerId = Guid.NewGuid(),
                    Name = "John Doe",
                    Email = "john.doe@example.com",
                    PreferredCurrency = Currency.Usd,
                    PreferredCurrencies = new [] { Currency.Usd, Currency.Eur }
                }),
                new ("cst_remove", DbRoutineType.StoredProcedure, false, new
                {
                    CustomerId = Guid.NewGuid(),
                }),
                new ("cst_get_by_filter", DbRoutineType.StoredProcedure, true, new
                {
                    SearchTerm = "@example.com",
                    PageSize = 100,
                }),
            }
        ],
    ];

    [Theory, Order(4)]
    [MemberData(nameof(GetRoutines))]
    public void T04_Should_Describe_Routines(
        DbProviderFamily providerFamily, (string RoutineName, DbRoutineType? RoutineType, bool ReturnsTable, object? Parameters)[] routines
        )
    {
        // Arrange
        _globalModel.Providers.TryGetValue(providerFamily, out var provider);
        provider.ShouldNotBeNull();

        var conventions = DbConventionSet.CreateBuilder(provider)
            .UseDefaultCaseStyle(CaseStyle.LowerSnakeCase)
            .ForRoutines()
            .UseProcedures(useNamedParameters: provider.SupportsNamedParameters)
            .ForParameters()
            .UsePrefix("p_")
            .ForEnums()
            .UseNames(CaseStyle.UpperSnakeCase)
            .UseMapping<Currency>(databaseTypeName: provider.SupportsEnumsAsCustomTypes ? "kernel.currency" : null)
            .Build();
        
        // Act
        foreach (var (routineName, routineType, returnsTable, parameters) in routines)
        {
            var routineDescriptor = conventions.Routines.BuildDescriptor(routineName, routineType, parameters, null, returnsTable);
            _output.Write(routineDescriptor);
        }
    }
}