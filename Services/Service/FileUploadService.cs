using System;
using System.Drawing;
using System.IO;
using ImageMagick;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;

namespace MarketPlace.API.Services.Service
{
    public class FileUploadService : IFileUploadService
    {
        public IWebHostEnvironment hostingEnvironment;
        public FileUploadService(IWebHostEnvironment environment)
        {
            hostingEnvironment = environment;
        }

        public void DeleteImage(string name, string path)
        {
            var fullPath = Path.Combine(hostingEnvironment.ContentRootPath, path) + name;
            if (System.IO.File.Exists(fullPath))
            {
                try
                {
                    System.IO.File.Delete(fullPath);
                }
                catch (System.Exception)
                {
                }
            }
        }

        public void DeleteImageWithThumb(string name, string path)
        {
            var fullPath = Path.Combine(hostingEnvironment.ContentRootPath, path) + name;
            if (System.IO.File.Exists(fullPath))
            {
                try
                {
                    System.IO.File.Delete(fullPath);
                }
                catch (System.Exception)
                {
                }
            }
            var fullPathThumb = Path.Combine(hostingEnvironment.ContentRootPath, path) + "thumb-" + name;
            if (System.IO.File.Exists(fullPathThumb))
            {
                try
                {
                    System.IO.File.Delete(fullPathThumb);
                }
                catch (System.Exception)
                {
                }
            }
        }

        public string UploadImage(IFormFile image, string path)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(hostingEnvironment.ContentRootPath))
                {
                    hostingEnvironment.ContentRootPath = Path.Combine(Directory.GetCurrentDirectory(), "");
                }
                var uploads = Path.Combine(hostingEnvironment.ContentRootPath, path);
                string ext = Path.GetExtension(image.FileName);
                Guid guid = Guid.NewGuid();
                var uniqueFileName = guid.ToString() + ext;
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }
                var filePath = Path.Combine(uploads, uniqueFileName);
                var fileStream = new FileStream(filePath, FileMode.Create);


                image.CopyTo(fileStream);
                fileStream.Close();
                return uniqueFileName;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public string UploadImageWithName(IFormFile image, string path, string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(hostingEnvironment.ContentRootPath))
                {
                    hostingEnvironment.ContentRootPath = Path.Combine(Directory.GetCurrentDirectory(), "");
                }
                var uploads = Path.Combine(hostingEnvironment.ContentRootPath, path);
                string ext = Path.GetExtension(image.FileName);
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }
                var filePath = Path.Combine(uploads, name + ext);
                var fileStream = new FileStream(filePath, FileMode.Create);


                image.CopyTo(fileStream);
                fileStream.Close();
                return name + ext;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public string UploadImageWhitThumb(IFormFile image, string path)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(hostingEnvironment.ContentRootPath))
                {
                    hostingEnvironment.ContentRootPath = Path.Combine(Directory.GetCurrentDirectory(), "");
                }
                var uploads = Path.Combine(hostingEnvironment.ContentRootPath, path);
                string ext = Path.GetExtension(image.FileName);
                Guid guid = Guid.NewGuid();
                var uniqueFileName = guid.ToString() + ext;

                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }
                var filePath = Path.Combine(uploads, uniqueFileName);
                var filePathThumb = Path.Combine(uploads, "thumb-" + uniqueFileName);

                var fileStream = new FileStream(filePath, FileMode.Create);
                image.CopyTo(fileStream);

          
                using (MagickImage imageT = new MagickImage(filePath))
                {
                    var width = (250 * imageT.Width) / imageT.Height;
                    MagickGeometry size = new MagickGeometry(width, 250);
                    size.IgnoreAspectRatio = true;
                    imageT.Resize(size);
                    // Save the result
                    imageT.Write(filePathThumb);
                }
                fileStream.Close();
                return uniqueFileName;
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }
        public bool ChangeDestOfFile(string fileName, string SourceFolder, string DestFolder , bool isMove = true)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(hostingEnvironment.ContentRootPath))
                {
                    hostingEnvironment.ContentRootPath = Path.Combine(Directory.GetCurrentDirectory(), "");
                }
                var SourceFolderPath = Path.Combine(hostingEnvironment.ContentRootPath, SourceFolder);
                var DestFolderPath = Path.Combine(hostingEnvironment.ContentRootPath, DestFolder);
                if (!Directory.Exists(DestFolderPath))
                {
                    Directory.CreateDirectory(DestFolderPath);
                }
                var filePath = Path.Combine(SourceFolderPath, fileName);

                var destFilePath = Path.Combine(DestFolderPath, fileName);

                if (System.IO.File.Exists(destFilePath))
                {
                    try
                    {
                        System.IO.File.Delete(destFilePath);
                    }
                    catch (System.Exception)
                    {
                    }
                }
                if(isMove) {
                 System.IO.File.Move(filePath, destFilePath);
                } else if(!isMove) {
                 System.IO.File.Copy(filePath, destFilePath);
                }
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
        public bool ChangeDestOfFileWhitThumb(string fileName, string SourceFolder, string DestFolder)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(hostingEnvironment.ContentRootPath))
                {
                    hostingEnvironment.ContentRootPath = Path.Combine(Directory.GetCurrentDirectory(), "");
                }
                var SourceFolderPath = Path.Combine(hostingEnvironment.ContentRootPath, SourceFolder);
                var DestFolderPath = Path.Combine(hostingEnvironment.ContentRootPath, DestFolder);
                if (!Directory.Exists(DestFolderPath))
                {
                    Directory.CreateDirectory(DestFolderPath);
                }
                var filePath = Path.Combine(SourceFolderPath, fileName);
                var filePathThumb = Path.Combine(SourceFolderPath, "thumb-" + fileName);

                var destFilePath = Path.Combine(DestFolderPath, fileName);
                var destFilePathThumb = Path.Combine(DestFolderPath, "thumb-" + fileName);


                System.IO.File.Move(filePath, destFilePath);
                System.IO.File.Move(filePathThumb, destFilePathThumb);
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public void DeleteDirectory(string path)
        {
            var fullPath = Path.Combine(hostingEnvironment.ContentRootPath, path);
            if (System.IO.Directory.Exists(fullPath))
            {
                try
                {
                    System.IO.Directory.Delete(fullPath, true);
                }
                catch (System.Exception)
                {
                }
            }
        }


    }
}