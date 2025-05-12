using Flowsy.Db.Unity.Test.Extensions;
using Flowsy.Db.Unity.Test.Mock;
using Flowsy.Db.Unity.Test.Mock.Model;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit.Abstractions;
using Xunit.Extensions.Ordering;

namespace Flowsy.Db.Unity.Test.Scenarios;

[Collection(Collections.DependencyInjection), Order(3)]
public class S06PrimaryAgentTest
{
    private readonly ServiceHost _serviceHost;
    private readonly GlobalModel _globalModel;
    private readonly ITestOutputHelper _output;

    public S06PrimaryAgentTest(ServiceHost serviceHost, GlobalModel globalModel, ITestOutputHelper output)
    {
        _serviceHost = serviceHost;
        _globalModel = globalModel;
        _output = output;
    }

    [Fact, Order(1)]
    public void T01_Should_GetResults_FromRoutine_Sync()
    {
        // Arrange
        using var scope = _serviceHost.ServiceProvider.CreateScope();
        var dbAgent = scope.ServiceProvider.GetRequiredService<IDbSecondaryAgent>();
        _output.Subscribe(dbAgent);

        // Act
        Customer[]? customers = null;
        Exception? exception = null;
        try
        {
            customers = dbAgent.GetFromRoutine<Customer>(
                "cst_get_by_filter",
                new {SearchTerm = "@example.com"}
            ).ToArray();
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        // Assert
        exception.ShouldBeNull();
        customers.ShouldNotBeNull();
        customers.ShouldNotBeEmpty();
    }
}