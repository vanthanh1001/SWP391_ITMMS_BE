using Microsoft.AspNetCore.Mvc;
using SWP391_ITMMS_Api.Services;
using System.Security.Cryptography;
using System.Text;

namespace SWP391_ITMMS_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly PublitioService _publitioService;
        private readonly IConfiguration _configuration;

        public TestController(PublitioService publitioService, IConfiguration configuration)
        {
            _publitioService = publitioService;
            _configuration = configuration;
        }

        [HttpGet("signature")]
        public IActionResult TestSignature()
        {
            try
            {
                var apiKey = _configuration["Publitio:ApiKey"];
                var apiSecret = _configuration["Publitio:ApiSecret"];
                
                // Use same timestamp as our service
                var reasonableTime = new DateTimeOffset(2025, 1, 16, 10, 0, 0, TimeSpan.Zero);
                var timestamp = reasonableTime.ToUnixTimeSeconds().ToString();
                var nonce = new Random().Next(10000000, 99999999).ToString();
                
                // Generate signature the same way
                var dataToHash = timestamp + nonce + apiSecret;
                string signature;
                
                using (var sha1 = SHA1.Create())
                {
                    var hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(dataToHash));
                    signature = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                }
                
                return Ok(new
                {
                    apiKey = apiKey,
                    timestamp = timestamp,
                    nonce = nonce,
                    signature = signature,
                    dataToHash = dataToHash,
                    message = "Signature generated successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpGet("bearer-auth")]
        public async Task<IActionResult> TestBearerAuth()
        {
            try
            {
                var bearerToken = _configuration["Publitio:BearerToken"];
                
                if (string.IsNullOrEmpty(bearerToken))
                {
                    return BadRequest("Bearer token not configured");
                }

                // Test with Bearer Token authentication
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

                var response = await httpClient.GetAsync("https://api.publit.io/v1/files/list");
                var content = await response.Content.ReadAsStringAsync();

                return Ok(new
                {
                    statusCode = (int)response.StatusCode,
                    status = response.StatusCode.ToString(),
                    content = content,
                    bearerToken = $"{bearerToken.Substring(0, 8)}...",
                    authMethod = "Bearer Token",
                    isSuccess = response.IsSuccessStatusCode
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpGet("files/list")]
        public async Task<IActionResult> TestListFiles()
        {
            try
            {
                var apiKey = _configuration["Publitio:ApiKey"];
                var apiSecret = _configuration["Publitio:ApiSecret"];
                
                // Generate auth parameters
                var reasonableTime = new DateTimeOffset(2025, 1, 16, 10, 0, 0, TimeSpan.Zero);
                var timestamp = reasonableTime.ToUnixTimeSeconds();
                var nonce = new Random().Next(10000000, 99999999).ToString();
                
                var dataToHash = timestamp + nonce + apiSecret;
                string signature;
                
                using (var sha1 = SHA1.Create())
                {
                    var hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(dataToHash));
                    signature = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                }

                // Test with simple GET request to list files
                var url = $"https://api.publit.io/v1/files/list?api_key={apiKey}&api_timestamp={timestamp}&api_nonce={nonce}&api_signature={signature}";
                
                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                return Ok(new
                {
                    statusCode = (int)response.StatusCode,
                    status = response.StatusCode.ToString(),
                    content = content,
                    url = url,
                    timestamp = timestamp,
                    nonce = nonce,
                    signature = signature
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }
} 