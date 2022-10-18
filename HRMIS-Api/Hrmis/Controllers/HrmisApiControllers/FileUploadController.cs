using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Hrmis.Models.Dto;
using Hrmis.Models.Services;
using Microsoft.AspNet.Identity;

namespace Hrmis.Controllers.HrmisApiControllers
{
    [Authorize]
    [RoutePrefix("api/File")]
    public class FileUploadController : BaseApiController
    {

        private readonly FileService _fileService;

        public FileUploadController()
        {
            _fileService = new FileService(HttpContext.Current.Server.MapPath("~/") + @"Uploads\Files\",
                HttpContext.Current.Server.MapPath("~/") + @"Uploads\Files\");
        }

        [Route("Upload/{cnic}")]
        public async Task<IHttpActionResult> FileUpload(string cnic)
        {
            try
            {
                
                if (!Request.Content.IsMimeMultipartContent())
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                if (string.IsNullOrWhiteSpace(cnic)) { return BadRequest(); }
                _fileService.User = UserManager.FindById(User.Identity.GetUserId());
                _fileService.Cnic = cnic.Replace("-","");

                await _fileService.Save(Request);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        

        [Route("View/{cnic}")]
        [HttpGet]
        public IHttpActionResult FileView(string cnic)
        {
            try
            {
                string[] filters = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                _fileService.User = UserManager.FindById(User.Identity.GetUserId());
                _fileService.Cnic = cnic.Replace("-", "");

                return Ok(_fileService.GetFiles(filters,Url.Request));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("FileRemove/{cnic}")]
        [HttpPost]
        public IHttpActionResult FileRemove(string cnic, [FromBody] List<FileDto> files)
        {
            try
            {
                if (string.IsNullOrEmpty(cnic)) return BadRequest();
                _fileService.User = UserManager.FindById(User.Identity.GetUserId());

                _fileService.Cnic = cnic.Replace("-", "");

                _fileService.RemoveFiles(files);
                return Ok("");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("DownloadFile/{cnic}/{fileName}/")]
        public HttpResponseMessage DownloadFile(string cnic, string fileName)
        {
            HttpResponseMessage result;
            try
            {
                if (string.IsNullOrEmpty(cnic)) return Request.CreateResponse(HttpStatusCode.BadRequest,"CNIC should not be empty");
                _fileService.Cnic = cnic.Replace("-", "");
                _fileService.User = UserManager.FindById(User.Identity.GetUserId());

                result = Request.CreateResponse(HttpStatusCode.OK);
                result.Content = new ByteArrayContent(_fileService.DownloadFile(fileName));
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = fileName
                };
                return result;
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.Gone,ex.Message);
            }
        }

        [HttpPost]
        [Route("DownloadFiles/{cnic}")]
        public IHttpActionResult DownloadFiles(string cnic, [FromBody] List<FileDto> files)
        {
           
            try
            {
                if (string.IsNullOrEmpty(cnic)) return BadRequest();
                _fileService.Cnic = cnic.Replace("-", "");
                _fileService.User = UserManager.FindById(User.Identity.GetUserId());

                _fileService.DownloadFiles(files);
                return Ok(cnic);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("Download/Zip/{cnic}")]
        public HttpResponseMessage DownloadZipFile(string cnic)
        {
            HttpResponseMessage result;
            try
            {

                if (string.IsNullOrEmpty(cnic)) return Request.CreateResponse(HttpStatusCode.BadRequest, "CNIC should not be empty");
                _fileService.Cnic = cnic.Replace("-", "");

                FileInfo fileInfo = _fileService.DownloadZipFile(cnic);
                byte[] bytes = File.ReadAllBytes(fileInfo.FullName);
                result = Request.CreateResponse(HttpStatusCode.OK);
                result.Content = new ByteArrayContent(bytes);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = fileInfo.Name
                };
                return result;
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.Gone,ex.Message);
            }
        }
    }
}
