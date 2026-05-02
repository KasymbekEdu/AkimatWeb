using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Npgsql.BackendMessages;

namespace AkimatWeb.Infrastructure;

public class CloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService()
    {
        // Өзіңнің API Key мен Secret-ті "Go to API Keys" батырмасын басып ал
        var account = new Account(
            "dfxu50dnx",
            "899158135238231",
            "CFXxJMYvMlwj0kXOngCj-_SeEZQ"
        );
        _cloudinary = new Cloudinary(account);
    }

    public async Task<string> UploadFileAsync(IFormFile file, string folderName)
    {
        if (file == null || file.Length == 0) return null;

        using var stream = file.OpenReadStream();
        var uploadParams = new RawUploadParams() // Raw қолдансақ видео да, фото да кете береді
        {
            File = new FileDescription(file.FileName, stream),
            Folder = folderName
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
        return uploadResult.SecureUrl.ToString(); // Базаға осы URL-ді сақтаймыз
    }
}