﻿using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;

namespace Business.Handlers;

public interface IFileHandler
{
    Task<string> UploadFileAsync(IFormFile file);
}

public class AzureFileHandler(string connString, string continerName) : IFileHandler
{
    private readonly BlobContainerClient _containerClient = new BlobContainerClient(connString, continerName);

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return null!;

        var fileExtension = Path.GetExtension(file.FileName);
        var fileName = $"n{Guid.NewGuid()}{fileExtension}";

        string contentType = !string.IsNullOrEmpty(file.ContentType)
            ? file.ContentType
            : "application/octet-stream";

        if ((contentType == "application/octet-stream" || string.IsNullOrEmpty(contentType))
            && fileExtension.Equals(".svg", StringComparison.OrdinalIgnoreCase))
            contentType = "image/svg+xml";

        BlobClient blobClient = _containerClient.GetBlobClient(fileName);
        var uploadOptions = new BlobUploadOptions { HttpHeaders = new BlobHttpHeaders { ContentType = contentType } };

        using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, uploadOptions);

        return blobClient.Uri.ToString();
    }
}
