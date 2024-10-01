using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Threading;
using Xunit;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using PhloSystemsAssessment.Controllers;
using PhloSystemsModel;

public class ProductControllerTests
{
    private readonly ProductController _controller;
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly Mock<ILogger<ProductController>> _mockLogger;

    public ProductControllerTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        var httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        _mockLogger = new Mock<ILogger<ProductController>>();
        _controller = new ProductController(httpClient, _mockLogger.Object);
    }

    [Fact]
    public async Task FilterProducts_NoFilters_ReturnsAllProducts()
    {
        // Arrange
        var mockProductList = new List<Product>
    {
        new Product { Id = 1, Name = "Green T-shirt", Price = 15, Sizes = new List<string> { "medium" }, Description = "A nice green t-shirt" },
        new Product { Id = 2, Name = "Blue Jeans", Price = 25, Sizes = new List<string> { "large" }, Description = "Stylish blue jeans" },
        new Product { Id = 3, Name = "Red Hat", Price = 10, Sizes = new List<string> { "small" }, Description = "A vibrant red hat" }
    };

        var jsonContent = JsonConvert.SerializeObject(mockProductList);
        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json")
        };

        // Set up the mock HTTP handler to return the mocked response
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act
        var result = await _controller.FilterProducts(null, null, null, null,null) as OkObjectResult; // No filters passed
        var resultValue = result?.Value as FilterResponse; // Cast to FilterResponse

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(resultValue);  // Ensure resultValue is not null
        Assert.Equal(3, resultValue.Products.Count); // Should return all products
    }

    [Fact]
    public async Task FilterProducts_WithFilters_ReturnsFilteredProducts()
    {
        // Arrange
        var mockProductList = new List<Product>
    {
        new Product { Id = 1, Name = "Green T-shirt", Price = 15, Sizes = new List<string> { "medium" }, Description = "A nice green t-shirt" },
        new Product { Id = 2, Name = "Blue Jeans", Price = 25, Sizes = new List<string> { "large" }, Description = "Stylish blue jeans" },
        new Product { Id = 3, Name = "Red Hat", Price = 10, Sizes = new List<string> { "small" }, Description = "A vibrant red hat" }
    };

        var jsonContent = JsonConvert.SerializeObject(mockProductList);
        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json")
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act
        var result = await _controller.FilterProducts(1, 10, 20, null, "green,blue") as OkObjectResult;
        var resultValue = result?.Value as FilterResponse; // Cast to FilterResponse

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(resultValue);  // Ensure resultValue is not null
        Assert.Equal(2, resultValue.Products.Count); // Should return filtered products
        foreach (var product in resultValue.Products)
        {
            Assert.True(
                product.Description.Contains("green", StringComparison.OrdinalIgnoreCase) ||
                product.Description.Contains("blue", StringComparison.OrdinalIgnoreCase),
                "Product description does not contain expected highlighted words.");
        }// Check if highlighted words are in the descriptions
    }


    

    [Fact]
    public async Task FilterProducts_WithInvalidFilters_ReturnsEmptyProducts()
    {
        // Arrange
        var mockProductList = new List<Product>
            {
                new Product { Id = 1, Name = "Green T-shirt", Price = 15, Sizes = new List<string> { "medium" }, Description = "A nice green t-shirt" },
                new Product { Id = 2, Name = "Blue Jeans", Price = 25, Sizes = new List<string> { "large" }, Description = "Stylish blue jeans" },
                new Product { Id = 3, Name = "Red Hat", Price = 10, Sizes = new List<string> { "small" }, Description = "A vibrant red hat" }
            };

        var jsonContent = JsonConvert.SerializeObject(mockProductList);
        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json")
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act
        var result = await _controller.FilterProducts(1,100, 200, null, "green") as OkObjectResult; // No products in this range
        var resultValue = result?.Value as FilterResponse; // Cast to FilterResponse

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(resultValue);  // Ensure resultValue is not null
        Assert.Empty(resultValue.Products); // Should return an empty product list
    }
}
