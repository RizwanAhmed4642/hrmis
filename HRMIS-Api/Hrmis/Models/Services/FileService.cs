using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Hrmis.Models.DbModel;
using Hrmis.Models.Dto;
using Hrmis.Models.ImageProcessor;
using Newtonsoft.Json;

namespace Hrmis.Models.Services
{
    public class FileService : IDisposable
    {
        /// <summary>
        /// Cnic string must be set before calling any service method
        /// </summary>
        public string Cnic { get; set; }

        /// <summary>
        /// Root Path for all Uploaded Image Files
        /// </summary>
        public string RootPath { get; set; }

        /// <summary>
        /// Root path for downloading Files
        /// </summary>
        public string RootDownloadPath { get; set; }

        /// <summary>
        /// Employee Directory Path by Provided CNIC
        /// </summary>
        public string DirPath => $"{RootPath}{Cnic.Replace("-", "")}";

        /// <summary>
        /// Employee Original Directory Path by Provided CNIC
        /// </summary>
        public string DirPathOriginalFiles => $@"{DirPath}\UnCompressed";

        /// <summary>
        /// Application User 
        /// </summary>
        public ApplicationUser User;


        private HR_System _db = new HR_System();

        public FileService(string rootPath, string rootDownloadPath)
        {
            RootPath = rootPath;
            RootDownloadPath = rootDownloadPath;
        }


        public async Task Save(HttpRequestMessage request)
        {
            MultipartMemoryStreamProvider provider = new MultipartMemoryStreamProvider();
            await request.Content.ReadAsMultipartAsync(provider);
            CreateDirectoryIfNotExists(DirPath, DirPathOriginalFiles);
            HrFile hrFileVm = JsonConvert.DeserializeObject<HrFile>(HttpContext.Current.Request.Params["efile"]);
            HrFile hrFile = GetHrFile(hrFileVm);

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    // New Employee
                    if (hrFile == null)
                    {
                        hrFileVm.TotalPages = CalculateMultipartContent(provider.Contents);
                        Entity_Lifecycle lifecycle = CreateEnityLifeCycle();
                       // hrFileVm.EntityLifeCycleId = lifecycle.Id;
                        hrFile = SaveHrFile(hrFileVm);
                    }
                    else
                    {
                        hrFile.TotalPages = CalculateMultipartContent(provider.Contents) + FindMaxFileNo() ;
                        UpdateHrFile(hrFile);
                    }

                    foreach (var file in provider.Contents)
                    {
                        if (file.Headers.ContentDisposition.Name == "\"efile\"") continue;
                        byte[] buffer = await file.ReadAsByteArrayAsync();

                        SaveImage(file, buffer, hrFile);
                    }

                    _db.SaveChanges();
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw;
                }
                finally
                {
                    Dispose();
                }
            }

        }


        private void SaveImage(HttpContent file, byte[] buffer, HrFile hrFile)
        {
            var filename = file.Headers.ContentDisposition.FileName.Trim('\"');

            var size = buffer.Length / 1024 / 1024;
            var ext = Path.GetExtension(filename.Replace("\"", string.Empty));

            var pageNo = FindMaxFileNo() + 1;

            var path = $@"{DirPath}\{pageNo}{ext}";
            var originalPath = $@"{DirPathOriginalFiles}\{pageNo}{ext}";

            if (!GetValidExtensions().Contains(ext, StringComparer.OrdinalIgnoreCase) || size > 5)
            {
                throw new Exception("Unable to Upload. File Size must be less than 5 MB and File Format must be " +
                                    string.Join(",", GetValidExtensions()));
            }

            var imgProcessor = new ImageProcessingManager(path);
            Image image = new Bitmap(ImagesUtil.GetImageFromBytes(buffer));
            int percentWidth = ((image.Size.Width / 100) * 45);
            int percentHeight = ((image.Size.Height / 100) * 45);

            imgProcessor.SaveCompressedImage(ImagesUtil.ScaleImage(image, percentWidth, percentHeight));
            image.Save(originalPath);

            Entity_Lifecycle lifecycle = CreateEnityLifeCycle();

            HrFileDetail detail = new HrFileDetail
            {
                ImagePath = path.Replace(HttpContext.Current.Server.MapPath("~/"), "~/").Replace(@"\", "/"),
                PageName = filename,
                PageNo = pageNo.ToString(),
                ImageQuality = int.Parse(ConfigurationManager.AppSettings["hrImageQualityLevel"]),
                HrFileId = hrFile.Id,
                EntityLifeCycleId = lifecycle.Id
            };
            _db.HrFileDetails.Add(detail);
        }

        private Entity_Lifecycle CreateEnityLifeCycle()
        {
            var lifecycle = new Entity_Lifecycle
            {
                Created_By = User.UserName,
                Created_Date = DateTime.UtcNow.AddHours(5),
                Users_Id = User.Id,
                Entity_Id = 7
            };

            _db.Entity_Lifecycle.Add(lifecycle);
            _db.SaveChanges();
            return lifecycle;
        }

        private HrFile GetHrFile(HrFile hrFile)
        {
            HrFile file;
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                //file = db.HrFiles.Where(x => x.CNIC == hrFile.CNIC).Include(x => x.HrFileDetails).FirstOrDefault();
                file = db.HrFiles.Where(x => x.CNIC == hrFile.CNIC).FirstOrDefault();
            }
            return file;
        }

        private HrFile SaveHrFile(HrFile file)
        {
            _db.HrFiles.Add(file);
            _db.SaveChanges();
            return file;
        }

        private HrFileDetail SaveHrFileDetail(HrFileDetail file)
        {
            _db.HrFileDetails.Add(file);
            _db.SaveChanges();
            return file;
        }


        private HrFile UpdateHrFile(HrFile file)
        {
            _db.Configuration.ProxyCreationEnabled = false;
            var hrFile = _db.HrFiles.FirstOrDefault(x => x.CNIC == file.CNIC);

            if (hrFile == null) return null;

            hrFile.DeptName = file.DeptName;
            hrFile.FileNo = file.FileNo;
            hrFile.Rack = file.Rack;
            hrFile.Row = file.Row;
            hrFile.Room = file.Room;
            hrFile.TotalPages = file.TotalPages;

            //Entity_Lifecycle lifecycle =
            //    _db.Entity_Lifecycle.FirstOrDefault(x => x.Id == hrFile.EntityLifeCycleId);
            //lifecycle?.Entity_Modified_Log.Add(new Entity_Modified_Log
            //{
            //    Modified_Date = DateTime.UtcNow.AddHours(5),
            //    Description = $"{file.FileNo} Modified",
            //    Modified_By = User.UserName,
            //});
            _db.SaveChanges();
            return hrFile;
        }


        private bool HasHrFileExists(HrFile hrFile)
        {
            bool isExists;
            if (hrFile == null) return false;
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                isExists = db.HrFiles.Count(x => x.CNIC == hrFile.CNIC) > 0;
            }
            return isExists;
        }

        public List<FileInfo> GetFiles(string[] filters)
        {
            List<FileInfo> files = new List<FileInfo>();

            foreach (var filter in filters)
            {
                files.AddRange(new DirectoryInfo(DirPath).GetFiles($"*{filter}", SearchOption.TopDirectoryOnly));
            }

            return files;
        }

        public List<FileDto> GetFiles(string[] filters, HttpRequestMessage request)
        {
            List<FileInfo> files = new List<FileInfo>();

            foreach (var filter in filters)
            {
                files.AddRange(new DirectoryInfo(DirPath).GetFiles($"*{filter}", SearchOption.TopDirectoryOnly));
            }
            var fitlDto = GenerateFileModels(files, request);
            return fitlDto;
        }

        public List<byte[]> GetFiles(string path, List<FileDto> fileDtos)
        {
            List<byte[]> files = new List<byte[]>();
            foreach (var file in fileDtos)
            {
                var fPath = path + @"\" + file.Name;
                if (File.Exists(fPath))
                {
                    files.Add(File.ReadAllBytes(fPath));
                }
            }
            return files;
        }

        public byte[] DownloadFile(string fileName)
        {
            var filePath = DirPath + @"\" + fileName;
            byte[] bytes = File.ReadAllBytes(filePath);
            File.WriteAllBytes(RootDownloadPath + @"\" + fileName, bytes);
            return bytes;
        }

        public void DownloadFiles(List<FileDto> files)
        {
            byte[] bytes = new ZipFileManager().CreateZipFile(DirPath, files);
            File.WriteAllBytes(DirPath + ".zip", bytes);
        }

        public FileInfo DownloadZipFile(string cnic)
        {
            var filePath = RootDownloadPath + @"\" + cnic + ".zip";
            return new FileInfo(filePath);
        }

        public List<FileDto> GenerateFileModels(List<FileInfo> files, HttpRequestMessage request)
        {
            List<FileDto> fileNameList = new List<FileDto>();
            foreach (var file in files)
            {
                FileDto dto = new FileDto();
                string filePath = file.FullName.Replace(HttpContext.Current.Server.MapPath("~/"), "~/")
                    .Replace(@"\", "/");
                dto.ImageSrc = (request.RequestUri.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped) +
                                VirtualPathUtility.ToAbsolute(filePath));
                dto.Name = file.Name;
                dto.Thumbnail =
                    $@"data:image/{file.Extension.Replace(".", "")};base64,{
                        ImagesUtil.ImageToBase64(
                            ImagesUtil.ResizeImage(Image.FromFile(file.FullName), new Size(154, 102)), ImageFormat.Bmp)
                        }";
                fileNameList.Add(dto);
            }

            return fileNameList;
        }


        public void RemoveFiles(List<FileDto> dto)
        {
            foreach (var item in dto)
            {
                try
                {
                    File.Delete(DirPath + "\\" + item.Name);
                }
                catch (Exception ex)
                {
                    // ignored
                }
            }
        }

        public List<string> ConvertPathToRelative(List<string> fileNames, HttpRequestMessage request)
        {
            List<string> fileNameList = new List<string>();
            foreach (var file in fileNames)
            {
                string filePath = file.Replace(HttpContext.Current.Server.MapPath("~/"), "~/").Replace(@"\", "/");
                fileNameList.Add(request.RequestUri.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped) +
                                 System.Web.VirtualPathUtility.ToAbsolute(filePath));
            }

            return fileNameList;
        }

        public List<string> ConvertImageToBase64(List<string> fileNames)
        {
            List<string> fileNameList = new List<string>();
            foreach (var file in fileNames)
            {
                var extension = Path.GetExtension(file);
                if (extension != null)
                {
                    string filePath =
                        $@"data:image/{extension.Replace(".", "")};base64,{ImagesUtil.ImageToBase64(file)}";
                    fileNameList.Add(filePath);
                }
            }

            return fileNameList;
        }

        public void CreateDirectoryIfNotExists(string dirPath, string originalFilesDir)
        {
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);

            }
            if (!Directory.Exists(originalFilesDir))
            {
                Directory.CreateDirectory(originalFilesDir);

            }
        }

        private int CalculateMultipartContent(Collection<HttpContent> contents)
        {
            return contents.Count(item => item.Headers.ContentDisposition.Name != "\"efile\"");
        }


        private int FindMaxFileNo()
        {
            int maxFileNo = 0;
            var files = GetFiles(GetValidExtensions().ToArray());
            foreach (var file in files)
            {
                try
                {
                    int length = file.Name.LastIndexOf(".", StringComparison.Ordinal) - 0;
                    var fileNo = Convert.ToInt32(file.Name.Substring(0, length));
                    if (fileNo > maxFileNo)
                    {
                        maxFileNo = fileNo;
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            return maxFileNo;
        }


        private List<string> GetValidExtensions()
        {
            return new List<string> { ".jpg", ".jpeg", ".png" };
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}