using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace SWP391_ITMMS_Api.Services
{
    public class PublitioService
    {
        private readonly HttpClient _httpClient;
        private readonly string _bearerToken;
        private readonly string _baseUrl = "https://api.publit.io/v1";

        public PublitioService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _bearerToken = configuration["Publitio:BearerToken"] ?? "";
            
            // Setup Bearer Token authentication
            if (!string.IsNullOrEmpty(_bearerToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _bearerToken);
            }
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            try
            {
                if (string.IsNullOrEmpty(_bearerToken))
                {
                    throw new Exception("Bearer token not configured");
                }

                Console.WriteLine($"Using Bearer Token authentication: {_bearerToken.Substring(0, 10)}...");

                // Create multipart form content
                using var content = new MultipartFormDataContent();
                
                // Add file content
                using var fileStream = file.OpenReadStream();
                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");
                content.Add(fileContent, "file", file.FileName);

                Console.WriteLine($"Uploading file: {file.FileName}, Size: {file.Length} bytes");

                // Make POST request to Publitio API
                var response = await _httpClient.PostAsync($"{_baseUrl}/files/create", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Publitio Response Status: {response.StatusCode}");
                Console.WriteLine($"Publitio Response: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = JsonDocument.Parse(responseContent);
                    if (jsonResponse.RootElement.TryGetProperty("url_preview", out var urlElement))
                    {
                        return urlElement.GetString() ?? "";
                    }
                    if (jsonResponse.RootElement.TryGetProperty("url_short", out var shortUrlElement))
                    {
                        return shortUrlElement.GetString() ?? "";
                    }
                    
                    // Fallback: return the full response if no specific URL found
                    return responseContent;
                }
                else
                {
                    throw new Exception($"Upload failed: {response.StatusCode} - {responseContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Upload error: {ex.Message}");
                throw new Exception($"Failed to upload file: {ex.Message}", ex);
            }
        }

        public async Task DeleteFileAsync(string fileId)
        {
            try
            {
                if (string.IsNullOrEmpty(_bearerToken))
                {
                    throw new Exception("Bearer token not configured");
                }

                var response = await _httpClient.DeleteAsync($"{_baseUrl}/files/delete/{fileId}");
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Delete failed: {response.StatusCode} - {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Delete error: {ex.Message}");
                throw new Exception($"Failed to delete file: {ex.Message}", ex);
            }
        }
    }
} 