using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AIWorkspace.Application.Common.Exceptions;
using AIWorkspace.Application.Common.Services;
using AIWorkspace.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace AIWorkspace.Infrastructure.Services;

public class ImageKitService : IImageKitService
{
    private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB
    private const string UploadUrl = "https://upload.imagekit.io/api/v1/files/upload";
    private const string ApiBaseUrl = "https://api.imagekit.io/v1";

    private readonly UploadSetting _uploadSetting;
    private readonly HttpClient _httpClient;

    public ImageKitService(IOptions<UploadSetting> uploadSetting, HttpClient httpClient)
    {
        _uploadSetting = uploadSetting.Value;
        _httpClient = httpClient;
    }

    public async Task<ImageKitUploadResult> UploadAsync(
        Stream fileStream,
        string fileName,
        CancellationToken cancellationToken = default
    )
    {
        if (fileStream.Length > MaxFileSize)
        {
            throw new BadRequestException(ErrorCodes.FileSizeTooLarge, new { MaxSizeMB = 5 });
        }

        using var formContent = new MultipartFormDataContent();

        // Read the stream into bytes
        using var memoryStream = new MemoryStream();
        await fileStream.CopyToAsync(memoryStream, cancellationToken);
        var fileBytes = memoryStream.ToArray();

        var fileContent = new ByteArrayContent(fileBytes);
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
        formContent.Add(fileContent, "file", fileName);
        formContent.Add(new StringContent("AIWorkspace"), "folder");
        formContent.Add(new StringContent(fileName), "fileName");
        formContent.Add(new StringContent(_uploadSetting.PublicKey), "publicKey");

        using var request = new HttpRequestMessage(HttpMethod.Post, UploadUrl)
        {
            Content = formContent,
        };
        request.Headers.Authorization = GetBasicAuthHeader();

        var response = await _httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InternalServerErrorException(
                ErrorCodes.ImageKitUploadFailed,
                new { StatusCode = (int)response.StatusCode, ErrorBody = errorBody }
            );
        }

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = JsonSerializer.Deserialize<ImageKitApiResponse>(
            responseBody,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        if (result is null || string.IsNullOrWhiteSpace(result.FileId))
        {
            throw new InternalServerErrorException(
                ErrorCodes.ImageKitUploadFailed,
                new { Message = "Invalid response from ImageKit" }
            );
        }

        return new ImageKitUploadResult(result.FileId, result.Url);
    }

    public async Task DeleteAsync(string fileId, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Delete,
            $"{ApiBaseUrl}/files/{fileId}"
        );
        request.Headers.Authorization = GetBasicAuthHeader();

        var response = await _httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InternalServerErrorException(
                ErrorCodes.ImageKitUploadFailed,
                new { StatusCode = (int)response.StatusCode, ErrorBody = errorBody }
            );
        }
    }

    private AuthenticationHeaderValue GetBasicAuthHeader()
    {
        var authValue = Convert.ToBase64String(
            Encoding.ASCII.GetBytes($"{_uploadSetting.PrivateKey}:")
        );
        return new AuthenticationHeaderValue("Basic", authValue);
    }

    private sealed class ImageKitApiResponse
    {
        public string FileId { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public long Size { get; set; }
        public string FilePath { get; set; } = string.Empty;
    }
}
