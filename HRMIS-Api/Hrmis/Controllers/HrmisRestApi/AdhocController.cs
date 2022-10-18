using Hrmis.Models.Common;
using Hrmis.Models.DbModel;
using Hrmis.Models.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Hrmis.Controllers.HrmisRestApi
{
    //[Authorize]
    [RoutePrefix("api/Adhoc")]
    public class AdhocController : ApiController
    {
        private readonly PublicService publicService;
        private TransferPostingService _transferPostingService;
        private List<string> postedHfs;

        public AdhocController()
        {
            _transferPostingService = new TransferPostingService();
        }
        [HttpPost]
        [Route("SaveAdhocJob")]
        public IHttpActionResult SaveAdhocJob([FromBody] AdhocSaveDTO obj)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var adhocJob = db.AdhocJobs.FirstOrDefault(x => x.Designation_Id == obj.job.Designation_Id);
                    if (adhocJob == null)
                    {
                        obj.job.CreatedBy = User.Identity.GetUserName();
                        obj.job.CreatedDate = DateTime.UtcNow.AddHours(5);
                        obj.job.UserId = User.Identity.GetUserId();
                        db.AdhocJobs.Add(obj.job);
                        db.SaveChanges();
                        foreach (var item in obj.JobDocuments)
                        {
                            var jobDoc = new AdhocDocumentRequired();
                            jobDoc.JobDocument_Id = item.Id;
                            jobDoc.Job_Id = obj.job.Id;
                            jobDoc.IsActive = true;
                            jobDoc.CreatedBy = User.Identity.GetUserName();
                            jobDoc.CreatedDate = DateTime.UtcNow.AddHours(5);
                            jobDoc.UserId = User.Identity.GetUserId();
                            db.AdhocDocumentRequireds.Add(jobDoc);
                            db.SaveChanges();
                        }
                        foreach (var item in obj.JobHFs)
                        {
                            var jobHF = new AdhocHF();
                            jobHF.Job_Id = obj.job.Id;
                            jobHF.HF_Id = item.HF_Id;
                            jobHF.HFMISCode = item.HFMISCode;
                            jobHF.SeatsOpen = item.SeatsOpen;
                            jobHF.IsActive = true;
                            jobHF.CreatedBy = User.Identity.GetUserName();
                            jobHF.CreatedDate = DateTime.UtcNow.AddHours(5);
                            jobHF.UserId = User.Identity.GetUserId();
                            db.AdhocHFs.Add(jobHF);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        adhocJob.IsActive = obj.job.IsActive;
                        adhocJob.AgeLimit = obj.job.AgeLimit;
                        adhocJob.RelevantExperience = obj.job.RelevantExperience;
                        adhocJob.Experience = obj.job.Experience;
                        adhocJob.SeatsOpen = obj.job.SeatsOpen;
                        adhocJob.EndDate = obj.job.EndDate;
                        adhocJob.StartDate = DateTime.UtcNow.AddHours(5);
                        db.Entry(adhocJob).State = EntityState.Modified;
                        db.SaveChanges();
                        AdhocLog jobLog = new AdhocLog();
                        jobLog.Job_Id = adhocJob.Id;
                        jobLog.TurnedON = adhocJob.IsActive;
                        jobLog.Remarks = "Job Turned " + (adhocJob.IsActive == true ? "On" : "Off") + ". By " + User.Identity.GetUserName();
                        jobLog.Username = User.Identity.GetUserName();
                        jobLog.DateTime = DateTime.UtcNow.AddHours(5);
                        jobLog.UserId = User.Identity.GetUserId();
                        db.AdhocLogs.Add(jobLog);
                        db.SaveChanges();
                        return Ok(adhocJob);
                    }
                    return Ok(obj.job);
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
        [Route("GetDegrees/{designationCatId}")]
        [HttpGet]
        public async Task<List<Degree>> GetDegrees(int designationCatId)
        {
            using (var db = new HR_System())
            {
                db.Configuration.ProxyCreationEnabled = false;
                if (designationCatId == 0)
                {
                    var degrees = await db.Degrees.Where(x => x.IsActive == true).OrderBy(x => x.QualificationTypeId).ToListAsync();
                    return degrees;
                }
                else
                {
                    var degreeIds = new List<int>() { 58, 65, 62, 119, 118, 117, 116 };
                    if (designationCatId == 802 || designationCatId == 1320)
                    {
                        degreeIds = new List<int>() { 93, 94, 58, 65, 62, 119, 118, 117, 116, 122, 123, 124, 125, 126, 127, 128, 130 };
                    }
                    List<int> consultants = new List<int>()
                {
                    362,365,368,369,373,302,374,375,381,382,383,384,385,387,390,1594,1598,2136,2313
                };
                    if (consultants.Contains(designationCatId))
                    {
                        degreeIds = new List<int>() { 93, 94, 58, 65, 62, 119, 118, 117, 116, 122, 123, 124, 125, 126, 127, 128 };
                        if (designationCatId == 385)
                        {
                            degreeIds.Add(137);
                        }
                        if (designationCatId == 362)
                        {
                            degreeIds.Add(130);
                        }
                        if (designationCatId == 365)
                        {
                            degreeIds.Add(138);
                        }
                        if (designationCatId == 368)
                        {
                            degreeIds.Add(139);
                        }
                        if (designationCatId == 369)
                        {
                            degreeIds.Add(140);
                        }
                        if (designationCatId == 373)
                        {
                            degreeIds.Add(147);
                        }
                        if (designationCatId == 374)
                        {
                            degreeIds.Add(141);
                        }
                        if (designationCatId == 381)
                        {
                            degreeIds.Add(142);
                        }
                        if (designationCatId == 382)
                        {
                            degreeIds.Add(143);
                            degreeIds.Add(120);
                        }
                        if (designationCatId == 390)
                        {
                            degreeIds.Add(144);
                        }
                        if (designationCatId == 1594)
                        {
                            degreeIds.Add(145);
                        }
                        if (designationCatId == 2136)
                        {
                            degreeIds.Add(146);
                        }
                        if (designationCatId == 384)
                        {
                            degreeIds.Add(148);
                        }

                    }
                    if (designationCatId == 302)
                    {
                        degreeIds = new List<int>() { 63, 93, 94, 98, 105, 58, 65, 129, 131, 132, 133 };
                    }
                    if (designationCatId == 431)
                    {
                        degreeIds = new List<int>() { 93, 94, 58, 65, 113, 134, 135, 136 };
                    }
                    var degrees = await db.Degrees.Where(x => degreeIds.Contains(x.Id) && x.IsActive == true).OrderBy(x => x.QualificationTypeId).ThenBy(x => x.CreatedBy).ToListAsync();
                    return degrees;
                }
            }
        }

        [HttpPost]
        [Route("SaveApplicant")]
        public IHttpActionResult SaveApplicant([FromBody] AdhocApplicant obj)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    if (obj.Id == 0)
                    {
                        obj.Status_Id = 1;
                        obj.IsActive = true;
                        obj.CreatedBy = User.Identity.GetUserName();
                        obj.CreatedDate = DateTime.UtcNow.AddHours(5);
                        obj.UserId = User.Identity.GetUserId();
                        db.AdhocApplicants.Add(obj);
                        db.SaveChanges();
                    }
                    else
                    {
                        obj.IsActive = true;
                        obj.CreatedBy = User.Identity.GetUserName();
                        obj.CreatedDate = DateTime.UtcNow.AddHours(5);
                        obj.UserId = User.Identity.GetUserId();
                        db.Entry(obj).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return Ok(obj);
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

        [HttpPost]
        [Route("SaveApplicationGrievance")]
        public IHttpActionResult SaveApplicationGrievance([FromBody] AdhocApplicationGrievance obj)
        {
            var currentDate = DateTime.UtcNow.AddHours(5);
            var endDate = "12/16/2021 23:59:59";
            DateTime EndDate = Convert.ToDateTime(endDate);
            try
            {
                if (currentDate < EndDate)
                {
                    using (var db = new HR_System())
                    {
                        db.Configuration.ProxyCreationEnabled = false;
                        if (obj.Id == 0)
                        {
                            obj.StatusId = 1;
                            obj.IsActive = true;
                            obj.CreatedBy = User.Identity.GetUserName();
                            obj.CreatedDate = DateTime.UtcNow.AddHours(5);
                            obj.UserId = User.Identity.GetUserId();
                            db.AdhocApplicationGrievances.Add(obj);
                            db.SaveChanges();
                        }
                        else
                        {
                            obj.IsActive = true;
                            obj.CreatedBy = User.Identity.GetUserName();
                            obj.CreatedDate = DateTime.UtcNow.AddHours(5);
                            obj.UserId = User.Identity.GetUserId();
                            db.Entry(obj).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        return Ok(obj);
                    }
                }
                else
                {
                    return Ok("False");
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
        [HttpGet]
        [Route("LockApplication/{applicationId}")]
        public IHttpActionResult LockApplication(int applicationId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var application = db.AdhocApplications.FirstOrDefault(x => x.Id == applicationId);
                    int statusId = (int)application.Status_Id;
                    if (application != null)
                    {
                        AdhocApplicationLog adhocApplicationLog = new AdhocApplicationLog();
                        adhocApplicationLog.Application_Id = application.Id;
                        adhocApplicationLog.StatusId = 4;
                        adhocApplicationLog.IsActive = true;
                        adhocApplicationLog.CreatedBy = User.Identity.GetUserName();
                        adhocApplicationLog.CreatedDate = DateTime.UtcNow.AddHours(5);
                        adhocApplicationLog.UserId = User.Identity.GetUserId();
                        db.AdhocApplicationLogs.Add(adhocApplicationLog);
                        //application.Status_Id = 4;
                        //db.Entry(application).State = EntityState.Modified;
                        //db.SaveChanges();



                        if (statusId != 4)
                        {
                            var applicant = db.AdhocApplicants.FirstOrDefault(x => x.Id == application.Applicant_Id);
                            if (applicant != null)
                            {
                                applicant.Status_Id = 4;
                                db.Entry(applicant).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            string MessageBody = "";
                            MessageBody = @"Dear Applicant, Thank you for applying on Adhoc basis. \nYour application has been received. Tracking No. " + application.Id + "\n\nRegards,\nHealth Information and Service Delivery Unit \nPrimary & Secondary Healthcare Department";

                            if (!string.IsNullOrEmpty(applicant.MobileNumber))
                            {
                                SMS sms1 = new SMS()
                                {
                                    UserId = User.Identity.GetUserId(),
                                    FKId = application.Id,
                                    MobileNumber = applicant.MobileNumber,
                                    Message = MessageBody
                                };
                                //Common.SendSMSTelenor(sms1);
                                return Ok(application);
                            }
                        }
                    }
                    return Ok(application);
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
        //[AllowAnonymous]
        //[HttpGet]
        //[Route("SendConfimationSMSToAll")]
        //public IHttpActionResult SendConfimationSMSToAll()
        //{
        //    try
        //    {
        //        using (var db = new HR_System())
        //        {
        //            db.Configuration.ProxyCreationEnabled = false;
        //            string MessageBody = "";
        //            int smsCount = 0;
        //            var applications = db.AdhocApplicationViews.Where(x => x.Status_Id == 4 && x.IsActive == true).ToList();
        //            foreach (var application in applications)
        //            {
        //                if (application != null)
        //                {
        //                    var applicant = db.AdhocApplicantViews.FirstOrDefault(x => x.Id == application.Applicant_Id && x.IsActive == true);
        //                    if (applicant != null && !string.IsNullOrEmpty(applicant.MobileNumber))
        //                    {
        //                        MessageBody = @"Dear Applicant, Thank you for applying on Adhoc basis. \nYour application has been received. Tracking No. " + application.Id + "\n\nRegards,\nHealth Information and Service Delivery Unit\nPrimary & Secondary Healthcare Department";
        //                        SMS sms1 = new SMS()
        //                        {
        //                            UserId = "Admin",
        //                            FKId = application.Id,
        //                            MobileNumber = applicant.MobileNumber,
        //                            Message = MessageBody
        //                        };
        //                        smsCount++;
        //                        Common.SendSMSTelenor(sms1);
        //                    }

        //                }
        //            }
        //            return Ok(smsCount);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        while (ex.InnerException != null)
        //        {
        //            ex = ex.InnerException;
        //        }
        //        Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
        //    }
        //}


        [HttpPost]
        [Route("SaveAdhocApplication")]
        public IHttpActionResult SaveAdhocApplication([FromBody] AdhocApplication obj)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var application = db.AdhocApplications.FirstOrDefault(x => x.Applicant_Id == obj.Applicant_Id && x.Designation_Id == obj.Designation_Id && x.IsActive == true);
                    if (application != null)
                    {
                        return Ok(application);
                    }
                    if (obj.Id == 0)
                    {
                        var job = db.AdhocJobs.FirstOrDefault(x => x.Designation_Id == obj.Designation_Id && x.IsActive == true);
                        obj.Job_Id = job.Id;
                        obj.Status_Id = 1;
                        obj.IsActive = true;
                        obj.CreatedBy = User.Identity.GetUserName();
                        obj.CreatedDate = DateTime.UtcNow.AddHours(5);
                        obj.UserId = User.Identity.GetUserId();
                        db.AdhocApplications.Add(obj);
                        db.SaveChanges();
                    }
                    return Ok(obj);
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

        [HttpPost]
        [Route("SaveAdhocPreference")]
        public IHttpActionResult AdhocApplication([FromBody] AdhocApplicationPreference applicantPreference)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var preferenceAlreadyExisit = db.AdhocApplicationPreferences.FirstOrDefault(x => x.JobApplication_Id == applicantPreference.JobApplication_Id && x.JobHF_Id == applicantPreference.JobHF_Id && x.IsActive == true);
                    if (preferenceAlreadyExisit != null)
                    {
                        return Ok("Already");
                    }
                    if (applicantPreference.Id == 0)
                    {
                        var recentPreferences = db.AdhocApplicationPreferences.Where(x => x.JobApplication_Id == applicantPreference.JobApplication_Id && x.IsActive == true).OrderByDescending(x => x.PreferenceOrder).ToList();
                        var lastPref = recentPreferences.FirstOrDefault();
                        if (lastPref != null)
                        {
                            applicantPreference.PreferenceOrder = lastPref.PreferenceOrder + 1;
                        }
                        else
                        {
                            applicantPreference.PreferenceOrder = 1;
                        }
                        applicantPreference.IsActive = true;
                        applicantPreference.CreatedDate = DateTime.UtcNow.AddHours(5);
                        applicantPreference.UserId = User.Identity.GetUserId();
                        applicantPreference.CreatedBy = User.Identity.GetUserName();
                        db.AdhocApplicationPreferences.Add(applicantPreference);
                        db.SaveChanges();

                        var applicant = db.AdhocApplicants.FirstOrDefault(x => x.Id == applicantPreference.JobApplicant_Id);

                        if (applicant.Status_Id < 7)
                        {
                            applicant.Status_Id = 7;
                            db.Entry(applicant).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    return Ok(applicantPreference);
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

        [HttpGet]
        [Route("GetApplication/{applicantId}")]
        public IHttpActionResult GetApplication(int applicantId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var recentApplication = db.AdhocApplicationViews.Where(x => x.Applicant_Id == applicantId && x.IsActive == true).ToList();
                    var recentPreference = db.AdhocApplicationPreferenceViews.Where(x => x.JobApplicant_Id == applicantId && x.IsActive == true).ToList();
                    return Ok(new { recentApplication, recentPreference });
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
        [HttpGet]
        [Route("GetAdhocApplicationByDesig/{applicantId}/{designationId}")]
        public IHttpActionResult getAdhocApplicationByDesig(int applicantId, int designationId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var application = db.AdhocApplicationViews.FirstOrDefault(x => x.Applicant_Id == applicantId && x.Designation_Id == designationId && x.IsActive == true);
                    return Ok(application);
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
        [HttpGet]
        [Route("GetApplicationSingle/{applicantId}/{designationId}")]
        public IHttpActionResult GetApplicationSingle(int applicantId, int designationId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var recentApplication = db.AdhocApplicationViews.FirstOrDefault(x => x.Applicant_Id == applicantId && x.Designation_Id == designationId && x.IsActive == true);
                    return Ok(recentApplication);
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
        [HttpGet]
        [Route("GetApplicationGrievances/{applicantId}")]
        public IHttpActionResult Grievances(int applicantId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var recentApplications = db.AdhocApplications.Where(x => x.Applicant_Id == applicantId && x.IsActive == true).Select(s => s.Id).ToList();
                    var grievances = db.AdhocApplicationGrievanceVs.Where(x => recentApplications.Contains((int)x.Application_Id) && x.IsActive == true).ToList();
                    return Ok(grievances);
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
        [HttpGet]
        [Route("GetApplicationPref/{applicationId}")]
        public IHttpActionResult GetApplicationPref(int applicationId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var preferences = db.AdhocApplicationPreferenceViews.Where(x => x.JobApplication_Id == applicationId && x.IsActive == true).OrderBy(o => o.PreferenceOrder).ToList();
                    return Ok(preferences);
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
        [HttpGet]
        [Route("GetApplicationPrefs/{applicationId}")]
        public IHttpActionResult GetApplicationPrefs(int applicationId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var preferences = db.AdhocApplicationPreferenceViews.Where(x => x.JobApplicant_Id == applicationId && x.IsActive == true).OrderBy(x => x.PreferenceOrder).ToList();
                    return Ok(preferences);
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
        [HttpPost]
        [Route("AdhocApplicantQualification")]
        public IHttpActionResult AdhocApplicantQualification([FromBody] AdhocApplicantQualification obj)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    if (obj.Id == 0)
                    {
                        var applicant = db.AdhocApplicants.FirstOrDefault(x => x.Id == obj.Applicant_Id);

                        obj.IsActive = true;
                        obj.CreatedBy = User.Identity.GetUserName();
                        obj.CreatedDate = DateTime.UtcNow.AddHours(5);
                        obj.UserId = User.Identity.GetUserId();
                        db.AdhocApplicantQualifications.Add(obj);
                        db.SaveChanges();
                        if (applicant.Status_Id < 5)
                        {
                            applicant.Status_Id = 5;
                            db.Entry(applicant).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    else if (obj.Id > 0 && obj.Old_Id > 0)
                    {
                        var qualification = db.AdhocApplicantQualifications.FirstOrDefault(x => x.Id == obj.Id);
                        if (qualification != null)
                        {
                            qualification.IsActive = false;
                            db.Entry(qualification).State = EntityState.Modified;
                            db.SaveChanges();

                            obj.Id = 0;
                            obj.UploadPath = null;
                            obj.IsActive = true;
                            obj.CreatedBy = User.Identity.GetUserName();
                            obj.CreatedDate = DateTime.UtcNow.AddHours(5);
                            obj.UserId = User.Identity.GetUserId();
                            db.AdhocApplicantQualifications.Add(obj);
                            db.SaveChanges();
                        }
                    }
                    return Ok(obj);
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
        [HttpPost]
        [Route("ChangeAdhocApplicantQualification")]
        public IHttpActionResult ChangeAdhocApplicantQualification([FromBody] AdhocApplicantQualification obj)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    if (obj.Id > 0)
                    {
                        var qualification = db.AdhocApplicantQualifications.FirstOrDefault(x => x.Id == obj.Id);
                        if (qualification != null)
                        {
                            qualification.TotalMarks = obj.TotalMarks;
                            qualification.ObtainedMarks = obj.ObtainedMarks;
                            db.Entry(qualification).State = EntityState.Modified;
                            db.SaveChanges();

                            var applicantLog = new AdhocApplicantLog();
                            applicantLog.Applicant_Id = obj.Applicant_Id;
                            applicantLog.Remarks = "Qualification marks changed";
                            applicantLog.IsActive = true;
                            applicantLog.CreatedBy = User.Identity.GetUserName();
                            applicantLog.CreatedDate = DateTime.UtcNow.AddHours(5);
                            applicantLog.UserId = User.Identity.GetUserId();
                            db.AdhocApplicantLogs.Add(applicantLog);
                            db.SaveChanges();
                        }
                    }
                    return Ok(obj);
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
        [HttpPost]
        [Route("AdhocApplicantExperience")]
        public IHttpActionResult AdhocApplicantExperience([FromBody] AdhocApplicantExperience obj)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    if (obj.Id == 0)
                    {
                        var applicant = db.AdhocApplicants.FirstOrDefault(x => x.Id == obj.Applicant_Id);

                        obj.IsActive = true;
                        obj.CreatedBy = User.Identity.GetUserName();
                        obj.CreatedDate = DateTime.UtcNow.AddHours(5);
                        obj.UserId = User.Identity.GetUserId();
                        db.AdhocApplicantExperiences.Add(obj);
                        db.SaveChanges();
                        if (applicant.Status_Id != 6)
                        {
                            applicant.Status_Id = 6;
                            db.Entry(applicant).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    return Ok(obj);
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
        [HttpGet]
        [Route("GetExperiences/{applicantId}")]
        public IHttpActionResult GetExperiences(int applicantId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var experiences = db.AdhocApplicantExperiences.Where(x => x.Applicant_Id == applicantId && x.IsActive == true).ToList();
                    return Ok(experiences);
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
        [HttpGet]
        [Route("GetApplicantQualificationById/{id}")]
        public IHttpActionResult GetApplicantQualificationById(int id)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var applicantQ = db.AdhocApplicantQualificationViews.FirstOrDefault(x => x.Id == id && x.IsActive == true);
                    return Ok(applicantQ);
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
        [HttpGet]
        [Route("GetApplicantQualification/{applicantId}")]
        public IHttpActionResult GetApplicantQualification(int applicantId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var applicantQ = db.AdhocApplicantQualificationViews.Where(x => x.Applicant_Id == applicantId && x.IsActive == true).OrderBy(k => k.QualificationTypeId).ToList();
                    return Ok(applicantQ);
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
        [HttpGet]
        [Route("GetApplicantDocuments/{applicantId}")]
        public IHttpActionResult GetApplicantDocuments(int applicantId)

        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var applicantDocuments = db.AdhocApplicantDocumentViews.Where(x => x.Applicant_Id == applicantId && x.IsActive == true).OrderBy(k => k.OrderBy).ToList();
                    return Ok(applicantDocuments);
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
        [HttpGet]
        [Route("GetMeritMarks")]
        public IHttpActionResult GetMeritMarks()
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var res = db.MeritMarks.ToList();
                    return Ok(res);
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
        [HttpGet]
        [Route("GetAdhocApplicationMarks/{applicationId}")]
        public IHttpActionResult GetAdhocApplicationMarks(int applicationId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var res = db.AdhocApplicationMarksViews.Where(x => x.Application_Id == applicationId && x.IsActive == true).OrderBy(x => x.Marks_Id).ToList();
                    return Ok(res);
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
        [HttpGet]
        [Route("RemoveExperience/{experienceId}")]
        public IHttpActionResult RemoveExperience(int experienceId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var experience = db.AdhocApplicantExperiences.FirstOrDefault(x => x.Id == experienceId);
                    //if (experience != null)
                    //{
                    //    experience.IsActive = false;
                    //    db.Entry(experience).State = EntityState.Modified;
                    //    db.SaveChanges();
                    //    return Ok(true);
                    //}
                    return Ok(false);
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
        [HttpPost]
        [Route("GetJobApplications")]
        public IHttpActionResult GetJobApplications([FromBody] AdhocApplicaitionFilters filters)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var preferences = db.AdhocApplicationPreferenceViews.Where(x => x.HFMISCode.StartsWith(filters.hfmisCode) && x.IsActive == true).ToList();
                    var preferenceIds = preferences.Select(x => x.JobApplication_Id).ToList();
                    var applications = db.AdhocApplicationViews.Where(x => preferenceIds.Contains(x.Id) && x.IsActive == true).GroupBy(x => new { x.Applicant_Id }).Select(x => new AdhocApplicantIds
                    {
                        Id = (int)x.Key.Applicant_Id

                    }).ToList();
                    var applicantIds = applications.Select(x => x.Id).ToList();
                    var applicantsQuery = db.AdhocApplicantViews.Where(x => applicantIds.Contains(x.Id) && x.IsActive == true).AsQueryable();
                    var applicants = applicantsQuery.OrderBy(l => l.Name).Skip(filters.Skip).Take(filters.PageSize).ToList();
                    var totalRecords = applicantsQuery.Count();
                    return Ok(new { applications, applicants, totalRecords });
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

        [HttpGet]
        [Route("GetAdhocApplicationStatus")]
        public IHttpActionResult GetAdhocApplicationStatus()
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var res = db.AdhocApplicationStatus.Where(x => x.IsActive == true).ToList();
                    return Ok(res);
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

        [HttpGet]
        [Route("GetAdhocJobs")]
        public IHttpActionResult GetAdhocJobs()
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var res = db.AdhocJobViews.Where(x => x.IsActive == true && x.OrderBy != null).OrderBy(x => x.OrderBy).ToList();
                    return Ok(res);
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

        [HttpGet]
        [Route("GetAdhocScrutiny/{applicationId}")]
        public IHttpActionResult GetAdhocScrutiny(int applicationId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var res = db.AdhocScrutinyViews.Where(x => x.Application_Id == applicationId && x.IsActive == true).OrderBy(x => x.Id).ToList();
                    return Ok(res);
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
        [HttpGet]
        [Route("GetAdhocScrutinyByApplicant/{applicantId}")]
        public IHttpActionResult GetAdhocScrutinyByApplicant(int applicantId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var res = db.AdhocScrutinyViews.Where(x => x.Applicant_Id == applicantId && x.IsActive == true).OrderBy(x => x.Id).ToList();
                    return Ok(res);
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
        [HttpGet]
        [Route("GetAdhocGrievance/{applicationId}")]
        public IHttpActionResult GetAdhocGrievance(int applicationId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var res = db.AdhocApplicationGrievances.FirstOrDefault(x => x.Application_Id == applicationId && x.IsActive == true);
                    return Ok(res);
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
        [HttpGet]
        [Route("GetAdhocGrievancesByApplicant/{applicantId}")]
        public IHttpActionResult GetAdhocGrievancesByApplicant(int applicantId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var applicationIds = db.AdhocApplications.Where(x => x.Applicant_Id == applicantId && x.IsActive == true).Select(x => x.Id).ToList();
                    var res = db.AdhocApplicationGrievances.Where(x => applicationIds.Contains((int)x.Application_Id) && x.IsActive == true).ToList();
                    return Ok(res);
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
        [HttpGet]
        [Route("RemoveGrievience/{Id}")]
        public IHttpActionResult RemoveGrievience(int Id)
        {
            var currentDate = DateTime.UtcNow.AddHours(5);
            var endDate = "12/16/2021 23:59:59";
            DateTime EndDate = Convert.ToDateTime(endDate);
            try
            {
                if (currentDate < EndDate)
                {
                    using (var db = new HR_System())
                    {
                        db.Configuration.ProxyCreationEnabled = false;
                        var grievance = db.AdhocApplicationGrievances.FirstOrDefault(x => x.Id == Id && x.IsActive == true);
                        if (grievance != null)
                        {
                            grievance.IsActive = false;
                            db.SaveChanges();
                        }
                        return Ok(grievance);
                    }
                }
                else
                {
                    return Ok("False");
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
        [HttpGet]
        [Route("UndoAdhocScrutiny/{id}")]
        public IHttpActionResult UndoAdhocScrutiny(int id)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var adhocScrutiny = db.AdhocScrutinies.FirstOrDefault(x => x.Id == id);
                    if (adhocScrutiny != null)
                    {
                        adhocScrutiny.IsActive = false;
                        db.Entry(adhocScrutiny).State = EntityState.Modified;
                        db.SaveChanges();
                        return Ok(true);
                    }
                    return Ok(false);
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
        [HttpPost]
        [Route("SaveAdhocScrutiny")]
        public IHttpActionResult SaveAdhocScrutiny([FromBody] AdhocScrutiny obj)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    if (obj.Application_Id > 0)
                    {
                        obj.IsActive = true;
                        obj.CreatedBy = User.Identity.GetUserName();
                        obj.CreatedDate = DateTime.UtcNow.AddHours(5);
                        obj.UserId = User.Identity.GetUserId();
                        db.AdhocScrutinies.Add(obj);
                        db.SaveChanges();
                    }
                    return Ok(obj);
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
        [HttpPost]
        [Route("SaveAdhocApplicantPMC")]
        public IHttpActionResult SaveAdhocApplicantPMC([FromBody] AdhocApplicantPMCDto obj)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    if (obj.AdhocApplicantPMC != null && obj.AdhocApplicantPMC.Applicant_Id > 0 && !string.IsNullOrEmpty(obj.AdhocApplicantPMC.Status))
                    {
                        obj.AdhocApplicantPMC.IsActive = true;
                        obj.AdhocApplicantPMC.CreatedBy = User.Identity.GetUserName();
                        obj.AdhocApplicantPMC.CreatedDate = DateTime.UtcNow.AddHours(5);
                        obj.AdhocApplicantPMC.UserId = User.Identity.GetUserId();
                        db.AdhocApplicantPMCs.Add(obj.AdhocApplicantPMC);
                        db.SaveChanges();
                        foreach (var qualification in obj.Qualifications)
                        {
                            qualification.PMC_Id = obj.AdhocApplicantPMC.Id;
                            qualification.IsActive = true;
                            qualification.CreatedBy = User.Identity.GetUserName();
                            qualification.CreatedDate = DateTime.UtcNow.AddHours(5);
                            qualification.UserId = User.Identity.GetUserId();
                            db.AdhocApplicantPMCQualifications.Add(qualification);
                            db.SaveChanges();
                        }
                    }
                    return Ok(obj);
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
        [HttpPost]
        [Route("SaveAdhocMerit")]
        public IHttpActionResult SaveAdhocMerit([FromBody] AdhocMerit obj)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    if (obj.Id == 0)
                    {
                        var matchedRecords = db.AdhocMerits.Where(x => x.BatchApplication_Id == obj.BatchApplication_Id).ToList();
                        matchedRecords.ForEach(e => { db.AdhocMerits.Remove(e); db.SaveChanges(); });
                        obj.CreatedBy = User.Identity.GetUserName();
                        obj.CreatedDate = DateTime.UtcNow.AddHours(5);
                        obj.UserId = User.Identity.GetUserId();
                        obj.IsActive = true;
                        db.AdhocMerits.Add(obj);
                        db.SaveChanges();
                    }
                    return Ok(obj);
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


        [HttpGet]
        [Route("GetAdhocMerit/{DesignationId}/{DistrictCode}")]
        public async Task<IHttpActionResult> GetAdhocMerit(int DesignationId, string DistrictCode)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var adhocInterview = await db.AdhocInterviewViews.FirstOrDefaultAsync(x => x.Designation_Id == DesignationId && x.DistrictCode.StartsWith(DistrictCode) && x.IsActive == true);
                    if (adhocInterview == null)
                    {
                        return Ok(new { NoInterview = true });
                    }
                    var batchApplicationIds = await db.AdhocInterviewBatchApplicationVs.Where(x => x.InterviewId == adhocInterview.Id && x.IsPresent == true && x.IsLocked == true && x.IsRejected != true).Select(x => x.Id).ToListAsync();
                    var adhocMerits = await db.AdhocMeritView2.Where(x => batchApplicationIds.Contains((int)x.BatchApplication_Id) && x.IsActive == true).OrderBy(x => x.MeritNumber).ToListAsync();

                    var applicationIds = adhocMerits.Select(x => x.Application_Id).ToList();
                    var applicantIds = await db.AdhocApplicationViews.Where(x => applicationIds.Contains(x.Id) && x.IsActive == true).GroupBy(x => new { x.Applicant_Id }).Select(x => x.Key.Applicant_Id).ToListAsync();
                    var applicantQualifications = await db.AdhocApplicantQualificationViews.Where(x => applicantIds.Contains(x.Applicant_Id) && x.IsActive == true).OrderBy(x => x.QualificationTypeId).ToListAsync();
                    var applicantExperiences = await db.AdhocApplicantExperiences.Where(x => applicantIds.Contains(x.Applicant_Id) && x.IsActive == true && !x.JobTitle.ToLower().Contains("house") && !x.Organization.ToLower().Contains("house") && !x.JobTitle.ToLower().Contains("grad") && !x.Organization.ToLower().Contains("grad") && !x.JobTitle.ToLower().Contains("pgr") && !x.Organization.ToLower().Contains("pgr") && !x.JobTitle.ToLower().Contains("houce") && !x.Organization.ToLower().Contains("houce") && !x.JobTitle.ToLower().Contains("train") && !x.Organization.ToLower().Contains("train")).OrderByDescending(x => x.FromDate).ToListAsync();

                    int total = db.AdhocInterviewBatchApplicationVs.Count(x => x.InterviewId == adhocInterview.Id && x.IsPresent == true);
                    int rejected = db.AdhocInterviewBatchApplicationVs.Count(x => x.InterviewId == adhocInterview.Id && x.IsPresent == true && x.IsRejected == true);
                    int accepted = db.AdhocInterviewBatchApplicationVs.Count(x => x.InterviewId == adhocInterview.Id && x.IsPresent == true && x.IsLocked == true && x.IsRejected != true);

                    return Ok(new { adhocInterview, adhocMerits, applicantQualifications, applicantExperiences, total, rejected, accepted });
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

        [HttpGet]
        [Route("GetAdhocMeritLocked/{DesignationId}/{DistrictCode}")]
        public async Task<IHttpActionResult> GetAdhocMeritLocked(int DesignationId, string DistrictCode)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var adhocInterview = await db.AdhocInterviewViews.FirstOrDefaultAsync(x => x.Designation_Id == DesignationId && x.DistrictCode.StartsWith(DistrictCode) && x.IsActive == true);
                    var batchApplicationIds = await db.AdhocInterviewBatchApplicationVs.Where(x => x.InterviewId == adhocInterview.Id && x.IsPresent == true && x.IsLocked == true && x.IsRejected != true).Select(x => x.Id).ToListAsync();
                    var adhocMerits = await db.AdhocMeritLockedViews.Where(x => batchApplicationIds.Contains((int)x.BatchApplication_Id) && x.IsActive == true).OrderBy(x => x.MeritNumber).ToListAsync();

                    var applicantIds = adhocMerits.Select(x => x.Applicant_Id).ToList();
                    var adhocPostings = await db.AdhocPostingFinalViews.Where(x => applicantIds.Contains((int)x.Applicant_Id) && x.Designation_Id == DesignationId && x.IsActive == true).OrderBy(x => x.MeritNumber).ToListAsync();
                    return Ok(new { adhocInterview, adhocMerits, adhocPostings });
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

        [HttpPost]
        [Route("SaveMeritVerification")]
        public IHttpActionResult SaveMeritVerification([FromBody] AdhocMeritVerification obj)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    if (obj.QualificationId != null && obj.QualificationId > 0)
                    {
                        var qualification = db.AdhocApplicantQualifications.FirstOrDefault(x => x.Id == obj.QualificationId);
                        if (qualification != null)
                        {
                            if (qualification.IsVerified == false)
                            {
                                qualification.IsVerified = false;
                            }
                            else
                            {
                                qualification.IsVerified = obj.IsVerified;
                            }
                            qualification.VerifiedDatetime = DateTime.UtcNow.AddHours(5);
                            qualification.VerifiedBy = User.Identity.GetUserName();
                            qualification.VerifiedByUserId = User.Identity.GetUserId();
                            db.Entry(qualification).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    if (obj.ExperienceId != null && obj.ExperienceId > 0)
                    {
                        var experience = db.AdhocApplicantExperiences.FirstOrDefault(x => x.Id == obj.ExperienceId);
                        if (experience != null)
                        {
                            var log = db.AdhocMeritVerifications.FirstOrDefault(x => x.ExperienceId == experience.Id && x.IsActive == true && x.IsVerified == true);
                            if (log == null)
                            {
                                experience.IsVerified = obj.IsVerified;
                            }
                            else
                            {
                                experience.IsVerified = true;
                            }
                            experience.VerifiedDatetime = DateTime.UtcNow.AddHours(5);
                            experience.VerifiedBy = User.Identity.GetUserName();
                            experience.VerifiedByUserId = User.Identity.GetUserId();
                            db.Entry(experience).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    if (obj.DocId != null && obj.DocId == 2)
                    {
                        var applicant = db.AdhocApplicants.FirstOrDefault(x => x.Id == obj.ApplicantId);
                        if (applicant != null)
                        {
                            var log = db.AdhocMeritVerifications.FirstOrDefault(x => x.DocId == 2 && x.IsActive == true && x.IsVerified == true);
                            if (log == null)
                            {
                                applicant.HifzVerified = obj.IsVerified;
                            }
                            else
                            {
                                applicant.HifzVerified = true;
                            }
                            applicant.HifzVerifiedDatetime = DateTime.UtcNow.AddHours(5);
                            applicant.HifzVerifiedBy = User.Identity.GetUserName();
                            applicant.HifzVerifiedUserId = User.Identity.GetUserId();
                            db.Entry(applicant).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    if (obj.DocId != null && obj.DocId == 1)
                    {

                        var applicant = db.AdhocApplicants.FirstOrDefault(x => x.Id == obj.ApplicantId);
                        if (applicant != null)
                        {
                            applicant.AgeVerified = obj.IsVerified;
                            db.Entry(applicant).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    obj.VerifiedDatetime = DateTime.UtcNow.AddHours(5);
                    obj.VerifiedBy = User.Identity.GetUserName();
                    obj.VerifiedByUserId = User.Identity.GetUserId();
                    obj.IsActive = true;
                    obj.CreatedBy = User.Identity.GetUserName();
                    obj.CreatedDate = DateTime.UtcNow.AddHours(5);
                    obj.UserId = User.Identity.GetUserId();
                    db.AdhocMeritVerifications.Add(obj);
                    db.SaveChanges();
                    return Ok(obj);
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
        [HttpGet]
        [Route("GetMeritVerification/{batchApplicationId}")]
        public IHttpActionResult GetMeritVerification(int batchApplicationId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var meritVerifications = db.AdhocMeritVerifications.Where(x => x.BatchApplicationId == batchApplicationId).ToList();
                    return Ok(meritVerifications);
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

        [HttpGet]
        [Route("GetMeritVerificationAll/{applicationId}")]
        public IHttpActionResult GetMeritVerificationAll(int applicationId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var meritVerifications = db.AdhocMeritVerifications.Where(x => x.ApplicationId == applicationId).ToList();
                    return Ok(meritVerifications);
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
        [HttpGet]
        [Route("GetAdhocApplicantPMC/{applicantId}")]
        public IHttpActionResult GetAdhocApplicantPMC(int applicantId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    AdhocApplicantPMCDto adhocApplicantPMCDto = new AdhocApplicantPMCDto();
                    adhocApplicantPMCDto.AdhocApplicantPMC = db.AdhocApplicantPMCs.FirstOrDefault(x => x.Applicant_Id == applicantId);
                    if (adhocApplicantPMCDto.AdhocApplicantPMC != null)
                    {
                        adhocApplicantPMCDto.Qualifications = db.AdhocApplicantPMCQualifications.Where(x => x.PMC_Id == adhocApplicantPMCDto.AdhocApplicantPMC.Id).ToList();
                    }
                    return Ok(adhocApplicantPMCDto);
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
        [HttpPost]
        [Route("SaveAdhocScrutinyGrievance")]
        public IHttpActionResult SaveAdhocScrutinyGrievance([FromBody] AdhocScrutiny obj)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    if (obj.Application_Id > 0)
                    {
                        obj.GrievanceAccepted = obj.IsAccepted;
                        obj.GrievanceAcceptedTime = DateTime.UtcNow.AddHours(5);
                        obj.GrievanceRemarks = obj.Remarks;
                        obj.IsActive = true;
                        obj.CreatedBy = User.Identity.GetUserName();
                        obj.CreatedDate = DateTime.UtcNow.AddHours(5);
                        obj.UserId = User.Identity.GetUserId();
                        db.AdhocScrutinies.Add(obj);
                        db.SaveChanges();

                        var dbAdhocScrutiny = db.AdhocScrutinies.FirstOrDefault(x => x.Id == obj.IsCorrectedByApplicant);
                        if (dbAdhocScrutiny != null)
                        {
                            dbAdhocScrutiny.IsCorrectedByApplicant = obj.Id;
                            db.SaveChanges();
                        }
                    }
                    return Ok(obj);
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
        [HttpPost]
        [Route("EditAdhocScrutiny")]
        public IHttpActionResult EditAdhocScrutiny([FromBody] AdhocScrutiny obj)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    if (obj.Id > 0)
                    {
                        var adhocScrutiny = db.AdhocScrutinies.FirstOrDefault(x => x.Id == obj.Id);
                        if (adhocScrutiny == null) return Ok(obj);
                        adhocScrutiny.IsAccepted = true;
                        adhocScrutiny.IsRejected = false;
                        adhocScrutiny.Reason_Id = null;
                        adhocScrutiny.Remarks = null;
                        db.Entry(adhocScrutiny).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return Ok(obj);
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
        [HttpPost]
        [Route("SaveAdhocScrutinyCommittee")]
        public IHttpActionResult SaveAdhocScrutinyCommittee([FromBody] AdhocScrutinyCommittee obj)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var adhocScrutinyCommittee = db.AdhocScrutinyCommittees.FirstOrDefault(x => x.DistrictCode.Equals(obj.DistrictCode) && x.IsActive == true);
                    if (adhocScrutinyCommittee != null)
                    {
                        adhocScrutinyCommittee.IsActive = false;
                        db.Entry(adhocScrutinyCommittee).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    if (!string.IsNullOrEmpty(obj.DistrictCode))
                    {
                        obj.IsActive = true;
                        obj.CreatedBy = User.Identity.GetUserName();
                        obj.CreateDate = DateTime.UtcNow.AddHours(5);
                        obj.UserId = User.Identity.GetUserId();
                        db.AdhocScrutinyCommittees.Add(obj);
                        db.SaveChanges();
                    }
                    return Ok(obj);
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
        [HttpGet]
        [Route("GetAdhocScrutinyCommittee/{hfmisCode}")]
        public IHttpActionResult GetAdhocScrutinyReasons(string hfmisCode)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var res = db.AdhocScrutinyCommittees.FirstOrDefault(x => x.DistrictCode.Equals(hfmisCode) && x.IsActive == true);
                    return Ok(res);
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
        [HttpGet]
        [Route("GetAdhocScrutinyReasons")]
        public IHttpActionResult GetAdhocScrutinyReasons()
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var res = db.AdhocScrutinyReasons.ToList();
                    return Ok(res);
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
        [HttpGet]
        [Route("GetAdhocApplicationApplicant/{id}/{applicationId}")]
        public IHttpActionResult GetAdhocApplicationApplicant(int id, int applicationId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var applicant = db.AdhocApplicantViews.FirstOrDefault(x => x.Id == id && x.IsActive == true);
                    var application = db.AdhocApplicationViews.FirstOrDefault(x => x.Id == applicationId && x.IsActive == true);
                    return Ok(new { applicant, application });
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
        [HttpGet]
        [Route("VerifyHifzPosition/{id}/{type}/{status}")]
        public IHttpActionResult VerifyHifzPosition(int id, int type, int status)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    if (type == 1 || type == 2)
                    {
                        var applicant = db.AdhocApplicants.FirstOrDefault(x => x.Id == id && x.IsActive == true);
                        if (applicant != null)
                        {
                            if (type == 1)
                            {
                                applicant.HifzVerified = status == 1 ? true : false;
                                applicant.HifzVerifiedBy = User.Identity.GetUserName();
                                applicant.HifzVerifiedUserId = User.Identity.GetUserId();
                                applicant.HifzVerifiedDatetime = DateTime.UtcNow.AddHours(5);
                                db.Entry(applicant).State = EntityState.Modified;
                                db.SaveChanges();
                                return Ok(true);
                            }
                            else if (type == 2)
                            {
                                applicant.PositionVerified = status == 1 ? true : false;
                                applicant.PositionVerifiedBy = User.Identity.GetUserName();
                                applicant.PositionVerifiedUserId = User.Identity.GetUserId();
                                applicant.PositionVerifiedDatetime = DateTime.UtcNow.AddHours(5);
                                db.Entry(applicant).State = EntityState.Modified;
                                db.SaveChanges();
                                return Ok(true);
                            }
                        }
                    }
                    else if (type == 3)
                    {
                        var qualification = db.AdhocApplicantQualifications.FirstOrDefault(x => x.Id == id);
                        if (qualification != null)
                        {
                            qualification.IsVerified = status == 1 ? true : false;
                            qualification.VerifiedDatetime = DateTime.UtcNow.AddHours(5);
                            qualification.VerifiedBy = User.Identity.GetUserName();
                            qualification.VerifiedByUserId = User.Identity.GetUserId();
                            db.Entry(qualification).State = EntityState.Modified;
                            db.SaveChanges();
                            return Ok(true);
                        }
                    }

                    return Ok(false);
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
        [HttpPost]
        [Route("GetAdhocJobApplications")]
        public IHttpActionResult GetAdhocJobApplications([FromBody] AdhocApplicaitionFilters filter)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var preferenceIds = db.AdhocApplicationPreferenceViews.Where(x => x.HFMISCode.StartsWith(filter.hfmisCode) && x.JobApplication_Id != null && x.IsActive == true).GroupBy(x => new { x.JobApplication_Id }).Select(x => new AdhocApplicantIds
                    {
                        Id = (int)x.Key.JobApplication_Id
                    }).ToList();
                    var preferenceId = preferenceIds.Select(x => x.Id).ToList();
                    var query = db.AdhocApplicationViews.AsQueryable();
                    if (filter.hfmisCode == "0")
                    {
                        query = query.Where(x => x.IsActive == true).AsQueryable();
                    }
                    else
                    {
                        query = query.Where(x => preferenceId.Contains(x.Id) && x.IsActive == true).AsQueryable();
                    }

                    if (filter.Designation_Id > 0)
                    {
                        query = query.Where(x => x.Designation_Id == filter.Designation_Id);
                    }
                    if (filter.Status_Id > 0)
                    {
                        query = query.Where(x => x.Status_Id == filter.Status_Id);
                    }
                    if (filter.Query != null && filter.Query.Length >= 2)
                    {
                        filter.Skip = 0;
                        var isNumber = int.TryParse(filter.Query.Replace(" ", ""), out int tracking);

                        if (new RootService().IsCNIC(filter.Query.Replace(" ", "")))
                        {
                            filter.Query = filter.Query.Replace("-", "");
                            query = query.Where(x => x.CNIC.Equals(filter.Query)).AsQueryable();
                        }
                        else if (isNumber)
                        {
                            var number = Convert.ToInt32(filter.Query);
                            query = query.Where(x => x.Id == number).AsQueryable();
                        }
                        else
                        {
                            query = query.Where(x => x.Name.ToLower().Contains(filter.Query.ToLower())).AsQueryable();
                        }
                    }

                    var applications = query.OrderByDescending(l => l.CreatedDate).Skip(filter.Skip).Take(filter.PageSize).ToList();
                    var totalRecords = query.Count();
                    return Ok(new TableResponse<AdhocApplicationView>() { List = applications, Count = totalRecords });
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

        [HttpPost]
        [Route("GetAdhocJobInterviews")]
        public IHttpActionResult GetAdhocJobInterviews([FromBody] AdhocApplicaitionFilters filter)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var query = db.AdhocInterviewViews.Where(x => x.IsActive == true).AsQueryable();

                    if (!string.IsNullOrEmpty(filter.hfmisCode))
                    {
                        query = query.Where(x => x.DistrictCode.StartsWith(filter.hfmisCode) && x.IsActive == true).AsQueryable();
                    }
                    if (filter.Designation_Id > 0)
                    {
                        query = query.Where(x => x.Designation_Id == filter.Designation_Id);
                    }
                    if (filter.Status_Id > 0)
                    {
                        query = query.Where(x => x.Status_Id == filter.Status_Id);
                    }
                    var applications = query.OrderByDescending(l => l.CreatedDate).Skip(filter.Skip).Take(filter.PageSize).ToList();
                    var totalRecords = query.Count();
                    return Ok(new TableResponse<AdhocInterviewView>() { List = applications, Count = totalRecords });
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
        [HttpPost]
        [Route("GetAdhocApplicationScrutiny")]
        public IHttpActionResult GetAdhocApplicationScrutiny([FromBody] AdhocApplicaitionFilters filter)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var preferenceIds = db.AdhocApplicationPreferenceViews.Where(x => x.HFMISCode.StartsWith(filter.hfmisCode) && x.JobApplication_Id != null && x.IsActive == true).GroupBy(x => new { x.JobApplication_Id }).Select(x => new AdhocApplicantIds
                    {
                        Id = (int)x.Key.JobApplication_Id
                    }).ToList();
                    var preferenceId = preferenceIds.Select(x => x.Id).ToList();
                    var query = db.AdhocApplicationViews.AsQueryable();
                    if (string.IsNullOrEmpty(filter.hfmisCode))
                    {
                        query = query.Where(x => x.IsActive == true).AsQueryable();
                    }
                    else
                    {
                        query = query.Where(x => preferenceId.Contains(x.Id) && x.IsActive == true).AsQueryable();
                    }

                    if (filter.Designation_Id > 0)
                    {
                        query = query.Where(x => x.Designation_Id == filter.Designation_Id);
                    }
                    if (filter.Status_Id > 0 && filter.Status_Id != 100)
                    {
                        query = query.Where(x => x.Status_Id == filter.Status_Id);
                    }
                    else if (filter.Status_Id == 100)
                    {
                        query = query.Where(x => x.Status_Id == 2 || x.Status_Id == 3);
                    }
                    else
                    {
                        query = query.Where(x => x.Status_Id != 1);
                    }
                    if (filter.Query != null && filter.Query.Length >= 2)
                    {
                        filter.Skip = 0;
                        var isNumber = int.TryParse(filter.Query.Replace(" ", ""), out int tracking);

                        if (new RootService().IsCNIC(filter.Query.Replace(" ", "")))
                        {
                            filter.Query = filter.Query.Replace("-", "");
                            query = query.Where(x => x.CNIC.Equals(filter.Query)).AsQueryable();
                        }
                        else if (isNumber)
                        {
                            var number = Convert.ToInt32(filter.Query);
                            query = query.Where(x => x.Id == number).AsQueryable();
                        }
                        else
                        {
                            query = query.Where(x => x.Name.ToLower().Contains(filter.Query.ToLower())).AsQueryable();
                        }
                    }

                    var applications = query.OrderByDescending(l => l.CreatedDate).Skip(filter.Skip).Take(filter.PageSize).ToList();
                    var totalRecords = query.Count();
                    return Ok(new TableResponse<AdhocApplicationView>() { List = applications, Count = totalRecords });
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
        [HttpPost]
        [Route("GetAdhocApplicationScrutinyPrint")]
        public IHttpActionResult GetAdhocApplicationScrutinyPrint([FromBody] AdhocApplicaitionFilters filter)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var preferenceIds = db.AdhocApplicationPreferenceViews.Where(x => x.HFMISCode.StartsWith(filter.hfmisCode) && x.JobApplication_Id != null && x.IsActive == true).GroupBy(x => new { x.JobApplication_Id }).Select(x => new AdhocApplicantIds
                    {
                        Id = (int)x.Key.JobApplication_Id
                    }).ToList();
                    var preferenceId = preferenceIds.Select(x => x.Id).ToList();
                    var query = db.AdhocApplicationViews.AsQueryable();
                    var report = new List<AdhocStatusGroupDto>();
                    var totalReport = new
                    {
                        Approved = 0,
                        Rejected = 0,
                        Submitted = 0
                    };
                    if (string.IsNullOrEmpty(filter.hfmisCode))
                    {
                        query = query.Where(x => x.IsActive == true).AsQueryable();
                    }
                    else
                    {
                        query = query.Where(x => preferenceId.Contains(x.Id) && x.IsActive == true).AsQueryable();
                    }

                    if (filter.Designation_Id > 0)
                    {
                        query = query.Where(x => x.Designation_Id == filter.Designation_Id);
                    }
                    if (filter.Status_Id > 0 && filter.Status_Id != 100)
                    {
                        query = query.Where(x => x.Status_Id == filter.Status_Id);
                    }
                    else if (filter.Status_Id == 100)
                    {
                        var queryTotal = query.AsQueryable();
                        report = queryTotal.Where(x => x.Status_Id == 4 || x.Status_Id == 2 || x.Status_Id == 3).GroupBy(x => new { x.Status_Id, x.StatusName }).Select(x => new AdhocStatusGroupDto
                        {
                            StatusId = (int)x.Key.Status_Id,
                            Name = x.Key.StatusName,
                            Total = x.Count()
                        }).ToList();
                        totalReport = new
                        {
                            Approved = queryTotal.Count(x => x.Status_Id == 2 && x.AcceptedByGrievance == null),
                            Rejected = queryTotal.Count(x => (x.Status_Id == 2 && x.AcceptedByGrievance == true) || x.Status_Id == 3),
                            Submitted = queryTotal.Count(x => x.Status_Id == 4)
                        };
                        query = query.Where(x => x.Status_Id == 2 || x.Status_Id == 3);
                    }
                    else
                    {
                        query = query.Where(x => x.Status_Id != 1);
                    }
                    if (filter.Query != null && filter.Query.Length >= 2)
                    {
                        filter.Skip = 0;
                        var isNumber = int.TryParse(filter.Query.Replace(" ", ""), out int tracking);

                        if (new RootService().IsCNIC(filter.Query.Replace(" ", "")))
                        {
                            filter.Query = filter.Query.Replace("-", "");
                            query = query.Where(x => x.CNIC.Equals(filter.Query)).AsQueryable();
                        }
                        else if (isNumber)
                        {
                            var number = Convert.ToInt32(filter.Query);
                            query = query.Where(x => x.Id == number).AsQueryable();
                        }
                        else
                        {
                            query = query.Where(x => x.Name.ToLower().Contains(filter.Query.ToLower())).AsQueryable();
                        }
                    }

                    var applications = query.OrderByDescending(l => l.MasterMarks).ToList();
                    var applicationIds = applications.Select(x => x.Id).ToList();
                    var adhocScrutinies = db.AdhocScrutinyViews.Where(x => applicationIds.Contains((int)x.Application_Id) && x.IsActive == true && x.Experience_Id == null && x.DocName != "Hafiz-e-Quran" && x.IsRejected == true).ToList();
                    return Ok(new { applications, report, totalReport, adhocScrutinies });
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
        [HttpPost]
        [Route("GetAdhocApplicationGrievanceScrutinyPrint")]
        public IHttpActionResult GetAdhocApplicationGrievanceScrutinyPrint([FromBody] AdhocApplicaitionFilters filter)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var preferenceIds = db.AdhocApplicationPreferenceViews.Where(x => x.HFMISCode.StartsWith(filter.hfmisCode) && x.JobApplication_Id != null && x.IsActive == true).GroupBy(x => new { x.JobApplication_Id }).Select(x => new AdhocApplicantIds
                    {
                        Id = (int)x.Key.JobApplication_Id
                    }).ToList();
                    var preferenceId = preferenceIds.Select(x => x.Id).ToList();
                    var query = db.AdhocApplicationViews.AsQueryable();
                    var report = new List<AdhocStatusGroupDto>();
                    var totalReport = new
                    {
                        Approved = 0,
                        Rejected = 0,
                        Submitted = 0
                    };
                    if (string.IsNullOrEmpty(filter.hfmisCode))
                    {
                        query = query.Where(x => x.IsActive == true).AsQueryable();
                    }
                    else
                    {
                        query = query.Where(x => preferenceId.Contains(x.Id) && x.IsActive == true).AsQueryable();
                    }
                    var applicantIds = db.AdhocGreivanceUploads.Where(x => x.IsActive == true).GroupBy(x => new { x.Applicant_Id }).Select(x => new AdhocApplicantIds
                    {
                        Id = (int)x.Key.Applicant_Id
                    }).ToList();
                    var applicantId = applicantIds.Select(x => x.Id).ToList();
                    query = query.Where(x => applicantId.Contains((int)x.Applicant_Id) && x.IsActive == true).AsQueryable();

                    if (filter.Designation_Id > 0)
                    {
                        query = query.Where(x => x.Designation_Id == filter.Designation_Id);
                    }
                    if (filter.Status_Id > 0 && filter.Status_Id != 100)
                    {
                        query = query.Where(x => x.Status_Id == filter.Status_Id);
                    }
                    else if (filter.Status_Id == 100)
                    {
                        var queryTotal = query.AsQueryable();
                        report = queryTotal.Where(x => x.Status_Id == 4 || x.Status_Id == 2 || x.Status_Id == 3).GroupBy(x => new { x.Status_Id, x.StatusName }).Select(x => new AdhocStatusGroupDto
                        {
                            StatusId = (int)x.Key.Status_Id,
                            Name = x.Key.StatusName,
                            Total = x.Count()
                        }).ToList();
                        totalReport = new
                        {
                            Approved = queryTotal.Count(x => x.Status_Id == 2),
                            Rejected = queryTotal.Count(x => x.Status_Id == 3),
                            Submitted = queryTotal.Count(x => x.Status_Id == 4)
                        };
                        query = query.Where(x => x.Status_Id == 2 || x.Status_Id == 3);
                    }
                    else
                    {
                        query = query.Where(x => x.Status_Id != 1);
                    }
                    if (filter.Query != null && filter.Query.Length >= 2)
                    {
                        filter.Skip = 0;
                        var isNumber = int.TryParse(filter.Query.Replace(" ", ""), out int tracking);

                        if (new RootService().IsCNIC(filter.Query.Replace(" ", "")))
                        {
                            filter.Query = filter.Query.Replace("-", "");
                            query = query.Where(x => x.CNIC.Equals(filter.Query)).AsQueryable();
                        }
                        else if (isNumber)
                        {
                            var number = Convert.ToInt32(filter.Query);
                            query = query.Where(x => x.Id == number).AsQueryable();
                        }
                        else
                        {
                            query = query.Where(x => x.Name.ToLower().Contains(filter.Query.ToLower())).AsQueryable();
                        }
                    }

                    var applications = query.OrderByDescending(l => l.MasterMarks).ToList();
                    var applicationIds = applications.Select(x => x.Id).ToList();
                    var adhocScrutinies = db.AdhocScrutinyViews.Where(x => applicationIds.Contains((int)x.Application_Id) && x.IsActive == true && x.Experience_Id == null && x.DocName != "Hafiz-e-Quran" && x.IsRejected == true).ToList();
                    return Ok(new { applications, report, totalReport, adhocScrutinies });
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

        [HttpPost]
        [Route("GetAdhocAcceptedGrievancePrint")]
        public IHttpActionResult GetAdhocAcceptedGrievancePrint([FromBody] AdhocApplicaitionFilters filter)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var preferenceIds = db.AdhocApplicationGrievances.Where(x => (x.StatusId == 2 || x.StatusId == 3) && x.IsActive == true).GroupBy(x => new { x.Application_Id }).Select(x => new AdhocApplicantIds
                    {
                        Id = (int)x.Key.Application_Id
                    }).ToList();
                    var preferenceId = preferenceIds.Select(x => x.Id).ToList();
                    var query = db.AdhocApplicationViews.AsQueryable();
                    var report = new List<AdhocStatusGroupDto>();
                    if (string.IsNullOrEmpty(filter.hfmisCode))
                    {
                        query = query.Where(x => x.IsActive == true).AsQueryable();
                    }
                    else
                    {
                        query = query.Where(x => preferenceId.Contains(x.Id) && x.IsActive == true).AsQueryable();
                    }

                    if (filter.Designation_Id > 0)
                    {
                        query = query.Where(x => x.Designation_Id == filter.Designation_Id);
                    }
                    if (filter.Status_Id > 0 && filter.Status_Id != 100)
                    {
                        query = query.Where(x => x.Status_Id == filter.Status_Id);
                    }
                    else if (filter.Status_Id == 100)
                    {
                        var queryTotal = query.AsQueryable();
                        report = queryTotal.Where(x => x.Status_Id == 4 || x.Status_Id == 2 || x.Status_Id == 3).GroupBy(x => new { x.Status_Id, x.StatusName }).Select(x => new AdhocStatusGroupDto
                        {
                            StatusId = (int)x.Key.Status_Id,
                            Name = x.Key.StatusName,
                            Total = x.Count()
                        }).ToList();

                        query = query.Where(x => x.Status_Id == 2 || x.Status_Id == 3);
                    }
                    else
                    {
                        query = query.Where(x => x.Status_Id != 1);
                    }
                    if (filter.Query != null && filter.Query.Length >= 2)
                    {
                        filter.Skip = 0;
                        var isNumber = int.TryParse(filter.Query.Replace(" ", ""), out int tracking);

                        if (new RootService().IsCNIC(filter.Query.Replace(" ", "")))
                        {
                            filter.Query = filter.Query.Replace("-", "");
                            query = query.Where(x => x.CNIC.Equals(filter.Query)).AsQueryable();
                        }
                        else if (isNumber)
                        {
                            var number = Convert.ToInt32(filter.Query);
                            query = query.Where(x => x.Id == number).AsQueryable();
                        }
                        else
                        {
                            query = query.Where(x => x.Name.ToLower().Contains(filter.Query.ToLower())).AsQueryable();
                        }
                    }

                    var applications = query.OrderByDescending(l => l.MasterMarks).ToList();
                    var applicationIds = applications.Select(x => x.Id).ToList();
                    var adhocScrutinies = db.AdhocScrutinyViews.Where(x => applicationIds.Contains((int)x.Application_Id) && x.IsActive == true && x.Experience_Id == null && x.DocName != "Hafiz-e-Quran" && x.IsRejected == true).ToList();
                    return Ok(new { applications, report, adhocScrutinies });
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
        [HttpPost]
        [Route("GetAdhocApplicationGrievances")]
        public IHttpActionResult GetAdhocApplicationGrievances([FromBody] AdhocApplicaitionFilters filter)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var preferenceIds = db.AdhocApplicationPreferenceViews.Where(x => x.HFMISCode.StartsWith(filter.hfmisCode) && x.JobApplication_Id != null && x.IsActive == true).GroupBy(x => new { x.JobApplication_Id }).Select(x => new AdhocApplicantIds
                    {
                        Id = (int)x.Key.JobApplication_Id
                    }).ToList();
                    var preferenceId = preferenceIds.Select(x => x.Id).ToList();
                    var query = db.AdhocApplicationGrievanceVs.AsQueryable();
                    if (string.IsNullOrEmpty(filter.hfmisCode))
                    {
                        query = query.Where(x => x.IsActive == true).AsQueryable();
                    }
                    else
                    {
                        query = query.Where(x => preferenceId.Contains((int)x.Application_Id) && x.IsActive == true).AsQueryable();
                    }

                    var applicantIds = db.AdhocGreivanceUploads.Where(x => x.IsActive == true).GroupBy(x => new { x.Applicant_Id }).Select(x => new AdhocApplicantIds
                    {
                        Id = (int)x.Key.Applicant_Id
                    }).ToList();
                    var applicantId = applicantIds.Select(x => x.Id).ToList();
                    query = query.Where(x => applicantId.Contains((int)x.Applicant_Id) && x.IsActive == true).AsQueryable();

                    if (filter.Designation_Id > 0)
                    {
                        query = query.Where(x => x.Designation_Id == filter.Designation_Id);
                    }
                    if (filter.Status_Id > 0)
                    {
                        query = query.Where(x => x.StatusId == filter.Status_Id);
                    }

                    if (!string.IsNullOrEmpty(filter.Query))
                    {
                        filter.Skip = 0;
                        var isNumber = int.TryParse(filter.Query.Replace(" ", ""), out int tracking);

                        if (new RootService().IsCNIC(filter.Query.Replace(" ", "")))
                        {
                            filter.Skip = 0;
                            filter.Query = filter.Query.Replace("-", "");
                            query = query.Where(x => x.CNIC.Equals(filter.Query)).AsQueryable();
                        }
                        else if (isNumber)
                        {
                            filter.Skip = 0;
                            var number = Convert.ToInt32(filter.Query);
                            query = query.Where(x => x.Application_Id == number).AsQueryable();
                        }
                        else
                        {
                            filter.Skip = 0;
                            query = query.Where(x => x.Remarks.ToLower().Contains(filter.Query.ToLower())).AsQueryable();
                        }
                    }
                    List<AdhocStatusGroupDto> report = query.GroupBy(x => new { x.StatusId }).Select(x => new AdhocStatusGroupDto
                    {
                        StatusId = (int)x.Key.StatusId,
                        Total = x.Count()
                    }).ToList();
                    var applications = query.OrderByDescending(l => l.CreatedDate).Skip(filter.Skip).Take(filter.PageSize).ToList();
                    var totalRecords = query.Count();
                    return Ok(new { table = new TableResponse<AdhocApplicationGrievanceV>() { List = applications, Count = totalRecords }, report });
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
        [HttpGet]
        [Route("GetAdhocApplicantsSummary")]
        public IHttpActionResult GetAdhocApplicantsSummary()
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var summary = db.AdhocApplicantsSummary().ToList();
                    return Ok(summary);
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
        [HttpGet]
        [Route("GetAdhocApplications/{applicantId}")]
        public IHttpActionResult GetAdhocApplications(int applicantId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var applications = db.AdhocApplicationViews.Where(x => x.Applicant_Id == applicantId && (x.Status_Id == 2 || x.Status_Id == 3 || x.Status_Id == 4) && x.IsActive == true).ToList();
                    return Ok(applications);
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
        [HttpGet]
        [Route("GetAdhocGrievanceUploads/{applicantId}")]
        public IHttpActionResult GetAdhocGrievanceUploads(int applicantId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var adhocGreivanceUploads = db.AdhocGreivanceUploads.Where(x => x.Applicant_Id == applicantId && x.IsActive == true).ToList();
                    return Ok(adhocGreivanceUploads);
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
        [HttpGet]
        [Route("ScrutinyDocVerification/{applicationId}")]
        public IHttpActionResult ScrutinyDocVerification(int applicationId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    List<AdhocScrutiny> modifyScrutiny = new List<AdhocScrutiny>();
                    var application = db.AdhocApplications.FirstOrDefault(x => x.Id == applicationId && x.IsActive == true);
                    if (application == null)
                    {
                        return Ok("Application not found");
                    }
                    else
                    {
                        if (application.Status_Id == 2)
                        {
                            var qualifications = db.AdhocApplicantQualifications.Where(x => x.Applicant_Id == application.Applicant_Id && x.IsActive == true).ToList();
                            var degrees = db.Degrees.Where(x => x.IsActive == true);
                            var applicantScrutiny = db.AdhocScrutinies.Where(x => x.Application_Id == applicationId && x.IsActive == true).ToList();
                            List<int> temp = new List<int>();
                            foreach (var qual in qualifications)
                            {
                                var q = applicantScrutiny.Where(x => x.Qualification_Id == qual.Id && x.IsActive == true).ToList();
                                if (q.Count == 0)
                                {
                                    temp.Add(qual.Id);
                                }
                                else
                                {
                                    if (q.Count > 1)
                                    {
                                        var takes = q.Count - 1;
                                        var qulificationScrutinylist = q.Skip(1).Take(takes).ToList();
                                        modifyScrutiny.AddRange(qulificationScrutinylist);
                                    }

                                }
                            }

                            var scrutinyDomCount = applicantScrutiny.Where(x => x.DocName == "Domicile" && x.IsActive == true).ToList();
                            var scrutinyPmcCount = applicantScrutiny.Where(x => x.DocName == "PMC" && x.IsActive == true).ToList();
                            var scrutinyHifzCount = applicantScrutiny.Where(x => x.DocName == "Hafiz-e-Quran" && x.IsActive == true).ToList();
                            if (scrutinyDomCount.Count > 1)
                            {
                                var takes = scrutinyDomCount.Count - 1;
                                var skip = 1;
                                var scrutinyDomTake = scrutinyDomCount.Skip(skip).Take(takes).ToList();
                                modifyScrutiny.AddRange(scrutinyDomTake);
                            }
                            if (scrutinyPmcCount.Count > 1)
                            {
                                var takes = scrutinyPmcCount.Count - 1;
                                var skip = 1;
                                var scrutinyPmcTake = scrutinyPmcCount.Skip(skip).Take(takes).ToList();
                                modifyScrutiny.AddRange(scrutinyPmcTake);
                            }
                            if (scrutinyHifzCount.Count > 1)
                            {
                                var takes = scrutinyHifzCount.Count - 1;
                                var skip = 1;
                                var scrutinyHifzTake = scrutinyHifzCount.Skip(skip).Take(takes).ToList();
                                modifyScrutiny.AddRange(scrutinyHifzCount);
                            }



                        }
                    }
                    return Ok("");
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
        [HttpPost]
        [Route("GetApprovedJobApplications")]
        public IHttpActionResult GetApprovedJobApplications([FromBody] AdhocApplicaitionFilters filters)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    var preferences = db.AdhocApplicationPreferenceViews.Where(x => x.HFMISCode.Substring(0, 6) == filters.hfmisCode && x.IsActive == true).ToList();
                    var preferenceIds = preferences.Select(x => x.JobApplication_Id).ToList();
                    var applications = db.AdhocApplicationViews.Where(x => preferenceIds.Contains(x.Id) && x.IsActive == true).GroupBy(x => new { x.Applicant_Id }).Select(x => new AdhocApplicantIds
                    {
                        Id = (int)x.Key.Applicant_Id

                    }).ToList();
                    var applicantIds = applications.Select(x => x.Id).ToList();
                    var applicantsQuery = db.AdhocApplicantViews.Where(x => applicantIds.Contains(x.Id) && x.Status_Id == 2 && x.IsActive == true).AsQueryable();
                    var applicants = applicantsQuery.OrderBy(l => l.Name).Skip(filters.Skip).Take(filters.PageSize).ToList();
                    var totalRecords = applicantsQuery.Count();
                    return Ok(new { applications, applicants, totalRecords });
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
        [HttpPost]
        [Route("GetJobApplicants")]
        public IHttpActionResult GetJobApplicants([FromBody] JobApplicaitionFilters filters)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var applicantsQuery = db.AdhocApplicantViews.Where(x => x.IsActive == true).AsQueryable();
                    var applicants = applicantsQuery.OrderBy(l => l.Name).Skip(filters.Skip).Take(filters.PageSize).ToList();
                    var totalRecords = applicantsQuery.Count();
                    return Ok(new TableResponse<AdhocApplicantView> { List = applicants, Count = totalRecords });
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
        [HttpPost]
        [Route("EditAdhocApplicant")]
        public IHttpActionResult EditAdhocApplicant([FromBody] AdhocApplicant adhocApplicant)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var applicant = db.AdhocApplicants.FirstOrDefault(x => x.CNIC.Equals(adhocApplicant.CNIC));
                    applicant.MobileNumber = adhocApplicant.MobileNumber;
                    applicant.Email = adhocApplicant.Email;
                    db.SaveChanges();

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
        [HttpGet]
        [Route("EqualizeExperienceMeritVerifications")]
        public IHttpActionResult EqualizeExperienceMeritVerifications()
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var experiences = db.AdhocApplicantExperiences.Where(x => x.IsVerified != false && !x.JobTitle.ToLower().Contains("house") && !x.Organization.ToLower().Contains("house") && !x.JobTitle.ToLower().Contains("grad") && !x.Organization.ToLower().Contains("grad") && !x.JobTitle.ToLower().Contains("pgr") && !x.Organization.ToLower().Contains("pgr") && !x.JobTitle.ToLower().Contains("train") && !x.Organization.ToLower().Contains("train") && x.IsActive == true).ToList();

                    foreach (var experience in experiences)
                    {

                        var verified = db.AdhocMeritVerifications.Where(x => x.ExperienceId == experience.Id && x.IsActive == true && x.IsVerified == true).Count();
                        var notVerified = db.AdhocMeritVerifications.Where(x => x.ExperienceId == experience.Id && x.IsActive == true && x.IsVerified == false).Count();
                        if (verified == 0 && notVerified == 0)
                        {
                        }
                        else if (verified >= notVerified)
                        {
                            experience.IsVerified = true;
                            db.SaveChanges();
                        }
                        else if (verified < notVerified)
                        {
                            experience.IsVerified = false;
                            db.SaveChanges();
                        }
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
        [HttpPost]
        [Route("ChangeApplicationGrievanceStatus")]
        public IHttpActionResult ChangeApplicationGrievenceStatus([FromBody] AdhocApplicationGrievance obj)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var dbGrievance = db.AdhocApplicationGrievances.FirstOrDefault(x => x.Id == obj.Id);
                    if (dbGrievance == null) return Ok(false);
                    dbGrievance.StatusId = obj.StatusId;
                    dbGrievance.StatusChangedTime = DateTime.UtcNow.AddHours(5);
                    dbGrievance.GrievanceAddressed = true;
                    db.SaveChanges();

                    var application = db.AdhocApplications.FirstOrDefault(x => x.Id == obj.Application_Id);
                    if (application != null)
                    {

                        var applicant = db.AdhocApplicants.FirstOrDefault(x => x.Id == application.Applicant_Id);
                        if (obj.StatusId == 2)
                        {
                            application.Status_Id = obj.StatusId;
                            application.GrievanceStatus = obj.StatusId;
                            application.AcceptedByGrievance = true;
                            application.GrievanceDateTime = DateTime.UtcNow.AddHours(5);
                            application.GrievanceRemarks = obj.Remarks;

                            string MessageBody = "";
                            MessageBody = @"Dear Applicant, Your application has been accepted by the District Scrutiny Committee, you are now eligible for interview\n\nRegards,\nHealth Information and Service Delivery Unit \nPrimary and Secondary Healthcare Department";

                            if (!string.IsNullOrEmpty(applicant.MobileNumber))
                            {
                                SMS sms1 = new SMS()
                                {
                                    UserId = User.Identity.GetUserId(),
                                    FKId = application.Id,
                                    MobileNumber = applicant.MobileNumber,
                                    //MobileNumber = "03214677763",
                                    Message = MessageBody
                                };
                                Common.SendSMSTelenor(sms1);
                            }
                        }
                        else if (obj.StatusId == 3)
                        {
                            string MessageBody = "";
                            MessageBody = @"Dear Applicant, Your application number " + application.Id.ToString() + " has been rejected by the District Scrutiny Committee";

                            application.GrievanceStatus = obj.StatusId;
                            application.AcceptedByGrievance = false;
                            application.GrievanceDateTime = DateTime.UtcNow.AddHours(5);
                            application.GrievanceRemarks = obj.Remarks;

                            MessageBody += "\n\nRemarks:\n";
                            MessageBody += obj.Remarks;

                            if (!string.IsNullOrEmpty(applicant.MobileNumber))
                            {
                                SMS sms1 = new SMS()
                                {
                                    UserId = User.Identity.GetUserId(),
                                    FKId = application.Id,
                                    MobileNumber = applicant.MobileNumber,
                                    //MobileNumber = "03214677763",
                                    Message = MessageBody
                                };
                                Common.SendSMSTelenor(sms1);
                            }
                        }
                        else if (obj.StatusId == 5)
                        {
                            string MessageBody = "";
                            MessageBody = @"Dear Applicant, Your application number " + application.Id.ToString() + " has been rejected by the District Scrutiny Committee";

                            application.GrievanceStatus = obj.StatusId;
                            application.AcceptedByGrievance = false;
                            application.GrievanceDateTime = DateTime.UtcNow.AddHours(5);
                            application.GrievanceRemarks = obj.Remarks;


                            if (!string.IsNullOrEmpty(applicant.MobileNumber))
                            {
                                SMS sms1 = new SMS()
                                {
                                    UserId = User.Identity.GetUserId(),
                                    FKId = application.Id,
                                    MobileNumber = applicant.MobileNumber,
                                    //MobileNumber = "03214677763",
                                    Message = MessageBody
                                };
                                Common.SendSMSTelenor(sms1);
                            }
                        }
                        db.Entry(application).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return Ok(obj);
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;

                }
                return BadRequest(ex.ToString());
            }
        }
        [HttpPost]
        [Route("ChangeApplicationGrievanceScrutinyStatus")]
        public IHttpActionResult ChangeApplicationGrievanceScrutinyStatus([FromBody] AdhocApplicationGrievance obj)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var dbGrievance = db.AdhocApplicationGrievances.FirstOrDefault(x => x.Id == obj.Id);
                    if (dbGrievance == null) return Ok(false);
                    dbGrievance.StatusId = obj.StatusId;
                    dbGrievance.StatusChangedTime = DateTime.UtcNow.AddHours(5);
                    dbGrievance.GrievanceAddressed = true;
                    db.SaveChanges();

                    var application = db.AdhocApplications.FirstOrDefault(x => x.Id == obj.Application_Id);
                    if (application != null)
                    {
                        var applicant = db.AdhocApplicants.FirstOrDefault(x => x.Id == application.Applicant_Id);
                        if (obj.StatusId == 2)
                        {
                            application.Status_Id = obj.StatusId;
                            string userId = User.Identity.GetUserId();
                            var user = db.C_User.FirstOrDefault(x => x.Id.Equals(userId));
                            if (user != null)
                            {
                                var district = db.Districts.FirstOrDefault(x => x.Code.Equals(user.DistrictID));
                                if (district != null)
                                {
                                    application.StatusChangedByDistrict = district.Name;
                                }
                            }
                            application.StatusChangedBy = obj.CreatedBy;
                            application.StatusChangedByUserId = obj.UserId;
                            application.StatusChangedDateTime = dbGrievance.StatusChangedTime;
                            string MessageBody = "";
                            MessageBody = @"Dear Applicant, Your application has been accepted by district scrutiny committee, you are now eligible for interview\n\nRegards,\nHealth Information and Service Delivery Unit \nPrimary & Secondary Healthcare Department";

                            if (!string.IsNullOrEmpty(applicant.MobileNumber))
                            {
                                SMS sms1 = new SMS()
                                {
                                    UserId = User.Identity.GetUserId(),
                                    FKId = application.Id,
                                    MobileNumber = applicant.MobileNumber,
                                    //MobileNumber = "03214677763",
                                    Message = MessageBody
                                };
                                Common.SendSMSTelenor(sms1);
                            }
                        }
                        else if (obj.StatusId == 5)
                        {
                            string MessageBody = "";
                            MessageBody = @"Dear Applicant, Your application is rejected by district scrutiny committee due to following reasons:\n";
                            string userId = User.Identity.GetUserId();
                            var user = db.C_User.FirstOrDefault(x => x.Id.Equals(userId));
                            if (user != null)
                            {
                                var district = db.Districts.FirstOrDefault(x => x.Code.Equals(user.DistrictID));
                                if (district != null)
                                {
                                    application.StatusChangedByDistrict = district.Name;
                                }
                            }
                            application.StatusChangedBy = obj.CreatedBy;
                            application.StatusChangedByUserId = obj.UserId;
                            application.StatusChangedDateTime = dbGrievance.StatusChangedTime;
                            var reasons = db.AdhocScrutinyViews.Where(x => x.Application_Id == application.Id && x.GrievanceAccepted == false && x.IsActive == true && x.Experience_Id == null).ToList();
                            foreach (var reason in reasons)
                            {
                                MessageBody += reason.DocName + ": " + reason.ReasonDetail + "\n";
                            }
                            //MessageBody += "\nYou can file online gievance against Application Number: " + application.Id;
                            MessageBody += "\n\nRegards,\nHealth Information and Service Delivery Unit \nPrimary and Secondary Healthcare Department";

                            if (!string.IsNullOrEmpty(applicant.MobileNumber))
                            {
                                SMS sms1 = new SMS()
                                {
                                    UserId = User.Identity.GetUserId(),
                                    FKId = application.Id,
                                    MobileNumber = applicant.MobileNumber,
                                    //MobileNumber = "03214677763",
                                    Message = MessageBody
                                };
                                Common.SendSMSTelenor(sms1);
                            }
                        }
                        db.Entry(application).State = EntityState.Modified;
                        db.SaveChanges();

                        AdhocApplicationLog adhocApplicationLog = new AdhocApplicationLog();
                        adhocApplicationLog.Application_Id = obj.Application_Id;
                        adhocApplicationLog.StatusId = obj.StatusId == 2 ? 2 : 3;
                        adhocApplicationLog.IsActive = true;
                        adhocApplicationLog.Remarks = "Grievance Scrutiny";
                        adhocApplicationLog.CreatedBy = User.Identity.GetUserName();
                        adhocApplicationLog.CreatedDate = DateTime.UtcNow.AddHours(5);
                        adhocApplicationLog.UserId = User.Identity.GetUserId();
                        db.AdhocApplicationLogs.Add(adhocApplicationLog);
                        db.SaveChanges();
                    }
                    return Ok(obj);
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;

                }
                return BadRequest(ex.ToString());
            }
        }
        [HttpPost]
        [Route("AcceptAdhocGrievanceScrutiny")]
        public IHttpActionResult AcceptAdhocGrievanceScrutiny([FromBody] AdhocScrutiny adhocScrutiny)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var dbAdhocScrutiny = db.AdhocScrutinies.FirstOrDefault(x => x.Id == adhocScrutiny.Id);
                    if (dbAdhocScrutiny == null) return Ok(false);
                    dbAdhocScrutiny.GrievanceAccepted = adhocScrutiny.GrievanceAccepted;
                    dbAdhocScrutiny.GrievanceAcceptedTime = DateTime.UtcNow.AddHours(5);
                    dbAdhocScrutiny.GrievanceRemarks = adhocScrutiny.Remarks;
                    db.Entry(dbAdhocScrutiny).State = EntityState.Modified;
                    db.SaveChanges();
                    return Ok(dbAdhocScrutiny);
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;

                }
                return BadRequest(ex.ToString());
            }
        }
        [HttpGet]
        [Route("SetAdhocMerit/{interviewId}")]
        public async Task<IHttpActionResult> SetAdhocMerit(int interviewId)
        {

            try
            {
                using (var db = new HR_System())
                {
                    int c = 0;
                    db.Configuration.ProxyCreationEnabled = false;
                    var interviewBatchApplications = await db.AdhocInterviewBatchApplicationVs.Where(x => x.IsPresent == true && x.IsLocked == true && x.IsRejected != true && x.InterviewId == interviewId).ToListAsync();

                    foreach (var batchApp in interviewBatchApplications)
                    {
                        c++;
                        if (batchApp.Application_Id == 436 || batchApp.Application_Id == 10380)
                        {

                        }

                        var application = await db.AdhocApplications.FirstOrDefaultAsync(x => x.Id == batchApp.Application_Id && x.IsActive == true && x.Status_Id == 2);
                        var applicationView = await db.AdhocApplicationViews.FirstOrDefaultAsync(x => x.Id == batchApp.Application_Id && x.IsActive == true && x.Status_Id == 2);
                        if (application != null)
                        {


                            var applicantQualifications = await db.AdhocApplicantQualifications.Where(x => x.Applicant_Id == application.Applicant_Id && x.IsActive == true).ToListAsync();
                            int obtainMarks = 0;

                            List<int> consultants = new List<int>()
                                {
                                    362,365,368,369,373,374,375,381,382,383,384,385,387,390,1594,1598,2136,2313
                                };
                            if (consultants.Contains((int)application.Designation_Id))
                            {
                                if (applicationView.Age <= 50)
                                {
                                    var matchedRecords = await db.AdhocApplicationMarks.Where(e => e.Application_Id == application.Id && e.BatchApplicationId == batchApp.Id && e.IsActive == true).ToListAsync();
                                    matchedRecords.ForEach(e => e.IsActive = false);
                                    await db.SaveChangesAsync();
                                    obtainMarks = consultantMarks(db, application, batchApp, applicantQualifications);
                                }
                                else
                                {
                                    application.Status_Id = 3;
                                    var matchedRecords = await db.AdhocInterviewBatchApplications.Where(x => x.Id == batchApp.Id && x.IsActive == true).ToListAsync();
                                    matchedRecords.ForEach(e => { e.IsRejected = true; });
                                    db.Entry(application).State = EntityState.Modified;
                                    await db.SaveChangesAsync();
                                }
                            }
                            else if (application.Designation_Id == 802 || application.Designation_Id == 1320)
                            {
                                if (applicationView.Age <= 50)
                                {
                                    var matchedRecords = await db.AdhocApplicationMarks.Where(e => e.Application_Id == application.Id && e.BatchApplicationId == batchApp.Id && e.IsActive == true).ToListAsync();
                                    matchedRecords.ForEach(e => e.IsActive = false);
                                    await db.SaveChangesAsync();
                                    obtainMarks = await MedicalOfficerMarks(db, application, batchApp, applicantQualifications);
                                }
                                else
                                {
                                    application.Status_Id = 3;
                                    var matchedRecords = await db.AdhocInterviewBatchApplications.Where(x => x.Id == batchApp.Id && x.IsActive == true).ToListAsync();
                                    matchedRecords.ForEach(e => { e.IsRejected = true; db.SaveChangesAsync(); });
                                    db.Entry(application).State = EntityState.Modified;
                                    await db.SaveChangesAsync();
                                }
                            }
                            else if (application.Designation_Id == 302)
                            {
                                var matchedRecords = await db.AdhocApplicationMarks.Where(e => e.Application_Id == application.Id && e.BatchApplicationId == batchApp.Id && e.IsActive == true).ToListAsync();
                                matchedRecords.ForEach(e => e.IsActive = false);
                                await db.SaveChangesAsync();
                                obtainMarks = chargeNurseMarks(db, application, batchApp, applicantQualifications);
                            }
                            else if (application.Designation_Id == 431)
                            {
                                var matchedRecords = await db.AdhocApplicationMarks.Where(e => e.Application_Id == application.Id && e.BatchApplicationId == batchApp.Id && e.IsActive == true).ToListAsync();
                                matchedRecords.ForEach(e => e.IsActive = false);
                                await db.SaveChangesAsync();
                                obtainMarks = dentalSurgeonMarks(db, application, batchApp, applicantQualifications);
                            }


                            //var marks = db.AdhocApplicationMarks.Where(x => x.Application_Id == applicationId && x.IsActive == true).ToList();
                            //return Ok(marks);
                        }
                    }

                    //if (application != null)
                    //{
                    //    var applicantQualifications = db.AdhocApplicantQualifications.Where(x => x.Applicant_Id == application.Applicant_Id && x.IsActive == true).ToList();
                    //    int obtainMarks = 0;

                    //    List<int> consultants = new List<int>()
                    //        {
                    //            362,365,368,369,373,302,374,375,381,382,383,384,385,387,390,1594,1598,2136,2313
                    //        };
                    //    if (consultants.Contains((int)application.Designation_Id))
                    //    {
                    //        obtainMarks = consultantMarks(db, application, applicantQualifications);
                    //    }
                    //    else if (application.Designation_Id == 802 || application.Designation_Id == 1320)
                    //    {
                    //        obtainMarks = medicalOfficerMarks(db, application, applicantQualifications);
                    //    }
                    //    else if (application.Designation_Id == 302)
                    //    {
                    //        obtainMarks = chargeNurseMarks(db, application, applicantQualifications);
                    //    }
                    //    else if (application.Designation_Id == 431)
                    //    {
                    //        obtainMarks = dentalSurgeonMarks(db, application, applicantQualifications);
                    //    }
                    //    var marks = db.AdhocApplicationMarks.Where(x => x.Application_Id == applicationId && x.IsActive == true).ToList();
                    //    //return Ok(marks);
                    //}
                    return Ok(true);
                    //}
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;

                }
                return BadRequest(ex.ToString());
            }
        }
        [HttpPost]
        [Route("ChangeApplicationStatus")]
        public IHttpActionResult ChangeApplicationStatus([FromBody] AdhocApplicationLog obj)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    obj.IsActive = true;
                    obj.CreatedBy = User.Identity.GetUserName();
                    obj.CreatedDate = DateTime.UtcNow.AddHours(5);
                    obj.UserId = User.Identity.GetUserId();
                    db.AdhocApplicationLogs.Add(obj);
                    db.SaveChanges();

                    var application = db.AdhocApplications.FirstOrDefault(x => x.Id == obj.Application_Id);
                    if (application != null)
                    {
                        application.Status_Id = obj.StatusId;

                        var applicant = db.AdhocApplicants.FirstOrDefault(x => x.Id == application.Applicant_Id);
                        if (obj.StatusId == 2)
                        {
                            var applicantQualifications = db.AdhocApplicantQualificationViews.Where(x => x.Applicant_Id == applicant.Id && x.IsActive == true).ToList();

                            // int obtainMarks = getObtainedMarksApplication(db, application, applicantQualifications);
                            var user = db.C_User.FirstOrDefault(x => x.Id.Equals(obj.UserId));
                            if (user != null)
                            {
                                var district = db.Districts.FirstOrDefault(x => x.Code.Equals(user.DistrictID));
                                if (district != null)
                                {
                                    application.StatusChangedByDistrict = district.Name;
                                }
                            }
                            application.StatusChangedBy = obj.CreatedBy;
                            application.StatusChangedByUserId = obj.UserId;
                            application.StatusChangedDateTime = obj.CreatedDate;
                            string MessageBody = "";
                            MessageBody = @"Dear Applicant, Your application has been accepted by district scrutiny committee, you are now eligible for interview\n\nRegards,\nHealth Information and Service Delivery Unit \nPrimary & Secondary Healthcare Department";

                            if (!string.IsNullOrEmpty(applicant.MobileNumber))
                            {
                                SMS sms1 = new SMS()
                                {
                                    UserId = User.Identity.GetUserId(),
                                    FKId = application.Id,
                                    MobileNumber = applicant.MobileNumber,
                                    //MobileNumber = "03214677763",
                                    Message = MessageBody
                                };
                                Common.SendSMSTelenor(sms1);
                            }
                        }
                        else if (obj.StatusId == 3)
                        {
                            string MessageBody = "";
                            MessageBody = @"Dear Applicant, Your application is rejected by district scrutiny committee due to following reasons:\n";
                            var user = db.C_User.FirstOrDefault(x => x.Id.Equals(obj.UserId));
                            if (user != null)
                            {
                                var district = db.Districts.FirstOrDefault(x => x.Code.Equals(user.DistrictID));
                                if (district != null)
                                {
                                    application.StatusChangedByDistrict = district.Name;
                                }
                            }
                            application.StatusChangedBy = obj.CreatedBy;
                            application.StatusChangedByUserId = obj.UserId;
                            application.StatusChangedDateTime = obj.CreatedDate;
                            var reasons = db.AdhocScrutinyViews.Where(x => x.Application_Id == application.Id && x.IsRejected == true && x.IsActive == true && x.Experience_Id == null).ToList();
                            foreach (var reason in reasons)
                            {
                                MessageBody += reason.DocName + ": " + reason.ReasonDetail + "\n";
                            }
                            MessageBody += "\nYou can file online gievance against Application Number: " + application.Id;
                            MessageBody += "\n\nRegards,\nHealth Information and Service Delivery Unit \nPrimary & Secondary Healthcare Department";

                            if (!string.IsNullOrEmpty(applicant.MobileNumber))
                            {
                                SMS sms1 = new SMS()
                                {
                                    UserId = User.Identity.GetUserId(),
                                    FKId = application.Id,
                                    MobileNumber = applicant.MobileNumber,
                                    //MobileNumber = "03214677763",
                                    Message = MessageBody
                                };
                                Common.SendSMSTelenor(sms1);
                            }
                        }
                        db.Entry(application).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return Ok(obj);
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;

                }
                return BadRequest(ex.ToString());
            }
        }
        [HttpPost]
        [Route("SaveAdhocInterviewBatchApplication")]
        public IHttpActionResult ChangeApplicationFinalStatus([FromBody] AdhocInterviewBatchApplication obj)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var adhocInterviewBatchApplication = db.AdhocInterviewBatchApplications.FirstOrDefault(x => x.Id == obj.Id);
                    if (adhocInterviewBatchApplication != null)
                    {
                        if (obj.IsActive == false)
                        {
                            adhocInterviewBatchApplication.InterviewMarks = null;
                            adhocInterviewBatchApplication.InterviewMarksDatetime = null;
                            adhocInterviewBatchApplication.InterviewMarksByUserId = null;
                            adhocInterviewBatchApplication.InterviewMarksBy = null;
                            adhocInterviewBatchApplication.IsRejected = null;
                            adhocInterviewBatchApplication.RejectedDatetime = null;
                            adhocInterviewBatchApplication.IsPresent = null;
                            adhocInterviewBatchApplication.PresentDatetime = null;
                            adhocInterviewBatchApplication.PositionHolder = null;
                            adhocInterviewBatchApplication.PositionDoc = null;
                            var application = db.AdhocApplications.FirstOrDefault(x => x.Id == adhocInterviewBatchApplication.Application_Id);
                            if (application != null)
                            {
                                var applicant = db.AdhocApplicants.FirstOrDefault(x => x.Id == application.Applicant_Id);
                                if (applicant != null)
                                {
                                    adhocInterviewBatchApplication.Position = null;
                                    adhocInterviewBatchApplication.PositionDoc = null;
                                    applicant.Position = null;
                                    db.Entry(applicant).State = EntityState.Modified;
                                }
                            }
                            db.Entry(adhocInterviewBatchApplication).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            if (obj.IsLocked == true)
                            {
                                adhocInterviewBatchApplication.IsLocked = obj.IsLocked;
                                adhocInterviewBatchApplication.LockedDatetime = DateTime.UtcNow.AddHours(5);
                                adhocInterviewBatchApplication.LockedByUserId = User.Identity.GetUserId();
                                adhocInterviewBatchApplication.LockedBy = User.Identity.GetUserName();
                                if (adhocInterviewBatchApplication.IsRejected == true)
                                {
                                    string MessageBody = "";
                                    var user = db.C_User.FirstOrDefault(x => x.Id.Equals(obj.UserId));
                                    if (user != null)
                                    {
                                        var district = db.Districts.FirstOrDefault(x => x.Code.Equals(user.DistrictID));
                                        if (district != null)
                                        {
                                            adhocInterviewBatchApplication.InterviewMarksBy = district.Name;
                                        }
                                    }
                                    MessageBody = @"Dear Applicant, Your application for district " + adhocInterviewBatchApplication.InterviewMarksBy +
                                        " is rejected.";

                                    var application = db.AdhocApplications.FirstOrDefault(x => x.Id == adhocInterviewBatchApplication.Application_Id);
                                    if (application != null)
                                    {
                                        var applicant = db.AdhocApplicants.FirstOrDefault(x => x.Id == application.Applicant_Id);
                                        if (applicant != null)
                                        {

                                            if (!string.IsNullOrEmpty(applicant.MobileNumber))
                                            {
                                                SMS sms1 = new SMS()
                                                {
                                                    UserId = User.Identity.GetUserId(),
                                                    FKId = application.Id,
                                                    MobileNumber = applicant.MobileNumber,
                                                    //MobileNumber = "03214677763",
                                                    Message = MessageBody
                                                };
                                                Common.SendSMSTelenor(sms1);
                                            }
                                        }
                                    }
                                }
                                db.Entry(adhocInterviewBatchApplication).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                                if (obj.InterviewMarks > 0)
                                {
                                    adhocInterviewBatchApplication.InterviewMarks = obj.InterviewMarks;
                                    adhocInterviewBatchApplication.InterviewMarksDatetime = DateTime.UtcNow.AddHours(5);
                                    adhocInterviewBatchApplication.InterviewMarksByUserId = User.Identity.GetUserId();
                                    adhocInterviewBatchApplication.InterviewMarksBy = User.Identity.GetUserName();
                                }
                                if (obj.IsRejected == true)
                                {
                                    adhocInterviewBatchApplication.IsRejected = obj.IsRejected;
                                    adhocInterviewBatchApplication.RejectedDatetime = DateTime.UtcNow.AddHours(5);
                                    adhocInterviewBatchApplication.InterviewMarksByUserId = User.Identity.GetUserId();
                                    adhocInterviewBatchApplication.InterviewMarksBy = User.Identity.GetUserName();
                                }
                                if (obj.IsPresent == true || obj.IsPresent == false)
                                {
                                    adhocInterviewBatchApplication.IsPresent = obj.IsPresent;
                                    adhocInterviewBatchApplication.PresentDatetime = DateTime.UtcNow.AddHours(5);
                                }
                                if (obj.PositionHolder != null)
                                {
                                    adhocInterviewBatchApplication.PositionHolder = obj.PositionHolder;
                                    adhocInterviewBatchApplication.PositionDoc = obj.PositionDoc;
                                    var application = db.AdhocApplications.FirstOrDefault(x => x.Id == adhocInterviewBatchApplication.Application_Id);
                                    if (application != null)
                                    {
                                        var applicant = db.AdhocApplicants.FirstOrDefault(x => x.Id == application.Applicant_Id);
                                        if (applicant != null)
                                        {
                                            adhocInterviewBatchApplication.Position = obj.Position;
                                            adhocInterviewBatchApplication.PositionDoc = obj.PositionDoc;
                                            applicant.Position = adhocInterviewBatchApplication.Position;
                                            db.Entry(applicant).State = EntityState.Modified;
                                        }
                                    }
                                }
                                db.Entry(adhocInterviewBatchApplication).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }

                    }
                    return Ok(obj);
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;

                }
                return BadRequest(ex.ToString());
            }
        }
        [HttpPost]
        [Route("AddInterviewMarks")]
        public IHttpActionResult AddInterviewMarks([FromBody] StatusChange obj)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var adhocApplication = db.AdhocApplications.FirstOrDefault(x => x.Id == obj.Id);
                    if (adhocApplication != null)
                    {
                        adhocApplication.InterviewMarks = obj.InterviewViewMarks;
                        db.SaveChanges();
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
                return BadRequest(ex.ToString());
            }
        }
        [HttpGet]
        [Route("JobApplicantDocuments/{applicantId}")]
        public IHttpActionResult JobApplicantDocuments(int applicantId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var jobApplicantDocumentViews = db.AdhocApplicantDocumentViews.Where(x => x.Applicant_Id == applicantId && x.IsActive == true).ToList();
                    return Ok(jobApplicantDocumentViews);
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
        [HttpGet]
        [Route("JobApplicantExperiences/{applicantId}")]
        public IHttpActionResult JobApplicantExperiences(int applicantId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var jobApplicantExperiences = db.AdhocApplicantDocuments.Where(x => x.Applicant_Id == applicantId && x.IsActive == true).ToList();
                    return Ok(jobApplicantExperiences);
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
        [HttpGet]
        [Route("AdhocApplicantContinueExp/{experienceId}")]
        public IHttpActionResult AdhocApplicantContinueExp(int experienceId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var applicantExperience = db.AdhocApplicantExperiences.FirstOrDefault(x => x.Id == experienceId && x.IsActive == true);
                    applicantExperience.IsContinued = true;
                    applicantExperience.ToDate = null;
                    db.SaveChanges();
                    return Ok(applicantExperience);
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
        [HttpGet]
        [Route("RemoveApplication/{Id}")]
        public IHttpActionResult RemoveApplication(int Id)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var application = db.AdhocApplications.FirstOrDefault(x => x.Id == Id);
                    if (application != null)
                    {
                        application.IsActive = false;
                        db.Entry(application).State = EntityState.Modified;
                        db.SaveChanges();
                        return Ok(true);
                    }
                    return Ok(false);
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
        [HttpGet]
        [Route("GetAdhocs/{categoryId}")]
        public IHttpActionResult GetAdhocs(int categoryId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var jobs = db.AdhocJobViews.Where(x => x.OrderBy == categoryId && x.IsActive == true).ToList();
                    var designationIds = jobs.Select(x => x.Designation_Id).ToList();
                    var vps = db.VpMastProfileViews.Where(x => designationIds.Contains(x.Desg_Id)).ToList();
                    return Ok(new { jobs, vps });
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
        [HttpGet]
        [Route("GetAdhocJob/{designationId}")]
        public IHttpActionResult GetAdhocJob(int designationId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    if (designationId == 802)
                    {
                        var job = db.AdhocJobViews.FirstOrDefault(x => x.Designation_Id == designationId && x.IsActive == true);
                        var job2 = db.AdhocJobViews.FirstOrDefault(x => x.Designation_Id == 1085 && x.IsActive == true);
                        return Ok(new { job, job2 });
                    }
                    else if (designationId == 1320)
                    {
                        var job = db.AdhocJobViews.FirstOrDefault(x => x.Designation_Id == designationId && x.IsActive == true);
                        var job2 = db.AdhocJobViews.FirstOrDefault(x => x.Designation_Id == 1157 && x.IsActive == true);
                        return Ok(new { job, job2 });
                    }
                    else if (designationId == 431)
                    {
                        var job = db.AdhocJobViews.FirstOrDefault(x => x.Designation_Id == designationId && x.IsActive == true);
                        return Ok(new { job });
                    }
                    else
                    {
                        var job = db.AdhocJobViews.FirstOrDefault(x => x.Designation_Id == designationId && x.IsActive == true);
                        return Ok(new { job });
                    }

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

        [HttpPost]
        [Route("SaveAdhocInterview")]
        public IHttpActionResult SaveAdhocInterview([FromBody] AdhocInterviewDto adhocInterviewDto)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var adhocInterview = db.AdhocInterviews.FirstOrDefault(x => x.DistrictCode.Equals(adhocInterviewDto.AdhocInterview.DistrictCode) && adhocInterviewDto.AdhocInterview.Designation_Id == x.Designation_Id && x.IsActive == true);
                    if (adhocInterview != null)
                    {
                        var adhocInterviewBatch = db.AdhocInterviewBatches.FirstOrDefault(x => x.Interview_Id == adhocInterviewDto.AdhocInterview.Id && x.BatchNo.Equals(adhocInterviewDto.AdhocInterviewBatch.AdhocInterviewBatch.BatchNo) && x.IsActive == true);
                        if (adhocInterviewBatch != null)
                        {
                            foreach (var applicationId in adhocInterviewDto.AdhocInterviewBatch.ApplicationIds)
                            {
                                var adhocInterviewBatchApplication = db.AdhocInterviewBatchApplications.FirstOrDefault(x => x.Batch_Id == adhocInterviewBatch.Id && applicationId == x.Application_Id && x.IsActive == true);
                                if (adhocInterviewBatchApplication == null)
                                {
                                    adhocInterviewBatchApplication = new AdhocInterviewBatchApplication();
                                    adhocInterviewBatchApplication.Application_Id = applicationId;
                                    adhocInterviewBatchApplication.Batch_Id = adhocInterviewBatch.Id;
                                    adhocInterviewBatchApplication.IsActive = true;
                                    adhocInterviewBatchApplication.CreatedDate = DateTime.UtcNow.AddHours(5);
                                    adhocInterviewBatchApplication.UserId = User.Identity.GetUserId();
                                    adhocInterviewBatchApplication.CreatedBy = User.Identity.GetUserName();
                                    db.AdhocInterviewBatchApplications.Add(adhocInterviewBatchApplication);
                                    db.SaveChanges();
                                }
                            }
                        }
                        else
                        {
                            adhocInterviewBatch = adhocInterviewDto.AdhocInterviewBatch.AdhocInterviewBatch;
                            adhocInterviewBatch.Interview_Id = adhocInterviewDto.AdhocInterview.Id;
                            adhocInterviewBatch.IsActive = true;
                            adhocInterviewBatch.CreatedDate = DateTime.UtcNow.AddHours(5);
                            adhocInterviewBatch.UserId = User.Identity.GetUserId();
                            adhocInterviewBatch.CreatedBy = User.Identity.GetUserName();
                            db.AdhocInterviewBatches.Add(adhocInterviewBatch);
                            db.SaveChanges();
                            foreach (var applicationId in adhocInterviewDto.AdhocInterviewBatch.ApplicationIds)
                            {
                                AdhocInterviewBatchApplication adhocInterviewBatchApplication = new AdhocInterviewBatchApplication();
                                adhocInterviewBatchApplication.Application_Id = applicationId;
                                adhocInterviewBatchApplication.Batch_Id = adhocInterviewBatch.Id;
                                adhocInterviewBatchApplication.IsActive = true;
                                adhocInterviewBatchApplication.CreatedDate = DateTime.UtcNow.AddHours(5);
                                adhocInterviewBatchApplication.UserId = User.Identity.GetUserId();
                                adhocInterviewBatchApplication.CreatedBy = User.Identity.GetUserName();
                                db.AdhocInterviewBatchApplications.Add(adhocInterviewBatchApplication);
                                db.SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        adhocInterviewDto.AdhocInterview.Status_Id = 1;
                        adhocInterviewDto.AdhocInterview.IsActive = true;
                        adhocInterviewDto.AdhocInterview.CreatedDate = DateTime.UtcNow.AddHours(5);
                        adhocInterviewDto.AdhocInterview.UserId = User.Identity.GetUserId();
                        adhocInterviewDto.AdhocInterview.CreatedBy = User.Identity.GetUserName();
                        db.AdhocInterviews.Add(adhocInterviewDto.AdhocInterview);
                        db.SaveChanges();
                        var adhocInterviewBacth = db.AdhocInterviewBatches.FirstOrDefault(x => x.Interview_Id == adhocInterviewDto.AdhocInterview.Id && x.IsActive == true);
                        if (adhocInterviewBacth != null)
                        {
                        }
                        else
                        {
                            var adhocInterviewBatch = adhocInterviewDto.AdhocInterviewBatch.AdhocInterviewBatch;
                            adhocInterviewBatch.Interview_Id = adhocInterviewDto.AdhocInterview.Id;
                            adhocInterviewBatch.IsActive = true;
                            adhocInterviewBatch.CreatedDate = DateTime.UtcNow.AddHours(5);
                            adhocInterviewBatch.UserId = User.Identity.GetUserId();
                            adhocInterviewBatch.CreatedBy = User.Identity.GetUserName();
                            db.AdhocInterviewBatches.Add(adhocInterviewBatch);
                            db.SaveChanges();
                            foreach (var applicationId in adhocInterviewDto.AdhocInterviewBatch.ApplicationIds)
                            {
                                AdhocInterviewBatchApplication adhocInterviewBatchApplication = new AdhocInterviewBatchApplication();
                                adhocInterviewBatchApplication.Application_Id = applicationId;
                                adhocInterviewBatchApplication.Batch_Id = adhocInterviewBatch.Id;
                                adhocInterviewBatchApplication.IsActive = true;
                                adhocInterviewBatchApplication.CreatedDate = DateTime.UtcNow.AddHours(5);
                                adhocInterviewBatchApplication.UserId = User.Identity.GetUserId();
                                adhocInterviewBatchApplication.CreatedBy = User.Identity.GetUserName();
                                db.AdhocInterviewBatchApplications.Add(adhocInterviewBatchApplication);
                                db.SaveChanges();
                            }
                        }
                    }
                    return Ok(adhocInterviewDto);
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
        [HttpPost]
        [Route("SaveAdhocInterviewBatchCommittee")]
        public IHttpActionResult SaveAdhocInterviewBatchCommittee([FromBody] AdhocInterviewBatchCommittee adhocInterviewBatchCommittee)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    if (adhocInterviewBatchCommittee.Id == 0)
                    {
                        adhocInterviewBatchCommittee.IsActive = true;
                        adhocInterviewBatchCommittee.CreatedDate = DateTime.UtcNow.AddHours(5);
                        adhocInterviewBatchCommittee.UserId = User.Identity.GetUserId();
                        adhocInterviewBatchCommittee.CreatedBy = "1st: " + User.Identity.GetUserName();
                        db.AdhocInterviewBatchCommittees.Add(adhocInterviewBatchCommittee);
                        db.SaveChanges();
                    }
                    else
                    {
                        var adhocInterviewBatchCommitteeDb = db.AdhocInterviewBatchCommittees.FirstOrDefault(x => x.Id == adhocInterviewBatchCommittee.Id && x.IsActive == true);
                        if (adhocInterviewBatchCommitteeDb != null)
                        {
                            adhocInterviewBatchCommitteeDb.Name = adhocInterviewBatchCommittee.Name;
                            adhocInterviewBatchCommitteeDb.Designation = adhocInterviewBatchCommittee.Designation;
                            adhocInterviewBatchCommitteeDb.Role = adhocInterviewBatchCommittee.Role;
                            adhocInterviewBatchCommitteeDb.Office = adhocInterviewBatchCommittee.Office;
                            adhocInterviewBatchCommitteeDb.IsActive = adhocInterviewBatchCommittee.IsActive;
                            adhocInterviewBatchCommitteeDb.CreatedDate = DateTime.UtcNow.AddHours(5);
                            adhocInterviewBatchCommitteeDb.CreatedBy += ", Then by: " + User.Identity.GetUserName() + " @ " + DateTime.UtcNow.AddHours(5).ToLongDateString();
                            db.Entry(adhocInterviewBatchCommitteeDb).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    return Ok(adhocInterviewBatchCommittee);
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
        [HttpGet]
        [Route("GetAdhocInterviewBatchCommittee/{interviewBatchId}")]
        public IHttpActionResult GetAdhocInterviewBatchCommittee(int interviewBatchId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var adhocInterviewBatchCommittee = db.AdhocInterviewBatchCommittees.Where(x => x.InterviewBatch_Id == interviewBatchId && x.IsActive == true).ToList();
                    return Ok(adhocInterviewBatchCommittee);
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
        [HttpGet]
        [Route("SendAdhocInterviewSMS/{interviewBatchId}")]
        public IHttpActionResult SendAdhocInterviewSMS(int interviewBatchId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    //                    if (interviewBatchId == 0)
                    //                    {
                    //                        return Ok(false);
                    //                        var adhocInterviewBatches = db.AdhocInterviewBatches.Where(x => x.IsActive == true).ToList();
                    //                        foreach (var batch in adhocInterviewBatches)
                    //                        {
                    //                            var adhocInterview = db.AdhocInterviewViews.FirstOrDefault(x => x.Id == batch.Interview_Id && x.IsActive == true);
                    //                            if (adhocInterview != null)
                    //                            {
                    //                                var adhocInterviewBatchApplications = db.AdhocInterviewBatchApplications.Where(x => x.Batch_Id == batch.Id && x.IsActive == true).ToList();
                    //                                int count = 0;
                    //                                int smsLogId = 0;
                    //                                foreach (var batchApp in adhocInterviewBatchApplications)
                    //                                {
                    //                                    var app = db.AdhocApplicationViews.FirstOrDefault(x => x.Id == batchApp.Application_Id && x.IsActive == true);
                    //                                    if (app != null)
                    //                                    {
                    //                                        string MessageBody = "";
                    //                                        MessageBody = @"Dear Applicant, \n It is informed you that the interview against the post of " + app.DesignationName + @" on adhoc basis of District " + adhocInterview.DistrictName +
                    //        " is scheduled on " + batch.Datetime.Value.ToString("dddd, dd MMMM yyyy hh:mm tt") + " at " + batch.Venue + @". \nYou are requested to appear for interview with original documents otherwise you will not be
                    //allowed to appear for interview.\n\nRegards,\nHealth Information and Service Delivery Unit \nPrimary and Secondary Healthcare Department";

                    //                                        if (!string.IsNullOrEmpty(app.MobileNumber))
                    //                                        {
                    //                                            SMS sms1 = new SMS()
                    //                                            {
                    //                                                UserId = User.Identity.GetUserId(),
                    //                                                FKId = app.Id,
                    //                                                MobileNumber = app.MobileNumber,
                    //                                                //MobileNumber = "03214677763",
                    //                                                Message = MessageBody
                    //                                            };
                    //                                            var smsLog = Common.SendSMSTelenor(sms1);
                    //                                            if (smsLog != null)
                    //                                            {
                    //                                                smsLogId = smsLog.Id;
                    //                                            }
                    //                                            batchApp.SMSLog_Id = smsLogId;
                    //                                            batchApp.SMS_Sent = true;
                    //                                            db.Entry(batchApp).State = EntityState.Modified;
                    //                                            db.SaveChanges();
                    //                                            count++;
                    //                                        }
                    //                                    }
                    //                                }
                    //                            }
                    //                        }
                    //                    }
                    //                    else
                    //                    {
                    //                        var adhocInterviewBatch = db.AdhocInterviewBatches.FirstOrDefault(x => x.Id == interviewBatchId && x.IsActive == true);
                    //                        if (adhocInterviewBatch != null)
                    //                        {
                    //                            var adhocInterview = db.AdhocInterviewViews.FirstOrDefault(x => x.Id == adhocInterviewBatch.Interview_Id && x.IsActive == true);
                    //                            if (adhocInterview != null)
                    //                            {

                    //                                var adhocInterviewBatchApplications = db.AdhocInterviewBatchApplications.Where(x => x.Batch_Id == adhocInterviewBatch.Id && x.IsActive == true).ToList();
                    //                                int count = 0;
                    //                                int smsLogId = 0;
                    //                                foreach (var batchApp in adhocInterviewBatchApplications)
                    //                                {
                    //                                    var app = db.AdhocApplicationViews.FirstOrDefault(x => x.Id == batchApp.Application_Id && x.IsActive == true);
                    //                                    if (app != null)
                    //                                    {
                    //                                        string MessageBody = "";
                    //                                        MessageBody = @"Dear Applicant, It is informed you that the interview against the post of " + app.DesignationName + @" on adhoc basis of District " + adhocInterview.DistrictName +
                    //        " is scheduled on " + adhocInterviewBatch.Datetime.Value.ToString("dddd, dd MMMM yyyy hh:mm tt") + " at " + adhocInterviewBatch.Venue + @". \nYou are requested to appear for interview with origional documents otherwise you will not be
                    //allowed to appear for interview.\n\nRegards,\nHealth Information and Service Delivery Unit \nPrimary and Secondary Healthcare Department";

                    //                                        if (!string.IsNullOrEmpty(app.MobileNumber))
                    //                                        {
                    //                                            SMS sms1 = new SMS()
                    //                                            {
                    //                                                UserId = User.Identity.GetUserId(),
                    //                                                FKId = app.Id,
                    //                                                MobileNumber = app.MobileNumber,
                    //                                                //MobileNumber = "03214677763",
                    //                                                Message = MessageBody
                    //                                            };
                    //                                            var smsLog = Common.SendSMSTelenor(sms1);
                    //                                            if (smsLog != null)
                    //                                            {
                    //                                                smsLogId = smsLog.Id;
                    //                                            }
                    //                                            batchApp.SMSLog_Id = smsLogId;
                    //                                            batchApp.SMS_Sent = true;
                    //                                            db.Entry(batchApp).State = EntityState.Modified;
                    //                                            db.SaveChanges();
                    //                                            count++;
                    //                                        }
                    //                                    }

                    //                                }
                    //                            }

                    //                        }
                    //                    }

                    return Ok(false);
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

        [HttpGet]
        [Route("ChangeAdhocInterviewStatus/{id}/{statusId}")]
        public async Task<IHttpActionResult> ChangeAdhocInterviewStatus(int id, int statusId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var adhocInterview = await db.AdhocInterviews.FirstOrDefaultAsync(x => x.Id == id && x.IsActive == true);

                    if (adhocInterview != null)
                    {
                        adhocInterview.Status_Id = statusId;
                        db.Entry(adhocInterview).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        if (adhocInterview.Status_Id >= 5)
                        {
                            var batchApplicationIds = await db.AdhocInterviewBatchApplicationVs.Where(x => x.InterviewId == adhocInterview.Id && x.IsPresent == true && x.IsLocked == true && x.IsRejected != true).Select(x => x.Id).ToListAsync();
                            var adhocMerits = await db.AdhocMeritView2.Where(x => batchApplicationIds.Contains((int)x.BatchApplication_Id) && x.IsActive == true && x.MatricMarks_Id != null && x.MasterMarks_Id != null)
                                .OrderByDescending(x => x.Total)
                                .ThenByDescending(x => x.TotalDays)
                                .ThenByDescending(x => x.MasterObtained)
                                .ToListAsync();
                            var adhocMeritsLocked = new List<AdhocMeritLocked>();
                            int count = 0;
                            double topTotalMarks = 0;
                            DateTime topDOB = new DateTime();
                            int topDegreeMarks = 0;
                            int days = 0;
                            int adhocMeritsLength = adhocMerits.Count;
                            foreach (var merit in adhocMerits)
                            {
                                count++;
                                var amlDb = db.AdhocMeritLockeds.FirstOrDefault(x => x.AdhocMerit_Id == merit.Id && x.IsActive == true);
                                if (amlDb == null)
                                {
                                    AdhocMeritLocked aml = new AdhocMeritLocked();
                                    aml.AdhocMerit_Id = merit.Id;
                                    aml.Applicant_Id = merit.Applicant_Id;
                                    aml.Application_Id = merit.Application_Id;
                                    aml.BatchApplication_Id = merit.BatchApplication_Id;
                                    aml.DistrictCode = merit.DistrictCode;
                                    aml.MeritNumber = count;
                                    if (aml.MeritNumber != count)
                                    {

                                    }
                                    aml.MatricMarks_Id = merit.MatricMarks_Id;
                                    aml.MatricTotal = merit.MatricTotal;
                                    aml.MatricObtained = merit.MatricObtained;
                                    aml.MatricPercent = merit.MatricPercent;
                                    aml.Matriculation = merit.Matriculation;
                                    aml.InterMarks_Id = merit.InterMarks_Id;
                                    aml.InterTotal = merit.InterTotal;
                                    aml.InterObtained = merit.InterObtained;
                                    aml.InterPercent = merit.InterPercent;
                                    aml.Intermediate = merit.Intermediate;
                                    aml.MasterMarks_Id = merit.MasterMarks_Id;
                                    aml.MasterTotal = merit.MasterTotal;
                                    aml.MasterObtained = merit.MasterObtained;
                                    aml.MasterPercent = Math.Round((double)merit.MasterPercent, 2);
                                    aml.Master = merit.Master;
                                    aml.FirstHigher = merit.FirstHigher;
                                    aml.FirstHigherMarks_Id = merit.FirstHigherMarks_Id;
                                    aml.SecondHigher = merit.SecondHigher;
                                    aml.SecondHigherMarks_Id = merit.SecondHigherMarks_Id;
                                    aml.ThirdHigher = merit.ThirdHigher;
                                    aml.ThirdHigherMarks_Id = merit.ThirdHigherMarks_Id;
                                    aml.FirstPosition = merit.FirstPosition;
                                    aml.FirstPositionMarks_Id = merit.FirstPositionMarks_Id;
                                    aml.SecondPosition = merit.SecondPosition;
                                    aml.SecondPositionMarks_Id = merit.SecondPositionMarks_Id;
                                    aml.ThirdPosition = merit.ThirdPosition;
                                    aml.ThirdPositionMarks_Id = merit.ThirdPositionMarks_Id;
                                    aml.OneYearExp = merit.OneYearExp;
                                    aml.TwoYearExp = merit.TwoYearExp;
                                    aml.ThreeYearExp = merit.ThreeYearExp;
                                    aml.FourYearExp = merit.FourYearExp;
                                    aml.FivePlusYearExp = merit.FivePlusYearExp;
                                    aml.ExperienceMarks_Id = merit.ExperienceMarks_Id;
                                    aml.Hafiz = merit.Hafiz;
                                    aml.HafizMarks_Id = merit.HafizMarks_Id;
                                    aml.Interview = merit.Interview;
                                    aml.Total = merit.Total;
                                    if (count == 1)
                                    {
                                        topTotalMarks = (double)aml.Total;
                                        topDOB = Convert.ToDateTime(merit.DOB.Value);
                                        topDegreeMarks = (int)aml.MasterObtained;
                                        aml.WhyAboveReason = "Merit # 1 Marks: " + aml.Total.ToString() + ", D.O.B: " + merit.DOB.Value.ToString("d/M/y") + ", Degree Marks: " + aml.MasterObtained;
                                    }
                                    if (count > 1)
                                    {
                                        if (aml.Total < topTotalMarks)
                                        {
                                            aml.WhyBelowReason = "Marks Difference: " + ((double)(topTotalMarks - aml.Total)).ToString("0.##") + ", Self Marks: " + aml.Total + ", Senior Marks: " + topTotalMarks;
                                            topTotalMarks = (double)aml.Total;
                                            topDOB = Convert.ToDateTime(merit.DOB.Value);
                                            topDegreeMarks = (int)aml.MasterObtained;
                                        }
                                        else
                                        {
                                            DateTime LowYear = Convert.ToDateTime(merit.DOB.Value);
                                            TimeSpan objTimeSpan = LowYear - topDOB;
                                            days = Convert.ToInt32(objTimeSpan.TotalDays);
                                            if (days == 0)
                                            {
                                                aml.WhyBelowReason = "Degree Marks Difference: " + (topDegreeMarks - aml.MasterObtained) + ", Self Degree Marks: " + aml.MasterObtained + ", Senior Degree Marks: " + topDegreeMarks;
                                            }
                                            else
                                            {
                                                aml.WhyBelowReason = "Days Difference: " + days + ", Self D.O.B: " + merit.DOB.Value.ToString("d/M/y") + ", Senior D.O.B: " + topDOB.ToString("d/M/y");
                                            }
                                        }
                                        topTotalMarks = (double)aml.Total;
                                        topDOB = Convert.ToDateTime(merit.DOB.Value);
                                        topDegreeMarks = (int)aml.MasterObtained;
                                    }
                                    aml.IsLocked = true;
                                    aml.IsValid = true;
                                    aml.TotalApplicants = adhocMeritsLength;
                                    //aml.Remarks
                                    //aml.Selected
                                    //aml.VacantSeats
                                    //aml.VacantSeatsAsOn
                                    aml.IsActive = true;
                                    aml.CreatedDate = DateTime.UtcNow.AddHours(5);
                                    aml.UserId = User.Identity.GetUserId();
                                    aml.CreatedBy = User.Identity.GetUserName();
                                    adhocMeritsLocked.Add(aml);
                                }
                            }
                            db.AdhocMeritLockeds.AddRange(adhocMeritsLocked);
                            await db.SaveChangesAsync();
                        }
                        return Ok(true);
                    }
                    return Ok(false);
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


        [HttpGet]
        [Route("MoveMerits/{id}")]
        public async Task<IHttpActionResult> MoveMerits(int id)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    var merits = new List<int>() { 81694,
81683,
81680,
81711,
81689,
81690,
89153,
89154,
78626,
78631,
78643,
78630,
89150,
89152,
78636,
78641,
78642,
78629,
78639,
78638,
91430,
91431,
91432,
91233,
91235,
78640,
86561,
86549,
86542,
86530,
91434,
91433,
86534,
86577,
86544,
86521,
86531,
86533,
86527,
86537,
86573,
86557,
86550,
86524,
86551,
86529,
86522,
86558,
86525,
86539,
88377,
88372,
77115,
77114,
86536,
86535,
88385,
88378,
88369,
88381,
88380,
88382,
90812,
90817,
90807,
90811,
90809,
88384,
90819,
90814,
90808,
90815,
90813,
90822,
79600,
79606,
90824,
90820,
90816,
90818,
79591,
79608,
79601,
79599,
79610,
79605,
73981,
73987,
73982,
91393,
91395,
79596,
73985,
73979,
73984,
73980,
73986,
73983,
79627,
85256,
85263,
85261,
73988,
73989,
93597,
93603,
93596,
93602,
93600,
93599,
91949,
93207,
93595,
93604,
93601,
93598,
90513,
90506,
90512,
91947,
91948,
91946,
90508,
90504,
90517,
90511,
90514,
90510,
92054,
90516,
90515,
90502,
90507,
90509,
86657,
92051,
86650,
86645,
86653,
92053,
75667,
75671,
75675,
75666,
75670,
92052,
75673,
75676,
75677,
75669,
75680,
75674,
81229,
81232,
81226,
75668,
75679,
75678,
81228,
81234,
81230,
81235,
81227,
81233,
86884,
86881,
86778,
92881,
92704,
92880,
86880,
86883,
86887,
86885,
86888,
86882,
86572,
86874,
86879,
86878,
86886,
86876,
86563,
86555,
86559,
86552,
86554,
86567,
89116,
89117,
89110,
89103,
89106,
86566,
89105,
89111,
89113,
89115,
89108,
89112,
89109,
89122,
89118,
89114,
89107,
89104,
81622,
81632,
81691,
81695,
81710,
81627,
81675,
81697,
81678,
81630,
81685,
81686,
81682,
81681,
81684,
81693,
81679,
81688,
81677,
81634,
81625,
81623,
81628,
81692,
81644,
81629,
81633,
81696,
81652,
81687};
                    foreach (var mId in merits)
                    {
                        var merit = await db.AdhocMeritView2.Where(x => x.Id == mId).FirstOrDefaultAsync();

                        if (merit != null)
                        {
                            var amlDb = db.AdhocMeritLockeds.FirstOrDefault(x => x.AdhocMerit_Id == merit.Id && x.IsActive == true);
                            if (amlDb == null)
                            {
                                AdhocMeritLocked aml = new AdhocMeritLocked();
                                aml.AdhocMerit_Id = merit.Id;
                                aml.Applicant_Id = merit.Applicant_Id;
                                aml.Application_Id = merit.Application_Id;
                                aml.BatchApplication_Id = merit.BatchApplication_Id;
                                aml.DistrictCode = merit.DistrictCode;
                                aml.MeritNumber = merit.MeritNumber;
                                aml.MeritNumberChanging = aml.MeritNumber;
                                aml.MatricMarks_Id = merit.MatricMarks_Id;
                                aml.MatricTotal = merit.MatricTotal;
                                aml.MatricObtained = merit.MatricObtained;
                                aml.MatricPercent = merit.MatricPercent;
                                aml.Matriculation = merit.Matriculation;
                                aml.InterMarks_Id = merit.InterMarks_Id;
                                aml.InterTotal = merit.InterTotal;
                                aml.InterObtained = merit.InterObtained;
                                aml.InterPercent = merit.InterPercent;
                                aml.Intermediate = merit.Intermediate;
                                aml.MasterMarks_Id = merit.MasterMarks_Id;
                                aml.MasterTotal = merit.MasterTotal;
                                aml.MasterObtained = merit.MasterObtained;
                                if (merit.MasterPercent != null)
                                {
                                    aml.MasterPercent = Math.Round((double)merit.MasterPercent, 2);
                                }
                                aml.Master = merit.Master;
                                aml.FirstHigher = merit.FirstHigher;
                                aml.FirstHigherMarks_Id = merit.FirstHigherMarks_Id;
                                aml.SecondHigher = merit.SecondHigher;
                                aml.SecondHigherMarks_Id = merit.SecondHigherMarks_Id;
                                aml.ThirdHigher = merit.ThirdHigher;
                                aml.ThirdHigherMarks_Id = merit.ThirdHigherMarks_Id;
                                aml.FirstPosition = merit.FirstPosition;
                                aml.FirstPositionMarks_Id = merit.FirstPositionMarks_Id;
                                aml.SecondPosition = merit.SecondPosition;
                                aml.SecondPositionMarks_Id = merit.SecondPositionMarks_Id;
                                aml.ThirdPosition = merit.ThirdPosition;
                                aml.ThirdPositionMarks_Id = merit.ThirdPositionMarks_Id;
                                aml.OneYearExp = merit.OneYearExp;
                                aml.TwoYearExp = merit.TwoYearExp;
                                aml.ThreeYearExp = merit.ThreeYearExp;
                                aml.FourYearExp = merit.FourYearExp;
                                aml.FivePlusYearExp = merit.FivePlusYearExp;
                                aml.ExperienceMarks_Id = merit.ExperienceMarks_Id;
                                aml.Hafiz = merit.Hafiz;
                                aml.HafizMarks_Id = merit.HafizMarks_Id;
                                aml.Interview = merit.Interview;
                                aml.Total = merit.Total;

                                aml.IsLocked = true;
                                aml.IsValid = true;
                                //aml.Remarks
                                //aml.Selected
                                //aml.VacantSeats
                                //aml.VacantSeatsAsOn
                                aml.IsActive = true;
                                aml.CreatedDate = DateTime.UtcNow.AddHours(5);
                                aml.UserId = User.Identity.GetUserId();
                                aml.CreatedBy = User.Identity.GetUserName();
                                db.AdhocMeritLockeds.Add(aml);
                                await db.SaveChangesAsync();
                            }
                        }

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

        [HttpGet]
        [Route("LockMeritOrder/{designationId}")]
        public IHttpActionResult LockMeritOrder(int designationId)

        {
            try
            {

                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    DateTime date = new DateTime(2022,03,09);
                    var dates = date.Date;
                    var meritsPosted = db.AdhocPostingFinalViews.Where(x => x.Designation_Id == designationId && x.IsActive == true && 
                    x.ESR_Id == null && EntityFunctions.TruncateTime(x.DateTime) == dates.Date).OrderBy(k => k.MeritNumber).ToList();
                    foreach (var merit in meritsPosted)
                    {

                        string userName = "System";
                        string userId = "System";
                        int desigId = 0;
                        bool isBHU = false;
                        HrProfile hrProfile;
                        HFListP hf;
                        AdhocPostingFinal adhocPostingFinal = db.AdhocPostingFinals.FirstOrDefault(x => x.Id == merit.Id);
                        if (adhocPostingFinal != null)
                        {
                            hf = db.HFListPs.FirstOrDefault(x => x.Id == adhocPostingFinal.PostingHF_Id);
                            if (hf != null)
                            {
                                if (hf.HFTypeCode.Equals("014"))
                                {
                                    desigId = 2404;
                                }
                                else
                                {
                                    desigId = (int)merit.Designation_Id;
                                }
                                var applicant = db.AdhocApplicants.FirstOrDefault(x => x.Id == merit.Applicant_Id);
                                if (applicant != null)
                                {
                                    hrProfile = db.HrProfiles.FirstOrDefault(x => x.CNIC.Equals(merit.CNIC));

                                    //var vPMaster = db.VpMastProfileViews.Where(x => x.HF_Id == merit.PostingHF_Id && x.Desg_Id == desigId)?.FirstOrDefault();

                                    //if (vPMaster != null)
                                    //{
                                    //    if (vPMaster.Vacant <= 0)
                                    //    {

                                    //    }
                                    //}

                                    if (hrProfile == null)
                                    {
                                        hrProfile = new HrProfile();
                                        hrProfile.EmployeeName = merit.Name;
                                        hrProfile.FatherName = merit.FatherName;
                                        hrProfile.CNIC = merit.CNIC;
                                        hrProfile.DateOfBirth = merit.DOB;

                                        string lastChar = merit.CNIC.Substring(merit.CNIC.Length - 1);
                                        hrProfile.Gender = Convert.ToInt32(lastChar) % 2 == 0 ? "Female" : "Male";

                                        hrProfile.MobileNo = merit.MobileNumber;
                                        hrProfile.LandlineNo = merit.MobileNumber;
                                        hrProfile.Faxno = merit.MobileNumber;

                                        hrProfile.Domicile_Id = applicant.Domicile_Id;
                                        hrProfile.MaritalStatus = applicant.MaritalStatus;
                                        hrProfile.EMaiL = applicant.Email;
                                        hrProfile.Religion_Id = applicant.Religion_Id;
                                        hrProfile.CorrespondenceAddress = applicant.Address;
                                        hrProfile.PermanentAddress = applicant.Address;
                                        hrProfile.PmdcNo = applicant.PMDCNumber;
                                        hrProfile.Language_Id = 9;
                                        hrProfile.Posttype_Id = 13;
                                        //hrProfile.Qualification_Id = 27;
                                        hrProfile.ContractStartDate = DateTime.UtcNow.AddHours(5);
                                        hrProfile.ContractEndDate = hrProfile.ContractStartDate.Value.AddYears(1);

                                    }
                                    hrProfile.Department_Id = 25;
                                    hrProfile.CurrentGradeBPS = 17;
                                    hrProfile.JoiningGradeBPS = 17;
                                    hrProfile.PrivatePractice = "No";

                                    hrProfile.HealthFacility_Id = merit.PostingHF_Id;
                                    hrProfile.HfmisCode = merit.PostingHFMISCode;
                                    hrProfile.WorkingHealthFacility_Id = merit.PostingHF_Id;
                                    hrProfile.WorkingHFMISCode = merit.PostingHFMISCode;

                                    hrProfile.WDesignation_Id = 1085;
                                    hrProfile.Designation_Id = desigId;

                                    hrProfile.EmpMode_Id = 3;
                                    hrProfile.Status_Id = 30;


                                    if (hrProfile.Id == 0)
                                    {
                                        Entity_Lifecycle eld = new Entity_Lifecycle();
                                        eld.Created_Date = DateTime.UtcNow.AddHours(5);
                                        eld.Created_By = userName;
                                        eld.Users_Id = userId;
                                        eld.IsActive = true;
                                        eld.Entity_Id = 2;

                                        db.Entity_Lifecycle.Add(eld);
                                        db.SaveChanges();
                                        hrProfile.EntityLifecycle_Id = eld.Id;
                                        db.HrProfiles.Add(hrProfile);
                                        db.SaveChanges();
                                    }
                                    else
                                    {
                                        db.Entry(hrProfile).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                    if (desigId == 1320)
                                    {
                                        desigId = 1157;
                                    }
                                    if (desigId == 802)
                                    {
                                        desigId = 1085;
                                    }

                                    var posted = _transferPostingService.UpdateVacancy(db, true, hrProfile.HfmisCode, desigId, hrProfile.EmpMode_Id, userName, userId);

                                    if (posted == true)
                                    {
                                        adhocPostingFinal.Profile_Id = hrProfile.Id;
                                        adhocPostingFinal.ESR_Id = 1;
                                        db.Entry(adhocPostingFinal).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                            }
                        }
                    }
                    return Ok(true);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("AdhocPosting/{applicationId}/{designationId}")]
        public async Task<IHttpActionResult> AdhocPosting(int applicationId, int designationId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var application = new AdhocApplicationView();
                    bool nearest = false;
                    int countSeatsVacant = 1;
                    var designationIds = new List<int>();
                    designationIds.Add(designationId);
                    if (applicationId == 10007)
                    {
                        var districts = db.Districts.Where(x => !x.Name.StartsWith("isla") && !x.Name.StartsWith("Lahore")).OrderBy(x => x.Name).ToList();
                        foreach (var district in districts)
                        {
                            countSeatsVacant = 1;
                            System.Diagnostics.Debug.WriteLine(district.Name);
                            if (district.Name == "Mianwali")
                            {
                                var a = 1;
                            }
                            if (district.Name == "Rawalpindi")
                            {
                                var a = 1;
                            }
                            int totalVacant = (int?)db.AdhocHFOpenedViews.AsNoTracking().Where(x => x.DistrictCode.Equals(district.Code) && designationIds.Contains((int)x.Designation_Id)  && x.BatchNo == 3 && x.IsActive == true).Select(x => x.SeatsLeft).Sum() ?? 0;
                            var merits = await db.AdhocMeritLockedViews.AsNoTracking().Where(x => x.IsPostedOnAdhoc == null && x.DistrictCode.Equals(district.Code) && x.Designation_Id == designationId && x.IsLocked == true && x.IsActive == true)
                                               .OrderBy(x => x.MeritNumber)
                                              .ToListAsync();
                            foreach (var merit in merits)
                            {

                                System.Diagnostics.Debug.WriteLine(merit.Application_Id);
                                if (merit.Application_Id == 13213)
                                {
                                    var bbb = 2;
                                }
                                if (countSeatsVacant <= totalVacant)
                                {
                                    var posting = await db.AdhocPostingFinalViews.AsNoTracking().FirstOrDefaultAsync(x => x.Applicant_Id == merit.Applicant_Id && x.IsActive == true);
                                    if (posting == null)
                                    {
                                        countSeatsVacant++;
                                        if (merit.DistrictCodeAllotted == true || merit.DistrictCodeAllotted == null)
                                        {
                                            application = await db.AdhocApplicationViews.FirstOrDefaultAsync(x => x.Id == merit.Application_Id && x.IsActive == true);

                                            if (application.Id == 13213)
                                            {

                                            }

                                            if (application.Id == 587)
                                            {

                                            }
                                            if (!string.IsNullOrEmpty(application.Gender) && !application.Gender.ToLower().Equals("female") && application.Designation_Id == 302)
                                            {
                                                totalVacant++;
                                                continue;
                                            }

                                            if (application != null && application.Status_Id == 2)
                                            {
                                                nearest = false;
                                                var preferences = await db.AdhocApplicationPreferenceViews.AsNoTracking().Where(x => x.DistrictCode.Equals(district.Code) && x.JobApplication_Id == application.Id && x.IsActive == true).OrderBy(x => x.PreferenceOrder).ToListAsync();
                                                int count = 0;
                                                foreach (var preference in preferences)
                                                {
                                                    count++;
                                                    var hfOpened = db.AdhocHFOpenedViews.AsNoTracking().FirstOrDefault(x => x.HF_Id == preference.JobHF_Id && x.Designation_Id == designationId && x.IsActive == true && x.BatchNo == 3);

                                                    if (hfOpened != null && hfOpened.SeatsLeft > 0 && hfOpened.SeatsLeft != null)
                                                    {
                                                        var res = await CheckAdhocPosting(db, preference, designationId, nearest, hfOpened, merit);

                                                        if (res == true)
                                                        {
                                                            break;
                                                        }

                                                    }
                                                    if (count == preferences.Count)
                                                    {
                                                        nearest = true;
                                                    }
                                                }
                                                if (nearest == true)
                                                {
                                                    count = 0;
                                                    foreach (var preference in preferences)
                                                    {
                                                        count++;
                                                        var hfOpened = await db.AdhocHFOpenedViews.AsNoTracking().FirstOrDefaultAsync(x => x.HF_Id == preference.JobHF_Id && x.IsActive == true && x.BatchNo == 3);
                                                        if (hfOpened != null)
                                                        {
                                                            var res = await CheckAdhocPosting(db, preference, designationId, nearest, hfOpened, merit);
                                                            if (res == true)
                                                            {
                                                                break;
                                                            }
                                                            else if (count >= preferences.Count)
                                                            {
                                                                //Not Posted
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //foreach (var m in merits.Where(x => x.MeritNumber > merit.MeritNumber).ToList())
                                            //{
                                            //    var mDb = db.AdhocMeritLockeds.FirstOrDefault(x => x.Id == m.Id);
                                            //    if (mDb != null)
                                            //    {
                                            //        mDb.MeritNumberChanging = mDb.MeritNumberChanging - 1;
                                            //        db.SaveChanges();
                                            //    }
                                            //}
                                            //int meritChangesAffected = db.Database.ExecuteSqlCommand($" UPDATE AdhocMeritLockedView set  MeritNumberChanging = MeritNumberChanging - 1 where MeritNumber > {merit.MeritNumber} and Designation_Id = {merit.Designation_Id} and DistrictCode = '{merit.DistrictCode}';");
                                            //System.Diagnostics.Debug.WriteLine($"Merit Changes Affected of Merit No {merit.MeritNumber} : {meritChangesAffected}");
                                        }
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
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
        public async Task<bool> CheckAdhocPosting(HR_System db, AdhocApplicationPreferenceView preference, int designationId, bool nearestPosting, AdhocHFOpenedView hfOpened, AdhocMeritLockedView merit)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                AdhocPostingLog postingLog = new AdhocPostingLog();
                int seatsLeft = 0;
                if (nearestPosting == false)
                {
                    seatsLeft = (int)hfOpened.SeatsLeft - 1;
                    AdhocPostingFinal postingFinal = new AdhocPostingFinal();
                    postingFinal.AdhocMeritLocked_Id = merit.Id;
                    postingFinal.Preference_Id = preference.Id;
                    postingFinal.PreferenceNumber = preference.PreferenceOrder;

                    postingFinal.ActualVPMaster_Id = hfOpened.VPMaster_Id;
                    postingFinal.ActualHF_Id = hfOpened.HF_Id;
                    postingFinal.ActualHFMISCode = hfOpened.HFMISCode;
                    postingFinal.ActualSeatNumber = hfOpened.Vacant - seatsLeft;

                    postingFinal.PostingVPMaster_Id = hfOpened.VPMaster_Id;
                    postingFinal.PostingHF_Id = hfOpened.HF_Id;
                    postingFinal.PostingHFMISCode = hfOpened.HFMISCode;
                    postingFinal.SeatNumber = postingFinal.ActualSeatNumber;

                    postingFinal.TotalSeats = hfOpened.Vacant;

                    postingFinal.IsActive = true;
                    postingFinal.DateTime = DateTime.UtcNow.AddHours(5);
                    postingFinal.Remarks = "Posted at preference number " + postingFinal.PreferenceNumber + " as " + merit.DesignationName + " in " + hfOpened.HFName;

                    db.AdhocPostingFinals.Add(postingFinal);
                    await db.SaveChangesAsync();

                    postingLog.Remarks = postingFinal.Remarks;
                    postingLog.Preference_Id = preference.Id;
                    postingLog.AdhocMeritLocked_Id = postingFinal.AdhocMeritLocked_Id;
                    postingLog.IsActive = true;
                    postingLog.DateTime = DateTime.UtcNow.AddHours(5);
                    db.AdhocPostingLogs.Add(postingLog);
                    await db.SaveChangesAsync();

                    var hfOpenedDb = await db.AdhocHFOpeneds.FirstOrDefaultAsync(x => x.Id == hfOpened.Id);
                    hfOpenedDb.SeatsLeft = seatsLeft;
                    db.Entry(hfOpenedDb).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return true;
                }
                else
                {
                    //Choosing Health Facility from nearest of Preference
                    var hf = await db.HFListPs.FirstOrDefaultAsync(x => x.Id == preference.JobHF_Id && x.IsActive == true);
                    if (hf != null)
                    {
                        postingLog = new AdhocPostingLog();
                        postingLog.Remarks = "Choosing Health Facility from nearest of Preference " + preference.HFName;
                        #region Near Bird Eye Distance
                        var designationIds = new List<int>();
                        designationIds.Add(designationId);
                        var hfOpenedHFIds = await db.AdhocHFOpenedViews.AsNoTracking().Where(x => designationIds.Contains(designationId) && x.DistrictCode.Equals(hf.DistrictCode) && x.SeatsLeft > 0 && x.SeatsLeft != null && x.BatchNo == 3).Select(x => x.HF_Id).ToListAsync();
                        var hfs = await db.HFListPs.Where(x => hfOpenedHFIds.Contains(x.Id) && x.Latitude != null && x.Longitude != null && x.IsActive == true).ToListAsync();

                        List<DistanceAndId> hfDistanceAndIds = new List<DistanceAndId>();
                        foreach (var h in hfs)
                        {
                            if (h.Latitude != null && h.Longitude != null)
                            {
                                double distance = CalculateDistance(Convert.ToDouble(hf.Latitude),
                                Convert.ToDouble(hf.Longitude),
                                Convert.ToDouble(h.Latitude),
                                Convert.ToDouble(h.Longitude));
                                if (distance >= 0)
                                {
                                    hfDistanceAndIds.Add(new DistanceAndId() { Id = h.Id, Distance = distance, FacilityName = h.FullName });
                                }
                            }
                        }
                        var distanceIds = hfDistanceAndIds.OrderBy(x => x.Distance).Select(x => x.Id).ToList();
                        HFListP nearestHF = null;
                        foreach (var hfDistanceAndId in distanceIds)
                        {
                            nearestHF = db.HFListPs.FirstOrDefault(x => x.Id == hfDistanceAndId);
                            if (nearestHF != null)
                            {
                                hfOpened = await db.AdhocHFOpenedViews.AsNoTracking().FirstOrDefaultAsync(x => x.HF_Id == nearestHF.Id && x.Designation_Id == designationId && x.IsActive == true && x.BatchNo == 3);

                                var postedMerits = await db.AdhocPostingFinalViews.Where(x => x.PostingHF_Id == preference.JobHF_Id && x.IsActive == true)
                                    .Select(x => x.MeritNumber).ToListAsync();

                                if (hfOpened != null && hfOpened.SeatsLeft > 0 && hfOpened.SeatsLeft != null)
                                {
                                    seatsLeft = (int)hfOpened.SeatsLeft - 1;
                                    AdhocPostingFinal postingFinal = new AdhocPostingFinal();
                                    postingFinal.AdhocMeritLocked_Id = merit.Id;
                                    postingFinal.Preference_Id = preference.Id;
                                    postingFinal.PreferenceNumber = preference.PreferenceOrder;

                                    postingFinal.ActualVPMaster_Id = hfOpened.VPMaster_Id;
                                    postingFinal.ActualHF_Id = hfOpened.HF_Id;
                                    postingFinal.ActualHFMISCode = hfOpened.HFMISCode;
                                    postingFinal.ActualSeatNumber = hfOpened.Vacant - seatsLeft;

                                    postingFinal.PostingVPMaster_Id = hfOpened.VPMaster_Id;
                                    postingFinal.PostingHF_Id = hfOpened.HF_Id;
                                    postingFinal.PostingHFMISCode = hfOpened.HFMISCode;
                                    postingFinal.SeatNumber = postingFinal.ActualSeatNumber;

                                    postingFinal.TotalSeats = hfOpened.Vacant;

                                    postingFinal.IsActive = true;
                                    postingFinal.DateTime = DateTime.UtcNow.AddHours(5);
                                    postingFinal.Remarks = "Posted at nearest facility to preference number " + postingFinal.PreferenceNumber + " as " + merit.DesignationName + " in " + hfOpened.HFName;

                                    db.AdhocPostingFinals.Add(postingFinal);
                                    await db.SaveChangesAsync();

                                    postingLog.Remarks = postingFinal.Remarks;
                                    postingLog.Preference_Id = preference.Id;
                                    postingLog.AdhocMeritLocked_Id = postingFinal.AdhocMeritLocked_Id;
                                    postingLog.IsActive = true;
                                    postingLog.DateTime = DateTime.UtcNow.AddHours(5);
                                    db.AdhocPostingLogs.Add(postingLog);
                                    await db.SaveChangesAsync();
                                    var hfOpenedDb = await db.AdhocHFOpeneds.FirstOrDefaultAsync(x => x.Id == hfOpened.Id);
                                    hfOpenedDb.SeatsLeft = seatsLeft;
                                    db.Entry(hfOpenedDb).State = EntityState.Modified;
                                    await db.SaveChangesAsync();
                                    return true;

                                }
                            }
                        }
                        #endregion
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpGet]
        [Route("AdhocPostingMOWMO/{applicationId}/{designationId}/{min}/{max}")]
        public async Task<IHttpActionResult> AdhocPostingMOWMO(int applicationId, int designationId, int min, int max)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var application = new AdhocApplicationView();
                    bool nearest = false;
                    bool districtAllotted = false;
                    int countSeatsVacant = 1;
                    var designationIds = new List<int>();
                    designationIds.Add(designationId);
                    int totalVacant = 0;
                    AdhocDistrictOpenedBHUView2 moWMOVacant = new AdhocDistrictOpenedBHUView2();
                    if (applicationId == 10007)
                    {
                        do
                        {
                            var districts = db.Districts.Where(x => !x.Name.StartsWith("isla")).OrderBy(x => x.Name).ToList();
                            for (int i = 0; i < districts.Count; i++)
                            {
                                var district = districts[i];
                                totalVacant = 0;
                                if (designationId == 802 || designationId == 1320)
                                {
                                    designationIds.Add(2404);
                                    var vacant = db.AdhocHFOpenedViews.AsNoTracking().Where(x => x.DistrictCode.Equals(district.Code) && x.Designation_Id == designationId && x.IsActive == true).Select(x => x.Vacant).ToList();
                                    var moWMOVacants = db.AdhocHFOpenedViews.AsNoTracking().Where(x => x.DistrictCode.Equals(district.Code) && x.Designation_Id == 2404 && x.IsActive == true).Select(x => x.Vacant).ToList();

                                    int vacantSum = vacant != null ? (int)vacant.Sum() : 0;
                                    int moWMOVacantSum = moWMOVacants != null ? (int)moWMOVacants.Sum() : 0;

                                    moWMOVacantSum = moWMOVacantSum % 2 == 1 ? ((moWMOVacantSum - 1) / 2) : (moWMOVacantSum / 2);
                                    totalVacant = vacantSum + moWMOVacantSum;
                                }
                                else
                                {
                                    totalVacant = (int)db.AdhocHFOpenedViews.AsNoTracking().Where(x => x.DistrictCode.Equals(district.Code) && designationIds.Contains((int)x.Designation_Id) && x.IsActive == true).Select(x => x.Vacant).Sum();
                                }
                                var districtMerits = await db.AdhocMeritLockedViews.AsNoTracking().Where(x => x.IsPosted == null && x.DistrictCode.Equals(district.Code) && x.Designation_Id == designationId && x.IsLocked == true && x.IsActive == true)
                                                                                                                .OrderBy(x => x.MeritNumber).ToListAsync();
                                for (int j = 0; j < districtMerits.Count; j++)
                                {
                                    var districtMerit = districtMerits[j];
                                    if (districtMerit.MeritNumberChanging <= totalVacant)
                                    {
                                        var meritDb = db.AdhocMeritLockeds.FirstOrDefault(x => x.Id == districtMerit.Id);
                                        if (meritDb != null)
                                        {
                                            meritDb.DistrictSelected = true;
                                            db.SaveChanges();
                                        }
                                    }
                                    if (districtMerit.MeritNumberChanging > totalVacant)
                                    {
                                        break;
                                    }
                                }
                            }
                            var meritSelected = await db.AdhocMeritLockedViews.AsNoTracking().Where(x => x.IsPosted == null && x.DistrictSelected == true && x.DistrictCodeAllotted == null && x.Designation_Id == designationId && x.IsLocked == true && x.IsActive == true)
                                                   .OrderBy(x => x.MeritNumber)
                                                  .ToListAsync();
                            if (meritSelected.Count == 0)
                            {
                                districtAllotted = true;
                            }
                            for (int j = 0; j < meritSelected.Count; j++)
                            {
                                var merit = meritSelected[j];
                                var selectedDistrictCodes = db.AdhocMeritLockeds.Where(x => x.Application_Id == merit.Application_Id && x.DistrictSelected == true
                                && x.IsActive == true && x.IsLocked == true).Select(x => x.DistrictCode).ToList();
                                var preferredDistrict = db.AdhocApplicationPreferenceViews.Where(x => x.JobApplication_Id == merit.Application_Id && selectedDistrictCodes.Contains(x.DistrictCode)).OrderBy(x => x.PreferenceOrder).FirstOrDefault();
                                if (preferredDistrict != null)
                                {
                                    var districtSelected = db.AdhocMeritLockeds.Where(x => x.Application_Id == merit.Application_Id && x.DistrictSelected == true
                                    && x.IsActive == true && x.IsLocked == true).ToList();

                                    foreach (var selectedDistrictCode in districtSelected)
                                    {
                                        if (selectedDistrictCode.DistrictCode.Equals(preferredDistrict.DistrictCode))
                                        {
                                            selectedDistrictCode.DistrictCodeAllotted = true;
                                            db.SaveChanges();
                                        }
                                        else
                                        {
                                            selectedDistrictCode.DistrictCodeAllotted = false;
                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }
                            foreach (var district in districts)
                            {
                                countSeatsVacant = 1;
                                totalVacant = 0;
                                if (designationId == 802 || designationId == 1320)
                                {
                                    var vacant = db.AdhocHFOpenedViews.AsNoTracking().Where(x => x.DistrictCode.Equals(district.Code) && x.Designation_Id == designationId && x.IsActive == true).Select(x => x.SeatsLeft).ToList();
                                    //var moWMOVacant = db.AdhocHFOpenedViews.AsNoTracking().Where(x => x.DistrictCode.Equals(district.Code) && x.Designation_Id == 2404 && x.IsActive == true).Select(x => x.SeatsLeft).ToList();
                                    int vacantSum = vacant != null ? (int)vacant.Sum() : 0;
                                    int moWMOVacantSum = 0;
                                    moWMOVacant = db.AdhocDistrictOpenedBHUView2.AsNoTracking().FirstOrDefault(x => x.DistrictCode.Equals(district.Code));

                                    if (moWMOVacant != null && !string.IsNullOrEmpty(moWMOVacant.DistrictCode))
                                    {
                                        if (designationId == 802)
                                        {
                                            moWMOVacantSum = (int)moWMOVacant.BHUForMOLeft;
                                        }
                                        if (designationId == 1320)
                                        {
                                            moWMOVacantSum = (int)moWMOVacant.BHUForWMOLeft;
                                        }
                                    }
                                    totalVacant = vacantSum + moWMOVacantSum;
                                }
                                else
                                {
                                    totalVacant = (int)db.AdhocHFOpenedViews.AsNoTracking().Where(x => x.DistrictCode.Equals(district.Code) && designationIds.Contains((int)x.Designation_Id) && x.IsActive == true).Select(x => x.SeatsLeft).Sum();
                                }
                                var merits = await db.AdhocMeritLockedViews.AsNoTracking().Where(x => x.IsPosted == null && x.DistrictCode.Equals(district.Code) && x.Designation_Id == designationId && x.IsLocked == true && x.IsActive == true)
                                                  .OrderBy(x => x.MeritNumber)
                                                  .ToListAsync();
                                foreach (var merit in merits)
                                {
                                    if (merit.Application_Id == 1361)
                                    {
                                        var app = 1;
                                    }
                                    if (merit.Application_Id == 11183 && district.Name == "Multan")
                                    {
                                        var app = 1;
                                    }
                                    if (countSeatsVacant <= totalVacant)
                                    {
                                        application = await db.AdhocApplicationViews.FirstOrDefaultAsync(x => x.Id == merit.Application_Id && x.IsActive == true);
                                        if (application != null)
                                        {
                                            if (!string.IsNullOrEmpty(application.Gender) && application.Designation_Id == 1320 && !application.Gender.ToLower().Equals("female"))
                                            {
                                            }
                                            else if (!string.IsNullOrEmpty(application.Gender) && application.Designation_Id == 802 && !application.Gender.ToLower().Equals("male"))
                                            {
                                            }
                                            else
                                            {
                                                countSeatsVacant++;
                                                if (merit.DistrictCodeAllotted == true || merit.DistrictCodeAllotted == null)
                                                {
                                                    var posting = await db.AdhocPostingFinalViews.AsNoTracking().FirstOrDefaultAsync(x => x.Applicant_Id == merit.Applicant_Id && x.IsActive == true);
                                                    if (posting == null)
                                                    {
                                                        if (application != null && application.Status_Id == 2)
                                                        {
                                                            nearest = false;
                                                            var preferences = await db.AdhocApplicationPreferenceViews.AsNoTracking().Where(x => x.DistrictCode.Equals(district.Code) && x.JobApplication_Id == application.Id && x.IsActive == true).OrderBy(x => x.PreferenceOrder).ToListAsync();
                                                            moWMOVacant = db.AdhocDistrictOpenedBHUView2.AsNoTracking().FirstOrDefault(x => x.DistrictCode.Equals(district.Code));
                                                            int count = 0;
                                                            postedHfs = new List<string>();
                                                            foreach (var preference in preferences)
                                                            {
                                                                count++;
                                                                if (!postedHfs.Contains(preference.HFName))
                                                                {
                                                                    int desigCompare = 802;
                                                                    if (preference.HFMISCode.Substring(12, 3) == "014")
                                                                    {
                                                                        if (designationId == 802 && moWMOVacant.BHUForMOLeft <= 0)
                                                                        {
                                                                            if (count == preferences.Count)
                                                                            {
                                                                                nearest = true;
                                                                            }
                                                                            continue;
                                                                        }
                                                                        else if (designationId == 1320 && moWMOVacant.BHUForWMOLeft <= 0)
                                                                        {
                                                                            if (count == preferences.Count)
                                                                            {
                                                                                nearest = true;
                                                                            }
                                                                            continue;
                                                                        }
                                                                        desigCompare = 2404;
                                                                    }
                                                                    else
                                                                    {
                                                                        desigCompare = designationId;
                                                                    }
                                                                    var hfOpened = db.AdhocHFOpenedView2.AsNoTracking().FirstOrDefault(x => x.HF_Id == preference.JobHF_Id && x.Designation_Id == desigCompare && x.IsActive == true);
                                                                    if (hfOpened != null && hfOpened.SeatsLeft > 0 && hfOpened.SeatsLeft != null)
                                                                    {
                                                                        postedHfs.Add(hfOpened.HFName);
                                                                        var res = await CheckAdhocPostingMOWMO(db, preference, designationId, nearest, hfOpened, merit, moWMOVacant);

                                                                        if (res == true)
                                                                        {
                                                                            var adhocMeritLocked = await db.AdhocMeritLockeds.FirstOrDefaultAsync(x => x.Id == merit.Id);
                                                                            adhocMeritLocked.IsPosted = true;
                                                                            db.Entry(adhocMeritLocked).State = EntityState.Modified;
                                                                            await db.SaveChangesAsync();
                                                                            break;
                                                                        }
                                                                    }

                                                                }
                                                                if (count == preferences.Count)
                                                                {
                                                                    nearest = true;
                                                                }
                                                            }
                                                            if (nearest == true)
                                                            {
                                                                count = 0;
                                                                postedHfs = new List<string>();
                                                                foreach (var preference in preferences)
                                                                {
                                                                    count++;
                                                                    if (!postedHfs.Contains(preference.HFName))
                                                                    {
                                                                        int desigCompare = 802;
                                                                        if (preference.HFMISCode.Substring(12, 3) == "014")
                                                                        {
                                                                            desigCompare = 2404;
                                                                        }
                                                                        else
                                                                        {
                                                                            desigCompare = designationId;
                                                                        }
                                                                        var hfOpened = await db.AdhocHFOpenedView2.AsNoTracking().FirstOrDefaultAsync(x => x.HF_Id == preference.JobHF_Id && x.Designation_Id == desigCompare && x.IsActive == true);
                                                                        if (hfOpened != null)
                                                                        {
                                                                            postedHfs.Add(hfOpened.HFName);
                                                                            var res = await CheckAdhocPostingMOWMO(db, preference, designationId, nearest, hfOpened, merit, moWMOVacant);
                                                                            if (res == true)
                                                                            {
                                                                                var adhocMeritLocked = await db.AdhocMeritLockeds.FirstOrDefaultAsync(x => x.Id == merit.Id);
                                                                                adhocMeritLocked.IsPosted = true;
                                                                                db.Entry(adhocMeritLocked).State = EntityState.Modified;
                                                                                await db.SaveChangesAsync();
                                                                                break;
                                                                            }
                                                                            else if (count >= preferences.Count)
                                                                            {
                                                                                //Not Posted
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    foreach (var m in merits.Where(x => x.MeritNumber > merit.MeritNumber).ToList())
                                                    {
                                                        var mDb = db.AdhocMeritLockeds.FirstOrDefault(x => x.Id == m.Id);
                                                        if (mDb != null)
                                                        {
                                                            mDb.MeritNumberChanging = mDb.MeritNumberChanging - 1;
                                                            db.SaveChanges();
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }

                        } while (districtAllotted == false);
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



        [HttpGet]
        [Route("AdhocPostingMOWMOSingle/{applicationId}/{designationId}/{min}/{max}")]
        public async Task<IHttpActionResult> AdhocPostingMOWMOSingle(int applicationId, int designationId, int min, int max)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var application = new AdhocApplicationView();
                    bool nearest = false;
                    bool districtAllotted = false;
                    int countSeatsVacant = 1;
                    var designationIds = new List<int>();
                    designationIds.Add(designationId);
                    int totalVacant = 0;
                    AdhocDistrictOpenedBHUView2 moWMOVacant = new AdhocDistrictOpenedBHUView2();
                    if (applicationId == 10007)
                    {

                        var districts = db.Districts.Where(x => !x.Name.StartsWith("isla") && !x.Name.StartsWith("Lahore")).OrderBy(x => x.Name).ToList();
                        foreach (var district in districts)
                        {
                            countSeatsVacant = 1;
                            totalVacant = 0;
                            if (designationId == 802 || designationId == 1320)
                            {
                                var vacant = db.AdhocHFOpenedView2.AsNoTracking().Where(x => x.DistrictCode.Equals(district.Code) && x.Designation_Id == designationId && x.IsActive == true).Select(x => x.SeatsLeft).ToList();
                                //var moWMOVacant = db.AdhocHFOpenedViews.AsNoTracking().Where(x => x.DistrictCode.Equals(district.Code) && x.Designation_Id == 2404 && x.IsActive == true).Select(x => x.SeatsLeft).ToList();
                                int vacantSum = vacant != null ? (int)vacant.Sum() : 0;
                                int moWMOVacantSum = 0;
                                moWMOVacant = db.AdhocDistrictOpenedBHUView2.AsNoTracking().FirstOrDefault(x => x.DistrictCode.Equals(district.Code));

                                if (moWMOVacant != null && !string.IsNullOrEmpty(moWMOVacant.DistrictCode))
                                {
                                    if (designationId == 802)
                                    {
                                        moWMOVacantSum = (int)moWMOVacant.BHUForMOLeft;
                                    }
                                    if (designationId == 1320)
                                    {
                                        moWMOVacantSum = (int)moWMOVacant.BHUForWMOLeft;
                                    }
                                }
                                totalVacant = vacantSum + moWMOVacantSum;
                            }
                            else
                            {
                                totalVacant = (int)db.AdhocHFOpenedView2.AsNoTracking().Where(x => x.DistrictCode.Equals(district.Code) && designationIds.Contains((int)x.Designation_Id) && x.IsActive == true).Select(x => x.SeatsLeft).Sum();
                            }
                            var merits = await db.AdhocMeritLockedViews.AsNoTracking().Where(x => x.IsPostedOnAdhoc == null && x.DistrictCode.Equals(district.Code) && x.Designation_Id == designationId && x.IsLocked == true && x.IsActive == true)
                                              .OrderBy(x => x.MeritNumber)
                                              .ToListAsync();
                            System.Diagnostics.Debug.WriteLine($"District {district.Name}");
                            foreach (var merit in merits)
                            {
                                if (merit.Application_Id == 1585)
                                {
                                    var app = 1;
                                }
                                if (merit.Application_Id == 323)
                                {
                                    var app = 1;
                                }
                                if (countSeatsVacant <= totalVacant)
                                {
                                    application = await db.AdhocApplicationViews.FirstOrDefaultAsync(x => x.Id == merit.Application_Id && x.IsActive == true);
                                    if (application != null)
                                    {
                                        if (!string.IsNullOrEmpty(application.Gender) && application.Designation_Id == 1320 && !application.Gender.ToLower().Equals("female"))
                                        {
                                        }
                                        else if (!string.IsNullOrEmpty(application.Gender) && application.Designation_Id == 802 && !application.Gender.ToLower().Equals("male"))
                                        {
                                        }
                                        else
                                        {
                                            countSeatsVacant++;
                                            if (merit.DistrictCodeAllotted == true || merit.DistrictCodeAllotted == null)
                                            {
                                                var posting = await db.AdhocPostingFinalViews.AsNoTracking().FirstOrDefaultAsync(x => x.Applicant_Id == merit.Applicant_Id && x.IsActive == true);
                                                if (posting == null)
                                                {
                                                    if (application != null && application.Status_Id == 2)
                                                    {
                                                        nearest = false;
                                                        var preferences = await db.AdhocApplicationPreferenceViews.AsNoTracking().Where(x => x.DistrictCode.Equals(district.Code) && x.JobApplication_Id == application.Id && x.IsActive == true).OrderBy(x => x.PreferenceOrder).ToListAsync();
                                                        moWMOVacant = db.AdhocDistrictOpenedBHUView2.AsNoTracking().FirstOrDefault(x => x.DistrictCode.Equals(district.Code));
                                                        int count = 0;
                                                        postedHfs = new List<string>();
                                                        foreach (var preference in preferences)
                                                        {
                                                            count++;
                                                            if (!postedHfs.Contains(preference.HFName))
                                                            {
                                                                int desigCompare = 802;
                                                                if (preference.HFMISCode.Substring(12, 3) == "014")
                                                                {
                                                                    if (designationId == 802 && moWMOVacant.BHUForMOLeft <= 0)
                                                                    {
                                                                        if (count == preferences.Count)
                                                                        {
                                                                            nearest = true;
                                                                        }
                                                                        continue;
                                                                    }
                                                                    else if (designationId == 1320 && moWMOVacant.BHUForWMOLeft <= 0)
                                                                    {
                                                                        if (count == preferences.Count)
                                                                        {
                                                                            nearest = true;
                                                                        }
                                                                        continue;
                                                                    }
                                                                    desigCompare = 2404;
                                                                }
                                                                else
                                                                {
                                                                    desigCompare = designationId;
                                                                }
                                                                var hfOpened = db.AdhocHFOpenedView2.AsNoTracking().FirstOrDefault(x => x.HF_Id == preference.JobHF_Id && x.Designation_Id == desigCompare && x.IsActive == true);
                                                                if (hfOpened != null && hfOpened.SeatsLeft > 0 && hfOpened.SeatsLeft != null)
                                                                {
                                                                    postedHfs.Add(hfOpened.HFName);
                                                                    var res = await CheckAdhocPostingMOWMO(db, preference, designationId, nearest, hfOpened, merit, moWMOVacant);

                                                                    if (res == true)
                                                                    {
                                                                        var app = await db.AdhocApplications.FirstOrDefaultAsync(x => x.Id == merit.Application_Id);
                                                                        app.IsPostedOnAdhoc = true;
                                                                        db.Entry(app).State = EntityState.Modified;
                                                                        await db.SaveChangesAsync();
                                                                        break;
                                                                    }
                                                                }

                                                            }
                                                            if (count == preferences.Count)
                                                            {
                                                                nearest = true;
                                                            }
                                                        }
                                                        if (nearest == true)
                                                        {
                                                            count = 0;
                                                            postedHfs = new List<string>();
                                                            foreach (var preference in preferences)
                                                            {
                                                                count++;
                                                                if (!postedHfs.Contains(preference.HFName))
                                                                {
                                                                    int desigCompare = 802;
                                                                    if (preference.HFMISCode.Substring(12, 3) == "014")
                                                                    {
                                                                        desigCompare = 2404;
                                                                    }
                                                                    else
                                                                    {
                                                                        desigCompare = designationId;
                                                                    }
                                                                    var hfOpened = await db.AdhocHFOpenedView2.AsNoTracking().FirstOrDefaultAsync(x => x.HF_Id == preference.JobHF_Id && x.Designation_Id == desigCompare && x.IsActive == true);
                                                                    if (hfOpened != null)
                                                                    {
                                                                        postedHfs.Add(hfOpened.HFName);
                                                                        var res = await CheckAdhocPostingMOWMO(db, preference, designationId, nearest, hfOpened, merit, moWMOVacant);
                                                                        if (res == true)
                                                                        {
                                                                            var app = await db.AdhocApplications.FirstOrDefaultAsync(x => x.Id == merit.Application_Id);
                                                                            app.IsPostedOnAdhoc = true;
                                                                            db.Entry(app).State = EntityState.Modified;
                                                                            await db.SaveChangesAsync();
                                                                            break;
                                                                        }
                                                                        else if (count >= preferences.Count)
                                                                        {
                                                                            //Not Posted
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //foreach (var m in merits.Where(x => x.MeritNumber > merit.MeritNumber).ToList())
                                                //{
                                                //    var mDb = db.AdhocMeritLockeds.FirstOrDefault(x => x.Id == m.Id);
                                                //    if (mDb != null)
                                                //    {
                                                //        mDb.MeritNumberChanging = mDb.MeritNumberChanging - 1;
                                                //        db.SaveChanges();
                                                //    }
                                                //}

                                                int meritChangesAffected = db.Database.ExecuteSqlCommand($" UPDATE AdhocMeritLockedView set  MeritNumberChanging = MeritNumberChanging - 1 where MeritNumber > {merit.MeritNumber} and Designation_Id = {merit.Designation_Id} and DistrictCode = '{merit.DistrictCode}';");
                                                System.Diagnostics.Debug.WriteLine($"Merit Changes Affected of Merit No {merit.MeritNumber} : {meritChangesAffected}");

                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
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

        public async Task<bool> CheckAdhocPostingMOWMO(HR_System db, AdhocApplicationPreferenceView preference, int designationId, bool nearestPosting, AdhocHFOpenedView2 hfOpened, AdhocMeritLockedView merit, AdhocDistrictOpenedBHUView2 moWMOVacant)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                AdhocPostingLog postingLog = new AdhocPostingLog();
                int seatsLeft = 0;
                if (nearestPosting == false)
                {
                    seatsLeft = (int)hfOpened.SeatsLeft - 1;

                    AdhocPostingFinal postingFinal = new AdhocPostingFinal();
                    postingFinal.AdhocMeritLocked_Id = merit.Id;
                    postingFinal.Preference_Id = preference.Id;
                    postingFinal.PreferenceNumber = preference.PreferenceOrder;

                    postingFinal.ActualVPMaster_Id = hfOpened.VPMaster_Id;
                    postingFinal.ActualHF_Id = hfOpened.HF_Id;
                    postingFinal.ActualHFMISCode = hfOpened.HFMISCode;
                    postingFinal.ActualSeatNumber = hfOpened.Vacant - seatsLeft;

                    postingFinal.PostingVPMaster_Id = hfOpened.VPMaster_Id;
                    postingFinal.PostingHF_Id = hfOpened.HF_Id;
                    postingFinal.PostingHFMISCode = hfOpened.HFMISCode;
                    postingFinal.SeatNumber = postingFinal.ActualSeatNumber;

                    postingFinal.TotalSeats = hfOpened.Vacant;

                    postingFinal.IsActive = true;
                    postingFinal.DateTime = DateTime.UtcNow.AddHours(5);
                    postingFinal.Remarks = "Posted at preference number " + postingFinal.PreferenceNumber + " as " + merit.DesignationName + " in " + hfOpened.HFName;

                    db.AdhocPostingFinals.Add(postingFinal);
                    await db.SaveChangesAsync();

                    postingLog.Remarks = postingFinal.Remarks;
                    postingLog.Preference_Id = preference.Id;
                    postingLog.AdhocMeritLocked_Id = postingFinal.AdhocMeritLocked_Id;
                    postingLog.IsActive = true;
                    postingLog.DateTime = DateTime.UtcNow.AddHours(5);
                    db.AdhocPostingLogs.Add(postingLog);
                    await db.SaveChangesAsync();


                    if (hfOpened.HFMISCode.Substring(12, 3) == "014")
                    {
                        var moWMOVacantDb = await db.AdhocDistrictOpenedBHU2.FirstOrDefaultAsync(x => x.Id == moWMOVacant.Id);
                        if (designationId == 802)
                        {
                            moWMOVacantDb.BHUForMOLeft--;
                        }
                        if (designationId == 1320)
                        {
                            moWMOVacantDb.BHUForWMOLeft--;
                        }
                        db.Entry(moWMOVacantDb).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                    }

                    var hfOpenedDb = await db.AdhocHFOpened2.FirstOrDefaultAsync(x => x.Id == hfOpened.Id);
                    hfOpenedDb.SeatsLeft = seatsLeft;
                    db.Entry(hfOpenedDb).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return true;
                }
                else
                {
                    //Choosing Health Facility from nearest of Preference
                    var hf = await db.HFListPs.FirstOrDefaultAsync(x => x.Id == preference.JobHF_Id && x.IsActive == true);
                    if (hf != null)
                    {
                        postingLog = new AdhocPostingLog();
                        postingLog.Remarks = "Choosing Health Facility from nearest of Preference " + preference.HFName;
                        #region Near Bird Eye Distance
                        var designationIds = new List<int>();
                        designationIds.Add(designationId);
                        if (designationId == 802 || designationId == 1320)
                        {
                            designationIds.Add(2404);
                        }
                        var hfOpenedHFIds = await db.AdhocHFOpenedView2.AsNoTracking().Where(x => designationIds.Contains((int)x.Designation_Id) && x.DistrictCode.Equals(hf.DistrictCode) && x.SeatsLeft > 0 && x.SeatsLeft != null).Select(x => x.HF_Id).ToListAsync();
                        var hfs = await db.HFListPs.Where(x => hfOpenedHFIds.Contains(x.Id) && x.Latitude != null && x.Longitude != null && x.IsActive == true).ToListAsync();

                        List<DistanceAndId> hfDistanceAndIds = new List<DistanceAndId>();
                        foreach (var h in hfs)
                        {
                            if (h.Latitude != null && h.Longitude != null)
                            {
                                double distance = CalculateDistance(Convert.ToDouble(hf.Latitude),
                                Convert.ToDouble(hf.Longitude),
                                Convert.ToDouble(h.Latitude),
                                Convert.ToDouble(h.Longitude));
                                if (distance >= 0)
                                {
                                    if (h.HFTypeCode == "014")
                                    {
                                        if (designationId == 802 && moWMOVacant.BHUForMOLeft > 0)
                                        {
                                            hfDistanceAndIds.Add(new DistanceAndId() { Id = h.Id, Distance = distance, FacilityName = h.FullName });
                                        }
                                        else if (designationId == 1320 && moWMOVacant.BHUForWMOLeft > 0)
                                        {
                                            hfDistanceAndIds.Add(new DistanceAndId() { Id = h.Id, Distance = distance, FacilityName = h.FullName });
                                        }
                                    }
                                    else
                                    {
                                        hfDistanceAndIds.Add(new DistanceAndId() { Id = h.Id, Distance = distance, FacilityName = h.FullName });
                                    }
                                }
                            }
                        }
                        var distanceIds = hfDistanceAndIds.OrderBy(x => x.Distance).Select(x => x.Id).ToList();
                        HFListP nearestHF = null;
                        foreach (var hfDistanceAndId in distanceIds)
                        {
                            nearestHF = db.HFListPs.FirstOrDefault(x => x.Id == hfDistanceAndId);
                            if (nearestHF != null)
                            {
                                hfOpened = await db.AdhocHFOpenedView2.AsNoTracking().FirstOrDefaultAsync(x => x.HF_Id == nearestHF.Id && designationIds.Contains((int)x.Designation_Id) && x.IsActive == true);

                                var postedMerits = await db.AdhocPostingFinalViews.Where(x => x.PostingHF_Id == preference.JobHF_Id && x.IsActive == true)
                                    .Select(x => x.MeritNumber).ToListAsync();

                                if (hfOpened != null && hfOpened.SeatsLeft > 0 && hfOpened.SeatsLeft != null)
                                {
                                    seatsLeft = (int)hfOpened.SeatsLeft - 1;
                                    AdhocPostingFinal postingFinal = new AdhocPostingFinal();
                                    postingFinal.AdhocMeritLocked_Id = merit.Id;
                                    postingFinal.Preference_Id = preference.Id;
                                    postingFinal.PreferenceNumber = preference.PreferenceOrder;

                                    postingFinal.ActualVPMaster_Id = hfOpened.VPMaster_Id;
                                    postingFinal.ActualHF_Id = hfOpened.HF_Id;
                                    postingFinal.ActualHFMISCode = hfOpened.HFMISCode;
                                    postingFinal.ActualSeatNumber = hfOpened.Vacant - seatsLeft;

                                    postingFinal.PostingVPMaster_Id = hfOpened.VPMaster_Id;
                                    postingFinal.PostingHF_Id = hfOpened.HF_Id;
                                    postingFinal.PostingHFMISCode = hfOpened.HFMISCode;
                                    postingFinal.SeatNumber = postingFinal.ActualSeatNumber;

                                    postingFinal.TotalSeats = hfOpened.Vacant;

                                    postingFinal.IsActive = true;
                                    postingFinal.DateTime = DateTime.UtcNow.AddHours(5);
                                    postingFinal.Remarks = "Posted at nearest facility to preference number " + postingFinal.PreferenceNumber + " as " + merit.DesignationName + " in " + hfOpened.HFName;

                                    db.AdhocPostingFinals.Add(postingFinal);
                                    await db.SaveChangesAsync();

                                    postingLog.Remarks = postingFinal.Remarks;
                                    postingLog.Preference_Id = preference.Id;
                                    postingLog.AdhocMeritLocked_Id = postingFinal.AdhocMeritLocked_Id;
                                    postingLog.IsActive = true;
                                    postingLog.DateTime = DateTime.UtcNow.AddHours(5);
                                    db.AdhocPostingLogs.Add(postingLog);
                                    await db.SaveChangesAsync();

                                    if (hfOpened.HFMISCode.Substring(12, 3) == "014")
                                    {
                                        var moWMOVacantDb = await db.AdhocDistrictOpenedBHU2.FirstOrDefaultAsync(x => x.Id == moWMOVacant.Id);
                                        if (designationId == 802)
                                        {
                                            moWMOVacantDb.BHUForMOLeft--;
                                        }
                                        if (designationId == 1320)
                                        {
                                            moWMOVacantDb.BHUForWMOLeft--;
                                        }
                                        db.Entry(moWMOVacantDb).State = EntityState.Modified;
                                        await db.SaveChangesAsync();
                                    }

                                    var hfOpenedDb = await db.AdhocHFOpened2.FirstOrDefaultAsync(x => x.Id == hfOpened.Id);
                                    hfOpenedDb.SeatsLeft = seatsLeft;
                                    db.Entry(hfOpenedDb).State = EntityState.Modified;
                                    await db.SaveChangesAsync();
                                    return true;

                                }
                            }
                        }
                        #endregion
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpGet]
        [Route("AdhocPostingConsultants/{applicationId}/{designationId}")]
        public async Task<IHttpActionResult> AdhocPostingConsultants(int applicationId, int designationId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var application = new AdhocApplicationView();
                    bool nearest = false;
                    bool districtAllotted = false;
                    int countSeatsVacant = 1;
                    var designationIds = new List<int>();
                    designationIds.Add(designationId);
                    int totalVacant = 0;
                    AdhocDistrictOpenedBHUView moWMOVacant = new AdhocDistrictOpenedBHUView();
                    if (applicationId == 10007)
                    {

                        var districts = db.Districts.Where(x => !x.Name.StartsWith("isla") && !x.Name.StartsWith("Lahore")).OrderBy(x => x.Name).ToList();
                        foreach (var district in districts)
                        {
                            countSeatsVacant = 1;
                            totalVacant = 0;

                            totalVacant = (int?)db.AdhocHFOpenedViews.AsNoTracking().Where(x => x.DistrictCode.Equals(district.Code) && designationIds.Contains((int)x.Designation_Id ) && x.IsActive == true && x.BatchNo == 3).Select(x => x.SeatsLeft).Sum() ?? 0;
                            var merits = await db.AdhocMeritLockedViews.AsNoTracking().Where(x => x.IsPostedOnAdhoc == null && x.DistrictCode.Equals(district.Code) && x.Designation_Id == designationId && x.IsLocked == true && x.IsActive == true)
                                              .OrderBy(x => x.MeritNumber)
                                              .ToListAsync();
                            System.Diagnostics.Debug.WriteLine($"District {district.Name}");
                            foreach (var merit in merits)
                            {
                                if (merit.Application_Id == 3321)
                                {
                                    var app = 1;
                                }
                                if (merit.Application_Id == 323)
                                {
                                    var app = 1;
                                }
                                if (countSeatsVacant <= totalVacant)
                                {
                                    application = await db.AdhocApplicationViews.FirstOrDefaultAsync(x => x.Id == merit.Application_Id && x.IsActive == true);
                                    if (application != null)
                                    {

                                        countSeatsVacant++;
                                        if (merit.DistrictCodeAllotted == true || merit.DistrictCodeAllotted == null)
                                        {
                                            var posting = await db.AdhocPostingFinalViews.AsNoTracking().FirstOrDefaultAsync(x => x.Applicant_Id == merit.Applicant_Id && x.IsActive == true);
                                            if (posting == null)
                                            {
                                                if (application != null && application.Status_Id == 2)
                                                {
                                                    nearest = false;
                                                    var preferences = await db.AdhocApplicationPreferenceViews.AsNoTracking().Where(x => x.DistrictCode.Equals(district.Code) && x.JobApplication_Id == application.Id && x.IsActive == true).OrderBy(x => x.PreferenceOrder).ToListAsync();
                                                    moWMOVacant = db.AdhocDistrictOpenedBHUViews.AsNoTracking().FirstOrDefault(x => x.DistrictCode.Equals(district.Code));
                                                    int count = 0;
                                                    postedHfs = new List<string>();
                                                    foreach (var preference in preferences)
                                                    {
                                                        count++;
                                                        if (!postedHfs.Contains(preference.HFName))
                                                        {
                                                            int desigCompare = 802;

                                                            desigCompare = designationId;
                                                            var hfOpened = db.AdhocHFOpenedViews.AsNoTracking().FirstOrDefault(x => x.HF_Id == preference.JobHF_Id && x.Designation_Id == desigCompare && x.IsActive == true && x.BatchNo == 3);
                                                            if (hfOpened != null && hfOpened.SeatsLeft > 0 && hfOpened.SeatsLeft != null)
                                                            {
                                                                postedHfs.Add(hfOpened.HFName);
                                                                var res = await CheckAdhocPostingConsultant(db, preference, designationId, nearest, hfOpened, merit, moWMOVacant);

                                                                if (res == true)
                                                                {
                                                                    var app = await db.AdhocApplications.FirstOrDefaultAsync(x => x.Id == merit.Application_Id);
                                                                    app.IsPostedOnAdhoc = true;
                                                                    db.Entry(app).State = EntityState.Modified;
                                                                    await db.SaveChangesAsync();
                                                                    break;
                                                                }
                                                            }

                                                        }
                                                        if (count == preferences.Count)
                                                        {
                                                            nearest = true;
                                                        }
                                                    }
                                                    if (nearest == true)
                                                    {
                                                        count = 0;
                                                        postedHfs = new List<string>();
                                                        foreach (var preference in preferences)
                                                        {
                                                            count++;
                                                            if (!postedHfs.Contains(preference.HFName))
                                                            {
                                                                int desigCompare = 802;

                                                                desigCompare = designationId;
                                                                var hfOpened = await db.AdhocHFOpenedViews.AsNoTracking().FirstOrDefaultAsync(x => x.HF_Id == preference.JobHF_Id && x.Designation_Id == desigCompare && x.IsActive == true && x.BatchNo == 3);
                                                                if (hfOpened != null)
                                                                {
                                                                    postedHfs.Add(hfOpened.HFName);
                                                                    var res = await CheckAdhocPostingConsultant(db, preference, designationId, nearest, hfOpened, merit, moWMOVacant);
                                                                    if (res == true)
                                                                    {
                                                                        var app = await db.AdhocApplications.FirstOrDefaultAsync(x => x.Id == merit.Application_Id);
                                                                        app.IsPostedOnAdhoc = true;
                                                                        db.Entry(app).State = EntityState.Modified;
                                                                        await db.SaveChangesAsync();
                                                                        break;
                                                                    }
                                                                    else if (count >= preferences.Count)
                                                                    {
                                                                        //Not Posted
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //foreach (var m in merits.Where(x => x.MeritNumber > merit.MeritNumber).ToList())
                                            //{
                                            //    var mDb = db.AdhocMeritLockeds.FirstOrDefault(x => x.Id == m.Id);
                                            //    if (mDb != null)
                                            //    {
                                            //        mDb.MeritNumberChanging = mDb.MeritNumberChanging - 1;
                                            //        db.SaveChanges();
                                            //    }
                                            //}

                                            int meritChangesAffected = db.Database.ExecuteSqlCommand($" UPDATE AdhocMeritLockedView set  MeritNumberChanging = MeritNumberChanging - 1 where MeritNumber > {merit.MeritNumber} and Designation_Id = {merit.Designation_Id} and DistrictCode = '{merit.DistrictCode}';");
                                            System.Diagnostics.Debug.WriteLine($"Merit Changes Affected of Merit No {merit.MeritNumber} : {meritChangesAffected}");

                                        }
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
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


        public async Task<bool> CheckAdhocPostingConsultant(HR_System db, AdhocApplicationPreferenceView preference, int designationId, bool nearestPosting, AdhocHFOpenedView hfOpened, AdhocMeritLockedView merit, AdhocDistrictOpenedBHUView moWMOVacant)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                AdhocPostingLog postingLog = new AdhocPostingLog();
                int seatsLeft = 0;
                if (nearestPosting == false)
                {
                    seatsLeft = (int)hfOpened.SeatsLeft - 1;

                    AdhocPostingFinal postingFinal = new AdhocPostingFinal();
                    postingFinal.AdhocMeritLocked_Id = merit.Id;
                    postingFinal.Preference_Id = preference.Id;
                    postingFinal.PreferenceNumber = preference.PreferenceOrder;

                    postingFinal.ActualVPMaster_Id = hfOpened.VPMaster_Id;
                    postingFinal.ActualHF_Id = hfOpened.HF_Id;
                    postingFinal.ActualHFMISCode = hfOpened.HFMISCode;
                    postingFinal.ActualSeatNumber = hfOpened.Vacant - seatsLeft;

                    postingFinal.PostingVPMaster_Id = hfOpened.VPMaster_Id;
                    postingFinal.PostingHF_Id = hfOpened.HF_Id;
                    postingFinal.PostingHFMISCode = hfOpened.HFMISCode;
                    postingFinal.SeatNumber = postingFinal.ActualSeatNumber;

                    postingFinal.TotalSeats = hfOpened.Vacant;

                    postingFinal.IsActive = true;
                    postingFinal.DateTime = DateTime.UtcNow.AddHours(5);
                    postingFinal.Remarks = "Posted at preference number " + postingFinal.PreferenceNumber + " as " + merit.DesignationName + " in " + hfOpened.HFName;

                    db.AdhocPostingFinals.Add(postingFinal);
                    await db.SaveChangesAsync();

                    postingLog.Remarks = postingFinal.Remarks;
                    postingLog.Preference_Id = preference.Id;
                    postingLog.AdhocMeritLocked_Id = postingFinal.AdhocMeritLocked_Id;
                    postingLog.IsActive = true;
                    postingLog.DateTime = DateTime.UtcNow.AddHours(5);
                    db.AdhocPostingLogs.Add(postingLog);
                    await db.SaveChangesAsync();



                    var hfOpenedDb = await db.AdhocHFOpeneds.FirstOrDefaultAsync(x => x.Id == hfOpened.Id);
                    hfOpenedDb.SeatsLeft = seatsLeft;
                    db.Entry(hfOpenedDb).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return true;
                }
                else
                {
                    //Choosing Health Facility from nearest of Preference
                    var hf = await db.HFListPs.FirstOrDefaultAsync(x => x.Id == preference.JobHF_Id && x.IsActive == true);
                    if (hf != null)
                    {
                        postingLog = new AdhocPostingLog();
                        postingLog.Remarks = "Choosing Health Facility from nearest of Preference " + preference.HFName;
                        #region Near Bird Eye Distance
                        var designationIds = new List<int>();
                        designationIds.Add(designationId);
                        if (designationId == 802 || designationId == 1320)
                        {
                            designationIds.Add(2404);
                        }
                        var hfOpenedHFIds = await db.AdhocHFOpenedViews.AsNoTracking().Where(x => designationIds.Contains((int)x.Designation_Id) && x.DistrictCode.Equals(hf.DistrictCode) && x.SeatsLeft > 0 && x.SeatsLeft != null && x.BatchNo == 3).Select(x => x.HF_Id).ToListAsync();
                        var hfs = await db.HFListPs.Where(x => hfOpenedHFIds.Contains(x.Id) && x.Latitude != null && x.Longitude != null && x.IsActive == true).ToListAsync();

                        List<DistanceAndId> hfDistanceAndIds = new List<DistanceAndId>();
                        foreach (var h in hfs)
                        {
                            if (h.Latitude != null && h.Longitude != null)
                            {
                                double distance = CalculateDistance(Convert.ToDouble(hf.Latitude),
                                Convert.ToDouble(hf.Longitude),
                                Convert.ToDouble(h.Latitude),
                                Convert.ToDouble(h.Longitude));
                                if (distance >= 0)
                                {

                                    hfDistanceAndIds.Add(new DistanceAndId() { Id = h.Id, Distance = distance, FacilityName = h.FullName });
                                }
                            }
                        }
                        var distanceIds = hfDistanceAndIds.OrderBy(x => x.Distance).Select(x => x.Id).ToList();
                        HFListP nearestHF = null;
                        foreach (var hfDistanceAndId in distanceIds)
                        {
                            nearestHF = db.HFListPs.FirstOrDefault(x => x.Id == hfDistanceAndId);
                            if (nearestHF != null)
                            {
                                hfOpened = await db.AdhocHFOpenedViews.AsNoTracking().FirstOrDefaultAsync(x => x.HF_Id == nearestHF.Id && designationIds.Contains((int)x.Designation_Id) && x.IsActive == true);

                                var postedMerits = await db.AdhocPostingFinalViews.Where(x => x.PostingHF_Id == preference.JobHF_Id && x.IsActive == true)
                                    .Select(x => x.MeritNumber).ToListAsync();

                                if (hfOpened != null && hfOpened.SeatsLeft > 0 && hfOpened.SeatsLeft != null)
                                {
                                    seatsLeft = (int)hfOpened.SeatsLeft - 1;
                                    AdhocPostingFinal postingFinal = new AdhocPostingFinal();
                                    postingFinal.AdhocMeritLocked_Id = merit.Id;
                                    postingFinal.Preference_Id = preference.Id;
                                    postingFinal.PreferenceNumber = preference.PreferenceOrder;

                                    postingFinal.ActualVPMaster_Id = hfOpened.VPMaster_Id;
                                    postingFinal.ActualHF_Id = hfOpened.HF_Id;
                                    postingFinal.ActualHFMISCode = hfOpened.HFMISCode;
                                    postingFinal.ActualSeatNumber = hfOpened.Vacant - seatsLeft;

                                    postingFinal.PostingVPMaster_Id = hfOpened.VPMaster_Id;
                                    postingFinal.PostingHF_Id = hfOpened.HF_Id;
                                    postingFinal.PostingHFMISCode = hfOpened.HFMISCode;
                                    postingFinal.SeatNumber = postingFinal.ActualSeatNumber;

                                    postingFinal.TotalSeats = hfOpened.Vacant;

                                    postingFinal.IsActive = true;
                                    postingFinal.DateTime = DateTime.UtcNow.AddHours(5);
                                    postingFinal.Remarks = "Posted at nearest facility to preference number " + postingFinal.PreferenceNumber + " as " + merit.DesignationName + " in " + hfOpened.HFName;

                                    db.AdhocPostingFinals.Add(postingFinal);
                                    await db.SaveChangesAsync();

                                    postingLog.Remarks = postingFinal.Remarks;
                                    postingLog.Preference_Id = preference.Id;
                                    postingLog.AdhocMeritLocked_Id = postingFinal.AdhocMeritLocked_Id;
                                    postingLog.IsActive = true;
                                    postingLog.DateTime = DateTime.UtcNow.AddHours(5);
                                    db.AdhocPostingLogs.Add(postingLog);
                                    await db.SaveChangesAsync();

                                    var hfOpenedDb = await db.AdhocHFOpeneds.FirstOrDefaultAsync(x => x.Id == hfOpened.Id);
                                    hfOpenedDb.SeatsLeft = seatsLeft;
                                    db.Entry(hfOpenedDb).State = EntityState.Modified;
                                    await db.SaveChangesAsync();
                                    return true;

                                }
                            }
                        }
                        #endregion
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpGet]
        [Route("AdhocPostingSMOSWMOSingle/{applicationId}/{designationId}/{min}/{max}")]
        public async Task<IHttpActionResult> AdhocPostingSMOSWMOSingle(int applicationId, int designationId, int min, int max)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var application = new AdhocApplicationView();
                    bool nearest = false;
                    bool districtAllotted = false;
                    int countSeatsVacant = 1;
                    var designationIds = new List<int>();
                    designationIds.Add(designationId);
                    int totalVacant = 0;
                    AdhocDistrictOpenedBHUView2 moWMOVacant = new AdhocDistrictOpenedBHUView2();
                    if (applicationId == 10007)
                    {

                        var districts = db.Districts.Where(x => !x.Name.StartsWith("isla") && !x.Name.StartsWith("Lahore")).OrderBy(x => x.Name).ToList();
                        foreach (var district in districts)
                        {
                            countSeatsVacant = 1;
                            totalVacant = 0;
                            if (designationId == 802 || designationId == 1320)
                            {
                                var vacant = db.AdhocHFOpenedView2.AsNoTracking().Where(x => x.DistrictCode.Equals(district.Code) && x.Designation_Id == designationId && x.IsActive == true).Select(x => x.SeatsLeft).ToList();

                                var sanctioned = db.AdhocHFOpenedView2.AsNoTracking().Where(x => x.DistrictCode.Equals(district.Code) && x.Designation_Id == designationId && x.IsActive == true).Select(x => x.SanctionVacant).ToList();
                                double number = sanctioned.Sum() ?? 0;
                                int numbers = (int)Math.Round(number, 0);
                                // int intPart = (int)number;
                                // double fractionalPart = number - intPart;
                                totalVacant = numbers;
                                //var ndess = fractionalPart > .46 ? intPart + 1 : intPart;
                            }
                            var merits = await db.AdhocMeritLockedViews.AsNoTracking().Where(x => x.IsPostedOnAdhoc == null && x.DistrictCode.Equals(district.Code) && x.Designation_Id == designationId && x.IsLocked == true && x.IsActive == true)
                                              .OrderBy(x => x.MeritNumber)
                                              .ToListAsync();
                            System.Diagnostics.Debug.WriteLine($"District {district.Name}");
                            foreach (var merit in merits)
                            {
                                if (merit.Application_Id == 1585)
                                {
                                    var app = 1;
                                }
                                if (merit.Application_Id == 323)
                                {
                                    var app = 1;
                                }
                                if (countSeatsVacant <= totalVacant)
                                {
                                    application = await db.AdhocApplicationViews.FirstOrDefaultAsync(x => x.Id == merit.Application_Id && x.IsActive == true);
                                    if (application != null)
                                    {
                                        if (!string.IsNullOrEmpty(application.Gender) && application.Designation_Id == 1320 && !application.Gender.ToLower().Equals("female"))
                                        {
                                        }
                                        else if (!string.IsNullOrEmpty(application.Gender) && application.Designation_Id == 802 && !application.Gender.ToLower().Equals("male"))
                                        {
                                        }
                                        else
                                        {
                                            countSeatsVacant++;
                                            if (merit.DistrictCodeAllotted == true || merit.DistrictCodeAllotted == null)
                                            {
                                                var posting = await db.AdhocPostingFinalViews.AsNoTracking().FirstOrDefaultAsync(x => x.Applicant_Id == merit.Applicant_Id && x.IsActive == true);
                                                if (posting == null)
                                                {
                                                    if (application != null && application.Status_Id == 2)
                                                    {
                                                        nearest = false;
                                                        var preferences = await db.AdhocApplicationPreferenceViews.AsNoTracking().Where(x => x.DistrictCode.Equals(district.Code) && x.JobApplication_Id == application.Id && x.IsActive == true).OrderBy(x => x.PreferenceOrder).ToListAsync();
                                                        moWMOVacant = db.AdhocDistrictOpenedBHUView2.AsNoTracking().FirstOrDefault(x => x.DistrictCode.Equals(district.Code));
                                                        int count = 0;
                                                        postedHfs = new List<string>();
                                                        foreach (var preference in preferences)
                                                        {
                                                            count++;
                                                            if (!postedHfs.Contains(preference.HFName))
                                                            {
                                                                int desigCompare = 802;
                                                                if (preference.HFMISCode.Substring(12, 3) == "014")
                                                                {
                                                                    if (designationId == 802)
                                                                    {
                                                                        if (count == preferences.Count)
                                                                        {
                                                                            nearest = true;
                                                                        }
                                                                        continue;
                                                                    }
                                                                    else if (designationId == 1320)
                                                                    {
                                                                        if (count == preferences.Count)
                                                                        {
                                                                            nearest = true;
                                                                        }
                                                                        continue;
                                                                    }
                                                                    desigCompare = 2404;
                                                                }
                                                                else
                                                                {
                                                                    desigCompare = designationId;
                                                                }
                                                                var hfOpened = db.AdhocHFOpenedView2.AsNoTracking().FirstOrDefault(x => x.HF_Id == preference.JobHF_Id && x.Designation_Id == desigCompare && x.IsActive == true);
                                                                if (hfOpened != null && hfOpened.SeatsLeft > 0 && hfOpened.SeatsLeft != null)
                                                                {
                                                                    postedHfs.Add(hfOpened.HFName);
                                                                    var res = await CheckAdhocPostingSMOSWMO(db, preference, designationId, nearest, hfOpened, merit, moWMOVacant);

                                                                    if (res == true)
                                                                    {
                                                                        var app = await db.AdhocApplications.FirstOrDefaultAsync(x => x.Id == merit.Application_Id);
                                                                        app.IsPostedOnAdhoc = true;
                                                                        db.Entry(app).State = EntityState.Modified;
                                                                        await db.SaveChangesAsync();
                                                                        break;
                                                                    }
                                                                }

                                                            }
                                                            if (count == preferences.Count)
                                                            {
                                                                nearest = true;
                                                            }
                                                        }
                                                        if (nearest == true)
                                                        {
                                                            count = 0;
                                                            postedHfs = new List<string>();
                                                            foreach (var preference in preferences)
                                                            {
                                                                count++;
                                                                if (!postedHfs.Contains(preference.HFName))
                                                                {
                                                                    int desigCompare = 802;
                                                                    
                                                                        desigCompare = designationId;
                                                                    
                                                                    var hfOpened = await db.AdhocHFOpenedView2.AsNoTracking().FirstOrDefaultAsync(x => x.HF_Id == preference.JobHF_Id && x.Designation_Id == desigCompare && x.IsActive == true);
                                                                    if (hfOpened != null)
                                                                    {
                                                                        postedHfs.Add(hfOpened.HFName);
                                                                        var res = await CheckAdhocPostingSMOSWMO(db, preference, designationId, nearest, hfOpened, merit, moWMOVacant);
                                                                        if (res == true)
                                                                        {
                                                                            var app = await db.AdhocApplications.FirstOrDefaultAsync(x => x.Id == merit.Application_Id);
                                                                            app.IsPostedOnAdhoc = true;
                                                                            db.Entry(app).State = EntityState.Modified;
                                                                            await db.SaveChangesAsync();
                                                                            break;
                                                                        }
                                                                        else if (count >= preferences.Count)
                                                                        {
                                                                            //Not Posted
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //foreach (var m in merits.Where(x => x.MeritNumber > merit.MeritNumber).ToList())
                                                //{
                                                //    var mDb = db.AdhocMeritLockeds.FirstOrDefault(x => x.Id == m.Id);
                                                //    if (mDb != null)
                                                //    {
                                                //        mDb.MeritNumberChanging = mDb.MeritNumberChanging - 1;
                                                //        db.SaveChanges();
                                                //    }
                                                //}

                                                int meritChangesAffected = db.Database.ExecuteSqlCommand($" UPDATE AdhocMeritLockedView set  MeritNumberChanging = MeritNumberChanging - 1 where MeritNumber > {merit.MeritNumber} and Designation_Id = {merit.Designation_Id} and DistrictCode = '{merit.DistrictCode}';");
                                                System.Diagnostics.Debug.WriteLine($"Merit Changes Affected of Merit No {merit.MeritNumber} : {meritChangesAffected}");

                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
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

        public async Task<bool> CheckAdhocPostingSMOSWMO(HR_System db, AdhocApplicationPreferenceView preference, int designationId, bool nearestPosting, AdhocHFOpenedView2 hfOpened, AdhocMeritLockedView merit, AdhocDistrictOpenedBHUView2 moWMOVacant)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                AdhocPostingLog postingLog = new AdhocPostingLog();
                int seatsLeft = 0;
                if (nearestPosting == false)
                {
                    seatsLeft = (int)hfOpened.SeatsLeft - 1;

                    AdhocPostingFinal postingFinal = new AdhocPostingFinal();
                    postingFinal.AdhocMeritLocked_Id = merit.Id;
                    postingFinal.Preference_Id = preference.Id;
                    postingFinal.PreferenceNumber = preference.PreferenceOrder;

                    postingFinal.ActualVPMaster_Id = hfOpened.VPMaster_Id;
                    postingFinal.ActualHF_Id = hfOpened.HF_Id;
                    postingFinal.ActualHFMISCode = hfOpened.HFMISCode;
                    postingFinal.ActualSeatNumber = hfOpened.Vacant - seatsLeft;

                    postingFinal.PostingVPMaster_Id = hfOpened.VPMaster_Id;
                    postingFinal.PostingHF_Id = hfOpened.HF_Id;
                    postingFinal.PostingHFMISCode = hfOpened.HFMISCode;
                    postingFinal.SeatNumber = postingFinal.ActualSeatNumber;

                    postingFinal.TotalSeats = hfOpened.Vacant;

                    postingFinal.IsActive = true;
                    postingFinal.DateTime = DateTime.UtcNow.AddHours(5);
                    postingFinal.Remarks = "Posted at preference number " + postingFinal.PreferenceNumber + " as " + merit.DesignationName + " in " + hfOpened.HFName;

                    db.AdhocPostingFinals.Add(postingFinal);
                    await db.SaveChangesAsync();

                    postingLog.Remarks = postingFinal.Remarks;
                    postingLog.Preference_Id = preference.Id;
                    postingLog.AdhocMeritLocked_Id = postingFinal.AdhocMeritLocked_Id;
                    postingLog.IsActive = true;
                    postingLog.DateTime = DateTime.UtcNow.AddHours(5);
                    db.AdhocPostingLogs.Add(postingLog);
                    await db.SaveChangesAsync();


                    //if (hfOpened.HFMISCode.Substring(12, 3) == "014")
                    //{
                    //    var moWMOVacantDb = await db.AdhocDistrictOpenedBHU2.FirstOrDefaultAsync(x => x.Id == moWMOVacant.Id);
                    //    if (designationId == 802)
                    //    {
                    //        moWMOVacantDb.BHUForMOLeft--;
                    //    }
                    //    if (designationId == 1320)
                    //    {
                    //        moWMOVacantDb.BHUForWMOLeft--;
                    //    }
                    //    db.Entry(moWMOVacantDb).State = EntityState.Modified;
                    //    await db.SaveChangesAsync();
                    //}

                    var hfOpenedDb = await db.AdhocHFOpened2.FirstOrDefaultAsync(x => x.Id == hfOpened.Id);
                    hfOpenedDb.SeatsLeft = seatsLeft;
                    db.Entry(hfOpenedDb).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return true;
                }
                else
                {
                    //Choosing Health Facility from nearest of Preference
                    var hf = await db.HFListPs.FirstOrDefaultAsync(x => x.Id == preference.JobHF_Id && x.IsActive == true);
                    if (hf != null)
                    {
                        postingLog = new AdhocPostingLog();
                        postingLog.Remarks = "Choosing Health Facility from nearest of Preference " + preference.HFName;
                        #region Near Bird Eye Distance
                        var designationIds = new List<int>();
                        designationIds.Add(designationId);
                        if (designationId == 802 || designationId == 1320)
                        {
                            designationIds.Add(2404);
                        }
                        var hfOpenedHFIds = await db.AdhocHFOpenedView2.AsNoTracking().Where(x => designationIds.Contains((int)x.Designation_Id) && x.DistrictCode.Equals(hf.DistrictCode) && x.SeatsLeft > 0 && x.SeatsLeft != null).Select(x => x.HF_Id).ToListAsync();
                        var hfs = await db.HFListPs.Where(x => hfOpenedHFIds.Contains(x.Id) && x.Latitude != null && x.Longitude != null && x.IsActive == true).ToListAsync();

                        List<DistanceAndId> hfDistanceAndIds = new List<DistanceAndId>();
                        foreach (var h in hfs)
                        {
                            if (h.Latitude != null && h.Longitude != null)
                            {
                                double distance = CalculateDistance(Convert.ToDouble(hf.Latitude),
                                Convert.ToDouble(hf.Longitude),
                                Convert.ToDouble(h.Latitude),
                                Convert.ToDouble(h.Longitude));
                                if (distance >= 0)
                                {

                                    hfDistanceAndIds.Add(new DistanceAndId() { Id = h.Id, Distance = distance, FacilityName = h.FullName });
                                }
                            }
                        }
                        var distanceIds = hfDistanceAndIds.OrderBy(x => x.Distance).Select(x => x.Id).ToList();
                        HFListP nearestHF = null;
                        foreach (var hfDistanceAndId in distanceIds)
                        {
                            nearestHF = db.HFListPs.FirstOrDefault(x => x.Id == hfDistanceAndId);
                            if (nearestHF != null)
                            {
                                hfOpened = await db.AdhocHFOpenedView2.AsNoTracking().FirstOrDefaultAsync(x => x.HF_Id == nearestHF.Id && designationIds.Contains((int)x.Designation_Id) && x.IsActive == true);

                                var postedMerits = await db.AdhocPostingFinalViews.Where(x => x.PostingHF_Id == preference.JobHF_Id && x.IsActive == true)
                                    .Select(x => x.MeritNumber).ToListAsync();

                                if (hfOpened != null && hfOpened.SeatsLeft > 0 && hfOpened.SeatsLeft != null)
                                {
                                    seatsLeft = (int)hfOpened.SeatsLeft - 1;
                                    AdhocPostingFinal postingFinal = new AdhocPostingFinal();
                                    postingFinal.AdhocMeritLocked_Id = merit.Id;
                                    postingFinal.Preference_Id = preference.Id;
                                    postingFinal.PreferenceNumber = preference.PreferenceOrder;

                                    postingFinal.ActualVPMaster_Id = hfOpened.VPMaster_Id;
                                    postingFinal.ActualHF_Id = hfOpened.HF_Id;
                                    postingFinal.ActualHFMISCode = hfOpened.HFMISCode;
                                    postingFinal.ActualSeatNumber = hfOpened.Vacant - seatsLeft;

                                    postingFinal.PostingVPMaster_Id = hfOpened.VPMaster_Id;
                                    postingFinal.PostingHF_Id = hfOpened.HF_Id;
                                    postingFinal.PostingHFMISCode = hfOpened.HFMISCode;
                                    postingFinal.SeatNumber = postingFinal.ActualSeatNumber;

                                    postingFinal.TotalSeats = hfOpened.Vacant;

                                    postingFinal.IsActive = true;
                                    postingFinal.DateTime = DateTime.UtcNow.AddHours(5);
                                    postingFinal.Remarks = "Posted at nearest facility to preference number " + postingFinal.PreferenceNumber + " as " + merit.DesignationName + " in " + hfOpened.HFName;

                                    db.AdhocPostingFinals.Add(postingFinal);
                                    await db.SaveChangesAsync();

                                    postingLog.Remarks = postingFinal.Remarks;
                                    postingLog.Preference_Id = preference.Id;
                                    postingLog.AdhocMeritLocked_Id = postingFinal.AdhocMeritLocked_Id;
                                    postingLog.IsActive = true;
                                    postingLog.DateTime = DateTime.UtcNow.AddHours(5);
                                    db.AdhocPostingLogs.Add(postingLog);
                                    await db.SaveChangesAsync();

                                    //if (hfOpened.HFMISCode.Substring(12, 3) == "014")
                                    //{
                                    //    var moWMOVacantDb = await db.AdhocDistrictOpenedBHU2.FirstOrDefaultAsync(x => x.Id == moWMOVacant.Id);
                                    //    if (designationId == 802)
                                    //    {
                                    //        moWMOVacantDb.BHUForMOLeft--;
                                    //    }
                                    //    if (designationId == 1320)
                                    //    {
                                    //        moWMOVacantDb.BHUForWMOLeft--;
                                    //    }
                                    //    db.Entry(moWMOVacantDb).State = EntityState.Modified;
                                    //    await db.SaveChangesAsync();
                                    //}

                                    var hfOpenedDb = await db.AdhocHFOpened2.FirstOrDefaultAsync(x => x.Id == hfOpened.Id);
                                    hfOpenedDb.SeatsLeft = seatsLeft;
                                    db.Entry(hfOpenedDb).State = EntityState.Modified;
                                    await db.SaveChangesAsync();
                                    return true;

                                }
                            }
                        }
                        #endregion
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        [HttpGet]
        [Route("CheckAdhocInterviewSMS/{interviewBatchId}")]
        public IHttpActionResult CheckAdhocInterviewSMS(int interviewBatchId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var adhocInterviewBatch = db.AdhocInterviewBatches.FirstOrDefault(x => x.Id == interviewBatchId && x.IsActive == true);
                    if (adhocInterviewBatch != null)
                    {
                        var adhocInterviewBatchApplications = db.AdhocInterviewBatchApplications.Where(x => x.Batch_Id == adhocInterviewBatch.Id && x.IsActive == true).ToList();
                        foreach (var batchApp in adhocInterviewBatchApplications)
                        {
                            var app = db.AdhocApplicationViews.FirstOrDefault(x => x.Id == batchApp.Application_Id && x.IsActive == true);
                            if (app != null)
                            {

                                var smsLog = Common.CheckMessageStatus((int)batchApp.SMSLog_Id);
                                if (smsLog != null)
                                {
                                    batchApp.SMS_Status = smsLog.Status;
                                    batchApp.SMS_StatusCheckTime = DateTime.UtcNow.AddHours(5);
                                    db.Entry(batchApp).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }

                        }
                    }
                    return Ok(false);
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

        [HttpGet]
        [Route("ActivePosting")]
        public IHttpActionResult ActivePosting()
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var postings = db.AdhocPostingFinals.ToList();
                    foreach (var posting in postings)
                    {
                        posting.IsActive = true;
                        db.SaveChanges();
                    }
                    return Ok(false);
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

        [HttpGet]
        [Route("GetAdhocInterview/{districtCode}/{designationId}")]
        public IHttpActionResult GetAdhocInterview(string districtCode, int designationId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var adhocInterview = db.AdhocInterviewViews.FirstOrDefault(x => x.DistrictCode.Equals(districtCode) && designationId == x.Designation_Id && x.IsActive == true);
                    return Ok(adhocInterview);
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
        [HttpGet]
        [Route("GetAdhocInterviews/{districtCode}")]
        public IHttpActionResult GetAdhocInterviews(string districtCode)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var adhocInterview = db.AdhocInterviewViews.Where(x => x.DistrictCode.StartsWith(districtCode) && x.IsActive == true).ToList();
                    return Ok(adhocInterview);
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
        [HttpGet]
        [Route("GetAdhocInterviewBatches/{interviewId}")]
        public IHttpActionResult GetAdhocInterviewBatches(int interviewId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var adhocInterviewBatches = db.AdhocInterviewBatches.Where(x => x.Interview_Id == interviewId && x.IsActive == true).ToList();
                    return Ok(adhocInterviewBatches);
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

        [HttpGet]
        [Route("GetAdhocInterviewBatchApplications/{interviewBatchId}")]
        public IHttpActionResult GetAdhocInterviewBatchApplications(int interviewBatchId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var adhocInterviewBatchApplications = db.AdhocInterviewBatchApplicationVs.Where(x => x.Batch_Id == interviewBatchId && x.IsActive == true).OrderBy(x => x.Id).ToList();
                    return Ok(adhocInterviewBatchApplications);
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
        [HttpPost]
        [Route("SearchAdhocInterviewBatchApplications")]
        public IHttpActionResult SearchAdhocInterviewBatchApplications([FromBody] AdhocApplicaitionFilters filter)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var query = db.AdhocInterviewBatchApplicationVs.Where(x => x.Batch_Id == filter.batchId && x.IsActive == true).AsQueryable();

                    if (filter.Status_Id > 0)
                    {
                        if (filter.Status_Id == 1 || filter.Status_Id == 2)
                        {
                            bool isPresent = filter.Status_Id == 1 ? true : false;
                            query = query.Where(x => x.IsPresent == isPresent).AsQueryable();
                        }
                        if (filter.Status_Id == 3)
                        {
                            query = query.Where(x => x.InterviewMarks >= 0).AsQueryable();
                        }
                        if (filter.Status_Id == 4)
                        {
                            query = query.Where(x => x.IsRejected == true).AsQueryable();
                        }
                        if (filter.Status_Id == 5)
                        {
                            query = query.Where(x => x.IsPresent == null).AsQueryable();
                        }
                        if (filter.Status_Id == 6)
                        {
                            query = query.Where(x => x.IsLocked == true).AsQueryable();
                        }
                        if (filter.Status_Id == 7)
                        {
                            query = query.Where(x => x.IsLocked == null).AsQueryable();
                        }
                    }
                    if (filter.Query != null && filter.Query.Length >= 2)
                    {
                        filter.Skip = 0;
                        var isNumber = int.TryParse(filter.Query.Replace(" ", ""), out int tracking);

                        if (new RootService().IsCNIC(filter.Query.Replace(" ", "")))
                        {
                            filter.Query = filter.Query.Replace("-", "");
                            query = query.Where(x => x.CNIC.Equals(filter.Query)).AsQueryable();
                        }
                        else if (isNumber)
                        {
                            var number = Convert.ToInt32(filter.Query);
                            query = query.Where(x => x.Application_Id == number).AsQueryable();
                        }
                        else
                        {
                            query = query.Where(x => x.Name.ToLower().Contains(filter.Query.ToLower())).AsQueryable();
                        }
                    }
                    var applications = query.OrderBy(l => l.CreatedDate).ToList();
                    return Ok(applications);
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
        [HttpGet]
        [Route("GetAdhocInterviewBatchApplication/{id}")]
        public IHttpActionResult GetAdhocInterviewBatchApplication(int id)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var adhocInterviewBatchApplication = db.AdhocInterviewBatchApplicationVs.FirstOrDefault(x => x.Id == id && x.IsActive == true);
                    return Ok(adhocInterviewBatchApplication);
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
        [HttpPost]
        [Route("SaveAdhocApplicationVerification")]
        public IHttpActionResult SaveAdhocApplicationVerification([FromBody] AdhocApplicationVerification adhocApplicationVerification)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    if (adhocApplicationVerification.Id == 0)
                    {
                        adhocApplicationVerification.CreatedDate = DateTime.UtcNow.AddHours(5);
                        adhocApplicationVerification.UserId = User.Identity.GetUserId();
                        adhocApplicationVerification.CreatedBy = User.Identity.GetUserName();
                        db.AdhocApplicationVerifications.Add(adhocApplicationVerification);
                        db.SaveChanges();
                    }
                    else if (adhocApplicationVerification.Id > 0)
                    {
                        var adhocVerification = db.AdhocApplicationVerifications.FirstOrDefault(x => x.Id == adhocApplicationVerification.Id);
                        if (adhocVerification != null)
                        {
                            adhocVerification.IsActive = false;
                            db.Entry(adhocVerification).State = EntityState.Modified;
                            db.SaveChanges();
                            return Ok(true);
                        }
                    }
                    return Ok(adhocApplicationVerification);
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
        [HttpGet]
        [Route("GetAdhocInterviewVerifications/{applicationId}/{batchApplicationId}")]
        public IHttpActionResult GetAdhocInterviewVerifications(int applicationId, int batchApplicationId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var adhocApplicationVerifications = db.AdhocApplicationVerifications.Where(x => x.Application_Id == applicationId && x.BatchApplication_Id == batchApplicationId && x.IsActive == true).ToList();
                    return Ok(adhocApplicationVerifications);
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
        [HttpGet]
        [Route("GetAdhocInterviewApplications/{applicantId}")]
        public IHttpActionResult GetAdhocInterviewApplications(int applicantId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var adhocInterviewApplications = db.AdhocInterviewBatchApplicationVs.Where(x => x.Applicant_Id == applicantId && x.IsActive == true).OrderBy(x => x.Id).ToList();
                    return Ok(adhocInterviewApplications);
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
        [HttpGet]
        [Route("GetAdhocPresentApplications/{applicantId}")]
        public IHttpActionResult GetAdhocPresentApplications(int applicantId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var adhocInterviewApplications = db.AdhocInterviewBatchApplicationVs.Where(x => x.Applicant_Id == applicantId && x.IsPresent == true && x.IsLocked == true && x.IsRejected != true && x.IsActive == true).OrderBy(x => x.Id).ToList();
                    return Ok(adhocInterviewApplications);
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
        [HttpGet]
        [Route("GetAdhocApplicationMerit/{applicationId}")]
        public IHttpActionResult GetAdhocApplicationMerit(int applicationId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var adhocInterviews = db.AdhocInterviewBatchApplicationVs.Where(x => x.Application_Id == applicationId && x.IsActive == true).OrderByDescending(x => x.IsPresent).ToList();
                    var adhocMerits = db.AdhocMeritLockedViews.Where(x => x.Application_Id == applicationId && x.IsActive == true).OrderBy(x => x.MeritNumber).ToList();
                    var adhocPostings = db.AdhocPostingFinalViews.FirstOrDefault(x => x.Application_Id == applicationId && x.IsActive == true);
                    return Ok(new { adhocInterviews, adhocMerits, adhocPostings });
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
        [HttpGet]
        [Route("GetAdhocMLVL/{DistrictCode}/{DesignationId}")]
        public IHttpActionResult GetAdhocMLVL(string DistrictCode, int DesignationId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var AdhocMeritLockeds = db.AdhocMeritLockedViews.Where(x => x.DistrictCode.Equals(DistrictCode) && x.Designation_Id == DesignationId && x.IsActive == true).OrderBy(x => x.MeritNumber).ToList();
                    return Ok(AdhocMeritLockeds);
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
        [HttpPost]
        [Route("SaveScrutinyMinutes")]
        public IHttpActionResult SaveScrutinyMinutes([FromBody] AdhocScrutinyMinute adhocScrutinyMinute)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var adhocScrutinyMinuteDb = db.AdhocScrutinyMinutes.FirstOrDefault(x => x.DistrictCode.Equals(adhocScrutinyMinute.DistrictCode) && adhocScrutinyMinute.Designation_Id == x.Designation_Id && x.IsActive == true);
                    if (adhocScrutinyMinuteDb != null)
                    {
                        return Ok(adhocScrutinyMinuteDb);
                    }
                    else
                    {
                        adhocScrutinyMinute.Printed = true;
                        adhocScrutinyMinute.IsActive = true;
                        adhocScrutinyMinute.CreatedDate = DateTime.UtcNow.AddHours(5);
                        adhocScrutinyMinute.UserId = User.Identity.GetUserId();
                        adhocScrutinyMinute.CreatedBy = User.Identity.GetUserName();
                        db.AdhocScrutinyMinutes.Add(adhocScrutinyMinute);
                        db.SaveChanges();
                    }
                    return Ok(adhocScrutinyMinute);
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
        [HttpPost]
        [Route("GetScrutinyMinutes")]
        public IHttpActionResult GetScrutinyMinutes([FromBody] AdhocScrutinyMinute adhocScrutinyMinute)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var adhocScrutinyMinuteDb = db.AdhocScrutinyMinutes.FirstOrDefault(x => x.DistrictCode.Equals(adhocScrutinyMinute.DistrictCode) && adhocScrutinyMinute.Designation_Id == x.Designation_Id && x.IsActive == true);
                    return Ok(adhocScrutinyMinuteDb);
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
        [HttpGet]
        [Route("GetApplicant/{cnic}")]
        public IHttpActionResult GetApplicant(string cnic)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var applicant = db.AdhocApplicantViews.FirstOrDefault(x => x.CNIC.Equals(cnic) && x.IsActive == true);
                    return Ok(applicant);
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

        [HttpGet]
        [Route("GetDocuments")]
        public IHttpActionResult GetDocuments()
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var documents = db.AdhocDocuments.Where(x => x.IsActive == true).ToList();
                    return Ok(documents);
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
        //[HttpGet]
        //[Route("GetadhocApplicationMarks/{applicationId}")]
        //public IHttpActionResult GetadhocApplicationMarks(int applicationId)
        //{
        //    try
        //    {
        //        using (var db = new HR_System())
        //        {
        //            db.Configuration.ProxyCreationEnabled = false;
        //            var marks = db.AdhocApplicationMarks.Where(x => x.Application_Id == applicationId && x.IsActive == true).ToList();
        //            return Ok(marks);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        while (ex.InnerException != null)
        //        {
        //            ex = ex.InnerException;
        //        }
        //        Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
        //    }
        //}
        [HttpGet]
        [Route("GetAdhocDistrictMerit/{designationId}/{districtCode}")]
        public IHttpActionResult GetAdhocDistrictMerit(int designationId, string districtCode)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var interview = db.AdhocInterviewViews.FirstOrDefault(x => x.Designation_Id == designationId && x.DistrictCode.StartsWith(districtCode) && x.IsActive == true);
                    if (interview != null)
                    {
                        var applicationIds = db.AdhocInterviewBatchApplicationVs.Where(x => x.InterviewId == interview.Id && x.IsActive == true).Select(x => x.Application_Id).ToList();
                        var batchApplicationIds = db.AdhocInterviewBatchApplicationVs.Where(x => x.InterviewId == interview.Id && x.IsPresent == true && x.IsLocked == true && x.IsRejected != true && x.IsActive == true).Select(x => x.Id).ToList();
                        //var rejectedIds = db.AdhocApplicationMarksViews.Where(x => batchApplicationIds.Contains((int)x.BatchApplicationId) && (x.Remarks == "No Scrutiny Found!" || x.Error == true) && x.IsActive == true).GroupBy(x => new { x.BatchApplicationId }).Select(x => x.Key.BatchApplicationId).ToList();

                        //&& !rejectedIds.Contains(x.Application_Id)
                        var marks = db.AdhocApplicationMarksViews.Where(x => batchApplicationIds.Contains((int)x.BatchApplicationId) && x.IsActive == true).ToList();
                        var applicantIds = db.AdhocApplicationMarksViews.Where(x => applicationIds.Contains(x.Application_Id) && x.IsActive == true).GroupBy(x => new { x.Applicant_Id }).Select(x => x.Key.Applicant_Id).ToList();
                        var applicantQualifications = db.AdhocApplicantQualificationViews.Where(x => applicantIds.Contains(x.Applicant_Id) && x.IsActive == true).ToList();
                        var applicantExperiences = db.AdhocApplicantExperiences.Where(x => applicantIds.Contains(x.Applicant_Id) && x.IsActive == true && !x.JobTitle.ToLower().Contains("house") && !x.Organization.ToLower().Contains("house") && !x.JobTitle.ToLower().Contains("grad") && !x.Organization.ToLower().Contains("grad") && !x.JobTitle.ToLower().Contains("pgr") && !x.Organization.ToLower().Contains("pgr") && !x.JobTitle.ToLower().Contains("houce") && !x.Organization.ToLower().Contains("houce") && !x.JobTitle.ToLower().Contains("train") && !x.Organization.ToLower().Contains("train")).ToList();

                        return Ok(new { marks, applicantQualifications, applicantExperiences });
                    }
                    return Ok("");
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
        [AllowAnonymous]
        [HttpGet]
        [Route("GetAdhocApplicantMarks/{ApplicantId}")]
        public IHttpActionResult GetAdhocApplicantMarks(int ApplicantId)
        {

            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var applicantQualifications = db.AdhocApplicantQualificationViews.Where(x => x.Applicant_Id == ApplicantId && x.IsActive == true).ToList();
                    var applicant = db.AdhocApplicants.Where(x => x.Id == ApplicantId && x.IsActive == true).FirstOrDefault();
                    int obtainMarks = getObtainedMarks(db, applicant, applicantQualifications);

                    return Ok(obtainMarks);
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                return BadRequest(ex.InnerException.ToString());
            }
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("GetAdhocApplicant/{ApplicantId}")]
        public IHttpActionResult GetAdhocApplicant(int ApplicantId)
        {

            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var applicant = db.AdhocApplicantViews.Where(x => x.Id == ApplicantId && x.IsActive == true).FirstOrDefault();
                    return Ok(applicant);
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                return BadRequest(ex.InnerException.ToString());
            }
        }

        [HttpGet]
        [Route("RemoveApplicantQualification/{Id}")]
        public IHttpActionResult RemoveApplicantQualification(int Id)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var ApplicantQualification = _db.AdhocApplicantQualifications.FirstOrDefault(x => x.Id == Id);

                    //if (ApplicantQualification != null)
                    //{
                    //    ApplicantQualification.IsActive = false;
                    //    _db.SaveChanges();
                    //}
                    return Ok(false);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("RemoveApplicantPreference/{id}")]
        public IHttpActionResult RemoveApplicantPreference(int id)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var applicationPreference = _db.AdhocApplicationPreferences.FirstOrDefault(x => x.Id == id);

                    if (applicationPreference != null)
                    {
                        applicationPreference.IsActive = false;
                        _db.SaveChanges();
                    }
                    int lastOrder = (int)applicationPreference.PreferenceOrder;
                    var belowPreferences = _db.AdhocApplicationPreferences.Where(x => x.PreferenceOrder > applicationPreference.PreferenceOrder && x.IsActive == true && x.JobApplicant_Id == applicationPreference.JobApplicant_Id && x.JobApplication_Id == applicationPreference.JobApplication_Id).ToList();
                    foreach (var pref in belowPreferences)
                    {
                        pref.PreferenceOrder = lastOrder++;
                        _db.SaveChanges();
                    }
                    return Ok(true);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpGet]
        [Route("ChangePreferenceOrder/{id}/{order}")]
        public IHttpActionResult ChangePreferenceOrder(int id, int order)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var applicationPreference = _db.AdhocApplicationPreferences.FirstOrDefault(x => x.Id == id);

                    if (applicationPreference != null)
                    {
                        var applicationPreference2 = _db.AdhocApplicationPreferences.FirstOrDefault(x => x.JobApplication_Id == applicationPreference.JobApplication_Id && x.PreferenceOrder == order && x.IsActive == true);
                        if (applicationPreference2 != null)
                        {
                            applicationPreference2.PreferenceOrder = applicationPreference.PreferenceOrder;
                            _db.Entry(applicationPreference2).State = EntityState.Modified;
                            _db.SaveChanges();
                            applicationPreference.PreferenceOrder = order;
                            _db.Entry(applicationPreference).State = EntityState.Modified;
                            _db.SaveChanges();
                        }
                    }

                    return Ok(true);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpGet]
        [Route("GetAdhocCounts")]
        public IHttpActionResult GetAdhocCounts()
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var res = _db.uspDesignationWiseApplicationsSummary().OrderBy(x => x.OrderBy).ToList();

                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpGet]
        [Route("GetAdhocDashboardCounts")]
        public IHttpActionResult GetAdhocDashboardCounts()
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var res = _db.uspDesignationWiseApplicationsSummary().OrderBy(x => x.OrderBy).ToList();
                    var grv = _db.uspAdhocApplicationGrievanceCountSummary().ToList();

                    return Ok(new { res, grv });
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpGet]
        [Route("GetAdhocApplicantCounts/{hfmisCode}")]
        public IHttpActionResult GetAdhocCounts(string hfmisCode)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var preferences = db.AdhocApplicationPreferenceViews
                       .Where(x => x.HFMISCode.StartsWith(hfmisCode) && x.IsActive == true)
                       .GroupBy(x => new { x.JobApplication_Id }).Select(x => new AdhocApplicantIds
                       {
                           Id = x.Key.JobApplication_Id ?? 0

                       }).ToList();
                    var pIds = preferences.Select(x => x.Id).ToList();
                    var applications = db.AdhocApplicationViews
                        .Where(x => pIds.Contains(x.Id) && x.IsActive == true)
                        .GroupBy(x => new { x.Applicant_Id }).Select(x => new AdhocApplicantIds
                        {
                            Id = (int)x.Key.Applicant_Id
                        }).ToList();
                    var aIds = applications.Select(x => x.Id).ToList();
                    var applicants = db.AdhocApplicantViews.Where(x => aIds.Contains(x.Id) && x.IsActive == true).Count();
                    var applicantsPending = db.AdhocApplicantViews.Where(x => aIds.Contains(x.Id) && (x.Status_Id == 1 || x.Status_Id == 4) && x.IsActive == true).Count();
                    var applicantsEligible = db.AdhocApplicantViews.Where(x => aIds.Contains(x.Id) && x.Status_Id == 2 && x.IsActive == true).Count();
                    var applicantsRejected = db.AdhocApplicantViews.Where(x => aIds.Contains(x.Id) && x.Status_Id == 3 && x.IsActive == true).Count();
                    return Ok(new { applicantsPending, applicants, applicantsEligible, applicantsRejected });
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpPost]
        [Route("GetAdhocVerificationData")]
        public IHttpActionResult GetAdhocVerificationData([FromBody] AdhocApplicaitionFilters filter)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    var query = db.AdhocApplicantViews.Where(x => x.IsActive == true);
                    if (filter.Status_Id == 1)
                    {
                        var Hafiz_e_QuranApplications = db.AdhocApplicationViews.Where(x => x.Status_Id == 2 && x.IsActive == true).Select(x => x.Applicant_Id).ToList();
                        query = db.AdhocApplicantViews.Where(x => x.Hafiz == true && Hafiz_e_QuranApplications.Contains(x.Id) && x.IsActive == true).AsQueryable();
                    }
                    if (filter.Status_Id == 2)
                    {
                        var PositionHolderApplications = db.AdhocInterviewBatchApplicationVs.Where(x => x.PositionHolder == true && x.IsActive == true).Select(x => x.Applicant_Id).ToList();
                        query = db.AdhocApplicantViews.Where(x => PositionHolderApplications.Contains(x.Id) && x.IsActive == true).AsQueryable();
                    }
                    if (filter.Status_Id == 3)
                    {
                        var approvedApplicants = db.AdhocApplicationViews.Where(x => x.Status_Id == 2 && (x.Designation_Id == 802 || x.Designation_Id == 1320) && x.IsActive == true).GroupBy(x => x.Applicant_Id).Select(x => x.Key.Value).ToList();
                        var higherQualifications = new List<int>() { 127, 123, 124, 126, 128, 130 };
                        var higherQualifiedApplicantIds = db.AdhocApplicantQualificationViews.Where(x => approvedApplicants.Contains((int)x.Applicant_Id) && higherQualifications.Contains((int)x.Required_Degree_Id) && x.IsActive == true).GroupBy(x => x.Applicant_Id).Select(x => x.Key).ToList();
                        //var scrutinyQualifiedApplicantsIds = db.AdhocScrutinyViews.Where(x => higherQualifiedApplicantIds.Contains((int)x.Applicant_Id) && x.IsAccepted == true && x.IsActive == true).GroupBy(x => x.Applicant_Id).Select(x => x.Key).ToList();
                        query = db.AdhocApplicantViews.Where(x => higherQualifiedApplicantIds.Contains(x.Id) && x.IsActive == true).AsQueryable();
                    }
                    if (filter.Status_Id == 5)
                    {
                        List<int> consultants = new List<int>()
                {
                    362,365,368,369,373,302,374,375,381,382,383,384,385,387,390,1594,1598,2136,2313
                };
                        var approvedApplicants = db.AdhocApplicationViews.Where(x => consultants.Contains((int)x.Designation_Id) && x.Status_Id == 2 && x.IsActive == true).GroupBy(x => x.Applicant_Id).Select(x => x.Key.Value).ToList();
                        var higherQualifications = new List<int>() { 127, 123, 124, 126, 128, 130 };
                        higherQualifications.Add(137);
                        higherQualifications.Add(130);
                        higherQualifications.Add(138);
                        higherQualifications.Add(139);
                        higherQualifications.Add(140);
                        higherQualifications.Add(147);
                        higherQualifications.Add(141);
                        higherQualifications.Add(142);
                        higherQualifications.Add(143);
                        higherQualifications.Add(120);
                        higherQualifications.Add(144);
                        higherQualifications.Add(145);
                        higherQualifications.Add(146);
                        higherQualifications.Add(148);
                        var higherQualifiedApplicantIds = db.AdhocApplicantQualificationViews.Where(x => approvedApplicants.Contains((int)x.Applicant_Id) && higherQualifications.Contains((int)x.Required_Degree_Id) && x.IsActive == true).GroupBy(x => x.Applicant_Id).Select(x => x.Key).ToList();
                        //var scrutinyQualifiedApplicantsIds = db.AdhocScrutinyViews.Where(x => higherQualifiedApplicantIds.Contains((int)x.Applicant_Id) && x.IsAccepted == true && x.IsActive == true).GroupBy(x => x.Applicant_Id).Select(x => x.Key).ToList();
                        query = db.AdhocApplicantViews.Where(x => higherQualifiedApplicantIds.Contains(x.Id) && x.IsActive == true).AsQueryable();
                    }
                    if (filter.Status_Id == 4)
                    {
                        var approvedApplicants = db.AdhocApplicationViews.Where(x => x.Status_Id == 2 && x.IsActive == true).Select(x => x.Applicant_Id).ToList();
                        var scrutinyExpApplicantsIds = db.AdhocScrutinies.Where(x => x.Experience_Id != null && x.IsRejected == true && x.IsActive == true).GroupBy(x => x.Experience_Id).Select(x => x.Key).ToList();
                        var experiencedApplicantsIds = db.AdhocApplicantExperiences.Where(x => !x.JobTitle.ToLower().Contains("house") && !x.Organization.ToLower().Contains("house") && !x.JobTitle.ToLower().Contains("grad") && !x.Organization.ToLower().Contains("grad") && !x.JobTitle.ToLower().Contains("houce") && !x.Organization.ToLower().Contains("houce") && !x.JobTitle.ToLower().Contains("pgr") && !x.Organization.ToLower().Contains("pgr") && !x.JobTitle.ToLower().Contains("train") && !x.Organization.ToLower().Contains("train") && scrutinyExpApplicantsIds.Contains((int)x.Id) && approvedApplicants.Contains((int)x.Applicant_Id) && x.IsActive == true).GroupBy(x => x.Applicant_Id).Select(x => x.Key).ToList();
                        query = db.AdhocApplicantViews.Where(x => experiencedApplicantsIds.Contains(x.Id) && x.IsActive == true).AsQueryable();
                    }
                    var applicants = query.OrderBy(l => l.Id).Skip(filter.Skip).Take(filter.PageSize).ToList();
                    var totalRecords = query.Count();
                    return Ok(new TableResponse<AdhocApplicantView>() { List = applicants, Count = totalRecords });
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpGet]
        [Route("GetAdhocDashboardDistrict/{hfmisCode}")]
        public IHttpActionResult GetAdhocDashboardDistrict(string hfmisCode)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var res = db.uspAdhocApplicationsDistDesiSummary(hfmisCode).OrderBy(x => x.OrderBy).ToList();
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpGet]
        [Route("GetAdhocPendencySummary")]
        public IHttpActionResult GetAdhocPendencySummary()
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var res = db.uspAdhocPendencyDistrictWiseSummary().ToList();
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [Route("GetAdhocVacants/{type}/{desigId}/{hfmisCode}")]
        [HttpGet]
        public IHttpActionResult GetAdhocVacants(string type, int desigId, string hfmisCode)
        {
            try
            {
                using (var db = new HR_System())
                {

                    db.Configuration.ProxyCreationEnabled = false;

                    if (type == "designations" && desigId == 0)
                    {
                        List<int?> desigsAllowed = new List<int?>();
                        desigsAllowed = db.AdhocJobs.Select(k => k.Designation_Id).ToList();
                        var designations = db.VpMeritPreferenceViews.Where(x => x.HFAC == 1 && desigsAllowed.Contains(x.Desg_Id) && x.Vacant > 0 && x.HFMISCode.StartsWith(hfmisCode)).GroupBy(x => new { x.Desg_Id, x.DsgName }).Select(x => new AdhocDesignationVacant
                        {
                            Id = x.Key.Desg_Id,
                            Name = x.Key.DsgName,
                            Sanctioned = x.Sum(k => k.TotalSanctioned),
                            Filled = x.Sum(k => k.TotalWorking),
                            Vacant = x.Sum(k => k.Vacant),
                            PHFMC = x.Sum(k => k.PHFMC),
                            Count = x.Count()
                        }).OrderBy(x => x.Name).ToList();
                        return Ok(new { designations });
                    }
                    if (type == "facilities" && desigId > 0)
                    {
                        var query = db.VpMeritPreferenceViews.Where(x => x.HFAC == 1 && x.Vacant > 0).AsQueryable();
                        if (desigId == 802)
                        {
                            query = query.Where(x => x.Desg_Id == 802 || x.Desg_Id == 2404 || x.Desg_Id == 1085).AsQueryable();
                        }
                        else if (desigId == 1320)
                        {
                            query = query.Where(x => x.Desg_Id == 1320 || x.Desg_Id == 2404 || x.Desg_Id == 1157).AsQueryable();
                        }
                        else if (desigId == 431)
                        {
                            query = query.Where(x => x.Desg_Id == 431 || x.Desg_Id == 1076).AsQueryable();
                        }
                        else
                        {
                            query = query.Where(x => x.Desg_Id == desigId).AsQueryable();
                        }
                        var districts = query.GroupBy(x => new { x.DistrictName, x.DistrictCode }).Select(x => new AdhocDistrictVacant
                        {
                            Code = x.Key.DistrictCode,
                            Name = x.Key.DistrictName,
                            Sanctioned = x.Sum(k => k.TotalSanctioned),
                            Filled = x.Sum(k => k.TotalWorking),
                            Vacant = x.Sum(k => k.Vacant),
                            PHFMC = x.Sum(k => k.PHFMC),
                            Count = x.Count()
                        }).OrderBy(x => x.Name).ToList();
                        var hfs = query.GroupBy(x => new { x.HF_Id, x.Desg_Id, x.DsgName, x.HFMISCode, x.HFName }).Select(x => new AdhocHFVacant
                        {
                            Id = x.Key.HF_Id,
                            HFMISCode = x.Key.HFMISCode,
                            Name = x.Key.HFName,
                            DsgName = x.Key.DsgName,
                            DesgId = x.Key.Desg_Id,
                            Sanctioned = x.Sum(k => k.TotalSanctioned),
                            Filled = x.Sum(k => k.TotalWorking),
                            Vacant = x.Sum(k => k.Vacant),
                            PHFMC = x.Sum(k => k.PHFMC),
                            Count = x.Count()
                        }).OrderBy(x => x.Name).ToList();
                        return Ok(new { hfs, districts });
                    }
                    //var hfs = await db.HFListPs.Where(x => x.HFMISCode.StartsWith(hfmisCode) && x.IsActive == true).OrderBy(x => x.FullName).ToListAsync();

                    return Ok(false);

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


        [Route("GetAdhocVacantDesignations/{type}/{desigId}/{hfmisCode}")]
        [HttpGet]
        public IHttpActionResult GetAdhocVacantDesignations(string type, int desigId, string hfmisCode)
        {
            try
            {
                using (var db = new HR_System())
                {

                    db.Configuration.ProxyCreationEnabled = false;

                    if (type == "designations" && desigId == 0)
                    {
                        List<int?> desigsAllowed = new List<int?>();
                        desigsAllowed = db.AdhocJobs.Select(k => k.Designation_Id).ToList();
                        var designations = db.VpMeritPreferenceViews.Where(x => x.HFAC == 1 && desigsAllowed.Contains(x.Desg_Id) && x.Vacant > 0 && x.HFMISCode.StartsWith(hfmisCode)).GroupBy(x => new { x.Desg_Id, x.DsgName }).Select(x => new AdhocDesignationVacant
                        {
                            Id = x.Key.Desg_Id,
                            Name = x.Key.DsgName,
                            Sanctioned = x.Sum(k => k.TotalSanctioned),
                            Filled = x.Sum(k => k.TotalWorking),
                            Vacant = x.Sum(k => k.Vacant),
                            PHFMC = x.Sum(k => k.PHFMC),
                            Count = x.Count()
                        }).OrderBy(x => x.Name).ToList();
                        return Ok(new { designations });
                    }
                    if (type == "facilities" && desigId > 0)
                    {
                        var query = db.VpMeritPreferenceViews.Where(x => x.HFAC == 1 && x.Vacant > 0).AsQueryable();
                        if (desigId == 802)
                        {
                            query = query.Where(x => x.Desg_Id == 802 || x.Desg_Id == 2404 || x.Desg_Id == 1085).AsQueryable();
                        }
                        else if (desigId == 1320)
                        {
                            query = query.Where(x => x.Desg_Id == 1320 || x.Desg_Id == 2404 || x.Desg_Id == 1157).AsQueryable();
                        }
                        else
                        {
                            query = query.Where(x => x.Desg_Id == desigId).AsQueryable();
                        }
                        var districts = query.GroupBy(x => new { x.DistrictName, x.DistrictCode, x.Desg_Id, x.DsgName }).Select(x => new AdhocDistrictVacant
                        {
                            Code = x.Key.DistrictCode,
                            Name = x.Key.DistrictName,
                            DesignationName = x.Key.DsgName,
                            DesignationId = x.Key.Desg_Id,
                            Sanctioned = x.Sum(k => k.TotalSanctioned),
                            Filled = x.Sum(k => k.TotalWorking),
                            Vacant = x.Sum(k => k.Vacant),
                            PHFMC = x.Sum(k => k.PHFMC),
                            Count = x.Count()
                        }).OrderBy(x => x.Name).ToList();
                        var hfs = query.GroupBy(x => new { x.HF_Id, x.Desg_Id, x.DsgName, x.HFMISCode, x.HFName }).Select(x => new AdhocHFVacant
                        {
                            Id = x.Key.HF_Id,
                            HFMISCode = x.Key.HFMISCode,
                            Name = x.Key.HFName,
                            DsgName = x.Key.DsgName,
                            DesgId = x.Key.Desg_Id,
                            Sanctioned = x.Sum(k => k.TotalSanctioned),
                            Filled = x.Sum(k => k.TotalWorking),
                            Vacant = x.Sum(k => k.Vacant),
                            PHFMC = x.Sum(k => k.PHFMC),
                            Count = x.Count()
                        }).OrderBy(x => x.Name).ToList();
                        return Ok(new { hfs, districts });
                    }
                    //var hfs = await db.HFListPs.Where(x => x.HFMISCode.StartsWith(hfmisCode) && x.IsActive == true).OrderBy(x => x.FullName).ToListAsync();

                    return Ok(false);

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
        [Route("UploadApplicantPhoto/{cnic}")]
        public async Task<IHttpActionResult> UploadApplicantPhoto(string cnic)
        {
            try
            {
                using (var db = new HR_System())
                {
                    if (!Request.Content.IsMimeMultipartContent())
                        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                    var applicant = db.AdhocApplicants.FirstOrDefault(x => x.CNIC == cnic && x.IsActive == true);
                    if (applicant == null)
                    {
                        return BadRequest("No Applicant");
                    }
                    string RootPath = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\AdhocApplicants\Photo";
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
                        filename = cnic + "_23.jpg";

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
                        applicant.ProfilePic = @"Uploads\AdhocApplicants\Photo\" + filename;
                        db.Entry(applicant).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return Ok(new { result = true, src = filename });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("UploadApplicantPMDC/{cnic}")]
        public async Task<IHttpActionResult> UploadApplicantPMDC(string cnic)
        {
            try
            {
                using (var db = new HR_System())
                {
                    var applicant = db.AdhocApplicants.FirstOrDefault(x => x.CNIC == cnic && x.IsActive == true);
                    if (applicant == null)
                    {
                        return BadRequest("No Applicant");
                    }
                    if (!Request.Content.IsMimeMultipartContent())
                        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                    string RootPath = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\AdhocApplicants\PMDC";
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
                        filename = Guid.NewGuid().ToString() + "_" + cnic + "." + FileExtension;
                        var buffer = await file.ReadAsByteArrayAsync();
                        var size = ((buffer.Length) / (1024)) / (1024);
                        var ext = Path.GetExtension(filename.Replace("\"", string.Empty));
                        List<string> validExtensions = new List<string>() { ".jpg", ".jpeg", ".png", ".gif", ".pdf" };
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
                        applicant.PMDCDoc = @"Uploads\AdhocApplicants\PMDC\" + filename;
                        db.Entry(applicant).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return Ok(new { result = true, src = filename });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("ReUploadApplicantPMDC/{cnic}")]
        public async Task<IHttpActionResult> ReUploadApplicantPMDC(string cnic)
        {
            try
            {
                using (var db = new HR_System())
                {
                    var applicant = db.AdhocApplicants.FirstOrDefault(x => x.CNIC == cnic && x.IsActive == true);
                    if (applicant == null)
                    {
                        return BadRequest("No Applicant");
                    }
                    if (!Request.Content.IsMimeMultipartContent())
                        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                    string RootPath = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\AdhocApplicants\PMDC";
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
                        filename = Guid.NewGuid().ToString() + "_" + cnic + "." + FileExtension;
                        var buffer = await file.ReadAsByteArrayAsync();
                        var size = ((buffer.Length) / (1024)) / (1024);
                        var ext = Path.GetExtension(filename.Replace("\"", string.Empty));
                        List<string> validExtensions = new List<string>() { ".jpg", ".jpeg", ".png", ".gif", ".pdf" };
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
                        applicant.PMDCDocOld = applicant.PMDCDoc;
                        applicant.PMDCDoc = @"Uploads\AdhocApplicants\PMDC\" + filename;
                        db.Entry(applicant).State = EntityState.Modified;
                        db.SaveChanges();

                        AdhocGreivanceUpload adhocGreivanceUpload = db.AdhocGreivanceUploads.FirstOrDefault(x => x.Applicant_Id == applicant.Id && x.DocId == 3);
                        if (adhocGreivanceUpload != null)
                        {
                            adhocGreivanceUpload.IsActive = false;
                            db.Entry(adhocGreivanceUpload).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                        adhocGreivanceUpload = new AdhocGreivanceUpload();
                        adhocGreivanceUpload.Applicant_Id = applicant.Id;
                        adhocGreivanceUpload.DocId = 3;
                        adhocGreivanceUpload.DocName = "PMC";
                        adhocGreivanceUpload.IsQualification = false;
                        adhocGreivanceUpload.IsActive = true;
                        adhocGreivanceUpload.CreatedDate = DateTime.UtcNow.AddHours(5);
                        adhocGreivanceUpload.CreatedBy = User.Identity.GetUserName();
                        adhocGreivanceUpload.UserId = User.Identity.GetUserId();
                        adhocGreivanceUpload.UploadPath = applicant.PMDCDoc;
                        db.AdhocGreivanceUploads.Add(adhocGreivanceUpload);
                        db.SaveChanges();
                    }
                    return Ok(new { result = true, src = filename });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("UploadApplicantDomicile/{cnic}")]
        public async Task<IHttpActionResult> UploadApplicantDomicile(string cnic)
        {
            try
            {
                using (var db = new HR_System())
                {
                    var applicant = db.AdhocApplicants.FirstOrDefault(x => x.CNIC == cnic && x.IsActive == true);
                    if (applicant == null)
                    {
                        return BadRequest("No Applicant");
                    }
                    if (!Request.Content.IsMimeMultipartContent())
                        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                    string RootPath = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\AdhocApplicants\Domicile";
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
                        filename = Guid.NewGuid().ToString() + "_" + cnic + "." + FileExtension;
                        var buffer = await file.ReadAsByteArrayAsync();
                        var size = ((buffer.Length) / (1024)) / (1024);
                        var ext = Path.GetExtension(filename.Replace("\"", string.Empty));
                        List<string> validExtensions = new List<string>() { ".jpg", ".jpeg", ".png", ".gif", ".pdf" };
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
                        applicant.DomicileDoc = @"Uploads\AdhocApplicants\Domicile\" + filename;
                        db.Entry(applicant).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return Ok(new { result = true, src = filename });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("ReUploadApplicantDomicile/{cnic}")]
        public async Task<IHttpActionResult> ReUploadApplicantDomicile(string cnic)
        {
            try
            {
                using (var db = new HR_System())
                {
                    var applicant = db.AdhocApplicants.FirstOrDefault(x => x.CNIC == cnic && x.IsActive == true);
                    if (applicant == null)
                    {
                        return BadRequest("No Applicant");
                    }
                    if (!Request.Content.IsMimeMultipartContent())
                        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                    string RootPath = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\AdhocApplicants\Domicile";
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
                        filename = Guid.NewGuid().ToString() + "_" + cnic + "." + FileExtension;
                        var buffer = await file.ReadAsByteArrayAsync();
                        var size = ((buffer.Length) / (1024)) / (1024);
                        var ext = Path.GetExtension(filename.Replace("\"", string.Empty));
                        List<string> validExtensions = new List<string>() { ".jpg", ".jpeg", ".png", ".gif", ".pdf" };
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
                        applicant.DomicileDocOld = applicant.DomicileDoc;
                        applicant.DomicileDoc = @"Uploads\AdhocApplicants\Domicile\" + filename;
                        db.Entry(applicant).State = EntityState.Modified;
                        db.SaveChanges();

                        AdhocGreivanceUpload adhocGreivanceUpload = db.AdhocGreivanceUploads.FirstOrDefault(x => x.Applicant_Id == applicant.Id && x.DocId == 1);
                        if (adhocGreivanceUpload != null)
                        {
                            adhocGreivanceUpload.IsActive = false;
                            db.Entry(adhocGreivanceUpload).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                        adhocGreivanceUpload = new AdhocGreivanceUpload();
                        adhocGreivanceUpload.Applicant_Id = applicant.Id;
                        adhocGreivanceUpload.DocId = 1;
                        adhocGreivanceUpload.DocName = "Domicile";
                        adhocGreivanceUpload.IsQualification = false;
                        adhocGreivanceUpload.IsActive = true;
                        adhocGreivanceUpload.CreatedDate = DateTime.UtcNow.AddHours(5);
                        adhocGreivanceUpload.CreatedBy = User.Identity.GetUserName();
                        adhocGreivanceUpload.UserId = User.Identity.GetUserId();
                        adhocGreivanceUpload.UploadPath = applicant.DomicileDoc;
                        db.AdhocGreivanceUploads.Add(adhocGreivanceUpload);
                        db.SaveChanges();
                    }
                    return Ok(new { result = true, src = filename });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("UploadCommitteeNotification/{id}")]
        public async Task<IHttpActionResult> UploadCommitteeNotification(int id)
        {
            try
            {
                using (var db = new HR_System())
                {
                    var scrutinyCommittee = db.AdhocScrutinyCommittees.FirstOrDefault(x => x.Id == id && x.IsActive == true);
                    if (scrutinyCommittee == null)
                    {
                        return BadRequest("No Committee");
                    }
                    if (!Request.Content.IsMimeMultipartContent())
                        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                    string RootPath = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\AdhocScrutiny\Notification";
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
                        filename = Guid.NewGuid().ToString() + "_" + scrutinyCommittee.Id.ToString() + "." + FileExtension;
                        var buffer = await file.ReadAsByteArrayAsync();
                        var size = ((buffer.Length) / (1024)) / (1024);
                        var ext = Path.GetExtension(filename.Replace("\"", string.Empty));
                        List<string> validExtensions = new List<string>() { ".jpg", ".jpeg", ".png", ".gif", ".pdf" };
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
                        scrutinyCommittee.NotificationPath = @"Uploads\AdhocScrutiny\Notification" + filename;
                        scrutinyCommittee.NotificationCreatedBy = User.Identity.GetUserName();
                        scrutinyCommittee.NotificationCreateDate = DateTime.UtcNow.AddHours(5);
                        scrutinyCommittee.NotificationUserId = User.Identity.GetUserId();
                        db.Entry(scrutinyCommittee).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return Ok(new { result = true, src = filename });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("UploadApplicantCNIC/{cnic}")]
        public async Task<IHttpActionResult> UploadApplicantCNIC(string cnic)
        {
            try
            {
                using (var db = new HR_System())
                {
                    var applicant = db.AdhocApplicants.FirstOrDefault(x => x.CNIC == cnic && x.IsActive == true);
                    if (applicant == null)
                    {
                        return BadRequest("No Applicant");
                    }
                    if (!Request.Content.IsMimeMultipartContent())
                        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                    string RootPath = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\AdhocApplicants\CNIC";
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
                        filename = Guid.NewGuid().ToString() + "_" + cnic + "." + FileExtension;
                        var buffer = await file.ReadAsByteArrayAsync();
                        var size = ((buffer.Length) / (1024)) / (1024);
                        var ext = Path.GetExtension(filename.Replace("\"", string.Empty));
                        List<string> validExtensions = new List<string>() { ".jpg", ".jpeg", ".png", ".gif", ".pdf" };
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
                        applicant.CNICDoc = @"Uploads\AdhocApplicants\CNIC\" + filename;
                        db.Entry(applicant).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return Ok(new { result = true, src = filename });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("UploadApplicantHifz/{cnic}")]
        public async Task<IHttpActionResult> UploadApplicantHifz(string cnic)
        {
            try
            {
                using (var db = new HR_System())
                {
                    var applicant = db.AdhocApplicants.FirstOrDefault(x => x.CNIC == cnic && x.IsActive == true);
                    if (applicant == null)
                    {
                        return BadRequest("No Applicant");
                    }
                    if (!Request.Content.IsMimeMultipartContent())
                        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                    string RootPath = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\AdhocApplicants\HifzCertificate";
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
                        filename = Guid.NewGuid().ToString() + "_" + cnic + "." + FileExtension;
                        var buffer = await file.ReadAsByteArrayAsync();
                        var size = ((buffer.Length) / (1024)) / (1024);
                        var ext = Path.GetExtension(filename.Replace("\"", string.Empty));
                        List<string> validExtensions = new List<string>() { ".jpg", ".jpeg", ".png", ".gif", ".pdf" };
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
                        applicant.Hafiz = true;
                        applicant.HifzDocument = @"Uploads\AdhocApplicants\HifzCertificate\" + filename;
                        db.Entry(applicant).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return Ok(new { result = true, src = filename });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("UploadPositionDoc/{cnic}")]
        public async Task<IHttpActionResult> UploadPositionDoc(string cnic)
        {
            try
            {
                using (var db = new HR_System())
                {
                    var applicant = db.AdhocApplicants.FirstOrDefault(x => x.CNIC == cnic && x.IsActive == true);
                    if (applicant == null)
                    {
                        return BadRequest("No Applicant");
                    }
                    if (!Request.Content.IsMimeMultipartContent())
                        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                    string RootPath = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\AdhocApplicants\PositionDoc";
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
                        filename = Guid.NewGuid().ToString() + "_" + cnic + "." + FileExtension;
                        var buffer = await file.ReadAsByteArrayAsync();
                        var size = ((buffer.Length) / (1024)) / (1024);
                        var ext = Path.GetExtension(filename.Replace("\"", string.Empty));
                        List<string> validExtensions = new List<string>() { ".jpg", ".jpeg", ".png", ".gif", ".pdf" };
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
                        applicant.PositionDoc = @"Uploads\AdhocApplicants\PositionDoc\" + filename;
                        db.Entry(applicant).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return Ok(new { result = true, src = filename });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("ReUploadApplicantHifz/{cnic}")]
        public async Task<IHttpActionResult> ReUploadApplicantHifz(string cnic)
        {
            try
            {
                using (var db = new HR_System())
                {
                    var applicant = db.AdhocApplicants.FirstOrDefault(x => x.CNIC == cnic && x.IsActive == true);
                    if (applicant == null)
                    {
                        return BadRequest("No Applicant");
                    }
                    if (!Request.Content.IsMimeMultipartContent())
                        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                    string RootPath = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\AdhocApplicants\HifzCertificate";
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
                        filename = Guid.NewGuid().ToString() + "_" + cnic + "." + FileExtension;
                        var buffer = await file.ReadAsByteArrayAsync();
                        var size = ((buffer.Length) / (1024)) / (1024);
                        var ext = Path.GetExtension(filename.Replace("\"", string.Empty));
                        List<string> validExtensions = new List<string>() { ".jpg", ".jpeg", ".png", ".gif", ".pdf" };
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
                        applicant.Hafiz = true;
                        applicant.HifzDocumentOld = applicant.HifzDocument;
                        applicant.HifzDocument = @"Uploads\AdhocApplicants\HifzCertificate\" + filename;
                        db.Entry(applicant).State = EntityState.Modified;
                        db.SaveChanges();


                        AdhocGreivanceUpload adhocGreivanceUpload = db.AdhocGreivanceUploads.FirstOrDefault(x => x.Applicant_Id == applicant.Id && x.DocId == 2);
                        if (adhocGreivanceUpload != null)
                        {
                            adhocGreivanceUpload.IsActive = false;
                            db.Entry(adhocGreivanceUpload).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                        adhocGreivanceUpload = new AdhocGreivanceUpload();
                        adhocGreivanceUpload.Applicant_Id = applicant.Id;
                        adhocGreivanceUpload.DocId = 2;
                        adhocGreivanceUpload.DocName = "Hafiz-e-Quran";
                        adhocGreivanceUpload.IsQualification = false;
                        adhocGreivanceUpload.IsActive = true;
                        adhocGreivanceUpload.CreatedDate = DateTime.UtcNow.AddHours(5);
                        adhocGreivanceUpload.CreatedBy = User.Identity.GetUserName();
                        adhocGreivanceUpload.UserId = User.Identity.GetUserId();
                        adhocGreivanceUpload.UploadPath = applicant.HifzDocument;
                        db.AdhocGreivanceUploads.Add(adhocGreivanceUpload);
                        db.SaveChanges();


                    }
                    return Ok(new { result = true, src = filename });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("UploadApplicantPositionDoc/{applicantId}")]
        public async Task<IHttpActionResult> UploadApplicantPositionDoc(int applicantId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    var applicant = db.AdhocApplicants.FirstOrDefault(x => x.Id == applicantId && x.IsActive == true);
                    if (applicant == null)
                    {
                        return BadRequest("No Applicant");
                    }
                    if (!Request.Content.IsMimeMultipartContent())
                        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                    string RootPath = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\AdhocApplicants\PositionDoc";
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
                        filename = Guid.NewGuid().ToString() + "_" + applicantId.ToString() + "." + FileExtension;
                        var buffer = await file.ReadAsByteArrayAsync();
                        var size = ((buffer.Length) / (1024)) / (1024);
                        var ext = Path.GetExtension(filename.Replace("\"", string.Empty));
                        List<string> validExtensions = new List<string>() { ".jpg", ".jpeg", ".png", ".gif", ".pdf" };
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
                        applicant.PositionDoc = @"Uploads\AdhocApplicants\PositionDoc\" + filename;
                        db.Entry(applicant).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return Ok(new { result = true, src = filename });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("UploadApplicantQualification/{id}")]

        public async Task<IHttpActionResult> UploadApplicantQualification(int id)
        {
            try
            {
                using (var db = new HR_System())
                {
                    var applicantQualification = db.AdhocApplicantQualifications.FirstOrDefault(x => x.Id == id);
                    if (applicantQualification == null)
                    {
                        return BadRequest("No Applicant Qualification");
                    }
                    if (!Request.Content.IsMimeMultipartContent())
                        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                    string RootPath = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\AdhocApplicants\Qualification";
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
                        filename = Guid.NewGuid().ToString() + "_" + id + "." + FileExtension;
                        var buffer = await file.ReadAsByteArrayAsync();
                        var size = ((buffer.Length) / (1024)) / (1024);
                        var ext = Path.GetExtension(filename.Replace("\"", string.Empty));
                        List<string> validExtensions = new List<string>() { ".jpg", ".jpeg", ".png", ".gif", ".pdf" };
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
                        applicantQualification.UploadPath = @"Uploads\AdhocApplicants\Qualification\" + filename;
                        //applicantQualification.UploadedInGrievance = true;
                        //applicantQualification.GrievanceDatetime = DateTime.UtcNow.AddHours(5);
                        //var scrutiny = db.AdhocScrutinies.FirstOrDefault(x => x.Qualification_Id == applicantQualification.Old_Id);
                        //if (scrutiny != null)
                        //{
                        //    applicantQualification.ScrutinyId = scrutiny.Id;
                        //}
                        db.Entry(applicantQualification).State = EntityState.Modified;
                        db.SaveChanges();
                        //var applicant = db.AdhocApplicants.FirstOrDefault(x => x.Id == applicantQualification.Applicant_Id);
                        //if (applicant != null)
                        //{
                        //    AdhocGreivanceUpload adhocGreivanceUpload = db.AdhocGreivanceUploads.FirstOrDefault(x => x.Applicant_Id == applicant.Id && x.QualificationId == applicantQualification.Old_Id);
                        //    if (adhocGreivanceUpload != null)
                        //    {
                        //        adhocGreivanceUpload.IsActive = false;
                        //        db.Entry(adhocGreivanceUpload).State = EntityState.Modified;
                        //        db.SaveChanges();
                        //    }

                        //    adhocGreivanceUpload = new AdhocGreivanceUpload();
                        //    adhocGreivanceUpload.Applicant_Id = applicantQualification.Applicant_Id;
                        //    adhocGreivanceUpload.IsQualification = true;
                        //    adhocGreivanceUpload.QualificationId = applicantQualification.Id;
                        //    adhocGreivanceUpload.IsActive = true;
                        //    adhocGreivanceUpload.CreatedDate = DateTime.UtcNow.AddHours(5);
                        //    adhocGreivanceUpload.CreatedBy = User.Identity.GetUserName();
                        //    adhocGreivanceUpload.UserId = User.Identity.GetUserId();
                        //    adhocGreivanceUpload.UploadPath = applicantQualification.UploadPath;
                        //    db.AdhocGreivanceUploads.Add(adhocGreivanceUpload);
                        //    db.SaveChanges();
                        //}



                    }
                    return Ok(new { result = true, src = filename });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Route("UploadScrutinyMinutes/{id}")]
        public async Task<IHttpActionResult> UploadScrutinyMinutes(int id)
        {
            try
            {
                using (var db = new HR_System())
                {
                    var adhocScrutinyMinute = db.AdhocScrutinyMinutes.FirstOrDefault(x => x.Id == id);
                    if (adhocScrutinyMinute == null)
                    {
                        return BadRequest("No Scrutiny Minutes");
                    }
                    if (!Request.Content.IsMimeMultipartContent())
                        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                    string RootPath = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\AdhocScrutiny\Minutes";
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
                        filename = Guid.NewGuid().ToString() + "_" + id + "." + FileExtension;
                        var buffer = await file.ReadAsByteArrayAsync();
                        var size = ((buffer.Length) / (1024)) / (1024);
                        var ext = Path.GetExtension(filename.Replace("\"", string.Empty));
                        List<string> validExtensions = new List<string>() { ".jpg", ".jpeg", ".png", ".gif", ".pdf" };
                        if (!validExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase) || size > 100)
                        {
                            throw new Exception(
                                "Unable to Upload. File Size must be less than 100 MB and File Format must be " +
                                string.Join(",", validExtensions));
                        }
                        using (FileStream fsOut = File.OpenWrite(RootPath + @"\" + filename))
                        {
                            fsOut.Write(buffer, 0, buffer.Length);
                        }
                        adhocScrutinyMinute.SignedCopyPath = @"Uploads\AdhocScrutiny\Minutes\" + filename;
                        adhocScrutinyMinute.SignedUploaded = true;
                        db.Entry(adhocScrutinyMinute).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return Ok(new { result = true, src = filename });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("UploadGrievanceScrutinyMinutes/{id}")]
        public async Task<IHttpActionResult> UploadGrievanceScrutinyMinutes(int id)
        {
            try
            {
                using (var db = new HR_System())
                {
                    var adhocScrutinyMinute = db.AdhocScrutinyMinutes.FirstOrDefault(x => x.Id == id);
                    if (adhocScrutinyMinute == null)
                    {
                        return BadRequest("No Scrutiny Minutes");
                    }
                    if (!Request.Content.IsMimeMultipartContent())
                        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                    string RootPath = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\AdhocScrutiny\GrievanceMinutes";
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
                        filename = Guid.NewGuid().ToString() + "_" + id + "." + FileExtension;
                        var buffer = await file.ReadAsByteArrayAsync();
                        var size = ((buffer.Length) / (1024)) / (1024);
                        var ext = Path.GetExtension(filename.Replace("\"", string.Empty));
                        List<string> validExtensions = new List<string>() { ".jpg", ".jpeg", ".png", ".gif", ".pdf" };
                        if (!validExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase) || size > 100)
                        {
                            throw new Exception(
                                "Unable to Upload. File Size must be less than 100 MB and File Format must be " +
                                string.Join(",", validExtensions));
                        }
                        using (FileStream fsOut = File.OpenWrite(RootPath + @"\" + filename))
                        {
                            fsOut.Write(buffer, 0, buffer.Length);
                        }
                        adhocScrutinyMinute.GrievanceMinutesCopyPath = @"Uploads\AdhocScrutiny\GrievanceMinutes\" + filename;
                        adhocScrutinyMinute.GrievanceMinutesUploaded = true;
                        adhocScrutinyMinute.GrievanceMinutes = DateTime.UtcNow.AddHours(5);
                        db.Entry(adhocScrutinyMinute).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return Ok(new { result = true, src = filename });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("UploadMeritList/{id}")]
        public async Task<IHttpActionResult> UploadMeritList(int id)
        {
            try
            {
                using (var db = new HR_System())
                {
                    var adhocInterview = db.AdhocInterviews.FirstOrDefault(x => x.Id == id);
                    if (adhocInterview == null)
                    {
                        return BadRequest("No Interview Found!");
                    }
                    if (!Request.Content.IsMimeMultipartContent())
                        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                    string RootPath = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\AdhocInterview\MList";
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
                        filename = Guid.NewGuid().ToString() + "_" + id + "." + FileExtension;
                        var buffer = await file.ReadAsByteArrayAsync();
                        var size = ((buffer.Length) / (1024)) / (1024);
                        var ext = Path.GetExtension(filename.Replace("\"", string.Empty));
                        List<string> validExtensions = new List<string>() { ".jpg", ".jpeg", ".png", ".gif", ".pdf" };
                        if (!validExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase) || size > 100)
                        {
                            throw new Exception(
                                "Unable to Upload. File Size must be less than 100 MB and File Format must be " +
                                string.Join(",", validExtensions));
                        }
                        using (FileStream fsOut = File.OpenWrite(RootPath + @"\" + filename))
                        {
                            fsOut.Write(buffer, 0, buffer.Length);
                        }
                        adhocInterview.MeritListPath = @"Uploads\AdhocInterview\MList" + filename;
                        adhocInterview.MertiListUploaded = true;
                        db.Entry(adhocInterview).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return Ok(new { result = true, src = filename });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("UploadJobApplicantAttachments/{cnic}")]
        public async Task<bool> UploadJobApplicantAttachments(string cnic)
        {
            try
            {
                using (var _db = new HR_System())
                {

                    var applicant = _db.AdhocApplicants.FirstOrDefault(x => x.CNIC.Equals(cnic));
                    if (applicant == null)
                    {
                        return false;
                    }
                    if (!Request.Content.IsMimeMultipartContent())
                        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                    string RootPath = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\AdhocApplicants\ApplicantDocuments\";
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
                        filename = Guid.NewGuid().ToString() + "_" + applicant.Id + "." + FileExtension;
                        AdhocApplicantDocument jobApplicantDocument = new AdhocApplicantDocument();
                        List<string> mkeys = key.Split('_').ToList();
                        if (mkeys.Count >= 3)
                        {
                            int lastIndex = mkeys.Count - 1;
                            string JobDocument_Id = mkeys[lastIndex--].Trim('\"');
                            //string Grade = mkeys[lastIndex--].Trim('\"');
                            //string Division = mkeys[lastIndex--].Trim('\"');
                            //string PassingYear = mkeys[lastIndex--].Trim('\"');
                            //string Degree = mkeys[lastIndex--].Trim('\"');
                            string ObtainedMarks = mkeys[lastIndex--].Trim('\"');
                            string TotalMarks = mkeys[lastIndex--].Trim('\"');
                            jobApplicantDocument.TotalMarks = TotalMarks != null ? Convert.ToDouble(TotalMarks) : 0;
                            jobApplicantDocument.ObtainedMarks = ObtainedMarks != null ? Convert.ToDouble(ObtainedMarks) : 0;
                            //jobApplicantDocument.Degree = Degree;
                            //if (!string.IsNullOrEmpty(PassingYear))
                            //{
                            //    jobApplicantDocument.PassingYear = Convert.ToDateTime("01/01/" + PassingYear);
                            //}
                            //jobApplicantDocument.Division = Division != null && !Division.Equals("undefined") ? Convert.ToInt32(Division) : 0;
                            //jobApplicantDocument.Grade = Grade;
                            jobApplicantDocument.JobDocument_Id = JobDocument_Id != null && !JobDocument_Id.Equals("undefined") ? Convert.ToInt32(JobDocument_Id) : 0;
                            jobApplicantDocument.UploadPath = @"Uploads\AdhocApplicants\ApplicantDocuments\" + filename;
                            jobApplicantDocument.IsActive = true;
                            jobApplicantDocument.Applicant_Id = applicant.Id;
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
                            _db.AdhocApplicantDocuments.Add(jobApplicantDocument);
                            _db.SaveChanges();
                            applicant.Status_Id = 3;
                            _db.Entry(applicant).State = EntityState.Modified;
                            _db.SaveChanges();
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

        [Route("UploadExperienceCertificate/{experienceId}")]
        public async Task<bool> UploaLeaveAttachement(int experienceId)
        {
            try
            {
                using (var _db = new HR_System())
                {

                    if (!Request.Content.IsMimeMultipartContent())
                        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                    string RootPath = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\Adhoc\ExperienceCertificate\";
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
                        filename = Guid.NewGuid().ToString() + "-" + experienceId + "." + FileExtension;


                        var experience = _db.AdhocApplicantExperiences.FirstOrDefault(x => x.Id == experienceId);
                        if (experience != null)
                        {
                            experience.UploadPath = @"Uploads/Adhoc/ExperienceCertificate/" + filename;
                            _db.Entry(experience).State = EntityState.Modified;
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

        private void CreateDirectoryIfNotExists(string dirPath) { if (!Directory.Exists(dirPath)) { Directory.CreateDirectory(dirPath); } }

        private int getObtainedMarks(HR_System db, AdhocApplicant interviewCandidate, List<AdhocApplicantQualificationView> quals)
        {
            int obtainedMarks = 0;
            var matric = quals.FirstOrDefault(x => x.DegreeName.Equals("Matriculation"));
            var inter = quals.FirstOrDefault(x => x.DegreeName.Equals("FSC (Pre- Medical)"));
            var mbbs1 = quals.FirstOrDefault(x => x.DegreeName.Equals("First Professional MBBS-I"));
            var mbbs2 = quals.FirstOrDefault(x => x.DegreeName.Equals("First Professional MBBS-II"));
            var mbbs3 = quals.FirstOrDefault(x => x.DegreeName.Equals("Second Professional MBBS"));
            var mbbs4 = quals.FirstOrDefault(x => x.DegreeName.Equals("Third Professional MBBS"));
            var mbbs5 = quals.FirstOrDefault(x => x.DegreeName.Equals("Final Professional MBBS"));
            var PhD = quals.FirstOrDefault(x => x.DegreeName.Equals("PhD"));
            var mPhil = quals.FirstOrDefault(x => x.DegreeName.Equals("MPhill"));
            var supraSpeciality = quals.FirstOrDefault(x => x.DegreeName.Equals("Supra Specialty"));

            if (matric != null)
            {
                var pct = (matric.ObtainedMarks / matric.TotalMarks) * 100;
                if (pct >= 60)
                {
                    obtainedMarks += 6;
                    interviewCandidate.MatricMarks = 6;
                }
                else if (pct >= 45 && pct < 60)
                {
                    obtainedMarks += 5;
                    interviewCandidate.MatricMarks = 5;
                }
                else if (pct < 45)
                {
                    obtainedMarks += 3;
                    interviewCandidate.MatricMarks = 3;
                }
            }
            if (inter != null)
            {
                var pct = (inter.ObtainedMarks / inter.TotalMarks) * 100;
                if (pct >= 60)
                {
                    obtainedMarks += 12;
                    interviewCandidate.InterMarks = 12;
                }
                else if (pct >= 45 && pct < 60)
                {
                    obtainedMarks += 11;
                    interviewCandidate.InterMarks = 11;
                }
                else if (pct < 45)
                {
                    obtainedMarks += 7;
                    interviewCandidate.InterMarks = 7;
                }
            }

            if (mbbs1 != null && mbbs2 != null && mbbs3 != null && mbbs4 != null && mbbs5 != null)
            {
                var totalMarks = mbbs1.TotalMarks + mbbs2.TotalMarks + mbbs3.TotalMarks + mbbs4.TotalMarks + mbbs5.TotalMarks;
                var obtainMarks = mbbs1.ObtainedMarks + mbbs2.ObtainedMarks + mbbs3.ObtainedMarks + mbbs4.ObtainedMarks + mbbs5.ObtainedMarks;
                var pct = (obtainMarks / totalMarks) * 100;
                if (pct >= 60)
                {
                    obtainedMarks += 17;
                    interviewCandidate.MasterMarks = 17;
                }
                else if (pct >= 45 && pct < 60)
                {
                    obtainedMarks += 16;
                    interviewCandidate.MasterMarks = 16;
                }
                else if (pct < 45)
                {
                    obtainedMarks += 11;
                    interviewCandidate.MasterMarks = 11;
                }
                //if (mast != null)
                //{
                //    var pct2 = (mast.ObtainedMarks / mast.TotalMarks) * 100;
                //    if (pct2 >= 60)
                //    {
                //        obtainedMarks += 35;
                //        interviewCandidate.MastMarks = 35;
                //    }
                //    else if (pct2 >= 45 && pct2 < 60)
                //    {
                //        obtainedMarks += 32;
                //        interviewCandidate.MastMarks = 32;
                //    }
                //    else if (pct2 < 45)
                //    {
                //        obtainedMarks += 21;
                //        interviewCandidate.MastMarks = 21;
                //    }
                //}
            }
            //else if (grad4 != null)
            //{
            //    var pct = (double)(grad4.ObtainedMarks);
            //    if (pct >= 2.4)
            //    {
            //        obtainedMarks += 35 + 17;
            //        interviewCandidate.GradMarks = 17;
            //        interviewCandidate.MastMarks = 35;
            //    }
            //    else if (pct >= 2.0 && pct < 2.4)
            //    {
            //        obtainedMarks += 32 + 16;
            //        interviewCandidate.GradMarks = 16;
            //        interviewCandidate.MastMarks = 32;
            //    }
            //    else if (pct < 2.0)
            //    {
            //        obtainedMarks += 21 + 11;
            //        interviewCandidate.GradMarks = 11;
            //        interviewCandidate.MastMarks = 21;
            //    }
            //}

            if (PhD != null)
            {
                obtainedMarks += 7;
                interviewCandidate.SecondHigherMarks = 7;
            }
            else if (mPhil != null)
            {
                obtainedMarks += 5;
                interviewCandidate.FirstHigherMarks = 5;
            }
            //interviewCandidate.Total = obtainedMarks;

            db.SaveChanges();
            //}
            return obtainedMarks;
        }


        private int consultantMarks(HR_System db, AdhocApplication application, AdhocInterviewBatchApplicationV batchApp, List<AdhocApplicantQualification> qualifications)
        {
            try
            {


                int tMarks = 0;
                //totalMarks = db.AdhocApplicationMarks.FirstOrDefault(x => x.Application_Id == application.Id && x.Marks_Id == (int)MarksEnum.Total && x.IsActive == true);
                var applicant = db.AdhocApplicants.FirstOrDefault(x => x.Id == application.Applicant_Id && x.Hafiz == true && x.HifzVerified != false && !string.IsNullOrEmpty(x.HifzDocument) && x.IsActive == true);
                if (applicant != null)
                {
                    var hifzMarks = db.AdhocApplicationMarks.FirstOrDefault(x => x.BatchApplicationId == batchApp.Id && x.Marks_Id == (int)MarksEnum.HafizeQuran && x.IsActive == true);
                    if (hifzMarks == null)
                    {
                        hifzMarks = new AdhocApplicationMark();
                        hifzMarks.Application_Id = application.Id;
                        hifzMarks.BatchApplicationId = batchApp.Id;
                        hifzMarks.Marks_Id = (int)MarksEnum.HafizeQuran;
                        hifzMarks.Marks = 5;
                        tMarks += (int)hifzMarks.Marks;
                        hifzMarks.Percentage = 100;
                        hifzMarks.IsActive = true;
                        hifzMarks.CreatedBy = User.Identity.GetUserName();
                        hifzMarks.CreatedDate = DateTime.UtcNow.AddHours(5);
                        hifzMarks.UserId = User.Identity.GetUserId();
                        hifzMarks.Remarks = "Hafiz-e-Quran Added @ " + hifzMarks.CreatedDate.ToString();
                        db.AdhocApplicationMarks.Add(hifzMarks);
                        db.SaveChanges();
                    }
                }
                applicant = db.AdhocApplicants.FirstOrDefault(x => x.Id == batchApp.Applicant_Id && x.IsActive == true);
                if (applicant.Position != null && applicant.Position > 0 && applicant.PositionVerified == true)
                {
                    var marksPos = db.AdhocApplicationMarks.FirstOrDefault(x => x.Application_Id == application.Id && x.Marks_Id == 10 && x.IsActive == true);
                    marksPos = new AdhocApplicationMark();
                    marksPos.Application_Id = application.Id;
                    marksPos.BatchApplicationId = batchApp.Id;
                    if (applicant.Position == 1)
                    {
                        marksPos.Marks_Id = 12;
                        marksPos.Marks = 5;
                    }
                    else if (applicant.Position == 2)
                    {
                        marksPos.Marks_Id = 13;
                        marksPos.Marks = 3;
                    }
                    else if (applicant.Position == 3)
                    {
                        marksPos.Marks_Id = 14;
                        marksPos.Marks = 2;
                    }

                    tMarks += (int)marksPos.Marks;
                    marksPos.Percentage = marksPos.Marks;
                    marksPos.Remarks = "Position";

                    marksPos.IsActive = true;
                    marksPos.CreatedBy = User.Identity.GetUserName();
                    marksPos.CreatedDate = DateTime.UtcNow.AddHours(5);
                    marksPos.UserId = User.Identity.GetUserId();
                    db.AdhocApplicationMarks.Add(marksPos);
                    db.SaveChanges();
                }
                bool fcps = false, mcps = false, msmd = false, mph = false;
                var higherQualifications = new List<int>();
                higherQualifications.Add(137);
                higherQualifications.Add(130);
                higherQualifications.Add(138);
                higherQualifications.Add(139);
                higherQualifications.Add(140);
                higherQualifications.Add(147);
                higherQualifications.Add(141);
                higherQualifications.Add(142);
                higherQualifications.Add(143);
                higherQualifications.Add(120);
                higherQualifications.Add(144);
                higherQualifications.Add(145);
                higherQualifications.Add(146);
                higherQualifications.Add(148);
                foreach (var qualification in qualifications)
                {
                    var marks = db.AdhocApplicationMarks.FirstOrDefault(x => x.BatchApplicationId == batchApp.Id && x.ApplicantQualification_Id == qualification.Id && x.IsActive == true);
                    if (marks == null)
                    {
                        marks = new AdhocApplicationMark();
                        marks.Application_Id = application.Id;
                        marks.BatchApplicationId = batchApp.Id;
                        marks.ApplicantQualification_Id = qualification.Id;
                        //var scrutinyDecision = db.AdhocScrutinies.FirstOrDefault(x => x.Qualification_Id == qualification.Id && x.IsAccepted == true && x.IsActive == true);
                        //if (scrutinyDecision == null)
                        //{
                        //    scrutinyDecision = db.AdhocScrutinies.FirstOrDefault(x => x.Qualification_Id == qualification.Id && x.GrievanceAccepted == true && x.IsActive == true);
                        //    if (scrutinyDecision == null)
                        //    {
                        //        marks.Remarks = "No Scrutiny Found!";
                        //        marks.Error = true;
                        //        marks.IsActive = true;
                        //        marks.CreatedBy = User.Identity.GetUserName();
                        //        marks.CreatedDate = DateTime.UtcNow.AddHours(5);
                        //        marks.UserId = User.Identity.GetUserId();
                        //        db.AdhocApplicationMarks.Add(marks);
                        //        db.SaveChanges();
                        //    }
                        //}
                        //if (scrutinyDecision != null)
                        //{
                        var pct = 0.00;
                        if (qualification.ObtainedMarks != null && qualification.ObtainedMarks > 0)
                        {
                            if (qualification.TotalMarks != null && qualification.TotalMarks > 0)
                            {
                                if (qualification.TotalMarks == 4)
                                {
                                    var cgpa = (double)qualification.ObtainedMarks;
                                    if (cgpa >= 3.63)
                                    {
                                        pct = (cgpa - 0.3) / 0.037;
                                    }
                                    else if (cgpa >= 3.25 && cgpa < 3.63)
                                    {
                                        pct = (cgpa - 0.29) / 0.037;
                                    }
                                    else if (cgpa >= 2.88 && cgpa < 3.25)
                                    {
                                        pct = (cgpa - 0.36) / 0.036;
                                    }
                                    else if (cgpa >= 2.5 && cgpa < 2.88)
                                    {
                                        pct = (cgpa - 0.28) / 0.037;
                                    }
                                    else if (cgpa >= 1.8 && cgpa < 2.5)
                                    {
                                        pct = (cgpa + 1.65) / 0.069;
                                    }
                                    else if (cgpa >= 1 && cgpa < 1.8)
                                    {
                                        pct = (cgpa + 2.16) / 0.079;
                                    }
                                    else if (cgpa > 0 && cgpa < 1)
                                    {
                                        pct = (cgpa / 0.0248);
                                    }
                                }
                                else if (qualification.TotalMarks == 5)
                                {
                                    var cgpa = (double)qualification.ObtainedMarks;
                                    if (cgpa >= 4.63)
                                    {
                                        pct = (cgpa - 1.3) / 0.037;
                                    }
                                    else if (cgpa >= 4.25 && cgpa < 4.63)
                                    {
                                        pct = (cgpa - 1.29) / 0.037;
                                    }
                                    else if (cgpa >= 3.88 && cgpa < 4.25)
                                    {
                                        pct = (cgpa - 1.36) / 0.036;
                                    }
                                    else if (cgpa >= 3.5 && cgpa < 3.88)
                                    {
                                        pct = (cgpa - 1.28) / 0.037;
                                    }
                                    else if (cgpa >= 2.8 && cgpa < 3.5)
                                    {
                                        pct = (cgpa + 0.65) / 0.069;
                                    }
                                    else if (cgpa >= 2 && cgpa < 2.8)
                                    {
                                        pct = (cgpa + 1.16) / 0.079;
                                    }
                                    else if (cgpa > 0 && cgpa < 2)
                                    {
                                        pct = (cgpa - 1 / 0.0248);
                                    }
                                }
                                else if (qualification.TotalMarks > 5)
                                {
                                    pct = (double)(qualification.ObtainedMarks / qualification.TotalMarks) * 100;
                                    pct = Math.Round(pct, 2);
                                }
                                if (qualification.Required_Degree_Id == (int)DgIdsEnum.Matriculation || qualification.Required_Degree_Id == (int)DgIdsEnum.OLevel)
                                {
                                    marks.Marks_Id = (int)MarksEnum.Matriculation;
                                    if (pct >= 60) { marks.Marks = 12; tMarks += (int)marks.Marks; }
                                    else if (pct >= 45 && pct < 60) { marks.Marks = 11; tMarks += (int)marks.Marks; }
                                    else if (pct < 45) { marks.Marks = 7; tMarks += (int)marks.Marks; }
                                    marks.Percentage = (double)pct;
                                }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.OLevel)
                                {
                                    marks.Marks_Id = (int)MarksEnum.OLevel;

                                }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.FSCPreMedical || qualification.Required_Degree_Id == (int)DgIdsEnum.ALevel)
                                {
                                    marks.Marks_Id = (int)MarksEnum.Intermedicate;
                                    if (pct >= 60) { marks.Marks = 23; tMarks += (int)marks.Marks; }
                                    else if (pct >= 45 && pct < 60) { marks.Marks = 21; tMarks += (int)marks.Marks; }
                                    else if (pct < 45) { marks.Marks = 14; tMarks += (int)marks.Marks; }
                                    marks.Percentage = (double)pct;
                                }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.ALevel)
                                {
                                    marks.Marks_Id = (int)MarksEnum.ALevel;
                                }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.BSNursing) { }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.FSCPreMedical) { }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.FirstProfessionalMBBSI)
                                {
                                }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.FirstProfessionalMBBSII) { }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.SecondProfessionalMBBS) { }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.ThirdProfessionalMBBS) { }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.FinalProfessionalMBBS)
                                {
                                    var mbbs1 = qualifications.FirstOrDefault(x => x.Required_Degree_Id == (int)DgIdsEnum.FirstProfessionalMBBSI);
                                    var mbbs2 = qualifications.FirstOrDefault(x => x.Required_Degree_Id == (int)DgIdsEnum.FirstProfessionalMBBSII);
                                    var mbbs3 = qualifications.FirstOrDefault(x => x.Required_Degree_Id == (int)DgIdsEnum.SecondProfessionalMBBS);
                                    var mbbs4 = qualifications.FirstOrDefault(x => x.Required_Degree_Id == (int)DgIdsEnum.ThirdProfessionalMBBS);
                                    var mbbs5 = qualifications.FirstOrDefault(x => x.Required_Degree_Id == (int)DgIdsEnum.FinalProfessionalMBBS);
                                    if (mbbs1 != null && mbbs2 != null && mbbs3 != null && mbbs4 != null && mbbs5 != null)
                                    {

                                        var total = mbbs1.TotalMarks + mbbs2.TotalMarks + mbbs3.TotalMarks + mbbs4.TotalMarks + mbbs5.TotalMarks;
                                        var obtainMarks = mbbs1.ObtainedMarks + mbbs2.ObtainedMarks + mbbs3.ObtainedMarks + mbbs4.ObtainedMarks + mbbs5.ObtainedMarks;
                                        pct = (double)(obtainMarks / total) * 100;
                                        marks.Marks_Id = (int)MarksEnum.Graduation;
                                        if (pct >= 60) { marks.Marks = 35; tMarks += (int)marks.Marks; }
                                        else if (pct >= 45 && pct < 60) { marks.Marks = 31; tMarks += (int)marks.Marks; }
                                        else if (pct < 45) { marks.Marks = 21; tMarks += (int)marks.Marks; }
                                        marks.Percentage = (double)pct;
                                        marks.Remarks = "MBBS";
                                        //if (mbbs2.TotalMarks != 600 && (mbbs5.DegreeType == "Pakistan" || string.IsNullOrEmpty(mbbs5.DegreeType)))
                                        //{
                                        //    marks.Remarks = "First Professional MBBS II total marks are not 600";
                                        //}
                                    }
                                    else if ((mbbs1 == null || mbbs2 == null || mbbs3 == null || mbbs4 == null) && mbbs5 != null)
                                    {
                                        marks.Marks_Id = (int)MarksEnum.Graduation;
                                        if (pct >= 60) { marks.Marks = 35; tMarks += (int)marks.Marks; }
                                        else if (pct >= 45 && pct < 60) { marks.Marks = 31; tMarks += (int)marks.Marks; }
                                        else if (pct < 45) { marks.Marks = 21; tMarks += (int)marks.Marks; }
                                        marks.Percentage = (double)pct;
                                    }
                                }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.FCPSPartII || higherQualifications.Contains(qualification.Required_Degree_Id) && (qualification.IsVerified == null || qualification.IsVerified == true))
                                {
                                    var fcps1 = qualifications.FirstOrDefault(x => x.Id == (int)DgIdsEnum.FCPSPartI);
                                    if (fcps1 != null)
                                    {
                                        fcps = true;
                                    }
                                    fcps = true;
                                }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.MCPSPartI && (qualification.IsVerified == null || qualification.IsVerified == true))
                                {
                                    if (qualification.ExpFrom != null && qualification.ExpTo != null)
                                    {
                                        var datediff = qualification.ExpFrom - qualification.ExpTo;
                                        if (datediff.Value.Days > (360 + 360))
                                        {
                                            mcps = true;
                                        }
                                    }
                                }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.MCPSPartII && (qualification.IsVerified == null || qualification.IsVerified == true))
                                {
                                    var mcps1 = qualifications.FirstOrDefault(x => x.Id == (int)DgIdsEnum.MCPSPartII);
                                    if (mcps1 != null)
                                    {
                                        mcps = true;
                                    }
                                    mcps = true;
                                }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.MSMDPartII && (qualification.IsVerified == null || qualification.IsVerified == true))
                                {
                                    var msmd1 = qualifications.FirstOrDefault(x => x.Id == (int)DgIdsEnum.MSMDPartI);
                                    if (msmd1 != null)
                                    {
                                        msmd = true;
                                    }
                                    msmd = true;
                                }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.MastersinPublicHealth && (qualification.IsVerified == null || qualification.IsVerified == true))
                                {

                                    mph = true;
                                }

                            }
                            else
                            {
                                marks.Remarks = "No Value in Total Marks";
                                marks.Error = true;
                            }
                        }
                        else
                        {
                            if ((qualification.Required_Degree_Id == (int)DgIdsEnum.FCPSPartII || higherQualifications.Contains(qualification.Required_Degree_Id)) && (qualification.IsVerified == null || qualification.IsVerified == true))
                            {
                                var fcps1 = qualifications.FirstOrDefault(x => x.Id == (int)DgIdsEnum.FCPSPartI);
                                if (fcps1 != null)
                                {
                                    fcps = true;
                                }
                                fcps = true;
                            }
                            else if (qualification.Required_Degree_Id == (int)DgIdsEnum.MCPSPartI && (qualification.IsVerified == null || qualification.IsVerified == true))
                            {
                                if (qualification.ExpFrom != null && qualification.ExpTo != null)
                                {
                                    var datediff = qualification.ExpFrom - qualification.ExpTo;
                                    if (datediff.Value.Days > (360 + 360))
                                    {
                                        mcps = true;
                                    }
                                }
                            }
                            else if (qualification.Required_Degree_Id == (int)DgIdsEnum.MCPSPartII && (qualification.IsVerified == null || qualification.IsVerified == true))
                            {
                                var mcps1 = qualifications.FirstOrDefault(x => x.Id == (int)DgIdsEnum.MCPSPartII);
                                if (mcps1 != null)
                                {
                                    mcps = true;
                                }
                                mcps = true;
                            }
                            else if (qualification.Required_Degree_Id == (int)DgIdsEnum.MSMDPartII && (qualification.IsVerified == null || qualification.IsVerified == true))
                            {
                                var msmd1 = qualifications.FirstOrDefault(x => x.Id == (int)DgIdsEnum.MSMDPartI);
                                if (msmd1 != null)
                                {
                                    msmd = true;
                                }
                                msmd = true;
                            }
                            else if (qualification.Required_Degree_Id == (int)DgIdsEnum.MastersinPublicHealth && (qualification.IsVerified == null || qualification.IsVerified == true))
                            {

                                mph = true;
                            }
                            else
                            {
                                marks.Remarks = "No Value in Obtained Marks";
                                marks.Error = true;
                            }
                        }
                        if (marks.Marks_Id > 0 || marks.Error == true)
                        {
                            marks.IsActive = true;
                            marks.CreatedBy = User.Identity.GetUserName();
                            marks.CreatedDate = DateTime.UtcNow.AddHours(5);
                            marks.UserId = User.Identity.GetUserId();
                            if (string.IsNullOrEmpty(marks.Remarks)) { marks.Remarks = "Marks Added @ " + marks.CreatedDate.ToString(); }
                            db.AdhocApplicationMarks.Add(marks);
                            //db.Entry(marks).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        //}
                    }
                }

                var marksHigher = new AdhocApplicationMark();
                marksHigher.Application_Id = application.Id;
                marksHigher.BatchApplicationId = batchApp.Id;
                if (fcps == true && mcps == true && mph == true && msmd == false)
                {
                    marksHigher.Marks_Id = (int)MarksEnum.SecondStageHigher;
                    marksHigher.Marks = 7; tMarks += (int)marksHigher.Marks;
                    marksHigher.Remarks = "fcps == true && mcps == true && mph == true && msmd == false";
                }
                if (fcps == true && msmd == true && mph == true && mcps == false)
                {
                    marksHigher.Marks_Id = (int)MarksEnum.SecondStageHigher;
                    marksHigher.Marks = 7; tMarks += (int)marksHigher.Marks;
                    marksHigher.Remarks = "fcps == true && msmd == true && mph == true && mcps == false";
                }
                if (mcps == true && msmd == true && mph == true && fcps == false)
                {
                    marksHigher.Marks_Id = (int)MarksEnum.SecondStageHigher;
                    marksHigher.Marks = 7; tMarks += (int)marksHigher.Marks;
                    marksHigher.Remarks = "mcps == true && msmd == true && mph == true && fcps == false";
                }
                if (mcps == true && msmd == true && fcps == true && mph == false)
                {
                    marksHigher.Marks_Id = (int)MarksEnum.SecondStageHigher;
                    marksHigher.Marks = 7; tMarks += (int)marksHigher.Marks;
                    marksHigher.Remarks = "mcps == true && msmd == true && fcps == true && mph == false";
                }

                //As
                if (fcps == true && mcps == true && mph == false && msmd == false)
                {
                    marksHigher.Marks_Id = (int)MarksEnum.SecondStageHigher;
                    marksHigher.Marks = 7; tMarks += (int)marksHigher.Marks;
                    marksHigher.Remarks = "fcps == true && mcps == true && mph == false && msmd == false";
                }
                if (fcps == true && mph == true && msmd == false && mcps == false)
                {
                    marksHigher.Marks_Id = (int)MarksEnum.SecondStageHigher;
                    marksHigher.Marks = 7; tMarks += (int)marksHigher.Marks;
                    marksHigher.Remarks = "fcps == true && mph == true && msmd == false && mcps == false";
                }
                if (fcps == true && msmd == true && mph == false && mcps == false)
                {
                    marksHigher.Marks_Id = (int)MarksEnum.SecondStageHigher;
                    marksHigher.Marks = 7; tMarks += (int)marksHigher.Marks;
                    marksHigher.Remarks = "fcps == true && msmd == true && mph == false && mcps == false";
                }
                //As
                if (fcps == false && mph == true && msmd == false && mcps == true)
                {
                    marksHigher.Marks_Id = (int)MarksEnum.FirstStageHigher;
                    marksHigher.Marks = 5; tMarks += (int)marksHigher.Marks;
                    marksHigher.Remarks = "fcps == false && mph == true && msmd == false && mcps == true";
                }
                if (fcps == false && msmd == true && mph == false && mcps == true)
                {
                    marksHigher.Marks_Id = (int)MarksEnum.SecondStageHigher;
                    marksHigher.Marks = 7; tMarks += (int)marksHigher.Marks;
                    marksHigher.Remarks = "fcps == false && msmd == true && mph == false && mcps == true";
                }
                //As
                if (fcps == false && mcps == false && mph == true && msmd == true)
                {
                    marksHigher.Marks_Id = (int)MarksEnum.SecondStageHigher;
                    marksHigher.Marks = 7; tMarks += (int)marksHigher.Marks;
                    marksHigher.Remarks = "fcps == false && mcps == false && mph == true && msmd == true";
                }
                if (fcps == true && mph == false && msmd == false && mcps == false)
                {
                    marksHigher.Marks_Id = (int)MarksEnum.FirstStageHigher;
                    marksHigher.Marks = 5; tMarks += (int)marksHigher.Marks;
                    marksHigher.Remarks = "fcps == true && mph == false && msmd == false && mcps == false";
                }

                if (fcps == false && mph == true && msmd == false && mcps == false)
                {
                    marksHigher.Marks_Id = (int)MarksEnum.FirstStageHigher;
                    marksHigher.Marks = 5; tMarks += (int)marksHigher.Marks;
                    marksHigher.Remarks = "fcps == false && mph == true && msmd == false && mcps == false";
                }

                if (fcps == false && mph == false && msmd == true && mcps == false)
                {
                    marksHigher.Marks_Id = (int)MarksEnum.FirstStageHigher;
                    marksHigher.Marks = 5; tMarks += (int)marksHigher.Marks;
                    marksHigher.Remarks = "fcps == false && mph == false && msmd == true && mcps == false";
                }

                if (fcps == false && mph == false && msmd == false && mcps == true)
                {
                    marksHigher.Marks_Id = (int)MarksEnum.FirstStageHigher;
                    marksHigher.Marks = 5; tMarks += (int)marksHigher.Marks;
                    marksHigher.Remarks = "fcps == false && mph == false && msmd == false && mcps == true";
                }
                if (marksHigher != null && marksHigher.Marks > 0)
                {
                    marksHigher.IsActive = true;
                    marksHigher.CreatedBy = User.Identity.GetUserName();
                    marksHigher.CreatedDate = DateTime.UtcNow.AddHours(5);
                    marksHigher.UserId = User.Identity.GetUserId();
                    if (string.IsNullOrEmpty(marksHigher.Remarks)) { marksHigher.Remarks = "Marks Added @ " + marksHigher.CreatedDate.ToString(); }
                    db.AdhocApplicationMarks.Add(marksHigher);
                    //db.Entry(marks).State = EntityState.Modified;
                    db.SaveChanges();
                }

                var experiences = db.AdhocApplicantExperiences.Where(x => x.Applicant_Id == batchApp.Applicant_Id && x.IsVerified != false && !x.JobTitle.ToLower().Contains("house") && !x.Organization.ToLower().Contains("house") && !x.JobTitle.ToLower().Contains("grad") && !x.Organization.ToLower().Contains("grad") && !x.JobTitle.ToLower().Contains("pgr") && !x.Organization.ToLower().Contains("pgr") && !x.JobTitle.ToLower().Contains("train") && !x.Organization.ToLower().Contains("train") && x.IsActive == true).ToList();

                int days = 0;
                double marksPerDay = 0.00273972602;
                var marksExp = db.AdhocApplicationMarks.FirstOrDefault(x => x.Application_Id == application.Id && x.BatchApplicationId == batchApp.Id && x.Marks_Id == 10 && x.IsActive == true);
                if (marksExp == null)
                {
                    foreach (var experience in experiences)
                    {
                        if (experience.IsVerified == true)
                        {
                            var scrutinyDecision = db.AdhocScrutinies.FirstOrDefault(x => x.Experience_Id == experience.Id && (x.IsAccepted == true || x.GrievanceAccepted == true) && x.IsActive == true);
                            if (scrutinyDecision != null && (scrutinyDecision.IsAccepted == true || scrutinyDecision.GrievanceAccepted == true))
                            {
                                if (experience.FromDate != null && experience.ToDate != null)
                                {
                                    if (experience.ToDate.Value > experience.FromDate.Value)
                                    {
                                        DateTime FromYear = Convert.ToDateTime(experience.FromDate.Value);
                                        DateTime ToYear = Convert.ToDateTime(experience.ToDate.Value);
                                        TimeSpan objTimeSpan = ToYear - FromYear;
                                        days += Convert.ToInt32(objTimeSpan.TotalDays);
                                    }
                                }
                                else if (experience.FromDate != null && experience.IsContinued == true)
                                {
                                    DateTime FromYear = Convert.ToDateTime(experience.FromDate.Value);
                                    DateTime ToYear = Convert.ToDateTime(new DateTime(2021, 12, 10));
                                    TimeSpan objTimeSpan = ToYear - FromYear;
                                    days += Convert.ToInt32(objTimeSpan.TotalDays);
                                }
                            }
                        }
                    }
                }
                if (days > 0)
                {
                    marksExp = new AdhocApplicationMark();
                    marksExp.Application_Id = application.Id;
                    marksExp.BatchApplicationId = batchApp.Id;
                    marksExp.Marks_Id = 10;
                    double dayMarks = days * marksPerDay;
                    if (days > 1825)
                    {
                        marksExp.Marks = 5;
                    }
                    else
                    {
                        marksExp.Marks = Convert.ToDouble(dayMarks.ToString("0.##"));
                        //marksExp.Marks = Math.Ceiling((dayMarks * 100) / 100);
                    }
                    tMarks += (int)marksExp.Marks;
                    marksExp.Percentage = marksExp.Marks;
                    marksExp.Remarks = "Experience";

                    marksExp.IsActive = true;
                    marksExp.CreatedBy = User.Identity.GetUserName();
                    marksExp.CreatedDate = DateTime.UtcNow.AddHours(5);
                    marksExp.UserId = User.Identity.GetUserId();
                    db.AdhocApplicationMarks.Add(marksExp);
                    db.SaveChanges();
                }
                var totalMarks = db.AdhocApplicationMarks.FirstOrDefault(x => x.BatchApplicationId == batchApp.Id && x.Marks_Id == (int)MarksEnum.Total && x.IsActive == true);
                if (totalMarks == null)
                {
                    totalMarks = new AdhocApplicationMark();
                    totalMarks.Application_Id = application.Id;
                    totalMarks.BatchApplicationId = batchApp.Id;
                    totalMarks.Marks_Id = (int)MarksEnum.Total;
                    totalMarks.Marks = tMarks;
                    totalMarks.Percentage = tMarks;
                    totalMarks.IsActive = true;
                    totalMarks.CreatedBy = User.Identity.GetUserName();
                    totalMarks.CreatedDate = DateTime.UtcNow.AddHours(5);
                    totalMarks.UserId = User.Identity.GetUserId();
                    totalMarks.Remarks = "Total Added @ " + totalMarks.CreatedDate.ToString();
                    db.AdhocApplicationMarks.Add(totalMarks);
                    db.SaveChanges();
                }
                return (int)totalMarks.Marks;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private async Task<int> MedicalOfficerMarks(HR_System db, AdhocApplication application, AdhocInterviewBatchApplicationV batchApp, List<AdhocApplicantQualification> qualifications)
        {
            try
            {
                int tMarks = 0;
                //totalMarks = db.AdhocApplicationMarks.FirstOrDefault(x => x.Application_Id == application.Id && x.Marks_Id == (int)MarksEnum.Total && x.IsActive == true);
                var applicant = db.AdhocApplicants.FirstOrDefault(x => x.Id == application.Applicant_Id && x.Hafiz == true && x.HifzVerified != false && !string.IsNullOrEmpty(x.HifzDocument) && x.IsActive == true);
                if (applicant != null)
                {
                    var hifzMarks = db.AdhocApplicationMarks.FirstOrDefault(x => x.BatchApplicationId == batchApp.Id && x.Marks_Id == (int)MarksEnum.HafizeQuran && x.IsActive == true);
                    if (hifzMarks == null)
                    {
                        hifzMarks = new AdhocApplicationMark();
                        hifzMarks.Application_Id = application.Id;
                        hifzMarks.BatchApplicationId = batchApp.Id;
                        hifzMarks.Marks_Id = (int)MarksEnum.HafizeQuran;
                        hifzMarks.Marks = 5;
                        tMarks += (int)hifzMarks.Marks;
                        hifzMarks.Percentage = 100;
                        hifzMarks.IsActive = true;
                        hifzMarks.CreatedBy = User.Identity.GetUserName();
                        hifzMarks.CreatedDate = DateTime.UtcNow.AddHours(5);
                        hifzMarks.UserId = User.Identity.GetUserId();
                        hifzMarks.Remarks = "Hafiz-e-Quran Added @ " + hifzMarks.CreatedDate.ToString();
                        db.AdhocApplicationMarks.Add(hifzMarks);
                        await db.SaveChangesAsync();
                    }
                }
                bool fcps = false, mcps = false, msmd = false, mph = false;
                foreach (var qualification in qualifications)
                {
                    var marks = db.AdhocApplicationMarks.FirstOrDefault(x => x.BatchApplicationId == batchApp.Id && x.ApplicantQualification_Id == qualification.Id && x.IsActive == true);
                    if (marks == null)
                    {
                        marks = new AdhocApplicationMark();
                        marks.Application_Id = application.Id;
                        marks.BatchApplicationId = batchApp.Id;
                        marks.ApplicantQualification_Id = qualification.Id;
                        //var scrutinyDecision = db.AdhocScrutinies.FirstOrDefault(x => x.Qualification_Id == qualification.Id && x.IsAccepted == true && x.IsActive == true);
                        //if (scrutinyDecision == null)
                        //{
                        //    scrutinyDecision = db.AdhocScrutinies.FirstOrDefault(x => x.Qualification_Id == qualification.Id && x.GrievanceAccepted == true && x.IsActive == true);
                        //    if (scrutinyDecision == null)
                        //    {
                        //        marks.Remarks = "No Scrutiny Found!";
                        //        marks.Error = true;
                        //        marks.IsActive = true;
                        //        marks.CreatedBy = User.Identity.GetUserName();
                        //        marks.CreatedDate = DateTime.UtcNow.AddHours(5);
                        //        marks.UserId = User.Identity.GetUserId();
                        //        db.AdhocApplicationMarks.Add(marks);
                        //        db.SaveChanges();
                        //    }
                        //}
                        //if (scrutinyDecision != null)
                        //{
                        var pct = 0.00;
                        if (qualification.ObtainedMarks != null && qualification.ObtainedMarks > 0)
                        {
                            if (qualification.TotalMarks != null && qualification.TotalMarks > 0)
                            {
                                if (qualification.TotalMarks == 4)
                                {
                                    var cgpa = (double)qualification.ObtainedMarks;
                                    if (cgpa >= 3.63)
                                    {
                                        pct = (cgpa - 0.3) / 0.037;
                                    }
                                    else if (cgpa >= 3.25 && cgpa < 3.63)
                                    {
                                        pct = (cgpa - 0.29) / 0.037;
                                    }
                                    else if (cgpa >= 2.88 && cgpa < 3.25)
                                    {
                                        pct = (cgpa - 0.36) / 0.036;
                                    }
                                    else if (cgpa >= 2.5 && cgpa < 2.88)
                                    {
                                        pct = (cgpa - 0.28) / 0.037;
                                    }
                                    else if (cgpa >= 1.8 && cgpa < 2.5)
                                    {
                                        pct = (cgpa + 1.65) / 0.069;
                                    }
                                    else if (cgpa >= 1 && cgpa < 1.8)
                                    {
                                        pct = (cgpa + 2.16) / 0.079;
                                    }
                                    else if (cgpa > 0 && cgpa < 1)
                                    {
                                        pct = (cgpa / 0.0248);
                                    }
                                }
                                else if (qualification.TotalMarks == 5)
                                {
                                    var cgpa = (double)qualification.ObtainedMarks;
                                    if (cgpa >= 4.63)
                                    {
                                        pct = (cgpa - 1.3) / 0.037;
                                    }
                                    else if (cgpa >= 4.25 && cgpa < 4.63)
                                    {
                                        pct = (cgpa - 1.29) / 0.037;
                                    }
                                    else if (cgpa >= 3.88 && cgpa < 4.25)
                                    {
                                        pct = (cgpa - 1.36) / 0.036;
                                    }
                                    else if (cgpa >= 3.5 && cgpa < 3.88)
                                    {
                                        pct = (cgpa - 1.28) / 0.037;
                                    }
                                    else if (cgpa >= 2.8 && cgpa < 3.5)
                                    {
                                        pct = (cgpa + 0.65) / 0.069;
                                    }
                                    else if (cgpa >= 2 && cgpa < 2.8)
                                    {
                                        pct = (cgpa + 1.16) / 0.079;
                                    }
                                    else if (cgpa > 0 && cgpa < 2)
                                    {
                                        pct = (cgpa - 1 / 0.0248);
                                    }
                                }
                                else if (qualification.TotalMarks > 5)
                                {
                                    pct = (double)(qualification.ObtainedMarks / qualification.TotalMarks) * 100;
                                    pct = Math.Round(pct, 2);
                                }
                                if (qualification.Required_Degree_Id == (int)DgIdsEnum.Matriculation || qualification.Required_Degree_Id == (int)DgIdsEnum.OLevel)
                                {
                                    marks.Marks_Id = (int)MarksEnum.Matriculation;
                                    if (pct >= 60) { marks.Marks = 12; tMarks += (int)marks.Marks; }
                                    else if (pct >= 45 && pct < 60) { marks.Marks = 11; tMarks += (int)marks.Marks; }
                                    else if (pct < 45) { marks.Marks = 7; tMarks += (int)marks.Marks; }
                                    marks.Percentage = (double)pct;
                                }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.OLevel)
                                {
                                    marks.Marks_Id = (int)MarksEnum.OLevel;

                                }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.FSCPreMedical || qualification.Required_Degree_Id == (int)DgIdsEnum.ALevel)
                                {
                                    marks.Marks_Id = (int)MarksEnum.Intermedicate;
                                    if (pct >= 60) { marks.Marks = 23; tMarks += (int)marks.Marks; }
                                    else if (pct >= 45 && pct < 60) { marks.Marks = 21; tMarks += (int)marks.Marks; }
                                    else if (pct < 45) { marks.Marks = 14; tMarks += (int)marks.Marks; }
                                    marks.Percentage = (double)pct;
                                }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.ALevel)
                                {
                                    marks.Marks_Id = (int)MarksEnum.ALevel;
                                }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.BSNursing) { }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.FSCPreMedical) { }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.FirstProfessionalMBBSI)
                                {
                                }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.FirstProfessionalMBBSII) { }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.SecondProfessionalMBBS) { }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.ThirdProfessionalMBBS) { }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.FinalProfessionalMBBS)
                                {
                                    var mbbs1 = qualifications.FirstOrDefault(x => x.Required_Degree_Id == (int)DgIdsEnum.FirstProfessionalMBBSI);
                                    var mbbs2 = qualifications.FirstOrDefault(x => x.Required_Degree_Id == (int)DgIdsEnum.FirstProfessionalMBBSII);
                                    var mbbs3 = qualifications.FirstOrDefault(x => x.Required_Degree_Id == (int)DgIdsEnum.SecondProfessionalMBBS);
                                    var mbbs4 = qualifications.FirstOrDefault(x => x.Required_Degree_Id == (int)DgIdsEnum.ThirdProfessionalMBBS);
                                    var mbbs5 = qualifications.FirstOrDefault(x => x.Required_Degree_Id == (int)DgIdsEnum.FinalProfessionalMBBS);
                                    if (mbbs1 != null && mbbs2 != null && mbbs3 != null && mbbs4 != null && mbbs5 != null)
                                    {

                                        var total = mbbs1.TotalMarks + mbbs2.TotalMarks + mbbs3.TotalMarks + mbbs4.TotalMarks + mbbs5.TotalMarks;
                                        var obtainMarks = mbbs1.ObtainedMarks + mbbs2.ObtainedMarks + mbbs3.ObtainedMarks + mbbs4.ObtainedMarks + mbbs5.ObtainedMarks;
                                        pct = (double)(obtainMarks / total) * 100;
                                        marks.Marks_Id = (int)MarksEnum.Graduation;
                                        if (pct >= 60) { marks.Marks = 35; tMarks += (int)marks.Marks; }
                                        else if (pct >= 45 && pct < 60) { marks.Marks = 31; tMarks += (int)marks.Marks; }
                                        else if (pct < 45) { marks.Marks = 21; tMarks += (int)marks.Marks; }
                                        marks.Percentage = (double)pct;
                                        marks.Remarks = "MBBS";
                                        //if (mbbs2.TotalMarks != 600 && (mbbs5.DegreeType == "Pakistan" || string.IsNullOrEmpty(mbbs5.DegreeType)))
                                        //{
                                        //    marks.Remarks = "First Professional MBBS II total marks are not 600";
                                        //}
                                    }
                                    else if ((mbbs1 == null || mbbs2 == null || mbbs3 == null || mbbs4 == null) && mbbs5 != null)
                                    {
                                        marks.Marks_Id = (int)MarksEnum.Graduation;
                                        if (pct >= 60) { marks.Marks = 35; tMarks += (int)marks.Marks; }
                                        else if (pct >= 45 && pct < 60) { marks.Marks = 31; tMarks += (int)marks.Marks; }
                                        else if (pct < 45) { marks.Marks = 21; tMarks += (int)marks.Marks; }
                                        marks.Percentage = (double)pct;
                                    }
                                }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.FCPSPartII && (qualification.IsVerified == null || qualification.IsVerified == true))
                                {

                                    var fcps1 = qualifications.FirstOrDefault(x => x.Id == (int)DgIdsEnum.FCPSPartI);
                                    if (fcps1 != null)
                                    {
                                        fcps = true;
                                    }
                                    fcps = true;

                                }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.MCPSPartI && (qualification.IsVerified == null || qualification.IsVerified == true))
                                {
                                    if (qualification.ExpFrom != null && qualification.ExpTo != null)
                                    {
                                        var datediff = qualification.ExpFrom - qualification.ExpTo;
                                        if (datediff.Value.Days > (360 + 360))
                                        {
                                            mcps = true;
                                        }
                                    }
                                }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.MCPSPartII && (qualification.IsVerified == null || qualification.IsVerified == true))
                                {

                                    var mcps1 = qualifications.FirstOrDefault(x => x.Id == (int)DgIdsEnum.MCPSPartII);
                                    if (mcps1 != null)
                                    {
                                        mcps = true;
                                    }
                                    mcps = true;


                                }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.MSMDPartII && (qualification.IsVerified == null || qualification.IsVerified == true))
                                {

                                    var msmd1 = qualifications.FirstOrDefault(x => x.Id == (int)DgIdsEnum.MSMDPartI);
                                    if (msmd1 != null)
                                    {
                                        msmd = true;
                                    }
                                    msmd = true;

                                }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.MastersinPublicHealth && (qualification.IsVerified == null || qualification.IsVerified == true))
                                {

                                    mph = true;
                                }

                            }
                            else
                            {
                                if (qualification.Required_Degree_Id == (int)DgIdsEnum.FCPSPartII && (qualification.IsVerified == null || qualification.IsVerified == true))
                                {
                                    var fcps1 = qualifications.FirstOrDefault(x => x.Id == (int)DgIdsEnum.FCPSPartI);
                                    if (fcps1 != null)
                                    {
                                        fcps = true;
                                    }
                                    fcps = true;
                                }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.MCPSPartI && (qualification.IsVerified == null || qualification.IsVerified == true))
                                {
                                    if (qualification.ExpFrom != null && qualification.ExpTo != null)
                                    {
                                        var datediff = qualification.ExpFrom - qualification.ExpTo;
                                        if (datediff.Value.Days > (360 + 360))
                                        {
                                            mcps = true;
                                        }
                                    }
                                }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.MCPSPartII && (qualification.IsVerified == null || qualification.IsVerified == true))
                                {

                                    var mcps1 = qualifications.FirstOrDefault(x => x.Id == (int)DgIdsEnum.MCPSPartII);
                                    if (mcps1 != null)
                                    {
                                        mcps = true;
                                    }
                                    mcps = true;

                                }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.MSMDPartII && (qualification.IsVerified == null || qualification.IsVerified == true))
                                {

                                    var msmd1 = qualifications.FirstOrDefault(x => x.Id == (int)DgIdsEnum.MSMDPartI);
                                    if (msmd1 != null)
                                    {
                                        msmd = true;
                                    }
                                    msmd = true;

                                }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.MastersinPublicHealth && (qualification.IsVerified == null || qualification.IsVerified == true))
                                {

                                    mph = true;
                                }
                                else
                                {
                                    marks.Remarks = "No Value in Total Marks";
                                    marks.Error = true;
                                }

                            }
                        }
                        else
                        {
                            if (qualification.Required_Degree_Id == (int)DgIdsEnum.FCPSPartII && (qualification.IsVerified == null || qualification.IsVerified == true))
                            {

                                var fcps1 = qualifications.FirstOrDefault(x => x.Id == (int)DgIdsEnum.FCPSPartI);
                                if (fcps1 != null)
                                {
                                    fcps = true;
                                }
                                fcps = true;

                            }
                            else if (qualification.Required_Degree_Id == (int)DgIdsEnum.MCPSPartI && (qualification.IsVerified == null || qualification.IsVerified == true))
                            {
                                if (qualification.ExpFrom != null && qualification.ExpTo != null)
                                {
                                    var datediff = qualification.ExpFrom - qualification.ExpTo;
                                    if (datediff.Value.Days > (360 + 360))
                                    {
                                        mcps = true;
                                    }
                                }
                            }
                            else if (qualification.Required_Degree_Id == (int)DgIdsEnum.MCPSPartII && (qualification.IsVerified == null || qualification.IsVerified == true))
                            {

                                var mcps1 = qualifications.FirstOrDefault(x => x.Id == (int)DgIdsEnum.MCPSPartII);
                                if (mcps1 != null)
                                {
                                    mcps = true;
                                }
                                mcps = true;

                            }
                            else if (qualification.Required_Degree_Id == (int)DgIdsEnum.MSMDPartII && (qualification.IsVerified == null || qualification.IsVerified == true))
                            {

                                var msmd1 = qualifications.FirstOrDefault(x => x.Id == (int)DgIdsEnum.MSMDPartI);
                                if (msmd1 != null)
                                {
                                    msmd = true;
                                }
                                msmd = true;
                            }
                            else if (qualification.Required_Degree_Id == (int)DgIdsEnum.MastersinPublicHealth && (qualification.IsVerified == null || qualification.IsVerified == true))
                            {

                                mph = true;
                            }
                            else
                            {
                                marks.Remarks = "No Value in Obtained Marks";
                                marks.Error = true;
                            }
                        }
                        if (marks.Marks_Id > 0 || marks.Error == true)
                        {
                            marks.IsActive = true;
                            marks.CreatedBy = User.Identity.GetUserName();
                            marks.CreatedDate = DateTime.UtcNow.AddHours(5);
                            marks.UserId = User.Identity.GetUserId();
                            if (string.IsNullOrEmpty(marks.Remarks)) { marks.Remarks = "Marks Added @ " + marks.CreatedDate.ToString(); }
                            db.AdhocApplicationMarks.Add(marks);
                            //db.Entry(marks).State = EntityState.Modified;
                            await db.SaveChangesAsync();
                        }
                        //}
                    }
                }

                var marksHigher = new AdhocApplicationMark();
                marksHigher.Application_Id = application.Id;
                marksHigher.BatchApplicationId = batchApp.Id;
                if (fcps == true && mcps == true && mph == true && msmd == false)
                {
                    marksHigher.Marks_Id = (int)MarksEnum.SecondStageHigher;
                    marksHigher.Marks = 7; tMarks += (int)marksHigher.Marks;
                    marksHigher.Remarks = "fcps == true && mcps == true && mph == true && msmd == false";
                }
                if (fcps == true && msmd == true && mph == true && mcps == false)
                {
                    marksHigher.Marks_Id = (int)MarksEnum.SecondStageHigher;
                    marksHigher.Marks = 7; tMarks += (int)marksHigher.Marks;
                    marksHigher.Remarks = "fcps == true && msmd == true && mph == true && mcps == false";
                }
                if (mcps == true && msmd == true && mph == true && fcps == false)
                {
                    marksHigher.Marks_Id = (int)MarksEnum.SecondStageHigher;
                    marksHigher.Marks = 7; tMarks += (int)marksHigher.Marks;
                    marksHigher.Remarks = "mcps == true && msmd == true && mph == true && fcps == false";
                }
                if (mcps == true && msmd == true && fcps == true && mph == false)
                {
                    marksHigher.Marks_Id = (int)MarksEnum.SecondStageHigher;
                    marksHigher.Marks = 7; tMarks += (int)marksHigher.Marks;
                    marksHigher.Remarks = "mcps == true && msmd == true && fcps == true && mph == false";
                }

                //As
                if (fcps == true && mcps == true && mph == false && msmd == false)
                {
                    marksHigher.Marks_Id = (int)MarksEnum.SecondStageHigher;
                    marksHigher.Marks = 7; tMarks += (int)marksHigher.Marks;
                    marksHigher.Remarks = "fcps == true && mcps == true && mph == false && msmd == false";
                }
                if (fcps == true && mph == true && msmd == false && mcps == false)
                {
                    marksHigher.Marks_Id = (int)MarksEnum.SecondStageHigher;
                    marksHigher.Marks = 7; tMarks += (int)marksHigher.Marks;
                    marksHigher.Remarks = "fcps == true && mph == true && msmd == false && mcps == false";
                }
                if (fcps == true && msmd == true && mph == false && mcps == false)
                {
                    marksHigher.Marks_Id = (int)MarksEnum.SecondStageHigher;
                    marksHigher.Marks = 7; tMarks += (int)marksHigher.Marks;
                    marksHigher.Remarks = "fcps == true && msmd == true && mph == false && mcps == false";
                }
                //As
                if (fcps == false && mph == true && msmd == false && mcps == true)
                {
                    marksHigher.Marks_Id = (int)MarksEnum.FirstStageHigher;
                    marksHigher.Marks = 5; tMarks += (int)marksHigher.Marks;
                    marksHigher.Remarks = "fcps == false && mph == true && msmd == false && mcps == true";
                }
                if (fcps == false && msmd == true && mph == false && mcps == true)
                {
                    marksHigher.Marks_Id = (int)MarksEnum.SecondStageHigher;
                    marksHigher.Marks = 7; tMarks += (int)marksHigher.Marks;
                    marksHigher.Remarks = "fcps == false && msmd == true && mph == false && mcps == true";
                }
                //As
                if (fcps == false && mcps == false && mph == true && msmd == true)
                {
                    marksHigher.Marks_Id = (int)MarksEnum.SecondStageHigher;
                    marksHigher.Marks = 7; tMarks += (int)marksHigher.Marks;
                    marksHigher.Remarks = "fcps == false && mcps == false && mph == true && msmd == true";
                }
                if (fcps == true && mph == false && msmd == false && mcps == false)
                {
                    marksHigher.Marks_Id = (int)MarksEnum.FirstStageHigher;
                    marksHigher.Marks = 5; tMarks += (int)marksHigher.Marks;
                    marksHigher.Remarks = "fcps == true && mph == false && msmd == false && mcps == false";
                }

                if (fcps == false && mph == true && msmd == false && mcps == false)
                {
                    marksHigher.Marks_Id = (int)MarksEnum.FirstStageHigher;
                    marksHigher.Marks = 5; tMarks += (int)marksHigher.Marks;
                    marksHigher.Remarks = "fcps == false && mph == true && msmd == false && mcps == false";
                }

                if (fcps == false && mph == false && msmd == true && mcps == false)
                {
                    marksHigher.Marks_Id = (int)MarksEnum.FirstStageHigher;
                    marksHigher.Marks = 5; tMarks += (int)marksHigher.Marks;
                    marksHigher.Remarks = "fcps == false && mph == false && msmd == true && mcps == false";
                }

                if (fcps == false && mph == false && msmd == false && mcps == true)
                {
                    marksHigher.Marks_Id = (int)MarksEnum.FirstStageHigher;
                    marksHigher.Marks = 5; tMarks += (int)marksHigher.Marks;
                    marksHigher.Remarks = "fcps == false && mph == false && msmd == false && mcps == true";
                }
                if (marksHigher != null && marksHigher.Marks > 0)
                {
                    marksHigher.IsActive = true;
                    marksHigher.CreatedBy = User.Identity.GetUserName();
                    marksHigher.CreatedDate = DateTime.UtcNow.AddHours(5);
                    marksHigher.UserId = User.Identity.GetUserId();
                    if (string.IsNullOrEmpty(marksHigher.Remarks)) { marksHigher.Remarks = "Marks Added @ " + marksHigher.CreatedDate.ToString(); }
                    db.AdhocApplicationMarks.Add(marksHigher);
                    //db.Entry(marks).State = EntityState.Modified;
                    db.SaveChanges();
                }

                var experiences = await db.AdhocApplicantExperiences.Where(x => x.Applicant_Id == batchApp.Applicant_Id && x.IsVerified != false && !x.JobTitle.ToLower().Contains("house") && !x.Organization.ToLower().Contains("house") && !x.JobTitle.ToLower().Contains("grad") && !x.Organization.ToLower().Contains("grad") && !x.JobTitle.ToLower().Contains("pgr") && !x.Organization.ToLower().Contains("pgr") && !x.JobTitle.ToLower().Contains("train") && !x.Organization.ToLower().Contains("train") && x.IsActive == true).ToListAsync();

                if (application.Id == 12289)
                {

                }
                applicant = db.AdhocApplicants.FirstOrDefault(x => x.Id == batchApp.Applicant_Id && x.IsActive == true);
                if (applicant.Position != null && applicant.Position > 0 && applicant.PositionVerified == true)
                {
                    var marksPos = new AdhocApplicationMark();
                    marksPos.Application_Id = application.Id;
                    marksPos.BatchApplicationId = batchApp.Id;
                    if (applicant.Position == 1)
                    {
                        marksPos.Marks_Id = 12;
                        marksPos.Marks = 5;
                    }
                    else if (applicant.Position == 2)
                    {
                        marksPos.Marks_Id = 13;
                        marksPos.Marks = 3;
                    }
                    else if (applicant.Position == 3)
                    {
                        marksPos.Marks_Id = 14;
                        marksPos.Marks = 2;
                    }

                    tMarks += (int)marksPos.Marks;
                    marksPos.Percentage = marksPos.Marks;
                    marksPos.Remarks = "Position";

                    marksPos.IsActive = true;
                    marksPos.CreatedBy = User.Identity.GetUserName();
                    marksPos.CreatedDate = DateTime.UtcNow.AddHours(5);
                    marksPos.UserId = User.Identity.GetUserId();
                    db.AdhocApplicationMarks.Add(marksPos);
                    await db.SaveChangesAsync();
                }
                int days = 0;
                double marksPerDay = 0.00273972602;
                var marksExp = db.AdhocApplicationMarks.FirstOrDefault(x => x.Application_Id == application.Id && x.BatchApplicationId == batchApp.Id && x.Marks_Id == 10 && x.IsActive == true);
                if (marksExp == null)
                {
                    foreach (var experience in experiences)
                    {
                        if (experience.IsVerified == true)
                        {
                            var scrutinyDecision = db.AdhocScrutinies.FirstOrDefault(x => x.Experience_Id == experience.Id && (x.IsAccepted == true || x.GrievanceAccepted == true) && x.IsActive == true);
                            if (scrutinyDecision != null && (scrutinyDecision.IsAccepted == true || scrutinyDecision.GrievanceAccepted == true))
                            {
                                if (experience.FromDate != null && experience.ToDate != null)
                                {
                                    if (experience.ToDate.Value > experience.FromDate.Value)
                                    {
                                        DateTime FromYear = Convert.ToDateTime(experience.FromDate.Value);
                                        DateTime ToYear = Convert.ToDateTime(experience.ToDate.Value);
                                        TimeSpan objTimeSpan = ToYear - FromYear;
                                        days += Convert.ToInt32(objTimeSpan.TotalDays);
                                    }
                                }
                                else if (experience.FromDate != null && experience.IsContinued == true)
                                {
                                    DateTime FromYear = Convert.ToDateTime(experience.FromDate.Value);
                                    DateTime ToYear = Convert.ToDateTime(new DateTime(2021, 12, 10));
                                    TimeSpan objTimeSpan = ToYear - FromYear;
                                    days += Convert.ToInt32(objTimeSpan.TotalDays);
                                }
                            }
                        }
                    }
                }
                if (days > 0)
                {
                    marksExp = new AdhocApplicationMark();
                    marksExp.Application_Id = application.Id;
                    marksExp.BatchApplicationId = batchApp.Id;
                    marksExp.Marks_Id = 10;
                    double dayMarks = days * marksPerDay;
                    if (days > 1825)
                    {
                        marksExp.Marks = 5;
                    }
                    else
                    {
                        marksExp.Marks = Convert.ToDouble(dayMarks.ToString("0.##"));
                        //marksExp.Marks = Math.Ceiling((dayMarks * 100) / 100);
                    }
                    tMarks += (int)marksExp.Marks;
                    marksExp.Percentage = marksExp.Marks;
                    marksExp.Remarks = "Experience";

                    marksExp.IsActive = true;
                    marksExp.CreatedBy = User.Identity.GetUserName();
                    marksExp.CreatedDate = DateTime.UtcNow.AddHours(5);
                    marksExp.UserId = User.Identity.GetUserId();
                    db.AdhocApplicationMarks.Add(marksExp);
                    await db.SaveChangesAsync();
                }
                var totalMarks = db.AdhocApplicationMarks.FirstOrDefault(x => x.BatchApplicationId == batchApp.Id && x.Marks_Id == (int)MarksEnum.Total && x.IsActive == true);
                if (totalMarks == null)
                {
                    totalMarks = new AdhocApplicationMark();
                    totalMarks.Application_Id = application.Id;
                    totalMarks.BatchApplicationId = batchApp.Id;
                    totalMarks.Marks_Id = (int)MarksEnum.Total;
                    totalMarks.Marks = tMarks;
                    totalMarks.Percentage = tMarks;
                    totalMarks.IsActive = true;
                    totalMarks.CreatedBy = User.Identity.GetUserName();
                    totalMarks.CreatedDate = DateTime.UtcNow.AddHours(5);
                    totalMarks.UserId = User.Identity.GetUserId();
                    totalMarks.Remarks = "Total Added @ " + totalMarks.CreatedDate.ToString();
                    db.AdhocApplicationMarks.Add(totalMarks);
                    await db.SaveChangesAsync();
                }
                return (int)totalMarks.Marks;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private int chargeNurseMarks(HR_System db, AdhocApplication application, AdhocInterviewBatchApplicationV batchApp, List<AdhocApplicantQualification> qualifications)
        {
            try
            {


                int tMarks = 0;
                //totalMarks = db.AdhocApplicationMarks.FirstOrDefault(x => x.Application_Id == application.Id && x.Marks_Id == (int)MarksEnum.Total && x.IsActive == true);
                var applicant = db.AdhocApplicants.FirstOrDefault(x => x.Id == application.Applicant_Id && x.Hafiz == true && x.HifzVerified != false && !string.IsNullOrEmpty(x.HifzDocument) && x.IsActive == true);
                if (applicant != null)
                {
                    var hifzMarks = db.AdhocApplicationMarks.FirstOrDefault(x => x.BatchApplicationId == batchApp.Id && x.Marks_Id == (int)MarksEnum.HafizeQuran && x.IsActive == true);
                    if (hifzMarks == null)
                    {
                        hifzMarks = new AdhocApplicationMark();
                        hifzMarks.Application_Id = application.Id;
                        hifzMarks.BatchApplicationId = batchApp.Id;
                        hifzMarks.Marks_Id = (int)MarksEnum.HafizeQuran;
                        hifzMarks.Marks = 5;
                        tMarks += (int)hifzMarks.Marks;
                        hifzMarks.Percentage = 100;
                        hifzMarks.IsActive = true;
                        hifzMarks.CreatedBy = User.Identity.GetUserName();
                        hifzMarks.CreatedDate = DateTime.UtcNow.AddHours(5);
                        hifzMarks.UserId = User.Identity.GetUserId();
                        hifzMarks.Remarks = "Hafiz-e-Quran Added @ " + hifzMarks.CreatedDate.ToString();
                        db.AdhocApplicationMarks.Add(hifzMarks);
                        db.SaveChanges();
                    }
                }
                bool fcps = false, mcps = false, msmd = false, mph = false;
                foreach (var qualification in qualifications)
                {
                    var marks = db.AdhocApplicationMarks.FirstOrDefault(x => x.BatchApplicationId == batchApp.Id && x.ApplicantQualification_Id == qualification.Id && x.IsActive == true);
                    if (marks == null)
                    {
                        marks = new AdhocApplicationMark();
                        marks.Application_Id = application.Id;
                        marks.BatchApplicationId = batchApp.Id;
                        marks.ApplicantQualification_Id = qualification.Id;
                        //var scrutinyDecision = db.AdhocScrutinies.FirstOrDefault(x => x.Qualification_Id == qualification.Id && x.IsAccepted == true && x.IsActive == true);
                        //if (scrutinyDecision == null)
                        //{
                        //    scrutinyDecision = db.AdhocScrutinies.FirstOrDefault(x => x.Qualification_Id == qualification.Id && x.GrievanceAccepted == true && x.IsActive == true);
                        //    if (scrutinyDecision == null)
                        //    {
                        //        marks.Remarks = "No Scrutiny Found!";
                        //        marks.Error = true;
                        //        marks.IsActive = true;
                        //        marks.CreatedBy = User.Identity.GetUserName();
                        //        marks.CreatedDate = DateTime.UtcNow.AddHours(5);
                        //        marks.UserId = User.Identity.GetUserId();
                        //        db.AdhocApplicationMarks.Add(marks);
                        //        db.SaveChanges();
                        //    }
                        //}
                        //if (scrutinyDecision != null)
                        //{
                        var pct = 0.00;
                        if (qualification.ObtainedMarks != null && qualification.ObtainedMarks > 0)
                        {
                            if (qualification.TotalMarks != null && qualification.TotalMarks > 0)
                            {
                                if (qualification.TotalMarks == 4)
                                {
                                    var cgpa = (double)qualification.ObtainedMarks;
                                    if (cgpa >= 3.63)
                                    {
                                        pct = (cgpa - 0.3) / 0.037;
                                    }
                                    else if (cgpa >= 3.25 && cgpa < 3.63)
                                    {
                                        pct = (cgpa - 0.29) / 0.037;
                                    }
                                    else if (cgpa >= 2.88 && cgpa < 3.25)
                                    {
                                        pct = (cgpa - 0.36) / 0.036;
                                    }
                                    else if (cgpa >= 2.5 && cgpa < 2.88)
                                    {
                                        pct = (cgpa - 0.28) / 0.037;
                                    }
                                    else if (cgpa >= 1.8 && cgpa < 2.5)
                                    {
                                        pct = (cgpa + 1.65) / 0.069;
                                    }
                                    else if (cgpa >= 1 && cgpa < 1.8)
                                    {
                                        pct = (cgpa + 2.16) / 0.079;
                                    }
                                    else if (cgpa > 0 && cgpa < 1)
                                    {
                                        pct = (cgpa / 0.0248);
                                    }
                                }
                                else if (qualification.TotalMarks == 5)
                                {
                                    var cgpa = (double)qualification.ObtainedMarks;
                                    if (cgpa >= 4.63)
                                    {
                                        pct = (cgpa - 1.3) / 0.037;
                                    }
                                    else if (cgpa >= 4.25 && cgpa < 4.63)
                                    {
                                        pct = (cgpa - 1.29) / 0.037;
                                    }
                                    else if (cgpa >= 3.88 && cgpa < 4.25)
                                    {
                                        pct = (cgpa - 1.36) / 0.036;
                                    }
                                    else if (cgpa >= 3.5 && cgpa < 3.88)
                                    {
                                        pct = (cgpa - 1.28) / 0.037;
                                    }
                                    else if (cgpa >= 2.8 && cgpa < 3.5)
                                    {
                                        pct = (cgpa + 0.65) / 0.069;
                                    }
                                    else if (cgpa >= 2 && cgpa < 2.8)
                                    {
                                        pct = (cgpa + 1.16) / 0.079;
                                    }
                                    else if (cgpa > 0 && cgpa < 2)
                                    {
                                        pct = (cgpa - 1 / 0.0248);
                                    }
                                }
                                else if (qualification.TotalMarks > 5)
                                {
                                    pct = (double)(qualification.ObtainedMarks / qualification.TotalMarks) * 100;
                                    pct = Math.Round(pct, 2);
                                }
                                if (qualification.Required_Degree_Id == (int)DgIdsEnum.Matriculation || qualification.Required_Degree_Id == (int)DgIdsEnum.OLevel)
                                {
                                    marks.Marks_Id = (int)MarksEnum.Matriculation;
                                    if (pct >= 60) { marks.Marks = 12; tMarks += (int)marks.Marks; }
                                    else if (pct >= 45 && pct < 60) { marks.Marks = 11; tMarks += (int)marks.Marks; }
                                    else if (pct < 45) { marks.Marks = 7; tMarks += (int)marks.Marks; }
                                    marks.Percentage = (double)pct;
                                }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.OLevel)
                                {
                                    marks.Marks_Id = (int)MarksEnum.OLevel;

                                }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.FSCPreMedical || qualification.Required_Degree_Id == (int)DgIdsEnum.ALevel)
                                {
                                    marks.Marks_Id = (int)MarksEnum.Intermedicate;
                                    if (pct >= 60) { marks.Marks = 23; tMarks += (int)marks.Marks; }
                                    else if (pct >= 45 && pct < 60) { marks.Marks = 21; tMarks += (int)marks.Marks; }
                                    else if (pct < 45) { marks.Marks = 14; tMarks += (int)marks.Marks; }
                                    marks.Percentage = (double)pct;
                                }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.ALevel)
                                {
                                    marks.Marks_Id = (int)MarksEnum.ALevel;
                                }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.BSNursing)
                                {
                                    var total = qualification.TotalMarks;
                                    var obtainMarks = qualification.ObtainedMarks;
                                    pct = (double)(obtainMarks / total) * 100;
                                    marks.Marks_Id = (int)MarksEnum.Graduation;
                                    if (pct >= 60) { marks.Marks = 35; tMarks += (int)marks.Marks; }
                                    else if (pct >= 45 && pct < 60) { marks.Marks = 31; tMarks += (int)marks.Marks; }
                                    else if (pct < 45) { marks.Marks = 21; tMarks += (int)marks.Marks; }
                                    marks.Percentage = (double)pct;
                                    marks.Remarks = "MBBS";
                                }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.FSCPreMedical) { }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.GeneralNursing1stYear) { }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.GeneralNursing2ndYear) { }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.GeneralNursing3rdYear) { }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.DiplomainGeneralNursingandMidwifery)
                                {
                                    var gn1 = qualifications.FirstOrDefault(x => x.Required_Degree_Id == (int)DgIdsEnum.GeneralNursing1stYear);
                                    var gn2 = qualifications.FirstOrDefault(x => x.Required_Degree_Id == (int)DgIdsEnum.GeneralNursing2ndYear);
                                    var gn3 = qualifications.FirstOrDefault(x => x.Required_Degree_Id == (int)DgIdsEnum.GeneralNursing3rdYear);
                                    var gn4 = qualifications.FirstOrDefault(x => x.Required_Degree_Id == (int)DgIdsEnum.DiplomainGeneralNursingandMidwifery);
                                    if (gn1 != null && gn2 != null && gn3 != null && gn4 != null)
                                    {

                                        var total = gn1.TotalMarks + gn2.TotalMarks + gn3.TotalMarks + gn4.TotalMarks;
                                        var obtainMarks = gn1.ObtainedMarks + gn2.ObtainedMarks + gn3.ObtainedMarks + gn4.ObtainedMarks;
                                        pct = (double)(obtainMarks / total) * 100;
                                        marks.Marks_Id = (int)MarksEnum.Graduation;
                                        if (pct >= 60) { marks.Marks = 35; tMarks += (int)marks.Marks; }
                                        else if (pct >= 45 && pct < 60) { marks.Marks = 31; tMarks += (int)marks.Marks; }
                                        else if (pct < 45) { marks.Marks = 21; tMarks += (int)marks.Marks; }
                                        marks.Percentage = (double)pct;
                                        marks.Remarks = "GN";
                                        //if (mbbs2.TotalMarks != 600 && (mbbs5.DegreeType == "Pakistan" || string.IsNullOrEmpty(mbbs5.DegreeType)))
                                        //{
                                        //    marks.Remarks = "First Professional MBBS II total marks are not 600";
                                        //}
                                    }
                                }
                            }
                            else
                            {
                                marks.Remarks = "No Value in Total Marks";
                                marks.Error = true;
                            }
                        }
                        else
                        {

                            marks.Remarks = "No Value in Obtained Marks";
                            marks.Error = true;
                        }
                        if (marks.Marks_Id > 0 || marks.Error == true)
                        {
                            marks.IsActive = true;
                            marks.CreatedBy = User.Identity.GetUserName();
                            marks.CreatedDate = DateTime.UtcNow.AddHours(5);
                            marks.UserId = User.Identity.GetUserId();
                            if (string.IsNullOrEmpty(marks.Remarks)) { marks.Remarks = "Marks Added @ " + marks.CreatedDate.ToString(); }
                            db.AdhocApplicationMarks.Add(marks);
                            //db.Entry(marks).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        //}
                    }
                }
                var experiences = db.AdhocApplicantExperiences.Where(x => x.Applicant_Id == batchApp.Applicant_Id && x.IsVerified != false && !x.JobTitle.ToLower().Contains("house") && !x.Organization.ToLower().Contains("house") && !x.JobTitle.ToLower().Contains("grad") && !x.Organization.ToLower().Contains("grad") && !x.JobTitle.ToLower().Contains("pgr") && !x.Organization.ToLower().Contains("pgr") && !x.JobTitle.ToLower().Contains("train") && !x.Organization.ToLower().Contains("train") && x.IsActive == true).ToList();
                if (application.Id == 10197)
                {

                }

                int days = 0;
                double marksPerDay = 0.00273972602;
                var marksExp = db.AdhocApplicationMarks.FirstOrDefault(x => x.Application_Id == application.Id && x.BatchApplicationId == batchApp.Id && x.Marks_Id == 10 && x.IsActive == true);
                if (marksExp == null)
                {
                    foreach (var experience in experiences)
                    {
                        if (experience.IsVerified == true)
                        {
                            var scrutinyDecision = db.AdhocScrutinies.FirstOrDefault(x => x.Experience_Id == experience.Id && (x.IsAccepted == true || x.GrievanceAccepted == true) && x.IsActive == true);
                            if (scrutinyDecision != null && (scrutinyDecision.IsAccepted == true || scrutinyDecision.GrievanceAccepted == true))
                            {
                                if (experience.FromDate != null && experience.ToDate != null)
                                {
                                    if (experience.ToDate.Value > experience.FromDate.Value)
                                    {
                                        DateTime FromYear = Convert.ToDateTime(experience.FromDate.Value);
                                        DateTime ToYear = Convert.ToDateTime(experience.ToDate.Value);
                                        TimeSpan objTimeSpan = ToYear - FromYear;
                                        days += Convert.ToInt32(objTimeSpan.TotalDays);
                                    }
                                }
                                else if (experience.FromDate != null && experience.IsContinued == true)
                                {
                                    DateTime FromYear = Convert.ToDateTime(experience.FromDate.Value);
                                    DateTime ToYear = Convert.ToDateTime(new DateTime(2021, 12, 10));
                                    TimeSpan objTimeSpan = ToYear - FromYear;
                                    days += Convert.ToInt32(objTimeSpan.TotalDays);
                                }
                            }
                        }
                    }
                }
                if (days > 0)
                {
                    marksExp = new AdhocApplicationMark();
                    marksExp.Application_Id = application.Id;
                    marksExp.BatchApplicationId = batchApp.Id;
                    marksExp.Marks_Id = 10;
                    double dayMarks = days * marksPerDay;
                    if (days > 1825)
                    {
                        marksExp.Marks = 5;
                    }
                    else
                    {
                        marksExp.Marks = Convert.ToDouble(dayMarks.ToString("0.##"));
                        //marksExp.Marks = Math.Ceiling((dayMarks * 100) / 100);
                    }
                    tMarks += (int)marksExp.Marks;
                    marksExp.Percentage = marksExp.Marks;
                    marksExp.Remarks = "Experience";

                    marksExp.IsActive = true;
                    marksExp.CreatedBy = User.Identity.GetUserName();
                    marksExp.CreatedDate = DateTime.UtcNow.AddHours(5);
                    marksExp.UserId = User.Identity.GetUserId();
                    db.AdhocApplicationMarks.Add(marksExp);
                    db.SaveChanges();
                }
                var totalMarks = db.AdhocApplicationMarks.FirstOrDefault(x => x.BatchApplicationId == batchApp.Id && x.Marks_Id == (int)MarksEnum.Total && x.IsActive == true);
                if (totalMarks == null)
                {
                    totalMarks = new AdhocApplicationMark();
                    totalMarks.Application_Id = application.Id;
                    totalMarks.BatchApplicationId = batchApp.Id;
                    totalMarks.Marks_Id = (int)MarksEnum.Total;
                    totalMarks.Marks = tMarks;
                    totalMarks.Percentage = tMarks;
                    totalMarks.IsActive = true;
                    totalMarks.CreatedBy = User.Identity.GetUserName();
                    totalMarks.CreatedDate = DateTime.UtcNow.AddHours(5);
                    totalMarks.UserId = User.Identity.GetUserId();
                    totalMarks.Remarks = "Total Added @ " + totalMarks.CreatedDate.ToString();
                    db.AdhocApplicationMarks.Add(totalMarks);
                    db.SaveChanges();
                }
                return (int)totalMarks.Marks;
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        private int dentalSurgeonMarks(HR_System db, AdhocApplication application, AdhocInterviewBatchApplicationV batchApp, List<AdhocApplicantQualification> qualifications)
        {
            try
            {


                int tMarks = 0;
                //totalMarks = db.AdhocApplicationMarks.FirstOrDefault(x => x.Application_Id == application.Id && x.Marks_Id == (int)MarksEnum.Total && x.IsActive == true);
                var applicant = db.AdhocApplicants.FirstOrDefault(x => x.Id == application.Applicant_Id && x.Hafiz == true && x.HifzVerified != false && !string.IsNullOrEmpty(x.HifzDocument) && x.IsActive == true);
                if (applicant != null)
                {
                    var hifzMarks = db.AdhocApplicationMarks.FirstOrDefault(x => x.BatchApplicationId == batchApp.Id && x.Marks_Id == (int)MarksEnum.HafizeQuran && x.IsActive == true);
                    if (hifzMarks == null)
                    {
                        hifzMarks = new AdhocApplicationMark();
                        hifzMarks.Application_Id = application.Id;
                        hifzMarks.BatchApplicationId = batchApp.Id;
                        hifzMarks.Marks_Id = (int)MarksEnum.HafizeQuran;
                        hifzMarks.Marks = 5;
                        tMarks += (int)hifzMarks.Marks;
                        hifzMarks.Percentage = 100;
                        hifzMarks.IsActive = true;
                        hifzMarks.CreatedBy = User.Identity.GetUserName();
                        hifzMarks.CreatedDate = DateTime.UtcNow.AddHours(5);
                        hifzMarks.UserId = User.Identity.GetUserId();
                        hifzMarks.Remarks = "Hafiz-e-Quran Added @ " + hifzMarks.CreatedDate.ToString();
                        db.AdhocApplicationMarks.Add(hifzMarks);
                        db.SaveChanges();
                    }
                }
                applicant = db.AdhocApplicants.FirstOrDefault(x => x.Id == batchApp.Applicant_Id && x.IsActive == true);
                if (applicant.Position != null && applicant.Position > 0 && applicant.PositionVerified == true)
                {
                    var marksPos = db.AdhocApplicationMarks.FirstOrDefault(x => x.Application_Id == application.Id && x.BatchApplicationId == batchApp.Id && x.Marks_Id == 10 && x.IsActive == true);
                    marksPos = new AdhocApplicationMark();
                    marksPos.Application_Id = application.Id;
                    marksPos.BatchApplicationId = batchApp.Id;
                    if (applicant.Position == 1)
                    {
                        marksPos.Marks_Id = 12;
                        marksPos.Marks = 5;
                    }
                    else if (applicant.Position == 2)
                    {
                        marksPos.Marks_Id = 13;
                        marksPos.Marks = 3;
                    }
                    else if (applicant.Position == 3)
                    {
                        marksPos.Marks_Id = 14;
                        marksPos.Marks = 2;
                    }

                    tMarks += (int)marksPos.Marks;
                    marksPos.Percentage = marksPos.Marks;
                    marksPos.Remarks = "Position";

                    marksPos.IsActive = true;
                    marksPos.CreatedBy = User.Identity.GetUserName();
                    marksPos.CreatedDate = DateTime.UtcNow.AddHours(5);
                    marksPos.UserId = User.Identity.GetUserId();
                    db.AdhocApplicationMarks.Add(marksPos);
                    db.SaveChanges();
                }
                bool fcps = false, mcps = false, msmd = false, mph = false;
                foreach (var qualification in qualifications)
                {
                    var marks = db.AdhocApplicationMarks.FirstOrDefault(x => x.BatchApplicationId == batchApp.Id && x.ApplicantQualification_Id == qualification.Id && x.IsActive == true);
                    if (marks == null)
                    {
                        marks = new AdhocApplicationMark();
                        marks.Application_Id = application.Id;
                        marks.BatchApplicationId = batchApp.Id;
                        marks.ApplicantQualification_Id = qualification.Id;
                        //var scrutinyDecision = db.AdhocScrutinies.FirstOrDefault(x => x.Qualification_Id == qualification.Id && x.IsAccepted == true && x.IsActive == true);
                        //if (scrutinyDecision == null)
                        //{
                        //    scrutinyDecision = db.AdhocScrutinies.FirstOrDefault(x => x.Qualification_Id == qualification.Id && x.GrievanceAccepted == true && x.IsActive == true);
                        //    if (scrutinyDecision == null)
                        //    {
                        //        marks.Remarks = "No Scrutiny Found!";
                        //        marks.Error = true;
                        //        marks.IsActive = true;
                        //        marks.CreatedBy = User.Identity.GetUserName();
                        //        marks.CreatedDate = DateTime.UtcNow.AddHours(5);
                        //        marks.UserId = User.Identity.GetUserId();
                        //        db.AdhocApplicationMarks.Add(marks);
                        //        db.SaveChanges();
                        //    }
                        //}
                        //if (scrutinyDecision != null)
                        //{
                        var pct = 0.00;
                        if (qualification.ObtainedMarks != null && qualification.ObtainedMarks > 0)
                        {
                            if (qualification.TotalMarks != null && qualification.TotalMarks > 0)
                            {
                                if (qualification.TotalMarks == 4)
                                {
                                    var cgpa = (double)qualification.ObtainedMarks;
                                    if (cgpa >= 3.63)
                                    {
                                        pct = (cgpa - 0.3) / 0.037;
                                    }
                                    else if (cgpa >= 3.25 && cgpa < 3.63)
                                    {
                                        pct = (cgpa - 0.29) / 0.037;
                                    }
                                    else if (cgpa >= 2.88 && cgpa < 3.25)
                                    {
                                        pct = (cgpa - 0.36) / 0.036;
                                    }
                                    else if (cgpa >= 2.5 && cgpa < 2.88)
                                    {
                                        pct = (cgpa - 0.28) / 0.037;
                                    }
                                    else if (cgpa >= 1.8 && cgpa < 2.5)
                                    {
                                        pct = (cgpa + 1.65) / 0.069;
                                    }
                                    else if (cgpa >= 1 && cgpa < 1.8)
                                    {
                                        pct = (cgpa + 2.16) / 0.079;
                                    }
                                    else if (cgpa > 0 && cgpa < 1)
                                    {
                                        pct = (cgpa / 0.0248);
                                    }
                                }
                                else if (qualification.TotalMarks == 5)
                                {
                                    var cgpa = (double)qualification.ObtainedMarks;
                                    if (cgpa >= 4.63)
                                    {
                                        pct = (cgpa - 1.3) / 0.037;
                                    }
                                    else if (cgpa >= 4.25 && cgpa < 4.63)
                                    {
                                        pct = (cgpa - 1.29) / 0.037;
                                    }
                                    else if (cgpa >= 3.88 && cgpa < 4.25)
                                    {
                                        pct = (cgpa - 1.36) / 0.036;
                                    }
                                    else if (cgpa >= 3.5 && cgpa < 3.88)
                                    {
                                        pct = (cgpa - 1.28) / 0.037;
                                    }
                                    else if (cgpa >= 2.8 && cgpa < 3.5)
                                    {
                                        pct = (cgpa + 0.65) / 0.069;
                                    }
                                    else if (cgpa >= 2 && cgpa < 2.8)
                                    {
                                        pct = (cgpa + 1.16) / 0.079;
                                    }
                                    else if (cgpa > 0 && cgpa < 2)
                                    {
                                        pct = (cgpa - 1 / 0.0248);
                                    }
                                }
                                else if (qualification.TotalMarks > 5)
                                {
                                    pct = (double)(qualification.ObtainedMarks / qualification.TotalMarks) * 100;
                                    pct = Math.Round(pct, 2);
                                }
                                if (qualification.Required_Degree_Id == (int)DgIdsEnum.Matriculation || qualification.Required_Degree_Id == (int)DgIdsEnum.OLevel)
                                {
                                    marks.Marks_Id = (int)MarksEnum.Matriculation;
                                    if (pct >= 60) { marks.Marks = 12; tMarks += (int)marks.Marks; }
                                    else if (pct >= 45 && pct < 60) { marks.Marks = 11; tMarks += (int)marks.Marks; }
                                    else if (pct < 45) { marks.Marks = 7; tMarks += (int)marks.Marks; }
                                    marks.Percentage = (double)pct;
                                }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.OLevel)
                                {
                                    marks.Marks_Id = (int)MarksEnum.OLevel;

                                }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.FSCPreMedical || qualification.Required_Degree_Id == (int)DgIdsEnum.ALevel)
                                {
                                    marks.Marks_Id = (int)MarksEnum.Intermedicate;
                                    if (pct >= 60) { marks.Marks = 23; tMarks += (int)marks.Marks; }
                                    else if (pct >= 45 && pct < 60) { marks.Marks = 21; tMarks += (int)marks.Marks; }
                                    else if (pct < 45) { marks.Marks = 14; tMarks += (int)marks.Marks; }
                                    marks.Percentage = (double)pct;
                                }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.ALevel)
                                {
                                    marks.Marks_Id = (int)MarksEnum.ALevel;
                                }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.BSNursing) { }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.FSCPreMedical) { }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.FirstProfessionalBDS) { }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.SecondProfessionalBDS) { }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.ThirdProfessionalBDS) { }
                                else if (qualification.Required_Degree_Id == (int)DgIdsEnum.FinalProfessionalBDS)
                                {
                                    var bds1 = qualifications.FirstOrDefault(x => x.Required_Degree_Id == (int)DgIdsEnum.FirstProfessionalBDS);
                                    var bds2 = qualifications.FirstOrDefault(x => x.Required_Degree_Id == (int)DgIdsEnum.SecondProfessionalBDS);
                                    var bds3 = qualifications.FirstOrDefault(x => x.Required_Degree_Id == (int)DgIdsEnum.ThirdProfessionalBDS);
                                    var bds4 = qualifications.FirstOrDefault(x => x.Required_Degree_Id == (int)DgIdsEnum.FinalProfessionalBDS);
                                    if (bds1 != null && bds2 != null && bds3 != null && bds4 != null)
                                    {

                                        var total = bds1.TotalMarks + bds2.TotalMarks + bds3.TotalMarks + bds4.TotalMarks;
                                        var obtainMarks = bds1.ObtainedMarks + bds2.ObtainedMarks + bds3.ObtainedMarks + bds4.ObtainedMarks;
                                        pct = (double)(obtainMarks / total) * 100;
                                        marks.Marks_Id = (int)MarksEnum.Graduation;
                                        if (pct >= 60) { marks.Marks = 35; tMarks += (int)marks.Marks; }
                                        else if (pct >= 45 && pct < 60) { marks.Marks = 31; tMarks += (int)marks.Marks; }
                                        else if (pct < 45) { marks.Marks = 21; tMarks += (int)marks.Marks; }
                                        marks.Percentage = (double)pct;
                                        marks.Remarks = "BDS";
                                    }
                                    else if (bds4 != null)
                                    {

                                        var total = bds4.TotalMarks;
                                        var obtainMarks = bds4.ObtainedMarks;
                                        pct = (double)(obtainMarks / total) * 100;
                                        marks.Marks_Id = (int)MarksEnum.Graduation;
                                        if (pct >= 60) { marks.Marks = 35; tMarks += (int)marks.Marks; }
                                        else if (pct >= 45 && pct < 60) { marks.Marks = 31; tMarks += (int)marks.Marks; }
                                        else if (pct < 45) { marks.Marks = 21; tMarks += (int)marks.Marks; }
                                        marks.Percentage = (double)pct;
                                        marks.Remarks = "BDS";
                                    }
                                }
                            }
                            else
                            {
                                marks.Remarks = "No Value in Total Marks";
                                marks.Error = true;
                            }
                        }
                        else
                        {

                            marks.Remarks = "No Value in Obtained Marks";
                            marks.Error = true;
                        }
                        if (marks.Marks_Id > 0 || marks.Error == true)
                        {
                            marks.IsActive = true;
                            marks.CreatedBy = User.Identity.GetUserName();
                            marks.CreatedDate = DateTime.UtcNow.AddHours(5);
                            marks.UserId = User.Identity.GetUserId();
                            if (string.IsNullOrEmpty(marks.Remarks)) { marks.Remarks = "Marks Added @ " + marks.CreatedDate.ToString(); }
                            db.AdhocApplicationMarks.Add(marks);
                            //db.Entry(marks).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        //}
                    }
                }
                var experiences = db.AdhocApplicantExperiences.Where(x => x.Applicant_Id == batchApp.Applicant_Id && x.IsVerified != false && !x.JobTitle.ToLower().Contains("house") && !x.Organization.ToLower().Contains("house") && !x.JobTitle.ToLower().Contains("grad") && !x.Organization.ToLower().Contains("grad") && !x.JobTitle.ToLower().Contains("pgr") && !x.Organization.ToLower().Contains("pgr") && !x.JobTitle.ToLower().Contains("train") && !x.Organization.ToLower().Contains("train") && x.IsActive == true).ToList();

                int days = 0;
                double marksPerDay = 0.00273972602;
                var marksExp = db.AdhocApplicationMarks.FirstOrDefault(x => x.Application_Id == application.Id && x.BatchApplicationId == batchApp.Id && x.Marks_Id == 10 && x.IsActive == true);
                if (marksExp == null)
                {
                    foreach (var experience in experiences)
                    {
                        if (experience.IsVerified == true)
                        {
                            var scrutinyDecision = db.AdhocScrutinies.FirstOrDefault(x => x.Experience_Id == experience.Id && (x.IsAccepted == true || x.GrievanceAccepted == true) && x.IsActive == true);
                            if (scrutinyDecision != null && (scrutinyDecision.IsAccepted == true || scrutinyDecision.GrievanceAccepted == true))
                            {
                                if (experience.FromDate != null && experience.ToDate != null)
                                {
                                    if (experience.ToDate.Value > experience.FromDate.Value)
                                    {
                                        DateTime FromYear = Convert.ToDateTime(experience.FromDate.Value);
                                        DateTime ToYear = Convert.ToDateTime(experience.ToDate.Value);
                                        TimeSpan objTimeSpan = ToYear - FromYear;
                                        days += Convert.ToInt32(objTimeSpan.TotalDays);
                                    }
                                }
                                else if (experience.FromDate != null && experience.IsContinued == true)
                                {
                                    DateTime FromYear = Convert.ToDateTime(experience.FromDate.Value);
                                    DateTime ToYear = Convert.ToDateTime(new DateTime(2021, 12, 10));
                                    TimeSpan objTimeSpan = ToYear - FromYear;
                                    days += Convert.ToInt32(objTimeSpan.TotalDays);
                                }
                            }
                        }
                    }
                }
                if (days > 0)
                {
                    marksExp = new AdhocApplicationMark();
                    marksExp.Application_Id = application.Id;
                    marksExp.BatchApplicationId = batchApp.Id;
                    marksExp.Marks_Id = 10;
                    double dayMarks = days * marksPerDay;
                    if (days > 1825)
                    {
                        marksExp.Marks = 5;
                    }
                    else
                    {
                        marksExp.Marks = Convert.ToDouble(dayMarks.ToString("0.##"));
                        //marksExp.Marks = Math.Ceiling((dayMarks * 100) / 100);
                    }
                    tMarks += (int)marksExp.Marks;
                    marksExp.Percentage = marksExp.Marks;
                    marksExp.Remarks = "Experience";

                    marksExp.IsActive = true;
                    marksExp.CreatedBy = User.Identity.GetUserName();
                    marksExp.CreatedDate = DateTime.UtcNow.AddHours(5);
                    marksExp.UserId = User.Identity.GetUserId();
                    db.AdhocApplicationMarks.Add(marksExp);
                    db.SaveChanges();
                }
                var totalMarks = db.AdhocApplicationMarks.FirstOrDefault(x => x.BatchApplicationId == batchApp.Id && x.Marks_Id == (int)MarksEnum.Total && x.IsActive == true);
                if (totalMarks == null)
                {
                    totalMarks = new AdhocApplicationMark();
                    totalMarks.Application_Id = application.Id;
                    totalMarks.BatchApplicationId = batchApp.Id;
                    totalMarks.Marks_Id = (int)MarksEnum.Total;
                    totalMarks.Marks = tMarks;
                    totalMarks.Percentage = tMarks;
                    totalMarks.IsActive = true;
                    totalMarks.CreatedBy = User.Identity.GetUserName();
                    totalMarks.CreatedDate = DateTime.UtcNow.AddHours(5);
                    totalMarks.UserId = User.Identity.GetUserId();
                    totalMarks.Remarks = "Total Added @ " + totalMarks.CreatedDate.ToString();
                    db.AdhocApplicationMarks.Add(totalMarks);
                    db.SaveChanges();
                }
                return (int)totalMarks.Marks;
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        public double CalculateDistance(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            double? rlat1 = Math.PI * latitude1 / 180;
            double? rlat2 = Math.PI * latitude2 / 180;
            double? theta = longitude1 - longitude2;
            double? rtheta = Math.PI * theta / 180;
            double? dist =
                Math.Sin(rlat1.Value) * Math.Sin(rlat2.Value) + Math.Cos(rlat1.Value) *
                Math.Cos(rlat2.Value) * Math.Cos(rtheta.Value);
            dist = Math.Acos(dist.Value);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;
            return Math.Round((double)dist, 1);
        }
    }
    public class AdhocSaveDTO
    {
        public AdhocJob job { get; set; }
        public List<AdhocDocument> JobDocuments { get; set; }
        public List<AdhocHF> JobHFs { get; set; }
    }
    public class AdhocDesignationVacant
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public int Sanctioned { get; set; }
        public int Filled { get; set; }
        public int Vacant { get; set; }
        public int? PHFMC { get; set; }
    }
    public class DistanceAndHFId
    {
        public int HFId { get; set; }
        public double Distance { get; set; }
        public string FacilityName { get; set; }
    }
    public class AdhocHFVacant
    {
        public int? Id { get; set; }
        public int? DesgId { get; set; }
        public string HFMISCode { get; set; }
        public string DsgName { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public int Sanctioned { get; set; }
        public int Filled { get; set; }
        public int Vacant { get; set; }
        public int? PHFMC { get; set; }
    }
    public class AdhocInterviewDto
    {
        public AdhocInterview AdhocInterview { get; set; }
        public AdhocInterviewBatchDto AdhocInterviewBatch { get; set; }
    }
    public class AdhocInterviewBatchDto
    {
        public AdhocInterviewBatch AdhocInterviewBatch { get; set; }
        public List<AdhocInterviewBatchCommittee> AdhocInterviewBatchCommittees { get; set; }
        public List<int> ApplicationIds { get; set; }
    }
    public class AdhocDistrictVacant
    {
        public int? Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string DesignationName { get; set; }
        public int DesignationId { get; set; }
        public int Count { get; set; }
        public int Sanctioned { get; set; }
        public int Filled { get; set; }
        public int Vacant { get; set; }
        public int? PHFMC { get; set; }
    }
    public class AdhocApplicationsDto
    {
        public int? Id { get; set; }
        public string HFMISCode { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public int Sanctioned { get; set; }
        public int Filled { get; set; }
        public int Vacant { get; set; }
        public int? PHFMC { get; set; }
    }
    public class AdhocApplicaitionFilters : Paginator
    {
        public string hfmisCode { get; set; }
        public string Query { get; set; }
        public string DesignationName { get; set; }
        public int Designation_Id { get; set; }
        public int applicantId { get; set; }
        public int batchId { get; set; }
        public int Status_Id { get; set; }

    }
    public class AdhocApplicantIds
    {
        public int Id { get; set; }
    }
    public class AdhocStatusGroupDto
    {
        public int StatusId { get; set; }
        public string Name { get; set; }
        public int Total { get; set; }
    }
    public class AdhocApplicantPMCDto
    {
        public AdhocApplicantPMC AdhocApplicantPMC { get; set; }
        public List<AdhocApplicantPMCQualification> Qualifications { get; set; }
    }
    public class AdhocApplicantsDto
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string FatherName { get; set; }
        public string DomicileName { get; set; }
        public string HFMISCode { get; set; }
        public string CNIC { get; set; }
        public int? Count { get; set; }
        public List<AdhocApplicantDocumentView> ApplicantDocuments { get; set; }
        public List<AdhocApplicantExperience> ApplicantExperiences { get; set; }
    }
    public enum DgIdsEnum
    {
        Matriculation = 58,
        FirstProfessionalMBBSI = 62,
        FSCPreMedical = 65,
        OLevel = 93,
        ALevel = 94,
        FirstProfessionalBDS = 113,
        FirstProfessionalMBBSII = 116,
        SecondProfessionalMBBS = 117,
        ThirdProfessionalMBBS = 118,
        FinalProfessionalMBBS = 119,
        FCPSPartI = 122,
        FCPSPartII = 123,
        MCPSPartI = 124,
        MSMDPartI = 125,
        MSMDPartII = 126,
        MCPSPartII = 127,
        MastersinPublicHealth = 128,
        BSNursing = 63,
        GeneralNursing1stYear = 129,
        DiplomainAnesthesia = 130,
        GeneralNursing2ndYear = 131,
        GeneralNursing3rdYear = 132,
        DiplomainGeneralNursingandMidwifery = 133,
        SecondProfessionalBDS = 134,
        ThirdProfessionalBDS = 135,
        FinalProfessionalBDS = 136,
        DMRD = 137,
        DiplomainCardiology = 138,
        DiplomainDermatology = 139,
        DLO = 140,
        DOMS = 141,
        DCH = 142,
        DCP = 143,
        MSUrology = 144,
        DiplomainNephrology = 145,
        DTCD = 146,
        DGO = 147,
        MPhill = 120,
        DPM = 148
    }
    public enum MarksEnum
    {
        Matriculation = 1,
        Intermedicate = 2,
        OLevel = 3,
        ALevel = 4,
        Masters = 5,
        FirstStageHigher = 6,
        SecondStageHigher = 7,
        ThirdStageHigher = 8,
        HafizeQuran = 9,
        Experience = 10,
        Interview = 11,
        FirstPosition = 12,
        SecondPosition = 13,
        ThirdPosition = 14,
        Total = 15,
        Graduation = 16
    }

}