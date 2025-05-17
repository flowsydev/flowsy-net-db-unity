using Flowsy.Core;
using Flowsy.Db.Unity.Conventions;
using Flowsy.Db.Unity.Test.Mock;
using Shouldly;
using Xunit.Abstractions;
using Xunit.Extensions.Ordering;

namespace Flowsy.Db.Unity.Test.Scenarios;

[Collection(Collections.ConventionSet), Order(3)]
public class S03MySqlConventionTest : IClassFixture<S03MySqlConventionTest.LocalModel>
{
    public class LocalModel
    {
        public DbConnectionOptions? ConnectionOptions { get; set; }
    }
    
    private readonly ServiceHost _serviceHost;
    private readonly GlobalModel _globalModel;
    private readonly LocalModel _localModel;
    private readonly ITestOutputHelper _output;

    public S03MySqlConventionTest(ServiceHost serviceHost, GlobalModel globalModel, LocalModel localModel, ITestOutputHelper output)
    {
        _serviceHost = serviceHost;
        _globalModel = globalModel;
        _localModel = localModel;
        _output = output;
    }

    [Fact, Order(3)]
    public void T03_Should_Configure_MySqlConnection()
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        // Arrange
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        _globalModel.Providers.TryGetValue(DbProviderFamily.MySql, out var provider);
        provider.ShouldNotBeNull();
        
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        // Act
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        var conventions = DbConventionSet.CreateBuilder(provider)
            .UseDefaultCaseStyle(CaseStyle.LowerSnakeCase)
            .ForRoutines()
            .UseProcedures(prefix: "pro_", suffix: "_pro")
            .ForParameters()
            .UsePrefix("p_")
            .ForEnums()
            .UseNames(CaseStyle.UpperSnakeCase)
            .Build();

        var options = new DbConnectionOptionsBuilder("Secondary")
            .WithProvider(provider)
            .WithConnectionString(_serviceHost.MySqlConnectionString)
            .WithConventions(conventions)
            .Build();
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        // Assert
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        options.ConnectionKey.ShouldBe("Secondary");
        options.Provider.Family.ShouldBe(DbProviderFamily.MySql);
        options.ConnectionString.ShouldBe(_serviceHost.MySqlConnectionString);
        
        options.Conventions.ShouldNotBeNull();
        
        options.Conventions.Provider.Family.ShouldBe(DbProviderFamily.MySql);
        
        options.Conventions.DefaultCaseStyle.ShouldBe(CaseStyle.LowerSnakeCase);
        
        options.Conventions.Routines.PreferredType.ShouldBe(DbRoutineType.StoredProcedure);
        options.Conventions.Routines.Procedures.Naming.Prefix.ShouldBe("pro_");
        options.Conventions.Routines.Procedures.Naming.Suffix.ShouldBe("_pro");
        
        options.Conventions.Parameters.Naming.Prefix.ShouldBe("p_");
        
        options.Conventions.Enums.ValueFormat.ShouldBe(DbEnumFormat.Name);
        options.Conventions.Enums.NameTranslator.ShouldNotBeNull();
        options.Conventions.Enums.NameTranslator.MemberNameCaseStyle.ShouldBe(CaseStyle.UpperSnakeCase);
        
        _localModel.ConnectionOptions = options;
    }
}