using Flowsy.Db.Unity.Test.Extensions;
using Npgsql;
using Shouldly;
using Xunit.Abstractions;
using Xunit.Extensions.Ordering;

namespace Flowsy.Db.Unity.Test.Scenarios;

[Collection(Collections.ConventionSet), Order(1)]
public class S01DbProviderTest : IClassFixture<S01DbProviderTest.Model>
{
    public class Model
    {
        public DbProvider? Provider { get; set; }
    }

    private readonly Model _model;
    private readonly ITestOutputHelper _output;

    public S01DbProviderTest(Model model, ITestOutputHelper output)
    {
        _model = model;
        _output = output;
    }

    [Fact, Order(1)]
    public void T01_Should_Create_Provider()
    {
        // Arrange
        DbProvider? provider = null;
        Exception? exception = null;
        
        // Act
        try
        {
            provider = new DbProvider(DbProviderFamily.Postgres, "Npgsql", NpgsqlFactory.Instance);
            
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
        _model.Provider = provider;
    }
    
    [Theory, Order(2)]
    [InlineData("crm.customer")]
    [InlineData("inventory.product")]
    [InlineData("car_rental.globalization.country")]
    public void T02_Should_Parse_ObjectName(string objectName)
    {
        // Arrange
        var provider = _model.Provider;
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
}