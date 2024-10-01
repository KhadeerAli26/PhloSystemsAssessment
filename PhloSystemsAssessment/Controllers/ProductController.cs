using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PhloSystemsModel;
using System.Text.RegularExpressions;

namespace PhloSystemsAssessment.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProductController> _logger;
        private const string ProductSourceUrl = "https://run.mocky.io/v3/cc147902-4a5a-4b1a-bc00-2220bafb49fd";

        public ProductController(HttpClient httpClient, ILogger<ProductController> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        [HttpGet("filter")]
        public async Task<IActionResult> FilterProducts(
            [FromQuery] int? id,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] string size,
            [FromQuery] string highlight)
        {
            var products = await GetProductsFromExternalSource();

            // Filter by Id if provided
            if (id.HasValue)
            {
                products = products.Where(p => p.Id == id.Value).ToList();
            }

            // Filter by minPrice
            if (minPrice.HasValue)
            {
                products = products.Where(p => p.Price >= minPrice.Value).ToList();
            }

            // Filter by maxPrice
            if (maxPrice.HasValue)
            {
                products = products.Where(p => p.Price <= maxPrice.Value).ToList();
            }

            // Filter by size
            if (!string.IsNullOrEmpty(size))
            {
                products = products.Where(p => p.Sizes.Contains(size)).ToList();
            }

            // Highlight words in the description if provided
            if (!string.IsNullOrEmpty(highlight))
            {
                var wordsToHighlight = highlight.Split(',');
                products.ForEach(p => p.Description = HighlightWords(p.Description, wordsToHighlight));
            }

            // Create filter response
            var result = new
            {
                Products = products,
                Filter = new
                {
                    MinPrice = products.Any() ? products.Min(p => p.Price) : (decimal?)null,
                    MaxPrice = products.Any() ? products.Max(p => p.Price) : (decimal?)null,
                    Sizes = products.SelectMany(p => p.Sizes).Distinct().ToArray(),
                    CommonWords = GetMostCommonWords(products.Select(p => p.Description).ToList())
                }
            };

            _logger.LogInformation("Filtered {count} products", products.Count);
            return Ok(result);
        }

        private async Task<List<Product>> GetProductsFromExternalSource()
        {
            var response = await _httpClient.GetStringAsync(ProductSourceUrl);
            _logger.LogInformation("Received response from product source");
            return JsonConvert.DeserializeObject<List<Product>>(response);
        }

        private string HighlightWords(string description, string[] words)
        {
            foreach (var word in words)
            {
                description = Regex.Replace(description, $"\\b{word}\\b", $"<em>{word}</em>", RegexOptions.IgnoreCase);
            }
            return description;
        }

        private string[] GetMostCommonWords(List<string> descriptions)
        {
            // This is a simplified word counting logic; feel free to enhance it
            var allWords = descriptions.SelectMany(d => d.Split(' ', StringSplitOptions.RemoveEmptyEntries)).ToList();
            var wordFrequency = allWords
                .GroupBy(w => w.ToLower())
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .Where(w => w.Length > 2) // Filtering very short words
                .Except(new[] { "the", "and", "for", "with", "that" }) // Exclude common words
                .Take(10)
                .ToArray();

            return wordFrequency;
        }
    }

}
