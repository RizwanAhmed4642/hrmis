using Hrmis.Models.Common;
using Hrmis.Models.DbModel;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Hrmis.Controllers.HrmisRestApi
{
    [Authorize]
    [RoutePrefix("api/Job")]
    public class JobController : ApiController
    {
        //private readonly ApplicationService _applicationService;

        //public ApplicationController()
        //{
        //    _applicationService = new ApplicationService();
        //}

        [HttpPost]
        [Route("SaveJob")]
        public IHttpActionResult SaveJob([FromBody] JobSaveDTO obj)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var job = db.Jobs.FirstOrDefault(x => x.Designation_Id == obj.job.Designation_Id);
                    if (job == null)
                    {
                        obj.job.CreatedBy = User.Identity.GetUserName();
                        obj.job.CreatedDate = DateTime.UtcNow.AddHours(5);
                        obj.job.UserId = User.Identity.GetUserId();
                        db.Jobs.Add(obj.job);
                        db.SaveChanges();
                        foreach (var item in obj.JobDocuments)
                        {
                            var jobDoc = new JobDocumentRequired();
                            jobDoc.JobDocument_Id = item.Id;
                            jobDoc.Job_Id = obj.job.Id;
                            jobDoc.IsActive = true;
                            jobDoc.CreatedBy = User.Identity.GetUserName();
                            jobDoc.CreatedDate = DateTime.UtcNow.AddHours(5);
                            jobDoc.UserId = User.Identity.GetUserId();
                            db.JobDocumentRequireds.Add(jobDoc);
                            db.SaveChanges();
                        }
                        foreach (var item in obj.JobHFs)
                        {
                            var jobHF = new JobHF();
                            jobHF.Job_Id = obj.job.Id;
                            jobHF.HF_Id = item.HF_Id;
                            jobHF.HFMISCode = item.HFMISCode;
                            jobHF.SeatsOpen = item.SeatsOpen;
                            jobHF.IsActive = true;
                            jobHF.CreatedBy = User.Identity.GetUserName();
                            jobHF.CreatedDate = DateTime.UtcNow.AddHours(5);
                            jobHF.UserId = User.Identity.GetUserId();
                            db.JobHFs.Add(jobHF);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        job.IsActive = obj.job.IsActive;
                        job.AgeLimit = obj.job.AgeLimit;
                        job.RelevantExperience = obj.job.RelevantExperience;
                        job.Experience = obj.job.Experience;
                        job.TotalPreferences = obj.job.TotalPreferences;
                        db.Entry(job).State = EntityState.Modified;
                        db.SaveChanges();
                        JobLog jobLog = new JobLog();
                        jobLog.Job_Id = job.Id;
                        jobLog.TurnedON = job.IsActive;
                        jobLog.Remarks = "Job Turned " + (job.IsActive == true ? "On" : "Off") + ". By " + User.Identity.GetUserName();
                        jobLog.Username = User.Identity.GetUserName();
                        jobLog.DateTime = DateTime.UtcNow.AddHours(5);
                        jobLog.UserId = User.Identity.GetUserId();
                        db.JobLogs.Add(jobLog);
                        db.SaveChanges();
                        return Ok(job);
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

        [HttpPost]
        [Route("SaveApplicant")]
        public IHttpActionResult SaveApplicant([FromBody] JobApplicant obj)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    if (obj.Id == 0)
                    {
                        obj.Status_Id = 2;
                        obj.IsActive = true;
                        obj.CreatedBy = User.Identity.GetUserName();
                        obj.CreatedDate = DateTime.UtcNow.AddHours(5);
                        obj.UserId = User.Identity.GetUserId();
                        db.JobApplicants.Add(obj);
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
        [Route("SaveApplication")]
        public IHttpActionResult SaveApplication([FromBody] JobApplication obj)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var recentApplications = db.JobApplications.Where(x => x.Applicant_Id == obj.Applicant_Id && x.IsActive == true).ToList();
                    if (recentApplications.Count >= 1)
                    {
                        return Ok("limit");
                    }
                    if (obj.Id == 0)
                    {
                        var applicant = db.JobApplicants.FirstOrDefault(x => x.Id == obj.Applicant_Id);

                        var job = db.Jobs.FirstOrDefault(x => x.Designation_Id == obj.Designation_Id && x.IsActive == true);
                        obj.Job_Id = job.Id;
                        obj.Status_Id = 1;
                        obj.IsActive = true;
                        obj.CreatedBy = User.Identity.GetUserName();
                        obj.CreatedDate = DateTime.UtcNow.AddHours(5);
                        obj.UserId = User.Identity.GetUserId();
                        db.JobApplications.Add(obj);
                        db.SaveChanges();
                        if (applicant.Status_Id != 5)
                        {
                            applicant.Status_Id = 5;
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
        [Route("GetApplication/{applicantId}/{designationId}")]
        public IHttpActionResult GetApplication(int applicantId, int designationId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var recentApplication = db.JobApplications.FirstOrDefault(x => x.Applicant_Id == applicantId && x.Designation_Id == designationId && x.IsActive == true);
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
        [HttpPost]
        [Route("SaveExperience")]
        public IHttpActionResult SaveExperience([FromBody] JobApplicantExperience obj)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    if (obj.Id == 0)
                    {
                        var applicant = db.JobApplicants.FirstOrDefault(x => x.Id == obj.Applicant_Id);

                        obj.IsActive = true;
                        obj.CreatedBy = User.Identity.GetUserName();
                        obj.CreatedDate = DateTime.UtcNow.AddHours(5);
                        obj.UserId = User.Identity.GetUserId();
                        db.JobApplicantExperiences.Add(obj);
                        db.SaveChanges();
                        if (applicant.Status_Id != 4)
                        {
                            applicant.Status_Id = 4;
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
                    var experiences = db.JobApplicantExperiences.Where(x => x.Applicant_Id == applicantId && x.IsActive == true).ToList();
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
        [Route("GetApplicantDocuments/{applicantId}")]
        public IHttpActionResult GetApplicantDocuments(int applicantId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var applicantDocuments = db.JobApplicantDocumentViews.Where(x => x.Applicant_Id == applicantId && x.IsActive == true).OrderBy(k => k.OrderBy).ToList();
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
        [Route("RemoveExperience/{experienceId}")]
        public IHttpActionResult RemoveExperience(int experienceId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var experience = db.JobApplicantExperiences.FirstOrDefault(x => x.Id == experienceId);
                    if (experience != null)
                    {
                        experience.IsActive = false;
                        db.Entry(experience).State = EntityState.Modified;
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
        [Route("GetJobApplications")]
        public IHttpActionResult GetJobApplications([FromBody] JobApplicaitionFilters filters)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var applications = db.JobApplicationViews.Where(x => x.Applicant_Id == filters.applicantId && x.IsActive == true).ToList();
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
        [HttpPost]
        [Route("GetJobApplicants")]
        public IHttpActionResult GetJobApplicants([FromBody] JobApplicaitionFilters filters)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var applicantsQuery = db.JobApplicants.Where(x => x.IsActive == true).AsQueryable();
                    var applicants = applicantsQuery.OrderBy(l => l.Name).Skip(filters.Skip).Take(filters.PageSize).ToList();
                    var totalRecords = applicantsQuery.Count();
                    return Ok(applicants);
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
        [Route("JobApplicantDocuments/{applicantId}")]
        public IHttpActionResult GetJobApplicants(int applicantId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var jobApplicantDocumentViews = db.JobApplicantDocumentViews.Where(x => x.Applicant_Id == applicantId && x.IsActive == true).ToList();
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
                    var jobApplicantExperiences = db.JobApplicantExperiences.Where(x => x.Applicant_Id == applicantId && x.IsActive == true).ToList();
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
        [Route("RemoveApplication/{Id}")]
        public IHttpActionResult RemoveApplication(int Id)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var application = db.JobApplications.FirstOrDefault(x => x.Id == Id);
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
        [Route("GetJobs")]
        public IHttpActionResult GetJobs()
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var jobs = db.JobViews.Where(x => x.IsActive == true).ToList();
                    return Ok(jobs);
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
                    var applicant = db.JobApplicantViews.FirstOrDefault(x => x.CNIC.Equals(cnic) && x.IsActive == true);
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
                    var documents = db.JobDocuments.Where(x => x.IsActive == true).ToList();
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
        [Route("GetPHFMCVacants/{type}/{desigId}")]
        [HttpGet]
        public IHttpActionResult GetPHFMCVacants(string type, int desigId)
        {
            try
            {
                using (var db = new HR_System())
                {

                    db.Configuration.ProxyCreationEnabled = false;
                    if (User.Identity.GetUserName() == "phfmc.hr")
                    {
                        if (type == "designations" && desigId == 0)
                        {
                            var designations = db.VpMeritPreferenceViews.Where(x => x.HFAC == 2 && x.Vacant > 0).GroupBy(x => new { x.Desg_Id, x.DsgName }).Select(x => new DesignationVacant
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
                            var query = db.VpMeritPreferenceViews.Where(x => x.Desg_Id == desigId && x.HFAC == 2 && x.Vacant > 0).AsQueryable();
                            var districts = query.GroupBy(x => new { x.DistrictName, x.DistrictCode }).Select(x => new DistrictVacant
                            {
                                Code = x.Key.DistrictCode,
                                Name = x.Key.DistrictName,
                                Sanctioned = x.Sum(k => k.TotalSanctioned),
                                Filled = x.Sum(k => k.TotalWorking),
                                Vacant = x.Sum(k => k.Vacant),
                                PHFMC = x.Sum(k => k.PHFMC),
                                Count = x.Count()
                            }).OrderBy(x => x.Name).ToList();
                            var hfs = query.GroupBy(x => new { x.HF_Id, x.HFMISCode, x.HFName }).Select(x => new HFVacant
                            {
                                Id = x.Key.HF_Id,
                                HFMISCode = x.Key.HFMISCode,
                                Name = x.Key.HFName,
                                Sanctioned = x.Sum(k => k.TotalSanctioned),
                                Filled = x.Sum(k => k.TotalWorking),
                                Vacant = x.Sum(k => k.Vacant),
                                PHFMC = x.Sum(k => k.PHFMC),
                                Count = x.Count()
                            }).OrderBy(x => x.Name).ToList();
                            return Ok(new { hfs, districts });
                        }

                    }
                    else
                    {
                        if (type == "designations" && desigId == 0)
                        {
                            List<int?> desigsAllowed = new List<int?>();
                            desigsAllowed = db.Jobs.Where(x => x.IsActive == true).Select(k => k.Designation_Id).ToList();
                            var designations = db.VpMeritPreferenceViews.Where(x => x.HFAC == 2 && desigsAllowed.Contains(x.Desg_Id) && x.Vacant > 0).GroupBy(x => new { x.Desg_Id, x.DsgName }).Select(x => new DesignationVacant
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
                            var query = db.VpMeritPreferenceViews.Where(x => x.Desg_Id == desigId && x.HFAC == 2 && x.Vacant > 0).AsQueryable();
                            var districts = query.GroupBy(x => new { x.DistrictName, x.DistrictCode }).Select(x => new DistrictVacant
                            {
                                Code = x.Key.DistrictCode,
                                Name = x.Key.DistrictName,
                                Sanctioned = x.Sum(k => k.TotalSanctioned),
                                Filled = x.Sum(k => k.TotalWorking),
                                Vacant = x.Sum(k => k.Vacant),
                                PHFMC = x.Sum(k => k.PHFMC),
                                Count = x.Count()
                            }).OrderBy(x => x.Name).ToList();
                            var hfs = query.GroupBy(x => new { x.HF_Id, x.HFMISCode, x.HFName }).Select(x => new HFVacant
                            {
                                Id = x.Key.HF_Id,
                                HFMISCode = x.Key.HFMISCode,
                                Name = x.Key.HFName,
                                Sanctioned = x.Sum(k => k.TotalSanctioned),
                                Filled = x.Sum(k => k.TotalWorking),
                                Vacant = x.Sum(k => k.Vacant),
                                PHFMC = x.Sum(k => k.PHFMC),
                                Count = x.Count()
                            }).OrderBy(x => x.Name).ToList();
                            return Ok(new { hfs, districts });
                        }
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
        [Route("GetAdhocVacants/{type}/{desigId}")]
        [HttpGet]
        public IHttpActionResult GetAdhocVacants(string type, int desigId)
        {
            try
            {
                using (var db = new HR_System())
                {

                    db.Configuration.ProxyCreationEnabled = false;

                    if (type == "designations" && desigId == 0)
                    {
                        List<int?> desigsAllowed = new List<int?>();
                        desigsAllowed = db.Jobs.Where(x => x.IsActive == true).Select(k => k.Designation_Id).ToList();
                        var designations = db.VpMeritPreferenceViews.Where(x => x.HFAC == 2 && desigsAllowed.Contains(x.Desg_Id) && x.Vacant > 0).GroupBy(x => new { x.Desg_Id, x.DsgName }).Select(x => new DesignationVacant
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
                        var query = db.VpMeritPreferenceViews.Where(x => x.Desg_Id == desigId && x.HFAC == 1 && x.Vacant > 0).AsQueryable();
                        var districts = query.GroupBy(x => new { x.DistrictName, x.DistrictCode }).Select(x => new DistrictVacant
                        {
                            Code = x.Key.DistrictCode,
                            Name = x.Key.DistrictName,
                            Sanctioned = x.Sum(k => k.TotalSanctioned),
                            Filled = x.Sum(k => k.TotalWorking),
                            Vacant = x.Sum(k => k.Vacant),
                            PHFMC = x.Sum(k => k.PHFMC),
                            Count = x.Count()
                        }).OrderBy(x => x.Name).ToList();
                        var hfs = query.GroupBy(x => new { x.HF_Id, x.HFMISCode, x.HFName }).Select(x => new HFVacant
                        {
                            Id = x.Key.HF_Id,
                            HFMISCode = x.Key.HFMISCode,
                            Name = x.Key.HFName,
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

                    string RootPath = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\PHFMCApplicants\Photo";
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

                    var applicant = _db.JobApplicants.FirstOrDefault(x => x.CNIC.Equals(cnic));
                    if (applicant == null)
                    {
                        return false;
                    }
                    if (!Request.Content.IsMimeMultipartContent())
                        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                    string RootPath = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\PHFMCApplicants\ApplicantDocuments\";
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
                        JobApplicantDocument jobApplicantDocument = new JobApplicantDocument();
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
                            jobApplicantDocument.UploadPath = @"Uploads\PHFMCApplicants\ApplicantDocuments\" + filename;
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
                            _db.JobApplicantDocuments.Add(jobApplicantDocument);
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

                    string RootPath = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\PHFMC\ExperienceCertificate\";
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


                        var experience = _db.JobApplicantExperiences.FirstOrDefault(x => x.Id == experienceId);
                        if (experience != null)
                        {
                            experience.UploadPath = @"Uploads/PHFMC/ExperienceCertificate/" + filename;
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
    }
    public class JobSaveDTO
    {
        public Job job { get; set; }
        public List<JobDocument> JobDocuments { get; set; }
        public List<JobHF> JobHFs { get; set; }
    }
    public class DesignationVacant
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public int Sanctioned { get; set; }
        public int Filled { get; set; }
        public int Vacant { get; set; }
        public int? PHFMC { get; set; }
    }
    public class HFVacant
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
    public class DistrictVacant
    {
        public int? Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public int Sanctioned { get; set; }
        public int Filled { get; set; }
        public int Vacant { get; set; }
        public int? PHFMC { get; set; }
    }
    public class JobApplicaitionsDto
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
    public class JobApplicaitionFilters : Paginator
    {
        public string hfmisCode { get; set; }
        public int applicantId { get; set; }
    }
    public class JobApplicantsDto
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string FatherName { get; set; }
        public string DomicileName { get; set; }
        public string HFMISCode { get; set; }
        public string CNIC { get; set; }
        public int? Count { get; set; }
        public List<JobApplicantDocumentView> ApplicantDocuments { get; set; }
        public List<JobApplicantExperience> ApplicantExperiences { get; set; }
    }
}
