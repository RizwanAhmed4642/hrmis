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
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using DPUruNet;
using Zen.Barcode;

namespace Hrmis.Controllers.HrmisRestApi
{
    [Authorize]
    [RoutePrefix("api/Attandance")]

    public class AttandanceController : ApiController
    {
        private readonly AttandanceService _attandanceService;

        public AttandanceController()
        {
            _attandanceService = new AttandanceService();
        }

        //// getAttandanceLog
        //[HttpPost]
        //[Route("GetAttandanceLog")]
        //public IHttpActionResult GetAttandanceLog([FromBody] AttandanceService.AttandanceFilter filter)
        //{
        //    try
        //    {
        //        return Ok(_attandanceService.GetAttandanceLog(filter, User.Identity.GetUserName(), User.Identity.GetUserId()));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.InnerException.InnerException.InnerException?.Message ?? ex.InnerException.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message);
        //    }
        //}


        //  getHfTypesList
        [HttpPost]
        [Route("GetLeaveList")]
        public IHttpActionResult GetLeaveList([FromBody] AttandanceService.AttandanceFilter filter)
        {
            try
            {
                return Ok(_attandanceService.GetLeaveList(filter, User.Identity.GetUserName(), User.Identity.GetUserId()));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException.InnerException.InnerException?.Message ?? ex.InnerException.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message);
            }
        }

        //  getAttendanceList
        [HttpPost]
        [Route("getAttendanceList")]
        public IHttpActionResult getAttendanceList([FromBody] AttandanceService.AttandanceFilter filter)
        {
            try
            {
                return Ok(_attandanceService.getAttendanceList(filter, User.Identity.GetUserName(), User.Identity.GetUserId()));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException.InnerException.InnerException?.Message ?? ex.InnerException.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message);
            }
        }

        [Route("GetEmpStatus")]
        [HttpGet]
        public async Task<List<HISDU_EmpStatus>> GetEmpStatus()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var st = await db.HISDU_EmpStatus.ToListAsync();
                return st;
            }
        }

        [HttpPost]
        [Route("EditEmpStatus")]
        public IHttpActionResult EditEmpStatus([FromBody] UserLog ul)
        {
            try
            {
                var st = _attandanceService.EditEmpStatus1(ul, User.Identity.GetUserName(), User.Identity.GetUserId());
                if (st == null) return BadRequest("Invalid");
                return Ok(st);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                return BadRequest(ex.Message);
            }
        }


        //  getAttendanceList
        [HttpPost]
        [Route("getAttendanceListRpt")]
        public IHttpActionResult getAttendanceListRpt([FromBody] AttandanceService.AttandanceFilter filter)
        {
            try
            {
                return Ok(_attandanceService.getAttendanceListRpt(filter, User.Identity.GetUserName(), User.Identity.GetUserId()));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException.InnerException.InnerException?.Message ?? ex.InnerException.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message);
            }
        }

        //  getAttendanceRec
        [HttpPost]
        [Route("getAttendanceRec")]
        public IHttpActionResult getAttendanceRec([FromBody] AttandanceService.AttandanceFilter filter)
        {
            try
            {
                return Ok(_attandanceService.getAttendanceRec(filter, User.Identity.GetUserName(), User.Identity.GetUserId()));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException.InnerException.InnerException?.Message ?? ex.InnerException.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message);
            }
        }




        //  GetLeaveReport
        [HttpPost]
        [Route("GetLeaveReport")]
        public IHttpActionResult GetLeaveReport([FromBody] AttandanceService.AttandanceFilter filter)
        {
            try
            {
                return Ok(_attandanceService.GetLeaveReport(filter, User.Identity.GetUserName(), User.Identity.GetUserId()));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException.InnerException.InnerException?.Message ?? ex.InnerException.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPost]
        [Route("GetMonths")]
        public IHttpActionResult GetMonths([FromBody] AttandanceService.AttandanceFilter filter)
        {
            try
            {
                return Ok(_attandanceService.GetMonths(filter, User.Identity.GetUserName(), User.Identity.GetUserId()));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException.InnerException.InnerException?.Message ?? ex.InnerException.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message);
            }
        }


        //  GetLeaveRec
        [HttpPost]
        [Route("GetLeaveRec")]
        public IHttpActionResult GetLeaveRec([FromBody] AttandanceService.AttandanceFilter filter)
        {
            try
            {
                return Ok(_attandanceService.GetLeaveRec(filter, User.Identity.GetUserName(), User.Identity.GetUserId()));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException.InnerException.InnerException?.Message ?? ex.InnerException.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message);
            }
        }


        // GET: Attandance
        ///  public ActionResult Index()
        //   {
        //      return View();
        //  }

        [HttpGet]
        [Route("getTotalLeaves/{cnic}")]
        public IHttpActionResult getTotalLeaves(string cnic)
        {
            try
            {
                return Ok(_attandanceService.GetTotalLeaves(cnic));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("getTotalLeaves1/{PId}")]
        public IHttpActionResult getTotalLeaves1(int PId)
        {
            try
            {
                return Ok(_attandanceService.GetTotalLeaves1(PId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[HttpGet]
        //[Route("GetYears")]
        //public IHttpActionResult GetYears()
        //{
        //    try
        //    {
        //        return Ok(_attandanceService.GetYears());
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        ////  getHfTypesList
        //[HttpPost]
        //[Route("GetYears")]
        //public IHttpActionResult GetYear([FromBody] AttandanceService.AttandanceFilter filter)
        //{
        //    try
        //    {
        //        return Ok(_attandanceService.GetYears(filter, User.Identity.GetUserName(), User.Identity.GetUserId()));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.InnerException.InnerException.InnerException?.Message ?? ex.InnerException.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message);
        //    }
        //}
        [HttpPost]
        [Route("GetProfiles")]
        public IHttpActionResult GetProfiles(AttandanceService.AttandanceFilter filters)
        {
            try
            {
                //if (User.IsInRole("PHFMC Admin") || User.IsInRole("PHFMC"))
                //{
                //    filters.roleName = "PHFMC Admin";
                //}
                return Ok(_attandanceService.GetProfiles(filters));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("GetProfilesInActive")]
        public IHttpActionResult GetProfilesInActive(AttandanceService.AttandanceFilter filters)
        {
            try
            {
                return Ok(_attandanceService.GetProfilesInActive(filters));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("GetDepartments")]
        [HttpGet]
        [AllowAnonymous]
     //   [ResponseType(typeof(HrDepartment))]
        public async Task<List<HrDepartment>> GetDepartments()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var hrDepartment = await db.HrDepartments.Where(x => x.Id.Equals(25)).ToListAsync();
                return hrDepartment;
            }
        }

        [Route("GetSubDepartments")]
        [HttpGet]
        [AllowAnonymous]
        //   [ResponseType(typeof(HrDepartment))]
        public async Task<List<HISDU_Department>> GetSubDepartments()
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var hrSubDepartment = await db.HISDU_Department.OrderBy(x => x.SubDept_Name).ToListAsync();
                return hrSubDepartment;
            }
        }

        [HttpPost]
        [Route("SearchHealthFacilities")]
        public IHttpActionResult SearchHealthFacilities([FromBody] SearchQuery searchQuery)
        {
            try
            {
                return Ok(_attandanceService.SearchHealthFacilities(searchQuery.Query, User.Identity.GetUserId()));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetProfile/{cnic}")]
        public IHttpActionResult GetProfile(string cnic)
        {
            try
            {
                return Ok(_attandanceService.GetProfile(cnic));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveProfile")]
        public IHttpActionResult SaveProfile([FromBody] HrProfile hrProfile)
        {
            try
            {
                var profile = _attandanceService.AddProfile(hrProfile, User.Identity.GetUserName(), User.Identity.GetUserId());
                if (profile == null) return BadRequest("Invalid");
                return Ok(profile);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("saveEmpAttendance")]
        public IHttpActionResult saveEmpAttendance([FromBody] Empattendance empAtt)
        {
            try
            {
                var empAt = _attandanceService.AddEmpAttendance(empAtt, User.Identity.GetUserName(), User.Identity.GetUserId());
                if (empAt == null) return BadRequest("Invalid");
                return Ok(empAt);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }


        [Route("UploadProfilePhoto/{id}")]
        public async Task<IHttpActionResult> FileUpload(int id)
        {
            try
            {
                using (var db = new HR_System())
                {
                    if (!Request.Content.IsMimeMultipartContent())
                        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                    string RootPath = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\ProfilePhotos\";
                    var dirPath = RootPath;

                    var provider = new MultipartMemoryStreamProvider();
                    await Request.Content.ReadAsMultipartAsync(provider);
                    var profile = db.HrProfiles.FirstOrDefault(x => x.Id == id);
                    if (profile == null) return Ok(new { result = false });

                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }

                    string filename = "";

                    foreach (var file in provider.Contents)
                    {
                        filename = profile.CNIC + "_23.jpg";

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

                        using (FileStream fsOut = File.OpenWrite(RootPath + @"\" + filename))
                        {
                            fsOut.Write(buffer, 0, buffer.Length);
                        }

                        profile.ProfilePhoto = filename;

                        if (profile.EntityLifecycle_Id == null)
                        {
                            Entity_Lifecycle elc = new Entity_Lifecycle();
                            elc.Created_Date = DateTime.UtcNow.AddHours(5);
                            elc.Created_By = User.Identity.GetUserName() + " (added after migration)";
                            elc.Users_Id = User.Identity.GetUserId();
                            elc.IsActive = true;
                            elc.Entity_Id = 9;
                            db.Entity_Lifecycle.Add(elc);
                            db.SaveChanges();
                            profile.EntityLifecycle_Id = elc.Id;
                        }
                        Entity_Modified_Log eml = new Entity_Modified_Log();
                        eml.Modified_By = User.Identity.GetUserId();
                        eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                        eml.Entity_Lifecycle_Id = (long)profile.EntityLifecycle_Id;
                        eml.Description = "Profile Photo Uploaded By " + User.Identity.GetUserName();
                        db.Entity_Modified_Log.Add(eml);
                        db.SaveChanges();

                        db.Entry(profile).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                    }
                    return Ok(new { result = true, src = filename });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Route("UploadProfileAttachments/{profile_Id}/{profileRemarks_Id}")]
        public async Task<bool> UploadProfileAttachments(int profile_Id, int profileRemarks_Id)
        {
            try
            {
                using (var _db = new HR_System())
                {

                    if (!Request.Content.IsMimeMultipartContent())
                        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                    string RootPath = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\Files\ProfileAttachments\";
                    var dirPath = RootPath;

                    var provider = new MultipartMemoryStreamProvider();
                    await Request.Content.ReadAsMultipartAsync(provider);

                    CreateDirectoryIfNotExists(dirPath);
                    string filename = "";

                    foreach (var file in provider.Contents)
                    {
                        string key = file.Headers.ContentDisposition.Name;
                        //if (!key.StartsWith(""\"file")) continue;
                        var FileExtension = Path.GetExtension(file.Headers.ContentDisposition.FileName.Trim('\"')).Substring(1);
                        filename = Guid.NewGuid().ToString() + "_" + profile_Id + "." + FileExtension;


                        var profileRemarks = _db.ProfileRemarks.FirstOrDefault(x => x.Id == profileRemarks_Id);
                        if (profileRemarks != null)
                        {
                            profileRemarks.FilePath = @"Uploads\Files\ProfileAttachments\" + filename;
                            _db.Entry(profileRemarks).State = EntityState.Modified;
                            _db.SaveChanges();
                        }

                        var buffer = await file.ReadAsByteArrayAsync();
                        var size = ((buffer.Length) / (1024)) / (1024);
                        var ext = Path.GetExtension(filename.Replace("\"", string.Empty));
                        List<string> validExtensions = new List<string>() { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".docx" };
                        if (!validExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase) || size > 15)
                        {
                            throw new Exception("Unable to Upload. File Size must be less than 5 MB and File Format must be " + string.Join(",", validExtensions));
                        }
                        using (FileStream fsOut = File.OpenWrite(RootPath + @"\" + filename))
                        {
                            fsOut.Write(buffer, 0, buffer.Length);
                        }
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return false;
            }
        }
        private void CreateDirectoryIfNotExists(string dirPath) { if (!Directory.Exists(dirPath)) { Directory.CreateDirectory(dirPath); } }

        [HttpPost]
        [Route("SaveLeave")]
        public IHttpActionResult SaveLeave([FromBody] EmpLeaveForm leave)
        {
            try
            {
                var lev = _attandanceService.saveLeaveRec(leave, User.Identity.GetUserName(), User.Identity.GetUserId());
                if (lev == null) return BadRequest("Invalid");
                return Ok(lev);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                return BadRequest(ex.Message);
            }
        }

    }
}