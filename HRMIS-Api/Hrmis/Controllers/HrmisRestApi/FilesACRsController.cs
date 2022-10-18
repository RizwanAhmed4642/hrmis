using Hrmis.Models.Common;
using Hrmis.Models.DbModel;
using Hrmis.Models.ImageProcessor;
using Hrmis.Models.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Zen.Barcode;

namespace Hrmis.Controllers.HrmisRestApi
{
    [Authorize]
    [RoutePrefix("api/FilesACRs")]
    public class FilesACRsController : ApiController
    {
        private readonly FilesACRService _filesACRService;

        public FilesACRsController()
        {
            _filesACRService = new FilesACRService();
        }
        [HttpGet]
        [Route("GetFileMoveMaster/{MID}")]
        public IHttpActionResult GetFileMoveMaster(int MID)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var fileMoveMaster = db.FileMoveMasterViews.FirstOrDefault(x => x.MID_Number == MID && x.IsActive == true);
                    var fileMoveDetails = db.FileMoveDetailViews.Where(x => x.Master_Id == fileMoveMaster.Id && x.IsActive == true).ToList();
                    if (fileMoveMaster == null) return BadRequest("Invalid");
                    return Ok(new { fileMoveMaster, fileMoveDetails });
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }

        // File upload directory action..

        [AllowAnonymous]
        [HttpGet]
        [Route("UploadFilesFolder")]
        public async Task<IHttpActionResult> UploadFilesFolder()
        {
            string filename = "";
            try
            {
                string Extension = "";
                string RootPath = HttpContext.Current.Server.MapPath("~/") + @"Uploads\Files\InquiriesFiles\";
                // Get all subdirectories

                string[] subdirectoryEntries = Directory.GetDirectories(RootPath);
                List<string> subdirectoryEntriesAll = new List<string>();
                //// Loop through them to see if they have any other subdirectories

                foreach (string subdirectory in subdirectoryEntries)
                    subdirectoryEntriesAll.Add(_filesACRService.LoadSubDirs(subdirectory));
                foreach (var item in subdirectoryEntriesAll)
                {
                    if (Directory.Exists(item))
                    {
                        DirectoryInfo di = new DirectoryInfo(item);
                        using (var _db = new HR_System())
                        {
                            FilesMaster fs = new FilesMaster();
                            fs.IsActive = true;
                            fs.Username = User.Identity.GetUserName();
                            fs.User_Id = User.Identity.GetUserId();
                            fs.DateTime = DateTime.UtcNow.AddHours(5);
                            fs.FileNumber = di.Name;
                            fs.FileType_Id = 3;
                            _db.FilesMasters.Add(fs);
                            _db.SaveChanges();

                            int Count = 1;
                            // Copy the files and overwrite destination files if they already exist.
                            foreach (FileInfo s in di.GetFiles())
                            {
                                filename = s.FullName;
                                var name = s.Name.Split('.');
                                var extension = s.Extension;
                                var newfilename = "";
                                if (!string.IsNullOrEmpty(name[0]))
                                {
                                    string r = name[0].Replace("image", "");
                                    int foundIndex = 0;
                                    for (int i = 0; i < r.Length; i++)
                                    {
                                        var a = r[i];
                                        if (a != '0')
                                        {
                                            foundIndex = i;
                                            break;
                                        }
                                    }
                                    newfilename = Guid.NewGuid().ToString() + "_" + r.Substring(foundIndex, (r.Length - foundIndex)) + extension;
                                    File.Move(filename, RootPath + di.Name + @"\" + newfilename);
                                }
                                FileDetail fsdetail = new FileDetail();
                                fsdetail.IsActive = true;
                                fsdetail.Username = User.Identity.GetUserName();
                                fsdetail.User_Id = User.Identity.GetUserId();
                                fsdetail.DateTime = DateTime.UtcNow.AddHours(5);
                                //fsdetail.Title = newfilename;
                                fsdetail.FileMaster_Id = fs.Id;
                                fsdetail.Path = di.Name + @"\" + newfilename;
                                _db.FileDetails.Add(fsdetail);
                                _db.SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Source path does not exist!");
                    }
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(new { result = true, src = filename });
        }

        [HttpPost]
        [Route("SubmitFileMovement")]
        public IHttpActionResult SubmitFileMovement([FromBody] FileMoveMaster fileMM)
        {
            try
            {
                var fileMoveMaster = _filesACRService.SubmitFileMovement(fileMM, User.Identity.GetUserName(), User.Identity.GetUserId());
                if (fileMoveMaster == null) return BadRequest("Invalid");
                Image barCode = Common.barCodeZ(Convert.ToInt32(fileMoveMaster.MID_Number));
                string imgSrc = "data:image / jpg; base64," + ImagesUtil.ImageToBase64(barCode, ImageFormat.Jpeg);

                return Ok(new { fileMoveMaster, barCode = imgSrc });
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("AddFile")]
        public IHttpActionResult AddFile([FromBody] DDS_Files dds)
        {
            try
            {
                if (dds.F_FileNumber == null) return Ok("Invalid");
                var ddsFile = _filesACRService.AddFile(dds, User.Identity.GetUserName(), User.Identity.GetUserId());

                Code128BarcodeDraw barcode = BarcodeDrawFactory.Code128WithChecksum;
                BarcodeSymbology s = BarcodeSymbology.Code39NC;
                BarcodeDraw drawObject = BarcodeDrawFactory.GetSymbology(s);
                var metrics = drawObject.GetDefaultMetrics(50);
                metrics.Scale = 1;
                Image barCode = barcode.Draw(ddsFile.Id.ToString(), metrics);
                string imgSrc = "data:image/jpg;base64," + ImagesUtil.ImageToBase64(barCode, ImageFormat.Jpeg);

                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    DDS_Files ddsFileDb = _db.DDS_Files.FirstOrDefault(x => x.Id == ddsFile.Id);
                    return Ok(new { ddsFile = ddsFileDb, barCode = imgSrc });
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("AddLawFile")]
        public IHttpActionResult AddLawFile([FromBody] LawFileDto file)
        {
            try
            {
                var res = _filesACRService.AddLawFile(file, User.Identity.GetUserName(), User.Identity.GetUserId());

                Code128BarcodeDraw barcode = BarcodeDrawFactory.Code128WithChecksum;
                BarcodeSymbology s = BarcodeSymbology.Code39NC;
                BarcodeDraw drawObject = BarcodeDrawFactory.GetSymbology(s);
                var metrics = drawObject.GetDefaultMetrics(50);
                metrics.Scale = 1;
                Image barCode = barcode.Draw(res.ID.ToString(), metrics);
                string imgSrc = "data:image/jpg;base64," + ImagesUtil.ImageToBase64(barCode, ImageFormat.Jpeg);

                return Ok(new { file = res, barCode = imgSrc });
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetLawFile/{Id}")]
        public IHttpActionResult GetLawFile(int Id)
        {
            try
            {
                var res = _filesACRService.GetLawFile(Id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetLawFilePetitioners/{fileId}")]
        public IHttpActionResult GetLawFilePetitioners(int fileId)
        {
            try
            {
                var res = _filesACRService.GetLawFilePetitioners(fileId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetLawFileOfficers/{fileId}")]
        public IHttpActionResult GetLawFileOfficers(int fileId)
        {
            try
            {
                var res = _filesACRService.GetLawFileOfficers(fileId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetLawFileRespondants/{fileId}")]
        public IHttpActionResult GetLawFileRespondants(int fileId)
        {
            try
            {
                var res = _filesACRService.GetLawFileRespondants(fileId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetFileAttachments/{fileId}")]
        public IHttpActionResult GetFileAttachments(int fileId)
        {
            try
            {
                var res = _filesACRService.GetFileAttachments(fileId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("RemoveFileAttachments/{fileId}")]
        public IHttpActionResult RemoveFileAttachments(int fileId)
        {
            try
            {
                var res = _filesACRService.RemoveFileAttachments(fileId, User.Identity.GetUserId());
                return Ok(res);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetLawFileAttachments/{fileId}")]
        public IHttpActionResult GetLawFileAttachments(int fileId)
        {
            try
            {
                var res = _filesACRService.GetLawFileAttachments(fileId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("RemoveLawFileAttachments/{fileId}")]
        public IHttpActionResult RemoveLawFileAttachments(int fileId)
        {
            try
            {
                var res = _filesACRService.RemoveLawFileAttachments(fileId, User.Identity.GetUserId());
                return Ok(res);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("RemoveLawFile/{fileId}")]
        public IHttpActionResult RemoveLawFile(int fileId)
        {
            try
            {
                var res = _filesACRService.RemoveLawFile(fileId, User.Identity.GetUserId());
                return Ok(res);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("DDSSouth/{ddsId}")]
        public IHttpActionResult DDSSouth(int ddsId)
        {
            try
            {
                var res = _filesACRService.DDSSouth(ddsId, User.Identity.GetUserId());
                return Ok(res);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("DDSPSHD/{ddsId}")]
        public IHttpActionResult DDSPSHD(int ddsId)
        {
            try
            {
                var res = _filesACRService.DDSPSHD(ddsId, User.Identity.GetUserId());
                return Ok(res);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("HideDuplicationFile/{ddsId}")]
        public IHttpActionResult HideDuplicationFile(int ddsId)
        {
            try
            {
                var res = _filesACRService.HideDuplicationFile(ddsId, User.Identity.GetUserId());
                return Ok(res);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("GetLawFiles")]
        public IHttpActionResult GetLawFiles([FromBody] LawFileFilters filters)
        {
            try
            {
                var res = _filesACRService.GetLawFiles(filters);
                return Ok(res);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [Route("UploadFileAttachment/{ddsId}")]
        public async Task<IHttpActionResult> UploadAttachment(int ddsId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    if (!Request.Content.IsMimeMultipartContent())
                        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                    string RootPath = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\Files\ACRFiles\";
                    var dirPath = RootPath;

                    var provider = new MultipartMemoryStreamProvider();
                    await Request.Content.ReadAsMultipartAsync(provider);
                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }
                    string filename = "";

                    foreach (var file in provider.Contents)
                    {
                        var buffer = await file.ReadAsByteArrayAsync();
                        var size = ((buffer.Length) / (1024)) / (1024);
                        var ext = Path.GetExtension(file.Headers.ContentDisposition.FileName.Trim('\"').Replace("\"", string.Empty));
                        filename = Guid.NewGuid().ToString() + "_" + ddsId.ToString() + ext;
                        List<string> validExtensions = new List<string>() { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                        if (!validExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase) || size > 4)
                        {
                            throw new Exception("Unable to Upload. File Size must be less than 4 MB and File Format must be " + string.Join(",", validExtensions));
                        }
                        using (FileStream fsOut = File.OpenWrite(RootPath + @"\" + filename))
                        {
                            fsOut.Write(buffer, 0, buffer.Length);
                        }

                        DDS_Attachments dDS_Attachments = new DDS_Attachments();
                        dDS_Attachments.DDs_Id = ddsId;
                        dDS_Attachments.DDs_Url = filename;
                        dDS_Attachments.LocalPath = User.Identity.GetUserId();
                        //dDS_Attachments.CreatedOn = DateTime.UtcNow.AddHours(5);
                        _db.DDS_Attachments.Add(dDS_Attachments);
                        _db.SaveChanges();
                    }
                    return Ok(new { result = true, src = filename });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetFilesList")]
        public async Task<IHttpActionResult> GetFilesList()
        {
            try
            {
                using (var _db = new HR_System())
                {
                    var res = _db.FilesMasters.Where(s => s.IsActive == true).ToList();
                    if (res == null || res.Count < 1)
                    {
                        res = new List<FilesMaster>();
                    }
                    return Ok(res);
                }

            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }

        [Route("UploadLawFile/{fileId}")]
        public async Task<IHttpActionResult> UploadLawFile(int fileId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    if (!Request.Content.IsMimeMultipartContent())
                        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                    string RootPath = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\Files\LawFiles\";
                    var dirPath = RootPath;

                    var provider = new MultipartMemoryStreamProvider();
                    await Request.Content.ReadAsMultipartAsync(provider);
                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }
                    string filename = "";

                    foreach (var file in provider.Contents)
                    {
                        var FileExtension = Path.GetExtension(file.Headers.ContentDisposition.FileName.Trim('\"')).Substring(1);
                        filename = Guid.NewGuid().ToString() + "_" + fileId + "." + FileExtension;
                        var buffer = await file.ReadAsByteArrayAsync();
                        var size = ((buffer.Length) / (1024)) / (1024);
                        List<string> validExtensions = new List<string>() { "jpg", "jpeg", "pdf", "docx", "doc" };
                        if (!validExtensions.Contains(FileExtension, StringComparer.OrdinalIgnoreCase) || size > 5)
                        {
                            throw new Exception("Unable to Upload. File Size must be less than 5 MB and File Format must be " + string.Join(",", validExtensions));
                        }
                        using (FileStream fsOut = File.OpenWrite(RootPath + @"\" + filename))
                        {
                            fsOut.Write(buffer, 0, buffer.Length);
                        }
                        LawFilesImage lawFilesImage = new LawFilesImage();
                        lawFilesImage.FilesID = fileId;
                        lawFilesImage.PhotName = filename;
                        lawFilesImage.CreatedBy = User.Identity.GetUserId();
                        lawFilesImage.CreatedOn = DateTime.UtcNow.AddHours(5);
                        _db.LawFilesImages.Add(lawFilesImage);
                        _db.SaveChanges();
                    }
                    return Ok(new { result = true, src = filename });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetDDsBarcode/{id}")]
        public IHttpActionResult GetDDsBarcode(int id)
        {
            try
            {
                Code128BarcodeDraw barcode = BarcodeDrawFactory.Code128WithChecksum;
                BarcodeSymbology s = BarcodeSymbology.Code39NC;
                BarcodeDraw drawObject = BarcodeDrawFactory.GetSymbology(s);
                var metrics = drawObject.GetDefaultMetrics(50);
                metrics.Scale = 1;
                Image barCode = barcode.Draw(id.ToString(), metrics);
                string imgSrc = "data:image/jpg;base64," + ImagesUtil.ImageToBase64(barCode, ImageFormat.Jpeg);
                return Ok(new { barCode = imgSrc });
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetFiles")]
        public IHttpActionResult GetFiles([FromBody] FilesACRsFilter filter)
        {
            try
            {
                var files = _filesACRService.GetFiles(filter, User.Identity.GetUserName(), User.Identity.GetUserId());
                if (files == null) return BadRequest("Invalid");
                return Ok(files);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetFileByCodeBar")]
        public IHttpActionResult GetFileByCodeBar([FromBody] FilesACRsFilter filter)
        {
            try
            {
                var files = _filesACRService.GetFileByCodeBar(filter.Barcode);
                if (files == null) return Ok("Invalid");
                return Ok(files);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("GetFilesByCNIC")]
        public IHttpActionResult GetFilesByCNIC([FromBody] FilesACRsFilter filter)
        {
            try
            {
                var files = _filesACRService.GetFilesByCNIC(filter.cnic);
                if (files == null) return Ok("Invalid");
                return Ok(files);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("GetDDSFiles")]
        public IHttpActionResult GetDDSFiles([FromBody] FilesACRsFilter filter)
        {
            try
            {
                //check for record room user or acr user or admin user
                //record room user will get personal files (not acrs)
                //acr user will get acrs not personal files
                //admin will get all files

                int type = -1;
                if (User.IsInRole("Administrator")) { type = 0; }
                if (User.IsInRole("ACR Room")) { type = 1; }
                if (User.IsInRole("Central Record Room")) { type = 2; }
                if (User.IsInRole("South Punjab")) { type = 3; }

                var files = _filesACRService.GetDDSFiles(filter, type, User.Identity.GetUserName(), User.Identity.GetUserId());
                if (files == null) return BadRequest("Invalid");
                return Ok(files);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetDDSDetails")]
        public IHttpActionResult GetDDSDetails([FromBody] FilesACRsFilter filter)
        {
            try
            {
                var details = _filesACRService.GetDDSDetails(filter, User.Identity.GetUserName(), User.Identity.GetUserId());
                if (details == null) return Ok("Invalid");
                return Ok(details);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveDDSDetails")]
        public IHttpActionResult SaveDDSDetails([FromBody] DDsDetail dDsDetail)
        {
            try
            {
                if (dDsDetail.DDS_Id == null || dDsDetail.DDS_Id == 0 || dDsDetail.FromPeriod == null || dDsDetail.ToPeriod == null)
                {
                    return Ok("Invalid");
                }
                using (var _db = new HR_System())
                {
                    Entity_Lifecycle elc = new Entity_Lifecycle();
                    elc.IsActive = true;
                    elc.Created_By = User.Identity.GetUserName();
                    elc.Users_Id = User.Identity.GetUserId();
                    elc.Created_Date = DateTime.UtcNow.AddHours(5);
                    elc.Entity_Id = 878;

                    _db.Entity_Lifecycle.Add(elc);
                    _db.SaveChanges();

                    dDsDetail.EntityLifecycle_Id = elc.Id;
                    _db.DDsDetails.Add(dDsDetail);
                    _db.SaveChanges();

                    return Ok(dDsDetail);
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("RemoveDDSDetail/{Id}")]
        public IHttpActionResult RemoveDDSDetail(int Id)
        {
            try
            {
                if (Id == 0)
                {
                    return Ok("Invalid");
                }
                using (var _db = new HR_System())
                {
                    var ddsDetail = _db.DDsDetails.Find(Id);

                    Entity_Modified_Log eml = new Entity_Modified_Log();
                    eml.Modified_By = User.Identity.GetUserId();
                    eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                    eml.Entity_Lifecycle_Id = (long)ddsDetail.EntityLifecycle_Id;
                    eml.Description = "DDsFile Detail Removed by " + User.Identity.GetUserName();
                    _db.Entity_Modified_Log.Add(eml);
                    _db.SaveChanges();
                    _db.DDsDetails.Remove(ddsDetail);
                    _db.SaveChanges();
                    return Ok(true);
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("GetDDSFilesByCNIC")]
        public IHttpActionResult GetDDSFilesByCNIC([FromBody] FilesACRsFilter filter)
        {
            try
            {
                var files = _filesACRService.GetDDSFilesByCNIC(filter.cnic);
                if (files == null) return Ok("Invalid");
                return Ok(files);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("GetFilesByFileNumber")]
        public IHttpActionResult GetFilesByFileNumber([FromBody] FilesACRsFilter filter)
        {
            try
            {
                var files = _filesACRService.GetFilesByFileNumber(filter.FileNumber);
                if (files == null) return BadRequest("Invalid");
                return Ok(files);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("GetDDSFilesByFileNumber")]
        public IHttpActionResult GetDDSFilesByFileNumber([FromBody] FilesACRsFilter filter)
        {
            try
            {
                var files = _filesACRService.GetDDSFilesByFileNumber(filter.FileNumber);
                if (files == null) return BadRequest("Invalid");
                return Ok(files);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("GetLawFilesByFileNumber")]
        public IHttpActionResult GetLawFilesByFileNumber([FromBody] FilesACRsFilter filter)
        {
            try
            {
                var files = _filesACRService.GetLawFilesByFileNumber(filter.FileNumber);
                if (files == null) return BadRequest("Invalid");
                return Ok(files);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("GetDDSFileById")]
        public IHttpActionResult GetDDSFileById([FromBody] FilesACRsFilter filter)
        {
            try
            {
                var ddsfile = _filesACRService.GetDDSFileById(filter.DDsId);
                if (ddsfile == null) return Ok("Invalid");
                return Ok(ddsfile);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("GetDDSFileByCodeBar")]
        public IHttpActionResult GetDDSFileByCodeBar([FromBody] FilesACRsFilter filter)
        {
            try
            {
                var ddsfile = _filesACRService.GetDDSFileByCodeBar(filter.Barcode);
                if (ddsfile == null) return Ok("Invalid");
                return Ok(ddsfile);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [Route("GenerateFileRequest/{file_Id}")]
        [HttpPost]
        public IHttpActionResult GenerateFileRequest([FromBody] ApplicationLog applog, int file_Id)
        {
            using (var db = new HR_System())
            {
                try
                {
                    List<ApplicationFileReqView> recentFileRequests = new List<ApplicationFileReqView>();

                    //var southFiles = db.DDS_Attachments.Where(x => x.DDs_Id == file_Id).ToList();
                    //if (southFiles.Count > 0)
                    //{
                    //    //return Ok(new { result = false, status = "File has been shifted to South Punjab, Secretariat." });
                    //}


                    //Check tracking Exist
                    var app = db.ApplicationMasters.FirstOrDefault(x => x.Id == applog.Application_Id && x.IsActive == true);
                    if (app == null)
                    {
                        return Ok(new { result = false, status = "No File / PUC / Document exist against tracking number " + (applog.Application_Id + 9001) });
                    }

                    var ddsFile = db.DDSViews.FirstOrDefault(x => x.Id == file_Id);
                    string ddsName = "";
                    string ddsFileNo = "";
                    if (ddsFile != null)
                    {
                        ddsFileNo = ddsFile.DiaryNo;
                        ddsName = ddsFile.Subject;
                    }
                    recentFileRequests = db.ApplicationFileReqViews.Where(x => (x.DDS_Id == file_Id || x.UpdatedFileNumber == ddsFileNo || x.FileNumber == ddsFileNo || x.F_FileNumber == ddsFileNo || x.DiaryNo == ddsFileNo) && (x.RequestStatus_Id == 2 || x.RequestStatus_Id == 1 || x.RequestStatus_Id == 11) && x.IsActive == true).OrderByDescending(x => x.Created_Date).ToList();
                    if (recentFileRequests.Count > 0)
                    {
                        return Ok(new { result = false, status = recentFileRequests[0].Status, madeBy = recentFileRequests[0].SectionName });
                    }
                    using (var trans = db.Database.BeginTransaction())
                    {
                        try
                        {
                            db.Configuration.ProxyCreationEnabled = false;
                            string userId = User.Identity.GetUserId();
                            var currentOfficer = db.P_SOfficers.FirstOrDefault(x => x.User_Id.Equals(userId));
                            ApplicationFileRecosition afr = new ApplicationFileRecosition();

                            afr.RequestStatus_Id = 1;

                            afr.DDS_Id = file_Id;

                            afr.RequestGenDateTime = DateTime.UtcNow.AddHours(5);
                            if (!string.IsNullOrEmpty(applog.Remarks))
                            {
                                afr.Reason = applog.Remarks;
                            }
                            afr.Created_By = User.Identity.GetUserName();
                            afr.Users_Id = User.Identity.GetUserId();
                            afr.Officer_Id = currentOfficer.Id;
                            afr.Created_Date = afr.RequestGenDateTime;
                            afr.IsActive = true;

                            db.ApplicationFileRecositions.Add(afr);
                            db.SaveChanges();

                            if (app != null)
                            {
                                afr.Application_Id = app.Id;
                                if (app.Profile_Id != null) afr.Profile_Id = app.Profile_Id;
                                else afr.Profile_Id = null;

                                applog.Remarks = applog.Remarks;

                                applog.FileRequestByOfficer_Id = currentOfficer.Id;
                                applog.FileRequestTime = afr.Created_Date;
                                applog.FileRequest_Id = afr.Id;

                                applog.DateTime = afr.Created_Date;
                                applog.CreatedBy = User.Identity.GetUserName();
                                applog.User_Id = userId;
                                applog.IsActive = true;

                                //File Requested
                                applog.Action_Id = 5;

                                var fromStatus = db.ApplicationStatus.FirstOrDefault(x => x.Id == app.Status_Id);
                                applog.FromStatus_Id = fromStatus.Id;
                                applog.FromStatus = fromStatus.Name;

                                app.Status_Id = 12;
                                app.StatusTime = applog.DateTime;
                                app.StatusByOfficer_Id = currentOfficer.Id;
                                app.StatusByOfficerName = currentOfficer.DesignationName;

                                applog.ToStatus_Id = 12;
                                applog.ToStatus = "File Requested";

                                applog.StatusByOfficer_Id = currentOfficer.Id;
                                applog.StatusByOfficer = currentOfficer.DesignationName;
                                db.ApplicationLogs.Add(applog);
                                db.SaveChanges();

                                app.FileRequest_Id = afr.Id;
                                app.FileRequested = true;
                                var fileRequestStatus = db.FileRequestStatus.FirstOrDefault(x => x.Id == afr.RequestStatus_Id);
                                app.FileRequestStatus = fileRequestStatus.ReuestStatus;
                                app.FileRequestStatus_Id = afr.RequestStatus_Id;
                                app.FileRequestTime = afr.RequestGenDateTime;
                                app.CurrentLog_Id = applog.Id;

                                db.SaveChanges();
                            }
                            else
                            {
                                afr.Application_Id = null;
                            }

                            trans.Commit();
                            return Ok(new { result = true, afrId = afr.Id });
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            return BadRequest(ex.Message);
                        }
                    }


                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }

        [Route("GetFileRequests")]
        [HttpPost]
        public IHttpActionResult GetFileRequests(FilesACRsFilter filter)
        {
            using (var db = new HR_System())
            {
                try
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    IQueryable<ApplicationFileReqView> query = db.ApplicationFileReqViews.Where(x => x.IsActive == true).AsQueryable();

                    var username = User.Identity.GetUserName();
                    if (!string.IsNullOrEmpty(username) && username.StartsWith("acr"))
                    {
                        query = query.Where(x => x.F_FileType_Id == 1);
                    }
                    else
                    {
                        query = query.Where(x => x.F_FileType_Id != 1);
                    }
                    if (filter.StatusId != 0)
                    {
                        if (filter.StatusId == 99)
                        {
                            query = query.Where(x => x.RequestStatus_Id == 1 || x.RequestStatus_Id == 2 || x.RequestStatus_Id == 11).AsQueryable();
                        }
                        else
                        {
                            query = query.Where(x => x.RequestStatus_Id == filter.StatusId).AsQueryable();
                        }
                    }
                    if (filter.Query != null && filter.Query.Length >= 2)
                    {
                        var isNumber = int.TryParse(filter.Query.Replace(" ", ""), out int tracking);

                        if (new RootService().IsCNIC(filter.Query.Replace(" ", "")))
                        {
                            filter.Query = filter.Query.Replace("-", "");
                            query = query.Where(x => x.CNIC.Equals(filter.Query)).AsQueryable();
                        }
                        else
                        {
                            query = query.Where(x => x.EmployeeName.ToLower().Contains(filter.Query.ToLower())
                            || x.SName.ToLower().Contains(filter.Query.ToLower())
                            || x.Name.ToLower().Contains(filter.Query.ToLower())
                            || x.FileNumber.ToLower().Contains(filter.Query.ToLower())
                            || x.UpdatedFileNumber.ToLower().Contains(filter.Query.ToLower())
                            || x.DiaryNo.ToLower().Contains(filter.Query.ToLower())
                            || x.Subject.ToLower().Contains(filter.Query.ToLower())
                            || x.F_FileNumber.ToLower().Contains(filter.Query.ToLower())
                            || x.F_Name.ToLower().Contains(filter.Query.ToLower())
                            || x.UpdatedFileNumber.ToLower().Contains(filter.Query.ToLower())
                            || x.SectionName.ToLower().Contains(filter.Query.ToLower())).AsQueryable();
                        }
                    }
                    List<ApplicationFileReqView> requests = null;
                    if (filter.StatusId == 2)
                    {
                        requests = query.OrderByDescending(x => x.RequestApproveDateTime).Skip(filter.Skip).Take(filter.PageSize).ToList();
                        return Ok(new TableResponse<ApplicationFileReqView>() { Count = query.Count(), List = requests });
                    }
                    if (filter.StatusId == 3)
                    {
                        requests = query.OrderByDescending(x => x.ReturnDateTime).Skip(filter.Skip).Take(filter.PageSize).ToList();
                        return Ok(new TableResponse<ApplicationFileReqView>() { Count = query.Count(), List = requests });
                    }
                    if (filter.StatusId == 99)
                    {
                        requests = query.OrderBy(k => k.Subject)
                            .ThenBy(k => k.Name)
                            .ThenBy(k => k.SName).Skip(filter.Skip).Take(filter.PageSize).ToList();
                        return Ok(new TableResponse<ApplicationFileReqView>() { Count = query.Count(), List = requests });
                    }
                    requests = query.OrderByDescending(x => x.RequestGenDateTime).Skip(filter.Skip).Take(filter.PageSize).ToList();
                    return Ok(new TableResponse<ApplicationFileReqView>() { Count = query.Count(), List = requests });
                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }

        }
        [Route("GetMyRequisitions")]
        [HttpPost]
        public IHttpActionResult GetMyRequisitions(FilesACRsFilter filter)
        {
            using (var db = new HR_System())
            {
                try
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    string userId = User.Identity.GetUserId();
                    P_SOfficers psOfficer = db.P_SOfficers.FirstOrDefault(x => x.User_Id.Equals(userId));
                    IQueryable<ApplicationFileReqView> query;
                    if (psOfficer != null)
                    {
                        query = db.ApplicationFileReqViews.Where(x => x.OfficerCode.ToString().StartsWith(psOfficer.Code.ToString()) && x.IsActive == true).AsQueryable();
                    }
                    else
                    {
                        query = db.ApplicationFileReqViews.Where(x => x.Users_Id.Equals(userId) && x.IsActive == true).AsQueryable();
                    }
                    if (filter.StatusId != 0)
                    {
                        query = query.Where(x => x.RequestStatus_Id == filter.StatusId).AsQueryable();
                    }
                    if (filter.Query != null && filter.Query.Length >= 2)
                    {
                        var isNumber = int.TryParse(filter.Query.Replace(" ", ""), out int tracking);

                        if (new RootService().IsCNIC(filter.Query.Replace(" ", "")))
                        {
                            filter.Query = filter.Query.Replace("-", "");
                            query = query.Where(x => x.CNIC.Equals(filter.Query)).AsQueryable();
                        }
                        else
                        {
                            query = query.Where(x => x.EmployeeName.ToLower().Contains(filter.Query.ToLower())
                            || x.SName.ToLower().Contains(filter.Query.ToLower())
                            || x.Subject.ToLower().Contains(filter.Query.ToLower())
                            || x.DiaryNo.ToLower().Contains(filter.Query.ToLower())
                            || x.F_FileNumber.ToLower().Contains(filter.Query.ToLower())
                            || x.F_Name.ToLower().Contains(filter.Query.ToLower())
                            || x.Reason.ToLower().Contains(filter.Query.ToLower())
                            ).AsQueryable();
                        }
                    }
                    var requests = query.OrderByDescending(x => x.RequestGenDateTime).Skip(filter.Skip).Take(filter.PageSize).ToList();
                    return Ok(new TableResponse<ApplicationFileReqView>() { Count = query.Count(), List = requests });
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }
        [Route("RemoveFileRequest/{Id}")]
        [HttpGet]
        public IHttpActionResult RemoveFileRequest(int Id)
        {
            using (var db = new HR_System())
            {
                try
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    ApplicationFileRecosition afr = db.ApplicationFileRecositions.FirstOrDefault(x => x.Id == Id);
                    if (afr != null)
                    {
                        if (afr.Application_Id != null)
                        {
                            var application = db.ApplicationMasters.FirstOrDefault(x => x.Id == afr.Application_Id && x.Status_Id == 12);
                            if (application != null)
                            {
                                application.Status_Id = 1;
                                db.Entry(application).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        afr.IsActive = false;
                        db.Entry(afr).State = EntityState.Modified;
                        db.SaveChanges();
                        return Ok(true);
                    }
                    else
                    {
                        return Ok(false);
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

        }
        [Route("FileAlreadyIssued/{Id}")]
        [HttpGet]
        public IHttpActionResult FileAlreadyIssued(int Id)
        {
            using (var db = new HR_System())
            {
                try
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    ApplicationFileRecosition afr = db.ApplicationFileRecositions.FirstOrDefault(x => x.Id == Id);
                    if (afr != null)
                    {
                        afr.RequestStatus_Id = 5;
                        db.Entry(afr).State = EntityState.Modified;
                        db.SaveChanges();
                        return Ok(true);
                    }
                    else
                    {
                        return Ok(false);
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

        }
        [Route("GetFileMovements")]
        [HttpPost]
        public IHttpActionResult GetFileMovements([FromBody] FilesACRsFilter filter)
        {
            using (var db = new HR_System())
            {
                try
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    string userId = User.Identity.GetUserId();
                    P_SOfficers psOfficer = db.P_SOfficers.FirstOrDefault(x => x.User_Id.Equals(userId));
                    var diary = db.uspOfficerDiary(psOfficer.Id).ToList();
                    var query = db.FileMoveMasterViews.AsQueryable();

                    if (filter.From == null && filter.To == null)
                    {
                        filter.From = new DateTime(1970, 1, 1);
                        filter.To = DateTime.Now.AddDays(1);
                    }
                    else if (filter.From != null && filter.To == null)
                    {
                        filter.To = DateTime.Now.AddDays(1);
                    }
                    else if (filter.From == null && filter.To == null)
                    {
                        filter.From = new DateTime(1970, 1, 1);
                        filter.To = filter.To.Value.AddDays(1);
                    }
                    else if (filter.From != null && filter.To != null)
                    {
                        filter.To = filter.To.Value.AddDays(1);
                    }
                    if (filter.From != null)
                    {
                        query = query.Where(x => x.DateTime >= filter.From).AsQueryable();
                    }
                    if (filter.To != null)
                    {
                        query = query.Where(x => x.DateTime <= filter.To).AsQueryable();
                    }

                    if (filter.FromOfficer_Id == 0 && filter.ToOfficer_Id != 0)
                    {
                        query = query.Where(x => x.FromOfficer_Id == psOfficer.Id && x.ToOfficer_Id == filter.ToOfficer_Id).AsQueryable();
                    }
                    else if (filter.ToOfficer_Id == 0 && filter.FromOfficer_Id != 0)
                    {
                        query = query.Where(x => x.FromOfficer_Id == filter.FromOfficer_Id && x.ToOfficer_Id == psOfficer.Id).AsQueryable();
                    }
                    if (filter.MIDNumber != 0)
                    {
                        query = query.Where(x => x.MID_Number == filter.MIDNumber).AsQueryable();
                    }
                    var count = query.Count();
                    //var list = query.OrderByDescending(x => x.DateTime).Skip(filter.Skip).Take(filter.PageSize).ToList();
                    var listMasters = query.OrderByDescending(x => x.DateTime).ToList();
                    var masterIds = listMasters.Select(y => y.Id).ToList();
                    var listDetails = db.FileMoveDetailViews.Where(x => masterIds.Contains((int)x.Master_Id)).ToList();

                    return Ok(new { Count = count, List = listMasters, listDetails });
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

        }
        [Route("GetDiaries")]
        [HttpPost]
        public IHttpActionResult GetDiaries([FromBody] FilesACRsFilter filter)
        {
            using (var db = new HR_System())
            {
                try
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    string userId = User.Identity.GetUserId();
                    P_SOfficers psOfficer = db.P_SOfficers.FirstOrDefault(x => x.User_Id.Equals(userId));
                    var diary = db.uspOfficerDiary(psOfficer.Id).ToList();

                    var query = db.FileMoveMasterViews.Where(x => (x.FromOfficer_Id == psOfficer.Id || x.ToOfficer_Id == psOfficer.Id)).AsQueryable();

                    if (filter.From == null && filter.To == null)
                    {
                        filter.From = new DateTime(1970, 1, 1);
                        filter.To = DateTime.Now.AddDays(1);
                    }
                    else if (filter.From != null && filter.To == null)
                    {
                        filter.To = DateTime.Now.AddDays(1);
                    }
                    else if (filter.From == null && filter.To == null)
                    {
                        filter.From = new DateTime(1970, 1, 1);
                        filter.To = filter.To.Value.AddDays(1);
                    }
                    else if (filter.From != null && filter.To != null)
                    {
                        filter.To = filter.To.Value.AddDays(1);
                    }
                    if (filter.From != null)
                    {
                        query = query.Where(x => x.DateTime >= filter.From).AsQueryable();
                    }
                    if (filter.To != null)
                    {
                        query = query.Where(x => x.DateTime <= filter.To).AsQueryable();
                    }

                    var count = query.Count();
                    //var list = query.OrderByDescending(x => x.DateTime).Skip(filter.Skip).Take(filter.PageSize).ToList();
                    var list = query.OrderByDescending(x => x.DateTime).ToList();
                    return Ok(new { Count = count, List = list, diary });
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

        }

        [Route("GetDiary")]
        [HttpPost]
        public IHttpActionResult GetDiary([FromBody] FilesACRsFilter filter)
        {
            using (var db = new HR_System())
            {
                try
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    string userId = User.Identity.GetUserId();
                    P_SOfficers psOfficer = db.P_SOfficers.FirstOrDefault(x => x.User_Id.Equals(userId));

                    if (filter.From == null && filter.To == null)
                    {
                        filter.From = new DateTime(1970, 1, 1);
                        filter.To = DateTime.Now.AddDays(1);
                    }
                    else if (filter.From != null && filter.To == null)
                    {
                        filter.To = DateTime.Now.AddDays(1);
                    }
                    else if (filter.From == null && filter.To == null)
                    {
                        filter.From = new DateTime(1970, 1, 1);
                        filter.To = filter.To.Value.AddDays(1);
                    }
                    else if (filter.From != null && filter.To != null)
                    {
                        filter.To = filter.To.Value.AddDays(1);
                    }

                    var diary = db.uspOfficerDiary2(psOfficer.Id, filter.From, filter.To).ToList();
                    return Ok(diary);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        [Route("IssueReturnFile/{officer_Id}/{toStatusId}")]
        [HttpPost]
        public async Task<IHttpActionResult> IssueReturnFileAsync([FromBody] List<int> reqIds, int officer_Id, int toStatusId)
        {
            using (var db = new HR_System())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.Configuration.ProxyCreationEnabled = false;
                        var afrLog = new ApplicationFileRecositionLog();
                        P_SOfficers officer = null;
                        int lastSMSOfficerId = 0;
                        string userId = User.Identity.GetUserId();
                        var fileRequestStatuses = db.FileRequestStatus.ToList();
                        var applog = new ApplicationLog();
                        var trackings = new List<int?>();
                        string messageFrom = "From: Central Record Room\n";
                        string messageStatus = toStatusId == 11 ? "Message: File Available\n" : toStatusId == 2 ? "Message: File Issued\n" : toStatusId == 3 ? "Message: File Returned\n" : "";
                        string messageFileNumbers = "";

                        foreach (var reqId in reqIds)
                        {
                            ApplicationFileRecosition afr = db.ApplicationFileRecositions.FirstOrDefault(x => x.Id == reqId);

                            if (afr != null)
                            {
                                if (officer_Id > 0)
                                {
                                    officer = db.P_SOfficers.FirstOrDefault(x => x.Id == officer_Id);
                                }
                                else
                                {
                                    officer = db.P_SOfficers.FirstOrDefault(x => x.User_Id == afr.Users_Id);
                                }
                                if (afr.FileUpdated_Id != null && afr.FileUpdated_Id > 0)
                                {
                                    var fileUpdated = db.FilesUpdateds.FirstOrDefault(x => x.Id == afr.FileUpdated_Id);
                                    messageFileNumbers += "File: " + fileUpdated.FileNumber + "\n";
                                }
                                else if (afr.DDS_Id != null && afr.DDS_Id > 0)
                                {
                                    var ddsFile = db.DDS_Files.FirstOrDefault(x => x.Id == afr.DDS_Id);
                                    messageFileNumbers += "File: " + ddsFile.DiaryNo + "\n";
                                }
                                if (toStatusId == 2)
                                {
                                    // Issue File
                                    if (officer != null)
                                    {
                                        afr.RecievedBy = officer.Name;
                                        afr.RecievedByCNIC = officer.CNIC;
                                        afr.RecievedByContactNo = officer.Contact;
                                        afr.RecievedByDesig = officer.DesignationName;
                                        afr.RequestApproveDateTime = DateTime.UtcNow.AddHours(5);
                                        applog.Action_Id = 6;
                                    }
                                }
                                else if (toStatusId == 3)
                                {
                                    // Return File
                                    afr.ReturnBy = officer.Name;
                                    afr.ReturnByCNIC = officer.CNIC;
                                    afr.ReturnByContactNo = officer.Contact;
                                    afr.ReturnByDesig = officer.DesignationName;
                                    afr.ReturnDateTime = DateTime.UtcNow.AddHours(5);
                                    applog.Action_Id = 7;
                                }
                                else if (toStatusId == 11)
                                {
                                    applog.Action_Id = 8;
                                    if (officer != null && lastSMSOfficerId != officer.Id)
                                    {

                                        if (!string.IsNullOrEmpty(officer.Contact))
                                        {
                                            //sms.MobileNumber = officer.Contact;
                                            //sms.Message = messageFrom + messageStatus + (reqIds.Count < 4 ? messageFileNumbers : reqIds.Count + " Files");
                                            //await Common.SMS_Send(sms);
                                            SMS sms = new SMS()
                                            {
                                                UserId = userId,
                                                FKId = afr.Id,
                                                MobileNumber = officer.Contact,
                                                Message = messageFrom + messageStatus + (reqIds.Count < 4 ? messageFileNumbers : reqIds.Count + " Files")
                                            };
                                            Thread t = new Thread(() => Common.SendSMSTelenor(sms));
                                            t.Start();
                                            //await Common.SendSMSTelenor(sms);
                                            applog.SMS_SentToOfficer = true;
                                            lastSMSOfficerId = officer.Id;
                                        }
                                    }

                                }
                                afrLog.FromStatus_Id = afr.RequestStatus_Id;
                                afr.RequestStatus_Id = toStatusId;
                                db.Entry(afr).State = EntityState.Modified;
                                db.SaveChanges();

                                afrLog.AFR_Id = afr.Id;
                                afrLog.ToStatus_Id = afr.RequestStatus_Id;
                                afrLog.Officer_Id = officer.Id;
                                afrLog.User_Id = User.Identity.GetUserId();
                                afrLog.UserName = User.Identity.GetUserName();
                                afrLog.DateTime = DateTime.UtcNow.AddHours(5);
                                db.ApplicationFileRecositionLogs.Add(afrLog);
                                db.SaveChanges();

                                if (afr.Application_Id != null)
                                {
                                    var application = db.ApplicationMasters.FirstOrDefault(x => x.Id == afr.Application_Id);
                                    if (application != null)
                                    {
                                        applog.Application_Id = application.Id;
                                        applog.FileRequest_Id = afr.Id;
                                        applog.FileRequestLog_Id = afrLog.Id;
                                        applog.DateTime = afrLog.DateTime;
                                        applog.CreatedBy = User.Identity.GetUserName();
                                        applog.User_Id = userId;
                                        applog.IsActive = true;
                                        db.ApplicationLogs.Add(applog);
                                        db.SaveChanges();

                                        application.FileRequestStatus_Id = toStatusId;
                                        application.FileRequestStatus = fileRequestStatuses.FirstOrDefault(x => x.Id == toStatusId).ReuestStatus;

                                        if (application.FileRequestStatus_Id == 11)
                                        {
                                            var curLogs = db.ApplicationLogs.Where(x => x.Application_Id == application.Id && x.Action_Id == 5 && x.ToStatus_Id == 12).ToList();
                                            if (curLogs.Count > 0)
                                            {
                                                var curLog = curLogs.Last();
                                                var fromStatusCur = db.ApplicationStatus.FirstOrDefault(x => x.Id == curLog.FromStatus_Id);
                                                var fromStatus = db.ApplicationStatus.FirstOrDefault(x => x.Id == application.Status_Id);
                                                applog.FromStatus_Id = fromStatus.Id;
                                                applog.FromStatus = fromStatus.Name;

                                                if (fromStatusCur.Id == 10)
                                                {
                                                    application.Status_Id = 1;

                                                    applog.ToStatus_Id = 1;
                                                    applog.ToStatus = "Under Process";

                                                }
                                                else
                                                {
                                                    application.Status_Id = fromStatusCur.Id;

                                                    applog.ToStatus_Id = fromStatusCur.Id;
                                                    applog.ToStatus = fromStatusCur.Name;
                                                }
                                                application.StatusTime = applog.DateTime;
                                                application.StatusByOfficer_Id = officer.Id;
                                                application.StatusByOfficerName = officer.DesignationName;

                                                applog.StatusByOfficer_Id = officer.Id;
                                                applog.StatusByOfficer = officer.DesignationName;
                                            }

                                        }
                                        application.CurrentLog_Id = applog.Id;


                                        db.SaveChanges();
                                        trackings.Add(application.TrackingNumber);
                                    }
                                }
                            }
                        }
                        trans.Commit();
                        return Ok(new { res = true, trackings });
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return BadRequest(ex.Message);
                    }
                }
            }
        }

        [Route("GetApplicationFileReqLogs/{afrId}")]
        [HttpGet]
        public IHttpActionResult GetApplicationFileReqLog(int afrId)
        {
            using (var db = new HR_System())
            {
                try
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var afsLogs = db.ApplicationFileRecositionLogs.Where(x => x.AFR_Id == afrId).OrderBy(x => x.DateTime).ToList();
                    return Ok(afsLogs);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }
        [Route("SaveApplicationFileReqLog")]
        [HttpPost]
        public IHttpActionResult SaveApplicationFileReqLog([FromBody] ApplicationFileRecositionLog applicationFileRecositionLog)
        {
            using (var db = new HR_System())
            {
                try
                {
                    if (applicationFileRecositionLog.AFR_Id > 0 && !string.IsNullOrEmpty(applicationFileRecositionLog.Remarks))
                    {
                        applicationFileRecositionLog.User_Id = User.Identity.GetUserId();
                        applicationFileRecositionLog.UserName = User.Identity.GetUserName();
                        applicationFileRecositionLog.DateTime = DateTime.UtcNow.AddHours(5);
                        db.ApplicationFileRecositionLogs.Add(applicationFileRecositionLog);
                        db.SaveChanges();
                        return Ok(applicationFileRecositionLog);
                    }
                    return Ok(false);
                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }
        [Route("FPrintCompare")]
        [HttpPost]
        public IHttpActionResult FPrintCompare([FromBody] FPPrint fprint)
        {
            try
            {
                P_SOfficers officer = new FingerprintSdk().SearchOfficer(fprint.metaData);
                if (officer != null)
                {
                    return Ok(new { result = true, officer = officer });
                }
                else
                {
                    return Ok(new { result = false });
                }
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        static string LoadSubDirs(string dir)
        {

            string[] subdirectoryEntries = Directory.GetDirectories(dir);
            string Filedirtory = dir;
            if (subdirectoryEntries.Length > 0)
            {
                foreach (string subdirectory in subdirectoryEntries)
                {

                    LoadSubDirs(subdirectory);

                }
            }

            return Filedirtory;
        }

    }
}
