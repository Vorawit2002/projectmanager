using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Application.Common.Models;
//using SkiaSharp;

namespace ProjectManagement.Application.Common.Interfaces;
public interface IMinIOService
{
    public Task<string> Upload(Stream filesteam, string bucketName, string filename);
    //public Task<string> UploadThumbnail(Stream filesteam, string bucketName, SKBitmap original, string filename);
    public Task<byte[]> DownloadToByteArray(string bucketName, string filePath);
    public Task<string> DownloadToUrl(string bucketName, string filePath);
}
