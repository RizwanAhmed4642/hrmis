using Hrmis.Models.Common;
using Hrmis.Models.CustomModels;
using Hrmis.Models.DbModel;
using Hrmis.Models.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Hrmis.Controllers.HrmisRestApi
{
    [Authorize]
    [RoutePrefix("api/OrderNotification")]
    public class OrderNotificationController : ApiController
    {
        private ApplicationService _applicationService;

        [Route("GetESR/{id}/{type}")]
        [HttpGet]
        public IHttpActionResult GetESR(int id, int type)
        {
            try
            {
                using (var db = new HR_System())
                {
                    OrderTracking orderTracking = new OrderTracking();
                    orderTracking.Type = type;
                    if (type == 1)
                    {
                        var esr = db.ESRLatestViews.FirstOrDefault(x => x.Id == id);
                        if (esr != null)
                        {
                            orderTracking.ESR_Id = id;
                            orderTracking.IsActive = true;
                            orderTracking.Datetime = DateTime.UtcNow.AddHours(5);
                            orderTracking.Username = User.Identity.GetUserName();
                            orderTracking.UserId = User.Identity.GetUserId();
                            db.OrderTrackings.Add(orderTracking);
                            db.SaveChanges();
                            return Ok(esr);
                        }
                        else
                        {
                            return Ok(false);
                        }
                    }
                    else if (type == 2)
                    {
                        var leaveOrder = db.LeaveOrderViews.FirstOrDefault(x => x.Id == id);
                        if (leaveOrder != null)
                        {
                            orderTracking.ELR_Id = id;
                            orderTracking.IsActive = true;
                            orderTracking.Datetime = DateTime.UtcNow.AddHours(5);
                            orderTracking.Username = User.Identity.GetUserName();
                            orderTracking.UserId = User.Identity.GetUserId();
                            db.OrderTrackings.Add(orderTracking);
                            db.SaveChanges();
                            return Ok(leaveOrder);
                        }
                        else
                        {
                            return Ok(false);
                        }
                    }
                    return Ok(false);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("GetESRReport/{district}/{transferTypeId}/{currentPage}/{itemsPerPage}")]
        [HttpPost]
        public IHttpActionResult GetESRReport([FromBody] ProfileQuery query, string district, int transferTypeId, int currentPage = 1, int itemsPerPage = 100)
        {

            try
            {
                using (var db = new HR_System())
                {

                    string userId = User.Identity.GetUserId();
                    string userName = User.Identity.GetUserName();
                    db.Configuration.ProxyCreationEnabled = false;

                    List<OrderAllView> report = new List<OrderAllView>();
                    IQueryable<OrderAllView> reportQuery = null;
                    int totalRecords = 0;


                    if (currentPage <= 0) currentPage = 1;
                    if (itemsPerPage <= 0) currentPage = 100;


                    if (userName.Equals("dpd") || User.IsInRole("Deputy Secretary"))
                    {
                        if (district.Equals("0") && transferTypeId == 0)
                        {
                            reportQuery = db.OrderAllViews.Where(x => x.IsActive == true).OrderByDescending(x => x.DateTime).AsQueryable();
                        }
                        else if (district.Equals("0") && transferTypeId != 0)
                        {
                            reportQuery = db.OrderAllViews.Where(x => x.TransferTypeID == transferTypeId && x.IsActive == true).OrderByDescending(x => x.DateTime).AsQueryable();
                        }
                        else if (!district.Equals("0") && transferTypeId == 0)
                        {
                            reportQuery = db.OrderAllViews.Where(x => x.UserName.Equals(district) && x.IsActive == true).OrderByDescending(x => x.DateTime).AsQueryable();
                        }
                        else if (!district.Equals("0") && transferTypeId != 0)
                        {
                            reportQuery = db.OrderAllViews.Where(x => x.UserName.Equals(district) && x.TransferTypeID == transferTypeId && x.IsActive == true).OrderByDescending(x => x.DateTime).AsQueryable();
                        }
                        else
                        {
                            return Ok(false);
                        }
                    }
                    else if (userName.StartsWith("sdp") || User.IsInRole("SDP"))
                    {
                        if (transferTypeId == 0)
                        {
                            reportQuery = db.OrderAllViews.Where(x => x.DistrictCode.StartsWith(district) && x.DistrictCode != null && x.IsActive == true).OrderByDescending(x => x.DateTime).AsQueryable();
                        }
                        else if (transferTypeId != 0)
                        {
                            reportQuery = db.OrderAllViews.Where(x => x.DistrictCode.StartsWith(district) && x.DistrictCode != null && x.TransferTypeID == transferTypeId && x.IsActive == true).OrderByDescending(x => x.DateTime).AsQueryable();
                        }
                        else
                        {
                            return Ok(false);
                        }
                    }
                    else
                    {
                        if (district.Equals("0") && transferTypeId == 0)
                        {
                            reportQuery = db.OrderAllViews.Where(x => x.IsActive == true && x.UserId.Equals(userId)).OrderByDescending(x => x.DateTime).AsQueryable();
                        }
                        else if (district.Equals("0") && transferTypeId != 0)
                        {
                            reportQuery = db.OrderAllViews.Where(x => x.UserName.Equals(district) && x.IsActive == true && x.UserId.Equals(userId)).OrderByDescending(x => x.DateTime).AsQueryable();
                        }
                        else if (!district.Equals("0") && transferTypeId == 0)
                        {
                            reportQuery = db.OrderAllViews.Where(x => x.UserName.Equals(district) && x.IsActive == true && x.UserId.Equals(userId)).OrderByDescending(x => x.DateTime).AsQueryable();
                        }
                        else if (!district.Equals("0") && transferTypeId != 0)
                        {
                            reportQuery = db.OrderAllViews.Where(x => x.UserName.Equals(district) && x.TransferTypeID == transferTypeId && x.IsActive == true && x.UserId.Equals(userId)).OrderByDescending(x => x.DateTime).AsQueryable();
                        }
                        else
                        {
                            return Ok(false);
                        }
                    }

                    if (!string.IsNullOrEmpty(query?.Query))
                    {
                        reportQuery = reportQuery.Where(x =>
                                                            x.EmployeeName.ToLower().Contains(query.Query.ToLower())
                                                        || x.CNIC.Equals(query.Query)
                                                        || x.OrderHTML.ToLower().Contains(query.Query.ToLower())
                                                        || x.Designation.Contains(query.Query));
                    }
                    reportQuery = reportQuery.Where(x => !string.IsNullOrEmpty(x.OrderHTML));
                    totalRecords = reportQuery.Count();
                    report = reportQuery.Skip((currentPage - 1) * itemsPerPage).Take(itemsPerPage).OrderByDescending(x => x.DateTime).ToList();

                    return Ok(new
                    {
                        esrlist = report,
                        totalRecords
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("RemoveLeaveorder/{Id}")]
        [HttpGet]
        public IHttpActionResult RemoveLeaveorder(int Id)
        {
            using (var db = new HR_System())
            {
                try
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    LeaveOrder lo = db.LeaveOrders.FirstOrDefault(x => x.Id == Id);
                    if (lo != null)
                    {
                        Entity_Lifecycle elc = db.Entity_Lifecycle.FirstOrDefault(x => x.Id == lo.EntityLifecycle_Id);
                        if (elc != null)
                        {
                            elc.IsActive = false;
                            db.Entry(elc).State = EntityState.Modified;
                            db.SaveChanges();
                            Entity_Modified_Log eml = new Entity_Modified_Log();
                            eml.Modified_By = User.Identity.GetUserId();
                            eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                            eml.Entity_Lifecycle_Id = (long)lo.EntityLifecycle_Id;
                            eml.Description = "Leave Order Removed by " + User.Identity.GetUserName();
                            db.Entity_Modified_Log.Add(eml);
                            db.SaveChanges();
                            return Ok(true);
                        }
                        else
                        {
                            return Ok(false);
                        }
                    }
                    else
                    {
                        return Ok(false);
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

        }
        [Route("RemoveESR/{Id}")]
        [HttpGet]
        public IHttpActionResult RemoveESR(int Id)
        {
            using (var db = new HR_System())
            {
                try
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    ESR esr = db.ESRs.FirstOrDefault(x => x.Id == Id);
                    if (esr != null)
                    {
                        Entity_Lifecycle elc = db.Entity_Lifecycle.FirstOrDefault(x => x.Id == esr.EntityLifecycle_Id);
                        if (elc != null)
                        {
                            elc.IsActive = false;
                            db.Entry(elc).State = EntityState.Modified;
                            db.SaveChanges();
                            Entity_Modified_Log eml = new Entity_Modified_Log();
                            eml.Modified_By = User.Identity.GetUserId();
                            eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                            eml.Entity_Lifecycle_Id = (long)esr.EntityLifecycle_Id;
                            eml.Description = "Service Order Removed by " + User.Identity.GetUserName();
                            db.Entity_Modified_Log.Add(eml);
                            db.SaveChanges();
                            return Ok(true);
                        }
                        else
                        {
                            return Ok(false);
                        }
                    }
                    else
                    {
                        return Ok(false);
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

        }
        [Route("SaveOrderProfile")]
        [HttpPost]
        public async Task<IHttpActionResult> SaveOrderProfile(HrProfile hrProfile)
        {
            using (var db = new HR_System())
            {
                try
                {
                    if (hrProfile.Id > 0)
                    {
                        db.Configuration.ProxyCreationEnabled = false;
                        var dbProfile = db.HrProfiles.FirstOrDefault(x => x.Id == hrProfile.Id);
                        if (dbProfile != null)
                        {

                            dbProfile.DateOfBirth = hrProfile.DateOfBirth;
                            dbProfile.FileNumber = hrProfile.FileNumber;
                            dbProfile.MobileNo = hrProfile.MobileNo;
                            dbProfile.EMaiL = hrProfile.EMaiL;

                            if (dbProfile.EntityLifecycle_Id == null)
                            {
                                Entity_Lifecycle elc = new Entity_Lifecycle();
                                elc.Created_Date = DateTime.UtcNow.AddHours(5);
                                elc.Created_By = User.Identity.GetUserName() + " (added after migration)";
                                elc.Users_Id = User.Identity.GetUserId();
                                elc.IsActive = true;
                                elc.Entity_Id = 9;
                                db.Entity_Lifecycle.Add(elc);
                                db.SaveChanges();
                                dbProfile.EntityLifecycle_Id = elc.Id;
                            }
                            Entity_Modified_Log eml = new Entity_Modified_Log();
                            eml.Modified_By = User.Identity.GetUserId(); ;
                            eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                            eml.Entity_Lifecycle_Id = (long)dbProfile.EntityLifecycle_Id;
                            eml.Description = "Profile Updated By " + User.Identity.GetUserName();
                            db.Entity_Modified_Log.Add(eml);
                            db.SaveChanges();

                            db.Entry(dbProfile).State = EntityState.Modified;
                            db.SaveChanges();
                            return Ok(dbProfile);
                        }
                    }
                    return Ok(false);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }

            }
        }
        [Route("UploadSignedOrder")]
        [HttpPost]
        public async Task<IHttpActionResult> UploadSignedOrder(EsrSigned esrSigned)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    if (esrSigned.ESR_Id != 0 || esrSigned.ELR_Id != 0)
                    {
                        ESRView esr = null;
                        LeaveOrderView elr = null;
                        ProfileDetailsView profile = null;
                        AppOrder appOrder = null;
                        ApplicationLog applicationLog = new ApplicationLog();
                        if (esrSigned.ESR_Id > 0)
                        {
                            esr = db.ESRViews.FirstOrDefault(x => x.Id == esrSigned.ESR_Id);
                            profile = db.ProfileDetailsViews.FirstOrDefault(x => x.Id == esr.Profile_Id);
                            esrSigned.BarcodeNo = "ESR-" + esr.Id;
                            appOrder = db.AppOrders.FirstOrDefault(x => x.ESR_Id == esrSigned.ESR_Id);
                        }
                        if (esrSigned.ELR_Id > 0)
                        {
                            esrSigned.BarcodeNo = "ELR-" + esrSigned.ELR_Id;
                            elr = db.LeaveOrderViews.FirstOrDefault(x => x.Id == esrSigned.ELR_Id);
                            profile = db.ProfileDetailsViews.FirstOrDefault(x => x.Id == elr.Profile_Id);
                            appOrder = db.AppOrders.FirstOrDefault(x => x.ELR_Id == esrSigned.ELR_Id);
                        }
                        if (profile != null)
                        {
                            esrSigned.Profile_Id = profile.Id;
                        }
                        esrSigned.Created_Date = DateTime.UtcNow.AddHours(5);
                        esrSigned.Created_By = User.Identity.GetUserId();
                        esrSigned.User_Id = User.Identity.GetUserName();
                        esrSigned.IsActive = true;
                        db.EsrSigneds.Add(esrSigned);
                        await db.SaveChangesAsync();
                        if (appOrder != null)
                        {
                            var application = db.ApplicationViews.FirstOrDefault(x => x.TrackingNumber == appOrder.TrackingNo);
                            if (application != null)
                            {
                                applicationLog.Application_Id = application.Id;
                                applicationLog.SMS_SentToApplicant = true;
                                applicationLog.ToStatus_Id = 4;
                                applicationLog.Remarks = esrSigned.Link;
                                _applicationService = new ApplicationService();
                                _applicationService.CreateApplicationLog(applicationLog, User.Identity.GetUserName(), User.Identity.GetUserId());
                            }
                        }


                        if (profile != null)
                        {
                            SendOrderSMS(profile, esrSigned, esrSigned.BarcodeNo, "", "0");
                        }
                        var serviceHistory = db.HrServiceHistories.OrderByDescending(k => k.Id).FirstOrDefault(x => x.Profile_Id == profile.Id && x.ESR_Id == esrSigned.ESR_Id);
                        if (serviceHistory != null && esrSigned.ESR_Id != null)
                        {
                            serviceHistory.OrderFilePath = esrSigned.Link;
                            db.Entry(serviceHistory).State = EntityState.Modified;
                            db.SaveChanges();

                            Entity_Modified_Log eml2 = new Entity_Modified_Log();
                            eml2.Modified_By = User.Identity.GetUserId();
                            eml2.Modified_Date = DateTime.UtcNow.AddHours(5);
                            eml2.Entity_Lifecycle_Id = (long)serviceHistory.EntityLifecycle_Id;
                            eml2.Description = "Service History Modified: Order File Uploaded by " + User.Identity.GetUserName();
                            db.Entity_Modified_Log.Add(eml2);
                            db.SaveChanges();
                        }
                        return Ok(esrSigned);
                    }
                    return Ok(false);
                }
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("GenerateOrderRequest")]
        [HttpPost]
        public async Task<IHttpActionResult> GenerateOrderRequest(PHFMCOrder pHFMCOrder)
        {
            using (var db = new HR_System())
            {
                try
                {
                    if (pHFMCOrder.Id == 0 && pHFMCOrder.ProfileId > 0)
                    {
                        db.Configuration.ProxyCreationEnabled = false;

                        var pOrder = db.PHFMCOrders.FirstOrDefault(x => x.ProfileId == pHFMCOrder.ProfileId && x.IsActive == true);
                        if (pOrder != null)
                        {
                            return Ok("Order Request Already Generated!");
                        }

                        pHFMCOrder.RequestedBy = User.Identity.GetUserName();
                        pHFMCOrder.RequestedByUserId = User.Identity.GetUserId();
                        pHFMCOrder.RequestTime = DateTime.UtcNow.AddHours(5);
                        pHFMCOrder.IsApproved = false;
                        pHFMCOrder.IsActive = true;

                        db.PHFMCOrders.Add(pHFMCOrder);
                        db.SaveChanges();
                        return Ok("Order request submitted to PHFMC head office");
                    }
                    return Ok(false);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }

            }
        }
        [Route("GetOrderRequests")]
        [HttpPost]
        public IHttpActionResult GetOrderRequests([FromBody] OrderFilters filter)
        {
            using (var db = new HR_System())
            {
                try
                {

                    db.Configuration.ProxyCreationEnabled = false;
                    var query = db.PHFMCOrderViews.Where(x => x.IsActive == true).AsQueryable();
                    if (!string.IsNullOrEmpty(filter.Query))
                    {
                        query = query.Where(x => x.EmployeeName.Contains(filter.Query)).AsQueryable();
                    }
                    if (!string.IsNullOrEmpty(filter.CNIC))
                    {
                        query = query.Where(x => x.CNIC.Equals(filter.CNIC)).AsQueryable();
                    }
                    if (filter.Approved == true)
                    {
                        query = query.Where(x => x.IsApproved == true).AsQueryable();
                    }
                    var count = query.Count();
                    var list = query.OrderByDescending(x => x.RequestTime).Skip(filter.Skip).Take(filter.PageSize).ToList();
                    return Ok(new TableResponse<PHFMCOrderView>() { Count = count, List = list });
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }

            }
        }
        [Route("ApproveOrderRequest/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> ApproveOrderRequest(int id)
        {
            using (var db = new HR_System())
            {
                try
                {

                    db.Configuration.ProxyCreationEnabled = false;
                    var pHFMCOrder = db.PHFMCOrders.FirstOrDefault(x => x.Id == id && x.IsActive == true);
                    if (pHFMCOrder != null)
                    {
                        pHFMCOrder.ApprovalBy = User.Identity.GetUserName();
                        pHFMCOrder.ApprovalByUserId = User.Identity.GetUserId();
                        pHFMCOrder.ApprovalTime = DateTime.UtcNow.AddHours(5);
                        pHFMCOrder.IsApproved = true;
                        db.Entry(pHFMCOrder).State = EntityState.Modified;
                        db.SaveChanges();
                        return Ok(pHFMCOrder);
                    }
                    return Ok(false);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }

            }
        }
        [Route("EditOrderRequest/{orderId}/{orderRequestId}")]
        [HttpGet]
        public IHttpActionResult EditOrderRequest(int orderId, int orderRequestId)
        {
            using (var db = new HR_System())
            {
                try
                {

                    db.Configuration.ProxyCreationEnabled = false;
                    var pHFMCOrder = db.PHFMCOrders.FirstOrDefault(x => x.Id == orderRequestId && x.IsActive == true);
                    if (pHFMCOrder != null)
                    {
                        pHFMCOrder.ESRId = orderId;
                        pHFMCOrder.OrderBy = User.Identity.GetUserName();
                        pHFMCOrder.OrderByUserId = User.Identity.GetUserId();
                        pHFMCOrder.OrderTime = DateTime.UtcNow.AddHours(5);
                        pHFMCOrder.OrderGenerated = true;
                        db.Entry(pHFMCOrder).State = EntityState.Modified;
                        db.SaveChanges();
                        return Ok(pHFMCOrder);
                    }
                    return Ok(false);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }

            }
        }
        [Route("CheckOrderRequest/{profileId}")]
        [HttpGet]
        public IHttpActionResult CheckOrderRequest(int profileId)
        {
            using (var db = new HR_System())
            {
                try
                {

                    db.Configuration.ProxyCreationEnabled = false;
                    var pHFMCOrder = db.PHFMCOrders.FirstOrDefault(x => x.ProfileId == profileId && x.IsActive == true);
                    return Ok(pHFMCOrder);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }

            }
        }

        [Route("UploadReqeustedSignedCopy/{orderId}/{orderRequestId}")]
        public async Task<bool> UploadReqeustedSignedCopy(int orderId, int orderRequestId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    var pHFMCOrder = _db.PHFMCOrders.FirstOrDefault(x => x.Id == orderRequestId && x.IsActive == true);
                    if (pHFMCOrder == null) { return false; }
                    if (!Request.Content.IsMimeMultipartContent())
                        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                    string RootPath = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\Orders\PHFMCOrders\";
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
                        filename = Guid.NewGuid().ToString() + "_" + orderId + "." + FileExtension;

                        string mkey = key.Split('_').LastOrDefault().Trim('\"');
                        pHFMCOrder.SignedOrderTime = DateTime.UtcNow.AddHours(5);
                        pHFMCOrder.SignedOrderPath = @"Uploads\Orders\PHFMCOrders\" + filename;
                        pHFMCOrder.SignedByUser = User.Identity.GetUserName();
                        pHFMCOrder.SignedByUserId = User.Identity.GetUserId();
                        pHFMCOrder.IsSigned = true;

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
                        _db.Entry(pHFMCOrder).State = EntityState.Modified;
                        _db.SaveChanges();
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


        private void SendOrderSMS(ProfileDetailsView EsrProfile, EsrSigned esrSigned, string barcode, string type, string hfmisCode)
        {
            if (hfmisCode.Length == 1)
            {
                if (EsrProfile.MobileNo != null && EsrProfile.MobileNo != "")
                {
                    string MessageBody = @"Dear, " + EsrProfile.EmployeeName + "\nCNIC: " + Common.DashifyCNIC(EsrProfile.CNIC) + "\n\nYour order has been uploaded" + "\nBarcode Number: " + barcode.ToString() + "\n\nDownload Link: " + esrSigned.Link.ToString();
                    //SMS sms = new SMS()
                    //{
                    //    MobileNumber = EsrProfile.MobileNo,
                    //    Message = MessageBody

                    //};
                    //sms.Status = await (await Common.SMS_Send(sms)).Content.ReadAsStringAsync();
                    //SaveMessageLog(sms, "SMS");
                    SMS sms = new SMS()
                    {
                        UserId = User.Identity.GetUserId(),
                        FKId = (int)(esrSigned.ESR_Id == null ? esrSigned.ELR_Id : esrSigned.ESR_Id),
                        MobileNumber = EsrProfile.MobileNo,
                        Message = MessageBody
                    };
                    Thread t = new Thread(() => Common.SendSMSTelenor(sms));
                    t.Start();
                }
            }
            else if (hfmisCode.Length == 6)
            {
                if (EsrProfile.MobileNo != null && EsrProfile.MobileNo != "" && EsrProfile.District != null)
                {
                    string MessageBody = @"Dear " + EsrProfile.EmployeeName + "\n(CNIC:" + Common.DashifyCNIC(EsrProfile.CNIC) + ")\n Your order is under process at District Health Authority, " + EsrProfile.District + ".\nPlease visit http://pshealth.punjab.gov.pk/ for further details.";
                    List<SMS> smsy = new List<SMS>();
                    //SMS sms = new SMS()
                    //{
                    //    MobileNumber = EsrProfile.MobileNo,
                    //    Message = MessageBody
                    //};
                    //smsy.Add(sms);
                    //Common.SMS_Send(smsy);
                    SMS sms = new SMS()
                    {
                        UserId = User.Identity.GetUserId(),
                        FKId = (int)esrSigned.ESR_Id,
                        MobileNumber = EsrProfile.MobileNo,
                        Message = MessageBody
                    };
                    Thread t = new Thread(() => Common.SendSMSTelenor(sms));
                    t.Start();
                }
            }
        }
        private void CreateDirectoryIfNotExists(string dirPath) { if (!Directory.Exists(dirPath)) { Directory.CreateDirectory(dirPath); } }

        private void SaveMessageLog(SMS sms, string type)
        {
            try
            {
                using (var db = new HR_System())
                {
                    db.MessageLogs.Add(new MessageLog() { FKId = sms.FKId.ToString(), MessageType = type, Response = sms.Status, SentDate = DateTime.UtcNow.AddHours(5), SentFrom = sms.From, SentTo = sms.MobileNumber, Text = sms.Message });
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }
        }
        public class OrderFilters : Paginator
        {
            public string Query { get; set; }
            public string CNIC { get; set; }
            public bool Approved { get; set; }
        }
    }
}
