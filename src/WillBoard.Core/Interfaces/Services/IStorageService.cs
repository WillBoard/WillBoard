namespace WillBoard.Core.Interfaces.Services
{
    public interface IStorageService
    {
        void CreateDirectory(string directoryPath);
        void DeleteDirectory(string directoryPath);

        void CreateBoardDirectory(string boardId);
        void DeleteBoardDirectory(string boardId);

        void DeleteFile(string filePath);

        string GetPreviewFilePath(string boardId, string fileName);
        string GetSourceFilePath(string boardId, string fileName);

        void DeletePreviewFile(string boardId, string fileName);
        void DeleteSourceFile(string boardId, string fileName);

        void CopyFile(string filePath, string destinationFilePath);

        void CopyPreviewFile(string boardId, string fileName, string destinationBoardId, string destinationFileName);
        void CopySourceFile(string boardId, string fileName, string destinationBoardId, string destinationFileName);
    }
}