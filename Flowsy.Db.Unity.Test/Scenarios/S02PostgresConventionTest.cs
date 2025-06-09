using System.Reflection;
using Dapper;
using Flowsy.Core;
using Flowsy.Db.Unity.Conventions;
using Flowsy.Db.Unity.Extensions;
using Flowsy.Db.Unity.Test.Extensions;
using Flowsy.Db.Unity.Test.Mock;
using Flowsy.Db.Unity.Test.Mock.Model;
using Npgsql;
using Shouldly;
using Xunit.Abstractions;
using Xunit.Extensions.Ordering;

namespace Flowsy.Db.Unity.Test.Scenarios;

[Collection(Collections.ConventionSet), Order(2)]
public class S02PostgresConventionTest : IClassFixture<S02PostgresConventionTest.LocalModel>
{
    public class LocalModel
    {
        public DbConnectionOptions? ConnectionOptions { get; set; }
    }
    
    private readonly ServiceHost _serviceHost;
    private readonly GlobalModel _globalModel;
    private readonly LocalModel _localModel;
    private readonly ITestOutputHelper _output;

    public S02PostgresConventionTest(ServiceHost serviceHost, GlobalModel globalModel, LocalModel localModel, ITestOutputHelper output)
    {
        _serviceHost = serviceHost;
        _globalModel = globalModel;
        _localModel = localModel;
        _output = output;
    }

    [Fact, Order(1)]
    public void T01_Should_Configure_PostgresConnection()
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        // Arrange
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        _globalModel.Providers.TryGetValue(DbProviderFamily.Postgres, out var provider);
        provider.ShouldNotBeNull();
        
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        // Act
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        var conventions = DbConventionSet.CreateBuilder(provider)
            .UseDefaultCaseStyle(CaseStyle.LowerSnakeCase)
            .ForRoutines()
            .UseFunctions(prefix: "fun_", useNamedParameters: true)
            .ForParameters()
            .UsePrefix("p_")
            .ForEnums()
            .UseNames(CaseStyle.UpperSnakeCase)
            .UseMapping<CustomerStatus>("crm.customer_status")
            .UseMapping<Currency>("kernel.currency")
            .Build();
        
        var options = new DbConnectionOptionsBuilder("Primary")
            .WithProvider(DbProviderFamily.Postgres, "Npgsql", NpgsqlFactory.Instance)
            .WithConnectionString(_serviceHost.PostgresConnectionString)
            .WithConventions(conventions)
            .Build();

        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        // Assert
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        
        options.ConnectionKey.ShouldBe("Primary");
        options.Provider.Family.ShouldBe(DbProviderFamily.Postgres);
        options.ConnectionString.ShouldBe(_serviceHost.PostgresConnectionString);
        
        options.Conventions.ShouldNotBeNull();
        
        options.Conventions.Provider.Family.ShouldBe(DbProviderFamily.Postgres);
        
        options.Conventions.DefaultCaseStyle.ShouldBe(CaseStyle.LowerSnakeCase);
        
        options.Conventions.Routines.PreferredType.ShouldBe(DbRoutineType.StoredFunction);
        options.Conventions.Routines.Functions.Naming.Prefix.ShouldBe("fun_");
        
        options.Conventions.Parameters.Naming.Prefix.ShouldBe("p_");
        
        options.Conventions.Enums.ValueFormat.ShouldBe(DbEnumFormat.Name);
        
        options.Conventions.Enums.NameTranslator.ShouldNotBeNull();
        options.Conventions.Enums.NameTranslator.MemberNameCaseStyle.ShouldBe(CaseStyle.UpperSnakeCase);

        var currencyMapping = options.Conventions.Enums.ResolveMapping<Currency>();
        currencyMapping.ShouldNotBeNull();
        currencyMapping.DatabaseTypeName.ShouldNotBeNull();
        currencyMapping.DatabaseTypeName.ToString().ShouldBe("kernel.currency");
        
        _localModel.ConnectionOptions = options;
    }

    [Fact, Order(3)]
    public void T03_Should_GetResults_FromRoutine_Sync()
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        // Arrange
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        var options = _localModel.ConnectionOptions;
        options.ShouldNotBeNull();
        
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        // Act
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        IList<Customer>? customers = null;
        Exception? exception = null;
        try
        {
            using var connection = options.CreateConnection();
            customers = connection.GetFromRoutine<Customer>(
                "crm.fun_cst_get_by_filter",
                new
                {
                    SearchTerm = "@example.com",
                    Status = null as CustomerStatus?
                }, 
                null,
                options.Conventions,
                onExecuting: c => _output.Write(c, null, "Command Executing"),
                onExecuted: (c, r) => _output.Write(c, r, "Command Executed")
            ).ToList();
        }
        catch (Exception ex)
        {
            exception = ex;
        }
        

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        // Assert
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        exception.ShouldBeNull();
        customers.ShouldNotBeNull();
        customers.ShouldNotBeEmpty();
    }
}