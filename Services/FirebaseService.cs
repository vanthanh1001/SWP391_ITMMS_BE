using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace SWP391_ITMMS_Api.Services
{
    public class FirebaseService
    {
        private readonly StorageClient _storageClient;
        private readonly string _bucketName;

        public FirebaseService(IConfiguration configuration)
        {
            try
            {
                var credentialSection = configuration.GetSection("Firebase:CredentialJson");
                var credentialDict = new Dictionary<string, string>
                {
                    { "type", credentialSection["type"] },
                    { "project_id", credentialSection["project_id"] },
                    { "private_key_id", credentialSection["private_key_id"] },
                    { "private_key", credentialSection["private_key"] },
                    { "client_email", credentialSection["client_email"] },
                    { "client_id", credentialSection["client_id"] },
                    { "auth_uri", credentialSection["auth_uri"] },
                    { "token_uri", credentialSection["token_uri"] },
                    { "auth_provider_x509_cert_url", credentialSection["auth_provider_x509_cert_url"] },
                    { "client_x509_cert_url", credentialSection["client_x509_cert_url"] },
                    { "universe_domain", credentialSection["universe_domain"] }
                };

                var credentialJson = JsonSerializer.Serialize(credentialDict);
                var credential = GoogleCredential.FromJson(credentialJson)
                    .CreateScoped("https://www.googleapis.com/auth/cloud-platform");

                if (FirebaseApp.DefaultInstance == null)
                {
                    FirebaseApp.Create(new AppOptions
                    {
                        Credential = credential,
                        ProjectId = credentialSection["project_id"]
                    });
                }

                _storageClient = StorageClient.Create(credential);
                
                // Get bucket name without .appspot.com
                var bucketName = configuration["Firebase:StorageBucket"];
                if (string.IsNullOrEmpty(bucketName))
                {
                    bucketName = "thanh-60c07";
                }
                else if (bucketName.EndsWith(".appspot.com"))
                {
                    bucketName = bucketName.Replace(".appspot.com", "");
                }
                _bucketName = bucketName;

                // Try to get or create bucket
                try
                {
                    var bucket = _storageClient.GetBucket(_bucketName);
                }
                catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    // Bucket doesn't exist, create it
                    _storageClient.CreateBucket(credentialSection["project_id"], _bucketName);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Firebase initialization failed: {ex.Message}", ex);
            }
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            try
            {
                var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                var obj = await _storageClient.UploadObjectAsync(
                    _bucketName,
                    uniqueFileName,
                    file.ContentType,
                    memoryStream);

                // Return the Firebase Storage URL
                return $"https://firebasestorage.googleapis.com/v0/b/{_bucketName}.appspot.com/o/{Uri.EscapeDataString(uniqueFileName)}?alt=media";
            }
            catch (Exception ex)
            {
                throw new Exception($"File upload failed: {ex.Message}", ex);
            }
        }

        public async Task DeleteFileAsync(string fileName)
        {
            try
            {
                await _storageClient.DeleteObjectAsync(_bucketName, fileName);
            }
            catch (Exception ex)
            {
                throw new Exception($"File deletion failed: {ex.Message}", ex);
            }
        }
    }
} 