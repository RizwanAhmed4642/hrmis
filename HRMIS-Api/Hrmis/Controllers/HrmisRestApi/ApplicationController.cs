using DPUruNet;
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
using Zen.Barcode;

namespace Hrmis.Controllers.HrmisRestApi
{
    [Authorize]
    [RoutePrefix("api/Application")]
    public class ApplicationController : ApiController
    {
        private readonly ApplicationService _applicationService;

        public ApplicationController()
        {
            _applicationService = new ApplicationService();
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("GetApplication/{Id}/{Tracking}")]
        public IHttpActionResult GetApplication(int Id, int Tracking)
        {
            try
            {
                var app = _applicationService.GetApplication(Id, Tracking, User.Identity.GetUserName(), User.Identity.GetUserId());
                return Ok(app);
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex);
                return Ok(ex);
            }
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("GetApplicationData/{Id}/{Type}")]
        public IHttpActionResult GetApplicationData(int Id, string Type)
        {
            try
            {
                return Ok(_applicationService.GetApplicationData(Id, Type, User.Identity.GetUserName(), User.Identity.GetUserId()));
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
        [Route("RemoveApplication/{Id}/{Tracking}")]
        public IHttpActionResult RemoveApplication([FromBody] ApplicationLog applicationLog, int Id, int Tracking)
        {
            try
            {
                return Ok(_applicationService.RemoveApplication(Id, Tracking, User.Identity.GetUserName(), User.Identity.GetUserId(), applicationLog));
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
        [Route("RemoveApplicationAttachment/{Id}")]
        public IHttpActionResult RemoveApplicationAttachment(int Id)
        {
            try
            {
                return Ok(_applicationService.RemoveApplicationAttachment(Id, User.Identity.GetUserName(), User.Identity.GetUserId()));
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
        [Route("ChangeFileNumberOfApplication")]
        public IHttpActionResult ChangeFileNumberOfApplication(ApplicationMaster application)
        {
            try
            {
                return Ok(_applicationService.ChangeFileNumberOfApplication(application, User.Identity.GetUserName(), User.Identity.GetUserId()));
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
        [Route("GetApplications")]
        public IHttpActionResult GetApplications([FromBody] ApplicationFilter filter)
        {
            try
            {
                return Ok(_applicationService.GetApplications(filter, User.Identity.GetUserName().ToLower(), User.Identity.GetUserId()));
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
        [Route("GetBarcodedApplications")]
        public IHttpActionResult GetBarcodedApplications([FromBody] ApplicationFilter filter)
        {
            try
            {
                return Ok(_applicationService.GetBarcodedApplications(filter, User.Identity.GetUserName().ToLower(), User.Identity.GetUserId()));
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
        [Route("GetApplicationsLawwing")]
        public IHttpActionResult GetApplicationsLawwing([FromBody] ApplicationFilter filter)
        {
            try
            {
                return Ok(_applicationService.GetApplicationsLawwing(filter, User.Identity.GetUserName().ToLower(), User.Identity.GetUserId()));
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
        [Route("GetApplicationLog/{Id}/{lastId}/{orderAsc}")]
        public IHttpActionResult GetApplicationLog(int Id, int lastId, bool orderAsc)
        {
            try
            {
                return Ok(_applicationService.GetApplicationLogs(Id, lastId, orderAsc, User.Identity.GetUserName(), User.Identity.GetUserId()));
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
        [Route("GetApplicationLogsLaw/{Id}")]
        public IHttpActionResult GetApplicationLogsLaw(int Id)
        {
            try
            {
                return Ok(_applicationService.GetApplicationLogsLaw(Id, User.Identity.GetUserName(), User.Identity.GetUserId()));
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
        [Route("GetInboxApplications")]
        public IHttpActionResult GetInboxApplications([FromBody] ApplicationFilter filter)
        {
            try
            {
                return Ok(_applicationService.GetInboxApplications(filter, User.Identity.GetUserName(), User.Identity.GetUserId()));
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
        [Route("GetSelectedInboxApplications")]
        public IHttpActionResult GetSelectedInboxApplications([FromBody] List<int> Ids)
        {
            try
            {
                return Ok(_applicationService.GetSelectedInboxApplications(Ids, User.Identity.GetUserName(), User.Identity.GetUserId()));
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
        [Route("GetInboxApplicationsHisdu")]
        public IHttpActionResult GetInboxApplicationsHisdu([FromBody] ApplicationFilter filter)
        {
            try
            {
                return Ok(_applicationService.GetInboxApplicationsHisdu(filter, User.Identity.GetUserName(), User.Identity.GetUserId()));
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
        [Route("GetSentApplications")]
        public IHttpActionResult GetSentApplications([FromBody] ApplicationFilter filter)
        {
            try
            {
                return Ok(_applicationService.GetSentApplications(filter, User.Identity.GetUserName(), User.Identity.GetUserId()));
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
        [Route("GetSummaries")]
        public IHttpActionResult GetSummaries([FromBody] ApplicationFilter filter)
        {
            try
            {
                return Ok(_applicationService.GetSummaries(filter, User.Identity.GetUserName(), User.Identity.GetUserId()));
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
        [Route("GetScannedDocuments")]
        public IHttpActionResult GetScannedDocuments([FromBody] ApplicationFilter filter)
        {
            try
            {
                return Ok(_applicationService.GetScannedDocuments(filter, User.Identity.GetUserName(), User.Identity.GetUserId()));
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
        [Route("GetInboxOffices/{pending}")]
        public IHttpActionResult GetInboxOffices(bool pending)
        {
            try
            {
                return Ok(_applicationService.GetInboxOffices(pending, User.Identity.GetUserName(), User.Identity.GetUserId()));
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
        [Route("GetOfficerFilesCount")]
        public IHttpActionResult GetOfficerFilesCount()
        {
            try
            {
                return Ok(_applicationService.GetOfficerFilesCount(User.Identity.GetUserName(), User.Identity.GetUserId()));
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
        [Route("GetOfficerFilesFiles/{type}")]
        public IHttpActionResult GetOfficerFilesFiles(int type)
        {
            try
            {
                return Ok(_applicationService.GetOfficerFilesFiles(type, User.Identity.GetUserName(), User.Identity.GetUserId()));
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
        [Route("GetInboxOfficesHisdu/{officer_Id}")]
        public IHttpActionResult GetInboxOfficesHisdu(int officer_Id)
        {
            try
            {
                return Ok(_applicationService.GetInboxOfficesHisdu(officer_Id, User.Identity.GetUserName(), User.Identity.GetUserId()));
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
        [Route("SubmitHrApplication")]
        public IHttpActionResult SubmitHrApplication([FromBody] HrApplication hrApplication)
        {
            try
            {
                var app = _applicationService.SubmitHrApplication(hrApplication, User.Identity.GetUserName(), User.Identity.GetUserId());
                return Ok(app);
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
        [Route("SubmitApplicationDocument")]
        public IHttpActionResult SubmitApplicationDocument([FromBody] ApplicationDocument document)
        {
            try
            {
                if (document == null || string.IsNullOrEmpty(document.Name)) return BadRequest();
                var appDoc = _applicationService.SubmitApplicationDocument(document, User.Identity.GetUserName(), User.Identity.GetUserId());
                return Ok(appDoc);
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
        [Route("SubmitApplicationDocumentType")]
        public IHttpActionResult SubmitApplicationDocumentType([FromBody] AppTypeDoc appTypeDoc)
        {
            try
            {
                if (appTypeDoc == null || appTypeDoc.ApplicationType_Id == 0 || appTypeDoc.Document_Id == 0) return BadRequest();
                var appTypeDocResult = _applicationService.SubmitApplicationDocumentType(appTypeDoc, User.Identity.GetUserName(), User.Identity.GetUserId());
                return Ok(appTypeDocResult);
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
        [HttpPost]
        [Route("SubmitApplication")]
        public async Task<IHttpActionResult> SubmitApplicationAsync([FromBody] ApplicationMaster application)
        {
            try
            {
                if (application == null) return BadRequest();
                //if (application.ApplicationType_Id == null) return Ok("Invalid");
                bool administrativeOffice = false;
                if (User.IsInRole("Section Officer") || User.IsInRole("Deputy Secretary") || User.IsInRole("Administrative Office"))
                {
                    administrativeOffice = true;
                }
                var app = await _applicationService.SubmitApplication(application, User.Identity.GetUserName(), User.Identity.GetUserId(), administrativeOffice);

                if (app != null && (app.ApplicationSource_Id == 5 || app.ApplicationSource_Id == 6))
                {
                    Code128BarcodeDraw barcode = BarcodeDrawFactory.Code128WithChecksum;
                    BarcodeSymbology s = BarcodeSymbology.Code39NC;
                    BarcodeDraw drawObject = BarcodeDrawFactory.GetSymbology(s);
                    var metrics = drawObject.GetDefaultMetrics(50);
                    metrics.Scale = 1;
                    Image barCode = barcode.Draw(application.TrackingNumber.ToString(), metrics);
                    string imgSrc = "data:image/jpg;base64," + ImagesUtil.ImageToBase64(barCode, ImageFormat.Jpeg);
                    return Ok(new { application, barCode = imgSrc });
                }
                else
                {
                    Image barCode = Common.barCodeZ(Convert.ToInt32(application.TrackingNumber));
                    string imgSrc = "data:image / jpg; base64," + ImagesUtil.ImageToBase64(barCode, ImageFormat.Jpeg);
                    return Ok(new { application, barCode = imgSrc });
                }
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("SubmitOrderApplication")]
        public async Task<IHttpActionResult> SubmitOrderApplication([FromBody] ApplicationMaster application)
        {
            try
            {
                if (application == null) return BadRequest();
                if (application.ApplicationType_Id == null) return Ok("Invalid");
                Image qrCode = null;
                var orderResponse = await _applicationService.GenerateOrder(application, User.Identity.GetUserName(), User.Identity.GetUserId());
                long barcodeId = application.Id == 0 ? application.ApplicationType_Id == 5 ? (orderResponse.leaveOrder.Id + 1003) : orderResponse.esr.Id : (long)orderResponse.applicationMaster.TrackingNumber;
                Image barCode = barCodeZ(Convert.ToInt32(barcodeId));
                string imgSrc = "data:image / jpg; base64," + ImagesUtil.ImageToBase64(barCode, ImageFormat.Jpeg);

                if (application.ApplicationType_Id == 5)
                {
                    qrCode = zenQR(orderResponse.leaveOrderView.LeaveTypeName + ", " + application.EmployeeName + ", " + application.CNIC + ", " + orderResponse.leaveOrderView.Designation);
                }
                else
                {
                    qrCode = zenQR(orderResponse.orderType + ", " + application.EmployeeName + ", " + application.CNIC + ", " + orderResponse.esrView.DesigFrom);
                }
                string qrSrc = "data:image / jpg; base64," + ImagesUtil.ImageToBase64(qrCode, ImageFormat.Jpeg);

                return Ok(new { orderResponse, barCode = imgSrc, qrSrc = qrSrc });
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GenerateQRManual/{id}/{type}")]
        public IHttpActionResult GenerateQRManual(int id, int type)
        {
            try
            {
                Image qrCode = null;
                using (var db = new HR_System())
                {
                    if (type == 1)
                    {
                        var esr = db.ESRViews.FirstOrDefault(x => x.Id == id);
                        if (esr != null)
                        {
                            var orderType = db.TransferTypes.FirstOrDefault(x => x.Id == esr.TransferTypeID);
                            if (orderType != null)
                            {
                                var esrs = db.ESRViews.Count(x => x.MutualESR_Id == esr.Id);
                                if (esrs > 0)
                                {
                                    qrCode = zenQR(orderType.Name + ", Combine Order, " + ", ESR-" + esr.Id);
                                    string qrSrc = "data:image / jpg; base64," + ImagesUtil.ImageToBase64(qrCode, ImageFormat.Jpeg);
                                    return Ok(new { qrSrc = qrSrc });
                                }
                                else
                                {
                                    qrCode = zenQR(orderType.Name + ", " + esr.EmployeeName + ", " + esr.CNIC + ", " + esr.DesigFrom);
                                    string qrSrc = "data:image / jpg; base64," + ImagesUtil.ImageToBase64(qrCode, ImageFormat.Jpeg);
                                    return Ok(new { qrSrc = qrSrc });
                                }
                            }
                        }
                    }
                    else if (type == 2)
                    {
                        var elr = db.LeaveOrderViews.FirstOrDefault(x => x.Id == id);
                        if (elr != null)
                        {

                            qrCode = zenQR("Leave Order" + elr.LeaveTypeName + ", " + elr.EmployeeName + ", " + elr.CNIC + ", " + elr.Designation);
                            string qrSrc = "data:image / jpg; base64," + ImagesUtil.ImageToBase64(qrCode, ImageFormat.Jpeg);
                            return Ok(new { qrSrc = qrSrc });
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("SubmitMutualOrder")]
        public async Task<IHttpActionResult> SubmitMutualOrder([FromBody] MutualOrderDto mutualOrderDto)
        {
            try
            {
                if (mutualOrderDto == null) return BadRequest();

                var orderResponse = await _applicationService.GenerateMutualOrder(mutualOrderDto, User.Identity.GetUserName(), User.Identity.GetUserId());
                long barcodeId = orderResponse.esr.Id;
                Image barCode = barCodeZ(Convert.ToInt32(barcodeId));
                string imgSrc = "data:image / jpg; base64," + ImagesUtil.ImageToBase64(barCode, ImageFormat.Jpeg);

                return Ok(new { orderResponse, barCode = imgSrc });
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("SubmitCombineOrder")]
        public async Task<IHttpActionResult> SubmitCombineOrderAsync([FromBody] CombineOrderDto combineOrderDto)
        {
            try
            {
                if (combineOrderDto.applicationMaster == null) return BadRequest();
                if (combineOrderDto.applicationMaster.ApplicationType_Id == null) return Ok("Invalid");
                List<OrderResponse> orderResponses = new List<OrderResponse>();
                long mutualESRId = 0;
                long barcodeId = 0;
                foreach (var application in combineOrderDto.applications)
                {
                    application.RawText = null;
                    if (mutualESRId == 0)
                    {
                        application.ApplicationSource_Id = null;
                        var orderResponse = await _applicationService.GenerateOrder(application, User.Identity.GetUserName(), User.Identity.GetUserId());
                        mutualESRId = combineOrderDto.applicationMaster.ApplicationType_Id == 5 ? orderResponse.leaveOrder.Id : orderResponse.esr.Id;
                        barcodeId = application.ApplicationType_Id == 5 ? (orderResponse.leaveOrder.Id + 1003) : orderResponse.esr.Id;
                        orderResponses.Add(orderResponse);
                    }
                    else
                    {
                        application.ApplicationSource_Id = (int)mutualESRId;
                        var orderResponse = await _applicationService.GenerateOrder(application, User.Identity.GetUserName(), User.Identity.GetUserId());
                        orderResponses.Add(orderResponse);
                    }
                }
                Image qrCode = null;
                Image barCode = barCodeZ(Convert.ToInt32(barcodeId));
                string imgSrc = "data:image / jpg; base64," + ImagesUtil.ImageToBase64(barCode, ImageFormat.Jpeg);
                var oresponse = orderResponses.FirstOrDefault();
                if (oresponse != null)
                {
                    if (oresponse.applicationMaster.ApplicationType_Id == 5)
                    {
                        qrCode = zenQR(oresponse.leaveOrderView.LeaveTypeName + ", Combine Order");
                    }
                    else
                    {
                        //qrCode = zenQR(oresponse.orderType + ", Combine Order, " + ", ESR-" + oresponse.esrView.Id);
                        qrCode = zenQR("Combine Order" + ", ESR-242133");
                    }
                }

                string qrSrc = "data:image / jpg; base64," + ImagesUtil.ImageToBase64(qrCode, ImageFormat.Jpeg);
                return Ok(new { orderResponses, barCode = imgSrc, qrSrc = qrSrc });
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GenerateBars/{trackingNumber}")]
        public IHttpActionResult GenerateBarcode(int trackingNumber)
        {
            try
            {
                Image barCode = Common.barCodeZ(trackingNumber);
                string imgSrc = "data:image / jpg; base64," + ImagesUtil.ImageToBase64(barCode, ImageFormat.Jpeg);

                return Ok(new { barCode = imgSrc });
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GenerateBarcodeRI/{trackingNumber}")]
        public IHttpActionResult GenerateBarcodeRI(int trackingNumber)
        {
            try
            {
                Code128BarcodeDraw barcode = BarcodeDrawFactory.Code128WithChecksum;
                BarcodeSymbology s = BarcodeSymbology.Code39NC;
                BarcodeDraw drawObject = BarcodeDrawFactory.GetSymbology(s);
                var metrics = drawObject.GetDefaultMetrics(50);
                metrics.Scale = 1;
                Image barCode = barcode.Draw(trackingNumber.ToString(), metrics);
                string imgSrc = "data:image/jpg;base64," + ImagesUtil.ImageToBase64(barCode, ImageFormat.Jpeg);
                return Ok(new
                {
                    barCode = imgSrc
                });
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GenerateOrderBars/{trackingNumber}")]
        public IHttpActionResult GenerateOrderBars(int trackingNumber)
        {
            try
            {
                Image barCode = barCodeZ(trackingNumber);
                string imgSrc = "data:image / jpg; base64," + ImagesUtil.ImageToBase64(barCode, ImageFormat.Jpeg);
                return Ok(new { barCode = imgSrc });
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GenerateOrderQr/{desc}")]
        public IHttpActionResult GenerateOrderQr(string desc)
        {
            try
            {
                Image qrCode = zenQR(desc);
                string imgSrc = "data:image / jpg; base64," + ImagesUtil.ImageToBase64(qrCode, ImageFormat.Jpeg);
                return Ok(new { qrCode = imgSrc });
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("ChangeSystemWithOrder/{esrId}")]
        public IHttpActionResult ChangeSystemWithOrder(int esrId)
        {
            try
            {
                if (esrId == 0) return Ok("Invalid");
                var user = new UserService().GetUser(User.Identity.GetUserId());
                if (user.UserName.ToLower().StartsWith("ceo.") || user.hfmiscode.Length == 1)
                {
                    return Ok(_applicationService.ChangeSystemWithOrder(esrId, user.UserName, user.Id));
                }
                return Ok("Invalid");
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [Route("UploadSignedOrder/{applicationId}/{esrId}/{elrId}")]
        public async Task<bool> UploadSignedOrder(int applicationId, int esrId, int elrId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    if (!Request.Content.IsMimeMultipartContent())
                        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                    string RootPath = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\Files\OrderAttachments\";
                    var dirPath = RootPath;

                    var provider = new MultipartMemoryStreamProvider();
                    await Request.Content.ReadAsMultipartAsync(provider);

                    CreateDirectoryIfNotExists(dirPath);
                    string filename = "";

                    foreach (var file in provider.Contents)
                    {
                        EsrAttachment esrAttachment = new EsrAttachment();
                        if (applicationId != 0)
                        {
                            esrAttachment.Application_Id = applicationId;

                        }
                        if (esrId != 0)
                        {
                            esrAttachment.ESR_Id = esrId;
                        }
                        if (elrId != 0)
                        {
                            esrAttachment.ESR_Id = elrId;
                        }
                        string key = file.Headers.ContentDisposition.Name;

                        var FileExtension = Path.GetExtension(file.Headers.ContentDisposition.FileName.Trim('\"')).Substring(1);

                        filename = Guid.NewGuid().ToString() + "_" + esrId + "." + FileExtension;

                        esrAttachment.OrderDoc_Id = 1;
                        esrAttachment.UploadPath = @"Uploads\Files\ApplicationAttachments\" + filename;
                        esrAttachment.IsActive = true;
                        esrAttachment.IsBase64 = false;
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
                        _db.EsrAttachments.Add(esrAttachment);
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
        [Route("UploadApplicationAttachments/{appId}")]
        public async Task<bool> UploadApplicationAttachments(int appId)
        {
            try
            {
                using (var _db = new HR_System())
                {

                    if (!Request.Content.IsMimeMultipartContent())
                        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                    string RootPath = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\Files\ApplicationAttachments\";
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
                        filename = Guid.NewGuid().ToString() + "_" + appId + "." + FileExtension;
                        ApplicationAttachment applicationAttachment = new ApplicationAttachment();
                        string mkey = key.Split('_').LastOrDefault().Trim('\"');
                        applicationAttachment.Document_Id = Convert.ToInt32(mkey);
                        applicationAttachment.UploadPath = @"Uploads\Files\ApplicationAttachments\" + filename;
                        applicationAttachment.IsActive = true;
                        applicationAttachment.IsBase64 = false;
                        if (applicationAttachment.Document_Id == 16)
                        {
                            applicationAttachment.ESR_Id = appId;
                        }
                        else
                        {
                            applicationAttachment.Application_Id = appId;
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
                        _db.ApplicationAttachments.Add(applicationAttachment);
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
        [Route("UploadSignedApplication/{appId}")]
        public async Task<bool> UploadSignedApplication(int appId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    if (!Request.Content.IsMimeMultipartContent())
                        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                    string RootPath = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\Files\ApplicationAttachments\";
                    var dirPath = RootPath;

                    var provider = new MultipartMemoryStreamProvider();
                    await Request.Content.ReadAsMultipartAsync(provider);

                    CreateDirectoryIfNotExists(dirPath);
                    string filename = "";

                    foreach (var file in provider.Contents)
                    {
                        string key = file.Headers.ContentDisposition.Name;

                        var FileExtension = Path.GetExtension(file.Headers.ContentDisposition.FileName.Trim('\"')).Substring(1);

                        filename = Guid.NewGuid().ToString() + "_" + appId + "." + FileExtension;
                        ApplicationAttachment applicationAttachment = new ApplicationAttachment();
                        applicationAttachment.Application_Id = appId;
                        applicationAttachment.Document_Id = 1;
                        applicationAttachment.UploadPath = @"Uploads\Files\ApplicationAttachments\" + filename;
                        applicationAttachment.IsActive = true;
                        applicationAttachment.IsBase64 = false;
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
                        _db.ApplicationAttachments.Add(applicationAttachment);
                        _db.SaveChanges();
                        var application = _db.ApplicationMasters.FirstOrDefault(x => x.Id == appId);
                        application.IsSigned = true;
                        application.SignededAppAttachement_Id = applicationAttachment.Id;
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

        [HttpPost]
        [Route("CreateApplicationLog")]
        public IHttpActionResult CreateApplicationLog([FromBody] ApplicationLog applicationLog)
        {
            try
            {
                if (applicationLog == null) return BadRequest();
                if (applicationLog.Application_Id == null || applicationLog.Application_Id == 0) return Ok("Invalid");
                var appLog = _applicationService.CreateApplicationLog(applicationLog, User.Identity.GetUserName(), User.Identity.GetUserId());
                return Ok(appLog);
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("CreateApplicationLogLaw")]
        public IHttpActionResult CreateApplicationLogLaw([FromBody] ApplicationLog applicationLog)
        {
            try
            {
                if (applicationLog == null) return BadRequest();
                if (applicationLog.Application_Id == null || applicationLog.Application_Id == 0) return Ok("Invalid");
                var appLog = _applicationService.CreateApplicationLog(applicationLog, User.Identity.GetUserName(), User.Identity.GetUserId());
                return Ok(new { data = appLog });
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("SubmitApplicationAttachments")]
        public IHttpActionResult SubmitApplicationAttachments([FromBody] List<ApplicationAttachment> applicationAttachments)
        {
            try
            {
                if (applicationAttachments.Count == 0) return Ok(true);
                var res = _applicationService.SubmitApplicationAttachments(applicationAttachments, User.Identity.GetUserName(), User.Identity.GetUserId());
                return Ok(res);
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("SubmitFileMovement")]
        public IHttpActionResult SubmitFileMovement([FromBody] List<int> applicationIds)
        {
            try
            {
                if (applicationIds == null) return BadRequest();
                if (applicationIds.Count == 0) return Ok("Invalid");
                var fileMoveMaster = _applicationService.SubmitFileMovement(applicationIds, User.Identity.GetUserName(), User.Identity.GetUserId());
                Image barCode = Common.barCodeZ(Convert.ToInt32(fileMoveMaster.MID_Number));
                string imgSrc = "data:image / jpg; base64," + ImagesUtil.ImageToBase64(barCode, ImageFormat.Jpeg);

                return Ok(new { fileMoveMaster, barCode = imgSrc });
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("SubmitFileMovement2")]
        public IHttpActionResult SubmitFileMovement2([FromBody] List<ApplicationMaster> applications)
        {
            try
            {
                if (applications == null) return BadRequest();
                if (applications.Count == 0) return Ok("Invalid");
                var fileMoveMaster = _applicationService.SubmitFileMovement2(applications, User.Identity.GetUserName(), User.Identity.GetUserId());
                Image barCode = Common.barCodeZ(Convert.ToInt32(fileMoveMaster.MID_Number));
                string imgSrc = "data:image / jpg; base64," + ImagesUtil.ImageToBase64(barCode, ImageFormat.Jpeg);

                return Ok(new { fileMoveMaster, barCode = imgSrc });
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("SubmitFileMovementFDO/{officer_Id}")]
        public IHttpActionResult SubmitFileMovementFDO([FromBody] List<ApplicationMaster> applications, int officer_Id)
        {
            try
            {
                if (applications == null) return BadRequest();
                if (applications.Count == 0) return Ok("Invalid");
                var fileMoveMaster = _applicationService.SubmitFileMovementFDO(applications, officer_Id, User.Identity.GetUserName(), User.Identity.GetUserId());
                Image barCode = Common.barCodeZ(Convert.ToInt32(fileMoveMaster.MID_Number));
                string imgSrc = "data:image / jpg; base64," + ImagesUtil.ImageToBase64(barCode, ImageFormat.Jpeg);

                return Ok(new { fileMoveMaster, barCode = imgSrc });
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("SearchProfile/{cnic}")]
        public IHttpActionResult SearchProfile(string cnic)
        {
            try
            {
                if (string.IsNullOrEmpty(cnic)) return BadRequest();
                if (cnic.Length != 13) return Ok("Invalid");
                return Ok(_applicationService.SearchProfile(cnic));
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("SearchHealthFacilities/{hfmisCode}")]
        public IHttpActionResult SearchHealthFacilities([FromBody] SearchQuery searchQuery, string hfmisCode)
        {
            try
            {
                return Ok(_applicationService.SearchHealthFacilities(searchQuery.Query, hfmisCode));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("CheckVacancy/{hf_Id}/{hfmisCode}/{designationId}")]
        public IHttpActionResult CheckVacancy(int hf_Id, string hfmisCode, int designationId)
        {
            try
            {
                if (hf_Id == 0) return BadRequest();
                if (hfmisCode.Length != 19) return BadRequest();
                if (designationId == 0) return BadRequest();
                if (User == null) return Unauthorized();
                return Ok(_applicationService.CheckVacancy(hf_Id, hfmisCode, designationId, User.Identity.GetUserName(), User.Identity.GetUserId()));
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetOrderDesignations/{hf_Id}/{hfmisCode}")]
        public IHttpActionResult CheckVacancy(int hf_Id, string hfmisCode)
        {
            try
            {
                if (hf_Id == 0) return BadRequest();
                if (hfmisCode.Length != 19) return BadRequest();
                if (User == null) return Unauthorized();
                var response = _applicationService.GetOrderDesignations(hf_Id, hfmisCode, User.Identity.GetUserName(), User.Identity.GetUserId());
                return Ok(response);
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetApplicationForLetter/{Id}/{Tracking}")]
        public IHttpActionResult GetApplicationForLetter(int Id, int Tracking)
        {
            try
            {
                return Ok(_applicationService.GetApplicationForLetter(Id, Tracking, User.Identity.GetUserName(), User.Identity.GetUserId()));
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetOfficersForLetter/{Id}/{Tracking}")]
        public IHttpActionResult GetOfficersForLetter(int Id, int Tracking)
        {
            try
            {
                return Ok(_applicationService.GetOfficersForLetter(Id, Tracking, User.Identity.GetUserName(), User.Identity.GetUserId()));
            }
            catch (Exception ex)
            {
                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [Route("FPrint")]
        [HttpPost]
        public IHttpActionResult FPrint(FPPrint fprint)
        {
            try
            {
                using (var db = new HR_System())
                {
                    string userId = User.Identity.GetUserId();
                    P_SOfficers p_SOfficer = db.P_SOfficers.FirstOrDefault(x => x.User_Id.Equals(userId));
                    var fingerprintVal = Fmd.SerializeXml(new FingerprintSdk().GetFmd(fprint.metaData));
                    //p_SOfficer.FingerPrint = fingerprintVal;
                    db.Entry(p_SOfficer).State = EntityState.Modified;
                    db.SaveChanges();
                    return Ok(true);
                }
            }
            catch (Exception ex)
            {

                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }
        [Route("FPrintRegister/{officer_Id}/{number}")]
        [HttpPost]
        public IHttpActionResult FPrintRegister([FromBody] FPPrint fprint, int officer_Id, int number)
        {
            try
            {
                using (var db = new HR_System())
                {
                    P_SOfficers p_SOfficer = db.P_SOfficers.FirstOrDefault(x => x.Id == officer_Id);

                    bool fpExist = true;
                    FingerPrint fp = db.FingerPrints.FirstOrDefault(x => x.PandSOfficer_Id == p_SOfficer.Id);

                    if (fp == null)
                    {
                        fpExist = false;
                        fp = new FingerPrint();
                    }
                    if (number == 1) fp.FP1 = Fmd.SerializeXml(new FingerprintSdk().GetFmd(fprint.metaData));
                    if (number == 2) fp.FP2 = Fmd.SerializeXml(new FingerprintSdk().GetFmd(fprint.metaData));
                    if (number == 3) fp.FP3 = Fmd.SerializeXml(new FingerprintSdk().GetFmd(fprint.metaData));
                    if (number == 4) fp.FP4 = Fmd.SerializeXml(new FingerprintSdk().GetFmd(fprint.metaData));
                    if (number == 5) fp.FP5 = Fmd.SerializeXml(new FingerprintSdk().GetFmd(fprint.metaData));

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
            }
            catch (Exception ex)
            {

                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }


        [Route("FPrintClear/{officerId}")]
        [HttpGet]
        public IHttpActionResult FPrintClear(int officerId)
        {
            using (var db = new HR_System())
            {
                using (var transc = db.Database.BeginTransaction())
                {
                    try
                    {
                        var pOffcer = db.P_SOfficers.FirstOrDefault(x => x.Id == officerId);
                        pOffcer.FingerPrint_Id = null;
                        db.SaveChanges();
                        var fps = db.FingerPrints.Where(x => x.PandSOfficer_Id == officerId).ToList();

                        db.FingerPrints.RemoveRange(fps);
                        db.SaveChanges();
                        transc.Commit();
                        return Ok("Ok");
                    }
                    catch (Exception ex)
                    {
                        transc.Rollback();
                        Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
                    }
                }
            }
        }

        [Route("FPrintCompare")]
        [HttpPost]
        public IHttpActionResult FPrintCompare([FromBody] FPPrint fprint)
        {
            try
            {
                using (var db = new HR_System())
                {

                    P_SOfficers officer = new FingerprintSdk().SearchOfficer(fprint.metaData);
                    if (officer != null)
                    {
                        return Ok(new { result = true, officer = officer });
                    }
                    else
                    {
                        return Ok(new { result = false });

                    }
                }
            }
            catch (Exception ex)
            {

                Common.EmailToMe(User.Identity.GetUserName(), User.Identity.GetUserId(), ex.Message, ex); return BadRequest(ex.Message);
            }
        }


        public Image barCodeZ(long Id)
        {
            Code128BarcodeDraw barcode = BarcodeDrawFactory.Code128WithChecksum;
            BarcodeSymbology s = BarcodeSymbology.Code39NC;
            BarcodeDraw drawObject = BarcodeDrawFactory.GetSymbology(s);
            var metrics = drawObject.GetDefaultMetrics(50);
            metrics.Scale = 1;
            string prefix = Id < 29000 ? "ELR-" : "ESR-";
            return drawObject.Draw(prefix + Id, metrics);
        }
        public Image zenQR(string desc)
        {
            Zen.Barcode.CodeQrBarcodeDraw qrcode = Zen.Barcode.BarcodeDrawFactory.CodeQr;
            //var s = QrEncodeMode.AlphaNumeric;
            //var drawObject = QRC.GetSymbology(s);
            //var metrics = drawObject.GetDefaultMetrics(50);
            //metrics.Scale = 1;
            return qrcode.Draw(desc, 80);
        }
        private void CreateDirectoryIfNotExists(string dirPath) { if (!Directory.Exists(dirPath)) { Directory.CreateDirectory(dirPath); } }
    }
    public class FPPrint
    {
        public string metaData { get; set; }
    }
    public class FPPrints
    {
        public List<string> metaData { get; set; }
    }
}
