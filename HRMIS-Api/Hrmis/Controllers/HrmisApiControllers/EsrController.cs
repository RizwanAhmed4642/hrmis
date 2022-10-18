using Hrmis.Models.Common;
using Hrmis.Models.DbModel;
using Hrmis.Models.ImageProcessor;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Hrmis.Models.Extensions;
using Hrmis.Models.Services;
using Zen.Barcode;

namespace Hrmis.Controllers.HrmisApiControllers
{
    [System.Web.Http.RoutePrefix("api/ESR")]
    public class EsrController : ApiController
    {

        private HR_System db;
        private TransferPostingService _transferPostingService;

        public EsrController()
        {
            db = new HR_System();
            _transferPostingService = new TransferPostingService();
        }

        [System.Web.Http.Route("Get/{id}")]
        [System.Web.Http.HttpGet]
        public async Task<IHttpActionResult> Get(long id)
        {
            try
            {

                db.Configuration.ProxyCreationEnabled = false;
                var esr = db.ESRViews.FirstOrDefault(x => x.Id == id);

                Image barCode = barCodeZ(esr.Id);
                string imgSrc = "data:image/jpg;base64," + ImagesUtil.ImageToBase64(barCode, ImageFormat.Jpeg);
                var result = new
                {
                    forwardToOfficers = db.EsrForwardToOfficers.Where(x => x.EsrID == id).ToList(),
                    esr = esr,
                    profile = db.ProfileThumbViews.FirstOrDefault(x => x.Id == esr.Profile_Id),
                    imgSrc = imgSrc
                };
                return Json(JsonConvert.SerializeObject(result));

            }
            catch (DbEntityValidationException dbx)
            {
                string message =
                   dbx.EntityValidationErrors.SelectMany(validationErrors => validationErrors.ValidationErrors)
                       .Aggregate("",
                           (current, validationError) =>
                               current +
                               $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                return null;

            }



        }

        [Route("Exists/{ID}")]
        [HttpGet]
        public async Task<IHttpActionResult> Exists(string ID)
        {
            try
            {
                Int64 IDinti = Convert.ToInt64(ID);
                string userName = User.Identity.GetUserName();
                if (userName == "dpd")
                {
                    return Ok(db.ESRViews.Where(x => x.Id == IDinti).Count());

                }
                else
                {
                    return Ok(db.ESRViews.Where(x => x.Id == IDinti && x.Created_By == userName).Count());
                }

            }
            catch (DbEntityValidationException dbx)
            {
                string message =
                   dbx.EntityValidationErrors.SelectMany(validationErrors => validationErrors.ValidationErrors)
                       .Aggregate("",
                           (current, validationError) =>
                               current +
                               $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                return null;

            }


        }

        [Route("SubmitLeave/{hfmisCode}")]
        [HttpPost]
        public IHttpActionResult SubmitLeave([FromBody] LeaveOrder leaveOrder, string hfmisCode)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;

                if (leaveOrder.Id == 0)
                {
                    if (leaveOrder.FromDate != null)
                    {
                        leaveOrder.FromDate = leaveOrder.FromDate.Value.Hour == 19 ? leaveOrder.FromDate.Value.AddHours(5) : leaveOrder.FromDate.Value;
                    }
                    if (leaveOrder.ToDate != null)
                    {
                        leaveOrder.ToDate = leaveOrder.ToDate.Value.Hour == 19 ? leaveOrder.ToDate.Value.AddHours(5) : leaveOrder.ToDate.Value;
                    }
                    if (leaveOrder.OrderDate != null)
                    {
                        leaveOrder.OrderDate = leaveOrder.OrderDate.Value.Hour == 19 ? leaveOrder.OrderDate.Value.AddHours(5) : leaveOrder.OrderDate.Value;

                    }
                    // move profile to PSHD if leave if more than 89 days and make seat vacant there 
                    if (leaveOrder.TotalDays > 89 && (leaveOrder.LeaveType_Id != 7 || leaveOrder.LeaveType_Id != 17 || leaveOrder.LeaveType_Id != 21 || leaveOrder.LeaveType_Id != 28))
                    {
                        HrProfile profile = db.HrProfiles.FirstOrDefault(x => x.Id == leaveOrder.Profile_Id);

                        _transferPostingService.UpdateVacancy(db, false, profile.HfmisCode, profile.Designation_Id, profile.EmpMode_Id, User.Identity.GetUserName(), User.Identity.GetUserId());

                        profile.HfmisCode = "0350020010030010002";
                        profile.HealthFacility_Id = 11606;

                        profile.WorkingHFMISCode = "0350020010030010002";
                        profile.WorkingHealthFacility_Id = 11606;

                        profile.Status_Id = 17;

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
                        Entity_Modified_Log emlProfile = new Entity_Modified_Log();
                        emlProfile.Modified_By = User.Identity.GetUserId();
                        emlProfile.Modified_Date = DateTime.UtcNow.AddHours(5);
                        emlProfile.Entity_Lifecycle_Id = (long)profile.EntityLifecycle_Id;
                        emlProfile.Description = "Profile edited during order generation By " + User.Identity.GetUserName();
                        db.Entity_Modified_Log.Add(emlProfile);

                        db.SaveChanges();

                    }
                    Entity_Lifecycle eld = new Entity_Lifecycle();
                    eld.Created_Date = DateTime.UtcNow.AddHours(5);
                    eld.Created_By = User.Identity.GetUserName();
                    eld.Users_Id = User.Identity.GetUserId();
                    eld.IsActive = true;
                    eld.Entity_Id = 59;
                    db.Entity_Lifecycle.Add(eld);
                    db.SaveChanges();
                    leaveOrder.EntityLifecycle_Id = eld.Id;
                    db.LeaveOrders.Add(leaveOrder);
                }
                db.SaveChanges();
                Code128BarcodeDraw barcode = BarcodeDrawFactory.Code128WithChecksum;
                BarcodeSymbology s = BarcodeSymbology.Code39NC;
                BarcodeDraw drawObject = BarcodeDrawFactory.GetSymbology(s);
                var metrics = drawObject.GetDefaultMetrics(50);
                metrics.Scale = 1;
                Image barCode = barcode.Draw("ELR-" + (leaveOrder.Id + 1003), metrics);
                string imgSrc = "data:image/jpg;base64," + ImagesUtil.ImageToBase64(barCode, ImageFormat.Jpeg);

                try
                {
                    var profileView = db.ProfileDetailsViews.FirstOrDefault(x => x.Id == leaveOrder.Profile_Id);
                    if (profileView != null)
                    {
                        SendSMS(profileView, hfmisCode);
                    }
                }
                catch (Exception e)
                {

                }

                return Ok(new { imgSrc = imgSrc, leaveOrder = leaveOrder });
            }
            catch (DbEntityValidationException dbx)
            {
                string message =
                   dbx.EntityValidationErrors.SelectMany(validationErrors => validationErrors.ValidationErrors)
                       .Aggregate("",
                           (current, validationError) =>
                               current +
                               $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                return BadRequest(message);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [Route("UpdateLeaveOrderHTML")]
        [HttpPost]
        public IHttpActionResult UpdateLeaveOrderHTML([FromBody] LeaveOrder leaveOrder)
        {
            try
            {
                LeaveOrder tempLeaveOrder = null;
                if (leaveOrder.Id != 0)
                {
                    db.Configuration.ProxyCreationEnabled = false;
                    tempLeaveOrder = db.LeaveOrders.FirstOrDefault(x => x.Id == leaveOrder.Id);
                    //Entity_Lifecycle elc = db.Entity_Lifecycle.Find(tempLeaveOrder.EntityLifecycle_Id);
                    //if (elc.Created_Date.Day == DateTime.Now.Day)
                    //{
                    //    elc.Last_Modified_By = User.Identity.GetUserName();
                    //    db.Entry(elc).State = EntityState.Modified;
                    //}
                    //else
                    //{
                    //    Entity_Modified_Log eml = new Entity_Modified_Log();
                    //    eml.Modified_By = User.Identity.GetUserId();
                    //    eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                    //    eml.Description = "Leave Order Updated / Modified by " + User.Identity.GetUserName();
                    //    eml.Entity_Lifecycle_Id = elc.Id;
                    //    db.Entity_Modified_Log.Add(eml);
                    //}
                    if (tempLeaveOrder != null)
                    {
                        tempLeaveOrder.OrderHTML = leaveOrder.OrderHTML;
                        db.Entry(tempLeaveOrder).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                return Ok(tempLeaveOrder);
            }
            catch (DbEntityValidationException dbx)
            {
                string message =
                   dbx.EntityValidationErrors.SelectMany(validationErrors => validationErrors.ValidationErrors)
                       .Aggregate("",
                           (current, validationError) =>
                               current +
                               $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                return BadRequest(message);

            }


        }
        [Route("GetLeaveorderHTML/{Id}")]
        [HttpGet]
        public IHttpActionResult GetLeaveorderHTML(int Id)
        {
            try
            {
                LeaveOrder lo = db.LeaveOrders.FirstOrDefault(x => x.Id == Id);
                if (lo != null)
                {
                    if (lo.OrderHTML != null)
                    {
                        return Ok(lo.OrderHTML);
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
        [Route("RemoveLeaveorder/{Id}")]
        [HttpGet]
        public IHttpActionResult RemoveLeaveorder(int Id)
        {
            try
            {
                LeaveOrder lo = db.LeaveOrders.FirstOrDefault(x => x.Id == Id);
                if (lo != null)
                {
                    Entity_Lifecycle elc = db.Entity_Lifecycle.FirstOrDefault(x => x.Id == lo.EntityLifecycle_Id);
                    if (elc != null)
                    {
                        elc.IsActive = false;
                        db.Entry(elc).State = EntityState.Modified;
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
        [Route("FormatDate/{loId}")]
        [HttpGet]
        public IHttpActionResult FormatDate(int loId)
        {
            try
            {
                LeaveOrder lo = db.LeaveOrders.FirstOrDefault(x => x.Id == loId);
                string fromDate = "";
                string toDate = "";
                if (lo.FromDate.HasValue)
                {
                    fromDate = lo.FromDate.Value.ToString("dd-MM-yyy");
                }
                if (lo.ToDate.HasValue)
                {
                    toDate = lo.ToDate.Value.ToString("dd-MM-yyy");
                }



                return Ok(new { fromDate, toDate });

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetEsrAppRes/{Id}")]
        [HttpGet]
        public IHttpActionResult GetEsrAppRes(int Id)
        {
            try
            {
                ESRView esrView = db.ESRViews.FirstOrDefault(x => x.Id == Id);
                ProfileDetailsView pdv = db.ProfileDetailsViews.FirstOrDefault(x => x.Id == esrView.Profile_Id);

                JsonResponseBody response = new JsonResponseBody()
                {
                    Status = Utility.StatusEnum.ok,
                    Body = new
                    {
                        EsrId = esrView.Id,
                        ProfileId = esrView.Profile_Id,
                        EmployeeName = esrView.EmployeeName,
                        FatherName = pdv.FatherName,
                        CNIC = Dashify(esrView.CNIC),
                        Designation = pdv.Designation_Name,
                        OrderDate = esrView.Created_Date.Value.ToString("dd-MM-yyy hh:mm:ss tt"),
                        SignedBy = esrView.EsrSectionOfficerName,
                        OrderType = esrView.TransferTypeID == 1 ? "Mutual Transfer"
                        : esrView.TransferTypeID == 2 ? "At Disposal"
                        : esrView.TransferTypeID == 3 ? "Suspend"
                        : esrView.TransferTypeID == 4 ? "General Transfer"
                        : esrView.TransferTypeID == 5 ? "Leave Order"
                        : esrView.TransferTypeID == 6 ? "Awaiting Posting"
                        : esrView.TransferTypeID == 7 ? "Notification"
                        : esrView.TransferTypeID == 8 ? "Adhoc Appointment"
                        : esrView.TransferTypeID == 9 ? "Consultant Order"
                        : esrView.TransferTypeID == 10 ? "Termination Order"
                        : esrView.TransferTypeID == 11 ? "Corrigendum Order" : ""
                    },
                    HasException = false,
                    Exception = null
                };
                return Ok(response);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetElrAppRes/{Id}")]
        [HttpGet]
        public IHttpActionResult GetElrAppRes(int Id)
        {
            try
            {
                LeaveOrderView loView = db.LeaveOrderViews.FirstOrDefault(x => x.Id == Id);
                ProfileDetailsView pdv = db.ProfileDetailsViews.FirstOrDefault(x => x.Id == loView.Profile_Id);

                if (loView == null)
                {
                    JsonResponseBody response = new JsonResponseBody()
                    {
                        Status = Utility.StatusEnum.notexist,
                        Body = null,
                        HasException = false,
                        Exception = null
                    };
                    return Ok(response);
                }
                else
                {
                    JsonResponseBody response = new JsonResponseBody()
                    {
                        Status = Utility.StatusEnum.ok,
                        Body = new
                        {
                            EsrId = loView.Id,
                            ProfileId = loView.Profile_Id,
                            EmployeeName = loView.EmployeeName,
                            FatherName = pdv.FatherName,
                            CNIC = Dashify(loView.CNIC),
                            Designation = pdv.Designation_Name,
                            OrderDate = loView.DateTime,
                            OrderType = "Leave Order",
                            SignedBy = loView.OfficerName
                        },
                        HasException = false,
                        Exception = null
                    };
                    return Ok(response);
                }

            }
            catch (Exception ex)
            {
                JsonResponseBody response = new JsonResponseBody()
                {
                    Status = Utility.StatusEnum.failed,
                    Body = null,
                    HasException = true,
                    Exception = ex
                };
                return Ok(response);
            }
        }


        private JsonResponseBody SearchServiceOrder(int id)
        {

            ESRView esrView = db.ESRViews.FirstOrDefault(x => x.Id == id);

            JsonResponseBody response = new JsonResponseBody();
           



            ProfileDetailsView pdv = db.ProfileDetailsViews.FirstOrDefault(x => x.Id == esrView.Profile_Id);

            if (esrView.TransferTypeID == 18)
            {
                response = new JsonResponseBody()
                {
                    Status = Utility.StatusEnum.ok,
                    Body = new
                    {
                        EsrId = esrView.Id,
                        FileNo = esrView.EmployeeFileNO,
                        OrderDate = esrView.Created_Date.Value.ToString("dd-MM-yyy hh:mm:ss tt"),
                        SignedBy = string.IsNullOrEmpty(esrView.EsrSectionOfficerName) ? string.IsNullOrEmpty(esrView.SectionOfficer) ? "N/A" : esrView.SectionOfficer : esrView.EsrSectionOfficerName,
                        OrderType = esrView.TransferTypeID == 1 ? "Mutual Transfer"
                  : esrView.TransferTypeID == 2 ? "At Disposal"
                  : esrView.TransferTypeID == 3 ? "Suspend"
                  : esrView.TransferTypeID == 4 ? "General Transfer"
                  : esrView.TransferTypeID == 5 ? "Leave Order"
                  : esrView.TransferTypeID == 6 ? "Awaiting Posting"
                  : esrView.TransferTypeID == 7 ? "Notification"
                  : esrView.TransferTypeID == 8 ? "Adhoc Appointment"
                  : esrView.TransferTypeID == 9 ? "Consultant Order"
                  : esrView.TransferTypeID == 10 ? "Termination Order"
                  : esrView.TransferTypeID == 11 ? "Corrigendum Order"
                  : esrView.TransferTypeID == 18 ? "Recommended Through PPSC/Adhoc" : ""

                  },
                    HasException = false,
                    Exception = null
                };
                return response;
            }
            else if (esrView.TransferTypeID == 12)
            {
                response = new JsonResponseBody()
                {
                    Status = Utility.StatusEnum.ok,
                    Body = new
                    {
                        EsrId = esrView.Id,
                        FileNo = esrView.EmployeeFileNO,
                        OrderDate = esrView.Created_Date.Value.ToString("dd-MM-yyy hh:mm:ss tt"),
                        SignedBy = string.IsNullOrEmpty(esrView.EsrSectionOfficerName) ? string.IsNullOrEmpty(esrView.SectionOfficer) ? "N/A" : esrView.SectionOfficer : esrView.EsrSectionOfficerName,
                        OrderType = esrView.TransferTypeID == 1 ? "Mutual Transfer"
                  : esrView.TransferTypeID == 2 ? "At Disposal"
                  : esrView.TransferTypeID == 3 ? "Suspend"
                  : esrView.TransferTypeID == 4 ? "General Transfer"
                  : esrView.TransferTypeID == 5 ? "Leave Order"
                  : esrView.TransferTypeID == 6 ? "Awaiting Posting"
                  : esrView.TransferTypeID == 7 ? "Notification"
                  : esrView.TransferTypeID == 8 ? "Adhoc Appointment"
                  : esrView.TransferTypeID == 9 ? "Consultant Order"
                  : esrView.TransferTypeID == 10 ? "Termination Order"
                  : esrView.TransferTypeID == 11 ? "Corrigendum Order"
                  : esrView.TransferTypeID == 18 ? "Recommended Through PPSC/Adhoc" : ""

                    },
                    HasException = false,
                    Exception = null
                };
                return response;
            }
            else
            {
                response = new JsonResponseBody()
                {
                    Status = Utility.StatusEnum.ok,
                    Body = new
                    {




                        EsrId = esrView.Id,

                        ProfileId = esrView.Profile_Id,
                        EmployeeName = esrView.EmployeeName,
                        FatherName = pdv.FatherName,
                        CNIC = Dashify(esrView.CNIC),
                        Designation = pdv.Designation_Name,


                        OrderDate = esrView.Created_Date.Value.ToString("dd-MM-yyy hh:mm:ss tt"),
                        SignedBy = string.IsNullOrEmpty(esrView.EsrSectionOfficerName) ? string.IsNullOrEmpty(esrView.SectionOfficer) ? "N/A" : esrView.SectionOfficer : esrView.EsrSectionOfficerName,
                        OrderType = esrView.TransferTypeID == 1 ? "Mutual Transfer"
                  : esrView.TransferTypeID == 2 ? "At Disposal"
                  : esrView.TransferTypeID == 3 ? "Suspend"
                  : esrView.TransferTypeID == 4 ? "General Transfer"
                  : esrView.TransferTypeID == 5 ? "Leave Order"
                  : esrView.TransferTypeID == 6 ? "Awaiting Posting"
                  : esrView.TransferTypeID == 7 ? "Notification"
                  : esrView.TransferTypeID == 8 ? "Adhoc Appointment"
                  : esrView.TransferTypeID == 9 ? "Consultant Order"
                  : esrView.TransferTypeID == 10 ? "Termination Order"
                  : esrView.TransferTypeID == 11 ? "Corrigendum Order"
                  : esrView.TransferTypeID == 12 ? "Recommended Through PPSC/Adhoc" : ""



                    },
                    HasException = false,
                    Exception = null
                };
                return response;
            }
            
          
        }

        private JsonResponseBody SerachLeaverOrder(int id)
        {
            id = id - 1003;
            LeaveOrderView loView = db.LeaveOrderViews.FirstOrDefault(x => x.Id == id);
            ProfileDetailsView pdv = db.ProfileDetailsViews.FirstOrDefault(x => x.Id == loView.Profile_Id);

            if (loView == null)
            {
                JsonResponseBody response = new JsonResponseBody()
                {
                    Status = Utility.StatusEnum.notexist,
                    Body = null,
                    HasException = false,
                    Exception = null
                };
                return response;
            }
            else
            {
                JsonResponseBody response = new JsonResponseBody()
                {
                    Status = Utility.StatusEnum.ok,
                    Body = new
                    {
                        EsrId = loView.Id,
                        ProfileId = loView.Profile_Id,
                        EmployeeName = loView.EmployeeName,
                        FatherName = pdv.FatherName,
                        CNIC = Dashify(loView.CNIC),
                        Designation = pdv.Designation_Name,
                        OrderDate = loView.DateTime,
                        OrderType = "Leave Order",
                        //SignedBy = string.IsNullOrEmpty(loView.OfficerName) ? "N/A" : loView.OfficerName
                        SignedBy = string.IsNullOrEmpty(loView.SignedBy) ? "N/A" : loView.SignedBy
                    },
                    HasException = false,
                    Exception = null
                };
                return response;
            }
        }
        //Mobile App Order Barcode

        [Route("GetEsrBarcode/{code}")]
        [HttpGet]
        public IHttpActionResult GetEsrAppRes(string code)
        {
            try
            {
                if (code.StartsWith("ESR-"))
                {
                    return Ok(SearchServiceOrder(Convert.ToInt32(code.Replace("ESR-", ""))));
                }
                else if (code.StartsWith("ELR-"))
                {
                    return Ok(SerachLeaverOrder(Convert.ToInt32(code.Replace("ELR-", ""))));
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex?.InnerException.Message ?? ex.Message);
            }

            return NotFound();
        }

        public Image barCodeZ(long Id)
        {
            Zen.Barcode.Code128BarcodeDraw barcode = Zen.Barcode.BarcodeDrawFactory.Code128WithChecksum;
            return barcode.Draw("ESR-" + Id, 100, 2);
        }
        [System.Web.Http.Route("GetSectionOfficers")]
        [System.Web.Http.HttpGet]
        public async Task<IHttpActionResult> GetSectionOfficers()
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                return Ok(await db.EsrSectionOfficers.ToListAsync());

            }
            catch (DbEntityValidationException dbx)
            {
                string message =
                   dbx.EntityValidationErrors.SelectMany(validationErrors => validationErrors.ValidationErrors)
                       .Aggregate("",
                           (current, validationError) =>
                               current +
                               $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                return null;
            }
        }

        [System.Web.Http.Route("UpdateOrderHTML")]
        [System.Web.Http.HttpPost]
        public async Task<IHttpActionResult> UpdateOrderHTML(object obj)
        {
            try
            {
                var jObj = JsonConvert.DeserializeObject<dynamic>(obj.ToString());
                int id = Convert.ToInt32(jObj.id.ToString());
                string OrderHTML = jObj.OrderHTML.ToString();

                db.Configuration.ProxyCreationEnabled = false;
                ESR esr = db.ESRs.Find(id);
                //Entity_Lifecycle elc = db.Entity_Lifecycle.Find(esr.EntityLifecycle_Id);
                if (esr != null)
                {
                    //if (elc.Created_Date.Day == DateTime.Now.Day)
                    //{
                    //    elc.Last_Modified_By = User.Identity.GetUserName();
                    //    db.Entry(elc).State = EntityState.Modified;
                    //}
                    //else
                    //{
                    //    Entity_Modified_Log eml = new Entity_Modified_Log();
                    //    eml.Modified_By = User.Identity.GetUserId();
                    //    eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                    //    eml.Description = "Order Updated / Modified by " + User.Identity.GetUserName();
                    //    eml.Entity_Lifecycle_Id = elc.Id;
                    //    db.Entity_Modified_Log.Add(eml);
                    //}
                    esr.OrderHTML = OrderHTML;
                    db.SaveChanges();
                    return Ok(true);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (DbEntityValidationException dbx)
            {
                string message =
                   dbx.EntityValidationErrors.SelectMany(validationErrors => validationErrors.ValidationErrors)
                       .Aggregate("",
                           (current, validationError) =>
                               current +
                               $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                return null;
            }
        }

        private string Dashify(string cnic)
        {
            StringBuilder returnCNIC = new StringBuilder();
            returnCNIC.Append(cnic[0]);
            returnCNIC.Append(cnic[1]);
            returnCNIC.Append(cnic[2]);
            returnCNIC.Append(cnic[3]);
            returnCNIC.Append(cnic[4]);
            returnCNIC.Append("-");
            returnCNIC.Append(cnic[5]);
            returnCNIC.Append(cnic[6]);
            returnCNIC.Append(cnic[7]);
            returnCNIC.Append(cnic[8]);
            returnCNIC.Append(cnic[9]);
            returnCNIC.Append(cnic[10]);
            returnCNIC.Append(cnic[11]);
            returnCNIC.Append("-");
            returnCNIC.Append(cnic[12]);
            return returnCNIC.ToString();
        }
        [System.Web.Http.Route("GetOrderHTML/{id}")]
        [System.Web.Http.HttpGet]
        public async Task<IHttpActionResult> GetOrderHTML(long id)
        {
            try
            {
                string userName = User.Identity.GetUserName();
                string data = "";
                if (userName == "dpd" || User.IsInRole("Deputy Secretary"))
                {
                    ESRView esrView = db.ESRViews.FirstOrDefault(x => x.Id == id);
                    if (esrView != null)
                    {
                        return Ok(esrView.OrderHTML);
                    }
                    else
                    {
                        return Ok(false);
                    }
                }
                else
                {
                    ESRView esrView = db.ESRViews.FirstOrDefault(x => x.Id == id && x.Created_By == userName);
                    if (esrView != null)
                    {
                        return Ok(esrView.OrderHTML);
                    }
                    else
                    {
                        return Ok(false);
                    }
                }
            }
            catch (DbEntityValidationException dbx)
            {
                string message =
                   dbx.EntityValidationErrors.SelectMany(validationErrors => validationErrors.ValidationErrors)
                       .Aggregate("",
                           (current, validationError) =>
                               current +
                               $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                return null;
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private void SendSMS(ProfileDetailsView EsrProfile, string hfmisCode)
        {
            if (hfmisCode.Length == 1)
            {
                if (EsrProfile.MobileNo != null && EsrProfile.MobileNo != "")
                {
                    string MessageBody = @"Dear " + EsrProfile.EmployeeName + "\n(CNIC:" + EsrProfile.CNIC + ")\n Your order is under process at Primaray and Secondary Healthcare Department Punjab. Please visit http://pshealth.punjab.gov.pk/ for further details.";

                    List<SMS> smsy = new List<SMS>();
                    SMS sms = new SMS()
                    {
                        MobileNumber = EsrProfile.MobileNo,
                        Message = MessageBody

                    };
                    smsy.Add(sms);

                    Common.SMS_Send(smsy);
                }
            }
            else if (hfmisCode.Length == 6)
            {
                if (EsrProfile.MobileNo != null && EsrProfile.MobileNo != "" && EsrProfile.District != null)
                {
                    string MessageBody = @"Dear " + EsrProfile.EmployeeName + "\n(CNIC:" + EsrProfile.CNIC + ")\n Your order is under process at District Health Authority, " + EsrProfile.District + ".\nPlease visit http://pshealth.punjab.gov.pk/ for further details.";

                    List<SMS> smsy = new List<SMS>();
                    SMS sms = new SMS()
                    {
                        MobileNumber = EsrProfile.MobileNo,
                        Message = MessageBody

                    };
                    smsy.Add(sms);

                    Common.SMS_Send(smsy);
                }
            }
            else
            {

            }

        }
    }

}