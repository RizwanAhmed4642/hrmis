using Hrmis.Models.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using System.Data.Entity.Validation;
using Hrmis.Models.Common;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using Hrmis.Models.ImageProcessor;
using System.Drawing.Imaging;
using System.Drawing;

namespace Hrmis.Controllers.HrmisApiControllers
{
    [RoutePrefix("api/PromotionApplication")]
    public class PromotionApplicationController : ApiController
    {
        #region Reporting
        [Route("ApplicationsReport")]
        [HttpGet]
        public IHttpActionResult ApplicationsReport()
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    return Ok(new { leaveApplications = db.LeaveApplications.ToList(), transferApplications = db.TransferApplications.ToList() });
                }
            }
            catch (DbEntityValidationException dbx)
            {
                string message = GetDbExMessage(dbx);
                return BadRequest(message);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }
        #endregion

        #region Master Code
        [Route("GetUserDetails")]
        [HttpGet]
        public IHttpActionResult GetUserDetails()
        {
            try
            {
                using (var db = new HR_System())
                {

                    db.Configuration.ProxyCreationEnabled = false;
                    string userId = User.Identity.GetUserId();
                    UserDetail userDetail = db.UserDetails.FirstOrDefault(x => x.UserId.Equals(userId));
                    return Ok(userDetail);
                }
            }
            catch (DbEntityValidationException dbx)
            {
                string message = GetDbExMessage(dbx);
                return BadRequest(message);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }
        [Route("GetApplicationsByCNIC/{CNIC}")]
        [HttpGet]
        public IHttpActionResult GetApplications(string CNIC)
        {
            try
            {
                using (var db = new HR_System())
                {
                    CNIC = CNIC.Replace("-", "");
                    db.Configuration.ProxyCreationEnabled = false;
                    var leaveApplications = db.LeaveApplication_View.Where(x => x.CNIC.Equals(CNIC)).OrderByDescending(x => x.Created_Date).ToList();
                    var transferApplications = db.TransferApplication_View.Where(x => x.CNIC.Equals(CNIC)).OrderByDescending(x => x.Created_Date).ToList();
                    return Ok(new { leaveApplications, transferApplications });
                }
            }
            catch (DbEntityValidationException dbx)
            {
                string message = GetDbExMessage(dbx);
                return BadRequest(message);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }
        [Route("GetAllApplications/{officeId}/{statusId}")]
        [HttpGet]
        public IHttpActionResult GetAllApplications(int officeId, int statusId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    string userId = User.Identity.GetUserId();
                    Nullable<int> officerId = (int)db.UserDetails.FirstOrDefault(x => x.UserId.Equals(userId)).SO_Id;
                    IQueryable<LeaveApplication_View> leaveApplications;
                    IQueryable<TransferApplication_View> transferApplications;
                    leaveApplications = db.LeaveApplication_View.Where(x => x.LeaveStatus_Id == statusId).OrderByDescending(x => x.Created_Date);
                    transferApplications = db.TransferApplication_View.Where(x => x.ApplicationStatus_Id == statusId).OrderByDescending(x => x.Created_Date);

                    if (officeId == OfficerLevels.FrontDesk)
                    {
                        leaveApplications = leaveApplications.Where(x => x.SO_Id == null && x.DS_Id == null && x.AS_Id == null && x.Secretary == null);
                        transferApplications = transferApplications.Where(x => x.SO_Id == null && x.DS_Id == null && x.AS_Id == null && x.Secretary == null);
                    }
                    else if (officeId == OfficerLevels.SectionOfficer)
                    {
                        leaveApplications = leaveApplications.Where(x => x.SO_Id == officerId && x.DS_Id == null && x.AS_Id == null && x.Secretary == null);
                        transferApplications = transferApplications.Where(x => x.SO_Id == officerId && x.DS_Id == null && x.AS_Id == null && x.Secretary == null);
                    }
                    else if (officeId == OfficerLevels.DeputySecretary)
                    {
                        leaveApplications = leaveApplications.Where(x => x.DS_Id == officerId && x.AS_Id == null && x.Secretary == null);
                        transferApplications = transferApplications.Where(x => x.DS_Id == officerId && x.AS_Id == null && x.Secretary == null);
                    }
                    else if (officeId == OfficerLevels.AdditionalSecretary)
                    {
                        leaveApplications = leaveApplications.Where(x => x.AS_Id == officerId && x.Secretary == null);
                        transferApplications = transferApplications.Where(x => x.AS_Id == officerId && x.Secretary == null);
                    }
                    else if (officeId == OfficerLevels.Secretary)
                    {
                        leaveApplications = leaveApplications.Where(x => x.Secretary == true);
                        transferApplications = transferApplications.Where(x => x.Secretary == true);
                    }
                    else if (officeId == OfficerLevels.Hisdu)
                    {
                        leaveApplications = leaveApplications.Where(x => x.LeaveStatus_Id == 2);
                        transferApplications = transferApplications.Where(x => x.ApplicationStatus_Id == 2);
                    }
                    var lAs = leaveApplications.ToList();
                    var tAs = transferApplications.ToList();
                    return Ok(new
                    {
                        leaveApplications = lAs,
                        transferApplications = tAs
                    });
                }
            }
            catch (DbEntityValidationException dbx)
            {
                string message = GetDbExMessage(dbx);
                return BadRequest(message);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [Route("GetOfficers/{officeId}")]
        [HttpGet]
        public IHttpActionResult GetOfficers(int officeId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    return Ok(db.Officers.Where(x => x.Office_Id == (officeId + 1)).ToList());
                }
            }
            catch (DbEntityValidationException dbx)
            {
                string message = GetDbExMessage(dbx);
                return BadRequest(message);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [Route("GetLeaveTypes")]
        [HttpGet]
        public IHttpActionResult GetLeaveTypes()
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    return Ok(db.LeaveTypes.ToList());
                }
            }
            catch (DbEntityValidationException dbx)
            {
                string message = GetDbExMessage(dbx);
                return BadRequest(message);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }


        [Route("SearchByTrackingID/{trackingID}")]
        [HttpGet]
        public IHttpActionResult SearchByTrackingID(string trackingID)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    int applicationType = 0;
                    int trNo = Convert.ToInt32(trackingID);
                    var resultLeave = db.LeaveApplication_View.FirstOrDefault(x => x.TrackingNo == trNo);
                    //var resultTransfer = db.TransferApplication_View.FirstOrDefault(x => x.TrackingNo == trNo);
                    var resultTransfer = db.TransferApplication_View.Where(x => x.TrackingNo == trNo).FirstOrDefault();
                    if (resultLeave != null)
                    {
                        applicationType = 1;
                        return Ok(new { resultLeave, applicationType });
                    }
                    else
                    {
                        applicationType = 2;
                        return Ok(new { resultTransfer, applicationType });
                    }
                }
            }
            catch (DbEntityValidationException dbx)
            {
                string message = GetDbExMessage(dbx);
                return BadRequest(message);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpPost]
        [Route("SavePromo/{reqType}/{mobile}/{email}/{networkId}/{remarks}")]
        public IHttpActionResult SavePromo([FromBody] ApplicationRequest applicationRequest, int reqType, string mobile, string email, int networkId, string remarks)
        {
            using (var db = new HR_System())
            {
                try
                {
                    FCRequest fcRequest = new FCRequest();
                    fcRequest.Profile_Id = applicationRequest.Profile_Id;
                    fcRequest.CNIC = applicationRequest.CNIC;
                    fcRequest.FileNumber = applicationRequest.FileNumber;
                    fcRequest.IsActive = true;
                    fcRequest.CREATION_DATE = DateTime.UtcNow.AddHours(5);
                    fcRequest.ContactProvided = mobile.Equals("null") ? null : mobile;
                    fcRequest.EmailProvided = email.Equals("null") ? null : email;
                    fcRequest.GSMNetwork_Id = networkId;
                    db.FCRequests.Add(fcRequest);
                    db.SaveChanges();

                    if (reqType == 1)
                    {
                        FCPromotion fcPromotion = new FCPromotion();
                        fcPromotion.Master_Id = fcRequest.ID;
                        fcPromotion.CurrentWorkStep_Id = 0;
                        fcPromotion.Remarks = remarks.Equals("null") ? null : remarks;
                        db.FCPromotions.Add(fcPromotion);
                        db.SaveChanges();
                        fcPromotion.TrackingNo = fcPromotion.ID + 1000;
                        db.Entry(fcPromotion).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else if (reqType == 2)
                    {
                        FCTransfer fcTransfer = new FCTransfer();
                        fcTransfer.Master_Id = fcRequest.ID;
                        fcTransfer.CurrentWorkStep_Id = 0;
                        fcTransfer.Remarks = remarks.Equals("null") ? null : remarks;
                        db.FCTransfers.Add(fcTransfer);
                        db.SaveChanges();
                        fcTransfer.TrackingNo = fcTransfer.ID + 1000;
                        fcTransfer.HF_Id = applicationRequest.HF_Id == null ? null : applicationRequest.HF_Id;
                        fcTransfer.HFMISCode = applicationRequest.HFMISCode;
                        db.Entry(fcTransfer).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else if (reqType == 3)
                    {

                    }

                    Entity_Lifecycle eld = new Entity_Lifecycle();
                    eld.Created_Date = DateTime.UtcNow.AddHours(5);
                    eld.Created_By = User.Identity.GetUserName();
                    eld.Users_Id = User.Identity.GetUserId();
                    eld.IsActive = true;
                    eld.Entity_Id = 12;
                    db.Entity_Lifecycle.Add(eld);
                    applicationRequest.EntityLifecycle_Id = eld.Id;
                    db.ApplicationRequests.Add(applicationRequest);
                    db.SaveChanges();


                    applicationRequest.TrackingID = applicationRequest.Id + 1000;
                    db.Entry(applicationRequest).State = EntityState.Modified;
                    db.SaveChanges();

                    string mobileNumber = db.HrProfiles.FirstOrDefault(x => x.Id == applicationRequest.Profile_Id).MobileNo;
                    mobileNumber = mobileNumber.Replace("-", "");
                    List<SMS> smsss = new List<SMS>();
                    SMS sms = new SMS()
                    {
                        MobileNumber = mobileNumber,
                        Message = "Application for Transfer has been recieved.\nYour Tracking ID: " + applicationRequest.TrackingID
                    };
                    smsss.Add(sms);
                    Common.SMS_Send(smsss);
                    return Ok(new { result = true, TrackingID = applicationRequest.TrackingID });
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
        }

        #endregion
        #region ONLINE APPLICATION FORM
        [HttpPost]
        [Route("SaveProfileReq")]
        public IHttpActionResult SaveProfileReq([FromBody] HrProfileReq hrProfileReq)
        {
            using (var db = new HR_System())
            {
                using (var transc = db.Database.BeginTransaction())
                {

                    try
                    {

                        Entity_Lifecycle eld = new Entity_Lifecycle();
                        eld.Created_Date = DateTime.UtcNow.AddHours(5);
                        if (User.Identity == null)
                        {
                            eld.Created_By = hrProfileReq.EmployeeName;
                            eld.Users_Id = "null";
                        }
                        else
                        {
                            eld.Created_By = User.Identity.GetUserName();
                            eld.Users_Id = User.Identity.GetUserId();
                        }
                        eld.IsActive = true;
                        eld.Entity_Id = 14;
                        db.Entity_Lifecycle.Add(eld);
                        db.SaveChanges();
                        hrProfileReq.EntityLifecycle_Id = eld.Id;
                        db.HrProfileReqs.Add(hrProfileReq);
                        db.SaveChanges();
                        transc.Commit();

                        return Ok(new { result = true, hrProfileReq = hrProfileReq });
                    }
                    catch (DbEntityValidationException dbx)
                    {
                        transc.Rollback();
                        return BadRequest(GetDbExMessage(dbx));
                    }
                    catch (Exception ex)
                    {
                        transc.Rollback();
                        return BadRequest(ex.Message);
                    }
                }
            }
        }

        [HttpPost]
        [Route("SaveLeaveApplication")]
        public IHttpActionResult SaveLeaveApplication([FromBody] LeaveApplication leaveApplication)
        {
            using (var db = new HR_System())
            {
                using (var transc = db.Database.BeginTransaction())
                {

                    try
                    {
                        bool toSectionOfficer = leaveApplication.SO_Id != null ? true : false;
                        Entity_Lifecycle eld = new Entity_Lifecycle();
                        eld.Created_Date = DateTime.UtcNow.AddHours(5);
                        if (User.Identity.GetUserName() == null || User.Identity.GetUserId() == null)
                        {
                            eld.Created_By = "OnlineApplicationForm";
                            eld.Users_Id = Convert.ToString(leaveApplication.Profile_Id);
                        }
                        else
                        {
                            eld.Created_By = User.Identity.GetUserName();
                            eld.Users_Id = User.Identity.GetUserId();
                        }
                        eld.IsActive = true;
                        eld.Entity_Id = 14;
                        db.Entity_Lifecycle.Add(eld);
                        db.SaveChanges();
                        leaveApplication.EntitylifeCycle_Id = eld.Id;
                        leaveApplication.LeaveStatus_Id = 1;
                        db.LeaveApplications.Add(leaveApplication);
                        db.SaveChanges();
                        leaveApplication.TrackingNo = (int)leaveApplication.Id + 1022;
                        db.SaveChanges();
                        Image barCode = barCodeZ(Convert.ToInt32(leaveApplication.TrackingNo));
                        string imgSrc = "data:image / jpg; base64," + ImagesUtil.ImageToBase64(barCode, ImageFormat.Jpeg);

                        transc.Commit();
                        string mobileNumber = db.HrProfiles.FirstOrDefault(x => x.Id == leaveApplication.Profile_Id).MobileNo;
                        mobileNumber = mobileNumber.Replace("-", "");
                        List<SMS> smsss = new List<SMS>();
                        SMS sms = new SMS();
                        sms.MobileNumber = mobileNumber;
                        sms.Message = "Your application has been recieved.\nYour Tracking ID: " + leaveApplication.TrackingNo;
                        smsss.Add(sms);
                        if (toSectionOfficer)
                        {
                            string soName = "";
                            soName = db.EsrSectionOfficers.FirstOrDefault(x => x.Id == leaveApplication.SO_Id).Name;
                            sms = new SMS();
                            sms.MobileNumber = mobileNumber;
                            sms.Message = "Your application has been submitted to " + soName + " for further processing.";
                            smsss.Add(sms);
                        }
                        Common.SMS_Send(smsss);
                        return Ok(new { result = true, trackingID = leaveApplication.TrackingNo, imgSrc = imgSrc });
                    }
                    catch (DbEntityValidationException dbx)
                    {
                        transc.Rollback();
                        return BadRequest(GetDbExMessage(dbx));
                    }
                    catch (Exception ex)
                    {
                        transc.Rollback();
                        return BadRequest(ex.Message);
                    }
                }
            }

        }

        [HttpPost]
        [Route("SaveTransferApplication")]
        public IHttpActionResult SaveTransferApplication([FromBody] TransferApplication transferApplication)
        {
            using (var db = new HR_System())
            {
                var trackinID = "";
                using (var transc = db.Database.BeginTransaction())
                {

                    try
                    {

                        Entity_Lifecycle eld = new Entity_Lifecycle();
                        eld.Created_Date = DateTime.UtcNow.AddHours(5);
                        if (User.Identity.GetUserName() == null || User.Identity.GetUserId() == null)
                        {
                            eld.Created_By = "OnlineApplicationForm";
                            eld.Users_Id = Convert.ToString(transferApplication.Profile_Id);
                        }
                        else
                        {
                            eld.Created_By = User.Identity.GetUserName();
                            eld.Users_Id = User.Identity.GetUserId();
                        }
                        eld.IsActive = true;
                        eld.Entity_Id = 14;
                        db.Entity_Lifecycle.Add(eld);
                        db.SaveChanges();
                        transferApplication.EntitylifeCycle_Id = eld.Id;
                        transferApplication.ApplicationStatus_Id = 1;
                        db.TransferApplications.Add(transferApplication);
                        db.SaveChanges();
                        transferApplication.TrackingNo = (int)transferApplication.Id + 100022;
                        db.SaveChanges();
                        Image barCode = barCodeZ(Convert.ToInt32(transferApplication.TrackingNo));
                        string imgSrc = "data:image / jpg; base64," + ImagesUtil.ImageToBase64(barCode, ImageFormat.Jpeg);
                        transc.Commit();
                        string mobileNumber = db.HrProfiles.FirstOrDefault(x => x.Id == transferApplication.Profile_Id).MobileNo;
                        mobileNumber = mobileNumber.Replace("-", "");
                        List<SMS> smsss = new List<SMS>();
                        SMS sms = new SMS()
                        {
                            MobileNumber = mobileNumber,
                            Message = "Your application has been recieved.\nYour Tracking ID: " + transferApplication.TrackingNo
                            //Message = "Application for Transfer has been recieved.\nYour Tracking ID: " + transferApplication.TrackingNo
                        };
                        smsss.Add(sms);
                        Common.SMS_Send(smsss);
                        return Ok(new { result = true, trackingID = transferApplication.TrackingNo, imgSrc = imgSrc });
                    }
                    catch (DbEntityValidationException dbx)
                    {
                        transc.Rollback();
                        return BadRequest(GetDbExMessage(dbx));
                    }
                    catch (Exception ex)
                    {
                        transc.Rollback();
                        return BadRequest(ex.Message);
                    }
                }
            }
        }


        [HttpGet]
        [Route("SendToSO/{eID}/{soId}/{applicationType}")]
        public IHttpActionResult SendToSO(int eID, byte soId, int applicationType)
        {
            using (var db = new HR_System())
            {
                using (var transc = db.Database.BeginTransaction())
                {

                    try
                    {
                        int? profileId = 0;
                        if (applicationType == 1)
                        {
                            LeaveApplication la = db.LeaveApplications.FirstOrDefault(x => x.Id == eID);
                            la.SO_Id = soId;
                            profileId = la.Profile_Id;
                            db.Entry(la).State = EntityState.Modified;
                            db.SaveChanges();
                            transc.Commit();

                        }
                        else if (applicationType == 2)
                        {
                            TransferApplication ta = db.TransferApplications.FirstOrDefault(x => x.Id == eID);
                            ta.SO_Id = soId;
                            profileId = ta.Profile_Id;
                            db.Entry(ta).State = EntityState.Modified;
                            db.SaveChanges();
                            transc.Commit();


                        }
                        string soName = "";
                        if (profileId != 0)
                        {
                            soName = db.EsrSectionOfficers.FirstOrDefault(x => x.Id == soId).Name;
                            string mobileNumber = db.HrProfiles.FirstOrDefault(x => x.Id == profileId).MobileNo;
                            mobileNumber = mobileNumber.Replace("-", "");
                            List<SMS> smsss = new List<SMS>();
                            //" + (applicationType == 1 ? "leave" : "transfer") + "
                            SMS sms = new SMS()
                            {
                                MobileNumber = mobileNumber,
                                Message = "Your application have been submitted to " + soName + " for further processing."
                            };
                            smsss.Add(sms);
                            Common.SMS_Send(smsss);
                        }

                        return Ok(new { result = true, soName = soName });
                    }
                    catch (DbEntityValidationException dbx)
                    {
                        transc.Rollback();
                        return BadRequest(GetDbExMessage(dbx));
                    }
                    catch (Exception ex)
                    {
                        transc.Rollback();
                        return BadRequest(ex.Message);
                    }
                }
            }
        }
        [Route("UpdateApplicationStatus/{entityId}/{statusId}/{applicationType}")]
        [HttpGet]
        public IHttpActionResult UpdateApplicationStatus(int entityId, int statusId, int applicationType)
        {
            try
            {
                using (var db = new HR_System())
                {
                    int? profileId = 0;
                    if (applicationType == 1)
                    {
                        LeaveApplication la = db.LeaveApplications.FirstOrDefault(x => x.Id == entityId);
                        la.LeaveStatus_Id = statusId;
                        profileId = la.Profile_Id;
                        db.Entry(la).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else if (applicationType == 2)
                    {
                        TransferApplication ta = db.TransferApplications.FirstOrDefault(x => x.Id == entityId);
                        ta.ApplicationStatus_Id = statusId;
                        profileId = ta.Profile_Id;
                        db.Entry(ta).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    if (profileId != 0)
                    {
                        string mobileNumber = db.HrProfiles.FirstOrDefault(x => x.Id == profileId).MobileNo;
                        mobileNumber = mobileNumber.Replace("-", "");
                        //" + (applicationType == 1 ? "leave" : "transfer") + "
                        List<SMS> smsss = new List<SMS>();
                        SMS sms = new SMS()
                        {
                            MobileNumber = mobileNumber,
                            Message = "Your application has been approved."
                        };
                        smsss.Add(sms);
                        Common.SMS_Send(smsss);
                    }
                    return Ok(new { result = true });
                }
            }
            catch (DbEntityValidationException dbx)
            {
                string message = GetDbExMessage(dbx);
                return BadRequest(message);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [Route("ForwardApplication/{entityId}/{officerLevel}/{toId}/{applicationType}")]
        [HttpGet]
        public IHttpActionResult ForwardApplication(int entityId, int officerLevel, int toId, int applicationType)
        {
            try
            {
                using (var db = new HR_System())
                {
                    int? profileId = 0;
                    if (applicationType == 1)
                    {
                        LeaveApplication la = db.LeaveApplications.FirstOrDefault(x => x.Id == entityId);
                        if (officerLevel == OfficerLevels.FrontDesk) la.SO_Id = (byte)toId;
                        if (officerLevel == OfficerLevels.SectionOfficer) la.DS_Id = toId;
                        if (officerLevel == OfficerLevels.DeputySecretary) la.AS_Id = toId;
                        if (officerLevel == OfficerLevels.AdditionalSecretary) la.Secretary = true;
                        profileId = la.Profile_Id;
                        db.Entry(la).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else if (applicationType == 2)
                    {
                        TransferApplication ta = db.TransferApplications.FirstOrDefault(x => x.Id == entityId);
                        if (officerLevel == OfficerLevels.FrontDesk) ta.SO_Id = (byte)toId;
                        if (officerLevel == OfficerLevels.SectionOfficer) ta.DS_Id = toId;
                        if (officerLevel == OfficerLevels.DeputySecretary) ta.AS_Id = toId;
                        if (officerLevel == OfficerLevels.AdditionalSecretary) ta.Secretary = true;
                        profileId = ta.Profile_Id;
                        db.Entry(ta).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    //string officerName = "";
                    if (profileId != 0)
                    {
                        string mobileNumber = db.HrProfiles.FirstOrDefault(x => x.Id == profileId).MobileNo;
                        mobileNumber = mobileNumber.Replace("-", "");
                        List<SMS> smsss = new List<SMS>();
                        //" + (applicationType == 1 ? "leave" : "transfer") + "
                        SMS sms = new SMS()
                        {
                            MobileNumber = mobileNumber,
                            Message = "Your application has been forwarded to " + (officerLevel == 0 ? "Section Officer" : officerLevel == 1 ? "Deputy Secretary" : officerLevel == 2 ? "Additional Secretary" : officerLevel == 3 ? " Secretary" : "") + " for further processing."
                        };
                        smsss.Add(sms);
                        Common.SMS_Send(smsss);
                    }
                    return Ok(new { result = true });
                }
            }
            catch (DbEntityValidationException dbx)
            {
                string message = GetDbExMessage(dbx);
                return BadRequest(message);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        public Image barCodeZ(long Id)
        {
            Zen.Barcode.Code128BarcodeDraw barcode = Zen.Barcode.BarcodeDrawFactory.Code128WithChecksum;
            return barcode.Draw(Convert.ToString(Id), 100, 2);
        }
        #endregion
        #region Helpers
        private string GetDbExMessage(DbEntityValidationException dbx)
        {
            return dbx.EntityValidationErrors.SelectMany(validationErrors => validationErrors.ValidationErrors)
                .Aggregate("",
                    (current, validationError) =>
                        current +
                        $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
        }
        [HttpGet]
        [Route("getTotalDays/{startDate}/{endDate}")]
        public IHttpActionResult getTotalDays(string startDate, string endDate)
        {
            string[] startDateSplit = startDate.Split('-');
            string[] endDateSplit = endDate.Split('-');
            DateTime st = new DateTime(Convert.ToInt32(startDateSplit[0]),
                Convert.ToInt32(startDateSplit[1]),
                Convert.ToInt32(startDateSplit[2]));
            DateTime tt = new DateTime(Convert.ToInt32(endDateSplit[0]),
                Convert.ToInt32(endDateSplit[1]),
                Convert.ToInt32(endDateSplit[2]));
            return Ok((tt - st).Days + 1);
        }
        #endregion
    }
}
