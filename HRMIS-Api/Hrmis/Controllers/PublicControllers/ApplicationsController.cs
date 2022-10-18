using Hrmis.Models.Common;
using Hrmis.Models.DbModel.FCDb;
using Hrmis.Models.Dto;
using Hrmis.Models.ImageProcessor;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Hrmis.Controllers.PublicControllers
{
    [Authorize]
    [RoutePrefix("api/ApplicationsZ")]
    public class ApplicationsController : ApiController
    {
        private HR_SystemFC db = new HR_SystemFC();
        public ApplicationsController()
        {
            db.Configuration.ProxyCreationEnabled = false;
        }
        [Route("AddProfile")]
        [HttpPost]
        public IHttpActionResult AddProfile(HrProfile profile)
        {
            try
            {
                profile.HealthFacility = null;
                profile.CNIC = profile.CNIC.Replace("-", "");
                if (profile.MobileNo != null)
                {
                    profile.MobileNo = profile.MobileNo.Replace("-", "");
                }

                bool profileExisit = db.ProfileDetailsViews.Count(e => e.Id == profile.Id) > 0;
                if (profileExisit)
                {
                    HrProfile tempProfile = db.HrProfiles.FirstOrDefault(x => x.Id == profile.Id);
                    tempProfile.EmployeeName = profile.EmployeeName;
                    tempProfile.FatherName = profile.FatherName;
                    tempProfile.CNIC = profile.CNIC;
                    tempProfile.Gender = profile.Gender;
                    tempProfile.WDesignation_Id = profile.WDesignation_Id;
                    tempProfile.HealthFacility_Id = profile.HealthFacility_Id;
                    tempProfile.HfmisCode = profile.HfmisCode;
                    tempProfile.MobileNo = profile.MobileNo;
                    tempProfile.EMaiL = profile.EMaiL;
                    db.Entry(tempProfile).State = EntityState.Modified;
                    db.SaveChanges();
                    return Ok(new { result = true, cnic = profile.CNIC });
                }
                else
                {
                    Entity_Lifecycle eld = new Entity_Lifecycle();
                    eld.Created_Date = DateTime.UtcNow.AddHours(5);
                    eld.Created_By = User.Identity.GetUserName();
                    eld.Users_Id = User.Identity.GetUserId();
                    eld.IsActive = true;
                    eld.Entity_Id = 9;

                    db.Entity_Lifecycle.Add(eld);
                    profile.EntityLifecycle_Id = eld.Id;
                    db.HrProfiles.Add(profile);
                    db.SaveChanges();
                    return Ok(new { result = true, cnic = profile.CNIC });
                }

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }
        [Route("SubmitApplication")]
        [HttpPost]
        public IHttpActionResult SubmitApplication(ApplicationsListModel model)
        {
            using (var transc = db.Database.BeginTransaction())
            {
                try
                {
                    //New
                    if (model.master.Id == 0)
                    {
                        #region If New
                        List<SMS> smsss = new List<SMS>();

                        int trackingNumber = 0;
                        Application application = model.master;
                        ApplicationPersonAppeared applicationPersonAppeared = model.personAppeared;
                        Entity_Lifecycle eld = new Entity_Lifecycle();
                        eld.Created_Date = DateTime.UtcNow.AddHours(5);
                        eld.Created_By = User.Identity.GetUserName();
                        eld.Users_Id = User.Identity.GetUserId();
                        eld.IsActive = true;
                        eld.Entity_Id = 23;
                        db.Entity_Lifecycle.Add(eld);
                        db.SaveChanges();
                        application.EntityLifecycle_Id = eld.Id;

                        if (application.IsPersonAppeared == false)
                        {
                            db.ApplicationPersonAppeareds.Add(applicationPersonAppeared);
                            db.SaveChanges();
                            application.PersonAppeared = applicationPersonAppeared.Id.ToString();
                        }
                        if (application.ApplicationType_Id == 1)
                        {
                            ApplicationLeave appLeave = new ApplicationLeave();
                            appLeave.Id = model.detail.Id;
                            appLeave.CNIC = model.detail.CNIC;
                            if (model.detail.FromDate.Value.Hour == 19)
                            {
                                appLeave.FromDate = model.detail.FromDate.Value.AddHours(5);

                            }
                            else
                            {
                                appLeave.FromDate = model.detail.FromDate;

                            }
                            if (model.detail.ToDate.Value.Hour == 19)
                            {
                                appLeave.ToDate = model.detail.ToDate.Value.AddHours(5);

                            }
                            else
                            {
                                appLeave.ToDate = model.detail.ToDate;

                            }
                            appLeave.TotalDays = model.detail.TotalDays;
                            appLeave.LeaveType_Id = model.detail.LeaveType_Id;
                            appLeave.Remarks = model.detail.Remarks;
                            db.ApplicationLeaves.Add(appLeave);
                            db.SaveChanges();
                            application.Application_Id = appLeave.Id;
                            application.Status_Id = 1;
                            db.Applications.Add(application);
                        }
                        else if (application.ApplicationType_Id == 2)
                        {

                            ApplicationTransfer appTransfer = new ApplicationTransfer();
                            appTransfer.Id = model.detail.Id;
                            appTransfer.CNIC = model.detail.CNIC;
                            appTransfer.FromHF_Id = model.detail.FromHF_Id;
                            appTransfer.FromDept_Id = model.detail.FromDept_Id;
                            appTransfer.ToHF_Id = model.detail.ToHF_Id;
                            appTransfer.ToDept_Id = model.detail.ToDept_Id;
                            appTransfer.Remarks = model.detail.Remarks;
                            db.ApplicationTransfers.Add(appTransfer);
                            db.SaveChanges();
                            application.Application_Id = appTransfer.Id;
                            application.TrackingNumber = application.Id + 901;
                            application.Status_Id = 1;
                            db.Applications.Add(application);
                        }
                        else if (application.ApplicationType_Id == 3)
                        {
                            ApplicationPromotion appPromotion = new ApplicationPromotion();
                            appPromotion.Id = model.detail.Id;
                            appPromotion.CNIC = model.detail.CNIC;
                            appPromotion.CurrentScale = model.detail.CurrentScale;
                            appPromotion.Remarks = model.detail.Remarks;
                            db.ApplicationPromotions.Add(appPromotion);
                            db.SaveChanges();
                            application.Application_Id = appPromotion.Id;
                            application.TrackingNumber = application.Id + 901;
                            application.Status_Id = 1;
                            db.Applications.Add(application);
                        }
                        else if (application.ApplicationType_Id == 4)
                        {
                            ApplicationRetirement appRetirement = new ApplicationRetirement();
                            appRetirement.Id = model.detail.Id;
                            appRetirement.CNIC = model.detail.CNIC;
                            appRetirement.RetirementType_Id = model.detail.RetirementType_Id;
                            appRetirement.Remarks = model.detail.Remarks;
                            db.ApplicationRetirements.Add(appRetirement);
                            db.SaveChanges();
                            application.Application_Id = appRetirement.Id;
                            application.TrackingNumber = application.Id + 901;
                            application.Status_Id = 1;
                            db.Applications.Add(application);
                        }
                        else if (application.ApplicationType_Id == 5)
                        {
                            ApplicationRecruitment appRecruitment = new ApplicationRecruitment();
                            appRecruitment.Id = model.detail.Id;
                            appRecruitment.CNIC = model.detail.CNIC;
                            appRecruitment.Remarks = model.detail.Remarks;
                            db.ApplicationRecruitments.Add(appRecruitment);
                            db.SaveChanges();
                            application.Application_Id = appRecruitment.Id;
                            application.TrackingNumber = application.Id + 901;
                            application.Status_Id = 1;
                            db.Applications.Add(application);
                        }
                        else if (application.ApplicationType_Id == 6)
                        {
                            ApplicationComplaint appComplaint = new ApplicationComplaint();
                            appComplaint.Id = model.detail.Id;
                            appComplaint.Remarks = model.detail.Remarks;
                            db.ApplicationComplaints.Add(appComplaint);
                            db.SaveChanges();
                            application.Application_Id = appComplaint.Id;
                            application.TrackingNumber = application.Id + 901;
                            application.Status_Id = 1;
                            db.Applications.Add(application);
                        }
                        else if (application.ApplicationType_Id == 7)
                        {
                            ApplicationNOC appNOC = new ApplicationNOC();
                            appNOC.Remarks = model.detail.Remarks;
                            db.ApplicationNOCs.Add(appNOC);
                            db.SaveChanges();
                            application.Application_Id = appNOC.Id;
                            application.TrackingNumber = application.Id + 901;
                            application.Status_Id = 1;
                            db.Applications.Add(application);
                        }
                        else if (application.ApplicationType_Id == 8)
                        {
                            ApplicationAdhoc appAdhoc = new ApplicationAdhoc();
                            appAdhoc.Id = model.detail.Id;
                            appAdhoc.CNIC = model.detail.CNIC;
                            appAdhoc.FromHF_Id = model.detail.FromHF_Id;
                            appAdhoc.FromDesignation_Id = model.detail.FromDesignation_Id;
                            appAdhoc.ToHF_Id = model.detail.ToHF_Id;
                            appAdhoc.ToDesignation_Id = model.detail.ToDesignation_Id;
                            appAdhoc.Remarks = model.detail.Remarks;
                            db.ApplicationAdhocs.Add(appAdhoc);
                            db.SaveChanges();
                            application.Application_Id = appAdhoc.Id;
                            application.TrackingNumber = application.Id + 901;
                            application.Status_Id = 1;
                            db.Applications.Add(application);
                        }
                        else if (application.ApplicationType_Id == 9)
                        {
                            ApplicationOther appOther = new ApplicationOther();
                            appOther.Id = model.detail.Id;
                            appOther.CNIC = model.detail.CNIC;
                            appOther.Remarks = model.detail.Remarks;
                            db.ApplicationOthers.Add(appOther);
                            db.SaveChanges();
                            application.Application_Id = appOther.Id;
                            application.TrackingNumber = application.Id + 901;
                            application.Status_Id = 1;
                            db.Applications.Add(application);
                        }
                        else if (application.ApplicationType_Id == 10)
                        {
                            ApplicationAdhocExtension adhocExtension = new ApplicationAdhocExtension();
                            adhocExtension.Id = model.detail.Id;
                            adhocExtension.Remarks = model.detail.Remarks;
                            db.ApplicationAdhocExtensions.Add(adhocExtension);
                            db.SaveChanges();
                            application.Application_Id = adhocExtension.Id;
                            application.TrackingNumber = application.Id + 901;
                            application.Status_Id = 1;
                            db.Applications.Add(application);
                        }
                        db.SaveChanges();
                        application.TrackingNumber = application.Id + 901;
                        db.Entry(application).State = EntityState.Modified;
                        db.SaveChanges();
                        if (application.IsPersonAppeared == false && applicationPersonAppeared.one != null)
                        {
                            string mobileNumber2 = applicationPersonAppeared.one;
                            mobileNumber2 = mobileNumber2.Replace("-", "");
                            SMS sms2 = new SMS()
                            {
                                MobileNumber = mobileNumber2,
                                Message = "Your application has been recieved at Facilitation Centre.\nTracking Number: " + application.TrackingNumber + "\nPrimary and Secondary Healthcare Department."
                            };
                            smsss.Add(sms2);
                        }
                        string mobileNumber = db.HrProfiles.FirstOrDefault(x => x.Id == application.Profile_Id).MobileNo;
                        if(mobileNumber != null)
                        {
                            mobileNumber = mobileNumber.Replace("-", "");
                            SMS sms = new SMS()
                            {
                                MobileNumber = mobileNumber,
                                Message = "Your application has been recieved at Facilitation Centre.\nTracking Number: " + application.TrackingNumber + "\nPrimary and Secondary Healthcare Department."
                            };
                            smsss.Add(sms);

                            Common.SMS_Send(smsss);
                        }
                        Image barCode = barCodeZ(Convert.ToInt32(application.TrackingNumber));
                        string imgSrc = "data:image / jpg; base64," + ImagesUtil.ImageToBase64(barCode, ImageFormat.Jpeg);

                        transc.Commit();

                        return Ok(new { result = true, application = application, trackingNumber = application.TrackingNumber, barCode = imgSrc });
                        #endregion
                    }
                    else if (model.master.Id != 0)
                    {
                        #region Edit
                        Application application = model.master;
                        //Entity_Modified_Log eml = new Entity_Modified_Log();
                        //eml.Modified_By = User.Identity.GetUserId();
                        //eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                        //db.Entity_Modified_Log.Add(eml);
                        //application.Entity_Lifecycle.Entity_Modified_Log.Add(eml);
                        //db.SaveChanges();
                        //application.EntityLifecycle_Id = eld.Id;

                        db.Entry(application).State = EntityState.Modified;

                        if (application.ApplicationType_Id == 1)
                        {
                            ApplicationLeave appLeave = db.ApplicationLeaves.FirstOrDefault(x => x.Id == model.detail.Id);
                            appLeave.FromDate = model.detail.FromDate;
                            appLeave.ToDate = model.detail.ToDate;
                            appLeave.TotalDays = model.detail.TotalDays;
                            appLeave.LeaveType_Id = model.detail.LeaveType_Id;
                            appLeave.Remarks = model.detail.Remarks;
                            db.Entry(appLeave).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else if (application.ApplicationType_Id == 2)
                        {
                            ApplicationTransfer appTransfer = db.ApplicationTransfers.FirstOrDefault(x => x.Id == model.detail.Id);
                            appTransfer.FromHF_Id = model.detail.FromHF_Id;
                            appTransfer.FromDept_Id = model.detail.FromDept_Id;
                            appTransfer.ToHF_Id = model.detail.ToHF_Id;
                            appTransfer.ToDept_Id = model.detail.ToDept_Id;
                            appTransfer.Remarks = model.detail.Remarks;
                            db.Entry(appTransfer).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else if (application.ApplicationType_Id == 3)
                        {
                            ApplicationPromotion appPromotion = db.ApplicationPromotions.FirstOrDefault(x => x.Id == model.detail.Id);
                            appPromotion.CurrentScale = model.detail.CurrentScale;
                            appPromotion.Remarks = model.detail.Remarks;
                            db.Entry(appPromotion).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else if (application.ApplicationType_Id == 4)
                        {
                            ApplicationRetirement appRetirement = db.ApplicationRetirements.FirstOrDefault(x => x.Id == model.detail.Id);
                            appRetirement.RetirementType_Id = model.detail.RetirementType_Id;
                            appRetirement.Remarks = model.detail.Remarks;
                            db.Entry(appRetirement).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else if (application.ApplicationType_Id == 5)
                        {
                            ApplicationRecruitment appRecruitment = db.ApplicationRecruitments.FirstOrDefault(x => x.Id == model.detail.Id);
                            appRecruitment.Id = model.detail.Id;
                            appRecruitment.CNIC = model.detail.CNIC;
                            appRecruitment.Remarks = model.detail.Remarks;
                            db.Entry(appRecruitment).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else if (application.ApplicationType_Id == 6)
                        {
                            ApplicationComplaint appComplaint = db.ApplicationComplaints.FirstOrDefault(x => x.Id == model.detail.Id);
                            appComplaint.Remarks = model.detail.Remarks;
                            db.Entry(appComplaint).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        db.SaveChanges();

                        Image barCode = barCodeZ(Convert.ToInt32(application.TrackingNumber));
                        string imgSrc = "data:image / jpg; base64," + ImagesUtil.ImageToBase64(barCode, ImageFormat.Jpeg);


                        transc.Commit();
                        return Ok(new { result = true, application = application, trackingNumber = application.TrackingNumber, barCode = imgSrc });
                        #endregion
                    }
                    else
                    {
                        return Ok("");
                    }

                }
                catch (Exception ex)
                {
                    transc.Rollback();
                    return Ok(ex.Message);
                }

            }

        }
        [Route("UploadApplicationAttachments/{appId}")]
        public async Task<IHttpActionResult> UploadApplicationAttachments(int appId)
        {
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                string RootPath = HttpContext.Current.Server.MapPath("~/") + @"Uploads\Files\ApplicationAttachments\";
                var dirPath = RootPath;

                var provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);

                CreateDirectoryIfNotExists(dirPath);
                string filename = "";

                foreach (var file in provider.Contents)
                {
                    filename = Guid.NewGuid().ToString() + "_" + appId + "_" + file.Headers.ContentDisposition.FileName.Trim('\"');
                    ApplicationAttachment aa = new ApplicationAttachment();
                    aa.Application_Id = appId;
                    aa.UploadedPath = @"Uploads\Files\ApplicationAttachments\" + filename;
                    aa.IsBase64 = false;
                    db.ApplicationAttachments.Add(aa);
                    var buffer = await file.ReadAsByteArrayAsync();
                    var size = ((buffer.Length) / (1024)) / (1024);
                    var ext = Path.GetExtension(filename.Replace("\"", string.Empty));
                    List<string> validExtensions = new List<string>() { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".docx" };
                    if (!validExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase) || size > 5)
                    {
                        throw new Exception("Unable to Upload. File Size must be less than 5 MB and File Format must be " + string.Join(",", validExtensions));
                    }
                    using (FileStream fsOut = File.OpenWrite(RootPath + @"\" + filename))
                    {
                        fsOut.Write(buffer, 0, buffer.Length);
                    }
                    db.SaveChanges();

                }
                return Ok(new { result = true, src = filename });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("UpdateRawApplication")]
        [HttpPost]
        public IHttpActionResult UpdateRawApplication(Application application)
        {
            try
            {
                if (application.Id != 0)
                {
                    db.Entry(application).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return Ok(new { result = true, application = application });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("GetApplications/{Id}")]
        [HttpGet]
        public IHttpActionResult GetApplications(int Id)
        {
            try
            {
                string user = User.Identity.GetUserName();
                string userId = User.Identity.GetUserId();
                P_SOfficers p_SOfficer = db.P_SOfficers.FirstOrDefault(x => x.User_Id.Equals(userId));
                int? officerCode = p_SOfficer == null ? 0 : p_SOfficer.Code;
                string officerCodeComparable = Convert.ToString(officerCode);
                officerCodeComparable = officerCodeComparable.Length > 5 ? officerCodeComparable.Substring(0, 5) : officerCodeComparable;
                List<ApplicationZView> appviews = new List<ApplicationZView>();
                if (Id == 0)
                {
                    foreach (ApplicationZView appView in db.ApplicationZViews.Where(x => x.OfficerCode != null && (x.FileRequested == null || x.FileRequested == false) && x.Status_Id == 1 && x.IsActive == true).OrderByDescending(x => x.Created_Date).ToList())
                    {
                        //string code = Convert.ToString(officerCode);
                        string appCode = Convert.ToString(appView.OfficerCode);
                        if (appCode.StartsWith(officerCodeComparable))
                        {
                            appviews.Add(appView);
                        }
                    }
                    return Ok(appviews);
                }
                else
                {
                    foreach (ApplicationZView appView in db.ApplicationZViews.Where(x => x.ApplicationType_Id == Id && x.OfficerCode != null && (x.FileRequested == null || x.FileRequested == false) && x.Status_Id == 1 && x.IsActive == true).OrderByDescending(x => x.Created_Date).ToList())
                    {
                        string code = Convert.ToString(officerCode);
                        string appCode = Convert.ToString(appView.OfficerCode);
                        if (appCode.StartsWith(code))
                        {
                            appviews.Add(appView);
                        }
                    }
                    return Ok(appviews);
                }
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("RemoveApplication/{appId}")]
        [HttpGet]
        public IHttpActionResult RemoveApplication(int appId)
        {
            try
            {
                long? elId = db.Applications.FirstOrDefault(x => x.Id == appId).EntityLifecycle_Id;
                Entity_Lifecycle el = db.Entity_Lifecycle.FirstOrDefault(x => x.Id == elId);
                el.IsActive = false;
                db.Entry(el).State = EntityState.Modified;
                db.SaveChanges();
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("GetApplicationDetails/{cnic}/{aplicationTypeId}/{detailId}")]
        [HttpGet]
        public IHttpActionResult GetApplicationDetails(string cnic, int aplicationTypeId, int detailId)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                ProfileDetailsView profile = db.ProfileDetailsViews.FirstOrDefault(x => x.CNIC.Equals(cnic));
                if (aplicationTypeId == 1)
                {
                    var application = db.ApplicationLeaves.FirstOrDefault(x => x.Id == detailId);
                    return Ok(new { profile, application });
                }
                else if (aplicationTypeId == 1)
                {

                    return Ok(new { profile, application = db.ApplicationLeaves.FirstOrDefault(x => x.Id == detailId) });
                }
                else if (aplicationTypeId == 2)
                {

                    return Ok(new { profile, application = db.ApplicationTransfers.FirstOrDefault(x => x.Id == detailId) });
                }
                else if (aplicationTypeId == 3)
                {

                    return Ok(new { profile, application = db.ApplicationPromotions.FirstOrDefault(x => x.Id == detailId) });
                }
                else if (aplicationTypeId == 4)
                {

                    return Ok(new { profile, application = db.ApplicationRetirements.FirstOrDefault(x => x.Id == detailId) });
                }
                else if (aplicationTypeId == 5)
                {

                    return Ok(new { profile, application = db.ApplicationRecruitments.FirstOrDefault(x => x.Id == detailId) });
                }
                else if (aplicationTypeId == 6)
                {

                    return Ok(new { profile, application = db.ApplicationComplaints.FirstOrDefault(x => x.Id == detailId) });
                }
                else if (aplicationTypeId == 7)
                {

                    return Ok(new { profile, application = db.ApplicationNOCs.FirstOrDefault(x => x.Id == detailId) });
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetApplicationsFC/{Id}")]
        [HttpGet]
        public IHttpActionResult GetApplicationsFC(int Id)
        {
            try
            {
                string userId = User.Identity.GetUserId();
                string userName = User.Identity.GetUserName();

                if (userName.Equals("managerfc"))
                {
                    List<ApplicationZView> appviews = new List<ApplicationZView>();
                    if (Id == 0)
                    {
                        return Ok(db.ApplicationZViews.Where(x => x.IsActive == true).OrderByDescending(x => x.Created_Date).ToList());
                    }
                    else
                    {
                        return Ok(db.ApplicationZViews.Where(x => x.ApplicationType_Id == Id && x.IsActive == true).OrderByDescending(x => x.Created_Date).ToList());
                    }
                }
                else
                {
                    List<ApplicationZView> appviews = new List<ApplicationZView>();
                    if (Id == 0)
                    {
                        return Ok(db.ApplicationZViews.Where(x => x.Users_Id.Equals(userId) && x.IsActive == true).OrderByDescending(x => x.Created_Date).ToList());
                    }
                    else
                    {
                        return Ok(db.ApplicationZViews.Where(x => x.ApplicationType_Id == Id && x.Users_Id.Equals(userId) && x.IsActive == true).OrderByDescending(x => x.Created_Date).ToList());
                    }
                }
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetApplicationsSummary")]
        [HttpGet]
        public IHttpActionResult GetApplicationsSummary()
        {
            try
            {
                return Ok(db.ApplicationSummaryTotals.ToList());
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetParliamentarianReport")]
        [HttpGet]
        public IHttpActionResult GetParliamentarianReport()
        {
            try
            {
                return Ok(db.ParliamentarianReportViews.ToList());
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetSummaryDetails/{designationName}/{applicationTypeId}")]
        [HttpGet]
        public IHttpActionResult GetApplicationsSummary(string designationName, int applicationTypeId)
        {
            try
            {
                List<ApplicationZView> list = new List<ApplicationZView>();
                if (applicationTypeId == 0)
                {
                    list = db.ApplicationZViews.Where(x => x.OfficerDesignation.Equals(designationName) && x.IsActive == true).OrderByDescending(x => x.Created_Date).ToList();
                }
                else
                {
                    list = db.ApplicationZViews.Where(x => x.OfficerDesignation.Equals(designationName) && x.ApplicationType_Id == applicationTypeId && x.IsActive == true).OrderByDescending(x => x.Created_Date).ToList();
                }
                return Ok(list);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("ForwardApplication/{appId}/{officerId}")]
        [HttpGet]
        public IHttpActionResult ForwardApplication(int appId, int officerId)
        {
            try
            {
                db.Applications.FirstOrDefault(x => x.Id == appId).PandSOfficer_Id = officerId;
                db.SaveChanges();
                return Ok(true);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetApplicationByTrackingId/{trackingId}")]
        [HttpGet]
        public IHttpActionResult GetApplicationByTrackingId(int trackingId)
        {
            try
            {

                Application application = db.Applications.FirstOrDefault(x => x.TrackingNumber == trackingId);
                ApplicationsListModel alm = new ApplicationsListModel();
                alm.master = application;
                ProfileDetailsView profile = db.ProfileDetailsViews.FirstOrDefault(x => x.Id == application.Profile_Id);

                if (application.ApplicationType_Id == 1)
                {
                    var applicationDetail = db.ApplicationLeaves.FirstOrDefault(x => x.Id == alm.master.Application_Id);
                    return Ok(new { application, applicationDetail, profile });
                }
                else if (application.ApplicationType_Id == 2)
                {
                    var applicationDetail = db.ApplicationTransfers.FirstOrDefault(x => x.Id == alm.master.Application_Id);
                    return Ok(new { application, applicationDetail, profile });
                }
                else if (application.ApplicationType_Id == 3)
                {
                    var applicationDetail = db.ApplicationPromotions.FirstOrDefault(x => x.Id == alm.master.Application_Id);
                    return Ok(new { application, applicationDetail, profile });
                }
                else if (application.ApplicationType_Id == 4)
                {
                    var applicationDetail = db.ApplicationRetirements.FirstOrDefault(x => x.Id == alm.master.Application_Id);
                    return Ok(new { application, applicationDetail, profile });
                }
                else if (application.ApplicationType_Id == 5)
                {
                    var applicationDetail = db.ApplicationRecruitments.FirstOrDefault(x => x.Id == alm.master.Application_Id);
                    return Ok(new { application, applicationDetail, profile });
                }
                else if (application.ApplicationType_Id == 6)
                {
                    var applicationDetail = db.ApplicationComplaints.FirstOrDefault(x => x.Id == alm.master.Application_Id);
                    return Ok(new { application, applicationDetail, profile });
                }
                else
                {
                    return Ok(new { application, profile });

                }
                //else if (application.ApplicationType_Id == 7)
                //{
                //    var applicationDetail = db.ApplicationTransfers.FirstOrDefault(x => x.Id == alm.master.Application_Id);
                //}
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("SearchByTrackingNumber/{number}")]
        [HttpGet]
        public IHttpActionResult SearchByTrackingNumber(int number)
        {
            return Ok(db.ApplicationZViews.Where(x => x.TrackingNumber == number && x.IsActive == true).ToList());
        }
        [Route("SearchByCNIC/{cnic}")]
        [HttpGet]
        public IHttpActionResult SearchByCNIC(string cnic)
        {
            return Ok(db.ApplicationZViews.Where(x => x.CNIC.Equals(cnic) && x.IsActive == true).ToList());
        }
        [Route("SearchByName/{query}")]
        [HttpGet]
        public IHttpActionResult SearchByName(string query = "")
        {
            return Ok(db.ApplicationZViews.Where(x => x.EmployeeName.ToLower().Contains(query.ToLower()) && x.IsActive == true).ToList());
        }
        [Route("GenereateFileRequest/{profile_Id}/{application_Id}/{file_Id}")]
        [HttpGet]
        public IHttpActionResult GenereateFileRequest(int profile_Id, int application_Id, int file_Id)
        {

            try
            {
                List<ApplicationFileReqView> recentFileRequests = db.ApplicationFileReqViews.Where(x => x.EmpFile_Id == file_Id && x.RequestStatus_Id != 3).OrderByDescending(x => x.Created_Date).ToList();
                if (recentFileRequests.Count > 0)
                {
                    return Ok(new { result = false, madeBy = recentFileRequests[0].SectionName });
                }
                int? trackingNumber = 0;
                HrProfile profile = db.HrProfiles.FirstOrDefault(x => x.Id == profile_Id);
                ApplicationFileRecosition afr = new ApplicationFileRecosition();

                if (profile != null) afr.Profile_Id = profile_Id;
                else afr.Profile_Id = null;
                Application app = db.Applications.FirstOrDefault(x => x.Id == application_Id);
                if (app != null)
                {
                    trackingNumber = app.TrackingNumber;
                    db.Applications.FirstOrDefault(x => x.Id == application_Id).FileRequested = true;
                    afr.Application_Id = application_Id;
                }
                else
                {
                    trackingNumber = 0;
                    afr.Application_Id = null;
                }

                afr.RequestStatus_Id = 1;

                afr.EmpFile_Id = file_Id;

                afr.RequestGenDateTime = DateTime.UtcNow.AddHours(5);

                Entity_Lifecycle elc = new Entity_Lifecycle();
                elc.Created_By = User.Identity.GetUserName();
                elc.Users_Id = User.Identity.GetUserId();
                elc.Created_Date = DateTime.UtcNow.AddHours(5);
                elc.IsActive = true;
                elc.Entity_Id = 54;

                db.Entity_Lifecycle.Add(elc);
                db.SaveChanges();

                afr.EntityLifecycle_Id = elc.Id;

                db.ApplicationFileRecositions.Add(afr);
                db.SaveChanges();
                return Ok(new { result = true, afrId = afr.Id });
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetFileRequests/{CurrentPage}/{ItemsPerPage}/{statusID}")]
        [HttpGet]
        public IHttpActionResult GetFileRequests(int CurrentPage, int ItemsPerPage, int statusID)
        {
            try
            {
                int totalRecords = db.ApplicationFileReqViews.Where(x => x.RequestStatus_Id == statusID && x.IsActive == true).Count();
                var requests = db.ApplicationFileReqViews.Where(x => x.RequestStatus_Id == statusID && x.IsActive == true).OrderByDescending(x => x.Created_Date).Skip((CurrentPage - 1) * ItemsPerPage).Take(ItemsPerPage).ToList();
                return Ok(new { totalRecords, requests });
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }
        [Route("GetFileRequestsStatus")]
        [HttpGet]
        public IHttpActionResult GetFileRequestsStatus()
        {
            try
            {
                string userId = User.Identity.GetUserId();
                return Ok(db.ApplicationFileReqViews.Where(x => x.Users_Id.Equals(userId) && x.IsActive == true).OrderByDescending(x => x.Created_Date).ToList());
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }
        [Route("RemoveFileRequests/{Id}")]
        [HttpGet]
        public IHttpActionResult RemoveFileRequests(int Id)
        {
            try
            {
                ApplicationFileRecosition afr = db.ApplicationFileRecositions.FirstOrDefault(x => x.Id == Id);
                Entity_Lifecycle elc = db.Entity_Lifecycle.FirstOrDefault(x => x.Id == afr.EntityLifecycle_Id);
                elc.IsActive = false;
                db.Entry(elc).State = EntityState.Modified;
                db.SaveChanges();
                return Ok(true);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }
        [Route("ReleaseFile/{ReqId}/{recievedBy}/{contact}")]
        [HttpGet]
        public IHttpActionResult ReleaseFile(int ReqId, string recievedBy, string contact)
        {
            try
            {
                ApplicationFileRecosition afr = db.ApplicationFileRecositions.FirstOrDefault(x => x.Id == ReqId);
                if (afr != null)
                {
                    afr.RecievedBy = recievedBy;
                    afr.RecievedByContactNo = contact;
                    afr.RequestStatus_Id = 2;
                    afr.RequestApproveDateTime = DateTime.UtcNow.AddHours(5);
                    db.Entry(afr).State = EntityState.Modified;
                    db.SaveChanges();
                    return Ok(new { result = true });
                }
                else
                {
                    return Ok(new { result = false, message = "No Application Exist" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("GetCNICByProfileId/{profile_Id}")]
        [HttpGet]
        public IHttpActionResult GetCNICByProfileId(int profile_Id)
        {
            try
            {
                string cnic = db.HrProfiles.FirstOrDefault(x => x.Id == profile_Id).CNIC;
                //return Ok(addDashes(cnic));
                return Ok(cnic);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("SubmitFileRecRemarks")]
        [HttpPost]
        public IHttpActionResult SubmitFileRecRemarks([FromBody]ApplicationFRecRemark applicationFRecRemark)
        {
            try
            {
                if (applicationFRecRemark.Id == 0)
                {
                    applicationFRecRemark.DateAndTime = DateTime.UtcNow.AddHours(5);
                    applicationFRecRemark.Created_By = User.Identity.GetUserName();
                    applicationFRecRemark.User_Id = User.Identity.GetUserId();
                    applicationFRecRemark.IsActive = true;
                    db.ApplicationFRecRemarks.Add(applicationFRecRemark);
                }
                else
                {
                    applicationFRecRemark.DateAndTime = DateTime.UtcNow.AddHours(5);
                    applicationFRecRemark.Created_By = User.Identity.GetUserName();
                    applicationFRecRemark.User_Id = User.Identity.GetUserId();
                    applicationFRecRemark.IsActive = true;
                    db.Entry(applicationFRecRemark).State = EntityState.Modified;
                }
                db.SaveChanges();
                return Ok(applicationFRecRemark);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("GetFileRecRemarks/{applicationRecosition_Id}")]
        [HttpGet]
        public IHttpActionResult GetFileRecRemarks(int applicationRecosition_Id)
        {
            try
            {
                string userID = User.Identity.GetUserId();
                return Ok(db.ApplicationFRecRemarks.Where(x => x.ApplicationFileRecosition_Id == applicationRecosition_Id && x.IsActive == true).ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("SubmitApplicationRemarks/{status_Id}")]
        [HttpPost]
        public IHttpActionResult SubmitApplicationRemarks([FromBody]ApplicationRemark applicationRemark, int status_Id)
        {
            try
            {
                if (status_Id == 0)
                {
                    if (applicationRemark.Id == 0)
                    {
                        applicationRemark.DateAndTime = DateTime.UtcNow.AddHours(5);
                        applicationRemark.Created_By = User.Identity.GetUserName();
                        applicationRemark.User_Id = User.Identity.GetUserId();
                        applicationRemark.IsActive = true;
                        db.ApplicationRemarks.Add(applicationRemark);
                    }
                    else
                    {
                        applicationRemark.DateAndTime = DateTime.UtcNow.AddHours(5);
                        applicationRemark.Created_By = User.Identity.GetUserName();
                        applicationRemark.User_Id = User.Identity.GetUserId();
                        applicationRemark.IsActive = true;
                        db.Entry(applicationRemark).State = EntityState.Modified;
                    }

                }
                else
                {
                    Application application = db.Applications.FirstOrDefault(x => x.Id == applicationRemark.Application_Id);
                    application.Status_Id = status_Id;
                    db.Entry(application).State = EntityState.Modified;
                }
                db.SaveChanges();
                return Ok(applicationRemark);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("GetApplicationRemarks/{application_Id}")]
        [HttpGet]
        public IHttpActionResult GetApplicationRemarks(int application_Id)
        {
            try
            {
                string userID = User.Identity.GetUserId();
                return Ok(db.ApplicationRemarks.Where(x => x.Application_Id == application_Id && x.IsActive == true).ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("IssueFile/{type}")]
        [HttpPost]
        public IHttpActionResult IssueFile(int type, [FromBody]FileIssue issueFileObject)
        {
            try
            {
                if (type == 1)
                {
                    ApplicationFileRecosition afr = db.ApplicationFileRecositions.FirstOrDefault(x => x.Id == issueFileObject.ReqId);
                    afr.RecievedBy = issueFileObject.IssueTo;
                    afr.RecievedByCNIC = issueFileObject.CNIC;
                    afr.RecievedByContactNo = issueFileObject.Contact;
                    afr.RecievedByDesig = issueFileObject.Designation;
                    afr.RequestStatus_Id = 2;
                    afr.EmpFile_Id = (int)issueFileObject.File_Id;
                    afr.RequestApproveDateTime = DateTime.UtcNow.AddHours(5);
                    db.Entry(afr).State = EntityState.Modified;
                    db.FileIssues.Add(issueFileObject);
                    db.SaveChanges();
                    return Ok(true);
                }
                else
                {
                    ApplicationFileRecosition afr = db.ApplicationFileRecositions.FirstOrDefault(x => x.Id == issueFileObject.ReqId);
                    afr.ReturnBy = issueFileObject.IssueTo;
                    afr.ReturnByCNIC = issueFileObject.CNIC;
                    afr.ReturnByContactNo = issueFileObject.Contact;
                    afr.ReturnByDesig = issueFileObject.Designation;
                    afr.EmpFile_Id = (int)issueFileObject.File_Id;
                    afr.RequestStatus_Id = 3;
                    afr.ReturnDateTime = DateTime.UtcNow.AddHours(5);
                    db.Entry(afr).State = EntityState.Modified;
                    db.SaveChanges();
                    return Ok(true);
                }
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("HrFile")]
        [HttpPost]
        public IHttpActionResult HrFile(HrFile hrFile)
        {
            try
            {
                //if file already exisit

                HrFile existingFile = db.HrFiles.FirstOrDefault(x => x.FileNo.Equals(hrFile.FileNo));

                if (existingFile == null)
                {
                    Entity_Lifecycle elc = new Entity_Lifecycle();
                    elc.Created_By = User.Identity.GetUserName();
                    elc.Users_Id = User.Identity.GetUserId();
                    elc.Created_Date = DateTime.UtcNow.AddHours(5);
                    elc.IsActive = true;
                    elc.Entity_Id = 94;

                    db.Entity_Lifecycle.Add(elc);
                    db.SaveChanges();

                    hrFile.EntityLifeCycleId = elc.Id;
                    db.HrFiles.Add(hrFile);
                    //db.Entry(hrFile).State = EntityState.Added;
                    db.SaveChanges();
                    return Ok(new { result = true, file = hrFile });
                }
                else
                {
                    //existingFile.Row = hrFile.Row;
                    //existingFile.Rack = hrFile.Rack;
                    //existingFile.Room = hrFile.Room;
                    hrFile.Id = existingFile.Id;
                    hrFile.EntityLifeCycleId = existingFile.EntityLifeCycleId;

                    Entity_Modified_Log eml = new Entity_Modified_Log();
                    eml.Modified_By = User.Identity.GetUserId();
                    eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                    eml.Entity_Lifecycle_Id = (long)hrFile.EntityLifeCycleId;
                    eml.Description = "File Record updated by " + User.Identity.GetUserName();
                    db.Entity_Modified_Log.Add(eml);
                    db.SaveChanges();
                    db.Entity_Lifecycle.FirstOrDefault(x => x.Id == hrFile.EntityLifeCycleId).Entity_Modified_Log.Add(eml);
                    var attach = db.HrFiles.Local.FirstOrDefault(x => x.Id == hrFile.Id);
                    if (attach != null)
                    {
                        attach.Rack = hrFile.Rack;
                        attach.Room = hrFile.Room;
                        attach.Row = hrFile.Row;
                        attach.FileNo = hrFile.FileNo;
                        attach.TotalPages = hrFile.TotalPages;
                        attach.EntityLifeCycleId = hrFile.EntityLifeCycleId;
                        attach.CNIC = hrFile.CNIC;
                        attach.HrProfileId = hrFile.HrProfileId;
                        db.Entry(attach).State = EntityState.Modified;
                    }
                    else
                    {
                        db.Entry(hrFile).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                    return Ok(new { result = true, file = hrFile });
                }

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetHrFile/{reqId}")]
        [HttpGet]
        public IHttpActionResult GetHrFile(int reqId)
        {
            try
            {
                ApplicationFileReqView afr = db.ApplicationFileReqViews.FirstOrDefault(x => x.Id == reqId);

                P_SOfficers officer = db.P_SOfficers.FirstOrDefault(x => x.User_Id.Equals(afr.Users_Id));

                if (officer == null)
                {
                    return Ok(new { file = db.HrFiles.FirstOrDefault(x => x.Id == afr.EmpFile_Id) });
                }
                else
                {
                    return Ok(new { file = db.HrFiles.FirstOrDefault(x => x.Id == afr.EmpFile_Id), officer = officer });
                }
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetOfficers")]
        [HttpGet]
        public IHttpActionResult GetOfficers()
        {
            using (var db = new HR_SystemFC())
            {
                try
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    string userId = User.Identity.GetUserId();
                    P_SOfficers officer = db.P_SOfficers.FirstOrDefault(x => x.User_Id.Equals(userId));
                    int? officerCode = 0;
                    if (officer != null)
                    {
                        officerCode = officer.Code;
                    }
                    string officerCodeComparable = Convert.ToString(officerCode);

                    List<P_SOfficers> officers = new List<P_SOfficers>();
                    P_SOfficers currentOfficers = db.P_SOfficers.FirstOrDefault(x => x.User_Id.Equals(userId));

                    foreach (var item in db.P_SOfficers.Where(x => x.Code != null && x.Code.ToString().StartsWith(officerCodeComparable) && !x.Code.ToString().Equals(officerCodeComparable)).ToList())
                    {
                        officers.Add(item);
                    }

                    return Ok(new { officers, currentOfficers });
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }
        [Route("GetHFMISCode/{HF_Id}")]
        [HttpGet]
        public IHttpActionResult GetHFMISCode(int HF_Id)
        {
            using (var db = new HR_SystemFC())
            {
                try
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    var obj = db.HealthFacilityDetails.Where(x => x.Id == HF_Id).Select(x => new
                    {
                        Id = x.Id,
                        DivisionCode = x.DivisionCode,
                        DistrictCode = x.DistrictCode,
                        TehsilCode = x.TehsilCode,
                        HFMISCode = x.HFMISCode,
                    });
                    return Ok(obj.ToList());
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }
        [Route("GetAllOfficers")]
        [HttpGet]
        public IHttpActionResult GetAllOfficers()
        {
            using (var db = new HR_SystemFC())
            {
                try
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    return Ok(db.P_SOfficers.ToList());
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }
        [Route("GetCurrentOfficer/{userId}")]
        [HttpGet]
        public IHttpActionResult GetCurrentOfficer(string userId)
        {
            using (var db = new HR_SystemFC())
            {
                try
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    P_SOfficers officer = db.P_SOfficers.FirstOrDefault(x => x.User_Id.Equals(userId));

                    if (officer == null)
                    {
                        return Ok(new { result = true, officer = officer });
                    }
                    else
                    {
                        return Ok(new { result = false, officer = officer });
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }
        [Route("PostPandSOfficer")]
        [HttpPost]
        public IHttpActionResult PostPandSOfficer(P_SOfficers p_sOfficer)
        {
            using (var db = new HR_SystemFC())
            {
                try
                {
                    if (p_sOfficer.Id == 0)
                    {
                        List<int?> P_SOfficersCodes = new List<int?>();

                        List<P_SOfficers> P_SOfficersList = db.P_SOfficers.Where(x => x.Code.ToString().StartsWith(p_sOfficer.Code.ToString())).OrderByDescending(x => x.Code).ToList();

                        string masterCode = P_SOfficersList.First().Code.ToString();
                        if (masterCode.Length == 5)
                        {
                            masterCode += "1";
                        }
                        else if (masterCode.Length >= 6)
                        {
                            int codeIncrement = Convert.ToInt32(masterCode) + 1;
                            masterCode = codeIncrement.ToString();
                        }
                        p_sOfficer.Code = Convert.ToInt32(masterCode);
                        p_sOfficer.User_Id = User.Identity.GetUserId();
                        db.P_SOfficers.Add(p_sOfficer);
                        db.SaveChanges();
                    }
                    return Ok(true);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }
        [Route("CalcDate/{fromDate}/{toDate}/{totalDays}")]
        [HttpGet]
        public IHttpActionResult CalcDate(string fromDate, string toDate, int totalDays)
        {
            try
            {
                if (toDate.Equals("noDate"))
                {
                    return Ok(Common.ToDate(fromDate, totalDays));

                }
                else
                {
                    return Ok(Common.CalculateDays(fromDate, toDate));
                }
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("HrFileEnter/{fileNo}/{employeeName}")]
        [HttpGet]
        public IHttpActionResult HrFileEnter(string fileNo, string employeeName)
        {
            try
            {
                HrFile hrfile = new HrFile();
                hrfile.FileNo = fileNo;
                hrfile.Room = employeeName;

                Entity_Lifecycle elc = new Entity_Lifecycle();
                elc.Created_By = User.Identity.GetUserName();
                elc.Users_Id = User.Identity.GetUserId();
                elc.Created_Date = DateTime.UtcNow.AddHours(5);
                elc.IsActive = true;
                elc.Entity_Id = 94;

                db.Entity_Lifecycle.Add(elc);
                db.SaveChanges();

                hrfile.EntityLifeCycleId = elc.Id;

                db.HrFiles.Add(hrfile);
                db.SaveChanges();
                return Ok(new { result = true, hrfile = hrfile });
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("HrFileEntered")]
        [HttpGet]
        public IHttpActionResult HrFileEntered()
        {
            try
            {

                return Ok(new { result = true, hrfiles = db.HrFiles.Where(x => !x.Room.Equals("Central Record Room")).OrderByDescending(x => x.EntityLifeCycleId).ToList() });
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("FPrint")]
        [HttpPost]
        public IHttpActionResult FPrint(FPPrint fprint)
        {
            try
            {
                string userId = User.Identity.GetUserId();
                P_SOfficers p_SOfficer = db.P_SOfficers.FirstOrDefault(x => x.User_Id.Equals(userId));
                //var fingerprintVal = Fmd.SerializeXml(new FingerprintSdk().GetFmd(fprint.metaData));
                //p_SOfficer.FingerPrint = fingerprintVal;
                db.Entry(p_SOfficer).State = EntityState.Modified;
                db.SaveChanges();
                return Ok(true);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("FPrintRegister/{officer_Id}/{number}")]
        [HttpPost]
        public IHttpActionResult FPrintRegister([FromBody] FPPrint fprint, int officer_Id, int number)
        {
            try
            {
                P_SOfficers p_SOfficer = db.P_SOfficers.FirstOrDefault(x => x.Id == officer_Id);

                bool fpExist = true;
                FingerPrint fp = db.FingerPrints.FirstOrDefault(x => x.PandSOfficer_Id == p_SOfficer.Id);

                if (fp == null)
                {
                    fpExist = false;
                    fp = new FingerPrint();
                }
                //if (number == 1) fp.FP1 = Fmd.SerializeXml(new FingerprintSdk().GetFmd(fprint.metaData));
                //if (number == 2) fp.FP2 = Fmd.SerializeXml(new FingerprintSdk().GetFmd(fprint.metaData));
                //if (number == 3) fp.FP3 = Fmd.SerializeXml(new FingerprintSdk().GetFmd(fprint.metaData));
                //if (number == 4) fp.FP4 = Fmd.SerializeXml(new FingerprintSdk().GetFmd(fprint.metaData));
                //if (number == 5) fp.FP5 = Fmd.SerializeXml(new FingerprintSdk().GetFmd(fprint.metaData));

                fp.PandSOfficer_Id = p_SOfficer.Id;
                fp.User_Id = p_SOfficer.User_Id;
                fp.DateTime = DateTime.UtcNow.AddHours(5);

                if (fpExist) db.Entry(fp).State = EntityState.Modified;
                else db.FingerPrints.Add(fp);
                db.Configuration.ProxyCreationEnabled = false;

                db.SaveChanges();

                p_SOfficer.FingerPrint_Id = fp.Id;
                db.Entry(p_SOfficer).State = EntityState.Modified;
                db.SaveChanges();
                return Ok(fp);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("SendSMS")]
        [HttpPost]
        public IHttpActionResult SendSMS([FromBody]List<SMS> smsss)
        {
            foreach (var sms in smsss)
            {
                sms.MobileNumber = sms.MobileNumber.Replace("-", "");
            }
            return Ok(Common.SMS_Send(smsss));
        }
        public Image barCodeZ(long Id)
        {
            Zen.Barcode.Code128BarcodeDraw barcode = Zen.Barcode.BarcodeDrawFactory.Code128WithChecksum;
            return barcode.Draw(Convert.ToString(Id), 100, 2);
        }


        private string addDashes(string cnic)
        {
            return cnic[0] + cnic[1] + cnic[2] + cnic[3] + cnic[4] + "-" +
                cnic[5] + cnic[6] + cnic[7] + cnic[8] + cnic[9] + cnic[10] + cnic[11] + "-" + cnic[12];


        }

        private void CreateDirectoryIfNotExists(string dirPath) { if (!Directory.Exists(dirPath)) { Directory.CreateDirectory(dirPath); } }
        protected override void Dispose(bool disposing) { if (disposing) { db.Dispose(); } base.Dispose(disposing); }

        private string GetDbExMessage(DbEntityValidationException dbx)
        {
            return dbx.EntityValidationErrors.SelectMany(validationErrors => validationErrors.ValidationErrors)
                .Aggregate("",
                    (current, validationError) =>
                        current +
                        $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
        }

    }
    public class FPPrint
    {
        public string metaData { get; set; }
    }
}
