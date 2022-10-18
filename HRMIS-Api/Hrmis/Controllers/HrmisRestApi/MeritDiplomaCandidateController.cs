using Hrmis.Models.Common;
using Hrmis.Models.DbModel;
using Hrmis.Models.Services;
using Hrmis.Models.ViewModels.Application;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Hrmis.Controllers.HrmisRestApi
{
    [RoutePrefix("api/MeritDiploma")]
    public class MeritDiplomaCandidateController : ApiController
    {
        private HR_System db = new HR_System();
        public readonly ProfileService _profileService;
        private readonly MeritDiplomaService _meritdiplomaSercive;
        public MeritDiplomaCandidateController()
        {
            _profileService = new ProfileService();
            _meritdiplomaSercive = new MeritDiplomaService();
            db.Configuration.ProxyCreationEnabled = false;
        }
        [HttpPost]
        [Route("SaveMeritDiplomaCandidate")]
        public IHttpActionResult SaveMeritDiplomaCandidate([FromBody]  MeritDiplomaCandidateViewModel obj)
        {
            try
            {
                var candidate = _meritdiplomaSercive.AddMeritCandidate(obj, User.Identity.GetUserName(), User.Identity.GetUserId());
                if (candidate == null) return BadRequest("Invalid");
                return Ok(candidate);
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [Route("GetMerit/{CNIC}")]
        [HttpGet]
        public IHttpActionResult GetMerit(string CNIC)
        {
            try
            {
                var merit = db.MeritsViews.Where(x => x.CNIC == CNIC).FirstOrDefault();

                return Ok(merit);
            }
            catch (Exception ex)
            {
                return Ok(false);
            }   
        }
        [HttpGet]
        [Route("GetProfile/{cnic}")]
        public IHttpActionResult GetProfile(string cnic)
        {
            try
            {
                var profile = _profileService.GetProfile(cnic);
                return Ok(profile);
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [Route("GetHFListsPreference")]
        [HttpGet]
        public IHttpActionResult GetHFListsPreference()
        {
            try
            {
                var hfs = db.HFListPs.Where(x => x.IsActive == true && x.HFAC == 4).ToList();

                return Ok(hfs);
            }
            catch (Exception ex)
            {
                return Ok(false);
            }
        }
        [Route("GetHFLists")]
        [HttpGet]
        public IHttpActionResult GetHFLists()
        {
            try
            {
                var hfs = db.HFListPs.Where(x => x.IsActive == true ).ToList();

                return Ok(hfs);
            }
            catch (Exception ex)
            {
                return Ok(false);
            }
        }
        [Route("GetDistrict")]
        [HttpGet]
        public IHttpActionResult GetDistrict()
        {
            try
            {
                var hfs = db.Districts.ToList();

                return Ok(hfs);
            }
            catch (Exception ex)
            {
                return Ok(false);
            }
        }
        [Route("UploadDocumentPhoto/{cnic}")]
        public async Task<IHttpActionResult> FileUpload(string cnic)
        {
            try
            {
                using (var db = new HR_System())
                {
                    if (!Request.Content.IsMimeMultipartContent())
                        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                    string RootPath = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\MeritPhotos\";
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
                        filename = cnic + "_candidate.jpg";

                        var buffer = await file.ReadAsByteArrayAsync();
                        var size = ((buffer.Length) / (1024)) / (1024);
                        var ext = Path.GetExtension(filename.Replace("\"", string.Empty));
                        List<string> validExtensions = new List<string>() { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                        if (!validExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase) || size > 5)
                        {
                            throw new Exception(
                                "Unable to Upload. File Size must be less than 5 MB and File Format must be " +
                                string.Join(",", validExtensions));
                        }
                        var candidate = db.MeritDiplomaCandidates.FirstOrDefault(x => x.CNIC == cnic);
                        candidate.UploadPath = filename;
                        db.SaveChanges();

                        using (FileStream fsOut = File.OpenWrite(RootPath + @"\" + filename))
                        {
                            fsOut.Write(buffer, 0, buffer.Length);
                        }



                    }
                    return Ok(new { result = true, src = filename });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
