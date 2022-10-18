using Hrmis.Models.Common;
using Hrmis.Models.DbModel;
using Hrmis.Models.Services;
using Hrmis.Models.ViewModels.Application;
using Hrmis.Models.ViewModels.Common;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Hrmis.Controllers.PublicControllers
{
    [AllowAnonymous]
    [RoutePrefix("api/Main")]
    public class MainController : ApiController
    {
        private readonly PublicService _publicService;
        private readonly RootService _rootService;
        private readonly DatabaseService _databaseService;

        public MainController()
        {
            _publicService = new PublicService();
            _rootService = new RootService();
            _databaseService = new DatabaseService();
        }
        [HttpGet]
        [Route("GetApplication/{Tracking}")]
        public IHttpActionResult GetApplication(int Tracking)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    ApplicationFts applicationFts = new ApplicationFts();
                    FilesACRService filesACRService = new FilesACRService();
                    applicationFts.application = _db.ApplicationViews.FirstOrDefault(x => x.TrackingNumber == Tracking && x.IsActive == true);
                    if (applicationFts.application == null)
                    {
                        return Ok("No Application Found.");
                    }
                    applicationFts.applicationAttachments = _db.AttachmentViews.Where(x => x.Application_Id == applicationFts.application.Id).ToList();
                    applicationFts.applicationTracks = _publicService.GetApplicationTrack(applicationFts.application.Id);
                    return Ok(applicationFts);
                }
                catch (Exception ex)
                {
                    Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex);
                    return Ok(ex);
                }
            }

        }

        [Route("TestUfone")]
        [HttpGet]
        public async Task<IHttpActionResult> TestUfone()
        {
            try
            {
                await Common.SendSMSUfone(null);
                return Ok(true);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [Route("GetPromotedCandidate/{CNIC}")]
        [HttpGet]
        public IHttpActionResult GetPromotedCandidate(string CNIC)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var profile = _db.ProfileDetailsViews.FirstOrDefault(x => x.CNIC.Equals(CNIC));
                    var candidate = _db.PromotedCandidateViews.FirstOrDefault(x => x.CNIC.Equals(CNIC));
                    return Ok(new { candidate, profile });
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }

        [Route("GetPreferencesList/{CNIC}")]
        [HttpGet]
        public IHttpActionResult GetPreferencesList(string CNIC)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var preferences = _db.APMOPrefsViews.Where(x => x.Id > 1074 && x.CNIC.Equals(CNIC)).OrderBy(x => x.DateTime).ToList();
                    return Ok(preferences);
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetPromotedCandidates")]
        public IHttpActionResult GetPromotedCandidates([FromBody] PromotionFilter filter)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    var Count = 0;
                    var List = new List<PromotedCandidateView2>();
                    IQueryable<PromotedCandidateView2> applicants = _db.PromotedCandidateView2.AsQueryable();
                    if (!string.IsNullOrWhiteSpace(filter.Query))
                    {
                        filter.Query = filter.Query.ToLower().Trim();
                        applicants = applicants.Where(x => x.CNIC.ToLower().Contains(filter.Query) || x.Name.ToLower().Contains(filter.Query)).AsQueryable();
                    }
                    if (filter.DesignationId != 0)
                    {
                        applicants = applicants.Where(x => x.DesignationId == filter.DesignationId).AsQueryable();
                    }

                    Count = applicants.Count();
                    List = applicants.OrderBy(x => x.Name).Skip(filter.Skip).Take(filter.PageSize).ToList();

                    //if (filter.ActiveDesignationId != 0)
                    //{
                    //    applicants = applicants.Where(x => x.MeritsActiveDesignationId == filter.ActiveDesignationId).AsQueryable();
                    //}

                    //var postings = db.usp_MeritPosting().OrderBy(x => x.MeritNumber).Skip(filter.Skip).Take(filter.PageSize).ToList();
                    //var summary = applicants.GroupBy(x => new
                    //{
                    //    x.Status
                    //}).Select(l => new MeritStats
                    //{
                    //    Name = l.Key.Status,
                    //    Count = l.Count()
                    //}).ToList();
                    return Ok(new { List, Count });
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("SavePromotedCandidate")]
        [HttpPost]
        public IHttpActionResult SavePromotedCandidate([FromBody] ConsultantProfile consultantProfile)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var candidate = _db.HrProfiles.FirstOrDefault(x => x.CNIC.Equals(consultantProfile.CNIC));
                    if (candidate.Id != 0)
                    {
                        candidate.EmployeeName = consultantProfile.EmployeeName;
                        candidate.FatherName = consultantProfile.FatherName;
                        candidate.Domicile_Id = consultantProfile.Domicile_Id;
                        candidate.Status_Id = consultantProfile.Status_Id;
                        candidate.DateOfBirth = consultantProfile.DateOfBirth;
                        candidate.HealthFacility_Id = consultantProfile.HF_Id;
                        candidate.Designation_Id = consultantProfile.designationId;
                        candidate.DateOfRegularization = consultantProfile.DateOfRegularization;
                        candidate.SeniorityNo = consultantProfile.SeniorityNo;
                        candidate.PermanentAddress = consultantProfile.PermanentAddress;
                        candidate.MobileNo = consultantProfile.MobileNo;
                        candidate.EMaiL = consultantProfile.EMaiL;
                        _db.Entry(candidate).State = EntityState.Modified;
                        _db.SaveChanges();

                        if (candidate.EntityLifecycle_Id == null)
                        {
                            Entity_Lifecycle elc = new Entity_Lifecycle();
                            elc.Created_Date = DateTime.UtcNow.AddHours(5);
                            elc.Created_By = consultantProfile.CNIC + " (added after migration)";
                            elc.Users_Id = consultantProfile.CNIC;
                            elc.IsActive = true;
                            elc.Entity_Id = 9;
                            _db.Entity_Lifecycle.Add(elc);
                            _db.SaveChanges();
                            candidate.EntityLifecycle_Id = elc.Id;
                        }
                        Entity_Modified_Log eml = new Entity_Modified_Log();
                        eml.Modified_By = consultantProfile.CNIC;
                        eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                        eml.Entity_Lifecycle_Id = (long)candidate.EntityLifecycle_Id;
                        eml.Description = "Profile Updated By " + consultantProfile.CNIC;
                        _db.Entity_Modified_Log.Add(eml);
                        _db.SaveChanges();


                        return Ok(candidate);

                    }
                    else
                    {
                        candidate = new HrProfile();
                        candidate.EmployeeName = consultantProfile.EmployeeName;
                        candidate.FatherName = consultantProfile.FatherName;
                        candidate.Domicile_Id = consultantProfile.Domicile_Id;
                        candidate.Status_Id = consultantProfile.Status_Id;
                        candidate.DateOfBirth = consultantProfile.DateOfBirth;
                        candidate.HealthFacility_Id = consultantProfile.HF_Id;
                        candidate.Designation_Id = consultantProfile.designationId;
                        candidate.DateOfRegularization = consultantProfile.DateOfRegularization;
                        candidate.SeniorityNo = consultantProfile.SeniorityNo;
                        candidate.PermanentAddress = consultantProfile.PermanentAddress;
                        candidate.MobileNo = consultantProfile.MobileNo;
                        candidate.EMaiL = consultantProfile.EMaiL;
                        _db.Entry(candidate).State = EntityState.Modified;
                        _db.SaveChanges();
                        Entity_Lifecycle elc = new Entity_Lifecycle();
                        elc.Created_Date = DateTime.UtcNow.AddHours(5);
                        elc.Created_By = consultantProfile.CNIC + " (added after migration)";
                        elc.Users_Id = consultantProfile.CNIC;
                        elc.IsActive = true;
                        elc.Entity_Id = 9;
                        _db.Entity_Lifecycle.Add(elc);
                        _db.SaveChanges();
                        candidate.EntityLifecycle_Id = elc.Id;
                        _db.SaveChanges();
                        return Ok(candidate);
                    }
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        [Route("GetVacancy/{designationId}")]
        public IHttpActionResult GetVacancy(int designationId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    //var hfIds = db.VPMasters.Where(x => x.Desg_Id == designationId && ((x.TotalSanctioned - x.TotalWorking) > 0 || (x.TotalSanctioned - x.TotalWorking) > 0)).Select(k => k.HF_Id).ToList();
                    //var hfs = db.HFListPs.Where(x => (hfIds.Contains(x.Id) || x.HFCategoryName.Equals("Tertiary")) && x.IsActive == true).OrderBy(k => k.FullName).ToList();
                    var vp = db.VpMProfileViews.Where(x => x.Desg_Id == designationId && (x.Vacant > 0 || x.OnPayScale > 0) && x.DistrictCode != "035002").ToList();
                    return Ok(vp);
                }
            }
            catch (Exception ex)
            {

                return BadRequest(ex.InnerException.InnerException.InnerException?.Message ?? ex.InnerException.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message);
            }
        }

        [Route("MobileAppVersion")]
        [HttpGet]
        public IHttpActionResult MobileAppVersion()
        {
            try
            {
                var result = _rootService.GetMobileAppVersion();
                return Ok(result);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [Route("GetProfile/{cnic}")]
        [HttpGet]
        public IHttpActionResult GetProfile(string cnic)
        {
            try
            {
                var result = _rootService.GetProfileExist(cnic);
                return Ok(result);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [Route("GetProfileExist/{id}/{cnic}/{mobileNo}/{email}")]
        [HttpGet]
        public IHttpActionResult GetProfileExist(int id, string cnic, string mobileNo, string email)
        {
            try
            {
                var result = _rootService.GetProfileExist(id, cnic, mobileNo, email);
                return Ok(result);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [Route("GetProfileExistDesignation/{cnic}")]
        [HttpGet]
        public IHttpActionResult GetProfileExistDesignation(string cnic)
        {
            try
            {
                var result = _rootService.GetProfileExistDesignation(cnic, new List<int?> { 1085, 1157, 2240 });
                return Ok(result);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [Route("GetPromotionApplyData/{cnic}")]
        [HttpGet]
        public IHttpActionResult GetPromotionApplyData(string cnic)
        {
            try
            {
                var result = _rootService.GetPromotionApplyData(cnic);
                return Ok(result);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [Route("GetPromotionApply")]
        [HttpPost]
        public IHttpActionResult GetPromotionApply(Filter filter)
        {
            try
            {
                var result = _rootService.GetPromotionApply(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [Route("GetProfileByCNIC/{cnic}")]
        [HttpGet]
        public IHttpActionResult GetProfileByCNIC(string cnic)
        {
            try
            {
                var result = _rootService.GetProfileByCNIC(cnic);
                return Ok(result);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [Route("GetCompleteProfileByCNIC/{cnic}")]
        [HttpGet]
        public IHttpActionResult GetCompleteProfileByCNIC(string cnic)
        {
            try
            {
                var result = _rootService.GetProfileByCNIC(cnic);
                return Ok(result);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [Route("GetProfiles/{imei}")]
        [HttpGet]
        public IHttpActionResult GetProfiles(long imei)
        {
            try
            {
                var result = _rootService.GetEmployeeProfiles(imei);
                return Ok(result);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetProfileApplicant/{cnic}/{id}")]
        public IHttpActionResult GetProfile(string cnic, int id)
        {
            try
            {
                var res = _rootService.GetProfileApplicant(cnic, id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("VerifyProfileForTransfer/{Id}/{resons_Id}")]
        public IHttpActionResult VerifyProfileForTransfer(int Id, int resons_Id)
        {
            try
            {
                return Ok(_rootService.VerifyProfileForTransfer(Id, resons_Id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        [Route("GetOrderDesignations/{hf_Id}/{hfmisCode}/{Designation_Id}")]
        public IHttpActionResult CheckVacancy(int hf_Id, string hfmisCode, int Designation_Id)
        {
            try
            {
                if (hf_Id == 0) return BadRequest();
                if (hfmisCode.Length != 19) return BadRequest();
                if (User == null) return Unauthorized();
                var response = _rootService.GetOrderDesignations(hf_Id, hfmisCode, Designation_Id, User.Identity.GetUserName(), User.Identity.GetUserId());
                return Ok(response);
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost]
        [Route("SaveServiceTemp")]
        public IHttpActionResult SaveServiceTemp([FromBody] ServiceTemp serviceTemp)
        {
            try
            {
                var res = _databaseService.SaveServiceTemp(serviceTemp, User.Identity.GetUserName(), User.Identity.GetUserId());
                if (res == null) return BadRequest("Invalid");
                return Ok(res);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetServiceTemp/{profileId}")]
        public IHttpActionResult GetServiceTemp(int profileId)
        {
            try
            {
                var res = _databaseService.GetServiceTemp(profileId, User.Identity.GetUserName(), User.Identity.GetUserId());
                if (res == null) return BadRequest("Invalid");
                return Ok(res);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpGet]
        [Route("RemoveServiceTemp/{Id}")]
        public IHttpActionResult RemoveServiceTemp(int Id)
        {
            try
            {
                var res = _databaseService.RemoveServiceTemp(Id, User.Identity.GetUserName(), User.Identity.GetUserId());
                return Ok(res);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }

        public class PromotionFilter : Paginator
        {
            public int Name { get; set; }
            public string CNIC { get; set; }
            public int DesignationId { get; set; }
            public string Query { get; set; }
        }





    }
}
