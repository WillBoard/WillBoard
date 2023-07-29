using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using WillBoard.Core.Entities;
using WillBoard.Core.Results;

namespace WillBoard.Core.Interfaces.Services
{
    public interface IFileService
    {
        byte[] GetMD5(in IFormFile file);

        Task<Status<string>> AddFileAsync(Board board, Post post, IFormFile file);

        string GetMimeType(in IFormFile formFile);
        string GetMimeType(Stream stream, string fileName);
        string GetMimeType(ReadOnlySpan<byte> file, string fileName);
    }
}