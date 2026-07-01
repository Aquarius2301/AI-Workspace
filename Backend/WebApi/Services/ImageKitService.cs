using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Infrastructure.Settings;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace WebApi.Services;

public sealed class ImageKitService
{
    private const long MaxSizeBeforeCompress = 10L * 1024 * 1024; // 10 MB
    private const int MaxDimension = 1920;
    private const string UploadApiUrl = "https://upload.imagekit.io/api/v1/files/upload";

    private readonly UploadSettings _settings;
    private readonly HttpClient _httpClient;

    public ImageKitService(IOptions<UploadSettings> settings, HttpClient httpClient)
    {
        _settings = settings.Value;
        _httpClient = httpClient;
    }

    /// <summary>
    /// Uploads an image to ImageKit. Automatically compresses if the image
    /// exceeds 10 MB. Returns the CDN URL of the uploaded image.
    /// </summary>
    /// <param name="imageStream">Stream containing the image data.</param>
    /// <param name="fileName">Original file name (used as a hint on ImageKit).</param>
    /// <param name="ct">Cancellation token.</param>
    public async Task<string> UploadImageAsync(
        Stream imageStream,
        string fileName,
        CancellationToken ct = default
    )
    {
        // 1. Read stream into a MemoryStream so we can inspect length & seek
        using var memoryStream = new MemoryStream();
        await imageStream.CopyToAsync(memoryStream, ct);
        var imageBytes = memoryStream.ToArray();

        // 2. Compress if > 10 MB
        if (imageBytes.Length > MaxSizeBeforeCompress)
        {
            imageBytes = CompressImage(imageBytes);
        }

        // 3. Base64 encode
        var base64 = Convert.ToBase64String(imageBytes);

        // 4. Call ImageKit API
        var formData = new[]
        {
            new KeyValuePair<string, string>("file", base64),
            new KeyValuePair<string, string>("fileName", fileName),
            new KeyValuePair<string, string>("useUniqueFileName", "true"),
            new KeyValuePair<string, string>("folder", "/avatars"),
        };

        using var content = new FormUrlEncodedContent(formData);

        // Basic auth: PrivateKey:
        var authValue = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_settings.PrivateKey}:"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Basic",
            authValue
        );

        var response = await _httpClient.PostAsync(UploadApiUrl, content, ct);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(ct);

        // 5. Parse response, extract "url"
        using var doc = JsonDocument.Parse(json);
        var url = doc.RootElement.GetProperty("url").GetString();

        return url ?? throw new InvalidOperationException("ImageKit did not return a URL.");
    }

    private static byte[] CompressImage(byte[] imageBytes)
    {
        using var image = Image.Load(imageBytes);

        // Resize if the longest edge exceeds MaxDimension
        if (image.Width > MaxDimension || image.Height > MaxDimension)
        {
            image.Mutate(x =>
            {
                if (image.Width >= image.Height)
                {
                    x.Resize(MaxDimension, 0); // auto‑height
                }
                else
                {
                    x.Resize(0, MaxDimension); // auto‑width
                }
            });
        }

        // Save as JPEG with quality 80 to reduce size
        using var output = new MemoryStream();
        var encoder = new JpegEncoder { Quality = 80 };
        image.Save(output, encoder);
        return output.ToArray();
    }
}
