using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Hrmis.Models.DbModel;
using System.Data.Entity.Validation;
using Microsoft.AspNet.Identity;
using System.Web;
using Hrmis.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.IO;
using System.Net.Http;
using Hrmis.Models.CustomModels;
using Hrmis.Models.Dto;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace Hrmis.Controllers.HrmisApiControllers
{
    [Authorize]
    [RoutePrefix("api/ProfileDetailsViews")]
    public class ProfileDetailsViewsController : ApiController
    {
        private HR_System db = new HR_System();
        public ProfileDetailsViewsController()
        {
            db.Configuration.ProxyCreationEnabled = false;
        }

        [Route("GetProfileDV")]
        [HttpGet]
        public IHttpActionResult GetProfileDetailsViews()
        {
            return Ok(db.ProfileDetailsViews.Take(50));
        }

        [Route("GetProfileByCNICForPublic/{cnic}")]
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult GetProfileThumbViewsForPublic(string cnic)
        {
            cnic = cnic.Replace("-", string.Empty);
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                ProfileDetailsSeniorityView profileDetailsView = db.ProfileDetailsSeniorityViews.FirstOrDefault(x => x.CNIC.Equals(cnic));
                if (profileDetailsView != null)
                {
                    return Ok(profileDetailsView);
                }
                else
                {
                    return Ok(404);
                }
            }
            catch (DbEntityValidationException dbEx) { return BadRequest(GetDbExMessage(dbEx)); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [Route("GetProfileByCNIC/{cnic}")]
        [HttpGet]
        public IHttpActionResult GetProfileThumbViews(string cnic)
        {
            cnic = cnic.Replace("-", string.Empty);
            try
            {
                string userId = User.Identity.GetUserId();
                var usermanger = new ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                var user = usermanger.FindById(userId);
                string userHfmisCode = (user.hfmiscode != null ?
                        user.hfmiscode : user.TehsilID != null ?
                            user.TehsilID : user.DistrictID != null ?
                                user.DistrictID : user.DivisionID != null ?
                                    user.DivisionID : "0");
                db.Configuration.ProxyCreationEnabled = false;
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
            catch (DbEntityValidationException dbEx) { return BadRequest(GetDbExMessage(dbEx)); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [Route("CheckProfileDuplicateByCNIC/{cnic}")]
        [HttpGet]
        public IHttpActionResult CheckProfileDuplicateByCNIC(string cnic)
        {
            cnic = cnic.Replace("-", string.Empty);
            try
            {

                var profile = db.ProfileDetailsViews.FirstOrDefault(x => x.CNIC.Equals(cnic));
                if (profile == null) return Ok(new { result = true, Id = 0 });
                else return Ok(new { result = false, Id = profile.Id });
            }
            catch (DbEntityValidationException dbEx) { return BadRequest(GetDbExMessage(dbEx)); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [Route("GetESRByCNIC/{cnic}")]
        [HttpGet]
        public IHttpActionResult GetESRByCNIC(string cnic)
        {
            try { return Ok(db.ESRViews.Where(x => x.CNIC.Equals(cnic)).ToList()); }
            catch (DbEntityValidationException dbEx) { return BadRequest(GetDbExMessage(dbEx)); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [Route("GetProfileCV/{hfmisCode}/{CadreID}/{DesignationID}/{StatusID}/{currentPage}/{itemsPerPage}")]
        [HttpGet]
        public IHttpActionResult GetProfileThumbViewsCv(string hfmisCode, int CadreID, int DesignationID, int StatusID, int currentPage = 1, int itemsPerPage = 100)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                db.Database.CommandTimeout = 60 * 2;
                string hftypecode = "";
                string userHfmisCode = "";
                //var results = db.ProfileThumbViews.Where(x => x.HfmisCode.StartsWith(hfmisCode)).Select(x => new
                //{
                //    Id = x.Id,
                //    EmployeeName = x.EmployeeName,
                //    CNIC = x.CNIC,
                //    Designation = x.Designation_Name,
                //    Scale=x.CurrentGradeBPS

                //}).OrderBy(x => x.EmployeeName).AsQueryable();
                string userId = User.Identity.GetUserId();
                if (userId != null)
                {
                    using (var db = new HR_System())
                    {

                        var usermanger = new ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                        var user = usermanger.FindById(userId);
                        hftypecode = user.HfTypeCode;
                        userHfmisCode = user.hfmiscode;
                    }
                }

                int totalRecords = 0;
                if (userHfmisCode == "0" && hftypecode != null)
                {
                    var results = db.ProfileThumbViews.Where(x => x.HfmisCode.StartsWith(hfmisCode) && (x.HfmisCode.Substring(12, 3).Equals(hftypecode))).AsQueryable();
                    if (CadreID != 0) results = results.Where(x => x.Cadre_Id == CadreID).AsQueryable();
                    if (DesignationID != 0) results = results.Where(x => x.Designation_Id == DesignationID).AsQueryable();
                    if (StatusID != 0) results = results.Where(x => x.Status_Id == StatusID).AsQueryable();
                    results = results.OrderBy(x => x.EmployeeName).AsQueryable();

                    totalRecords = results.Count();
                    var profiles = results.Skip((currentPage - 1) * itemsPerPage).Take(itemsPerPage).ToList();
                    return Ok(new { profiles, totalRecords });
                }
                else
                {
                    var results = db.ProfileThumbViews.Where(x => x.HfmisCode.StartsWith(hfmisCode) && !string.IsNullOrEmpty(x.EmployeeName)).AsQueryable();
                    if (CadreID != 0) results = results.Where(x => x.Cadre_Id == CadreID).AsQueryable();
                    if (DesignationID != 0) results = results.Where(x => x.Designation_Id == DesignationID).AsQueryable();
                    if (StatusID != 0) results = results.Where(x => x.Status_Id == StatusID).AsQueryable();
                    results = results.OrderBy(x => x.EmployeeName).AsQueryable();

                    totalRecords = results.Count();
                    var profiles = results.Skip((currentPage - 1) * itemsPerPage).Take(itemsPerPage).ToList();
                    return Ok(new { profiles, totalRecords });
                }
                //var results = db.ProfileThumbViews.Where(x => x.HfmisCode.StartsWith(hfmisCode) ||  string.IsNullOrEmpty(x.HfmisCode) && !string.IsNullOrEmpty(x.EmployeeName) && !string.IsNullOrEmpty(x.CNIC) && x.Cadre_Id != null).AsQueryable();
                ////  var results = db.ProfileThumbViews.Where(x => x.HfmisCode.StartsWith(hfmisCode) && !string.IsNullOrEmpty(x.EmployeeName)).AsQueryable();
                // var results = db.ProfileDetailsViews.Where(x => x.HfmisCode.StartsWith(hfmisCode) && !string.IsNullOrEmpty(x.EmployeeName)).AsQueryable();

                //   if (CadreID != 0) results = results.Where(x => x.Cadre_Id == CadreID).AsQueryable(); //t 30-4-18
                //  if (DesignationID != 0) results = results.Where(x => x.Designation_Id == DesignationID).AsQueryable();

                //if (DesignationID != 0) results = results.Where(x => x.Designation_Id == DesignationID).AsQueryable();
                //results = results.OrderBy(x => x.EmployeeName).AsQueryable();

                //int totalRecords = results.Count();
                //var profiles = results.Skip((currentPage - 1) * itemsPerPage).Take(itemsPerPage).ToList();
                //return Ok(new { profiles, totalRecords });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("GetProfileSeniority/{hfmisCode}/{desgId}/{scaleId}/{currentPage}/{itemsPerPage}")]
        [HttpGet]
        public IHttpActionResult GetProfileSeniority(string hfmisCode, int desgId, int scaleId, int currentPage = 1, int itemsPerPage = 100)
        {
            try
            {
                var query = db.ProfileDetailsViewActives.Where(x => x.EmpMode_Name == "Regular" && x.StatusName == "Active" && (x.HfmisCode.StartsWith(hfmisCode) || x.WorkingHFMISCode.StartsWith(hfmisCode))).OrderByDescending(x => x.LengthOfService).AsQueryable();
                if (desgId > 0)
                {
                    query = query.Where(x => x.Designation_Id == desgId);
                }
                else if (scaleId > 0)
                {
                    query = query.Where(x => x.CurrentGradeBPS == scaleId);
                }
                return Ok(new { profiles = query.Skip((currentPage - 1) * itemsPerPage).Take(itemsPerPage).ToList(), totalRecords = query.Count() });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [AllowAnonymous]
        [Route("DownloadProfileSeniority/{hfmisCode}/{desgId}/{scaleId}")]
        [HttpGet]
        public IHttpActionResult DownloadProfileSeniority(string hfmisCode, int desgId, int scaleId)
        {
            try
            {
                var query = db.ProfileDetailsViewActives.Where(x => x.EmpMode_Name == "Regular" && x.StatusName == "Active" && (x.HfmisCode.StartsWith(hfmisCode) || x.WorkingHFMISCode.StartsWith(hfmisCode))).OrderByDescending(x => x.LengthOfService).AsQueryable();
                if (desgId > 0)
                {
                    query = query.Where(x => x.Designation_Id == desgId);
                }
                else if (scaleId > 0)
                {
                    query = query.Where(x => x.CurrentGradeBPS == scaleId);
                }
                var list = query.Select(x => new ProfileActiveSeniority() { EmployeeName = x.EmployeeName, CNIC = x.CNIC, BirthDate = x.DateOfBirth, DateOfFirstAppointment = x.DateOfFirstAppointment, HealthFacility = x.HealthFacilityFullName, LengthOfService = x.LengthOfService, WorkingHealthFacility = x.WorkingHealthFacilityFullName, Designation = x.Designation_Name, WorkingDesignation = x.WDesignation_Name }).ToList();
                var gv = new GridView();
                gv.DataSource = list;
                gv.DataBind();
                HttpContext.Current.Response.ClearContent();
                HttpContext.Current.Response.Buffer = true;
                HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=SeniorityList.xls");
                HttpContext.Current.Response.ContentType = "application/ms-excel";
                HttpContext.Current.Response.Charset = "";
                StringWriter objStringWriter = new StringWriter();
                HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                gv.RenderControl(objHtmlTextWriter);
                HttpContext.Current.Response.Output.Write(objStringWriter.ToString());
                HttpContext.Current.Response.End();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Route("GetProfilePendingTransfer/{hfmisCode}/{currentPage}/{itemsPerPage}")]
        [HttpGet]
        public IHttpActionResult GetProfilePendingTransfer(string hfmisCode, int currentPage = 1, int itemsPerPage = 100)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                db.Database.CommandTimeout = 60 * 2;
                var results = db.HrProfiles.Where(x => x.HfmisCode.StartsWith(hfmisCode) && x.Status_Id == 30).Select(x => new
                {
                    Id = x.Id,
                    EmployeeName = x.EmployeeName,
                    FatherName = x.FatherName,
                    DateOfBirth = x.DateOfBirth,
                    CNIC = x.CNIC,
                }).OrderBy(x => x.EmployeeName).AsQueryable();
                int totalRecords = results.Count();
                var profiles = results.Skip((currentPage - 1) * itemsPerPage).Take(itemsPerPage).ToList();
                return Ok(new { profiles, totalRecords });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("SearchProfile/{currentPage}/{itemsPerPage}/{hfmisCode}")]
        [HttpPost]
        public IHttpActionResult SearchProfile([FromBody] ProfileQuery query, string hfmisCode, int currentPage = 1, int itemsPerPage = 100)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                db.Database.CommandTimeout = 60 * 2;

                var results = db.ProfileThumbViews.Where(x => x.HfmisCode.StartsWith(hfmisCode)).AsQueryable();
                results = results.Where(x => x.EmployeeName.ToLower().Contains(query.Query.ToLower()) || x.CNIC.Equals(query.Query)).OrderBy(x => x.EmployeeName).AsQueryable();
                //  var results = db.ProfileThumbViews.Where(x => x.EmployeeName.ToLower().Contains(query.Query.ToLower()) || x.CNIC.Equals(query.Query)).OrderBy(x => x.EmployeeName).AsQueryable();
                int totalRecords = results.Count();
                var profiles = results.Skip((currentPage - 1) * itemsPerPage).Take(itemsPerPage).ToList();
                return Ok(new { profiles, totalRecords });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("ProfilesInfo/{hfmisCode}")]
        [HttpGet]
        public IHttpActionResult GetProfilesInfo(string hfmisCode)
        {
            try
            {
                int hfId = db.HealthFacilities.FirstOrDefault(x => x.HfmisCode.Equals(hfmisCode)).Id;
                List<HFEMpProfile> data = new List<HFEMpProfile>();
                int c = 1;
                //foreach (HrProfileStatu status in db.HrProfileStatus)
                //{
                //    var profiles = db.ProfileDetailsViews.Where(x => (x.HfmisCode.StartsWith(hfmisCode) || x.HealthFacility_Id == hfId) && x.Status_Id == status.Id).OrderByDescending(x => x.CurrentGradeBPS).ToList();
                //    if(profiles.Count > 0)
                //    {
                //        HFEMpProfile item = new HFEMpProfile
                //        {
                //            Id = c++,
                //            Name = status.Name,
                //            Count = profiles.Count,
                //            Profiles = profiles
                //        };
                //        data.Add(item);
                //    }
                //}
                data = db.ProfileDetailsViews.Where(x => (x.HfmisCode.StartsWith(hfmisCode) || x.HealthFacility_Id == hfId)).OrderByDescending(x => x.CurrentGradeBPS).ThenBy(x => x.Designation_Name)
                    .ToList().GroupBy(x => x.StatusName).Select(x => new HFEMpProfile() { Id = c++, Name = x.Key, Count = x.Count(), Show = false, Profiles = x.ToList() }).ToList();

                return Ok(data);
            }
            catch (DbEntityValidationException ex)
            {
                return BadRequest(GetDbExMessage(ex));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }

        }

        private int countProfiles(int startIndex, int TotalToTake)
        {
            return db.ProfileThumbViews.Count();
        }

        [HttpGet]
        [Route("GetProfileDetailsView/{id}")]
        // GET: api/ProfileDetailsViews/5
        [ResponseType(typeof(ProfileDetailsView))]
        public async Task<IHttpActionResult> GetProfileDetailsView(int id)
        {
            ProfileDetailsView profileDetailsView = null;
            try
            {
                profileDetailsView = await db.ProfileDetailsViews.FirstOrDefaultAsync(x => x.Id == id);
                if (profileDetailsView == null)
                {
                    return NotFound();
                }
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }


            return Ok(profileDetailsView);
        }


        [HttpPost]
        [Route("AddToPool/{id}")]
        public async Task<IHttpActionResult> AddToPool(int id)
        {
            using (var transc = db.Database.BeginTransaction())
            {
                try
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var profile = db.HrProfiles.FirstOrDefault(x => x.Id == id);
                    if (profile == null) return BadRequest("Not Found");

                    profile.AddToEmployeePool = true;

                    if (profile.EntityLifecycle_Id == null)
                    {
                        Entity_Lifecycle elc = new Entity_Lifecycle();
                        elc.Created_Date = DateTime.UtcNow.AddHours(5);
                        elc.Created_By = User.Identity.GetUserName() + " (added after migration)";
                        elc.Users_Id = User.Identity.GetUserId();
                        elc.IsActive = true;
                        elc.Entity_Id = 9;
                        db.Entity_Lifecycle.Add(elc);
                        await db.SaveChangesAsync();
                        profile.EntityLifecycle_Id = elc.Id;
                    }

                    Entity_Modified_Log eml = new Entity_Modified_Log();
                    eml.Modified_By = User.Identity.GetUserId();
                    eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                    eml.Entity_Lifecycle_Id = (long)profile.EntityLifecycle_Id;
                    eml.Description = "Profile added to Pool By " + User.Identity.GetUserName();
                    db.Entity_Modified_Log.Add(eml);

                    db.Entry(profile).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    transc.Commit();
                    return Ok(profile);
                }
                catch (DbUpdateConcurrencyException dbEx)
                {
                    transc.Rollback();
                    return BadRequest(dbEx.Message);

                }
                catch (Exception ex)
                {
                    transc.Rollback();
                    return BadRequest(ex.Message);
                }
            }
        }

        [HttpPost]
        [Route("RemoveFromPool/{id}")]
        public async Task<IHttpActionResult> RemoveFromPool(int id)
        {
            using (var transc = db.Database.BeginTransaction())
            {
                try
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var profile = db.HrProfiles.FirstOrDefault(x => x.Id == id);
                    if (profile == null) return BadRequest("Not Found");

                    profile.AddToEmployeePool = false;

                    if (profile.EntityLifecycle_Id == null)
                    {
                        Entity_Lifecycle elc = new Entity_Lifecycle();
                        elc.Created_Date = DateTime.UtcNow.AddHours(5);
                        elc.Created_By = User.Identity.GetUserName() + " (added after migration)";
                        elc.Users_Id = User.Identity.GetUserId();
                        elc.IsActive = true;
                        elc.IsActive = true;
                        elc.Entity_Id = 9;
                        db.Entity_Lifecycle.Add(elc);
                        await db.SaveChangesAsync();
                        profile.EntityLifecycle_Id = elc.Id;
                    }

                    Entity_Modified_Log eml = new Entity_Modified_Log();
                    eml.Modified_By = User.Identity.GetUserId();
                    eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                    eml.Entity_Lifecycle_Id = (long)profile.EntityLifecycle_Id;
                    eml.Description = "Profile removed from Pool By " + User.Identity.GetUserName();
                    db.Entity_Modified_Log.Add(eml);

                    db.Entry(profile).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    transc.Commit();
                    return Ok(profile);
                }
                catch (DbUpdateConcurrencyException dbEx)
                {
                    transc.Rollback();
                    return BadRequest(dbEx.Message);
                }
                catch (Exception ex)
                {
                    transc.Rollback();
                    return BadRequest(ex.Message);
                }
            }
        }

        [HttpPost]
        [Route("PostProfile")]
        public async Task<IHttpActionResult> PostProfile(HrProfile hrProfile)
        {
            hrProfile.HealthFacility = null;
            hrProfile.CNIC = hrProfile.CNIC.Replace("-", "");
            hrProfile.MobileNo = hrProfile.MobileNo.Replace("-", "");
         
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (ProfileDetailsViewExists(hrProfile.Id))
                {
                    if (hrProfile.EntityLifecycle_Id != null)
                    {
                        Entity_Modified_Log eml = new Entity_Modified_Log();
                        if (User.Identity.GetUserId() == null)
                        {
                            eml.Modified_By = Convert.ToString(hrProfile.Id);
                        }
                        else
                        {
                            eml.Modified_By = User.Identity.GetUserId();
                        }
                        eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                        eml.Entity_Lifecycle_Id = (long)hrProfile.EntityLifecycle_Id;
                        eml.Description = "Profile Updated By " + User.Identity.GetUserName();
                        db.Entity_Modified_Log.Add(eml);
                        db.SaveChanges();
                        db.Entity_Lifecycle.FirstOrDefault(x => x.Id == hrProfile.EntityLifecycle_Id).Entity_Modified_Log.Add(eml);
                    }
                    else
                    {
                        Entity_Lifecycle eld = new Entity_Lifecycle();
                        eld.Created_Date = DateTime.UtcNow.AddHours(5);
                        if (User.Identity.GetUserName() == null || User.Identity.GetUserId() == null)
                        {
                            eld.Created_By = "OnlineApplicationForm";
                            eld.Users_Id = "OnlineApplicationForm";
                        }
                        else
                        {
                            eld.Created_By = User.Identity.GetUserName();
                            eld.Users_Id = User.Identity.GetUserId();
                        }
                        eld.IsActive = true;
                        eld.Entity_Id = 9;
                        db.Entity_Lifecycle.Add(eld);
                        await db.SaveChangesAsync();
                        hrProfile.EntityLifecycle_Id = eld.Id;
                        Entity_Modified_Log eml = new Entity_Modified_Log();
                        eml.Modified_By = eld.Users_Id;
                        eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                        eml.Entity_Lifecycle_Id = (long)hrProfile.EntityLifecycle_Id;
                        eml.Description = "Profile Updated By " + User.Identity.GetUserName();
                        db.Entity_Modified_Log.Add(eml);
                        db.SaveChanges();
                        db.Entity_Lifecycle.FirstOrDefault(x => x.Id == hrProfile.EntityLifecycle_Id).Entity_Modified_Log.Add(eml);
                    }
                    db.Entry(hrProfile).State = EntityState.Modified;
                }
                else
                {
                    Entity_Lifecycle eld = new Entity_Lifecycle();
                    eld.Created_Date = DateTime.UtcNow.AddHours(5);
                    if (User.Identity.GetUserName() == null || User.Identity.GetUserId() == null)
                    {
                        eld.Created_By = "OnlineApplicationForm";
                        eld.Users_Id = "OnlineApplicationForm";
                    }
                    else
                    {
                        eld.Created_By = User.Identity.GetUserName();
                        eld.Users_Id = User.Identity.GetUserId();
                    }
                    eld.IsActive = true;
                    eld.Entity_Id = 9;

                    db.Entity_Lifecycle.Add(eld);
                    hrProfile.EntityLifecycle_Id = eld.Id;
                    db.HrProfiles.Add(hrProfile);
                }
                await db.SaveChangesAsync();
                return Ok(new { Id = hrProfile.Id, CNIC = hrProfile.CNIC });
            }
            catch (DbEntityValidationException dbEx)
            {

                return BadRequest(GetDbExMessage(dbEx));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        [HttpPost]
        [Route("PostProfilePublic")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> PostProfilePublic(HrProfile hrProfile)
        {
            hrProfile.HealthFacility = null;
            hrProfile.CNIC = hrProfile.CNIC.Replace("-", "");
            hrProfile.MobileNo = hrProfile.MobileNo.Replace("-", "");
            hrProfile.Status_Id = 32;
            if (hrProfile.Status_Id == 0)
            {
                hrProfile.Status_Id = 16;
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (ProfileDetailsViewExists(hrProfile.Id))
                {
                    if (hrProfile.EntityLifecycle_Id != null)
                    {
                        Entity_Modified_Log eml = new Entity_Modified_Log();
                        if (User.Identity.GetUserId() == null)
                        {
                            eml.Modified_By = Convert.ToString(hrProfile.Id);
                        }
                        else
                        {
                            eml.Modified_By = User.Identity.GetUserId();
                        }
                        eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                        eml.Entity_Lifecycle_Id = (long)hrProfile.EntityLifecycle_Id;
                        eml.Description = "Profile Updated By " + User.Identity.GetUserName();
                        db.Entity_Modified_Log.Add(eml);
                        db.SaveChanges();
                        db.Entity_Lifecycle.FirstOrDefault(x => x.Id == hrProfile.EntityLifecycle_Id).Entity_Modified_Log.Add(eml);
                    }
                    db.Entry(hrProfile).State = EntityState.Modified;
                }
                else
                {
                    Entity_Lifecycle eld = new Entity_Lifecycle();
                    eld.Created_Date = DateTime.UtcNow.AddHours(5);
                    if (User.Identity.GetUserName() == null || User.Identity.GetUserId() == null)
                    {
                        eld.Created_By = "OnlineApplicationForm";
                        eld.Users_Id = "OnlineApplicationForm";
                    }
                    else
                    {
                        eld.Created_By = User.Identity.GetUserName();
                        eld.Users_Id = User.Identity.GetUserId();
                    }
                    eld.IsActive = true;
                    eld.Entity_Id = 9;

                    db.Entity_Lifecycle.Add(eld);
                    hrProfile.EntityLifecycle_Id = eld.Id;
                    db.HrProfiles.Add(hrProfile);
                }

                var newObj = new SeniorityDetail
                {
                    DateOfAppointment = hrProfile.DateOfFirstAppointment,
                    DateOfBirth = hrProfile.DateOfBirth,
                    Designation_Id = hrProfile.Designation_Id,
                    FatherName = hrProfile.FatherName,
                    Name = hrProfile.EmployeeName,
                    Profile_Id = hrProfile.Id,
                    SeniorityMaster_Id = null,
                    SeniorityNo = hrProfile.SeniorityNo,
                    Status = "Saved",
                };

                db.SeniorityDetails.Add(newObj);

                await db.SaveChangesAsync();
                return Ok(new { Id = hrProfile.Id, CNIC = hrProfile.CNIC });
            }
            catch (DbEntityValidationException dbEx)
            {

                return BadRequest(GetDbExMessage(dbEx));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }



        [Route("SearchProfile/{cnic}/{code}")]
        [HttpGet]
        public IHttpActionResult SearchProfile(string cnic, string code)
        {
            cnic = cnic.Replace("-", string.Empty);
            try
            {
                ProfileDetailsView profile = db.ProfileDetailsViews.FirstOrDefault(x => x.CNIC.Equals(cnic));
                if (profile != null)
                {
                    bool inRangePrfile = false;
                    if (!string.IsNullOrEmpty(profile.HfmisCode))
                    {
                        if (profile.HfmisCode.StartsWith(code) || profile.AddToEmployeePool == true)
                        {
                            inRangePrfile = true;
                        }
                        else
                        {
                            inRangePrfile = false;
                        }
                    }
                    else
                    {
                        inRangePrfile = true;
                    }
                    return Ok(new
                    {
                        Id = inRangePrfile ? profile.Id : 0,
                        FullName = inRangePrfile ? profile.EmployeeName + " (" + profile.WDesignation_Name + " - " + profile.District + ")" : "Profile does not exist in your " + (code.Length == 3 ? "division" : code.Length == 6 ? "district" : code.Length == 9 ? "tehsil" : "region") + " - Please contact HISDU."
                    });
                }
                else
                {
                    return Ok(false);
                }
            }
            catch (DbEntityValidationException dbx) { return BadRequest(GetDbExMessage(dbx)); }
        }

        [Route("GetStatus")]
        [HttpGet]
        public async Task<IHttpActionResult> GetStatus()
        {

            try
            {

                return Ok(await db.HrProfileStatus.OrderBy(x => x.Name).ToListAsync());
            }
            catch (DbEntityValidationException dbx) { return BadRequest(GetDbExMessage(dbx)); }
        }
        [Route("UploadProfilePhoto/{profileId}")]
        public async Task<IHttpActionResult> UploadProfilePhoto(int profileId)
        {
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                string RootPath = HttpContext.Current.Server.MapPath("~/") + @"Uploads\ProfilePhotos\";
                var dirPath = RootPath;

                var provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);

                CreateDirectoryIfNotExists(dirPath);
                string filename = "";
                if (profileId == 0)
                {
                    foreach (var file in provider.Contents)
                    {
                        //filename = merit.Id + "_OfferLetter." + file.Headers.ContentDisposition.FileName.Trim('\"').Split('.')[1];
                        filename = Guid.NewGuid().ToString() + "_23.jpg";
                        var buffer = await file.ReadAsByteArrayAsync();
                        var size = ((buffer.Length) / (1024)) / (1024);
                        var ext = Path.GetExtension(filename.Replace("\"", string.Empty));
                        List<string> validExtensions = new List<string>() { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                        if (!validExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase) || size > 5)
                        {
                            throw new Exception("Unable to Upload. File Size must be less than 5 MB and File Format must be " + string.Join(",", validExtensions));
                        }
                        using (FileStream fsOut = File.OpenWrite(RootPath + @"\" + filename))
                        {
                            fsOut.Write(buffer, 0, buffer.Length);
                        }
                    }
                }
                else
                {
                    HrProfile hrProfile = db.HrProfiles.FirstOrDefault(x => x.Id == profileId);

                    foreach (var file in provider.Contents)
                    {
                        //filename = merit.Id + "_OfferLetter." + file.Headers.ContentDisposition.FileName.Trim('\"').Split('.')[1];
                        filename = hrProfile.CNIC + "_23.jpg";
                        var buffer = await file.ReadAsByteArrayAsync();
                        var size = ((buffer.Length) / (1024)) / (1024);
                        var ext = Path.GetExtension(filename.Replace("\"", string.Empty));
                        List<string> validExtensions = new List<string>() { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                        if (!validExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase) || size > 5)
                        {
                            throw new Exception("Unable to Upload. File Size must be less than 5 MB and File Format must be " + string.Join(",", validExtensions));
                        }
                        if (File.Exists(HttpContext.Current.Server.MapPath(@"~\Uploads\ProfilePhotos\" + hrProfile.CNIC + "_23.jpg")) && buffer.Length > 0)
                        {
                            File.Delete(HttpContext.Current.Server.MapPath(@"~\Uploads\ProfilePhotos\" + hrProfile.CNIC + "_23.jpg"));
                        }
                        using (FileStream fsOut = File.OpenWrite(RootPath + @"\" + filename))
                        {
                            fsOut.Write(buffer, 0, buffer.Length);
                        }
                        hrProfile.ProfilePhoto = filename;
                        await db.SaveChangesAsync();
                    }
                }

                return Ok(new { result = true, src = @"/Uploads/ProfilePhotos/" + filename });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("ConfirmJoining/{id}")]
        public async Task<IHttpActionResult> ConfirmJoining(int id)
        {
            using (var transc = db.Database.BeginTransaction())
            {
                try
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var profile = db.HrProfiles.FirstOrDefault(x => x.Id == id);
                    if (profile == null) return BadRequest("Not Found");

                    profile.Status_Id = 2;

                    if (profile.EntityLifecycle_Id == null)
                    {
                        Entity_Lifecycle elc = new Entity_Lifecycle();
                        elc.Created_Date = DateTime.UtcNow.AddHours(5);
                        elc.Created_By = User.Identity.GetUserName() + " (added after migration)";
                        elc.Users_Id = User.Identity.GetUserId();
                        elc.IsActive = true;
                        elc.Entity_Id = 9;
                        db.Entity_Lifecycle.Add(elc);
                        await db.SaveChangesAsync();
                        profile.EntityLifecycle_Id = elc.Id;
                    }

                    Entity_Modified_Log eml = new Entity_Modified_Log();
                    eml.Modified_By = User.Identity.GetUserId();
                    eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                    eml.Entity_Lifecycle_Id = (long)profile.EntityLifecycle_Id;
                    eml.Description = "Profile confirmed joining to HF by " + User.Identity.GetUserName();
                    db.Entity_Modified_Log.Add(eml);
                    await db.SaveChangesAsync();
                    transc.Commit();
                    return Ok(true);
                }
                catch (DbUpdateConcurrencyException dbEx)
                {
                    transc.Rollback();
                    return BadRequest(dbEx.Message);

                }
                catch (Exception ex)
                {
                    transc.Rollback();
                    return BadRequest(ex.Message);
                }
            }
        }

        [HttpGet]
        [Route("CheckMoCNIC/{cnic}")]
        public IHttpActionResult CheckMoCNIC(string cnic)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var profile = db.HrProfiles.FirstOrDefault(x => x.CNIC.Equals(cnic));

                if (profile == null) return BadRequest("Not Found");

                if (profile.Designation_Id == 802 || profile.WDesignation_Id == 802)
                {
                    string hfTypeCode = profile.HfmisCode.Substring(12, 3);
                    if (hfTypeCode.Equals("011") || hfTypeCode.Equals("012"))
                    {
                        return Ok(true);
                    }
                    else
                    {
                        return Ok("Not Eligible Health Facility");
                    }
                }
                else
                {
                    return Ok("Not Eligible Designation");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private void CreateDirectoryIfNotExists(string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
        }

        #region MSS PROFILES
        [AllowAnonymous]
        [Route("GetMSSProfiles")]
        [HttpPost]
        public IHttpActionResult Profiles()
        {
            //try
            //{
            //    return Ok(db.ProfileThumbViews.Where(x => x.Cadre_Id == 2 || x.Cadre_Id == 12 || x.Cadre_Id == 3).ToList());
            //}
            //catch (DbEntityValidationException dbEx)
            //{
            //    return BadRequest(GetDbExMessage(dbEx));
            //}
            //catch (Exception ex)
            //{
            //    return BadRequest(ex.Message);
            //}
            var formData = HttpContext.Current.Request.Params;
            var length = Convert.ToInt32(formData["length"] + "");
            var draw = formData["draw"] + "";
            var start = Convert.ToInt32(formData["start"] + "");
            var orderIndex = Convert.ToInt32(formData["order[0][column]"] + "");
            var orderWise = formData["order[0][dir]"] + "";
            var search = formData["search[value]"] + "";
            var model = MakeGridList(length, start, orderIndex, orderWise, search);
            return Json(new
            {
                draw = draw,
                recordsFiltered = model.Rows,
                recordsTotal = model.Rows,
                data = model.Grid
            });
        }
        public DataTableModel<ProfileThumbView> MakeGridList(int length, int start, int orderIndex, string orderWise, string search)
        {
            var model = new DataTableModel<ProfileThumbView>();


            var list = db.ProfileThumbViews.Where(x => !x.EmployeeName.Equals("")).AsQueryable();
            switch (orderIndex)
            {
                case 0:
                    list = orderWise == "asc" ? list.OrderBy(p => p.Id) : list.OrderByDescending(p => p.Id);
                    break;
                case 1:

                    list = orderWise == "asc" ? list.OrderBy(p => p.EmployeeName) : list.OrderByDescending(p => p.EmployeeName);
                    break;
                case 2:
                    list = orderWise == "asc" ? list.OrderBy(p => p.FatherName) : list.OrderByDescending(p => p.FatherName);
                    break;
                case 3:
                    list = orderWise == "asc" ? list.OrderBy(p => p.CNIC) : list.OrderByDescending(p => p.CNIC);
                    break;
                case 4:
                    list = orderWise == "asc" ? list.OrderBy(p => p.Designation_Name) : list.OrderByDescending(p => p.Designation_Name);
                    break;
                default:
                    list = list;
                    break;
            }
            if (!string.IsNullOrEmpty(search))
            {
                list = list.Where(x => x.EmployeeName.ToLower().Contains(search.ToLower())
                || x.CNIC.ToLower().Contains(search.ToLower())
                || x.Designation_Name.ToString().ToLower().Contains(search.ToLower())
                || x.Id.ToString().ToLower().Contains(search.ToLower())
                );
            }

            var totalCont = list.Count();
            var fList = list.Skip(start).Take(length).ToList();

            var tempList = fList.ToList();

            model.Grid = tempList;
            model.Rows = totalCont;
            return model;
        }

        [AllowAnonymous]
        [Route("Profiles/{cnic}")]
        [HttpGet]
        public IHttpActionResult Profile(string cnic)
        {
            try
            {
                return Ok(db.ProfileDetailsViews.FirstOrDefault(x => x.CNIC.Equals(cnic)));
            }
            catch (DbEntityValidationException dbEx)
            {
                return BadRequest(GetDbExMessage(dbEx));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [AllowAnonymous]
        [Route("ProfileById/{id}")]
        [HttpGet]
        public IHttpActionResult ProfileById(int id)
        {
            try
            {
                return Ok(db.ProfileDetailsViews.FirstOrDefault(x => x.Id == id));
            }
            catch (DbEntityValidationException dbEx)
            {
                return BadRequest(GetDbExMessage(dbEx));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [AllowAnonymous]
        [Route("ProfileExists/{cnic}")]
        [HttpGet]
        public IHttpActionResult ProfileExists(string cnic)
        {
            try
            {
                int profileId = 0;
                var profile = db.ProfileDetailsViews.FirstOrDefault(x => x.CNIC.Equals(cnic));
                if (profile != null)
                {
                    profileId = profile.Id;
                }
                return Ok(new { profileId });
            }
            catch (DbEntityValidationException dbEx)
            {
                return BadRequest(GetDbExMessage(dbEx));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("GetHealthFacilityD/{code}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetHealthFacilityDetails(string code = "")
        {
            try
            {
                return Ok(await db.HFDetails.Where(x => x.HFMISCode.StartsWith(code) && x.HFTypeCode.Equals("011") && x.HFTypeCode.Equals("012") && x.IsActive == true).Select(x => new { x.Id, x.FullName, x.HFMISCode, x.DivisionName, x.DistrictName, x.TehsilName }).ToListAsync());
            }
            catch (DbEntityValidationException dbx)
            {
                return BadRequest(GetDbExMessage(dbx));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        private string GetDbExMessage(DbEntityValidationException dbx)
        {
            return dbx.EntityValidationErrors.SelectMany(validationErrors => validationErrors.ValidationErrors)
                .Aggregate("",
                    (current, validationError) =>
                        current +
                        $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProfileDetailsViewExists(int id)
        {
            return db.ProfileDetailsViews.Count(e => e.Id == id) > 0;
        }
    }
    public class BRpt
    {
        public string DistrictName { get; set; }
        public int Total { get; set; }
        public int? NoBeds { get; set; }
    }

    public class DataTableModel<T> where T : class
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<T> Grid { get; set; }
        public int Rows { get; set; }
    }
}