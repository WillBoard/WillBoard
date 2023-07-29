using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using WillBoard.Core.Interfaces.Services;

namespace WillBoard.Infrastructure.Services.Instance
{
    public class InstanceStorageService : IStorageService
    {
        private readonly ILogger _logger;
        private readonly IHostEnvironment _hostEnvironment;

        public InstanceStorageService(ILogger<InstanceStorageService> logger, IHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _hostEnvironment = hostEnvironment;
        }

        public void CreateDirectory(string directoryPath)
        {
            if (Directory.Exists(directoryPath))
            {
                _logger.LogWarning("Can not create directory \"{DirectoryPath}\" because directory already exist.", directoryPath);
                return;
            }

            Directory.CreateDirectory(directoryPath);
        }

        public void DeleteDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                _logger.LogWarning("Can not delete directory \"{DirectoryPath}\" because directory does not exist.", directoryPath);
                return;
            }

            Directory.Delete(directoryPath, true);
        }

        public void CreateBoardDirectory(string boardId)
        {
            string previewPath = _hostEnvironment.ContentRootPath + "/wwwroot/boards/" + boardId + "/preview";
            string sourcePath = _hostEnvironment.ContentRootPath + "/wwwroot/boards/" + boardId + "/source";

            CreateDirectory(previewPath);
            CreateDirectory(sourcePath);
        }

        public void DeleteBoardDirectory(string boardId)
        {
            string boardPath = _hostEnvironment.ContentRootPath + "/wwwroot/boards/" + boardId;

            DeleteDirectory(boardPath);
        }

        public void DeleteFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                _logger.LogWarning("Can not delete file \"{FilePath}\" because file does not exist.", filePath);
                return;
            }

            File.Delete(filePath);
        }

        public string GetPreviewFilePath(string boardId, string fileName)
        {
            return _hostEnvironment.ContentRootPath + "/wwwroot/boards/" + boardId + "/preview/" + fileName;
        }

        public string GetSourceFilePath(string boardId, string fileName)
        {
            return _hostEnvironment.ContentRootPath + "/wwwroot/boards/" + boardId + "/source/" + fileName;
        }

        public void DeletePreviewFile(string boardId, string fileName)
        {
            string path = GetPreviewFilePath(boardId, fileName);
            DeleteFile(path);
        }

        public void DeleteSourceFile(string boardId, string fileName)
        {
            string path = GetSourceFilePath(boardId, fileName);
            DeleteFile(path);
        }

        public void CopyFile(string filePath, string destinationFilePath)
        {
            if (!File.Exists(filePath))
            {
                _logger.LogWarning("Can not copy file from \"{FilePath}\" to \"{DestinationFilePath}\" because file does not exist.", filePath, destinationFilePath);
                return;
            }

            File.Copy(filePath, destinationFilePath);
        }

        public void CopyPreviewFile(string boardId, string fileName, string destinationBoardId, string destinationFileName)
        {
            string path = GetPreviewFilePath(boardId, fileName);
            string destinationPath = GetPreviewFilePath(destinationBoardId, destinationFileName);
            CopyFile(path, destinationPath);
        }

        public void CopySourceFile(string boardId, string fileName, string destinationBoardId, string destinationFileName)
        {
            string path = GetSourceFilePath(boardId, fileName);
            string destinationPath = GetSourceFilePath(destinationBoardId, destinationFileName);
            CopyFile(path, destinationPath);
        }
    }
}