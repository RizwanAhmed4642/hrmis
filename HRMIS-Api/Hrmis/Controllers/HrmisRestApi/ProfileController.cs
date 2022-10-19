using Hrmis.Models;
using Hrmis.Models.Common;
using Hrmis.Models.DbModel;
using Hrmis.Models.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;

namespace Hrmis.Controllers.HrmisRestApi
{
    [Authorize]
    [RoutePrefix("api/Profile")]
    public class ProfileController : ApiController
    {
        public readonly ProfileService _profileService;
        public readonly UserLogsSErvice _userLogsSErvice;       

        public ProfileController()
        {
            _profileService = new ProfileService();
            _userLogsSErvice = new UserLogsSErvice();   

        }

        [HttpPost]
        [Route("SaveLogs")]
        public IHttpActionResult SaveLogs([FromBody] LogClass obj)
        
        
        {
            try
            {
                string id = User.Identity.GetUserId();
                
                _userLogsSErvice.SaveProfileLoggedInInfo(id, "GetProfileSearch", obj);
                
                return Ok();
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveProfile")]
        public IHttpActionResult SaveProfile([FromBody] HrProfile hrProfile)
        {
            try
            {
                var profile = _profileService.AddProfile(hrProfile, User.Identity.GetUserName(), User.Identity.GetUserId());
                if (profile == null) return BadRequest("Invalid");
                return Ok(profile);
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("SaveHealthWorker")]
        public IHttpActionResult SaveHealthWorker([FromBody] HrHealthWorker hrHealthWorker)
        {
            try
            {
                var profile = _profileService.SaveHealthWorker(hrHealthWorker, User.Identity.GetUserName(), User.Identity.GetUserId());
                if (profile == null) return BadRequest("Invalid");
                return Ok(profile);
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("SaveFocalPerson")]
        public IHttpActionResult SaveFocalPerson([FromBody] HrFocalPerson hrFocalPerson)
        {
            try
            {
                var profile = _profileService.SaveFocalPerson(hrFocalPerson, User.Identity.GetUserName(), User.Identity.GetUserId());
                if (profile == null) return BadRequest("Invalid");
                return Ok(profile);
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("RemoveFocalPerson/{id}")]
        public IHttpActionResult RemoveFocalPerson(int id)
        {
            try
            {
                var res = _profileService.RemoveFocalPerson(id, User.Identity.GetUserName(), User.Identity.GetUserId());
                return Ok(res);
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("SaveShortProfile")]
        public IHttpActionResult SaveShortProfile([FromBody] HrProfile hrProfile)
        {
            try
            {
                var profile = _profileService.SaveShortProfile(hrProfile, User.Identity.GetUserName(), User.Identity.GetUserId());
                if (profile == null) return BadRequest("Invalid");
                return Ok(profile);
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetHealthWorker/{cnic}")]
        public IHttpActionResult GetHealthWorker(string cnic)
        {
            try
            {
                var res = _profileService.GetHealthWorker(cnic);
                var uxer = User.Identity.GetUserName();
                

                if (res == null) return BadRequest("Invalid");
                return Ok(res);
            }
            catch (Exception ex)
            {

                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetSurgeries/{code}/{filter}")]
        public IHttpActionResult GetSurgeries(string code, string filter)
        {
            try
            {
                var res = _profileService.GetSurgeries(code, filter);
                return Ok(res);
            }
            catch (Exception ex)
            {

                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("ShiftProfileToPSH/{profileId}")]
        public IHttpActionResult ShiftProfileToPSH(int profileId)
        {
            try
            {
                var res = _profileService.ShiftProfileToPSH(profileId, User.Identity.GetUserName(), User.Identity.GetUserId());
                return Ok(res);
            }
            catch (Exception ex)
            {

                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveServiceHistory")]
        public IHttpActionResult SaveServiceHistory([FromBody] HrServiceHistory hrServiceHistory)
        {
            try
            {
                var res = _profileService.SaveServiceHistory(hrServiceHistory, User.Identity.GetUserName(), User.Identity.GetUserId());
                if (res == null) return BadRequest("Invalid");
                return Ok(res);
            }
            catch (Exception ex)
            {

                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetServiceHistory/{profileId}")]
        public IHttpActionResult GetServiceHistory(int profileId)
        {
            try
            {
                var res = _profileService.GetServiceHistory(profileId, User.Identity.GetUserName(), User.Identity.GetUserId());
                if (res == null) return BadRequest("Invalid");
                return Ok(res);
            }
            catch (Exception ex)
            {

                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("SaveHrQualification")]
        public IHttpActionResult SaveHrQualification([FromBody] HrQualification hrQualification)
        {
            try
            {
                var res = _profileService.SaveHrQualification(hrQualification, User.Identity.GetUserName(), User.Identity.GetUserId());
                if (res == null) return BadRequest("Invalid");
                return Ok(res);
            }
            catch (Exception ex)
            {

                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetHrQualification/{profileId}")]
        public IHttpActionResult GetHrQualification(int profileId)
        {
            try
            {
                var res = _profileService.GetHrQualification(profileId, User.Identity.GetUserName(), User.Identity.GetUserId());
                if (res == null) return BadRequest("Invalid");
                return Ok(res);
            }
            catch (Exception ex)
            {

                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("RemoveHrQualification/{Id}")]
        public IHttpActionResult RemoveHrQualification(int Id)
        {
            try
            {
                var res = _profileService.RemoveHrQualification(Id, User.Identity.GetUserName(), User.Identity.GetUserId());
                return Ok(res);
            }
            catch (Exception ex)
            {

                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        // Controller to get files master..
        [HttpPost]
        [Route("GetFileMaster")]
        public IHttpActionResult GetFileMaster([FromBody] FilesACRsFilter filters)
        {
            try
            {
                var res = _profileService.GetFilesMaster(filters, User.Identity.GetUserName(), User.Identity.GetUserId());
                if (res == null) return BadRequest("Invalid");
                return Ok(res);
            }
            catch (Exception ex)
            {

                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex);
                return BadRequest(ex.Message);
            }
        }


        //[HttpGet]
        //[Route("GetFilesList/{profileId}")]
        //public async Task<IHttpActionResult> GetFilesList(int ProfileId)
        //{
        //    try
        //    {
        //        using (var _db = new HR_System())
        //        {
        //            var res = _db.FilesMasters.Where(s => s.IsActive == true).ToList();
        //            if (res == null || res.Count < 1)
        //            {
        //                res = new List<FilesMaster>();
        //            }
        //            return Ok(res);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        while (ex.InnerException != null) { ex = ex.InnerException; }
        //        return BadRequest(ex.Message);
        //    }
        //}

        [HttpGet]
        [Route("LockServiceHistory/{profileId}")]
        public IHttpActionResult LockServiceHistory(int profileId)
        {
            try
            {
                var res = _profileService.LockServiceHistory(profileId, User.Identity.GetUserName(), User.Identity.GetUserId());
                return Ok(res);
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("RemoveServiceHistory/{Id}")]
        public IHttpActionResult RemoveServiceHistory(int Id)
        {
            try
            {
                var res = _profileService.RemoveServiceHistory(Id, User.Identity.GetUserName(), User.Identity.GetUserId());
                return Ok(res);
            }
            catch (Exception ex)
            {

                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        [Route("SaveAttachedPerson")]
        public IHttpActionResult SaveAttachedPerson([FromBody] HrAttachedPerson hrAttachedPerson)
        {
            try
            {
                var res = _profileService.SaveAttachedPerson(hrAttachedPerson, User.Identity.GetUserName(), User.Identity.GetUserId());
                if (res == null) return BadRequest("Invalid");
                return Ok(res);
            }
            catch (Exception ex)
            {

                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetAttachedPerson/{profileId}")]
        public IHttpActionResult GetAttachedPerson(int profileId)
        {
            try
            {
                var res = _profileService.GetAttachedPerson(profileId, User.Identity.GetUserName(), User.Identity.GetUserId());
                if (res == null) return BadRequest("Invalid");
                return Ok(res);
            }
            catch (Exception ex)
            {

                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("GetAttachedPersons")]
        public IHttpActionResult GetAttachedPersons([FromBody] MapFilters mapFilters)
        {
            try
            {
                //var res = _profileService.GetAttachedPersons(mapFilters); 
                var res = _profileService.GetAnaesthesiaTree(mapFilters);
                return Ok(res);
            }
            catch (Exception ex)
            {

                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("GetEmployeePersons")]
        public IHttpActionResult GetEmployeePersons([FromBody] MapFilters mapFilters)
        {
            try
            {
                //var res = _profileService.GetAttachedPersons(mapFilters); 
                var res = _profileService.GetEmployeePersons(mapFilters);
                return Ok(res);
            }
            catch (Exception ex)
            {

                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("GetAnesthesiaPersons")]
        public IHttpActionResult GetAnesthesiaPersons([FromBody] MapFilters mapFilters)
        {
            try
            {
                var res = _profileService.GetAnesthesiaPersons(mapFilters);
                return Ok(res);
            }
            catch (Exception ex)
            {

                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("RemoveAttachedPerson/{Id}")]
        public IHttpActionResult RemoveAttachedPerson(int Id)
        {
            try
            {
                var res = _profileService.RemoveAttachedPerson(Id, User.Identity.GetUserName(), User.Identity.GetUserId());
                return Ok(res);
            }
            catch (Exception ex)
            {

                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetSeniority")]
        public IHttpActionResult GetSeniority([FromBody] ProfileFilters filters)
        {
            try
            {
                var res = _profileService.GetSeniority(filters);
                return Ok(res);
            }
            catch (Exception ex)
            {

                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetDuplication/{cnic}")]
        public IHttpActionResult GetDuplication(string cnic)
        {
            try
            {
                return Ok(_profileService.GetDuplication(cnic));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        [Route("DeleteDuplication/{id}/{cnic}")]
        public IHttpActionResult DeleteDuplication(int id, string cnic)
        {
            try
            {
                if (id != null)
                {
                    return Ok(_profileService.DeleteDuplication(id, cnic, User.Identity.GetUserName(), User.Identity.GetUserId()));
                }
                else
                {
                    return Ok("Id Not Found");
                }


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveLeaveRecord")]
        public IHttpActionResult SaveLeaveRecord([FromBody] HrLeaveRecord hrLeaveRecord)
        {
            try
            {
                var res = _profileService.SaveLeaveRecord(hrLeaveRecord, User.Identity.GetUserName(), User.Identity.GetUserId());
                if (res == null) return BadRequest("Invalid");
                return Ok(res);
            }
            catch (Exception ex)
            {

                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetLeaveRecord/{profileId}")]
        public IHttpActionResult GetLeaveRecord(int profileId)
        {
            try
            {
                var res = _profileService.GetLeaveRecord(profileId, User.Identity.GetUserName(), User.Identity.GetUserId());
                if (res == null) return BadRequest("Invalid");
                return Ok(res);
            }
            catch (Exception ex)
            {

                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        [Route("SubmitForReview")]
        public IHttpActionResult SubmitForReview([FromBody] HrReviewSubmission reviewSubmission)
        {
            try
            {
                var res = _profileService.SubmitForReview(reviewSubmission, User.Identity.GetUserName(), User.Identity.GetUserId());
                if (res == null) return BadRequest("Invalid");
                return Ok(res);
            }
            catch (Exception ex)
            {

                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("SubmitReview")]
        public IHttpActionResult SubmitReview([FromBody] HrReview review)
        {
            try
            {
                var res = _profileService.SubmitReview(review, User.Identity.GetUserName(), User.Identity.GetUserId());
                if (res == null) return BadRequest("Invalid");
                return Ok(res);
            }
            catch (Exception ex)
            {

                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveHrComplain")]
        public IHttpActionResult SaveHrComplain([FromBody] HrComplain hrHrComplain)
        {
            try
            {
                var res = _profileService.SaveHrComplain(hrHrComplain, User.Identity.GetUserName(), User.Identity.GetUserId());
                if (res == null) return BadRequest("Invalid");
                return Ok(res);
            }
            catch (Exception ex)
            {

                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetHrReviews/{reviewSubmissionId}")]
        public IHttpActionResult GetHrReviews(int reviewSubmissionId)
        {
            try
            {
                var res = _profileService.GetHrReviews(reviewSubmissionId, User.Identity.GetUserName(), User.Identity.GetUserId());
                if (res == null) return BadRequest("Invalid");
                return Ok(res);
            }
            catch (Exception ex)
            {

                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetHrComplain")]
        public IHttpActionResult GetHrComplain()
        {
            try
            {
                var res = _profileService.GetHrCompalins(User.Identity.GetUserName(), User.Identity.GetUserId());
                if (res == null) return BadRequest("Invalid");
                return Ok(res);
            }
            catch (Exception ex)
            {

                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        //[HttpGet]
        //[Route("RemoveHrComplain/{Id}")]
        //public IHttpActionResult RemoveHrComplain(int Id)
        //{
        //    try
        //    {
        //        var res = _profileService.RemoveHrComplain(Id, User.Identity.GetUserName(), User.Identity.GetUserId());
        //        return Ok(res);
        //    }
        //    catch (Exception ex)
        //    {
        //        
        //        Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
        //    }
        //}

        [HttpPost]
        [Route("GetProfileServiceDetail")]
        public IHttpActionResult GetServiceDetails([FromBody] ProfileFilters filter)
        {
            try
            {
                var details = _profileService.GetServiceDetails(filter, User.Identity.GetUserName(), User.Identity.GetUserId());
                if (details == null) return Ok("Invalid");
                return Ok(details);
            }
            catch (Exception ex)
            {

                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("RemoveLeaveRecord/{Id}")]
        public IHttpActionResult RemoveLeaveRecord(int Id)
        {
            try
            {
                var res = _profileService.RemoveLeaveRecord(Id, User.Identity.GetUserName(), User.Identity.GetUserId());
                return Ok(res);
            }
            catch (Exception ex)
            {

                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("SaveInquiry")]
        public IHttpActionResult SaveInquiry([FromBody] InquiryDtoModel inquiryModel)
        {
            try
            {
                if (inquiryModel.hrInquiry.Profile_Id == 0) { return BadRequest(); }
                var result = _profileService.AddProfileInquiry(inquiryModel, User.Identity.GetUserName(), User.Identity.GetUserId());
                return Ok(result);
            }
            catch (Exception ex)
            {

                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }

        [Route("FPrintRegister/{profile_Id}")]
        [HttpPost]
        public IHttpActionResult FPrintRegister([FromBody] List<FPPrint> fprints, int profile_Id)
        {

            try
            {
                if (profile_Id == 0) { return BadRequest(); }
                var result = _profileService.FPrintRegister(fprints, profile_Id, User.Identity.GetUserId());
                return Ok(result);
            }
            catch (Exception ex)
            {

                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }


        }
        [Route("FPrintCompare/{profile_Id}/{typeInOut}")]
        [HttpPost]
        public IHttpActionResult FPrintCompare([FromBody] FPPrint fprint, int profile_Id, int typeInOut)
        {

            try
            {
                if (profile_Id == 0) { return BadRequest(); }
                var result = new FingerprintSdk().SearchProfile(fprint.metaData, profile_Id, typeInOut);
                return Ok(result);
            }
            catch (Exception ex)
            {

                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
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
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetProfile/{cnic}")]
        public IHttpActionResult GetProfile(string cnic)
        {
            try
            {
                return Ok(_profileService.GetProfile(cnic));
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetProfileInquiries/{profileId}")]
        public IHttpActionResult GetProfileInquiries(int profileId)
        {
            try
            {
                var inquiries = _profileService.GetProfileInquiries(profileId);
                var inquiryPenalties = _profileService.GetInquiryPenalties(inquiries);
                return Ok(new { inquiries, inquiryPenalties });
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetVaccinationCertificate/{profileId}")]
        public IHttpActionResult GetVaccinationCertificate(int profileId)
        {
            try
            {
                var certificate = _profileService.GetVaccinationCertificate(profileId);
                return Ok(certificate);
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("GetProfiles")]
        public IHttpActionResult GetProfiles(ProfileFilters filters)
        {
            try
            {
                filters.UserId = User.Identity.GetUserId();
                filters.UserName = User.Identity.GetUserName();
                if (User.IsInRole("PHFMC Admin") || User.IsInRole("PHFMC"))
                {
                    filters.roleName = "PHFMC Admin";
                }
                if (User.IsInRole("PACP"))
                {
                    filters.roleName = "PACP";
                }
                if (User.IsInRole("South Punjab"))
                {
                    filters.roleName = "South Punjab";
                }
                return Ok(_profileService.GetProfiles(filters));
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("GetSeniorityList")]
        public IHttpActionResult GetSeniorityList(ProfileFilters filters)
        {
            try
            {
                return Ok(_profileService.GetSeniorityList(filters));
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("GetSeniorityListFixed")]
        public IHttpActionResult GetSeniorityListFixed(ProfileFilters filters)
        {
            try
            {
                return Ok(_profileService.GetSeniorityListFixed(filters));
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetSeniorityApplicant/{cnic}")]
        public IHttpActionResult GetSeniorityApplicant(string cnic)
        {
            try
            {
                return Ok(_profileService.GetSeniorityApplicant(cnic));
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("GenerateSeniorityList/{categoryId}/{pass1}/{pass2}")]
        public IHttpActionResult GenerateSeniorityList(int categoryId, int pass1, int pass2)
        {
            try
            {
                if(pass1 == 2002 && pass2 == 2029)
                {
                    return Ok(_profileService.GenerateSeniorityList(categoryId));
                }
                return Ok(false);
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("GetNewSeniorityList")]
        public IHttpActionResult GetNewSeniorityList(ProfileFilters filters)
        {
            try
            {
                return Ok(_profileService.GetNewSeniorityList(filters));
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("VerifyProfileForPromotion/{profileId}")]
        public IHttpActionResult VerifyProfileForPromotion(int profileId)
        {
            try
            {
                return Ok(_profileService.VerifyProfileForPromotion(profileId, User.Identity.GetUserName(), User.Identity.GetUserId()));
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("SendSMSToEmployee")]
        public IHttpActionResult SendSMSToEmployee([FromBody] HrSMSEmployee hrSMSEmployee)
        {
            try
            {
                return Ok(_profileService.SendSMSToEmployee(hrSMSEmployee, User.Identity.GetUserName(), User.Identity.GetUserId()));
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("GetProfileReviews")]
        public IHttpActionResult GetProfileReviews(ProfileFilters filters)
        {
            try
            {
                if (User.IsInRole("PHFMC Admin") || User.IsInRole("PHFMC"))
                {
                    filters.roleName = "PHFMC Admin";
                }
                return Ok(_profileService.GetProfileReviews(filters));
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetProfileReview/{profileId}")]
        public IHttpActionResult GetProfileReview(int profileId)
        {
            try
            {

                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var profileReview = _db.HrReviewSubmissionViews.FirstOrDefault(x => x.ProfileId == profileId && x.IsActive == true);
                    return Ok(profileReview);
                }
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("GetProfilesMobile")]
        public IHttpActionResult GetProfilesMobile(ProfileFilters filters)
        {
            try
            {
                if (User.IsInRole("PHFMC Admin") || User.IsInRole("PHFMC"))
                {
                    filters.roleName = "PHFMC Admin";
                }
                return Ok(_profileService.GetProfiles(filters));
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("GetProfilesInPool")]
        public IHttpActionResult GetProfilesInPool(ProfileFilters filters)
        {
            try
            {
                return Ok(_profileService.GetProfilesInPool(filters));
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("PostProfileRemarks")]
        public IHttpActionResult PostProfileRemarks(ProfileRemark profileRemark)
        {
            try
            {
                return Ok(_profileService.PostProfileRemarks(profileRemark, User.Identity.GetUserName(), User.Identity.GetUserId()));
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetProfileRemarks/{profileId}")]
        public IHttpActionResult GetProfileRemarks(int profileId)
        {
            try
            {
                return Ok(_profileService.GetProfileRemarks(profileId));
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetProfileLogs/{profileId}")]
        public IHttpActionResult GetProfileLogs(int profileId)
        {
            try
            {
                return Ok(_profileService.GetProfileLogs(profileId));
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("RemoveProfileRemarks/{id}")]
        public IHttpActionResult RemoveProfileRemarks(int id)
        {
            try
            {
                return Ok(_profileService.RemoveProfileRemarks(id, User.Identity.GetUserName(), User.Identity.GetUserId()));
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
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
                        filename = Guid.NewGuid().ToString() + "-" + profile_Id + "." + FileExtension;


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

        [Route("UploadVaccinationCertificate/{profile_Id}/{certificateNumber}/{typeId}")]
        public async Task<IHttpActionResult> UploadVaccinationCertificate(int profile_Id, string certificateNumber, int typeId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    //check file already exist
                    //var attachments = _db.ProfileAttachmentsViews.Where(x => x.Profile_Id == profile_Id && x.IsActive == true);
                    //if (attachments.Count() > 0)
                    //{
                    //    foreach (var attachment in attachments)
                    //    {
                    //        var elc = _db.Entity_Lifecycle.FirstOrDefault(x => x.Id == attachment.EntityLifecycle_Id);
                    //        if (elc != null)
                    //        {
                    //            elc.IsActive = false;
                    //        }
                    //    }
                    //}
                    //_db.SaveChanges();
                    if (!Request.Content.IsMimeMultipartContent())
                        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                    string RootPath = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\Files\VaccinationCertificates\";
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
                        filename = Guid.NewGuid().ToString() + "-" + profile_Id + "." + FileExtension;


                        var profileAttachment = new ProfileAttachment();
                        profileAttachment.Profile_Id = profile_Id;
                        profileAttachment.Number = certificateNumber;
                        var profileAttachmentType = _db.ProfileAttachmentTypes.FirstOrDefault(x => x.Id == typeId);
                        if(profileAttachmentType != null)
                        {
                            profileAttachment.DocumentTitle = profileAttachmentType.Name;
                        }

                        Entity_Lifecycle elc = new Entity_Lifecycle();
                        elc.Created_Date = DateTime.UtcNow.AddHours(5);
                        elc.Created_By = User.Identity.GetUserName();
                        elc.Users_Id = User.Identity.GetUserId();
                        elc.IsActive = true;
                        elc.Entity_Id = 900;
                        _db.Entity_Lifecycle.Add(elc);
                        _db.SaveChanges();

                        profileAttachment.EntityLifecycle_Id = elc.Id;
                        profileAttachment.FilePath = @"Uploads\Files\VaccinationCertificates\" + filename;


                        var buffer = await file.ReadAsByteArrayAsync();
                        var size = ((buffer.Length) / (1024)) / (1024);
                        var ext = Path.GetExtension(filename.Replace("\"", string.Empty));
                        List<string> validExtensions = new List<string>() { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".docx" };
                        if (!validExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase) || size > 2)
                        {
                            return Ok("Unable to Upload. File Size must be less than 2 MB and File Format must be " + string.Join(",", validExtensions));
                        }
                        using (FileStream fsOut = File.OpenWrite(RootPath + @"\" + filename))
                        {
                            fsOut.Write(buffer, 0, buffer.Length);
                        }
                        _db.ProfileAttachments.Add(profileAttachment);
                        _db.SaveChanges();
                    }
                    return Ok(true);
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }

        [Route("UploadServiceAttachement/{serviceHistory_Id}")]
        public async Task<bool> UploadServiceAttachement(int serviceHistory_Id)
        {
            try
            {
                using (var _db = new HR_System())
                {

                    if (!Request.Content.IsMimeMultipartContent())
                        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                    string RootPath = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\Files\ServiceHistory\";
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
                        filename = Guid.NewGuid().ToString() + "-" + serviceHistory_Id + "." + FileExtension;


                        var serviceHistory = _db.HrServiceHistories.FirstOrDefault(x => x.Id == serviceHistory_Id);
                        if (serviceHistory != null)
                        {
                            serviceHistory.OrderFilePath = @"Uploads/Files/ServiceHistory/" + filename;
                            _db.Entry(serviceHistory).State = EntityState.Modified;
                            _db.SaveChanges();
                        }

                        var buffer = await file.ReadAsByteArrayAsync();
                        var size = ((buffer.Length) / (1024)) / (1024);
                        var ext = Path.GetExtension(filename.Replace("\"", string.Empty));
                        List<string> validExtensions = new List<string>() { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".docx" };
                        if (!validExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase) || size > 10)
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

        [Route("UploaLeaveAttachement/{leaveRecord_Id}")]
        public async Task<bool> UploaLeaveAttachement(int leaveRecord_Id)
        {
            try
            {
                using (var _db = new HR_System())
                {

                    if (!Request.Content.IsMimeMultipartContent())
                        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                    string RootPath = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\Files\LeaveRecord\";
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
                        filename = Guid.NewGuid().ToString() + "-" + leaveRecord_Id + "." + FileExtension;


                        var leaveRecord = _db.HrLeaveRecords.FirstOrDefault(x => x.Id == leaveRecord_Id);
                        if (leaveRecord != null)
                        {
                            leaveRecord.OrderFilePath = @"Uploads/Files/LeaveRecord/" + filename;
                            _db.Entry(leaveRecord).State = EntityState.Modified;
                            _db.SaveChanges();
                        }

                        var buffer = await file.ReadAsByteArrayAsync();
                        var size = ((buffer.Length) / (1024)) / (1024);
                        var ext = Path.GetExtension(filename.Replace("\"", string.Empty));
                        List<string> validExtensions = new List<string>() { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".docx" };
                        if (!validExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase) || size > 10)
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
        [HttpPost]
        [Route("GetProfilesInActive")]
        public IHttpActionResult GetProfilesInActive(ProfileFilters filters)
        {
            try
            {
                if (User.IsInRole("South Punjab"))
                {
                    filters.roleName = "South Punjab";
                }
                return Ok(_profileService.GetProfilesInActive(filters));
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetProfileDetail/{cnic}/{type}")]
        public IHttpActionResult GetFile(string cnic, int type)
        {
            try
            {
                if (type == 1)
                {
                    return Ok(_profileService.GetFile(cnic));
                }
                if (type == 2)
                {
                    return Ok(_profileService.GetApplications(cnic));
                }
                if (type == 3)
                {
                    return Ok(_profileService.GetOrders(cnic));
                }
                if (type == 4)
                {
                    return Ok(_profileService.GetLeaveOrders(cnic));
                }
                if (type == 5)
                {
                    return Ok(_profileService.GetLeaveOrders(cnic));
                }
                if (type == 6)
                {
                    var profile = _profileService.GetProfile(cnic);
                    return Ok(_profileService.GetLeaveRecord(profile.Id, User.Identity.GetUserName(), User.Identity.GetUserId()));
                }
                if (type == 7)
                {
                    var profile = _profileService.GetProfile(cnic);
                    return Ok(_profileService.GetServiceHistory(profile.Id, User.Identity.GetUserName(), User.Identity.GetUserId()));
                }
                if (type == 11)
                {
                    var profile = _profileService.GetProfile(cnic);
                    return Ok(_profileService.GetHrQualification(profile.Id, User.Identity.GetUserName(), User.Identity.GetUserId()));
                }
                if (type == 0)
                {
                    return Ok(_profileService.GetProfile(cnic));
                }

                return BadRequest("Invalid Type");
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetAnyProfile/{cnic}")]
        public IHttpActionResult GetAnyProfile(string cnic)
        {
            try
            {
                return Ok(_profileService.GetAnyProfile(cnic));
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [Route("GetProfileByCNIC/{cnic}")]
        [HttpGet]
        public IHttpActionResult GetProfileByCNIC(string cnic)
        {
            cnic = cnic.Replace("-", string.Empty);
            try
            {
                using (var db = new HR_System())
                {

                    db.Configuration.ProxyCreationEnabled = false;
                    string userId = User.Identity.GetUserId();
                    var usermanger = new ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                    var user = usermanger.FindById(userId);
                    string userHfmisCode = (user.hfmiscode != null ?
                            user.hfmiscode : user.TehsilID != null ?
                                user.TehsilID : user.DistrictID != null ?
                                    user.DistrictID : user.DivisionID != null ?
                                        user.DivisionID : "0");
                    ProfileDetailsView profileDetailsView = db.ProfileDetailsViews.FirstOrDefault(x => x.CNIC.Equals(cnic));
                    if (profileDetailsView != null)
                    {
                        if (profileDetailsView.HfmisCode != null)
                        {
                            if (profileDetailsView.HfmisCode.StartsWith(userHfmisCode) || profileDetailsView.AddToEmployeePool == true)
                            {
                                return Ok(profileDetailsView);
                            }
                            else
                            {
                                return Ok(false);
                            }
                        }
                        else
                        {
                            return Ok(profileDetailsView);
                        }
                    }
                    else
                    {
                        return Ok(404);
                    }
                }

            }
            catch (Exception ex) { Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message); }
        }
        [Route("GetProfileById/{Id}")]
        [HttpGet]
        public IHttpActionResult GetProfileById(int Id)
        {
            try
            {
                using (var db = new HR_System())
                {

                    db.Configuration.ProxyCreationEnabled = false;
                    ProfileDetailsView profileDetailsView = db.ProfileDetailsViews.FirstOrDefault(x => x.Id == Id);
                    return Ok(profileDetailsView);
                }

            }
            catch (Exception ex) { Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message); }
        }
        [HttpGet]
        [Route("AddToPool/{id}")]
        public IHttpActionResult AddToPool(int id)
        {
            try
            {
                return Ok(_profileService.AddToPool(id, User.Identity.GetUserName(), User.Identity.GetUserId()));
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("RemoveFromPool/{id}")]
        public IHttpActionResult RemoveFromPool(int id)
        {
            try
            {
                return Ok(_profileService.RemoveFromPool(id, User.Identity.GetUserName(), User.Identity.GetUserId()));
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("ConfirmJoining/{id}/{joiningDate}")]
        public IHttpActionResult ConfirmJoining(int id, string joiningDate)
        {
            try
            {
                return Ok(_profileService.ConfirmJoining(id, joiningDate, User.Identity.GetUserName(), User.Identity.GetUserId()));
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("NotJoined/{id}")]
        public IHttpActionResult NotJoined(int id)
        {
            try
            {
                return Ok(_profileService.NotJoined(id, User.Identity.GetUserName(), User.Identity.GetUserId()));
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetSeniorityApplicantProfile/{cnic}")]
        public IHttpActionResult GetSeniorityApplicantProfile(string cnic)
        {
            try
            {
                return Ok(_profileService.GetSeniorityApplicantProfile(cnic));
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveSeniorityApplicationProfile")]
        public IHttpActionResult SaveSeniorityApplicationProfile()
        {
            try
            {
               if (ModelState.IsValid)
               {
                    var data = HttpContext.Current;
                    var modelData = HttpContext.Current.Request.Form;
                    using (HR_System _db = new HR_System())
                    {
                        HrSeniorityApplication hrSeniorityApplication = new HrSeniorityApplication();
                        hrSeniorityApplication = JsonConvert.DeserializeObject<HrSeniorityApplication>(modelData["Model"]);

                        string RootPath1 = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\Promotion\CNIC";
                        string RootPath2 = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\Promotion\OrderCopy";
                        string RootPath3 = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\Promotion\AssumptionReport";
                        string RootPath4 = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\Promotion\PPSCMeritList";
                        string RootPath5 = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\Promotion\PromotionOrder";
                        string RootPath6 = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\Promotion\ContractOrderCopy";
                        string RootPath7 = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\Promotion\ContractJoining";
                        string RootPath8 = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\Promotion\Status";

                        //string DeleteRootPath = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\";
                        var dirPath = RootPath1;
                        CreateDirectoryIfNotExists(RootPath1);
                        CreateDirectoryIfNotExists(RootPath2);
                        CreateDirectoryIfNotExists(RootPath3);
                        CreateDirectoryIfNotExists(RootPath4);
                        CreateDirectoryIfNotExists(RootPath5);

                        CreateDirectoryIfNotExists(RootPath6);
                        CreateDirectoryIfNotExists(RootPath7);
                        CreateDirectoryIfNotExists(RootPath8);

                        if (hrSeniorityApplication.Id > 0)
                        {
                            _db.Entry(hrSeniorityApplication).State = EntityState.Modified;
                            _db.SaveChanges();
                            


                            var Id = hrSeniorityApplication.Id;
                            HrSeniorityApplication HrSeniority = _db.HrSeniorityApplications.Find(Id);

                            HrSeniorityApplicationLog log = new HrSeniorityApplicationLog();

                            log.HrSeniorityApplicationId = Id;
                            log.CreatedBy = HrSeniority.CNIC;
                            log.CreatedDate = DateTime.UtcNow.AddHours(5);
                            log.IsActive = true;
                            log.Description = "Data Updated";
                            _db.HrSeniorityApplicationLogs.Add(log);
                            _db.SaveChanges();

                            var logId = log.Id;
                            HrSeniorityApplicationLog applicationLog = _db.HrSeniorityApplicationLogs.Find(logId);


                            if (data.Request.Files.Count > 0)
                            {
                                for (int i = 0; i < data.Request.Files.Count; i++)
                                {
                                    if (data.Request.Files.GetKey(i) == "CNIC")
                                    {
                                        var file = data.Request.Files.Get(i);
                                        if (file != null)
                                        {
                                            if (HrSeniority != null)
                                            {
                                                applicationLog.Description = "Data and Images Modified";
                                                applicationLog.CNIC_FilePath = hrSeniorityApplication.CNIC_FilePath;
                                                //FileInfo oldFile = new FileInfo(DeleteRootPath + @"\" + HrSeniority.CNIC_FilePath);
                                                //if (oldFile.Exists)
                                                //{
                                                //    oldFile.Delete();
                                                //}

                                                var filename = this.PostedFile(file, 1);
                                                var extension = Path.GetExtension(file.FileName);
                                                hrSeniorityApplication.CNIC_FilePath = "Uploads/Promotion/CNIC/" + filename + extension;
                                                hrSeniorityApplication.CNIC_FileName = file.FileName;
                                                _db.Entry(HrSeniority).State = EntityState.Modified;
                                                _db.SaveChanges();

                                                
                                                _db.Entry(applicationLog).State = EntityState.Modified;
                                                _db.SaveChanges();
                                            }
                                        }
                                    }
                                    else if (data.Request.Files.GetKey(i) == "OrderCopy")
                                    {
                                        var file = data.Request.Files.Get(i);
                                        if (file != null)
                                        {
                                            applicationLog.Description = "Data and Images Modified";
                                            applicationLog.OrderCopy_FilePath = hrSeniorityApplication.OrderCopy_FilePath;
                                            var filename = this.PostedFile(file, 2);
                                            var extension = Path.GetExtension(file.FileName);
                                            hrSeniorityApplication.OrderCopy_FilePath = "Uploads/Promotion/OrderCopy/" + filename + extension;
                                            hrSeniorityApplication.OrderCopy_FileName = file.FileName;
                                            _db.Entry(HrSeniority).State = EntityState.Modified;
                                            _db.SaveChanges();

                                           
                                            _db.Entry(applicationLog).State = EntityState.Modified;
                                            _db.SaveChanges();
                                        }
                                    }
                                    else if (data.Request.Files.GetKey(i) == "ChargeAssumption")
                                    {
                                        var file = data.Request.Files.Get(i);
                                        if (file != null)
                                        {
                                            applicationLog.Description = "Data and Images Modified";
                                            applicationLog.AssumptionReport_FilePath = hrSeniorityApplication.AssumptionReport_FilePath;
                                            var filename = this.PostedFile(file, 3);
                                            var extension = Path.GetExtension(file.FileName);
                                            hrSeniorityApplication.AssumptionReport_FilePath = "Uploads/Promotion/AssumptionReport/" + filename + extension;
                                            hrSeniorityApplication.AssumptionReport_FileName = file.FileName;
                                            _db.Entry(HrSeniority).State = EntityState.Modified;
                                            _db.SaveChanges();

                                            
                                            _db.Entry(applicationLog).State = EntityState.Modified;
                                            _db.SaveChanges();
                                        }
                                    }
                                    else if (data.Request.Files.GetKey(i) == "PPSCMeritList")
                                    {
                                        var file = data.Request.Files.Get(i);
                                        if (file != null)
                                        {
                                            applicationLog.Description = "Data and Images Modified";
                                            applicationLog.MeritList_FilePath =    hrSeniorityApplication.MeritList_FilePath;
                                            var filename = this.PostedFile(file, 4);
                                            var extension = Path.GetExtension(file.FileName);
                                            hrSeniorityApplication.MeritList_FilePath = "Uploads/Promotion/PPSCMeritList/" + filename + extension;
                                            hrSeniorityApplication.MeritList_FileName = file.FileName;
                                            _db.Entry(HrSeniority).State = EntityState.Modified;
                                            _db.SaveChanges();

                                            _db.Entry(applicationLog).State = EntityState.Modified;
                                            _db.SaveChanges();
                                        }
                                    }
                                    else if (data.Request.Files.GetKey(i) == "PromotionOrder")
                                    {
                                        var file = data.Request.Files.Get(i);
                                        if (file != null)
                                        {
                                            applicationLog.Description = "Data and Images Modified";
                                            applicationLog.PromotionOrderCopy_FilePath = hrSeniorityApplication.PromotionCopy_FilePath;
                                            var filename = this.PostedFile(file, 5);
                                            var extension = Path.GetExtension(file.FileName);
                                            hrSeniorityApplication.PromotionCopy_FilePath = "Uploads/Promotion/PromotionOrder/" + filename + extension;
                                            hrSeniorityApplication.PromotionCopy_FileName = file.FileName;
                                            _db.Entry(HrSeniority).State = EntityState.Modified;
                                            _db.SaveChanges();


                                            _db.Entry(applicationLog).State = EntityState.Modified;
                                            _db.SaveChanges();

                                        }
                                    }


                                    else if (data.Request.Files.GetKey(i) == "ContractOrderCopy")
                                    {
                                        var file = data.Request.Files.Get(i);
                                        if (file != null)
                                        {
                                            applicationLog.Description = "Data and Images Modified";
                                            applicationLog.ContractOrderCopy_FilePath = hrSeniorityApplication.ContractOrderCopy_FilePath;
                                            var filename = this.PostedFile(file, 6);
                                            var extension = Path.GetExtension(file.FileName);
                                            hrSeniorityApplication.ContractOrderCopy_FilePath = "Uploads/Promotion/ContractOrderCopy/" + filename + extension;
                                            hrSeniorityApplication.ContractOrderCopy_FileName = file.FileName;
                                            _db.Entry(HrSeniority).State = EntityState.Modified;
                                            _db.SaveChanges();


                                            _db.Entry(applicationLog).State = EntityState.Modified;
                                            _db.SaveChanges();

                                        }
                                    }
                                    else if (data.Request.Files.GetKey(i) == "ContractJoining")
                                    {
                                        var file = data.Request.Files.Get(i);
                                        if (file != null)
                                        {
                                            applicationLog.Description = "Data and Images Modified";
                                            applicationLog.ContractJoining_FilePath = hrSeniorityApplication.ContractJoining_FilePath;
                                            var filename = this.PostedFile(file, 7);
                                            var extension = Path.GetExtension(file.FileName);
                                            hrSeniorityApplication.ContractJoining_FilePath = "Uploads/Promotion/ContractJoining/" + filename + extension;
                                            hrSeniorityApplication.ContractJoining_FileName = file.FileName;
                                            _db.Entry(HrSeniority).State = EntityState.Modified;
                                            _db.SaveChanges();


                                            _db.Entry(applicationLog).State = EntityState.Modified;
                                            _db.SaveChanges();

                                        }
                                    }
                                    else if (data.Request.Files.GetKey(i) == "Status")
                                    {
                                        var file = data.Request.Files.Get(i);
                                        if (file != null)
                                        {
                                            applicationLog.Description = "Data and Images Modified";
                                            applicationLog.Status_FilePath = hrSeniorityApplication.Status_FilePath;
                                            var filename = this.PostedFile(file, 8);
                                            var extension = Path.GetExtension(file.FileName);
                                            hrSeniorityApplication.Status_FilePath = "Uploads/Promotion/Status/" + filename + extension;
                                            hrSeniorityApplication.Status_FileName = file.FileName;
                                            _db.Entry(HrSeniority).State = EntityState.Modified;
                                            _db.SaveChanges();


                                            _db.Entry(applicationLog).State = EntityState.Modified;
                                            _db.SaveChanges();

                                        }
                                    }
                                }
                            }

                            if(hrSeniorityApplication.IsLocked == true)
                            {
                                SMS sms = new SMS()
                                {
                                    UserId = User.Identity.GetUserId(),
                                    FKId = hrSeniorityApplication.Id,
                                    MobileNumber = hrSeniorityApplication.MobileNo,
                                    Message = "Promotion of Consultant Doctors (BS-18) To the Rank of Senior Consultant (BS-19)\nYour Application has been received and is currently under review.\nTracking Number: " + hrSeniorityApplication.Id + "\n\nRegards, Health Information and Service Delivery Unit\nPrimary and Secondary Healthcare Department"
                                };
                                Common.SendSMSTelenor(sms);
                            }
                        }
                        else
                        {

                          
                            _db.HrSeniorityApplications.Add(hrSeniorityApplication);
                            _db.SaveChanges();


                            var Id = hrSeniorityApplication.Id;
                            HrSeniorityApplication HrSeniority = _db.HrSeniorityApplications.Find(Id);
                            HrSeniorityApplicationLog log = new HrSeniorityApplicationLog();

                            log.HrSeniorityApplicationId = Id;
                            log.CreatedBy = HrSeniority.CNIC;
                            log.CreatedDate = DateTime.UtcNow.AddHours(5);
                            log.IsActive = true;
                            log.Description = "Data Created";
                            _db.HrSeniorityApplicationLogs.Add(log);
                            _db.SaveChanges();

                            var logId = log.Id;
                            HrSeniorityApplicationLog applicationLog = _db.HrSeniorityApplicationLogs.Find(logId);

                           
                            if (data.Request.Files.Count > 0)
                            {
                                for (int i = 0; i < data.Request.Files.Count; i++)
                                {
                                    if (data.Request.Files.GetKey(i) == "CNIC")
                                    {
                                        var file = data.Request.Files.Get(i);
                                        if (file != null)
                                        {
                                            if (HrSeniority != null)
                                            {
                                                var filename = this.PostedFile(file,1);
                                                var extension = Path.GetExtension(file.FileName);
                                                hrSeniorityApplication.CNIC_FilePath = "Uploads/Promotion/CNIC/" + filename + extension;
                                                hrSeniorityApplication.CNIC_FileName = file.FileName;
                                                _db.Entry(HrSeniority).State = EntityState.Modified;
                                                _db.SaveChanges();
                                            }
                                        }
                                    }
                                    
                                    
                                    else if (data.Request.Files.GetKey(i) == "OrderCopy")
                                    {
                                        var file = data.Request.Files.Get(i);
                                        if (file != null)
                                        {
                                            var filename = this.PostedFile(file,2);
                                            var extension = Path.GetExtension(file.FileName);
                                            hrSeniorityApplication.OrderCopy_FilePath = "Uploads/Promotion/OrderCopy/" + filename + extension;
                                            hrSeniorityApplication.OrderCopy_FileName = file.FileName;
                                            _db.Entry(HrSeniority).State = EntityState.Modified;
                                            _db.SaveChanges();
                                        }
                                    }
                                    else if (data.Request.Files.GetKey(i) == "ChargeAssumption")
                                    {
                                        var file = data.Request.Files.Get(i);
                                        if (file != null)
                                        {
                                            var filename = this.PostedFile(file,3);
                                            var extension = Path.GetExtension(file.FileName);
                                            hrSeniorityApplication.AssumptionReport_FilePath = "Uploads/Promotion/AssumptionReport/" + filename + extension;
                                            hrSeniorityApplication.AssumptionReport_FileName = file.FileName;
                                            _db.Entry(HrSeniority).State = EntityState.Modified;
                                            _db.SaveChanges();
                                        }
                                    }
                                    else if (data.Request.Files.GetKey(i) == "PPSCMeritList")
                                    {
                                        var file = data.Request.Files.Get(i);
                                        if (file != null)
                                        {
                                            var filename = this.PostedFile(file, 4);
                                            var extension = Path.GetExtension(file.FileName);
                                            hrSeniorityApplication.MeritList_FilePath = "Uploads/Promotion/PPSCMeritList/" + filename + extension;
                                            hrSeniorityApplication.MeritList_FileName = file.FileName;
                                            _db.Entry(HrSeniority).State = EntityState.Modified;
                                            _db.SaveChanges();

                                        }
                                    }
                                    else if (data.Request.Files.GetKey(i) == "PromotionOrder")
                                    {
                                        var file = data.Request.Files.Get(i);
                                        if (file != null)
                                        {
                                            var filename = this.PostedFile(file, 5);
                                            var extension = Path.GetExtension(file.FileName);
                                            hrSeniorityApplication.PromotionCopy_FilePath = "Uploads/Promotion/PromotionOrder/" + filename + extension;
                                            hrSeniorityApplication.PromotionCopy_FileName = file.FileName;
                                            _db.Entry(HrSeniority).State = EntityState.Modified;
                                            _db.SaveChanges();
                                        }
                                    }


                                    else if (data.Request.Files.GetKey(i) == "ContractOrderCopy")
                                    {
                                        var file = data.Request.Files.Get(i);
                                        if (file != null)
                                        {
                                            var filename = this.PostedFile(file, 6);
                                            var extension = Path.GetExtension(file.FileName);
                                            hrSeniorityApplication.ContractOrderCopy_FilePath = "Uploads/Promotion/ContractOrderCopy/" + filename + extension;
                                            hrSeniorityApplication.ContractOrderCopy_FileName = file.FileName;
                                            _db.Entry(HrSeniority).State = EntityState.Modified;
                                            _db.SaveChanges();
                                        }
                                    }
                                    else if (data.Request.Files.GetKey(i) == "ContractJoining")
                                    {
                                        var file = data.Request.Files.Get(i);
                                        if (file != null)
                                        {
                                            var filename = this.PostedFile(file, 7);
                                            var extension = Path.GetExtension(file.FileName);
                                            hrSeniorityApplication.ContractJoining_FilePath = "Uploads/Promotion/ContractJoining/" + filename + extension;
                                            hrSeniorityApplication.ContractJoining_FileName = file.FileName;
                                            _db.Entry(HrSeniority).State = EntityState.Modified;
                                            _db.SaveChanges();
                                        }
                                    }
                                    else if (data.Request.Files.GetKey(i) == "Status")
                                    {
                                        var file = data.Request.Files.Get(i);
                                        if (file != null)
                                        {
                                            var filename = this.PostedFile(file, 8);
                                            var extension = Path.GetExtension(file.FileName);
                                            hrSeniorityApplication.Status_FilePath = "Uploads/Promotion/Status/" + filename + extension;
                                            hrSeniorityApplication.Status_FileName = file.FileName;
                                            _db.Entry(HrSeniority).State = EntityState.Modified;
                                            _db.SaveChanges();
                                        }
                                    }

                                }
                            }
                        }
                    }
                    return Ok(true);
               }
                return Ok(true);
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("ChangeSeniorityApplicationStatus/{Id}/{StatusId}")]
        public IHttpActionResult ChangeSeniorityApplicationStatus(int Id, int StatusId)
        {
            try
            {
                return Ok(_profileService.ChangeSeniorityApplicationStatus(Id, StatusId, User.Identity.GetUserName(), User.Identity.GetUserId()));
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }


        private void CreateDirectoryIfNotExists(string dirPath) { if (!Directory.Exists(dirPath)) { Directory.CreateDirectory(dirPath); } }

       public string PostedFile(HttpPostedFile file, int Id)
       {
            var fileName = Guid.NewGuid().ToString();
            var fileExtension = Path.GetExtension(file.FileName);
            string RootPath = "";

            if(Id == 1)
            {
                RootPath = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\Promotion\CNIC";
            }
            if (Id == 2)
            {
                 RootPath = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\Promotion\OrderCopy";
            }
            if (Id == 3)
            {
                 RootPath = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\Promotion\AssumptionReport";
            }
            if (Id == 4)
            {
                 RootPath = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\Promotion\PPSCMeritList";
            }
            if (Id == 5)
            {
                 RootPath = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\Promotion\PromotionOrder";
            }

            if (Id == 6)
            {
                RootPath = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\Promotion\ContractOrderCopy";
            }
            if (Id == 7)
            {
                RootPath = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\Promotion\ContractJoining";
            }
            if (Id == 8)
            {
                RootPath = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\Promotion\Status";
            }

            file.SaveAs(RootPath + @"\" + fileName + fileExtension);
            //file.SaveAs(HttpContext.Current.Server.MapPath("~") + "/Uploads/Promotion" + fileName);
            return fileName;       
       }
    }
}
