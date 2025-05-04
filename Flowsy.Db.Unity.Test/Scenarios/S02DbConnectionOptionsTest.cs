using Flowsy.Db.Unity.Test.Extensions;
using Flowsy.Db.Unity.Test.Mock;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shouldly;
using Xunit.Abstractions;
using Xunit.Extensions.Ordering;

namespace Flowsy.Db.Unity.Test.Scenarios;

[Collection(Collections.DependencyInjection), Order(1)]
public class S02DbConnectionOptionsTest
{
    private readonly ServiceHost _serviceHost;
    private readonly ITestOutputHelper _output;

    public S02DbConnectionOptionsTest(ServiceHost serviceHost, ITestOutputHelper output)
    {
        _serviceHost = serviceHost;
        _output = output;
    }
    
    [Fact, Order(1)]
    public async Task T01_UnnamedOptions_Should_BeRegistered()
    {
        // Arrange
        await using var scope = _serviceHost.ServiceProvider.CreateAsyncScope();
        var snapshot = scope.ServiceProvider.GetService<IOptionsSnapshot<DbConnectionOptions>>();
        snapshot.ShouldNotBeNull();
        
        // Act
        var options = snapshot.Get(null);
        _output.WriteHeader(options.GetType().Name);
        _output.WriteLine("ConnectionKey: {0}", options.ConnectionKey);
        _output.WriteLine("Provider.Family: {0}", options.Provider.Family);
        _output.WriteLine("Provider.InvariantName: {0}", options.Provider.InvariantName);
        _output.WriteLine("Provider.Factory: {0}", options.Provider.Factory?.GetType().Name);
        _output.WriteLine("ConnectionFactoryType: {0}", options.ConnectionFactoryType.FullName);
        _output.WriteLine("AgentType: {0}", options.AgentType.FullName);
        _output.WriteLine("UnitOfWorkType: {0}", options.UnitOfWorkType.FullName);
        
        // Assert
        options.ShouldNotBeNull();
    }
    
    [Theory, Order(2)]
    [InlineData("Primary")]
    [InlineData("Secondary")]
    public async Task T02_NamedOptions_Should_BeRegistered(string connectionKey)
    {
        // Arrange
        await using var scope = _serviceHost.ServiceProvider.CreateAsyncScope();
        var snapshot = scope.ServiceProvider.GetService<IOptionsSnapshot<DbConnectionOptions>>();
        snapshot.ShouldNotBeNull();
        
        // Act
        var options = snapshot.Get(connectionKey);
        _output.WriteHeader(options.GetType().Name);
        _output.WriteLine("ConnectionKey: {0}", options.ConnectionKey);
        _output.WriteLine("Provider.Family: {0}", options.Provider.Family);
        _output.WriteLine("Provider.InvariantName: {0}", options.Provider.InvariantName);
        _output.WriteLine("Provider.Factory: {0}", options.Provider.Factory?.GetType().Name);
        _output.WriteLine("ConnectionFactoryType: {0}", options.ConnectionFactoryType.FullName);
        _output.WriteLine("AgentType: {0}", options.AgentType.FullName);
        _output.WriteLine("UnitOfWorkType: {0}", options.UnitOfWorkType.FullName);
        
        // Assert
        options.ShouldNotBeNull();
    }
}