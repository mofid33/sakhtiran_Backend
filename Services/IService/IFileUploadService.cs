using Microsoft.AspNetCore.Http;

namespace MarketPlace.API.Services.IService
{
    public interface IFileUploadService
    {
        string UploadImage(IFormFile image, string path);
        string UploadImageWithName(IFormFile image, string path,string name);
        string UploadImageWhitThumb(IFormFile image, string path);
        void DeleteImage(string name, string path);
        void DeleteDirectory(string path);
        void DeleteImageWithThumb(string name, string path);
        bool ChangeDestOfFile(string fileName, string SourceFolder, string DestFolder, bool isMove = true);
        bool ChangeDestOfFileWhitThumb(string fileName, string SourceFolder, string DestFolder);

    }
}