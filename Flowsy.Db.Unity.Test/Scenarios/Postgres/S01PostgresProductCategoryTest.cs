using Microsoft.Extensions.DependencyInjection;
using Flowsy.Db.Unity.Test.Mock;
using Flowsy.Db.Unity.Test.Mock.Infrastructure.Database;
using Flowsy.Db.Unity.Test.Mock.Infrastructure.Time;
using Flowsy.Db.Unity.Test.Mock.Model;
using Flowsy.Db.Unity.Test.Infrastructure.Testing.Ordering;
using Shouldly;

namespace Flowsy.Db.Unity.Test.Scenarios.Postgres;

// Copilot:
// - This test class is designed to validate CRUD operations on product categories only (not other entities).

/// <summary>
/// Given a PostgreSQL database for an eCommerce application
/// When the application connects to the database and performs CRUD operations on product categories
/// Then the operations should succeed and the data should be correctly stored and retrieved
/// </summary>
[Collection(Collections.PostgresProductCategories), Order(1)]
public class C01S01PostgresProductCategoryTest
{
    private const string ConnectionKey = DbConnections.Postgres;
    
    private readonly ServiceHost _host;
    private readonly ITestOutputHelper _output;
    
    public C01S01PostgresProductCategoryTest(ServiceHost host, ITestOutputHelper output)
    {
        _host = host;
        _output = output;
    }

    [Theory, Order(1)]
    [InlineData("electronics", "Electronics", "Devices and gadgets")]
    [InlineData("fashion", "Fashion", "Clothing and accessories")]
    [InlineData("home_kitchen", "Home & Kitchen", "Household items and kitchenware")]
    [InlineData("sports", "Sports", "Sporting goods and outdoor equipment")]
    [InlineData("miscellaneous", "Miscellaneous", "Miscellaneous items")]
    [InlineData("undesired_one", "Undesired One", "This will be deleted in the last test")] // This will be deleted in the last test
    [InlineData("undesired_two", "Undesired Two", "This will be deleted in the last test")] // This will be deleted in the last test
    [InlineData("undesired_three", "Undesired Three", "This will be deleted in the last test")] // This will be deleted in the last test
    public async Task T01_Should_Create_Product_Category(string code, string name, string description)
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var scope = _host.CreateAsyncScope();
        
        var connectionHub = scope.ServiceProvider.GetService<IDbConnectionHub>();
        connectionHub.ShouldNotBeNull();
        
        await using var db = await connectionHub.CreateSessionAsync(ConnectionKey, cancellationToken: cancellationToken);
        
        // Act
        Exception? exception = null;
        try
        {
            _output.WriteLine("Creating product category: {0} | {1} | {2}", code, name, description);
            
            await db.ExecuteRoutineAsync(
                "shopping.product_category_create",
                new
                {
                    ProductCategoryId = Guid.NewGuid(),
                    Code = code, 
                    Name = name,
                    Description = description,
                    CreationInstant = Clock.GetTimestamp()
                },
                cancellationToken
                );
            
            _output.WriteLine("Product category created successfully: {0} | {1} | {2}", code, name, description);
        }
        catch (Exception ex)
        {
            exception = ex;
            _output.WriteLine(ex.ToString());
        }

        // Assert
        exception.ShouldBeNull();
    }
    
    [Theory, Order(2)]
    [InlineData("electronics", "Electronics")]
    [InlineData("fashion", "Fashion")]
    [InlineData("home_kitchen", "Home & Kitchen")]
    [InlineData("sports", "Sports")]
    public async Task T02_Should_Read_Product_Category(string code, string expectedName)
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var scope = _host.CreateAsyncScope();
        
        var connectionHub = scope.ServiceProvider.GetService<IDbConnectionHub>();
        connectionHub.ShouldNotBeNull();
        
        await using var db = await connectionHub.CreateSessionAsync(ConnectionKey, cancellationToken: cancellationToken);
        
        // Act
        _output.WriteLine("Retrieving product category: {0}", code);
            
        var existingCategory = await db.QuerySingleFromRoutineAsync<ProductCategoryDetail>(
            "shopping.product_category_get_detail_by_key",
            new { Key = code },
            cancellationToken
        );
            
        _output.WriteLine("Product category retrieved successfully: {0}{1}", Environment.NewLine, existingCategory);

        // Assert
        existingCategory.ShouldNotBeNull();
        existingCategory.Code.ShouldBe(code);
        existingCategory.Name.ShouldBe(expectedName);
        existingCategory.Description.ShouldNotBeNull();
        existingCategory.CreationInstant.ShouldNotBe(DateTimeOffset.MinValue);
        existingCategory.LastMutationInstant.ShouldBeNull();
    }
    
    [Theory, Order(3)]
    [InlineData("electronics", "Consumer Electronics", "Electronic devices, gadgets and accessories")]
    [InlineData("fashion", "Fashion & Style", "Trendy clothing, shoes and fashion accessories")]
    [InlineData("home_kitchen", "Home & Kitchen Essentials", "Essential items for home and kitchen use")]
    [InlineData("sports", "Sports & Recreation", "Equipment for sports, fitness and outdoor activities")]
    public async Task T03_Should_Update_Product_Category(string code, string newName, string newDescription)
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var scope = _host.CreateAsyncScope();
        
        var connectionHub = scope.ServiceProvider.GetService<IDbConnectionHub>();
        connectionHub.ShouldNotBeNull();
        
        await using var db = await connectionHub.CreateSessionAsync(ConnectionKey, cancellationToken: cancellationToken);
        
        // Act
        _output.WriteLine("Retrieving product category by code: {0}", code);
        var existingCategory = await db.QuerySingleFromRoutineAsync<ProductCategoryOverview>(
            "shopping.product_category_get_overview_by_key",
            new { Key = code },
            cancellationToken
        );
        existingCategory.ShouldNotBeNull();
        
        _output.WriteLine("Updating product category: {0} | {1} | {2}", code, newName, newDescription);
            
        await db.ExecuteRoutineAsync(
            "shopping.product_category_update",
            new
            {
                existingCategory.ProductCategoryId,
                Code = code,
                Name = newName,
                Description = newDescription,
                LastMutationInstant = Clock.GetTimestamp()
            },
            cancellationToken
        );
            
        _output.WriteLine("Product category updated successfully: {0}", code);
            
        var updatedCategory = await db.QuerySingleFromRoutineAsync<ProductCategoryDetail>(
            "shopping.product_category_get_detail_by_key",
            new
            {
                Key = existingCategory.ProductCategoryId.ToString(),
            },
            cancellationToken
        );
        
        _output.WriteLine("Updated category verified: {0}{1}", Environment.NewLine, updatedCategory);

        // Assert
        updatedCategory.ShouldNotBeNull();
        
        updatedCategory.Code.ShouldBe(code);
        updatedCategory.Name.ShouldBe(newName);
        updatedCategory.Description.ShouldBe(newDescription);
        updatedCategory.CreationInstant.ShouldNotBe(DateTimeOffset.MinValue);
        updatedCategory.LastMutationInstant.ShouldNotBeNull();
        updatedCategory.LastMutationInstant.Value.ShouldBeGreaterThan(updatedCategory.CreationInstant);
    }
    
    [Theory, Order(4)]
    [InlineData("undesired_one")]
    [InlineData("undesired_two")]
    [InlineData("undesired_three")]
    public async Task T04_Should_Delete_Product_Category(string code)
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var scope = _host.CreateAsyncScope();
        
        var connectionHub = scope.ServiceProvider.GetService<IDbConnectionHub>();
        connectionHub.ShouldNotBeNull();
        
        await using var db = await connectionHub.CreateSessionAsync(ConnectionKey, cancellationToken: cancellationToken);
        
        // Act
        _output.WriteLine("Retrieving product category by code before deletion: {0}", code);
        var existingCategory = await db.QuerySingleFromRoutineAsync<ProductCategoryOverview>(
            "shopping.product_category_get_overview_by_key",
            new { Key = code },
            cancellationToken
        );
        existingCategory.ShouldNotBeNull();
        
        _output.WriteLine("Deleting product category: {0}", code);
            
        await db.ExecuteRoutineAsync(
            "shopping.product_category_delete_by_id",
            new
            {
                existingCategory.ProductCategoryId
            },
            cancellationToken
        );
            
        _output.WriteLine("Product category deleted successfully: {0}", code);
            
        // Verify deletion by attempting to retrieve the category
        var deletedCategory = await db.QuerySingleOrDefaultFromRoutineAsync<ProductCategoryDetail>(
            "shopping.product_category_get_detail_by_key",
            new { Key = code },
            cancellationToken
        );

        // Assert
        deletedCategory.ShouldBeNull();
        _output.WriteLine("Confirmed that product category {0} no longer exists", code);
    }
}
