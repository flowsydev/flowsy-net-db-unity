using Flowsy.Db.Unity.Test.Mock;
using Flowsy.Db.Unity.Test.Mock.Infrastructure.Database;
using Flowsy.Db.Unity.Test.Mock.Infrastructure.Time;
using Flowsy.Db.Unity.Test.Mock.Model;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit.Abstractions;
using Xunit.Extensions.Ordering;

namespace Flowsy.Db.Unity.Test.Scenarios.Postgres;

// Copilot:
// - This test class is designed to validate CRUD operations on products only (not other entities).
// - Instead of using `IDbSession.Query*FromRoutineAsync`, it uses `IDbSession.Query*Async` with raw SQL queries to interact with the database.
// - Instead of using `IDbSession.ExecuteRoutineAsync`, it uses `IDbSession.ExecuteAsync` with raw SQL commands to perform database operations.

/// <summary>
/// Given a PostgreSQL database for an eCommerce application
/// When the application connects to the database and performs CRUD operations on products
/// Then the operations should succeed and the data should be correctly stored and retrieved
/// </summary>
[Collection(Collections.Postgres), Order(2)]
public class S02PostgresProductTest
{
    private const string ConnectionKey = DbConnections.Postgres;
    
    private readonly ServiceHost _host;
    private readonly ITestOutputHelper _output;

    public S02PostgresProductTest(ServiceHost host, ITestOutputHelper output)
    {
        _host = host;
        _output = output;
    }
    
    [Theory, Order(1)]
    [InlineData("i_phone_15_pro", "iPhone 15 Pro", "Latest Apple smartphone with titanium design", 999.99, Currency.Usd, "electronics")]
    [InlineData("cotton_t_shirt", "Cotton T-Shirt", "Comfortable cotton t-shirt in various colors", 29.99, Currency.Usd, "fashion")]
    [InlineData("kitchen_blender_pro", "Kitchen Blender Pro", "High-performance blender for smoothies and soups", 149.99, Currency.Usd, "home_kitchen")]
    [InlineData("professional_soccer_ball", "Professional Soccer Ball", "FIFA approved soccer ball for professional matches", 79.99, Currency.Usd, "sports")]
    [InlineData("undesired_product_one", "Undesired Product One", "This product will be deleted later", 10.00, Currency.Usd, "miscellaneous")]
    [InlineData("undesired_product_two", "Undesired Product Two", "This product will also be deleted later", 20.00, Currency.Usd, "miscellaneous")]
    [InlineData("undesired_product_three", "Undesired Product Three", "This product will be deleted as well", 30.00, Currency.Usd, "miscellaneous")]
    public async Task T01_Should_Create_Product(string sku, string name, string description, decimal price, Currency currency, string categoryCode)
    {
        // Arrange
        await using var scope = _host.CreateAsyncScope();
        
        var connectionHub = scope.ServiceProvider.GetService<IDbConnectionHub>();
        connectionHub.ShouldNotBeNull();
        
        await using var db = await connectionHub.CreateSessionAsync(ConnectionKey);
        
        // Act
        // Get the product category ID by code
        _output.WriteLine("Retrieving product category by code: {0}", categoryCode);
        var productCategory = await db.QuerySingleFromRoutineAsync<ProductCategoryOverview>(
            "shopping.product_category_get_overview_by_key",
            new { Key = categoryCode }
        );
        productCategory.ShouldNotBeNull();
        
        Exception? exception = null;
        try
        {
            // Generate sample tag IDs based on product category
            var tagIds = GenerateTagIdsForCategory(categoryCode);
            
            _output.WriteLine("Creating product: {0} | {1} | {2} | {3} {4} | Tags: [{5}]", 
                sku, name, description, price, currency, string.Join(", ", tagIds));
            
            await db.ExecuteAsync(
                """
                insert into shopping.product (product_id, sku, name, description, price, currency, product_category_id, tag_ids, creation_instant) 
                values (@p_product_id, @p_sku, @p_name, @p_description, @p_price, @p_currency::shopping.currency, @p_product_category_id, @p_tag_ids, @p_creation_instant)
                """,
                new
                {
                    ProductId = Guid.NewGuid(),
                    Sku = sku,
                    Name = name,
                    Description = description,
                    Price = price,
                    Currency = currency,
                    productCategory.ProductCategoryId,
                    TagIds = tagIds,
                    CreationInstant = Clock.GetTimestamp()
                }
            );
            
            _output.WriteLine("Product created successfully: {0} | {1} | {2} | {3} {4} | Tags: [{5}]", 
                sku, name, description, price, currency, string.Join(", ", tagIds));
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
    [InlineData("i_phone_15_pro", "iPhone 15 Pro")]
    [InlineData("cotton_t_shirt", "Cotton T-Shirt")]
    [InlineData("kitchen_blender_pro", "Kitchen Blender Pro")]
    [InlineData("professional_soccer_ball", "Professional Soccer Ball")]
    public async Task T02_Should_Read_Product(string sku, string expectedName)
    {
        // Arrange
        await using var scope = _host.CreateAsyncScope();
        
        var connectionHub = scope.ServiceProvider.GetService<IDbConnectionHub>();
        connectionHub.ShouldNotBeNull();
        
        await using var db = await connectionHub.CreateSessionAsync(ConnectionKey);
        
        // Act
        _output.WriteLine("Retrieving product: {0}", sku);
            
        var existingProduct = await db.QuerySingleAsync<ProductDetail>(
            """
            select
                p.product_id,
                p.sku,
                p.name,
                p.description,
                p.price,
                p.currency::text,
                p.product_category_id,
                p.tag_ids,
                p.creation_instant,
                p.last_mutation_instant
            from shopping.product as p
            where p.sku = @p_sku
            """,
            new { Sku = sku }
        );
            
        _output.WriteLine("Product retrieved successfully: {0}{1}", Environment.NewLine, existingProduct);

        // Assert
        existingProduct.ShouldNotBeNull();
        existingProduct.Sku.ShouldBe(sku);
        existingProduct.Name.ShouldBe(expectedName);
        existingProduct.Description.ShouldNotBeNull();
        existingProduct.Price.ShouldBeGreaterThan(0);
        existingProduct.Currency.ShouldBe(Currency.Usd);
        existingProduct.ProductCategoryId.ShouldNotBe(Guid.Empty);
        existingProduct.TagIds.ShouldNotBeNull();
        existingProduct.TagIds.ShouldNotBeEmpty();
        existingProduct.CreationInstant.ShouldNotBe(DateTimeOffset.MinValue);
        existingProduct.LastMutationInstant.ShouldBeNull();
        
        _output.WriteLine("Product has {0} tags: [{1}]", 
            existingProduct.TagIds.Length, string.Join(", ", existingProduct.TagIds));
    }
    
    [Theory, Order(3)]
    [InlineData("i_phone_15_pro", "iPhone 15 Pro Max", "Latest Apple smartphone with titanium design and larger display", 1199.99)]
    [InlineData("cotton_t_shirt", "Premium Cotton T-Shirt", "High-quality cotton t-shirt in various colors and sizes", 39.99)]
    [InlineData("kitchen_blender_pro", "Kitchen Blender Pro Max", "Commercial-grade blender for smoothies, soups and more", 199.99)]
    [InlineData("professional_soccer_ball", "FIFA Professional Soccer Ball", "Official FIFA approved soccer ball for professional matches", 89.99)]
    public async Task T03_Should_Update_Product(string sku, string newName, string newDescription, decimal newPrice)
    {
        // Arrange
        await using var scope = _host.CreateAsyncScope();
        
        var connectionHub = scope.ServiceProvider.GetService<IDbConnectionHub>();
        connectionHub.ShouldNotBeNull();
        
        await using var db = await connectionHub.CreateSessionAsync(ConnectionKey);
        
        // Act
        _output.WriteLine("Retrieving product by SKU: {0}", sku);
        var existingProduct = await db.QuerySingleAsync<ProductOverview>(
            """
            select
                p.product_id,
                p.sku,
                p.name,
                p.price,
                p.currency::text,
                p.product_category_id,
                p.tag_ids
            from shopping.product as p
            where p.sku = @p_sku
            """,
            new { Sku = sku }
        );
        existingProduct.ShouldNotBeNull();
        
        // Generate updated tag IDs (add new tags to existing ones)
        var updatedTagIds = existingProduct.TagIds?.ToList() ?? new List<int>();
        updatedTagIds.AddRange([999, 1000]); // Add some new tags
        
        _output.WriteLine("Updating product: {0} | {1} | {2} | {3} {4} | Tags: [{5}]", 
            sku, newName, newDescription, newPrice, existingProduct.Currency, string.Join(", ", updatedTagIds));
            
        await db.ExecuteAsync(
            """
            update shopping.product 
            set 
                name = @p_name,
                description = @p_description,
                price = @p_price,
                tag_ids = @p_tag_ids,
                last_mutation_instant = @p_last_mutation_instant
            where product_id = @p_product_id
            """,
            new
            {
                existingProduct.ProductId,
                Name = newName,
                Description = newDescription,
                Price = newPrice,
                TagIds = updatedTagIds.ToArray(),
                LastMutationInstant = Clock.GetTimestamp()
            }
        );
            
        _output.WriteLine("Product updated successfully: {0}", sku);
            
        var updatedProduct = await db.QuerySingleAsync<ProductDetail>(
            """
            select
                p.product_id,
                p.sku,
                p.name,
                p.description,
                p.price,
                p.currency::text,
                p.product_category_id,
                p.tag_ids,
                p.creation_instant,
                p.last_mutation_instant
            from shopping.product as p
            where p.product_id = @p_product_id
            """,
            new { existingProduct.ProductId }
        );
        
        _output.WriteLine("Updated product verified: {0}{1}", Environment.NewLine, updatedProduct);

        // Assert
        updatedProduct.ShouldNotBeNull();
        
        updatedProduct.Sku.ShouldBe(sku);
        updatedProduct.Name.ShouldBe(newName);
        updatedProduct.Description.ShouldBe(newDescription);
        updatedProduct.Price.ShouldBe(newPrice);
        updatedProduct.Currency.ShouldBe(Currency.Usd);
        updatedProduct.ProductCategoryId.ShouldNotBe(Guid.Empty);
        updatedProduct.TagIds.ShouldNotBeNull();
        updatedProduct.TagIds.ShouldContain(999);
        updatedProduct.TagIds.ShouldContain(1000);
        updatedProduct.CreationInstant.ShouldNotBe(DateTimeOffset.MinValue);
        updatedProduct.LastMutationInstant.ShouldNotBeNull();
        updatedProduct.LastMutationInstant.Value.ShouldBeGreaterThan(updatedProduct.CreationInstant);
        
        _output.WriteLine("Updated product has {0} tags: [{1}]", 
            updatedProduct.TagIds.Length, string.Join(", ", updatedProduct.TagIds));
    }
    
    [Theory, Order(4)]
    [InlineData("undesired_product_one")]
    [InlineData("undesired_product_two")]
    [InlineData("undesired_product_three")]
    public async Task T04_Should_Delete_Product(string sku)
    {
        // Arrange
        await using var scope = _host.CreateAsyncScope();
        
        var connectionHub = scope.ServiceProvider.GetService<IDbConnectionHub>();
        connectionHub.ShouldNotBeNull();
        
        await using var db = await connectionHub.CreateSessionAsync(ConnectionKey);
        
        // Act
        _output.WriteLine("Retrieving product by SKU before deletion: {0}", sku);
        var existingProduct = await db.QuerySingleAsync<ProductOverview>(
            """
            select
                p.product_id,
                p.sku,
                p.name,
                p.price,
                p.currency::text,
                p.product_category_id,
                p.tag_ids
            from shopping.product as p
            where p.sku = @p_sku
            """,
            new { Sku = sku }
        );
        existingProduct.ShouldNotBeNull();
        
        _output.WriteLine("Deleting product: {0}", sku);
            
        await db.ExecuteAsync(
            """
            delete from shopping.product 
            where product_id = @p_product_id
            """,
            new { existingProduct.ProductId }
        );
            
        _output.WriteLine("Product deleted successfully: {0}", sku);
            
        // Verify deletion by attempting to retrieve the product
        var deletedProduct = await db.QuerySingleOrDefaultAsync<ProductDetail>(
            """
            select
                p.product_id,
                p.sku,
                p.name,
                p.description,
                p.price,
                p.currency::text,
                p.product_category_id,
                p.tag_ids,
                p.creation_instant,
                p.last_mutation_instant
            from shopping.product as p
            where p.sku = @p_sku
            """,
            new { Sku = sku }
        );

        // Assert
        deletedProduct.ShouldBeNull();
        _output.WriteLine("Confirmed that product {0} no longer exists", sku);
    }
    
    [Fact, Order(5)]
    public async Task T05_Should_Search_Products_By_Tag_Ids()
    {
        // Arrange
        await using var scope = _host.CreateAsyncScope();
        
        var connectionHub = scope.ServiceProvider.GetService<IDbConnectionHub>();
        connectionHub.ShouldNotBeNull();
        
        await using var db = await connectionHub.CreateSessionAsync(ConnectionKey);
        
        // Act
        // Search for products with electronics-related tags (1, 2, 3)
        var searchTagIds = new[] { 1, 2, 3 };
        _output.WriteLine("Searching products with tags: [{0}]", string.Join(", ", searchTagIds));
        
        var products = await db.QueryAsync<ProductOverview>(
            """
            select
                p.product_id,
                p.sku,
                p.name,
                p.price,
                p.currency::text,
                p.product_category_id,
                p.tag_ids
            from shopping.product as p
            where p.tag_ids && @p_tag_ids
            order by p.name
            """,
            new { TagIds = searchTagIds }
        );
        
        var productList = products.ToList();
        _output.WriteLine("Found {0} products with specified tags", productList.Count);
        
        foreach (var product in productList)
        {
            _output.WriteLine("  - {0} | {1} | Tags: [{2}]", 
                product.Sku, product.Name, string.Join(", ", product.TagIds ?? Array.Empty<int>()));
        }
        
        // Assert
        productList.ShouldNotBeEmpty();
        productList.ShouldAllBe(p => p.TagIds != null && p.TagIds.Any(t => searchTagIds.Contains(t)));
    }
    
    /// <summary>
    /// Generates sample tag IDs based on product category code.
    /// </summary>
    private static int[] GenerateTagIdsForCategory(string categoryCode)
    {
        return categoryCode switch
        {
            "electronics" => [1, 2, 3, 100, 101],      // Technology, Mobile, Innovation, Premium, Latest
            "fashion" => [4, 5, 6, 102, 103],          // Clothing, Style, Comfort, Cotton, Casual
            "home_kitchen" => [7, 8, 9, 104, 105],     // Kitchen, Appliance, Home, Professional, High-Performance
            "sports" => [10, 11, 12, 106, 107],        // Sports, Fitness, Outdoor, Professional, FIFA
            "miscellaneous" => [13, 14, 15],           // Other, General, Misc
            _ => [99]                                   // Unknown
        };
    }
}