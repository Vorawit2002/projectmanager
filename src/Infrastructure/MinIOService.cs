using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using Minio.Exceptions;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Domain;

namespace ProjectManagement.Infrastructure.Services;
public class MinIOService : IMinIOService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<MinIOService> _logger;
    public MinIOService(IConfiguration configuration, ILogger<MinIOService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }
    // File uploader task.
    public async Task<string> Upload(Stream filesteam,string bucketName, string filepath)
    {
        //---------------------
        var endpoint = Environment.GetEnvironmentVariable("MinIOUrlExternal") ?? _configuration["MinIO:MinIOUrlExternal"];
        var accessKey = Environment.GetEnvironmentVariable("MinIOAccessKey") ?? _configuration["MinIO:MinIOAccessKey"];
        var secretKey = Environment.GetEnvironmentVariable("MinIOSecretKey") ?? _configuration["MinIO:MinIOSecretKey"];
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            _logger.LogInformation("MinIOUrlExternal: " + endpoint);
            endpoint = Environment.GetEnvironmentVariable("MinIOUrlInternal") ?? _configuration["MinIO:MinIOUrlInternal"];
            _logger.LogInformation("MinIOUrlInternal: " + endpoint);
            _logger.LogInformation("MinIOAccessKey: " + accessKey);
            _logger.LogInformation("MinIOSecretKey: " + secretKey);
            _logger.LogInformation("bucketName: " + bucketName);
            _logger.LogInformation("filePath: " + filepath);
        }
        try
        {
            var minioClient = new MinioClient()
                                .WithEndpoint(endpoint)
                                .WithCredentials(accessKey, secretKey)
                                .WithSSL(false)
                                .Build();

        string contentType = MIMEAssistant.GetMIMEType(filepath);
        
            string objectName = filepath;

            // Make a bucket on the server, if not already present.
            var beArgs = new BucketExistsArgs()
                .WithBucket(bucketName);
            bool found = await minioClient.BucketExistsAsync(beArgs).ConfigureAwait(false);
            if (!found)
            {
                var mbArgs = new MakeBucketArgs()
                    .WithBucket(bucketName);
                await minioClient.MakeBucketAsync(mbArgs).ConfigureAwait(false);
            }
            // Upload a file to bucket.
            var putObjectArgs = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithStreamData(filesteam)
                .WithObjectSize(filesteam.Length)
                .WithContentType(contentType);
            await minioClient.PutObjectAsync(putObjectArgs).ConfigureAwait(false);
            //filesteam.Dispose();
            _logger.LogInformation("Successfully uploaded " + objectName);
            return filepath;
        }
        catch (MinioException e)
        {
            _logger.LogInformation("failed uploaded " + e.Message);
        }
        throw new ArgumentException("filePath cannot be null");
    }

    public async Task<byte[]> DownloadToByteArray(string bucketName, string filePath)
    {
        var endpoint = Environment.GetEnvironmentVariable("MinIOUrlExternal") ?? _configuration["MinIO:MinIOUrlExternal"];
        var accessKey = Environment.GetEnvironmentVariable("MinIOAccessKey") ?? _configuration["MinIO:MinIOAccessKey"];
        var secretKey = Environment.GetEnvironmentVariable("MinIOSecretKey") ?? _configuration["MinIO:MinIOSecretKey"];
        _logger.LogInformation("MinIOUrlExternal: " + endpoint);
        _logger.LogInformation("MinIOAccessKey: " + accessKey);
        _logger.LogInformation("MinIOSecretKey: " + secretKey);
        _logger.LogInformation("bucketName: " + bucketName);
        _logger.LogInformation("filePath: " + filePath);
        try
        {
            var minioClient = new MinioClient()
                                .WithEndpoint(endpoint)
                                .WithCredentials(accessKey, secretKey)
                                .WithSSL(false)
                                .Build();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                GetObjectArgs getObjectArgs = new GetObjectArgs()
                                  .WithBucket(bucketName)
                                  .WithObject(filePath)
                                  .WithFile(filePath) //maybe file name if not working.
                                  .WithCallbackStream(stream =>
                                   {
                                       stream.CopyTo(memoryStream);
                                   }); ;
                await minioClient.GetObjectAsync(getObjectArgs);
                byte[] objectBytes = memoryStream.ToArray();
                _logger.LogInformation("Successfully download " + filePath);
                return objectBytes;
            }
        }
        catch (MinioException e)
        {
            _logger.LogInformation("failed uploaded " + e.Message);
            return default!;
        }
    }

    public async Task<string> DownloadToUrl(string bucketName, string filePath)
    {
        var endpoint = Environment.GetEnvironmentVariable("MinIOUrlExternal") ?? _configuration["MinIO:MinIOUrlExternal"];
        var accessKey = Environment.GetEnvironmentVariable("MinIOAccessKey") ?? _configuration["MinIO:MinIOAccessKey"];
        var secretKey = Environment.GetEnvironmentVariable("MinIOSecretKey") ?? _configuration["MinIO:MinIOSecretKey"];
        _logger.LogInformation("MinIOUrlExternal: " + endpoint);
        _logger.LogInformation("MinIOAccessKey: " + accessKey);
        _logger.LogInformation("MinIOSecretKey: " + secretKey);
        _logger.LogInformation("bucketName: " + bucketName);
        _logger.LogInformation("filePath: " + filePath);
        try
        {
            var minioClient = new MinioClient()
                                .WithEndpoint(endpoint)
                                .WithCredentials(accessKey, secretKey)
                                .WithSSL(false)
                                .Build();

            int expire = 604800;
            PresignedGetObjectArgs presignedArgs = new PresignedGetObjectArgs()
                                         .WithBucket(bucketName)
                                         .WithObject(filePath)
                                         .WithExpiry(expire);

            string url = await minioClient.PresignedGetObjectAsync(presignedArgs);
            return url;
        }
        catch (MinioException e)
        {
            _logger.LogInformation("failed download " + e.Message);
            return default!;
        }
    }
}
