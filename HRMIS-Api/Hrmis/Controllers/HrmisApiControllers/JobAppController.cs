using Hrmis.Models;
using Hrmis.Models.Common;
using Hrmis.Models.DbModel;
using Hrmis.Models.ImageProcessor;
using Hrmis.Models.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using Novacode;
using Spire.Doc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Zen.Barcode;
using Image = System.Drawing.Image;

namespace Hrmis.Controllers.HrmisApiControllers
{
    [System.Web.Http.RoutePrefix("api/JobApp")]
    public class JobAppController : ApiController
    {
        private HR_System db = new HR_System();
        private string[] types = { "011", "012", "025", "013", "014", "015", "021", "023", "024", "027", "029", "030", "033", "036", "037", "039" };
        private HashSet<string> filledTehsils = new HashSet<string>();
        private TransferPostingService _transferPostingService;
        public JobAppController()
        {
            _transferPostingService = new TransferPostingService();
            db.Configuration.ProxyCreationEnabled = false;
        }



        //[Route("GetSummary")]
        //[HttpGet]
        //public IHttpActionResult GetSummary()
        //{
        //    try
        //    {

        //        int New = db.Merits.Where(x => x.Status.Equals("New")).Count();
        //        int Existing = db.Merits.Where(x => x.Status.Equals("Existing")).Count();
        //        int ProfileBuilt = db.Merits.Where(x => x.Status.Equals("ProfileBuilt")).Count();
        //        int Accepted = db.Merits.Where(x => x.Status.Equals("Accepted")).Count();
        //        int Completed = db.Merits.Where(x => x.Status.Equals("Completed")).Count();
        //        int Acknowledged = db.Merits.Where(x => x.Status.Equals("Acknowledged")).Count();
        //        int Total = db.Merits.Count();
        //        return Ok(new
        //        {
        //            result = true,
        //            New = New,
        //            Existing = Existing,
        //            ProfileBuilt = ProfileBuilt,
        //            Accepted = Accepted,
        //            Completed = Completed,
        //            Acknowledged = Acknowledged,
        //            Total = Total
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(false);
        //    }


        //}


        [Route("GetSummary")]
        [HttpGet]
        public IHttpActionResult GetSummary()
        {
            //List<int> meritIds = new List<int>();
            //int merits = 0;
            //int noMerits = 0;
            //int noFileExist = 0;

            //WMOReportMobileModel wmoReportMobileModel = new WMOReportMobileModel();

            //wmoReportMobileModel.Title = "WMO Portal";
            //wmoReportMobileModel.SubTitle = "Summary";

            //wmoReportMobileModel.Stats = new List<WMOReportStats>();

            //int New = db.Merits.Where(x => x.Status.Equals("New")).Count();
            //int Existing = db.Merits.Where(x => x.Status.Equals("Existing")).Count();
            //int ProfileBuilt = db.Merits.Where(x => x.Status.Equals("ProfileBuilt")).Count();
            //int Acknowledged = db.Merits.Where(x => x.Status.Equals("Acknowledged")).Count();
            //int Total = db.Merits.Count();

            //wmoReportMobileModel.Stats.Add(new WMOReportStats("No Action", (New + Existing)));
            //wmoReportMobileModel.Stats.Add(new WMOReportStats("Review Profile Only", ProfileBuilt));
            //wmoReportMobileModel.Stats.Add(new WMOReportStats("Acknowledged", Acknowledged));

            //wmoReportMobileModel.FooterName = "Total";
            //wmoReportMobileModel.FooterValue = Total;

            //wmoReportMobileModel.RefreshButtonText = "Refresh";

            //int acceptanceWithPreferences = 0;
            //int acceptanceWithPreferencesCorruptFile = 0;

            //int acceptanceOnly = 0;
            //int acceptanceOnlyCorruptFile = 0;
            //int noAcceptanceAndPreferences = 0;

            //foreach (var merit in db.Merits.Where(x => x.Status.Equals("Accepted") || x.Status.Equals("Completed")))
            //{
            //    bool prefExist = db.MeritPreferences.Where(x => x.Merit_Id == merit.Id).Count() > 0 ? true : false;
            //    if (prefExist)
            //    {
            //        acceptanceWithPreferences++;
            //    }
            //    else if (!prefExist)
            //    {
            //        acceptanceOnly++;
            //    }
            //    //if (prefExist && File.Exists(HttpContext.Current.Server.MapPath(@"~\Uploads\Acceptances\" + merit.Id + "_OfferLetter.jpg")))
            //    //{
            //    //    try
            //    //    {
            //    //        using (var test = System.Drawing.Image.FromFile(HttpContext.Current.Server.MapPath(@"~\Uploads\Acceptances\" + merit.Id + "_OfferLetter.jpg")))
            //    //        {
            //    //            acceptanceWithPreferences++;
            //    //        }
            //    //    }
            //    //    catch (Exception ex)
            //    //    {
            //    //        acceptanceWithPreferencesCorruptFile++;
            //    //    }
            //    //}
            //    //else if (!prefExist && File.Exists(HttpContext.Current.Server.MapPath(@"~\Uploads\Acceptances\" + merit.Id + "_OfferLetter.jpg")))
            //    //{
            //    //    try
            //    //    {
            //    //        using (var test = System.Drawing.Image.FromFile(HttpContext.Current.Server.MapPath(@"~\Uploads\Acceptances\" + merit.Id + "_OfferLetter.jpg")))
            //    //        {
            //    //            acceptanceOnly++;
            //    //        }
            //    //    }
            //    //    catch (Exception ex)
            //    //    {
            //    //        acceptanceOnlyCorruptFile++;
            //    //    }
            //    //}
            //    //else if (!prefExist && !File.Exists(HttpContext.Current.Server.MapPath(@"~\Uploads\Acceptances\" + merit.Id + "_OfferLetter.jpg")))
            //    //{
            //    //    noAcceptanceAndPreferences++;
            //    //}
            //}
            //wmoReportMobileModel.Stats.Add(new WMOReportStats("Acceptance with Prefrences", acceptanceWithPreferences));
            //wmoReportMobileModel.Stats.Add(new WMOReportStats("Acceptance Only", acceptanceOnly));
            int c = 0;
            List<int?> missingMerits = new List<int?>();
            foreach (var merit in db.Merits)
            {
                try
                {
                    if (File.Exists(@"D:\Projects\HRMIS\API\Hrmis\Uploads\Acceptances\" + merit.Id + "_OfferLetter.jpg"))
                    {
                        c++;
                    }
                    else
                    {
                        missingMerits.Add(merit.MeritNumber);
                    }
                }
                catch (Exception ex)
                {
                }
            }
            return Ok(new { c, missingMerits });
        }


        [Route("GetMerit/{CNIC}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetMerit(string CNIC)
        {
            try
            {
                var merit = db.Merits.Include(x => x.HrDesignation).Include(x => x.MeritActiveDesignation).Where(x => x.CNIC == CNIC && db.MeritActiveDesignations.Count(y => y.Desg_Id == x.Designation_Id && y.IsActive == "Y") > 0)
                    .OrderByDescending(x => x.Id).FirstOrDefault();


                return Ok(merit);
            }
            catch (Exception ex)
            {
                return Ok(false);
            }


        }


        [Route("GetMeritById/{id=id}")]
        [HttpGet]
        public IHttpActionResult GetMeritById(int id)
        {
            try
            {
                var merit = db.Merits.FirstOrDefault(x => x.Id == id);
                return Ok(merit);
            }
            catch (Exception ex)
            {
                return Ok(false);
            }
        }



        [Route("GetMerits/{Filter}/{CurrentPage}/{ItemsPerPage}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetMerits(string Filter, int CurrentPage, int ItemsPerPage)
        {
            try
            {
                int ItemsToSkip = (CurrentPage - 1) * ItemsPerPage;

                List<Merit> merits = null;
                int totalRecords = 0;
                if (Filter.Equals("Completed"))
                {
                    merits = db.Merits.Where(x => x.Status.Equals("Accepted") || x.Status.Equals("Completed")).OrderBy(x => x.Id).Skip((CurrentPage - 1) * ItemsPerPage).Take(ItemsPerPage).ToList();
                    totalRecords = db.Merits.Where(x => x.Status.Equals("Accepted") || x.Status.Equals("Completed")).Count();

                }
                else
                {
                    merits = db.Merits.Where(x => x.Status.Equals(Filter)).OrderBy(x => x.Id).Skip((CurrentPage - 1) * ItemsPerPage).Take(ItemsPerPage).ToList();
                    totalRecords = db.Merits.Where(x => x.Status.Equals(Filter)).Count();
                }
                return Ok(new { merits = merits, totalRecords = totalRecords });
            }
            catch (Exception ex)
            {
                return Ok(false);
            }


        }

        [Route("GetMeritsSearch/{CurrentPage}/{ItemsPerPage}/{Filter}/{Query}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetMeritsSearch(int CurrentPage, int ItemsPerPage, string Filter, string Query)
        {
            try
            {
                int ItemsToSkip = (CurrentPage - 1) * ItemsPerPage;
                int totalRecords = 0;
                IQueryable<Merit> merits = db.Merits.Where(x => x.CNIC == Query || x.Name.ToLower().Contains(Query.ToLower())).AsQueryable();

                if (Filter == "All")
                {
                    merits = db.Merits.OrderBy(x => x.Id).Skip((CurrentPage - 1) * ItemsPerPage).Take(ItemsPerPage);
                    totalRecords = merits.Count();
                }
                else
                {
                    Query = Query.Replace("-", "");
                    int meritNum;
                    if (int.TryParse(Query, out meritNum))
                    {
                        merits = merits.Where(x => x.MeritNumber == meritNum);
                    }

                    merits = merits.OrderBy(x => x.Id).Skip((CurrentPage - 1) * ItemsPerPage).Take(ItemsPerPage).AsQueryable();
                    totalRecords = merits.Count();
                }

                return Ok(new { merits = merits.ToList(), totalRecords = totalRecords });
            }
            catch (Exception ex)
            {
                return Ok(false);
            }


        }

        [Route("GetDownloadOfferLink/{CNIC}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetDownloadOfferLink(string CNIC)
        {
            try
            {

                var merit = db.Merits.Where(x => x.CNIC == CNIC).OrderByDescending(x => x.Id).FirstOrDefault();

                string key = Common.RandomString(5);
                merit.OfferLetterKey = key;
                merit.OfferLetterKeyExpiry = DateTime.UtcNow.AddHours(5).AddMinutes(30);
                db.SaveChanges();
                string url = "/api/JobApp/DownloadOfferLetter/" + merit.Id + "/" + merit.OfferLetterKey;

                return Ok(url);
            }
            catch (Exception ex)
            {
                return Ok(false);
            }
        }

        [Route("GetDownloadOfferLinkById/{Id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetDownloadOfferLinkById(int Id)
        {
            try
            {
                var merit = db.Merits.Where(x => x.Id == Id).FirstOrDefault();

                string key = Common.RandomString(5);
                merit.OfferLetterKey = key;
                merit.OfferLetterKeyExpiry = DateTime.UtcNow.AddHours(5).AddMinutes(30);
                db.SaveChanges();
                string url = "/api/JobApp/DownloadOfferLetter/" + merit.Id + "/" + merit.OfferLetterKey;

                return Ok(url);
            }
            catch (Exception ex)
            {
                return Ok(false);
            }
        }

        [AllowAnonymous]
        [Route("DownloadOfferLetter/{MeritNumber}/{OfferLetterKey}")]
        [HttpGet]
        public HttpResponseMessage DownloadOfferLetter(int MeritNumber, string OfferLetterKey)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            HttpResponseMessage result;
            try
            {

                var merit = db.Merits.Include(x => x.MeritActiveDesignation).Include(x => x.HrDesignation).Include(x => x.HrDesignation.HrScale).Where(x => x.Id == MeritNumber && x.OfferLetterKey == OfferLetterKey).OrderByDescending(x => x.Id).FirstOrDefault();

                var designationName = $"{merit.HrDesignation?.Name}";
                designationName += merit.HrDesignation?.HrScale != null ? $" (BS-{merit.HrDesignation?.HrScale?.Name}) " : string.Empty;

                if (merit == null) return Request.CreateResponse(HttpStatusCode.BadRequest, "Download link is recognized");
                var CurrentDT = DateTime.UtcNow.AddHours(5);
                if (merit.OfferLetterKeyExpiry < CurrentDT) return Request.CreateResponse(HttpStatusCode.BadRequest, "Download link expired");

                //string filepath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/Uploads/Offers/WmoOffer16-08-2017.docx"); 
                string filepath = System.Web.Hosting.HostingEnvironment.MapPath($@"~/Uploads/Offers/{merit.MeritActiveDesignation.OfferLetterFileName}");
                using (DocX document = DocX.Load(filepath))
                {
                    document.ReplaceText("{{EmployeeName}}", merit.Name);
                    document.ReplaceText("{{FatherName}}", merit.FatherName);
                    document.ReplaceText("{{MobileNumber}}", merit.MobileNumber);
                    document.ReplaceText("{{Address}}", merit.Address);
                    document.ReplaceText("{{DesignationName}}", designationName.ToUpper());
                    document.ReplaceText("{{DesignationNameLower}}", designationName);
                    document.ReplaceText("{{MeritNumber}}", merit.MeritNumber.ToString() ?? string.Empty);
                    document.ReplaceText("{{PPSCNumber}}", merit.PPSCNumber ?? string.Empty);
                    document.ReplaceText("{{PPSCDate}}", merit.PPSCDate?.ToString("D") ?? string.Empty);
                    document.ReplaceText("{{LetterNumber}}", merit.LetterNumber ?? string.Empty);
                    document.ReplaceText("{{LetterDate}}", merit.LetterDate?.ToString("dd/MM/yyyy") ?? string.Empty);
                    document.ReplaceText("{{SrNo}}", merit.PPSCSrNo == null ? string.Empty : $"PPSC Merit No: {merit.MeritNumber}");

                    using (var str = new MemoryStream())
                    {
                        document.SaveAs(str);
                        //byte[] dataFile = str.ToArray();
                        byte[] dataFile = SaveAsPdf(str);

                        result = Request.CreateResponse(HttpStatusCode.OK);
                        result.Content = new ByteArrayContent(dataFile);
                        result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                        {
                            FileName = merit.MeritNumber.ToString() + ".pdf"
                        };
                        merit.OfferLetterKey = null;
                        merit.OfferLetterKeyExpiry = null;
                        db.SaveChanges();
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.Gone, ex.Message);
            }
        }

        [AllowAnonymous]
        [Route("DownloadAdvertisement/{jobId}")]
        [HttpGet]
        public HttpResponseMessage DownloadAdvertisement(int jobId)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            HttpResponseMessage result;
            try
            {

                //var merit = db.Merits.Include(x => x.MeritActiveDesignation).Include(x => x.HrDesignation).Include(x => x.HrDesignation.HrScale).Where(x => x.Id == MeritNumber && x.OfferLetterKey == OfferLetterKey).OrderByDescending(x => x.Id).FirstOrDefault();
                var job = db.AdhocJobViews.FirstOrDefault(x => x.Id == jobId);
                if (job != null)
                {
                    //var designationName = $"{merit.HrDesignation?.Name}";
                    //designationName += merit.HrDesignation?.HrScale != null ? $" (BS-{merit.HrDesignation?.HrScale?.Name}) " : string.Empty;

                    //if (merit == null) return Request.CreateResponse(HttpStatusCode.BadRequest, "Download link is recognized");
                    var CurrentDT = DateTime.UtcNow.AddHours(5);
                    //if (merit.OfferLetterKeyExpiry < CurrentDT) return Request.CreateResponse(HttpStatusCode.BadRequest, "Download link expired");

                    //string filepath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/Uploads/Offers/WmoOffer16-08-2017.docx"); 
                    string filepath = System.Web.Hosting.HostingEnvironment.MapPath($@"~/Uploads/Offers/adhocAd.docx");
                    using (DocX document = DocX.Load(filepath))
                    {
                        document.ReplaceText("{{PostName}}", job.DesignationName);
                        document.ReplaceText("{{NoOfPosts}}", job.SeatsOpen.ToString());
                        document.ReplaceText("{{Qualification}}", "");
                        document.ReplaceText("{{ExpSkills}}", job.Experience.ToString());
                        document.ReplaceText("{{Salary}}", "");
                        document.ReplaceText("{{AgeLimit}}", job.AgeLimit.ToString());

                        using (var str = new MemoryStream())
                        {
                            document.SaveAs(str);
                            //byte[] dataFile = str.ToArray();
                            byte[] dataFile = SaveAsPdf(str);

                            result = Request.CreateResponse(HttpStatusCode.OK);
                            result.Content = new ByteArrayContent(dataFile);
                            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                            {
                                FileName = "adhocAd" + CurrentDT.ToString("dd/mm/yyyy") + ".pdf"
                            };
                            //merit.OfferLetterKey = null;
                            //merit.OfferLetterKeyExpiry = null;
                            //db.SaveChanges();
                            return result;
                        }
                    }
                }
                return new HttpResponseMessage();
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.Gone, ex.Message);
            }
        }

        [Route("GetDownloadAcceptanceLetterLink/{CNIC}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetDownloadAcceptanceLetterLink(string CNIC)
        {
            try
            {
                var merit = db.Merits.Where(x => x.CNIC == CNIC).OrderByDescending(x => x.Id).FirstOrDefault();

                string key = Common.RandomString(5);
                merit.OfferLetterKey = key;
                merit.OfferLetterKeyExpiry = DateTime.UtcNow.AddHours(5).AddMinutes(30);
                db.SaveChanges();

                string url = "/api/JobApp/DownloadAcceptanceLetter/" + merit.Id + "/" + merit.OfferLetterKey;

                return Ok(url);
            }
            catch (Exception ex)
            {
                return Ok(false);
            }
        }

        [Route("UpdateSMOSts")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateSMOSatus(PromotionApply promotionApply)
        {
            try
            {
                var promotionApplyDb = db.PromotionApplies.FirstOrDefault(x => x.Id == promotionApply.Id);
                if (promotionApplyDb != null)
                {
                    promotionApplyDb.ProfileReviewTime = DateTime.UtcNow.AddHours(5);
                    promotionApplyDb.Status = "ProfileReviewed";
                    db.Entry(promotionApplyDb).State = EntityState.Modified;
                    db.SaveChanges();
                    return Ok(promotionApplyDb);
                }
                return Ok(false);
            }
            catch (Exception ex)
            {
                return Ok(false);
            }
        }
        [Route("SubmitSMOApplication")]
        [HttpPost]
        public async Task<IHttpActionResult> SubmitSMOApplication(PromotionApply promotionApply)
        {
            try
            {
                var promotionApplyDb = db.PromotionApplies.FirstOrDefault(x => x.Id == promotionApply.Id);
                promotionApplyDb.ToDesignation_Id = promotionApply.ToDesignation_Id;
                promotionApplyDb.Status = "Completed";
                promotionApplyDb.CompletedTime = DateTime.UtcNow.AddHours(5);
                db.Entry(promotionApplyDb).State = EntityState.Modified;
                db.SaveChanges();
                return Ok(promotionApplyDb);
            }
            catch (Exception ex)
            {
                return Ok(false);
            }
        }
        [Route("UploadAcceptance/{id}")]
        public async Task<IHttpActionResult> UploadAcceptance(int id)
        {
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                string RootPath = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\Acceptances\";
                var dirPath = RootPath;

                var provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);

                CreateDirectoryIfNotExists(dirPath);
                string filename = "";
                Merit merit = db.Merits.FirstOrDefault(x => x.Id == id);
                if (File.Exists(HttpContext.Current.Server.MapPath(@"~\wwwroot\Uploads\Acceptances\" + merit.Id + "_OfferLetter.jpg")))
                {
                    File.Delete(HttpContext.Current.Server.MapPath(@"~\wwwroot\Uploads\Acceptances\" + merit.Id + "_OfferLetter.jpg"));
                }
                foreach (var file in provider.Contents)
                {
                    //filename = merit.Id + "_OfferLetter." + file.Headers.ContentDisposition.FileName.Trim('\"').Split('.')[1];
                    filename = merit.Id + "_OfferLetter.jpg";
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
                    bool prefExist = db.MeritPreferences.Where(x => x.Merit_Id == merit.Id).Count() > 0 ? true : false;
                    if (merit.Status.Equals("ProfileBuilt") || merit.Status.Equals("Existing"))
                    {
                        merit.Status = "Accepted";
                    }
                    if (prefExist)
                    {
                        merit.Status = "Completed";
                    }
                    db.Entry(merit).State = EntityState.Modified;

                    await db.SaveChangesAsync();
                }
                return Ok(new { result = true, src = filename, status = merit.Status });
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

        [AllowAnonymous]
        [Route("DownloadAcceptanceLetter/{MeritNumber}/{OfferLetterKey}")]
        [HttpGet]
        public HttpResponseMessage DownloadAcceptanceLetter(int MeritNumber, string OfferLetterKey)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            HttpResponseMessage result;
            try
            {

                var merit = db.Merits.Include(x => x.MeritActiveDesignation).Include(x => x.HrDesignation).Include(x => x.HrDesignation.HrScale).Where(x => x.Id == MeritNumber && x.OfferLetterKey == OfferLetterKey).OrderByDescending(x => x.Id).FirstOrDefault();


                if (merit == null) return Request.CreateResponse(HttpStatusCode.BadRequest, "Download link is recognized");
                var CurrentDT = DateTime.UtcNow.AddHours(5);
                if (merit.OfferLetterKeyExpiry < CurrentDT) return Request.CreateResponse(HttpStatusCode.BadRequest, "Download link expired");

                string filepath = System.Web.Hosting.HostingEnvironment.MapPath($@"~/Uploads/Offers/{merit.MeritActiveDesignation?.AcceptanceLetterFileName}");
                var activeDsg = db.MeritActiveDesignations.FirstOrDefault(x => x.Id == merit.MeritsActiveDesignationId);
                using (DocX document = DocX.Load(filepath))
                {

                    document.ReplaceText("{{EmployeeName}}", merit.Name ?? string.Empty);
                    document.ReplaceText("{{EmployeeName}}", merit.Name ?? string.Empty);
                    document.ReplaceText("{{FatherName}}", merit.FatherName ?? string.Empty);
                    document.ReplaceText("{{MeritNumber}}", merit.MeritNumber?.ToString() ?? string.Empty);
                    document.ReplaceText("{{MobileNumber}}", merit.MobileNumber ?? string.Empty);
                    document.ReplaceText("{{Address}}", merit.Address ?? string.Empty);
                    document.ReplaceText("{{Email}}", merit.Email ?? string.Empty);
                    document.ReplaceText("{{DesginationName}}", merit.HrDesignation?.Name?.ToUpper() ?? string.Empty);
                    document.ReplaceText("{{DesginationNameLower}}", merit.HrDesignation?.Name ?? string.Empty);
                    document.ReplaceText("{{ApplicationNo}}", merit?.ApplicationNumber?.ToString() ?? string.Empty);
                    if (activeDsg?.DateEnd < DateTime.Now)
                    {
                        document.ReplaceText("{{Dated}}", activeDsg?.DateEnd?.ToString("dd/MM/yyyy") ?? string.Empty);
                    }
                    else
                    {
                        document.ReplaceText("{{Dated}}", DateTime.UtcNow.AddHours(5).ToString("dd/MM/yyyy"));
                    }

                    using (var str = new MemoryStream())
                    {
                        document.SaveAs(str);
                        //byte[] dataFile = str.ToArray();
                        byte[] dataFile = SaveAsPdf(str);

                        result = Request.CreateResponse(HttpStatusCode.OK);
                        result.Content = new ByteArrayContent(dataFile);
                        result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                        {
                            FileName = "AcceptanceLetter.pdf"
                        };
                        merit.OfferLetterKey = null;
                        merit.OfferLetterKeyExpiry = null;
                        db.SaveChanges();
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.Gone, ex.Message);
            }
        }

        [AllowAnonymous]
        [Route("DownloadAcceptanceLetterSMO/{profile_Id}")]
        [HttpGet]
        public HttpResponseMessage DownloadAcceptanceLetterSMO(int profile_Id)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            HttpResponseMessage result;
            try
            {

                db.Configuration.ProxyCreationEnabled = false;
                var profile = db.ProfileDetailsViews.FirstOrDefault(x => x.Id == profile_Id);


                if (profile == null) return Request.CreateResponse(HttpStatusCode.BadRequest, "Download link is not recognized");
                var CurrentDT = DateTime.UtcNow.AddHours(5);

                string filepath = System.Web.Hosting.HostingEnvironment.MapPath($@"~/Uploads/Offers/OfferAcceptanceLetter.docx");
                using (DocX document = DocX.Load(filepath))
                {

                    document.ReplaceText("{{EmployeeName}}", profile.EmployeeName ?? string.Empty);
                    document.ReplaceText("{{FatherName}}", profile.FatherName ?? string.Empty);
                    using (var str = new MemoryStream())
                    {
                        document.SaveAs(str);
                        //byte[] dataFile = str.ToArray();
                        byte[] dataFile = SaveAsPdf(str);

                        result = Request.CreateResponse(HttpStatusCode.OK);
                        result.Content = new ByteArrayContent(dataFile);
                        result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                        {
                            FileName = "AcceptanceLetter.pdf"
                        };
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.Gone, ex.Message);
            }
        }

        [Route("UploadAcceptanceSMO/{id}")]
        public async Task<IHttpActionResult> UploadAcceptanceSMO(int id)
        {
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                string RootPath = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\SMOAcceptances\";
                var dirPath = RootPath;

                var provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);

                CreateDirectoryIfNotExists(dirPath);
                string filename = "";
                var promotionApply = db.PromotionApplies.FirstOrDefault(x => x.Id == id);
                if (File.Exists(HttpContext.Current.Server.MapPath(@"~\wwwroot\Uploads\SMOAcceptances\" + promotionApply.Id + "_AcceptanceLetter.jpg")))
                {
                    File.Delete(HttpContext.Current.Server.MapPath(@"~\wwwroot\Uploads\SMOAcceptances\" + promotionApply.Id + "_AcceptanceLetter.jpg"));
                }
                foreach (var file in provider.Contents)
                {
                    filename = promotionApply.Id + "_AcceptanceLetter.jpg";
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
                    promotionApply.AcceptancePath = filename;
                    promotionApply.Status = "Accepted";
                    promotionApply.AcceptanceTime = DateTime.UtcNow.AddHours(5);
                    db.Entry(promotionApply).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                return Ok(new { result = true, src = filename });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("AcceptanceFileExists/{MeritId}")]
        [HttpGet]
        public IHttpActionResult AcceptanceFileExists(int MeritId)
        {
            try
            {
                var merit = db.Merits.FirstOrDefault(x => x.Id == MeritId);
                bool fileExisit = File.Exists(HttpContext.Current.Server.MapPath(@"~\Uploads\Acceptances\" + MeritId + "_OfferLetter.jpg"));
                bool prefExist = db.MeritPreferences.Where(x => x.Merit_Id == merit.Id).Count() > 0 ? true : false;

                if (fileExisit && prefExist)
                {
                    merit.Status = "Completed";
                    db.SaveChanges();
                }
                else if (fileExisit && !prefExist)
                {
                    merit.Status = "Accepted";
                    db.SaveChanges();
                }
                else if (!fileExisit)
                {
                    merit.Status = "ProfileBuilt";
                    db.SaveChanges();
                }
                return Ok(new { fileExisit, merit });
            }
            catch (Exception ex)
            {
                return Ok(false);
            }


        }


        [Route("UpdateMeritStatus/{MeritId}/{Status}")]
        [HttpGet]
        public async Task<IHttpActionResult> UpdateMeritStatus(int MeritId, string Status)
        {
            try
            {
                var merit = db.Merits.FirstOrDefault(x => x.Id == MeritId);
                var desg = db.HrDesignations.FirstOrDefault(x => x.Id == merit.Designation_Id);
                merit.Status = Status;
                db.SaveChanges();

                string message = "";
                switch (Status)
                {
                    case "Accepted":

                        message = $"You have accepted the offer for Posting of {desg?.Name}";
                        break;

                    case "Acknowledged":

                        message = $"Your application for Posting of {desg?.Name} has been acknowledged.";

                        break;

                    default:
                        message = "You request has been sucessfully submitted. Thank you";
                        break;
                }
                var profile = db.HrProfiles.FirstOrDefault(x => x.CNIC == merit.CNIC);
                Common.SMS_Send(new List<SMS>() {
                            new SMS()
                                {
                                    MobileNumber = profile.MobileNo,
                                    Message = message
                                }
                            });

                try
                {
                    Common.SendEmail(profile.EMaiL, "Primary Secondary Healthcare Department", message);
                }
                catch (Exception ex)
                {


                }


                return Ok(true);
            }
            catch (Exception ex)
            {
                return Ok(false);
            }


        }

        [Route("SavePgPreference/{meritId}/{meritPgId=meritPgId}/{dsitrictId=dsitrictId}/{districtCode=districtCode}")]
        [HttpGet]

        public IHttpActionResult SavePgPreference(int meritId, int meritPgId, int dsitrictId, string districtCode)
        {

            try
            {
                var merit = db.Merits.FirstOrDefault(x => x.Id == meritId);
                var preferencesOrder = db.MeritPGDistricts.Where(x => x.Merit_Id == meritId).Max(x => x.PrefrencesOrder) ?? 0;
                int totalPrefs = db.MeritPGDistricts.Where(x => x.Merit_Id == merit.Id).Count();
                if (db.MeritPGDistricts.Count(x => x.Merit_Id == meritId && x.Districts_Id == dsitrictId) > 0) return Ok(true);
                if (totalPrefs < 10)
                {
                    db.MeritPGDistricts.Add(new MeritPGDistrict
                    {
                        Merit_Id = meritId,
                        MeritPG_Id = meritPgId,
                        DateCreated = DateTime.UtcNow.AddHours(5),
                        DistrictCode = districtCode,
                        Districts_Id = dsitrictId,
                        PrefrencesOrder = ++preferencesOrder
                    });
                    db.SaveChanges();
                    totalPrefs++;
                }

                if (totalPrefs == 10)
                {
                    merit.Status = "Completed";
                    db.Entry(merit).State = EntityState.Modified;
                    db.SaveChanges();
                    string message = "Your preferences has been saved successfully. Thank you.";
                    var profile = db.HrProfiles.FirstOrDefault(x => x.CNIC == merit.CNIC);
                    Common.SMS_Send(new List<SMS>() {
                            new SMS()
                                {
                                    MobileNumber = profile.MobileNo,
                                    Message = message
                                }
                            });

                    string message2 = "You have completed all the steps successfully. For further information please visit our website.";
                    Common.SMS_Send(new List<SMS>() {
                            new SMS()
                                {
                                    MobileNumber = profile.MobileNo,
                                    Message = message2
                                }
                            });

                    try
                    {
                        Common.SendEmail(profile.EMaiL, "Primary Secondary Healthcare Department", message2);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                return Ok(true);

            }
            catch (Exception)
            {

                return Ok(false);
            }
        }

        [Route("SavePreferences/{MeritId}/{hfmisCode=hfmisCode}/{hfId=hfId}")]
        [HttpGet]
        public IHttpActionResult SavePreferences(int MeritId, string hfmisCode, int? hfId)
        {
            try
            {
                if (hfId == 0)
                {
                    hfId = null;
                }
                if (db.MeritPreferences.Count(x => x.Merit_Id == MeritId && x.HfmisCode == hfmisCode && x.HF_Id == hfId) > 0)
                {
                    return Ok(new { result = true, meritPrefs = GetPreferencesHealthFacilities(MeritId) });
                };

                List<MeritPreference> listy = new List<MeritPreference>();
                var merit = db.Merits.FirstOrDefault(x => x.Id == MeritId);
                var preferencesOrder = db.MeritPreferences.Where(x => x.Merit_Id == MeritId).Max(x => x.PrefrencesOrder) ?? 0;
                int totalPrefs = db.MeritPreferences.Where(x => x.Merit_Id == merit.Id).Count();

                db.MeritPreferences.Add(new MeritPreference
                {
                    Merit_Id = MeritId,
                    HfmisCode = hfmisCode,
                    HF_Id = hfId,
                    PrefrencesOrder = ++preferencesOrder,
                    IsActive = true
                });
                db.SaveChanges();
                totalPrefs++;

                merit.Status = "Completed";
                db.Entry(merit).State = EntityState.Modified;
                db.SaveChanges();
                string message = "Your preferences has been saved successfully. Thank you.";
                //Common.SMS_Send(new List<SMS>() {
                //        new SMS()
                //            {
                //                MobileNumber = merit.MobileNumber,
                //                Message = message
                //            }
                //        });

                string message2 = "You have completed all the steps successfully. For further information please visit our website.";
                //Common.SMS_Send(new List<SMS>() {
                //        new SMS()
                //            {
                //                MobileNumber = merit.MobileNumber,
                //                Message = message2
                //            }
                //        });

                //try
                //{
                //    Common.SendEmail(merit.Email, "Primary Secondary Healthcare Department", message2);
                //}
                //catch (Exception ex)
                //{

                //}

                return Ok(new { result = true, meritPrefs = GetPreferencesHealthFacilities(MeritId) });
            }
            catch (Exception ex)
            {
                return Ok(false);
            }
        }

        [Route("SavePreferencesFinal/{MeritId}")]
        [HttpGet]
        public IHttpActionResult SavePreferencesFinal(int MeritId)
        {
            try
            {
                List<MeritPreference> meritPrefs = db.MeritPreferences.Where(x => x.Merit_Id == MeritId && x.IsActive == true).OrderBy(x => x.PrefrencesOrder).ToList();
                List<MeritPreferencesFinal> meritPrefsFinal = db.MeritPreferencesFinals.Where(x => x.Merit_Id == MeritId && x.IsActive == true).OrderBy(x => x.PrefrencesOrder).ToList();
                if (meritPrefsFinal.Count > 0)
                {
                    foreach (var preference in meritPrefsFinal)
                    {
                        preference.IsActive = false;
                        db.Entry(preference).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                foreach (var preference in meritPrefs)
                {
                    db.MeritPreferencesFinals.Add(new MeritPreferencesFinal
                    {
                        Merit_Id = MeritId,
                        HfmisCode = preference.HfmisCode,
                        HF_Id = preference.HF_Id,
                        PrefrencesOrder = preference.PrefrencesOrder,
                        IsActive = preference.IsActive
                    });
                    db.SaveChanges();
                }
                return Ok(new { result = true, meritPrefs = GetPreferencesHealthFacilities(MeritId) });
            }
            catch (Exception ex)
            {
                return Ok(false);
            }
        }
        private List<HealthFacilityDetail> GetPreferencesHealthFacilities(int meritId)
        {
            List<HealthFacilityDetail> hfs = new List<HealthFacilityDetail>();
            List<MeritPreference> meritPrefs = db.MeritPreferences.Where(x => x.Merit_Id == meritId).ToList();
            foreach (var prefs in meritPrefs)
            {
                HealthFacilityDetail hf = db.HealthFacilityDetails.FirstOrDefault(x => x.HFMISCode.Equals(prefs.HfmisCode));
                hfs.Add(hf);
            }
            return hfs;
        }

        [HttpGet]
        [Route("GetPostingPlan/{meritActiveDsgId=meritActiveDsgId}/{query=query}")]
        public IHttpActionResult GetPostingPlan(int meritActiveDsgId, string query)
        {
            try
            {
                List<ViewMeritsVp> applicants = new List<ViewMeritsVp>();
                if (!string.IsNullOrWhiteSpace(query))
                {
                    int value;
                    if (int.TryParse(query, out value))
                    {
                        applicants = db.ViewMeritsVps.Where(x => x.MeritsActiveDesignationId == meritActiveDsgId && (x.MeritNumber == value || x.ApplicationNumber == value)).OrderBy(x => x.MeritNumber).ToList();
                    }
                    else
                    {
                        query = query.ToLower();
                        applicants = db.ViewMeritsVps.Where(x => x.MeritsActiveDesignationId == meritActiveDsgId && (x.CNIC.ToLower().Contains(query) || x.Name.ToLower().Contains(query) || x.MobileNumber.ToLower().Contains(query) || x.FatherName.ToLower().Contains(query) || x.DomicileDistrict.ToLower().Contains(query) || x.FullName.ToLower().Contains(query))).OrderBy(x => x.MeritNumber).ToList();
                    }
                }
                else
                {
                    applicants = db.ViewMeritsVps.Where(x => x.MeritsActiveDesignationId == meritActiveDsgId).OrderBy(x => x.MeritNumber).ToList();
                }
                return Ok(applicants);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetMerits")]
        public IHttpActionResult GetMerits([FromBody] MeritsFilter filter)
        {
            try
            {
                //var cnics = GetDAMerits();
                IQueryable<MeritsView> applicants = db.MeritsViews.AsQueryable();
                if (!string.IsNullOrWhiteSpace(filter.Query))
                {
                    int value;
                    if (int.TryParse(filter.Query, out value))
                    {
                        applicants = applicants.Where(x => (x.MeritNumber == value || x.ApplicationNumber == value)).OrderBy(x => x.MeritNumber).AsQueryable();
                    }
                    else
                    {
                        filter.Query = filter.Query.ToLower();
                        applicants = applicants.Where(x => x.CNIC.ToLower().Contains(filter.Query)
                        || x.Name.ToLower().Contains(filter.Query)
                        || x.MobileNumber.ToLower().Contains(filter.Query)
                        || x.FatherName.ToLower().Contains(filter.Query)
                        //|| x.DomicileDistrict.ToLower().Contains(filter.Query)
                        //|| x.FullName.ToLower().Contains(filter.Query)
                        )
                        .OrderBy(x => x.MeritNumber).AsQueryable();
                    }
                }
                if (filter.ActiveDesignationId != 0)
                {
                    applicants = applicants.Where(x => x.MeritsActiveDesignationId == filter.ActiveDesignationId).AsQueryable();
                }
                if (filter.DesignationId != 0)
                {
                    applicants = applicants.Where(x => x.Designation_Id == filter.DesignationId).AsQueryable();
                }
                if (filter.SpecialQuota == true)
                {
                    var Count = applicants.Count();
                    var List = applicants.OrderByDescending(x => x.IsDisabled).ThenBy(x => x.MeritNumber).Skip(filter.Skip).Take(filter.PageSize).ToList();
                    var postings = db.usp_MeritPosting().OrderBy(x => x.MeritNumber).Skip(filter.Skip).Take(filter.PageSize).ToList();
                    var summary = applicants.GroupBy(x => new
                    {
                        x.Status
                    }).Select(l => new MeritStats
                    {
                        Name = l.Key.Status,
                        Count = l.Count()
                    }).ToList();
                    return Ok(new { List, Count, postings, summary });
                }
                else
                {
                    var Count = applicants.Count();
                    var List = applicants.OrderBy(x => x.MeritNumber).Skip(filter.Skip).Take(filter.PageSize).ToList();
                    var postings = db.usp_MeritPosting().OrderBy(x => x.MeritNumber).Skip(filter.Skip).Take(filter.PageSize).ToList();
                    var summary = applicants.GroupBy(x => new
                    {
                        x.Status
                    }).Select(l => new MeritStats
                    {
                        Name = l.Key.Status,
                        Count = l.Count()
                    }).ToList();
                    return Ok(new { List, Count, postings, summary });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("GetMeritsByCNIC")]
        public IHttpActionResult GetMeritsByCNIC([FromBody] MeritsFilter filter)
        {
            try
            {
                var cnics = GetDAMerits2();
                IQueryable<MeritsView> applicants = db.MeritsViews.Where(x => cnics.Contains(x.CNIC)).AsQueryable();
                if (!string.IsNullOrWhiteSpace(filter.Query))
                {
                    int value;
                    if (int.TryParse(filter.Query, out value))
                    {
                        applicants = applicants.Where(x => (x.MeritNumber == value || x.ApplicationNumber == value)).OrderBy(x => x.MeritNumber).AsQueryable();
                    }
                    else
                    {
                        filter.Query = filter.Query.ToLower();
                        applicants = applicants.Where(x => x.CNIC.ToLower().Contains(filter.Query)
                        || x.Name.ToLower().Contains(filter.Query)
                        || x.MobileNumber.ToLower().Contains(filter.Query)
                        || x.FatherName.ToLower().Contains(filter.Query)
                        //|| x.DomicileDistrict.ToLower().Contains(filter.Query)
                        //|| x.FullName.ToLower().Contains(filter.Query)
                        )
                        .OrderBy(x => x.MeritNumber).AsQueryable();
                    }
                }
                var Count = applicants.Count();
                var List = applicants.OrderBy(x => x.MeritNumber).Skip(filter.Skip).Take(filter.PageSize).ToList();
                var postings = db.usp_MeritPosting().OrderBy(x => x.MeritNumber).Skip(filter.Skip).Take(filter.PageSize).ToList();
                var summary = applicants.GroupBy(x => new
                {
                    x.Status
                }).Select(l => new MeritStats
                {
                    Name = l.Key.Status,
                    Count = l.Count()
                }).ToList();
                return Ok(new { List, Count, postings, summary });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetMeritPreferences/{MeritID}")]
        public IHttpActionResult GetMeritPreferences(int MeritID)
        {
            try
            {
                var preferences = db.ViewMeritPreferences.Where(x => x.Merit_Id == MeritID).OrderBy(x => x.PrefrencesOrder).ToList();

                return Ok(preferences);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetMeritPosting/{MeritID}")]
        public IHttpActionResult GetMeritPosting(int MeritID)
        {
            try
            {
                var meritPosting = db.MeritPostingViews.FirstOrDefault(x => x.Merit_Id == MeritID);

                return Ok(meritPosting);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetAwaitingPosting/{MeritID}")]
        public IHttpActionResult GetAwaitingPosting(int MeritID)
        {
            try
            {
                var meritPosting = db.AwaitingPostingViews.FirstOrDefault(x => x.Merit_Id == MeritID);

                return Ok(meritPosting);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetHrPosting/{CNIC}")]
        public IHttpActionResult GetHrPosting(string CNIC)
        {
            try
            {
                var meritPosting = db.HrPostingViews.FirstOrDefault(x => x.CNIC == CNIC);

                return Ok(meritPosting);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetPostedMerits/{HfmisCode}/{desigId}/{activedesigId}")]
        public IHttpActionResult GetPostedMerits(string HfmisCode, int desigId, int activedesigId)
        {
            try
            {
                var meritPosting = db.MeritPostingViews.Where(x => x.PostingHFMISCode.Equals(HfmisCode) && x.Designation_Id == desigId && x.MeritsActiveDesignationId == activedesigId && x.IsActive == true).OrderBy(x => x.MeritNumber).ToList();
                if (HfmisCode.Substring(12, 3) == "014")
                {
                    desigId = 2404;
                }
                var vpMaster = db.VpMeritPreferenceViews.FirstOrDefault(x => x.HFMISCode.Equals(HfmisCode) && x.Desg_Id == desigId);
                if (vpMaster != null)
                {
                    var vpDetails = db.VpDViews.Where(x => x.Master_Id == vpMaster.Id).ToList();
                    return Ok(new { meritPosting, vpMaster, vpDetails });
                }
                else
                {
                    if (HfmisCode.Substring(12, 3) == "015")
                    {
                        desigId = 2404;
                    }
                    vpMaster = db.VpMeritPreferenceViews.FirstOrDefault(x => x.HFMISCode.Equals(HfmisCode) && x.Desg_Id == desigId);
                    if (vpMaster != null)
                    {
                        var vpDetails = db.VpDViews.Where(x => x.Master_Id == vpMaster.Id).ToList();
                        return Ok(new { meritPosting, vpMaster, vpDetails });
                    }
                }
                return Ok(new { meritPosting, vpMaster });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("PostMerit/{Designation_Id}/{MeritsActiveDesignationId}")]
        public IHttpActionResult PostMerit(int Designation_Id, int MeritsActiveDesignationId)
        {
            try
            {
                if (Designation_Id != 0)
                {
                    var postings = db.MeritPostings.Where(x => x.Designation_Id == Designation_Id && x.MeritsActiveDesignationId == MeritsActiveDesignationId).ToList();
                    db.MeritPostings.RemoveRange(postings);
                    db.SaveChanges();
                    var merits = db.Merits.Where(x => x.MeritsActiveDesignationId == MeritsActiveDesignationId && x.Designation_Id == Designation_Id).OrderByDescending(k => k.IsDisabled).ThenBy(y => y.MeritNumber).ToList();
                    foreach (var merit in merits)
                    {
                        if (merit.MeritNumber == 38)
                        {

                        }
                        else
                        {
                            if (merit != null && (merit.Status.Equals("Accepted") || merit.Status.Equals("Completed")))
                            {
                                string message = "Merit No. " + merit.MeritNumber + ", " + merit.Name + " " + merit.FatherName + ", ";
                                var preferences = db.MeritPreferences.Where(x => x.Merit_Id == merit.Id && !x.HfmisCode.StartsWith("035002")).OrderBy(k => k.PrefrencesOrder).ToList();
                                foreach (var preference in preferences)
                                {
                                    var postedBefore = db.MeritPostings.Where(x => x.Designation_Id == merit.Designation_Id && x.PostingHFMISCode.Equals(preference.HfmisCode) && x.IsActive == true && x.MeritsActiveDesignationId == MeritsActiveDesignationId).ToList();
                                    var postedBeforeAdhoc = db.MeritPostings.Where(x => x.Designation_Id == merit.Designation_Id && x.PostingHFMISCode.Equals(preference.HfmisCode) && x.IsActive == true && x.MeritsActiveDesignationId == MeritsActiveDesignationId && x.IsOnAdhocee == true).ToList();
                                    var postedHF = db.HFOpenedViews.FirstOrDefault(x => x.Designation_Id == merit.Designation_Id && x.HFMISCode.Equals(preference.HfmisCode) && x.IsActive == true && x.MeritsActiveDesignationId == MeritsActiveDesignationId);
                                    if (postedHF != null)
                                    {
                                        if (postedHF.Seats > postedBefore.Count)
                                        {
                                            message += "Preference No." + preference.PrefrencesOrder + ", ";
                                            MeritPosting meritPosting = new MeritPosting();
                                            meritPosting.Merit_Id = merit.Id;
                                            meritPosting.Designation_Id = merit.Designation_Id;
                                            meritPosting.PostingHF_Id = postedHF.HF_Id;
                                            meritPosting.MeritsActiveDesignationId = MeritsActiveDesignationId;
                                            meritPosting.PostingHFMISCode = postedHF.HFMISCode;
                                            meritPosting.PostingHFSeatNumber = postedBefore.Count + 1;
                                            message += "Seat No. " + (postedBefore.Count + 1);
                                            meritPosting.PostingHFTotalSeats = postedHF.Seats;
                                            meritPosting.PostingHFAdhocSeats = postedHF.SeatsAdhoc;
                                            meritPosting.HFOpened_Id = postedHF.Id;
                                            meritPosting.Preference_Id = preference.id;
                                            meritPosting.PreferencesNumber = preference.PrefrencesOrder;
                                            meritPosting.IsOnAdhocee = false;
                                            meritPosting.IsActive = true;
                                            meritPosting.DateTime = DateTime.UtcNow.AddHours(5);
                                            db.MeritPostings.Add(meritPosting);
                                            meritPosting.Remarks = message;
                                            db.SaveChanges();
                                            break;
                                        }
                                        else if (postedHF.SeatsAdhoc > postedBeforeAdhoc.Count)
                                        {
                                            message += "Preference No." + preference.PrefrencesOrder + ", ";
                                            MeritPosting meritPosting = new MeritPosting();
                                            meritPosting.Merit_Id = merit.Id;
                                            meritPosting.Designation_Id = merit.Designation_Id;
                                            meritPosting.MeritsActiveDesignationId = MeritsActiveDesignationId;
                                            meritPosting.PostingHF_Id = postedHF.HF_Id;
                                            meritPosting.PostingHFMISCode = postedHF.HFMISCode;
                                            meritPosting.PostingHFSeatNumber = postedBefore.Count + 1;
                                            message += "Adhoc Seat No. " + (postedBefore.Count + 1);
                                            meritPosting.PostingHFTotalSeats = postedHF.Seats;
                                            meritPosting.PostingHFAdhocSeats = postedHF.SeatsAdhoc;
                                            meritPosting.HFOpened_Id = postedHF.Id;
                                            meritPosting.Preference_Id = preference.id;
                                            meritPosting.PreferencesNumber = preference.PrefrencesOrder;
                                            meritPosting.IsOnAdhocee = true;
                                            meritPosting.IsActive = true;
                                            meritPosting.DateTime = DateTime.UtcNow.AddHours(5);
                                            db.MeritPostings.Add(meritPosting);
                                            meritPosting.Remarks = message;
                                            db.SaveChanges();
                                            break;
                                        }
                                        //else
                                        //{
                                        //    List<int> highMerits = postedBefore.Select(x => x.Id).ToList();
                                        //    var higherMerits = db.MeritsViews.Where(x => highMerits.Contains(x.Id));
                                        //    foreach (var hMerit in higherMerits)
                                        //    {
                                        //        message = hMerit.Name + ", " + hMerit.FatherName + " (" + hMerit.MeritNumber + ") already posted at preference " + preference.PrefrencesOrder + " : " + postedHF.FullName + "/n";
                                        //    }
                                        //}
                                    }
                                }
                            }
                        }
                    }

                }
                //else
                //{
                //    var merit = db.Merits.FirstOrDefault(x => x.Id == MeritID);
                //    if (merit != null)
                //    {
                //        var preferences = db.MeritPreferences.Where(x => x.Merit_Id == merit.Id).OrderBy(k => k.PrefrencesOrder).ToList();
                //        foreach (var preference in preferences)
                //        {
                //            var postedBefore = db.MeritPostings.Where(x => x.Designation_Id == merit.Designation_Id && x.PostingHFMISCode.Equals(preference.HfmisCode) && x.IsActive == true).ToList();
                //            var postedHF = db.HFOpenedViews.FirstOrDefault(x => x.Designation_Id == merit.Designation_Id && x.HFMISCode.Equals(preference.HfmisCode) && x.IsActive == true);
                //            if (postedHF != null)
                //            {
                //                if (postedHF.Seats > postedBefore.Count)
                //                {
                //                    MeritPosting meritPosting = new MeritPosting();
                //                    meritPosting.Merit_Id = merit.Id;
                //                    meritPosting.Designation_Id = merit.Designation_Id;
                //                    meritPosting.PostingHF_Id = postedHF.HF_Id;
                //                    meritPosting.PostingHFMISCode = postedHF.HFMISCode;
                //                    meritPosting.PostingHFSeatNumber = postedBefore.Count + 1;
                //                    meritPosting.PostingHFTotalSeats = postedHF.Seats;
                //                    meritPosting.Preference_Id = preference.id;
                //                    meritPosting.PreferencesNumber = preference.PrefrencesOrder;
                //                    meritPosting.IsActive = true;
                //                    meritPosting.DateTime = DateTime.UtcNow.AddHours(5);
                //                    db.MeritPostings.Add(meritPosting);
                //                    db.SaveChanges();
                //                    break;
                //                }
                //            }
                //        }
                //    }
                //}

                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("PostSingleMerit/{MeritId}")]
        public IHttpActionResult PostSingleMerit(int MeritId)
        {
            try
            {
                if (MeritId > 0)
                {
                    //var postings = db.MeritPostings.Where(x => x.Designation_Id == Designation_Id).ToList();
                    //db.MeritPostings.RemoveRange(postings);
                    //db.SaveChanges();
                    var merit = db.Merits.FirstOrDefault(x => x.Id == MeritId);
                    if (merit != null && (merit.Status.Equals("Accepted") || merit.Status.Equals("Completed")))
                    {
                        string message = "Merit No. " + merit.MeritNumber + (merit.IsDisabled == true ? " (Special Quota)" : "") + ", " + merit.Name + " " + merit.FatherName + ", ";
                        var preferences = db.MeritPreferences.Where(x => x.Merit_Id == merit.Id).OrderBy(k => k.PrefrencesOrder).ToList();
                        foreach (var preference in preferences)
                        {
                            var postedBefore = db.MeritPostings.Where(x => x.Designation_Id == merit.Designation_Id && x.PostingHFMISCode.Equals(preference.HfmisCode) && x.IsActive == true).ToList();
                            var postedHF = db.HFOpenedViews.FirstOrDefault(x => x.Designation_Id == merit.Designation_Id && x.HFMISCode.Equals(preference.HfmisCode) && x.IsActive == true);
                            if (postedHF != null)
                            {
                                if (postedHF.Seats > postedBefore.Count)
                                {
                                    message += "Preference No." + preference.PrefrencesOrder + ", ";
                                    MeritPosting meritPosting = new MeritPosting();
                                    meritPosting.Merit_Id = merit.Id;
                                    meritPosting.Designation_Id = merit.Designation_Id;
                                    meritPosting.PostingHF_Id = postedHF.HF_Id;
                                    meritPosting.PostingHFMISCode = postedHF.HFMISCode;
                                    meritPosting.PostingHFSeatNumber = postedBefore.Count + 1;
                                    message += "Seat No. " + (postedBefore.Count + 1);
                                    meritPosting.PostingHFTotalSeats = postedHF.Seats;
                                    meritPosting.HFOpened_Id = postedHF.Id;
                                    meritPosting.Preference_Id = preference.id;
                                    meritPosting.PreferencesNumber = preference.PrefrencesOrder;
                                    meritPosting.IsActive = true;
                                    meritPosting.DateTime = DateTime.UtcNow.AddHours(5);
                                    db.MeritPostings.Add(meritPosting);
                                    meritPosting.Remarks = message;
                                    db.SaveChanges();
                                    break;
                                }
                                //else
                                //{
                                //    List<int> highMerits = postedBefore.Select(x => x.Id).ToList();
                                //    var higherMerits = db.MeritsViews.Where(x => highMerits.Contains(x.Id));
                                //    foreach (var hMerit in higherMerits)
                                //    {
                                //        message = hMerit.Name + ", " + hMerit.FatherName + " (" + hMerit.MeritNumber + ") already posted at preference " + preference.PrefrencesOrder + " : " + postedHF.FullName + "/n";
                                //    }
                                //}
                            }
                        }
                    }

                }


                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("PostMeritManually/{MeritID}/{HF_Id}/{HFMISCode}")]
        public IHttpActionResult PostMeritManually(int MeritID, int HF_Id, string HFMISCode)
        {
            try
            {
                var merit = db.Merits.FirstOrDefault(x => x.Id == MeritID);
                //if (merit != null && (merit.Status.Equals("Accepted") || merit.Status.Equals("Completed")))
                if (merit != null)
                {
                    string message = "Merit No. " + merit.MeritNumber + ", " + merit.Name + " " + merit.FatherName + ", ";
                    if (HF_Id == 0)
                    {
                        var disposalDistrict = db.Districts.FirstOrDefault(x => x.Code == HFMISCode);
                        int? designationId = merit.Designation_Id;
                        var postedBefore = db.MeritPostings.Where(x => x.Designation_Id == merit.Designation_Id && x.PostingHFMISCode.Equals(disposalDistrict.Code) && x.IsActive == true).ToList();
                        var preference = db.MeritPreferences.FirstOrDefault(x => x.Merit_Id == merit.Id && x.HfmisCode.Equals(disposalDistrict.Code));
                        if (preference != null)
                        {
                            var meritPosting = db.MeritPostings.FirstOrDefault(x => x.Merit_Id == MeritID);
                            if (meritPosting == null)
                            {
                                meritPosting = new MeritPosting();
                                meritPosting.Merit_Id = merit.Id;
                                meritPosting.Designation_Id = merit.Designation_Id;
                            }
                            meritPosting.MeritsActiveDesignationId = 68;
                            message += "Preference No." + preference.PrefrencesOrder + ", ";
                            meritPosting.PostingHFMISCode = disposalDistrict.Code;
                            meritPosting.IsPG = true;
                            meritPosting.DistrictCode = disposalDistrict.Code;
                            meritPosting.PostingHFSeatNumber = postedBefore.Count + 1;
                            message += "Seat No. " + meritPosting.PostingHFSeatNumber;
                            meritPosting.ActualPostingHFMISCode = disposalDistrict.Code;
                            meritPosting.PreferencesNumber = preference.PrefrencesOrder;
                            meritPosting.Preference_Id = preference.id;
                            meritPosting.IsActive = true;
                            meritPosting.DateTime = DateTime.UtcNow.AddHours(5);
                            meritPosting.Remarks = message;
                            meritPosting.IsOnAdhocee = false;
                            meritPosting.ESR_Id = 1;
                            if (meritPosting.Id == 0)
                            {
                                db.MeritPostings.Add(meritPosting);
                            }
                            else
                            {
                                db.Entry(meritPosting).State = EntityState.Modified;
                            }
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        var postedHF = db.HFOpenedViews.FirstOrDefault(x => x.Designation_Id == merit.Designation_Id && x.HF_Id == HF_Id && x.IsActive == true);
                        int? designationId = merit.Designation_Id;
                        if (postedHF.HFMISCode.Substring(12, 3) == "014")
                        {
                            designationId = 2404;
                        }
                        var vPMaster = db.VpMeritPreferenceViews.Where(x => x.HFMISCode.Equals(postedHF.HFMISCode) && x.Desg_Id == designationId)?.FirstOrDefault();
                        if (vPMaster != null)
                        {
                            if (vPMaster.Vacant + vPMaster.Adhoc <= 0)
                            {
                                return Ok(false);
                            }
                        }
                        else
                        {
                            if (postedHF.HFMISCode.Substring(12, 3) == "015")
                            {
                                designationId = 2404;
                            }
                            vPMaster = db.VpMeritPreferenceViews.FirstOrDefault(x => x.HFMISCode.Equals(postedHF.HFMISCode) && x.Desg_Id == designationId);
                            if (vPMaster != null)
                            {
                                if (vPMaster.Vacant + vPMaster.Adhoc <= 0)
                                {
                                    return Ok(false);
                                }
                            }
                        }
                        var postedBefore = db.MeritPostings.Where(x => x.Designation_Id == merit.Designation_Id && x.PostingHFMISCode.Equals(postedHF.HFMISCode) && x.IsActive == true).ToList();
                        var preference = db.MeritPreferences.FirstOrDefault(x => x.Merit_Id == merit.Id && x.HfmisCode.Equals(postedHF.HFMISCode));
                        if (postedHF != null)
                        {
                            if (preference == null)
                            {
                                int preferenceOrder = 0;
                                var preferences = db.MeritPreferences.Where(x => x.Merit_Id == merit.Id).OrderByDescending(k => k.PrefrencesOrder).Select(k => k.PrefrencesOrder).ToList();
                                if (preferences.Count > 0) { preferenceOrder = (int)preferences.Max(); }
                                preferenceOrder += 1;
                                preference = new MeritPreference
                                {
                                    Merit_Id = merit.Id,
                                    HfmisCode = postedHF.HFMISCode,
                                    HF_Id = postedHF.HF_Id,
                                    PrefrencesOrder = preferenceOrder,
                                    IsActive = true
                                };
                                db.MeritPreferences.Add(preference);
                                db.SaveChanges();
                            }
                            var meritPosting = db.MeritPostings.FirstOrDefault(x => x.Merit_Id == MeritID);
                            if (meritPosting == null)
                            {
                                meritPosting = new MeritPosting();
                                meritPosting.Merit_Id = merit.Id;
                                meritPosting.Designation_Id = merit.Designation_Id;
                            }
                            if (vPMaster.Vacant <= 0 && vPMaster.Adhoc > 0)
                            {
                                meritPosting.IsOnAdhocee = true;
                            }
                            else
                            {
                                meritPosting.IsOnAdhocee = false;
                            }
                            meritPosting.MeritsActiveDesignationId = 68;
                            message += "Preference No." + preference.PrefrencesOrder + ", ";
                            meritPosting.PostingHF_Id = postedHF.HF_Id;
                            meritPosting.PostingHFMISCode = postedHF.HFMISCode;
                            meritPosting.PostingHFSeatNumber = postedBefore.Count + 1;
                            message += "Seat No. " + meritPosting.PostingHFSeatNumber;
                            meritPosting.PostingHFTotalSeats = postedHF.Seats;
                            meritPosting.ActualPostingHF_Id = postedHF.HF_Id;
                            meritPosting.ActualPostingHFMISCode = postedHF.HFMISCode;
                            meritPosting.PostingHFTotalSeats = postedHF.Seats;
                            meritPosting.PreferencesNumber = preference.PrefrencesOrder;
                            meritPosting.Preference_Id = preference.id;
                            meritPosting.IsActive = true;
                            meritPosting.DateTime = DateTime.UtcNow.AddHours(5);
                            meritPosting.Remarks = message;
                            meritPosting.ESR_Id = 0;
                            if (meritPosting.Id == 0)
                            {
                                db.MeritPostings.Add(meritPosting);
                            }
                            else
                            {
                                db.Entry(meritPosting).State = EntityState.Modified;
                            }
                            db.SaveChanges();

                            //var preferences = db.MeritPreferences.Where(x => x.Merit_Id == merit.Id).OrderBy(k => k.PrefrencesOrder).ToList();
                            //foreach (var preference in preferences)
                            //{
                            //    var postedBefore = db.MeritPostings.Where(x => x.Designation_Id == merit.Designation_Id && x.PostingHFMISCode.Equals(preference.HfmisCode) && x.IsActive == true).ToList();
                            //    var postedHF = db.HFOpenedViews.FirstOrDefault(x => x.Designation_Id == merit.Designation_Id && x.HFMISCode.Equals(preference.HfmisCode) && x.IsActive == true);
                            //    if (postedHF != null)
                            //    {
                            //        if (postedHF.Seats > postedBefore.Count)
                            //        {
                            //            MeritPosting meritPosting = new MeritPosting();
                            //            meritPosting.Merit_Id = merit.Id;
                            //            meritPosting.Designation_Id = merit.Designation_Id;
                            //            meritPosting.PostingHF_Id = postedHF.HF_Id;
                            //            meritPosting.PostingHFMISCode = postedHF.HFMISCode;
                            //            meritPosting.PostingHFSeatNumber = postedBefore.Count + 1;
                            //            meritPosting.PostingHFTotalSeats = postedHF.Seats;
                            //            meritPosting.Preference_Id = preference.id;
                            //            meritPosting.PreferencesNumber = preference.PrefrencesOrder;
                            //            meritPosting.IsActive = true;
                            //            meritPosting.DateTime = DateTime.UtcNow.AddHours(5);
                            //            db.MeritPostings.Add(meritPosting);
                            //            db.SaveChanges();
                            //            break;
                            //        }
                            //    }
                            //}
                        }
                    }
                }


                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("AddHrProfile/{meritId}/{hfId}")]
        public IHttpActionResult AddHrProfiles(int meritId, int hfId)
        {
            using (var transc = db.Database.BeginTransaction())
            {
                try
                {
                    var merit = db.Merits.FirstOrDefault(x => x.Id == meritId);
                    var hf = db.HealthFacilities.FirstOrDefault(x => x.Id == hfId);
                    if (merit != null && hf != null)
                    {
                        var designations = db.HrDesignations.ToList();
                        var dbProfile = db.HrProfiles.FirstOrDefault(x => x.CNIC == merit.CNIC);
                        if (dbProfile == null)
                        {
                            HrProfile hrProfile = new HrProfile();
                            hrProfile.EmployeeName = merit.Name;
                            hrProfile.FatherName = merit.FatherName;
                            hrProfile.CNIC = merit.CNIC;
                            hrProfile.DateOfBirth = merit.DOB;
                            hrProfile.Gender = Convert.ToInt32(merit.CNIC.Last()) % 2 == 0 ? "Female" : "Male";
                            hrProfile.Domicile_Id = merit.Domicile_Id;
                            hrProfile.Department_Id = 25;
                            hrProfile.EmpMode_Id = 13;
                            hrProfile.Status_Id = 30;
                            hrProfile.Designation_Id = merit.Designation_Id;
                            hrProfile.WDesignation_Id = merit.Designation_Id;
                            hrProfile.HealthFacility_Id = hf.Id;
                            hrProfile.HfmisCode = hf.HfmisCode;
                            hrProfile.WorkingHealthFacility_Id = hf.Id;
                            hrProfile.WorkingHFMISCode = hf.HfmisCode;
                            var designation = designations.FirstOrDefault(x => x.Id == hrProfile.Designation_Id);
                            if (designation != null)
                            {
                                hrProfile.CurrentGradeBPS = designation.HrScale_Id;
                                hrProfile.JoiningGradeBPS = designation.HrScale_Id;
                                hrProfile.Postaanctionedwithscale = designation.HrScale_Id.ToString();
                                hrProfile.Cadre_Id = designation.Cadre_Id;
                            }
                            hrProfile.MaritalStatus = merit.MaritalStatus;
                            hrProfile.MobileNo = merit.MobileNumber;
                            hrProfile.LandlineNo = merit.MobileSec;
                            hrProfile.EMaiL = merit.Email;
                            hrProfile.PermanentAddress = merit.Address;
                            Entity_Lifecycle elc = new Entity_Lifecycle();

                            elc.IsActive = true;
                            elc.Created_By = User.Identity.GetUserName();
                            elc.Created_Date = DateTime.UtcNow.AddHours(5);
                            elc.Entity_Id = 9;
                            elc.Users_Id = User.Identity.GetUserId();

                            db.Entity_Lifecycle.Add(elc);
                            db.SaveChanges();
                            hrProfile.EntityLifecycle_Id = elc.Id;

                            db.HrProfiles.Add(hrProfile);
                            db.SaveChanges();
                            transc.Commit();
                            return Ok(true);
                        }
                        else
                        {
                            transc.Rollback();
                            return Ok(false);
                        }
                    }
                    transc.Rollback();
                    return Ok(false);
                }
                catch (Exception ex)
                {
                    transc.Rollback();
                    return BadRequest(ex.Message);
                }
            }
        }


        [HttpGet]
        [Route("LockMeritOrder")]
        public IHttpActionResult LockMeritOrder()
        {
            try
            {

                db.Configuration.ProxyCreationEnabled = false;
                var merits = db.Merits.Where(x => x.Id > 14072 && x.Designation_Id == 302 && x.MeritNumber <= 300).OrderBy(k => k.MeritNumber).ToList();
                foreach (var merit in merits)
                {

                    string userName = User.Identity.GetUserName();
                    string userId = User.Identity.GetUserId();
                    HrProfile hrProfile;
                    var postedHF = db.MeritPostings.FirstOrDefault(x => x.Merit_Id == merit.Id);
                    if (postedHF != null)
                    {
                        hrProfile = db.HrProfiles.FirstOrDefault(x => x.CNIC.Equals(merit.CNIC));

                        var vPMaster = db.VpMastProfileViews.Where(x => x.HFMISCode.Equals(postedHF.PostingHFMISCode) && x.Desg_Id == merit.Designation_Id)?.FirstOrDefault();

                        if (vPMaster != null)
                        {
                            if (vPMaster.Vacant + vPMaster.Adhoc <= 0)
                            {

                            }
                        }

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

                            hrProfile.Domicile_Id = merit.Domicile_Id;
                            hrProfile.MaritalStatus = merit.MaritalStatus;
                        }
                        else
                        {
                            // _transferPostingService.UpdateVacancy(db, false, hrProfile.HfmisCode, hrProfile.Designation_Id, hrProfile.EmpMode_Id, userName, userId);
                        }

                        hrProfile.Department_Id = 25;
                        hrProfile.CurrentGradeBPS = 16;

                        hrProfile.HealthFacility_Id = postedHF.PostingHF_Id;
                        hrProfile.HfmisCode = postedHF.PostingHFMISCode;
                        hrProfile.WorkingHealthFacility_Id = postedHF.PostingHF_Id;
                        hrProfile.WorkingHFMISCode = postedHF.PostingHFMISCode;

                        hrProfile.WDesignation_Id = merit.Designation_Id;
                        hrProfile.Designation_Id = merit.Designation_Id;

                        hrProfile.EmpMode_Id = 13;
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

                        _transferPostingService.UpdateVacancy(db, true, hrProfile.HfmisCode, hrProfile.Designation_Id, hrProfile.EmpMode_Id, userName, userId);

                        var hfOpenedView = db.HFOpenedViews.FirstOrDefault(x => x.IsActive == true && x.HF_Id == hrProfile.HealthFacility_Id && x.Designation_Id == hrProfile.Designation_Id);
                        if (hfOpenedView != null)
                        {
                            var hfOpened = db.HFOpenedPostings.FirstOrDefault(x => x.Id == hfOpenedView.Id);
                            if (hfOpened.Seats == null)
                            {

                            }
                            if (hfOpened.Seats <= 0)
                            {

                            }
                            int seatsBefore = (int)hfOpened.Seats;
                            hfOpened.Seats = seatsBefore <= 0 ? 0 : (hfOpened.Seats - 1);


                            if (hfOpened.EntityLifecycle_Id > 0)
                            {
                                Entity_Modified_Log eml = new Entity_Modified_Log();
                                eml.Description = "Seat Filled by Merit No. " + merit.MeritNumber + ". Before: " + seatsBefore + ", After: " + hfOpened.Seats;
                                eml.Modified_By = userId;
                                eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                                eml.Entity_Lifecycle_Id = (long)hfOpened.EntityLifecycle_Id;
                                db.Entity_Modified_Log.Add(eml);
                                db.SaveChanges();
                                db.Entry(hfOpened).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {

                            }

                        }
                        else
                        {

                        }
                    }
                    else
                    {

                    }
                    //string MessageBody = @"Dear " + EsrProfile.EmployeeName + "\n(CNIC:" + EsrProfile.CNIC + ")\n Your order is under process at Primary and Secondary Healthcare Department Punjab. Please visit http://pshealth.punjab.gov.pk/ for further details.";
                    //SMS sms = new SMS()
                    //{
                    //    UserId = userId,
                    //    FKId = esr.Id,
                    //    MobileNumber = esr.MobileNo,
                    //    Message = MessageBody
                    //};
                    //await Common.SendSMSTelenor(sms);
                }
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("LockMerits")]
        public IHttpActionResult LockMerits()
        {
            using (var transc = db.Database.BeginTransaction())
            {
                try
                {
                    var merits = db.Merits.Where(x => x.MeritsActiveDesignationId == 68 && (x.Status.Equals("Accepted") || x.Status.Equals("Completed"))).OrderByDescending(k => k.IsDisabled).ThenBy(l => l.MeritNumber).ToList();
                    foreach (var merit in merits)
                    {
                        var pg = db.MeritPGs.FirstOrDefault(x => x.Merit_Id == merit.Id);
                        if (pg != null)
                        {
                            if (pg.isPG == false)
                            {
                                LockMeritOrderSingle(merit.Id);
                            }
                        }
                    }
                    transc.Commit();
                    return Ok(false);
                }
                catch (Exception ex)
                {
                    transc.Rollback();
                    return BadRequest(ex.Message);
                }
            }
        }

        [HttpGet]
        [Route("AddHW")]
        public IHttpActionResult AddHW()
        {
            try
            {

                db.Configuration.ProxyCreationEnabled = false;
                db.Database.CommandTimeout = 60 * 5;
                var hws = db.HrHealthWorkers.Select(x => x.CNIC).ToList();
                var profiles = db.ProfileDetailsViews.Where(x => (x.HFTypeCode.Equals("011") || x.HFTypeCode.Equals("012")) && x.StatusName.Equals("Active") && !hws.Contains(x.CNIC)).ToList();

                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("LockMeritOrderSingle/{MeritId}")]
        public IHttpActionResult LockMeritOrderSingle(int MeritId)
        {
            try
            {

                db.Configuration.ProxyCreationEnabled = false;
                var merit = db.Merits.FirstOrDefault(x => x.Id == MeritId);
                if (merit != null)
                {
                    string userName = User.Identity.GetUserName();
                    string userId = User.Identity.GetUserId();
                    HrProfile hrProfile;
                    var postedHF = db.MeritPostings.FirstOrDefault(x => x.Merit_Id == merit.Id);
                    if (postedHF != null)
                    {
                        hrProfile = db.HrProfiles.FirstOrDefault(x => x.CNIC.Equals(merit.CNIC));

                        int? designationId = merit.Designation_Id;

                        if (postedHF.PostingHFMISCode.Length == 6)
                        {
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

                                hrProfile.Domicile_Id = merit.Domicile_Id;
                                hrProfile.MaritalStatus = merit.MaritalStatus;
                            }

                            hrProfile.Department_Id = 25;
                            hrProfile.CurrentGradeBPS = 17;

                            hrProfile.WDesignation_Id = merit.Designation_Id;
                            hrProfile.Designation_Id = merit.Designation_Id;

                            hrProfile.EmpMode_Id = 13;
                            hrProfile.Status_Id = 14;


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
                        }
                        else
                        {
                            if (postedHF.PostingHFMISCode.Substring(12, 3) == "014")
                            {
                                designationId = 2404;
                            }

                            var vPMaster = db.VpMeritPreferenceViews.Where(x => x.HFMISCode.Equals(postedHF.PostingHFMISCode) && x.Desg_Id == designationId)?.FirstOrDefault();

                            if (vPMaster != null)
                            {
                                if (vPMaster.Vacant + vPMaster.Adhoc <= 0)
                                {
                                    return Ok(false);
                                }
                                if (vPMaster.Vacant <= 0 && vPMaster.Adhoc > 0)
                                {
                                    _transferPostingService.UpdateVacancy(db, false, vPMaster.HFMISCode, designationId, 3, userName, userId);
                                }
                            }
                            else
                            {
                                if (postedHF.PostingHFMISCode.Substring(12, 3) == "015")
                                {
                                    designationId = 2404;
                                }
                                vPMaster = db.VpMeritPreferenceViews.Where(x => x.HFMISCode.Equals(postedHF.PostingHFMISCode) && x.Desg_Id == designationId)?.FirstOrDefault();

                                if (vPMaster != null)
                                {
                                    if (vPMaster.Vacant + vPMaster.Adhoc <= 0)
                                    {
                                        return Ok(false);
                                    }
                                    if (vPMaster.Vacant <= 0 && vPMaster.Adhoc > 0)
                                    {
                                        _transferPostingService.UpdateVacancy(db, false, vPMaster.HFMISCode, designationId, 3, userName, userId);
                                    }
                                }
                            }

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

                                hrProfile.Domicile_Id = merit.Domicile_Id;
                                hrProfile.MaritalStatus = merit.MaritalStatus;
                            }
                            else
                            {
                                if (hrProfile.EmpMode_Id == 13)
                                {
                                    if (!string.IsNullOrEmpty(hrProfile.HfmisCode) && hrProfile.HfmisCode.Length == 19)
                                    {

                                        if (hrProfile.HfmisCode.Substring(12, 3) == "014")
                                        {
                                            designationId = 2404;
                                        }
                                        else if (hrProfile.HfmisCode.Substring(12, 3) == "015")
                                        {
                                            designationId = 2404;
                                        }
                                        else
                                        {
                                            designationId = 1320;
                                        }
                                        _transferPostingService.UpdateVacancy(db, false, hrProfile.HfmisCode, designationId, hrProfile.EmpMode_Id, userName, userId);
                                    }
                                }
                            }

                            hrProfile.Department_Id = 25;
                            hrProfile.CurrentGradeBPS = 17;

                            hrProfile.HealthFacility_Id = postedHF.PostingHF_Id;
                            hrProfile.HfmisCode = postedHF.PostingHFMISCode;
                            hrProfile.WorkingHealthFacility_Id = postedHF.PostingHF_Id;
                            hrProfile.WorkingHFMISCode = postedHF.PostingHFMISCode;

                            hrProfile.WDesignation_Id = merit.Designation_Id;
                            hrProfile.Designation_Id = merit.Designation_Id;

                            hrProfile.EmpMode_Id = 13;
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

                            if (hrProfile.HfmisCode.Substring(12, 3) == "014")
                            {
                                designationId = 2404;
                            }
                            else if (hrProfile.HfmisCode.Substring(12, 3) == "015")
                            {
                                designationId = 2404;
                            }
                            else
                            {
                                designationId = 1320;

                            }
                            _transferPostingService.UpdateVacancy(db, true, hrProfile.HfmisCode, designationId, hrProfile.EmpMode_Id, userName, userId);
                        }

                        postedHF.ESR_Id = 1;
                        db.Entry(postedHF).State = EntityState.Modified;
                        db.SaveChanges();
                        var hfOpenedView = db.HFOpenedViews.FirstOrDefault(x => x.IsActive == true && x.HF_Id == hrProfile.HealthFacility_Id && x.Designation_Id == hrProfile.Designation_Id);
                        if (hfOpenedView != null)
                        {
                            var hfOpened = db.HFOpenedPostings.FirstOrDefault(x => x.Id == hfOpenedView.Id);
                            if (hfOpened != null)
                            {
                                if (hfOpened.Seats == null)
                                {

                                }
                                if (hfOpened.Seats <= 0)
                                {

                                }
                                int seatsBefore = hfOpened.Seats == null ? 0 : (int)hfOpened.Seats;
                                hfOpened.Seats = seatsBefore <= 0 ? 0 : (hfOpened.Seats - 1);
                                if (hfOpened.EntityLifecycle_Id > 0)
                                {
                                    Entity_Modified_Log eml = new Entity_Modified_Log();
                                    eml.Description = "Seat Filled by Merit No. " + merit.MeritNumber + ". Before: " + seatsBefore + ", After: " + hfOpened.Seats;
                                    eml.Modified_By = userId;
                                    eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                                    eml.Entity_Lifecycle_Id = (long)hfOpened.EntityLifecycle_Id;
                                    db.Entity_Modified_Log.Add(eml);
                                    db.SaveChanges();
                                    db.Entry(hfOpened).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }




                        }
                    }
                    //string MessageBody = @"Dear " + EsrProfile.EmployeeName + "\n(CNIC:" + EsrProfile.CNIC + ")\n Your order is under process at Primary and Secondary Healthcare Department Punjab. Please visit http://pshealth.punjab.gov.pk/ for further details.";
                    //SMS sms = new SMS()
                    //{
                    //    UserId = userId,
                    //    FKId = esr.Id,
                    //    MobileNumber = esr.MobileNo,
                    //    Message = MessageBody
                    //};
                    //await Common.SendSMSTelenor(sms);
                }
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        [Route("PostCandidate/{MeritID}/{HF_Id}/{HFMISCode}")]
        public IHttpActionResult PostCandidate(int MeritID, int HF_Id, string HFMISCode)
        {
            try
            {
                var merit = db.Merits.FirstOrDefault(x => x.Id == MeritID);
                //if (merit != null && (merit.Status.Equals("Accepted") || merit.Status.Equals("Completed")))
                if (merit != null)
                {
                    string message = "Merit No. " + merit.MeritNumber + ", " + merit.Name + " " + merit.FatherName + ", ";
                    if (HF_Id == 0)
                    {
                        var disposalDistrict = db.Districts.FirstOrDefault(x => x.Code == HFMISCode);
                        int? designationId = merit.Designation_Id;
                        var postedBefore = db.MeritPostings.Where(x => x.Designation_Id == merit.Designation_Id && x.PostingHFMISCode.Equals(disposalDistrict.Code) && x.IsActive == true).ToList();
                        var preference = db.MeritPreferences.FirstOrDefault(x => x.Merit_Id == merit.Id && x.HfmisCode.Equals(disposalDistrict.Code));
                        if (preference != null)
                        {
                            var meritPosting = db.MeritPostings.FirstOrDefault(x => x.Merit_Id == MeritID);
                            if (meritPosting == null)
                            {
                                meritPosting = new MeritPosting();
                                meritPosting.Merit_Id = merit.Id;
                                meritPosting.Designation_Id = merit.Designation_Id;
                            }
                            meritPosting.MeritsActiveDesignationId = 68;
                            message += "Preference No." + preference.PrefrencesOrder + ", ";
                            meritPosting.PostingHFMISCode = disposalDistrict.Code;
                            meritPosting.IsPG = true;
                            meritPosting.DistrictCode = disposalDistrict.Code;
                            meritPosting.PostingHFSeatNumber = postedBefore.Count + 1;
                            message += "Seat No. " + meritPosting.PostingHFSeatNumber;
                            meritPosting.ActualPostingHFMISCode = disposalDistrict.Code;
                            meritPosting.PreferencesNumber = preference.PrefrencesOrder;
                            meritPosting.Preference_Id = preference.id;
                            meritPosting.IsActive = true;
                            meritPosting.DateTime = DateTime.UtcNow.AddHours(5);
                            meritPosting.Remarks = message;
                            meritPosting.IsOnAdhocee = false;
                            meritPosting.ESR_Id = 1;
                            if (meritPosting.Id == 0)
                            {
                                db.MeritPostings.Add(meritPosting);
                            }
                            else
                            {
                                db.Entry(meritPosting).State = EntityState.Modified;
                            }
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        var postedHF = db.HFListPs.FirstOrDefault(x => x.Id == HF_Id && x.IsActive == true);
                        int? designationId = merit.Designation_Id;
                        if (postedHF.HFMISCode.Substring(12, 3) == "014")
                        {
                            designationId = 2404;
                        }
                        var vPMaster = db.VpMastProfileViews.Where(x => x.HF_Id == postedHF.Id && x.Desg_Id == designationId).FirstOrDefault();
                        if (vPMaster != null)
                        {
                            if (vPMaster.Vacant + vPMaster.Adhoc <= 0)
                            {
                                return Ok(false);
                            }
                        }
                        else
                        {
                            if (postedHF.HFMISCode.Substring(12, 3) == "015")
                            {
                                designationId = 2404;
                            }
                            vPMaster = db.VpMastProfileViews.FirstOrDefault(x => x.HF_Id == postedHF.Id && x.Desg_Id == designationId);
                            if (vPMaster != null)
                            {
                                if (vPMaster.Vacant + vPMaster.Adhoc <= 0)
                                {
                                    return Ok(false);
                                }
                            }
                        }
                        var postedBefore = db.AwaitingPostings.Where(x => x.Designation_Id == merit.Designation_Id && x.PostingHFMISCode.Equals(postedHF.HFMISCode) && x.IsActive == true).ToList();
                        if (postedHF != null)
                        {
                            var awaitingPosting = db.AwaitingPostings.FirstOrDefault(x => x.Merit_Id == MeritID);
                            if (awaitingPosting == null)
                            {
                                awaitingPosting = new AwaitingPosting();
                                awaitingPosting.Merit_Id = merit.Id;
                                awaitingPosting.Designation_Id = merit.Designation_Id;
                            }
                            if (vPMaster.Vacant <= 0 && vPMaster.Adhoc > 0)
                            {
                                awaitingPosting.IsOnAdhocee = true;
                            }
                            else
                            {
                                awaitingPosting.IsOnAdhocee = false;
                            }
                            awaitingPosting.PostingHF_Id = postedHF.Id;
                            awaitingPosting.PostingHFMISCode = postedHF.HFMISCode;
                            awaitingPosting.PostingHFSeatNumber = postedBefore.Count + 1;
                            message += "Seat No. " + awaitingPosting.PostingHFSeatNumber;
                            awaitingPosting.ActualPostingHF_Id = postedHF.Id;
                            awaitingPosting.ActualPostingHFMISCode = postedHF.HFMISCode;
                            awaitingPosting.PostingHFTotalSeats = vPMaster.Vacant;
                            awaitingPosting.IsActive = true;
                            awaitingPosting.DateTime = DateTime.UtcNow.AddHours(5);
                            awaitingPosting.Remarks = message;
                            awaitingPosting.ESR_Id = 0;
                            if (awaitingPosting.Id == 0)
                            {
                                db.AwaitingPostings.Add(awaitingPosting);
                            }
                            else
                            {
                                db.Entry(awaitingPosting).State = EntityState.Modified;
                            }
                            db.SaveChanges();
                        }
                    }
                }


                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("PostCandidateHF/{MeritID}/{HF_Id}")]
        public IHttpActionResult PostCandidateHF(int MeritID, int HF_Id)
        {
            try
            {
                var merit = db.Merits.FirstOrDefault(x => x.Id == MeritID);
                //if (merit != null && (merit.Status.Equals("Accepted") || merit.Status.Equals("Completed")))
                if (merit != null)
                {
                    string message = "Merit No. " + merit.MeritNumber + ", " + merit.Name + " " + merit.FatherName + ", ";
                    var postedHF = db.HFListPs.FirstOrDefault(x => x.Id == HF_Id && x.IsActive == true);
                    int? designationId = merit.Designation_Id;
                    if (postedHF.HFMISCode.Substring(12, 3) == "014")
                    {
                        designationId = 2404;
                    }
                    var vPMaster = db.VpMastProfileViews.Where(x => x.HF_Id == postedHF.Id && x.Desg_Id == designationId).FirstOrDefault();
                    if (vPMaster != null)
                    {
                        if (vPMaster.Vacant + vPMaster.Adhoc <= 0)
                        {
                            return Ok(false);
                        }
                    }
                    else
                    {
                        if (postedHF.HFMISCode.Substring(12, 3) == "015")
                        {
                            designationId = 2404;
                        }
                        vPMaster = db.VpMastProfileViews.FirstOrDefault(x => x.HF_Id == postedHF.Id && x.Desg_Id == designationId);
                        if (vPMaster != null)
                        {
                            if (vPMaster.Vacant + vPMaster.Adhoc <= 0)
                            {
                                return Ok(false);
                            }
                        }
                    }
                    var postedBefore = db.AwaitingPostings.Where(x => x.Designation_Id == merit.Designation_Id && x.PostingHFMISCode.Equals(postedHF.HFMISCode) && x.IsActive == true).ToList();
                    if (postedHF != null)
                    {
                        var awaitingPosting = db.AwaitingPostings.FirstOrDefault(x => x.Merit_Id == MeritID);
                        if (awaitingPosting == null)
                        {
                            awaitingPosting = new AwaitingPosting();
                            awaitingPosting.Merit_Id = merit.Id;
                            awaitingPosting.Designation_Id = merit.Designation_Id;
                        }
                        if (vPMaster.Vacant <= 0 && vPMaster.Adhoc > 0)
                        {
                            awaitingPosting.IsOnAdhocee = true;
                        }
                        else
                        {
                            awaitingPosting.IsOnAdhocee = false;
                        }
                        awaitingPosting.PostingHF_Id = postedHF.Id;
                        awaitingPosting.PostingHFMISCode = postedHF.HFMISCode;
                        awaitingPosting.PostingHFSeatNumber = postedBefore.Count + 1;
                        message += "Seat No. " + awaitingPosting.PostingHFSeatNumber;
                        awaitingPosting.ActualPostingHF_Id = postedHF.Id;
                        awaitingPosting.ActualPostingHFMISCode = postedHF.HFMISCode;
                        awaitingPosting.PostingHFTotalSeats = vPMaster.Vacant;
                        awaitingPosting.IsActive = true;
                        awaitingPosting.DateTime = DateTime.UtcNow.AddHours(5);
                        awaitingPosting.Remarks = message;
                        awaitingPosting.ESR_Id = 0;
                        if (awaitingPosting.Id == 0)
                        {
                            db.AwaitingPostings.Add(awaitingPosting);
                        }
                        else
                        {
                            db.Entry(awaitingPosting).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                    }
                }


                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("LockAwaitingPostingOrder/{MeritId}")]
        public IHttpActionResult LockAwaitingPostingOrder(int MeritId)
        {
            try
            {

                db.Configuration.ProxyCreationEnabled = false;
                var merit = db.Merits.FirstOrDefault(x => x.Id == MeritId);
                if (merit != null)
                {
                    string userName = User.Identity.GetUserName();
                    string userId = User.Identity.GetUserId();
                    HrProfile hrProfile;
                    var postedHF = db.AwaitingPostings.FirstOrDefault(x => x.Merit_Id == merit.Id);
                    if (postedHF != null)
                    {
                        hrProfile = db.HrProfiles.FirstOrDefault(x => x.CNIC.Equals(merit.CNIC));

                        int? designationId = merit.Designation_Id;

                        if (postedHF.PostingHFMISCode.Length == 6)
                        {
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

                                hrProfile.Domicile_Id = merit.Domicile_Id;
                                hrProfile.MaritalStatus = merit.MaritalStatus;
                            }

                            hrProfile.Department_Id = 25;
                            hrProfile.CurrentGradeBPS = 17;

                            hrProfile.WDesignation_Id = merit.Designation_Id;
                            hrProfile.Designation_Id = merit.Designation_Id;

                            hrProfile.EmpMode_Id = 13;
                            hrProfile.Status_Id = 14;


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
                        }
                        else
                        {
                            if (postedHF.PostingHFMISCode.Substring(12, 3) == "014")
                            {
                                designationId = 2404;
                            }

                            var vPMaster = db.VpMastProfileViews.Where(x => x.HF_Id == postedHF.PostingHF_Id && x.Desg_Id == designationId)?.FirstOrDefault();

                            if (vPMaster != null)
                            {
                                if (vPMaster.Vacant + vPMaster.Adhoc <= 0)
                                {
                                    return Ok(false);
                                }
                                if (vPMaster.Vacant <= 0 && vPMaster.Adhoc > 0)
                                {
                                    _transferPostingService.UpdateVacancy(db, false, vPMaster.HFMISCode, designationId, 3, userName, userId);
                                }
                            }
                            else
                            {
                                if (postedHF.PostingHFMISCode.Substring(12, 3) == "015")
                                {
                                    designationId = 2404;
                                }
                                vPMaster = db.VpMastProfileViews.Where(x => x.HF_Id == hrProfile.HealthFacility_Id && x.Desg_Id == designationId).FirstOrDefault();

                                if (vPMaster != null)
                                {
                                    if (vPMaster.Vacant + vPMaster.Adhoc <= 0)
                                    {
                                        return Ok(false);
                                    }
                                    if (vPMaster.Vacant <= 0 && vPMaster.Adhoc > 0)
                                    {
                                        _transferPostingService.UpdateVacancy(db, false, vPMaster.HFMISCode, designationId, 3, userName, userId);
                                    }
                                }
                            }

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

                                hrProfile.Domicile_Id = merit.Domicile_Id;
                                hrProfile.MaritalStatus = merit.MaritalStatus;
                            }
                            else
                            {
                                if (hrProfile.EmpMode_Id == 13)
                                {
                                    if (!string.IsNullOrEmpty(hrProfile.HfmisCode) && hrProfile.HfmisCode.Length == 19)
                                    {

                                        if (hrProfile.HfmisCode.Substring(12, 3) == "014")
                                        {
                                            designationId = 2404;
                                        }
                                        else if (hrProfile.HfmisCode.Substring(12, 3) == "015")
                                        {
                                            designationId = 2404;
                                        }
                                        _transferPostingService.UpdateVacancy(db, false, hrProfile.HfmisCode, designationId, hrProfile.EmpMode_Id, userName, userId);
                                    }
                                }
                            }

                            hrProfile.Department_Id = 25;
                            hrProfile.CurrentGradeBPS = 17;

                            hrProfile.HealthFacility_Id = postedHF.PostingHF_Id;
                            hrProfile.HfmisCode = postedHF.PostingHFMISCode;
                            hrProfile.WorkingHealthFacility_Id = postedHF.PostingHF_Id;
                            hrProfile.WorkingHFMISCode = postedHF.PostingHFMISCode;

                            hrProfile.WDesignation_Id = merit.Designation_Id;
                            hrProfile.Designation_Id = merit.Designation_Id;

                            hrProfile.EmpMode_Id = 13;
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

                            if (hrProfile.HfmisCode.Substring(12, 3) == "014")
                            {
                                designationId = 2404;
                            }
                            else if (hrProfile.HfmisCode.Substring(12, 3) == "015")
                            {
                                designationId = 2404;
                            }
                            _transferPostingService.UpdateVacancy(db, true, hrProfile.HfmisCode, designationId, hrProfile.EmpMode_Id, userName, userId);
                        }

                        postedHF.ESR_Id = 1;
                        db.Entry(postedHF).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    //string MessageBody = @"Dear " + EsrProfile.EmployeeName + "\n(CNIC:" + EsrProfile.CNIC + ")\n Your order is under process at Primary and Secondary Healthcare Department Punjab. Please visit http://pshealth.punjab.gov.pk/ for further details.";
                    //SMS sms = new SMS()
                    //{
                    //    UserId = userId,
                    //    FKId = esr.Id,
                    //    MobileNumber = esr.MobileNo,
                    //    Message = MessageBody
                    //};
                    //await Common.SendSMSTelenor(sms);
                }
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        [Route("PostPromotedCandidate/{CNIC}/{HF_Id}")]
        public IHttpActionResult PostPromotedCandidate(string CNIC, int HF_Id)
        {
            try
            {
                var merit = db.PromotedCandidates.FirstOrDefault(x => x.CNIC == CNIC);
                //if (merit != null && (merit.Status.Equals("Accepted") || merit.Status.Equals("Completed")))
                if (merit != null)
                {
                    string message = ""; //"Merit No. " + merit.CNIC + ", " + merit.Name + " " + merit.FatherName + ", ";

                    var postedHF = db.HFListPs.FirstOrDefault(x => x.Id == HF_Id && x.IsActive == true);
                    int? designationId = merit.DesignationId;

                    var vPMaster = db.VpMProfileViews.Where(x => x.HF_Id == postedHF.Id && x.Desg_Id == designationId).FirstOrDefault();
                    if (vPMaster != null)
                    {
                        if (vPMaster.Vacant + vPMaster.OnPayScale <= 0)
                        {
                            return Ok(false);
                        }
                    }
                    else
                    {
                        vPMaster = db.VpMProfileViews.FirstOrDefault(x => x.HF_Id == postedHF.Id && x.Desg_Id == designationId);
                        if (vPMaster != null)
                        {
                            if (vPMaster.Vacant + vPMaster.OnPayScale <= 0)
                            {
                                return Ok(false);
                            }
                        }
                    }
                    var postedBefore = db.HrPostings.Where(x => x.Designation_Id == merit.DesignationId && x.PostingHFMISCode.Equals(postedHF.HFMISCode) && x.IsActive == true).ToList();
                    if (postedHF != null)
                    {
                        var awaitingPosting = db.HrPostings.FirstOrDefault(x => x.FK_Id == merit.Id);
                        if (awaitingPosting == null)
                        {
                            awaitingPosting = new HrPosting();
                            awaitingPosting.FK_Id = merit.Id;
                            awaitingPosting.Designation_Id = merit.DesignationId;
                        }
                        awaitingPosting.PostingHF_Id = postedHF.Id;
                        awaitingPosting.PostingHFMISCode = postedHF.HFMISCode;
                        awaitingPosting.PostingHFSeatNumber = postedBefore.Count + 1;
                        message += "Seat No. " + awaitingPosting.PostingHFSeatNumber;
                        awaitingPosting.ActualPostingHF_Id = postedHF.Id;
                        awaitingPosting.ActualPostingHFMISCode = postedHF.HFMISCode;
                        awaitingPosting.PostingHFTotalSeats = vPMaster.Vacant;
                        awaitingPosting.IsActive = true;
                        awaitingPosting.DateTime = DateTime.UtcNow.AddHours(5);
                        awaitingPosting.Remarks = message;
                        awaitingPosting.ESR_Id = 0;
                        if (awaitingPosting.Id == 0)
                        {
                            db.HrPostings.Add(awaitingPosting);
                        }
                        else
                        {
                            db.Entry(awaitingPosting).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                    }
                }


                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        [Route("LockHrPostingOrder/{PromotedId}")]
        public IHttpActionResult LockHrPostingOrder(int PromotedId)
        {
            try
            {

                db.Configuration.ProxyCreationEnabled = false;
                var merit = db.PromotedCandidates.FirstOrDefault(x => x.Id == PromotedId);
                if (merit != null)
                {
                    string userName = User.Identity.GetUserName();
                    string userId = User.Identity.GetUserId();
                    HrProfile hrProfile;
                    var postedHF = db.HrPostings.FirstOrDefault(x => x.FK_Id == merit.Id);
                    if (postedHF != null)
                    {
                        hrProfile = db.HrProfiles.FirstOrDefault(x => x.CNIC.Equals(merit.CNIC));

                        int? designationId = merit.DesignationId;

                        var vPMaster = db.VpMProfileViews.Where(x => x.HF_Id == hrProfile.HealthFacility_Id && x.Desg_Id == designationId)?.FirstOrDefault();

                        if (vPMaster != null)
                        {
                            if (vPMaster.Vacant + vPMaster.OnPayScale <= 0)
                            {
                                return Ok(false);
                            }
                            if (vPMaster.Vacant <= 0 && vPMaster.OnPayScale > 0)
                            {
                                _transferPostingService.UpdateVacancy(db, false, vPMaster.HFMISCode, designationId, 3, userName, userId);
                            }
                        }
                        else
                        {

                            vPMaster = db.VpMProfileViews.Where(x => x.HF_Id == hrProfile.HealthFacility_Id && x.Desg_Id == designationId).FirstOrDefault();

                            if (vPMaster != null)
                            {
                                if (vPMaster.Vacant + vPMaster.OnPayScale <= 0)
                                {
                                    return Ok(false);
                                }
                                if (vPMaster.Vacant <= 0 && vPMaster.OnPayScale > 0)
                                {
                                    _transferPostingService.UpdateVacancy(db, false, vPMaster.HFMISCode, designationId, 3, userName, userId);
                                }
                            }
                        }

                        if (hrProfile == null)
                        {
                            var candidate = db.PromotedCandidateViews.FirstOrDefault(x => x.CNIC == merit.CNIC);
                            if (candidate != null)
                            {
                                hrProfile = new HrProfile();
                                hrProfile.EmployeeName = candidate.Name;
                                hrProfile.FatherName = candidate.FatherName;
                                hrProfile.CNIC = merit.CNIC;
                                hrProfile.DateOfBirth = candidate.DateOfBirth;

                                string lastChar = merit.CNIC.Substring(merit.CNIC.Length - 1);
                                hrProfile.Gender = Convert.ToInt32(lastChar) % 2 == 0 ? "Female" : "Male";

                                hrProfile.MobileNo = candidate.MobileNumber;
                            }
                        }
                        else
                        {
                            if (hrProfile.EmpMode_Id == 13)
                            {
                                if (!string.IsNullOrEmpty(hrProfile.HfmisCode) && hrProfile.HfmisCode.Length == 19)
                                {
                                    _transferPostingService.UpdateVacancy(db, false, hrProfile.HfmisCode, designationId, hrProfile.EmpMode_Id, userName, userId);
                                }
                            }
                        }

                        hrProfile.Department_Id = 25;
                        hrProfile.CurrentGradeBPS = 17;

                        hrProfile.HealthFacility_Id = postedHF.PostingHF_Id;
                        hrProfile.HfmisCode = postedHF.PostingHFMISCode;
                        hrProfile.WorkingHealthFacility_Id = postedHF.PostingHF_Id;
                        hrProfile.WorkingHFMISCode = postedHF.PostingHFMISCode;

                        hrProfile.WDesignation_Id = merit.DesignationId;
                        hrProfile.Designation_Id = merit.DesignationId;

                        hrProfile.EmpMode_Id = 13;
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
                        _transferPostingService.UpdateVacancy(db, true, hrProfile.HfmisCode, designationId, hrProfile.EmpMode_Id, userName, userId);

                        postedHF.ESR_Id = 1;
                        db.Entry(postedHF).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    //string MessageBody = @"Dear " + EsrProfile.EmployeeName + "\n(CNIC:" + EsrProfile.CNIC + ")\n Your order is under process at Primary and Secondary Healthcare Department Punjab. Please visit http://pshealth.punjab.gov.pk/ for further details.";
                    //SMS sms = new SMS()
                    //{
                    //    UserId = userId,
                    //    FKId = esr.Id,
                    //    MobileNumber = esr.MobileNo,
                    //    Message = MessageBody
                    //};
                    //await Common.SendSMSTelenor(sms);
                }
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        [Route("MeritOrder/{MeritID}")]
        public IHttpActionResult MeritOrder([FromBody] ESR esrDto, int MeritID)
        {
            try
            {
                if (MeritID > 0)
                {
                    ESR esr = new ESR();
                    HrProfile hrProfile;

                    var merit = db.Merits.FirstOrDefault(x => x.Id == MeritID);
                    var postedHF = db.MeritPostings.FirstOrDefault(x => x.Merit_Id == MeritID);

                    hrProfile = db.HrProfiles.FirstOrDefault(x => x.CNIC.Equals(merit.CNIC));

                    var vPMaster = db.VPMasters.Where(x => x.HFMISCode.Equals(postedHF.PostingHFMISCode) && x.Desg_Id == merit.Designation_Id)?.FirstOrDefault();

                    if (vPMaster != null)
                    {
                        if (vPMaster.TotalSanctioned - vPMaster.TotalWorking <= 0)
                        {
                            return Ok(false);
                        }
                    }

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

                        hrProfile.Domicile_Id = merit.Domicile_Id;
                        hrProfile.MaritalStatus = merit.MaritalStatus;
                    }

                    hrProfile.Department_Id = 25;
                    hrProfile.CurrentGradeBPS = 18;

                    hrProfile.HealthFacility_Id = postedHF.PostingHF_Id;
                    hrProfile.HfmisCode = postedHF.PostingHFMISCode;
                    hrProfile.WorkingHealthFacility_Id = postedHF.PostingHF_Id;
                    hrProfile.WorkingHFMISCode = postedHF.PostingHFMISCode;

                    hrProfile.WDesignation_Id = merit.Designation_Id;
                    hrProfile.Designation_Id = merit.Designation_Id;

                    hrProfile.EmpMode_Id = 13;
                    hrProfile.Status_Id = 30;

                    esr.CNIC = hrProfile.CNIC;
                    esr.DepartmentFrom = hrProfile.Department_Id;

                    esr.BPSTo = 18;
                    esr.DesignationTo = merit.Designation_Id;
                    esr.DepartmentTo = 25;
                    esr.HF_Id_To = postedHF.PostingHF_Id;
                    esr.HfmisCodeTo = postedHF.PostingHFMISCode;

                    esr.SectionOfficer = "Section Officer (SC)";
                    //esr.EsrSectionOfficerID = ????;
                    esr.OrderHTML = esrDto.OrderHTML;
                    string userName = User.Identity.GetUserName();
                    string userId = User.Identity.GetUserId();
                    esr.ResponsibleUser = userName;
                    esr.IsActive = true;
                    Entity_Lifecycle eld = new Entity_Lifecycle();
                    eld.Created_Date = DateTime.UtcNow.AddHours(5);
                    eld.Created_By = userName;
                    eld.Users_Id = userId;
                    eld.IsActive = true;
                    eld.Entity_Id = 5;

                    db.Entity_Lifecycle.Add(eld);
                    db.SaveChanges();

                    if (hrProfile.Id == 0)
                    {
                        db.HrProfiles.Add(hrProfile);
                        db.SaveChanges();
                    }
                    else
                    {
                        db.Entry(hrProfile).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    esr.EntityLifecycle_Id = eld.Id;
                    esr.Profile_Id = hrProfile.Id;
                    esr.ResponsibleUser = eld.Created_By;
                    db.ESRs.Add(esr);
                    hrProfile.AddToEmployeePool = true;
                    db.SaveChanges();

                    Code128BarcodeDraw barcode = BarcodeDrawFactory.Code128WithChecksum;
                    BarcodeSymbology s = BarcodeSymbology.Code39NC;
                    BarcodeDraw drawObject = BarcodeDrawFactory.GetSymbology(s);
                    var metrics = drawObject.GetDefaultMetrics(50);
                    metrics.Scale = 1;
                    string prefix = "ESR-";

                    string id = esr.Id.ToString();
                    Image barCode = drawObject.Draw(prefix + id, metrics);


                    string imgSrc = "data:image / jpg; base64," + ImagesUtil.ImageToBase64(barCode, ImageFormat.Jpeg);

                    _transferPostingService.UpdateVacancy(db, true, esr.HfmisCodeTo, esr.DesignationTo, hrProfile.EmpMode_Id, userName, userId);
                    //string MessageBody = @"Dear " + EsrProfile.EmployeeName + "\n(CNIC:" + EsrProfile.CNIC + ")\n Your order is under process at Primary and Secondary Healthcare Department Punjab. Please visit http://pshealth.punjab.gov.pk/ for further details.";
                    //SMS sms = new SMS()
                    //{
                    //    UserId = userId,
                    //    FKId = esr.Id,
                    //    MobileNumber = esr.MobileNo,
                    //    Message = MessageBody
                    //};
                    //await Common.SendSMSTelenor(sms);
                    return Ok(new { esr, hrProfile, imgSrc });
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

        /// <summary>
        /// /
        /// By hard code Working 
        [HttpGet]
        [Route("MeritOrderDesignation/{meritActive}/{DesignationId}")]
        public IHttpActionResult MeritOrderList(int meritActive, int DesignationId)
        {
            try
            {
                if (meritActive > 0)
                {
                    var meritpostinglist = db.MeritPostings.Where(x => x.Designation_Id == DesignationId && x.MeritsActiveDesignationId == meritActive && x.IsActive == true).ToList();
                    foreach (var postedHF in meritpostinglist)
                    {

                    
                    //ESR esr = new ESR();
                    HrProfile hrProfile;

                    var merit = db.Merits.FirstOrDefault(x => x.Id == postedHF.Merit_Id);
                    hrProfile = db.HrProfiles.FirstOrDefault(x => x.CNIC.Equals(merit.CNIC));

                    var vPMaster = db.VPMasters.Where(x => x.HFMISCode.Equals(postedHF.PostingHFMISCode) && x.Desg_Id == merit.Designation_Id)?.FirstOrDefault();

                   

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

                        hrProfile.Domicile_Id = merit.Domicile_Id;
                        hrProfile.MaritalStatus = merit.MaritalStatus;
                    }

                    hrProfile.Department_Id = 25;
                    hrProfile.CurrentGradeBPS = 18;

                    hrProfile.HealthFacility_Id = postedHF.PostingHF_Id;
                    hrProfile.HfmisCode = postedHF.PostingHFMISCode;
                    hrProfile.WorkingHealthFacility_Id = postedHF.PostingHF_Id;
                    hrProfile.WorkingHFMISCode = postedHF.PostingHFMISCode;

                    hrProfile.WDesignation_Id = merit.Designation_Id;
                    hrProfile.Designation_Id = merit.Designation_Id;

                    hrProfile.EmpMode_Id = 13;
                    hrProfile.Status_Id = 30;

                    //esr.CNIC = hrProfile.CNIC;
                    //esr.DepartmentFrom = hrProfile.Department_Id;

                    //esr.BPSTo = 18;
                    //esr.DesignationTo = merit.Designation_Id;
                    //esr.DepartmentTo = 25;
                    //esr.HF_Id_To = postedHF.PostingHF_Id;
                    //esr.HfmisCodeTo = postedHF.PostingHFMISCode;

                    //esr.SectionOfficer = "Section Officer (SC)";
                    ////esr.EsrSectionOfficerID = ????;
                    //esr.OrderHTML = esrDto.OrderHTML;
                    string userName = "Admin";
                    string userId = "Admin";
                    //esr.ResponsibleUser = userName;
                   // esr.IsActive = true;
                    Entity_Lifecycle eld = new Entity_Lifecycle();
                    eld.Created_Date = DateTime.UtcNow.AddHours(5);
                    eld.Created_By = userName;
                    eld.Users_Id = userId;
                    eld.IsActive = true;
                    eld.Entity_Id = 5;

                    db.Entity_Lifecycle.Add(eld);
                    db.SaveChanges();

                    if (hrProfile.Id == 0)
                    {
                        db.HrProfiles.Add(hrProfile);
                        db.SaveChanges();
                    }
                    else
                    {
                        db.Entry(hrProfile).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                   // esr.EntityLifecycle_Id = eld.Id;
                    //esr.Profile_Id = hrProfile.Id;
                   // esr.ResponsibleUser = eld.Created_By;
                   // db.ESRs.Add(esr);
                    hrProfile.AddToEmployeePool = true;
                        //db.SaveChanges();

                        //Code128BarcodeDraw barcode = BarcodeDrawFactory.Code128WithChecksum;
                        //BarcodeSymbology s = BarcodeSymbology.Code39NC;
                        //BarcodeDraw drawObject = BarcodeDrawFactory.GetSymbology(s);
                        //var metrics = drawObject.GetDefaultMetrics(50);
                        //metrics.Scale = 1;
                        //string prefix = "ESR-";

                        //string id = esr.Id.ToString();
                        //Image barCode = drawObject.Draw(prefix + id, metrics);


                        //string imgSrc = "data:image / jpg; base64," + ImagesUtil.ImageToBase64(barCode, ImageFormat.Jpeg);

                        if (vPMaster != null)
                        {
                            if (vPMaster.TotalSanctioned - vPMaster.TotalWorking <= 0)
                            {
                                //return Ok(false);
                            }
                            else
                            {
                                _transferPostingService.UpdateVacancy(db, true, postedHF.PostingHFMISCode, DesignationId, hrProfile.EmpMode_Id, userName, userId);
                            }
                        }

                       
                        //string MessageBody = @"Dear " + EsrProfile.EmployeeName + "\n(CNIC:" + EsrProfile.CNIC + ")\n Your order is under process at Primary and Secondary Healthcare Department Punjab. Please visit http://pshealth.punjab.gov.pk/ for further details.";
                        //SMS sms = new SMS()
                        //{
                        //    UserId = userId,
                        //    FKId = esr.Id,
                        //    MobileNumber = esr.MobileNo,
                        //    Message = MessageBody
                        //};
                        //await Common.SendSMSTelenor(sms);
                    }
                    return Ok(true);
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



        //Currently only for charge nurse
        [HttpGet]
        [Route("GetPreferencesVacancyColor/{hfmisCode}")]
        public IHttpActionResult GetPreferencesVacancyColor(string hfmisCode)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                if (hfmisCode == null) return Ok("black");

                int designationId = 1320;
                if (hfmisCode.Substring(12, 3) == "014")
                {
                    designationId = 2404;
                }
                else if (hfmisCode.Substring(12, 3) == "015")
                {
                    designationId = 2404;
                }
                var vpMaster = db.VpMeritPreferenceViews.FirstOrDefault(x => x.HFMISCode.Equals(hfmisCode) && x.Desg_Id == designationId);
                if (vpMaster == null)
                {
                    return Ok("#dc3545");
                }

                if (vpMaster.HFAC == 2 || vpMaster.HFAC == 3)
                {
                    return Ok("#dc3545");
                }
                if ((vpMaster.Vacant + ((int)vpMaster.Adhoc)) <= 0)
                {
                    return Ok("#dc3545");
                }
                else if (vpMaster.Adhoc > 0 && vpMaster.Vacant <= 0)
                {
                    return Ok("#ffc107");
                }
                return Ok("black");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpGet]
        [Route("GetPreferencesVacancy/{hfmisCode}")]
        public IHttpActionResult GetPreferencesVacancy(string hfmisCode)
        {
            try
            {
                var vacancy = db.VpMeritPreferenceViews.Where(x => x.HFMISCode.StartsWith(hfmisCode) && (x.Desg_Id == 1320 || x.Desg_Id == 2404)).OrderByDescending(x => x.Vacant).ToList();

                return Ok(vacancy);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        private void SaveMessageLog(SMS sms, string type)
        {
            try
            {
                db.MessageLogs.Add(new MessageLog() { FKId = sms.FKId.ToString(), MessageType = type, Response = sms.Status, SentDate = DateTime.UtcNow.AddHours(5), SentFrom = sms.From, SentTo = sms.MobileNumber, Text = sms.Message });
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="meritActiveDsgId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GenerateMessages/{meritActiveDsgId=meritActiveDsgId}")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> GenerateMessages(int meritActiveDsgId)
        {

            try
            {
                var activeDesg = db.MeritActiveDesignations.FirstOrDefault(x => x.Id == meritActiveDsgId);

                var merits = db.Merits.Include(x => x.HrDesignation).Where(x => x.MeritsActiveDesignationId == meritActiveDsgId && (x.Status == "Completed" || x.Status == ("Accepted"))).ToList().Skip(461).ToList();
                //string jsonStr = File.ReadAllText(@"E:\Work\Projects\DotNet\HISDU\Docs\HRMIS\PreferencesDocsWmo\ChargeNurseNotJoined.json");
                //var merits = JsonConvert.DeserializeObject<List<Merit>>(jsonStr).Select(x => {
                //    x.MobileNumber = "0" + x.MobileNumber;
                //    return x;
                //}).ToList();

                foreach (var item in merits)
                {
                    var message = string.Empty;
                    var meritPg = db.MeritPGs.FirstOrDefault(x => x.Merit_Id == item.Id);
                    var meritPgDistricts = db.MeritPGDistricts.Where(x => x.Merit_Id == item.Id).ToList();
                    var meritPreferences = db.MeritPreferences.Where(x => x.Merit_Id == item.Id).ToList();
                    var days = (activeDesg.DateEnd - DateTime.Now.Date)?.Days ?? 0;
                    //if (item.Status == "New")
                    //{
                    //message = $"Dear {item.Name}, Visit https://pshealth.punjab.gov.pk. Click on generate password. Password will be sent on Mobile and Email.{activeDesg?.DateEnd?.ToString("D")} is the last Date of Preference submission against the post of {item.HrDesignation?.Name}. Other wise it shall be considered that you are not interested to join as {item.HrDesignation?.Name}. ";
                    //message = $"Dear Dr {item.Name}, Your posting order as Women Medical Officer has been uploaded on official website i.e https://pshealth.punjab.gov.pk. You are directed to submit your joining at your place of posting before 20th November 2018.";
                    //message = $"It is informed you that joining period for the post of Women Medical Officers is 26-11-2018. Further who also applied for the extension in joining period, they shall join on or before 6-12-2018.";
                    message = $"Dear Dr. {item.Name}  Today dated 26th November 2018 is the last date for the joining of Women Medical Officer however who had submitted their Grievance and their Grievance result is awaited their joining time is 28th November 2018.";
                    //}
                    //else if (item.Status == "Existing" || item.Status == "ProfileBuilt")
                    //{
                    //    message = $"Dear {item.Name}, Visit https://pshealth.punjab.gov.pk. {activeDesg?.DateEnd?.ToString("D")} is the last Date of Preference submission against the post of {item.HrDesignation?.Name}. Account will be deactive tomorrow. Other wise it shall be considered that you are not interested to join as {item.HrDesignation?.Name}.";
                    //}
                    //else if ((meritPg?.isPG == true && meritPgDistricts.Count == 0) || (meritPg?.isPG == false && meritPreferences.Count == 0))
                    //{
                    //    AspNetUser user = null;
                    //    using (var db = new PMIS2Entities())
                    //    {
                    //        user = db.AspNetUsers.FirstOrDefault(x => x.UserName == item.CNIC);
                    //    }
                    //    message = $"Dear {item.Name},Please complete your 10 preferences. Visit https://pshealth.punjab.gov.pk. Password is {user.hashynoty} .{days} days remaining. {activeDesg?.DateEnd?.ToString("D")} is the last Date of Preference submission against the post of {item.HrDesignation?.Name}. {days} days remaining. Other wise it shall be considered that you are not interested to join as {item.HrDesignation?.Name}.";
                    //}
                    //else if ((meritPg?.isPG == true && meritPgDistricts.Count < 10) || (meritPg?.isPG == false && meritPreferences.Count < 10)) {

                    //    AspNetUser user = null;
                    //    using (var db = new PMIS2Entities())
                    //    {
                    //        user = db.AspNetUsers.FirstOrDefault(x => x.UserName == item.CNIC);
                    //    }
                    //    message = $"Dear {item.Name},Preferences saved but complete your 10 preferences. Visit https://pshealth.punjab.gov.pk. Password is {user.hashynoty} .{days} days remaining.";
                    //}
                    //else if ((meritPg?.isPG == true && meritPgDistricts.Count >= 10) || (meritPg?.isPG == false && meritPreferences.Count >= 10))
                    //{

                    //    message = $"Dear {item.Name}, You have successfully given 10 preferences.";
                    //}
                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        SMS sms = new SMS() { Message = message, MobileNumber = item.MobileNumber, FKId = item.Id };
                        sms.Status = await (await Common.SMS_Send(sms)).Content.ReadAsStringAsync();
                        SaveMessageLog(sms, "SMS");
                    }

                }

                return Ok("");
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        /// <summary>
        /// 1. Fetch Applicants Order by Merit No
        /// 2. Get Preferences of each Applicant.
        /// 3. Search vacancy Position of Selected Preferences.
        /// 4. if vacant then hire and update vacancy position on temporary table.
        /// 5. if not vacant then check if adhoc are working remove adhocee and update vacancy position on temporary table.
        /// 6. if post is not vacant and adhoc are not working and then move on to next preferences and repeat from point (4).
        /// 7. if both persons has same preferences then before searching vacancy position on master table search from temporary table.
        /// </summary>
        /// <param name="meritActiveDsgId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GeneratePostingPlan/{meritActiveDsgId=meritActiveDsgId}")]
        [AllowAnonymous]
        private IHttpActionResult GeneratePostingPlan(int meritActiveDsgId)
        {

            try
            {
                var count = 0;
                var applicants = new List<Merit>();

                applicants = db.Merits.Where(x => x.MeritsActiveDesignationId == meritActiveDsgId && ((x.Status == "Completed") || x.Status == "Accepted") && (x.MeritPGs.FirstOrDefault().isPG == false || x.MeritPGs.FirstOrDefault() == null)).Include(x => x.District).Include(x => x.MeritPGs).Include(x => x.MeritActiveDesignation).OrderByDescending(x => x.IsDisabled).ThenBy(x => x.MeritNumber).ToList();
                applicants.AddRange(db.Merits.Where(x => x.MeritsActiveDesignationId == meritActiveDsgId && ((x.Status == "Completed") || x.Status == "Accepted") && x.MeritPGs.FirstOrDefault().isPG == true).Include(x => x.District).Include(x => x.MeritPGs).Include(x => x.MeritActiveDesignation).OrderByDescending(x => x.IsDisabled).ThenBy(x => x.MeritNumber).ToList());
                foreach (var item in applicants)
                {
                    SetApplicantPosting(item);
                    Trace.WriteLine($"Applicants Adjusted --------------> {++count}");
                }
                return Ok("");
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        private void SetApplicantPosting(Merit merit)
        {
            if (merit.MeritNumber == 13)
            {
                var a = 0;
            }
            var isHired = false;
            var preferences = GetPreferences(merit);
            var pg = merit.MeritPGs.FirstOrDefault();
            MeritPreference firstPref = null;

            //if applicant is not PG trainee and 
            if (preferences.Count == 0 && (pg == null || pg?.isPG == false))
            {

                var tehsils = GetTehsilsOtherThan(merit.District?.Code, merit.MeritActiveDesignation.ExcludedDistrictCode);
                isHired = PostInAnyTehsil(tehsils, merit, "NoPrefrence");
                return;
            };

            //if applicant is on PG trainee
            if (preferences.Count == 0 && pg?.isPG == true)
            {
                var mPGdistricts = db.MeritPGDistricts.Where(x => x.MeritPG_Id == pg.Id && x.Merit_Id == merit.Id).Include(x => x.District).OrderBy(x => x.PrefrencesOrder).ToList();

                if (mPGdistricts.Count > 0)
                {
                    isHired = PostInAnyTehsil(mPGdistricts, merit, "PG");
                }
                if (!isHired)
                {
                    var tehsils = GetTehsilsOtherThan(merit.District?.Code, merit.MeritActiveDesignation.ExcludedDistrictCode);
                    isHired = PostInAnyTehsil(tehsils, merit, "PGNoPrefrence");
                    return;
                }
                return;
            };

            int count = 0;
            foreach (var pref in preferences)
            {
                count++;
                if (count == 1) firstPref = pref;
                int sanctioned = 0, filled = 0, vacant = 0, regular = 0, adhoc = 0, contract = 0;
                var tempVacancy = GetVacancyPostionFromTempTable(merit, pref.HF_Id);
                var vacancy = GetVacancyPosition(merit, pref.HF_Id);
                var hf = GetHealthFacility(pref.HF_Id ?? -1);
                int bhuVacantCount = -1;

                if (tempVacancy != null)
                {
                    sanctioned = tempVacancy.CurSanctioned ?? 0;
                    filled = tempVacancy.CurFilled ?? 0;
                    vacant = tempVacancy.CurVacant ?? 0;
                    regular = tempVacancy.CurRegular ?? 0;
                    adhoc = tempVacancy.CurAdhoc ?? 0;
                    contract = tempVacancy.CurContract ?? 0;
                }
                else if (vacancy != null)
                {
                    sanctioned = vacancy.TotalSanctioned;
                    filled = vacancy.TotalWorking;
                    vacant = vacancy.Vacant ?? 0;
                    regular = vacancy.Regular ?? 0;
                    adhoc = vacancy.Adhoc ?? 0;
                    contract = vacancy.Contract ?? 0;
                };


                if (hf.HFTypeName == "Basic Health Unit" || hf.FullName.StartsWith("MMC") && (merit.Designation_Id == 802))
                {

                    var tempCount = GetVacantCountBHUFromDistrictsTemp(merit, pref.HfmisCode.Substring(0, 6));
                    if (tempCount == null)
                    {
                        tempCount = GetVacantCountBHUFromDistricts(merit, pref.HfmisCode.Substring(0, 6), new int[] { 802, 1320, 2404 });
                        if (tempCount > 0)
                        {
                            tempCount = tempCount / 2;
                            SaveVacantCountFromDistrict(merit, pref.HfmisCode.Substring(0, 6), tempCount ?? 0);
                        }
                    }
                    if (tempCount > 0)
                    {
                        bhuVacantCount = tempCount ?? -1;
                    }
                }

                if (hf.HFTypeName == "Basic Health Unit" || hf.FullName.StartsWith("MMC") && (merit.Designation_Id == 802))
                {
                    if (vacant > 0 && bhuVacantCount > 0)
                    {
                        bhuVacantCount--;
                        UpdateVacancyOnTempTable(merit, pref.HF_Id, pref.HfmisCode, sanctioned, filled, vacant, regular, adhoc, contract, pref.id, null, "Vacant");
                        UpdateVacantCountFromDistrict(merit, pref.HfmisCode.Substring(0, 6), bhuVacantCount);
                        isHired = true;
                        break;
                    }

                    if (adhoc > 0 && bhuVacantCount > 0)
                    {

                        bhuVacantCount--;
                        UpdateVacancyOnTempTable(merit, pref.HF_Id, pref.HfmisCode, sanctioned, filled, vacant, regular, adhoc, contract, pref.id, null, "Adhoc");
                        UpdateVacantCountFromDistrict(merit, pref.HfmisCode.Substring(0, 6), bhuVacantCount);
                        isHired = true;
                        break;
                    }
                    continue;
                }


                if (vacant > 0)
                {
                    UpdateVacancyOnTempTable(merit, pref.HF_Id, pref.HfmisCode, sanctioned, filled, vacant, regular, adhoc, contract, pref.id, null, "Vacant");
                    isHired = true;
                    break;
                }

                if (adhoc > 0)
                {
                    UpdateVacancyOnTempTable(merit, pref.HF_Id, pref.HfmisCode, sanctioned, filled, vacant, regular, adhoc, contract, pref.id, null, "Adhoc");
                    isHired = true;
                    break;
                }

                if (contract > 0 && merit.Designation_Id == 302) // Charge Nurse
                {
                    var contractCount = OnContractCount(hf.Id, merit);
                    var contractTempCount = OnContractTempCount(hf.Id, merit);
                    if (contractTempCount >= contractCount) continue;
                    UpdateVacancyOnTempTable(merit, pref.HF_Id, pref.HfmisCode, sanctioned, filled, vacant, regular, adhoc, contract, pref.id, null, "Contract");
                    isHired = true;
                    break;
                }
            }

            if (!isHired)
            {

                var isPosted = PostInAnyTehsil(GetPreferencesTehsils(preferences, merit.MeritActiveDesignation.ExcludedDistrictCode), merit, "NoVacancy");
                if (!isPosted)
                {
                    var tehsils = GetTehsilsOtherThan(merit.District?.Code, merit.MeritActiveDesignation.ExcludedDistrictCode);
                    isHired = PostInAnyTehsil(tehsils, merit, "NoVacancy");
                }
            };
        }


        private int OnContractCount(int hfId, Merit merit)
        {
            return db.Merits.Count(x => x.MeritsActiveDesignationId == merit.MeritsActiveDesignationId && x.HF_Id == hfId);
        }

        private int OnContractTempCount(int hfId, Merit merit)
        {
            return db.MeritsVps.Count(x => x.MeritActiveDesignation_Id == merit.MeritsActiveDesignationId && x.HF_Id == hfId && (x.Status == "Contract" || x.Status == "NoVacancyContract"));
        }

        private List<District> GetPreferencesDistricts(List<MeritPreference> preferences)
        {
            var districtCodes = preferences.Select(x => x.HfmisCode.Substring(0, 6)).Distinct().ToList();
            List<District> dis = new List<District>();
            foreach (var item in districtCodes)
            {
                dis.Add(db.Districts.FirstOrDefault(x => x.Code == item));
            }
            return dis;
        }

        private List<Tehsil> GetPreferencesTehsils(List<MeritPreference> preferences, string excludedDistrictCode)
        {
            var tehsilCodes = preferences.Select(x => x.HfmisCode.Substring(0, 9)).Distinct().ToList();
            List<Tehsil> tehsils = new List<Tehsil>();
            foreach (var code in tehsilCodes)
            {
                tehsils.AddRange(db.TehsilsDistances.Where(x => !x.DestinationCode.StartsWith(excludedDistrictCode ?? "-1") && x.OriginCode == code && x.DestinationCode.StartsWith(code.Substring(0, 6))).OrderBy(x => x.Distance).Include(x => x.Tehsil1).Select(x => x.Tehsil1).ToList());
            }
            return tehsils;
        }


        private void UpdateVacantCountFromDistrict(Merit merit, string districtCode, int vacant)
        {
            if (vacant < 0) return;
            var temp = db.MeritsVpDistricts.FirstOrDefault(x => x.MeritActiveDesgId == merit.MeritsActiveDesignationId && x.MeritVpDistrictCode == districtCode);
            temp.Vacant = vacant;
            db.SaveChanges();
        }
        private void SaveVacantCountFromDistrict(Merit merit, string districtCode, int vacant)
        {
            db.MeritsVpDistricts.Add(new MeritsVpDistrict() { MeritActiveDesgId = merit.MeritsActiveDesignationId, MeritVpDistrictCode = districtCode, Vacant = vacant });
            db.SaveChanges();
        }

        private int? GetVacantCountBHUFromDistrictsTemp(Merit merit, string districtCode)
        {
            return db.MeritsVpDistricts.FirstOrDefault(x => x.MeritActiveDesgId == merit.MeritsActiveDesignationId && x.MeritVpDistrictCode == districtCode)?.Vacant;
        }

        private int? GetVacantCountBHUFromDistricts(Merit merit, string districtCode, int[] desgIds)
        {
            //var sum = db.VpMDViews.Where(x => desgIds.Contains(x.Desg_Id) && x.HFTypeName == "Basic Health Unit" && x.HFMISCode.StartsWith(districtCode)).Select(x => x.Vacant + x.Adhoc).ToList();
            return db.VpMDViews.Where(x => desgIds.Contains(x.Desg_Id) && (x.HFTypeName == "Basic Health Unit" || x.FullName.StartsWith("MMC")) && x.HFMISCode.StartsWith(districtCode)).Select(x => x.Vacant + x.Adhoc).Sum(x => x);
        }

        private HFList GetHealthFacility(int hfId)
        {
            return db.HFLists.FirstOrDefault(x => x.Id == hfId);
        }

        private bool PostInAnyDistrict(List<District> districts, Merit merit, string status)
        {
            bool isPosted = false;
            foreach (var item in districts)
            {
                isPosted = SetApplicantPosting(merit, GetVacancyPositionFromDistrict(merit, item.Code ?? "-1"), null, status);
                if (isPosted) break;
            }
            return isPosted;
        }

        private bool PostInAnyDistrict(List<MeritPGDistrict> districts, Merit merit, string status)
        {
            bool isPosted = false;
            foreach (var item in districts)
            {
                isPosted = SetApplicantPosting(merit, GetVacancyPositionFromDistrict(merit, item.District.Code ?? "-1"), item.Id, status);
                if (isPosted) break;
            }
            return isPosted;
        }

        private bool PostInAnyTehsil(List<Tehsil> tehsils, Merit merit, string status)
        {
            bool isPosted = false;
            foreach (var item in tehsils)
            {
                if (filledTehsils.Contains(item.Code)) continue;
                isPosted = SetApplicantPosting(merit, GetVacancyPositionFromTehsil(merit, item.Code ?? "-1"), null, status);
                if (isPosted) break;
                filledTehsils.Add(item.Code);
            }
            return isPosted;
        }

        private bool PostInAnyTehsil(List<MeritPGDistrict> districts, Merit merit, string status)
        {
            bool isPosted = false;
            foreach (var item in districts)
            {
                if (filledTehsils.Contains(item.District.Code)) continue;
                isPosted = SetApplicantPosting(merit, GetVacancyPositionFromTehsil(merit, item.District.Code ?? "-1"), item.Id, status);
                if (isPosted) break;
                filledTehsils.Add(item.District.Code);
            }
            return isPosted;
        }



        //private List<District> GetDistrictsOtherThan(string districtCode,string excludedDistrict)
        //{
        //    var divCode = districtCode.Substring(0, 3);
        //    List<District> dis = new List<District>();
        //    dis = db.Districts.Where(x => x.Code.StartsWith(divCode) && x.Code != excludedDistrict).OrderBy(x=> districtCode).ToList();
        //    return dis;
        //}

        private List<District> GetDistrictsOtherThan(string districtCode, string excludedDistrict)
        {
            var dis = db.DistrictsDistances.Where(x => x.DestinationCode != excludedDistrict && x.OriginCode == districtCode).OrderBy(x => x.Distance).Include(x => x.District1).Select(x => x.District1).ToList();
            return dis;
        }

        private List<Tehsil> GetTehsilsOtherThan(string districtCode, string excludedDistrictCode)
        {
            List<Tehsil> _tehsils = new List<Tehsil>();
            var capitalTehsilCode = db.Districts.FirstOrDefault(x => x.Code == districtCode).CapitalTehsilCode;
            var districtsCodes = GetDistrictsOtherThan(districtCode, excludedDistrictCode).Select(x => x.Code).ToList();
            //var tehsils = db.TehsilsDistances.Where(x => !x.DestinationCode.StartsWith(excludedDistrictCode) && x.OriginCode == capitalTehsilCode).OrderBy(x => x.Distance).Include(x => x.Tehsil1).Select(x => x.Tehsil1).ToList();
            var tehsils = db.TehsilsDistances.Where(x => !x.DestinationCode.StartsWith(excludedDistrictCode ?? "-1") && x.OriginCode == capitalTehsilCode).OrderBy(x => x.Distance).Include(x => x.Tehsil1).Select(x => x.Tehsil1).ToList();
            foreach (var code in districtsCodes)
            {
                _tehsils.AddRange(tehsils.Where(x => x.Code.Substring(0, 6) == code).ToList());
            }
            return _tehsils;
        }

        private bool SetApplicantPosting(Merit merit, List<VpMDView> vps, int? pgDistrictId, string status)
        {
            bool isHired = false;
            foreach (var item in vps)
            {
                var hf = GetHealthFacility(item.HFId ?? -1);
                int sanctioned = 0, filled = 0, vacant = 0, regular = 0, adhoc = 0, contract = 0;
                var tempVacancy = GetVacancyPostionFromTempTable(merit, item.HFId);
                var vacancy = GetVacancyPosition(merit, item.HFId);
                int bhuVacantCount = -1;


                if (tempVacancy != null)
                {
                    sanctioned = tempVacancy.CurSanctioned ?? 0;
                    filled = tempVacancy.CurFilled ?? 0;
                    vacant = tempVacancy.CurVacant ?? 0;
                    regular = tempVacancy.CurRegular ?? 0;
                    adhoc = tempVacancy.CurAdhoc ?? 0;
                    contract = tempVacancy.CurContract ?? 0;
                }
                else if (vacancy != null)
                {
                    sanctioned = vacancy.TotalSanctioned;
                    filled = vacancy.TotalWorking;
                    vacant = vacancy.Vacant ?? 0;
                    regular = vacancy.Regular ?? 0;
                    adhoc = vacancy.Adhoc ?? 0;
                    contract = vacancy.Contract ?? 0;
                };

                if (hf.HFTypeName == "Basic Health Unit" || hf.FullName.StartsWith("MMC") && (merit.Designation_Id == 802))
                {

                    var tempCount = GetVacantCountBHUFromDistrictsTemp(merit, item.HFMISCode.Substring(0, 6));
                    if (tempCount == null)
                    {
                        tempCount = GetVacantCountBHUFromDistricts(merit, item.HFMISCode.Substring(0, 6), new int[] { 802, 1320, 2404 });
                        if (tempCount > 0)
                        {
                            tempCount = tempCount / 2;
                            SaveVacantCountFromDistrict(merit, item.HFMISCode.Substring(0, 6), tempCount ?? 0);
                        }
                    }
                    if (tempCount > 0)
                    {
                        bhuVacantCount = tempCount ?? -1;
                    }
                }
                if (hf.HFTypeName == "Basic Health Unit" || hf.FullName.StartsWith("MMC") && (merit.Designation_Id == 802))
                {
                    if (vacant > 0)
                    {
                        bhuVacantCount--;
                        UpdateVacancyOnTempTable(merit, item.HFId, item.HFMISCode, sanctioned, filled, vacant, regular, adhoc, contract, null, pgDistrictId, status + "Vacant");
                        UpdateVacantCountFromDistrict(merit, item.HFMISCode.Substring(0, 6), bhuVacantCount);
                        isHired = true;
                        break;
                    }
                    if (adhoc > 0)
                    {
                        bhuVacantCount--;
                        UpdateVacancyOnTempTable(merit, item.HFId, item.HFMISCode, sanctioned, filled, vacant, regular, adhoc, contract, null, pgDistrictId, status + "Adhoc");
                        UpdateVacantCountFromDistrict(merit, item.HFMISCode.Substring(0, 6), bhuVacantCount);
                        isHired = true;
                        break;
                    }
                    continue;
                }

                if (vacant > 0)
                {
                    UpdateVacancyOnTempTable(merit, item.HFId, item.HFMISCode, sanctioned, filled, vacant, regular, adhoc, contract, null, pgDistrictId, status + "Vacant");
                    isHired = true;
                    break;
                }
                if (adhoc > 0)
                {
                    UpdateVacancyOnTempTable(merit, item.HFId, item.HFMISCode, sanctioned, filled, vacant, regular, adhoc, contract, null, pgDistrictId, status + "Adhoc");
                    isHired = true;
                    break;
                }

                if (contract > 0 && merit.Designation_Id == 302) // Charge Nurse
                {
                    var contractCount = OnContractCount(hf.Id, merit);
                    var contractTempCount = OnContractTempCount(hf.Id, merit);
                    if (contractTempCount >= contractCount) continue;
                    UpdateVacancyOnTempTable(merit, item.HFId, item.HFMISCode, sanctioned, filled, vacant, regular, adhoc, contract, null, pgDistrictId, status + "Contract");
                    isHired = true;
                    break;
                }
            }
            return isHired;
        }

        private void UpdateVacancyOnTempTable(Merit merit, int? hfId, string hfmisCode, int sanctioned, int filled, int vacant, int regular, int adhoc, int contract, int? prefId, int? pgDisId, string status)
        {
            var meritVp = db.MeritsVps.FirstOrDefault(x => x.Merit_Id == merit.Id);
            if (meritVp != null) db.MeritsVps.Remove(meritVp);
            meritVp = new MeritsVp()
            {
                HrDesignation_Id = merit.Designation_Id,
                HF_Id = hfId,
                HFMISCode = hfmisCode,
                Merit_Id = merit.Id,
                ProcessedDate = DateTime.UtcNow.AddHours(5),
                MeritActiveDesignation_Id = merit.MeritsActiveDesignationId,
                Status = status,
                TotalSanctioned = sanctioned,
                TotalFilled = filled,
                TotalVacant = vacant,
                TotalRegular = regular,
                TotalAdhoc = adhoc,
                TotalContract = contract,
                MeritPreference_Id = prefId,
                MeritPGDistricts_Id = pgDisId
            };
            if (status == "Vacant" || status == "NoPrefrenceVacant" || status == "NoVacancyVacant" || status == "PGVacant" || status == "PGNoPrefrenceVacant")
            {
                meritVp.CurSanctioned = sanctioned;
                meritVp.CurFilled = filled + 1;
                meritVp.CurVacant = vacant - 1;
                meritVp.CurRegular = regular + 1;
                meritVp.CurAdhoc = adhoc;
                meritVp.CurContract = contract;
            }
            else if (status == "Adhoc" || status == "NoPrefrenceAdhoc" || status == "NoVacancyAdhoc" || status == "PGAdhoc" || status == "PGNoPrefrenceAdhoc")
            {
                meritVp.CurSanctioned = sanctioned;
                meritVp.CurFilled = filled;
                meritVp.CurVacant = vacant;
                meritVp.CurRegular = regular + 1;
                meritVp.CurAdhoc = adhoc - 1;
                meritVp.CurContract = contract;
            }
            else if (status == "Contract" || status == "NoPrefrenceContract" || status == "NoVacancyContract" || status == "PGContract" || status == "PGNoPrefrenceContract")
            {
                meritVp.CurSanctioned = sanctioned;
                meritVp.CurFilled = filled;
                meritVp.CurVacant = vacant;
                meritVp.CurRegular = regular + 1;
                meritVp.CurAdhoc = adhoc;
                meritVp.CurContract = contract - 1;
            }

            db.MeritsVps.Add(meritVp);
            db.SaveChanges();
        }
        private VpMDView GetVacancyPosition(Merit merit, int? hfId)
        {
            var queru = db.VpMDViews.AsQueryable();
            if (merit.Designation_Id == 1320 || merit.Designation_Id == 802 || merit.Designation_Id == 2404)
            {
                return db.VpMDViews.FirstOrDefault(x => (x.Desg_Id == merit.Designation_Id || (x.Desg_Id == 2404 && (x.HFTypeName == "Basic Health Unit" || x.FullName.StartsWith("MMC")))) && x.HFId == hfId && (x.Vacant > 0 || x.Adhoc > 0));
            }
            return db.VpMDViews.FirstOrDefault(x => x.Desg_Id == merit.Designation_Id && x.HFId == hfId);
        }

        private List<VpMDView> GetVacancyPositionFromDistrict(Merit merit, string districtCode)
        {
            if (merit.Designation_Id == 1320 || merit.Designation_Id == 802 || merit.Designation_Id == 2404)
            {
                return db.VpMDViews.Where(x => (x.Desg_Id == merit.Designation_Id || (x.Desg_Id == 2404 && (x.HFTypeName == "Basic Health Unit" || x.FullName.StartsWith("MMC")))) && x.DistrictCode.StartsWith(districtCode) && types.Contains(x.HFTypeCode)
                && !x.DistrictCode.StartsWith("035002") && (x.Vacant > 0 || x.Adhoc > 0)).OrderBy(x => x.HFTypeName == "District Headquarter Hospital" ? 0 : 1)
                .ThenBy(x => x.HFTypeName == "Tehsil Headquarter Hospital" ? 0 : 1)
                .ThenBy(x => x.HFTypeName == "Rural Health Center" ? 0 : 1)
                .ThenBy(x => x.HFTypeName == "Basic Health Unit" ? 0 : 1)
                .ToList();
            }

            return db.VpMDViews.Where(x => x.Desg_Id == merit.Designation_Id
        && x.DistrictCode.StartsWith(districtCode) && types.Contains(x.HFTypeCode)
        && !x.DistrictCode.StartsWith("035002") && (x.Vacant > 0 || x.Adhoc > 0)).OrderBy(x => x.HFTypeName == "District Headquarter Hospital" ? 0 : 1)
        .ThenBy(x => x.HFTypeName == "Tehsil Headquarter Hospital" ? 0 : 1)
        .ThenBy(x => x.HFTypeName == "Rural Health Center" ? 0 : 1)
        .ThenBy(x => x.HFTypeName == "Basic Health Unit" ? 0 : 1)
        .ToList();
        }

        private List<VpMDView> GetVacancyPositionFromTehsil(Merit merit, string tehsilCode)
        {
            if (merit.Designation_Id == 1320 || merit.Designation_Id == 802 || merit.Designation_Id == 2404)
            {
                return db.VpMDViews.Where(x => (x.Desg_Id == merit.Designation_Id || (x.Desg_Id == 2404 && (x.HFTypeName == "Basic Health Unit" || x.FullName.StartsWith("MMC")))) && x.TehsilCode.StartsWith(tehsilCode) && types.Contains(x.HFTypeCode)
                 && (x.Vacant > 0 || x.Adhoc > 0)).OrderBy(x => x.HFTypeName == "District Headquarter Hospital" ? 0 : 1)
                .ThenBy(x => x.HFTypeName == "Tehsil Headquarter Hospital" ? 0 : 1)
                .ThenBy(x => x.HFTypeName == "Rural Health Center" ? 0 : 1)
                .ThenBy(x => x.HFTypeName == "Basic Health Unit" ? 0 : 1)
                .ToList();
            }

            return db.VpMDViews.Where(x => x.Desg_Id == merit.Designation_Id
                && x.TehsilCode.StartsWith(tehsilCode) && types.Contains(x.HFTypeCode)
                && (x.Vacant > 0 || x.Adhoc > 0 || x.Contract > 0)).OrderBy(x => x.HFTypeName == "District Headquarter Hospital" ? 0 : 1)
                .ThenBy(x => x.HFTypeName == "Tehsil Headquarter Hospital" ? 0 : 1)
                .ThenBy(x => x.HFTypeName == "Rural Health Center" ? 0 : 1)
                .ThenBy(x => x.HFTypeName == "Basic Health Unit" ? 0 : 1)
                .ToList();
        }

        private MeritsVp GetVacancyPostionFromTempTable(Merit merit, int? hfId)
        {
            var query = db.MeritsVps.OrderByDescending(x => x.Id).AsQueryable();
            return query.FirstOrDefault(x => x.HrDesignation_Id == merit.Designation_Id && x.HF_Id == hfId && x.MeritActiveDesignation_Id == merit.MeritsActiveDesignationId);
        }

        private List<MeritPreference> GetPreferences(Merit merit)
        {

            return db.MeritPreferences.Where(x => x.Merit_Id == merit.Id && x.IsActive == true).OrderBy(x => x.PrefrencesOrder).ToList();
        }


        [Route("RemovePreference/{hfmisCode}/{Merit_Id}")]
        [HttpGet]
        public IHttpActionResult RemovePreference(int Merit_Id, string hfmisCode)
        {
            using (var transc = db.Database.BeginTransaction())
            {
                try
                {
                    var reorder = 0;
                    var prefrences = db.MeritPreferences.Where(x => x.Merit_Id == Merit_Id).ToList();
                    foreach (var pre in prefrences)
                    {
                        if (pre.HfmisCode == hfmisCode)
                        {
                            db.MeritPreferences.Remove(pre);
                        }
                        else
                        {
                            pre.PrefrencesOrder = ++reorder;
                        }
                    }
                    db.SaveChanges();
                    transc.Commit();
                    return Ok(true);
                }
                catch (Exception ex)
                {
                    transc.Rollback();
                    return BadRequest(ex.Message);
                }
            }
        }

        //[Route("RemoveDuplicates/{meritDesgId=meritDesgId}")]
        //[HttpGet]
        //public IHttpActionResult RemoveDuplicates(int meritDesgId)
        //{
        //    try
        //    {
        //        int[] meritIds = { 10197};
        //        int[] hfIds = { 12160};



        //        List<MeritPreference> preference = new List<MeritPreference>();
        //        for (int i = 0; i < meritIds.Length; i++)
        //        {
        //            var meritId = meritIds[i];
        //            var hfId = hfIds[i];

        //            preference.AddRange(db.MeritPreferences.Where(x => x.Merit_Id == meritId && x.HF_Id == hfId).OrderBy(x => x.PrefrencesOrder).ToList().Skip(1).ToList());
        //        }

        //        foreach (var item in preference)
        //        {
        //            db.MeritPreferences.Remove(item);
        //        }
        //        db.SaveChanges();
        //         return Ok("");
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //}


        //        [Route("SetPG")]
        //        [HttpGet]
        //        public IHttpActionResult SetPG()
        //        {

        //            var meritIds = new int?[] { 6695
        //,6700
        //,6704
        //,6718
        //,6725
        //,6726
        //,6728
        //,6730
        //,6738
        //,6746
        //,6752
        //,6754
        //,6756
        //,6759
        //,6767
        //,6770
        //,6776
        //,6779
        //,6780
        //,6802
        //,6805
        //,6808
        //,6811
        //,6813
        //,6815
        //,6816
        //,6817
        //,6818
        //,6824
        //,6826
        //,6828
        //,6864
        //,6866
        //,6874
        //,6876
        //,6877
        //,6886
        //,6892
        //,6914
        //,6920
        //,6925
        //,6930
        //,6932
        //,6934
        //,6939
        //,6940
        //,6944
        //,6953
        //,6964
        //,6977
        //,7004
        //,7021
        //,7026
        //,7036
        //,7039
        //,7041
        //,7044
        //,7052
        //,7056
        //,7064
        //,7065
        //,7066
        //,7067
        //,7069
        //,7071
        //,7101
        //,7125
        //,7138
        //,7161
        //,7181
        //,7182
        //,7191
        //,7196
        //,7201
        //,7202
        //,7205
        //,7206
        //,7208
        //,7209
        //,7211
        //,7215
        //,7231
        //,7233
        //,7243
        //,7248
        //,7250
        //,7253
        //,7272
        //,7273
        //,7288
        //,7306
        //,7308
        //,7331
        //,7341
        //,7343
        //,7346
        //,7355
        //,7375
        //,7382
        //,7385
        //,7432
        //,7441
        //,7442
        //,7476
        //,7497
        //,7508
        //,7514
        //,7515
        //,7520
        //,7527
        //,7536
        //,7543
        //,7581
        //,7583
        //,7593
        //,7597
        //,7620
        //,7623
        //,7633
        //,7637
        //,7644
        //,7650
        //,7657
        //,7659
        //,7668
        //,7672
        //,7685
        //,7694
        //,7701
        //,7705
        //,7727
        //,7795
        //,7805
        //,7806
        //,7808
        //,7810
        //,7812
        //,7820
        //,7835
        //,7861
        //,7873
        //,7885
        //,7888
        //,7898
        //,7901
        //,7910
        //,7912
        //,7916
        //,7945
        //,7947
        //,7954
        //,7973
        //,7985
        //,7987
        //,8001
        //,8015
        //,8037
        //,8040
        //,8043
        //,8045
        //,8070
        //,8071
        //,8075
        //,8083
        //,8087
        //,8089
        //,8090
        //,8097
        //,8106
        //,8122
        //,8128
        //,8129
        //,8130
        //,8131
        //,8155
        //,8162
        //,8172
        //,8178
        //,8203
        //,8229
        //,8250
        //,8263
        //,8264
        //,8274
        //,8282
        //,8283
        //,8288
        //,8291
        //,8292
        //,8302
        //,8319
        //,8320
        //,8348
        //,8363
        //,8394
        //,8402
        //,8425
        //,8429
        //,8440
        //,8453
        //,8457
        //,8458
        //,8472
        //,8476
        //,8480
        //,8481
        //,8518
        //,8525
        //,8573
        //,8586
        //,8608
        //,8610
        //,8625
        //,8626
        //,8632
        //,8639
        //,8640
        //,8647
        //,8653
        //,8657
        //,8658
        //,8659
        //,8669
        //,8691
        //,8705
        //,8716
        //,8717
        //,8731
        //,8732
        //,8740
        //,8757
        //,8761
        //,8762
        //,8791
        //,8820
        //,8840
        //,8860
        //,8866
        //,8888
        //,8891
        //,8911
        //,8912
        //,8913
        //,8918
        //,8934
        //,9020
        //,9021
        //,9022
        //,9039
        //,9040
        //,9045
        //,9079
        //,9089
        //,9093
        //,9100
        //,9103
        //,9104
        //,9119
        //,9121
        //,9129
        //,9147
        //,9155
        //,9163
        //,9181
        //,9184
        //,9201
        //,9215
        //,9220
        //,9225
        //,9232
        //,9236
        //,9237
        //,9238
        //,9242
        //,9250
        //,9290
        //,9292
        //,9293
        //,9297
        //,9347
        //,9363
        //,9379
        //,9395
        //,9405}; // WMO PG less THAN One Year 

        //            meritIds = new int?[]{
        //                10444
        //,10462
        //,10526
        //,10579
        //,10696
        //,10946
        //,11207
        //,11327
        //,11336
        //,11441
        //,11493
        //,11580
        //,11632
        //,11997
        //,12019
        //,12200
        //,12379
        //,12424
        //,12440
        //,12480
        //,12516
        //,12592
        //,12659
        //,13011
        //,13040
        //,13759
        //,13784  };  // MO less than One Year


        //            var pgs = db.MeritPGs.Include(x => x.Merit).Where(x => meritIds.Contains(x.Merit_Id)).ToList();

        //            List<MeritPGDistrict> pgDistricts = new List<MeritPGDistrict>();

        //            using (var trans = db.Database.BeginTransaction())
        //            {
        //                try
        //                {
        //                    foreach (var pg in pgs)
        //                    {

        //                        var districtCodes = db.MeritPreferences.Where(x => x.Merit_Id == pg.Merit_Id).OrderBy(x => x.PrefrencesOrder)
        //                            .ToList().Select(x => x.HfmisCode.Substring(0, 6)).Distinct().ToList();

        //                        int count = 0;
        //                        foreach (var code in districtCodes)
        //                        {
        //                            var district = db.Districts.Where(x => x.Code == code).FirstOrDefault();
        //                            pgDistricts.Add(new MeritPGDistrict() { MeritPG_Id = pg.Id, Merit_Id = pg.Merit.Id, Districts_Id = district.Id, DistrictCode = district.Code, PrefrencesOrder = ++count, DateCreated = DateTime.UtcNow.AddHours(5) });
        //                        }

        //                    }
        //                    db.MeritPGDistricts.AddRange(pgDistricts);
        //                    db.SaveChanges();
        //                    trans.Commit();
        //                }
        //                catch (Exception)
        //                {

        //                    throw;
        //                }
        //            }

        //            return Ok("ok");
        //        }

        [Route("RemovePgPreference/{meritId}/{meritPgDistrictId}")]
        [HttpGet]
        public IHttpActionResult RemovePgPreference(int meritId, int meritPgDistrictId)
        {
            using (var transc = db.Database.BeginTransaction())
            {
                try
                {
                    var reorder = 0;
                    var prefrences = db.MeritPGDistricts.Where(x => x.Merit_Id == meritId).ToList();
                    foreach (var pre in prefrences)
                    {
                        if (pre.Id == meritPgDistrictId)
                        {
                            db.MeritPGDistricts.Remove(pre);
                        }
                        else
                        {
                            pre.PrefrencesOrder = ++reorder;
                        }
                    }
                    db.SaveChanges();
                    transc.Commit();
                    return Ok(true);
                }
                catch (Exception ex)
                {
                    transc.Rollback();
                    return BadRequest(ex.Message);
                }
            }
        }


        [Route("RemoveAcceptance/{Merit_Id}")]
        [HttpGet]
        public IHttpActionResult RemoveAcceptance(int Merit_Id)
        {
            try
            {
                if (File.Exists(HttpContext.Current.Server.MapPath(@"~\Uploads\Acceptances\" + Merit_Id + "_OfferLetter.jpg")))
                {
                    File.Delete(HttpContext.Current.Server.MapPath(@"~\Uploads\Acceptances\" + Merit_Id + "_OfferLetter.jpg"));

                }
                var merit = db.Merits.FirstOrDefault(x => x.Id == Merit_Id);
                merit.Status = "ProfileBuilt";
                db.Entry(merit).State = EntityState.Modified;
                db.SaveChanges();
                return Ok(true);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }
        [Route("GetPreferences/{MeritID}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetPreferences(int MeritID)
        {
            try
            {

                List<string> hfs = new List<string>();
                List<MeritPreference> meritPrefs = db.MeritPreferences.Where(x => x.Merit_Id == MeritID && x.IsActive == true).OrderBy(x => x.PrefrencesOrder).ToList();
                foreach (var prefs in meritPrefs)
                {
                    string hfName = db.HealthFacilityDetails.FirstOrDefault(x => x.HFMISCode.Equals(prefs.HfmisCode)).FullName;
                    hfs.Add(hfName);
                }
                //var listy = db.Database.SqlQuery<string>("select top 10 hf.FullName from MeritPreferences mp left join HealthFacilityDetail hf on hf.HFMISCode = mp.HfmisCode where mp.Merit_Id=@param", new SqlParameter("@param", MeritID)).ToList();
                return Ok(hfs.Take(10));
            }
            catch (Exception ex)
            {
                return Ok(false);
            }

        }
        [Route("GetGrievances/{MeritID}")]
        [HttpGet]
        public IHttpActionResult GetGrievances(int MeritID)
        {
            try
            {
                List<string> hfs = new List<string>();
                List<WMO_Grievances> meritPrefs = db.WMO_Grievances.Where(x => x.Merit_Id == MeritID).OrderBy(x => x.Id).ToList();
                foreach (var prefs in meritPrefs)
                {
                    string hfName = db.HealthFacilityDetails.FirstOrDefault(x => x.HFMISCode.Equals(prefs.HFMISCode)).FullName;
                    hfs.Add(hfName);
                }
                List<MeritGrievience> mgs = db.MeritGrieviences.Where(x => x.Merit_Id == MeritID).ToList();
                return Ok(new { hfs = hfs.Take(10), mgs = mgs });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [Route("GetPreferencesList/{MeritID}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetPreferencesList(int MeritID)
        {
            try
            {

                List<HealthFacilityDetail> hfs = new List<HealthFacilityDetail>();
                List<MeritPreference> meritPrefs = db.MeritPreferences.Where(x => x.Merit_Id == MeritID && x.IsActive == true).OrderBy(x => x.PrefrencesOrder).ToList();
                foreach (var prefs in meritPrefs)
                {
                    HealthFacilityDetail hf = db.HealthFacilityDetails.FirstOrDefault(x => x.HFMISCode.Equals(prefs.HfmisCode));
                    hfs.Add(hf);
                }
                // var listy = db.Database.SqlQuery<string>("select top 10 dis.Name from MeritPreferences mp left join Districts dis on dis.Code = mp.HfmisCode where mp.Merit_Id=@param", new SqlParameter("@param", MeritID)).ToList();
                return Ok(hfs);
            }
            catch (Exception ex)
            {
                return Ok(false);
            }
        }


        [Route("ChangePGStatus/{activeDesignationId}/{meritNumber}/{isPG}")]
        [HttpGet]
        public IHttpActionResult ChangePGStatus(int activeDesignationId, int meritNumber, int isPG)
        {
            try
            {
                var merit = db.Merits.FirstOrDefault(x => x.MeritsActiveDesignationId == activeDesignationId && x.MeritNumber == meritNumber);
                if (merit != null)
                {
                    var meritPG = db.MeritPGs.FirstOrDefault(x => x.Merit_Id == merit.Id);
                    if (meritPG != null)
                    {
                        if (isPG == 1)
                        {
                            meritPG.isPG = true;
                        }
                        else
                        {
                            meritPG.isPG = false;
                        }
                        db.SaveChanges();
                    }
                    var meritPreferences = db.MeritPreferences.Where(x => x.Merit_Id == meritPG.Merit_Id).ToList();
                    db.MeritPreferences.RemoveRange(meritPreferences);
                    db.SaveChanges();
                    return Ok(true);
                }
                return Ok(false);
            }
            catch (Exception ex)
            {
                return Ok(false);
            }
        }


        [Route("GetPgPreferences/{meritId}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetPgPreferences(int meritId)
        {
            try
            {
                //List<MeritPgDistrictViewModel> meritPgPrefs = db.MeritPGDistricts.Where(x => x.Merit_Id == meritId).OrderBy(x => x.PrefrencesOrder)
                //    .Include(x => x.District).Select(x => new MeritPgDistrictViewModel() { Id = x.Id, Merit_Id = x.Merit_Id, MeritPG_Id = x.MeritPG_Id, DistrictCode = x.DistrictCode, Districts_Id = x.Districts_Id, District = new DistrictViewModel() { Id = x.District.Id, Name = x.District.Name, Code = x.District.Code } }).ToList();
                List<HealthFacilityDetail> hfs = new List<HealthFacilityDetail>();
                List<MeritPreference> meritPrefs = db.MeritPreferences.Where(x => x.Merit_Id == meritId && x.IsActive == true).OrderBy(x => x.PrefrencesOrder).ToList();
                foreach (var prefs in meritPrefs)
                {
                    var district = db.Districts.FirstOrDefault(x => x.Code.Equals(prefs.HfmisCode));

                    HealthFacilityDetail hf = new HealthFacilityDetail
                    {
                        FullName = district.Name,
                        HFMISCode = district.Code
                    };
                    hfs.Add(hf);
                }
                return Ok(hfs);
            }
            catch (Exception ex)
            {
                return Ok(false);
            }
        }




        //[Route("SaveGrievance/{MeritId}/{hfmisCode}")]
        //[HttpGet]
        //public IHttpActionResult SaveGrievances(int MeritId, string hfmisCode)
        //{
        //    try
        //    {
        //        List<WMO_Grievances> listy = new List<WMO_Grievances>();
        //        var merit = db.Merits.FirstOrDefault(x => x.Id == MeritId);

        //        db.WMO_Grievances.Add(new WMO_Grievances
        //        {
        //            Merit_Id = MeritId,
        //            HFMISCode = hfmisCode,
        //            DateTime = DateTime.UtcNow.AddHours(5)
        //        });
        //        db.SaveChanges();

        //        int totalPrefs = db.WMO_Grievances.Where(x => x.Merit_Id == merit.Id).Count();
        //        if (totalPrefs == 10)
        //        {
        //            merit.Status = "Completed";
        //            db.Entry(merit).State = EntityState.Modified;
        //            db.SaveChanges();
        //            string message = "Your grievances has been saved successfully. Thank you.";
        //            var profile = db.HrProfiles.FirstOrDefault(x => x.CNIC == merit.CNIC);
        //            Common.SMS_Send(new List<SMS>() {
        //                    new SMS()
        //                        {
        //                            MobileNumber = profile.MobileNo,
        //                            Message = message
        //                        }
        //                    });


        //        }
        //        List<HealthFacilityDetail> hfs = new List<HealthFacilityDetail>();
        //        List<WMO_Grievances> meritPrefs = db.WMO_Grievances.Where(x => x.Merit_Id == MeritId).ToList();
        //        foreach (var prefs in meritPrefs)
        //        {
        //            HealthFacilityDetail hf = db.HealthFacilityDetails.FirstOrDefault(x => x.HFMISCode.Equals(prefs.HFMISCode));
        //            hfs.Add(hf);
        //        }
        //        return Ok(new { result = true, meritPrefs = hfs });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(false);
        //    }


        //}

        [Route("RemoveGrievance/{Merit_Id}/{hfmisCode}")]
        [HttpGet]
        public IHttpActionResult RemoveGrievance(int Merit_Id, string hfmisCode)
        {
            try
            {
                var pref = db.WMO_Grievances.FirstOrDefault(x => x.Merit_Id == Merit_Id && x.HFMISCode.Equals(hfmisCode));
                if (pref != null)
                {
                    db.WMO_Grievances.Remove(pref);
                }
                db.SaveChanges();
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("GetPreferencesListGrievance/{MeritID}")]
        [HttpGet]
        public IHttpActionResult GetPreferencesListGrievance(int MeritID)
        {
            try
            {

                List<HealthFacilityDetail> hfs = new List<HealthFacilityDetail>();
                List<WMO_Grievances> meritPrefs = db.WMO_Grievances.Where(x => x.Merit_Id == MeritID).ToList();
                foreach (var prefs in meritPrefs)
                {
                    HealthFacilityDetail hf = db.HealthFacilityDetails.FirstOrDefault(x => x.HFMISCode.Equals(prefs.HFMISCode));
                    hfs.Add(hf);
                }
                return Ok(hfs);
            }
            catch (Exception ex)
            {
                return Ok(false);
            }
        }

        [Route("isPG/{meritId=meritId}/{isOnPGT=isOnPGT}/{date=date}/{hfId=hfId}/{sepcsId=sepcsId}/{hfName=hfName}")]
        [HttpPost]
        public async Task<IHttpActionResult> isPG(int meritId, bool isOnPGT, DateTime? date, int? hfId, int? sepcsId, string hfName)
        {
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                string RootPath = HttpContext.Current.Server.MapPath("~/") + @"Uploads\Acceptances\";
                var dirPath = RootPath;
                var provider = new MultipartMemoryStreamProvider();
                if (isOnPGT)
                {
                    await Request.Content.ReadAsMultipartAsync(provider);
                }

                CreateDirectoryIfNotExists(dirPath);
                string filename = null;

                foreach (var file in provider.Contents)
                {
                    filename = meritId + "_AdmissionDocs" + Path.GetExtension(file.Headers.ContentDisposition.FileName.Trim('\"'));
                    var buffer = await file.ReadAsByteArrayAsync();
                    var size = ((buffer.Length) / (1024)) / (1024);
                    var ext = Path.GetExtension(filename.Replace("\"", string.Empty));
                    List<string> validExtensions = new List<string>() { ".png", ".PNG", ".jpg", ".JPG", ".jpeg", ".pdf", ".PDF" };
                    if (!validExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase) || size > 5)
                    {
                        throw new Exception("Unable to Upload. File Size must be less than 5 MB and File Format must be " + string.Join(",", validExtensions));
                    }
                    using (FileStream fsOut = File.OpenWrite(RootPath + @"\" + filename))
                    {
                        fsOut.Write(buffer, 0, buffer.Length);
                    }
                }


                DateTime dateFrom = date ?? DateTime.Now;
                if (date > DateTime.Now) date = date = DateTime.Now;

                MeritPG mpg = db.MeritPGs.FirstOrDefault(x => x.Merit_Id == meritId);
                if (mpg == null)
                {
                    mpg = new MeritPG();
                    mpg.Merit_Id = meritId;
                    mpg.HealthFacilities_Id = hfId;
                    mpg.Specialization_Id = sepcsId;
                    mpg.FileName = filename;
                    mpg.HealthFacilityName = hfName == "undefined" ? null : hfName;
                    //mpg.isPG = (((DateTime.Now - dateFrom)).Days / 365) >= 1 ? true : false;
                    mpg.isPG = isOnPGT;
                    mpg.PGFrom = date;
                    mpg.DateTime = DateTime.UtcNow.AddHours(5);
                    db.MeritPGs.Add(mpg);
                }
                else
                {
                    mpg.HealthFacilities_Id = hfId;
                    mpg.Specialization_Id = sepcsId;
                    mpg.FileName = filename;
                    mpg.HealthFacilityName = hfName;
                    mpg.isPG = (((DateTime.Now - dateFrom)).Days / 365) >= 1 ? true : false;
                    mpg.PGFrom = date;
                    mpg.DateTime = DateTime.UtcNow.AddHours(5);
                    db.Entry(mpg).State = EntityState.Modified;
                }
                db.SaveChanges();
                return Ok(mpg);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("isPGInit/{MeritID}")]
        [HttpGet]
        public IHttpActionResult isPGInit(int MeritID)
        {
            try
            {
                var isPG = db.MeritPGViews.FirstOrDefault(x => x.Merit_Id == MeritID);
                if (isPG == null)
                {
                    return Ok(404);
                }
                else
                {
                    return Ok(isPG);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }


        //[Route("SubmitGrievances")]
        //[HttpPost]
        //public IHttpActionResult SubmitGrievances(MeritGrievience mg)
        //{
        //    try
        //    {

        //        mg.CreatedDate = DateTime.UtcNow.AddHours(5);

        //        if (mg.Id != 0)
        //        {
        //            db.Entry(mg).State = EntityState.Modified;
        //        }
        //        else if (mg.Id == 0)
        //        {
        //            db.MeritGrieviences.Add(mg);
        //        }
        //        db.SaveChanges();
        //        return Ok(new { result = true });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}


        [Route("SubmitGrievances")]
        [HttpPost]
        public HttpResponseMessage SubmitGrievances(MeritGrievience mg)
        {
            try
            {

                mg.CreatedDate = DateTime.UtcNow.AddHours(5);

                if (mg.Id != 0)
                {
                    db.Entry(mg).State = EntityState.Modified;
                }
                else if (mg.Id == 0)
                {
                    db.MeritGrieviences.Add(mg);
                }
                db.SaveChanges();


                var response = Request.CreateResponse(HttpStatusCode.OK);
                HttpResponseMessage result;


                var merit = db.Merits.Include(x => x.MeritActiveDesignation).Include(x => x.HrDesignation).Include(x => x.HrDesignation.HrScale).Where(x => x.Id == mg.Merit_Id).OrderByDescending(x => x.Id).FirstOrDefault();
                if (merit == null) return Request.CreateResponse(HttpStatusCode.BadRequest, "Record not found");

                string filepath = System.Web.Hosting.HostingEnvironment.MapPath($@"~/Uploads/Offers/{merit.MeritActiveDesignation?.GrievanceLetterFileName}");
                var activeDsg = db.MeritActiveDesignations.FirstOrDefault(x => x.Id == merit.MeritsActiveDesignationId);
                using (DocX document = DocX.Load(filepath))
                {

                    document.ReplaceText("{{EmployeeName}}", merit.Name ?? string.Empty);
                    document.ReplaceText("{{FatherName}}", merit.FatherName ?? string.Empty);
                    document.ReplaceText("{{MeritNumber}}", merit.MeritNumber?.ToString() ?? string.Empty);
                    document.ReplaceText("{{MobileNumber}}", merit.MobileNumber ?? string.Empty);
                    document.ReplaceText("{{Address}}", merit.Address ?? string.Empty);
                    document.ReplaceText("{{DesginationName}}", merit.HrDesignation?.Name?.ToUpper() ?? string.Empty);
                    document.ReplaceText("{{DesginationNameLower}}", merit.HrDesignation?.Name ?? string.Empty);
                    document.ReplaceText("{{ApplicationNo}}", merit?.ApplicationNumber?.ToString() ?? string.Empty);
                    document.ReplaceText("{{grienvances}}", mg?.Remarks ?? string.Empty);
                    if (activeDsg?.DateEnd < DateTime.Now)
                    {
                        document.ReplaceText("{{Dated}}", activeDsg?.DateEnd?.ToString("dd/MM/yyyy") ?? string.Empty);
                    }
                    else
                    {
                        document.ReplaceText("{{Dated}}", DateTime.UtcNow.AddHours(5).ToString("dd/MM/yyyy"));
                    }

                    using (var str = new MemoryStream())
                    {
                        document.SaveAs(str);
                        //byte[] dataFile = str.ToArray();
                        byte[] dataFile = SaveAsPdf(str);

                        result = Request.CreateResponse(HttpStatusCode.OK);
                        result.Content = new ByteArrayContent(dataFile);
                        result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                        {
                            FileName = "Grienvance.pdf"
                        };
                        merit.OfferLetterKey = null;
                        merit.OfferLetterKeyExpiry = null;
                        db.SaveChanges();
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }
        }


        [Route("UploadGrievance/{meritId=meritId}")]
        [HttpPost]
        public async Task<IHttpActionResult> UploadGrievance(int meritId)
        {
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                string RootPath = HttpContext.Current.Server.MapPath("~/") + @"Uploads\Acceptances\";
                var dirPath = RootPath;
                var provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);

                CreateDirectoryIfNotExists(dirPath);
                string filename = null;

                foreach (var file in provider.Contents)
                {
                    filename = meritId + "_GrievanceDocs" + Path.GetExtension(file.Headers.ContentDisposition.FileName.Trim('\"'));
                    var buffer = await file.ReadAsByteArrayAsync();
                    var size = ((buffer.Length) / (1024)) / (1024);
                    var ext = Path.GetExtension(filename.Replace("\"", string.Empty));
                    List<string> validExtensions = new List<string>() { ".png", ".PNG", ".jpg", ".JPG", ".jpeg", ".pdf", ".PDF" };
                    if (!validExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase) || size > 10)
                    {
                        throw new Exception("Unable to Upload. File Size must be less than 10 MB and File Format must be " + string.Join(",", validExtensions));
                    }
                    using (FileStream fsOut = File.OpenWrite(RootPath + @"\" + filename))
                    {
                        fsOut.Write(buffer, 0, buffer.Length);
                    }
                }

                var merit = db.Merits.FirstOrDefault(x => x.Id == meritId);
                merit.GrievanceLetter = filename;
                db.SaveChanges();
                Common.SMS_Send(new List<SMS>() {
                            new SMS()
                                {
                                    MobileNumber = merit.MobileNumber,
                                    Message = "Your grievances saved successfully."
                                }
                            });
                Common.SendEmail(merit.Email, "Grievances saved", $"Dear  {merit.Name}, Your grievances saved successfully.");
                return Ok("ok");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }




        public static byte[] SaveAsPdf(Stream str)
        {
            //Load Document
            Document document = new Document();

            document.LoadFromStream(str, FileFormat.Docx);

            using (var stream = new MemoryStream())
            {
                //Convert Word to PDF
                document.SaveToStream(stream, FileFormat.PDF);
                return stream.ToArray();
            }
        }
        [Route("UploadReAcceptance/{CNIC}")]
        public async Task<IHttpActionResult> UploadReAcceptance(string CNIC)
        {
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                string RootPath = HttpContext.Current.Server.MapPath("~/") + @"Uploads\Acceptances\";
                var dirPath = RootPath;

                var provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);

                CreateDirectoryIfNotExists(dirPath);
                string filename = "";
                Merit merit = db.Merits.FirstOrDefault(x => x.CNIC.Equals(CNIC));
                foreach (var file in provider.Contents)
                {
                    //filename = merit.Id + "_OfferLetter." + file.Headers.ContentDisposition.FileName.Trim('\"').Split('.')[1];
                    filename = merit.Id + "_OfferLetter.jpg";
                    var buffer = await file.ReadAsByteArrayAsync();
                    var size = ((buffer.Length) / (1024)) / (1024);
                    var ext = Path.GetExtension(filename.Replace("\"", string.Empty));
                    List<string> validExtensions = new List<string>() { ".jpg", ".jpeg" };
                    if (!validExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase) || size > 5)
                    {
                        throw new Exception("Unable to Upload. File Size must be less than 5 MB and File Format must be " + string.Join(",", validExtensions));
                    }
                    using (FileStream fsOut = File.OpenWrite(RootPath + @"\" + filename))
                    {
                        fsOut.Write(buffer, 0, buffer.Length);
                    }

                    merit.Status = "Completed";
                    db.Entry(merit).State = EntityState.Modified;

                    await db.SaveChangesAsync();
                }
                return Ok(new { result = true, src = filename });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("ResendNotifications/{CNIC}")]
        [HttpGet]
        public async Task<IHttpActionResult> ResendNotifications(string CNIC)
        {
            try
            {

                var merit = db.Merits.FirstOrDefault(x => x.CNIC.Equals(CNIC));
                merit.Status = "ProfileBuilt";
                db.SaveChanges();

                string message = "Dear Candidate,\nWe did not recieve your signed copy of acceptance letter. You are requested to resubmit your acceptance. Please carefully read the guidelines on upload button. You can also change your preferences on Online Portal.";

                var profile = db.HrProfiles.FirstOrDefault(x => x.CNIC == merit.CNIC);
                Common.SMS_Send(new List<SMS>() {
                            new SMS()
                                {
                                    MobileNumber = profile.MobileNo,
                                    Message = message
                                }
                            });

                try
                {
                    Common.SendEmail(profile.EMaiL, "Primary Secondary Healthcare Department", message);
                }
                catch (Exception ex)
                {


                }
                //if (File.Exists(HttpContext.Current.Server.MapPath(@"~\Uploads\Acceptances\" + merit.Id + "_OfferLetter.jpg")))
                //{
                //    File.Delete(HttpContext.Current.Server.MapPath(@"~\Uploads\Acceptances\" + merit.Id + "_OfferLetter.jpg"));
                //}

                return Ok(true);
            }
            catch (Exception ex)
            {
                return Ok(false);
            }
        }

        [Route("CountStatus")]
        [HttpGet]
        public IHttpActionResult CountStatus()
        {
            try
            {
                List<int> counts = new List<int>();
                int count = 0;
                count = db.Merits.Where(x => x.Status.Equals("Accepted") || x.Status.Equals("Completed")).Count();
                counts.Add(count);
                count = db.Merits.Where(x => x.Status.Equals("Acknowledged")).Count();
                counts.Add(count);
                count = db.Merits.Where(x => x.Status.Equals("ReAcceptence")).Count();
                counts.Add(count);
                return Ok(counts);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }



        #region Khizar for Consultants

        [HttpGet]
        [Route("GetActiveMeritDesignations")]
        public IHttpActionResult GetActiveMeritDesignations()
        {
            try
            {
                var actvDesignations = db.MeritActiveDesignations.Where(x => x.IsActive == "Y").Select(x => x.Desg_Id).ToList();
                var desginations = db.HrDesignationViews.Where(x => actvDesignations.Contains(x.Id));
                return Ok(desginations);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }



        [HttpPost]
        [Route("SaveActiveDesignation")]
        public async Task<IHttpActionResult> SaveActiveDesignation()
        {
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                string RootPath = HttpContext.Current.Server.MapPath("~/") + @"Uploads\Offers\";

                var provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);
                MeritActiveDesignation model = null;
                CreateDirectoryIfNotExists(RootPath);
                string filename = "", offerLetter = null, acceptanceLetter = null;

                foreach (var item in provider.Contents.Where(x => x.Headers.ContentDisposition.Name.Replace("\"", "") == "model"))
                {
                    model = JsonConvert.DeserializeObject<MeritActiveDesignation>(await item.ReadAsStringAsync());
                    break;
                }
                foreach (var file in provider.Contents)
                {
                    if (file.Headers.ContentDisposition.Name.Replace("\"", "") == "model")
                    {
                        continue;
                    }

                    if (file.Headers.ContentDisposition.Name.Replace("\"", "") == "offerLetter")
                    {
                        filename = $"{Common.RandomString(6)}_{model.DateStart?.ToString("dd_MM_yyyy")}_{model.Name.Replace(" ", "_")}_OfferLetter{Path.GetExtension(file.Headers.ContentDisposition.FileName.Replace("\"", ""))}";
                        offerLetter = filename;
                    }
                    else if (file.Headers.ContentDisposition.Name.Replace("\"", "") == "acceptanceLetter")
                    {
                        filename = $"{Common.RandomString(6)}_{model.DateStart?.ToString("dd_MM_yyyy")}_{model.Name.Replace(" ", "_")}_Acceptance{Path.GetExtension(file.Headers.ContentDisposition.FileName.Replace("\"", ""))}";
                        acceptanceLetter = filename;
                    }

                    var buffer = await file.ReadAsByteArrayAsync();
                    var size = ((buffer.Length) / (1024)) / (1024);
                    var ext = Path.GetExtension(filename.Replace("\"", string.Empty));
                    List<string> validExtensions = new List<string>() { ".docx", ".doc" };
                    if (!validExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase) || size > 5)
                    {
                        throw new Exception("Unable to Upload. File Size must be less than 5 MB and File Format must be " + string.Join(",", validExtensions));
                    }
                    using (FileStream fsOut = File.OpenWrite(RootPath + @"\" + filename))
                    {
                        fsOut.Write(buffer, 0, buffer.Length);
                    }
                }

                if (model.Id > 0)
                {
                    var existing = db.MeritActiveDesignations.FirstOrDefault(x => x.Id == model.Id);
                    if (existing != null)
                    {
                        existing.ApplicantsCount = model.ApplicantsCount;
                        existing.DateStart = model.DateStart;
                        existing.DateEnd = model.DateEnd;
                        existing.Desg_Id = model.Desg_Id;
                        existing.Name = model.Name;
                        existing.PreferencesOnly = model.PreferencesOnly;
                        existing.OfferLetterFileName = offerLetter;
                        existing.AcceptanceLetterFileName = acceptanceLetter;
                        db.SaveChanges();
                        return Ok("ok");
                    }
                }

                model.IsActive = "Y";
                model.OfferLetterFileName = offerLetter;
                model.AcceptanceLetterFileName = acceptanceLetter;
                db.MeritActiveDesignations.Add(model);
                db.SaveChanges();
                return Ok("ok");
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("GetActiveDesignationsDetails")]
        public IHttpActionResult GetActiveDesignationsDetails()
        {
            try
            {
                var list = db.MeritActiveDesignations.Where(x => x.IsActive == "Y").ToList();
                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("GetEnableDesignationsDetails")]
        public IHttpActionResult GetEnableDesignationsDetails()
        {
            try
            {
                var list = db.MeritActiveDesignations.Where(x => x.IsEnable == true).ToList();
                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("GetMeritActiveDesignation/{id=id}")]
        public IHttpActionResult GetActiveDesignation(int id)
        {
            try
            {
                var list = db.MeritActiveDesignations.FirstOrDefault(x => x.Id == id);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("RemoveActiveDesignations/{id=id}")]
        public IHttpActionResult RemoveActiveDesignations(int id)
        {
            try
            {
                var item = db.MeritActiveDesignations.FirstOrDefault(x => x.Id == id);
                item.IsActive = "N";
                db.SaveChanges();
                return Ok("ok");
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        private ApplicationUser GetUserByName(string username)
        {
            using (var db = new ApplicationDbContext())
            {
                return db.Users.FirstOrDefault(x => x.UserName == username);
            }
        }

        [HttpPost]
        [Route("SaveApplicant/{generatePassword}")]
        public IHttpActionResult SaveApplicant(bool generatePassword, [FromBody] Merit model)
        {
            try
            {

                string password = string.Empty;
                var desg = db.HrDesignations.FirstOrDefault(x => x.Id == model.MeritActiveDesignation.Desg_Id);
                var profile = db.HrProfiles.FirstOrDefault(x => x.CNIC == model.CNIC.Replace("-", ""));
                var user = GetUserByName(model.CNIC.Replace("-", ""));

                if (profile == null)
                {
                    db.HrProfiles.Add(new HrProfile() { EmployeeName = model.Name, FatherName = model.FatherName, EMaiL = model.Email, CNIC = model.CNIC?.Replace("-", ""), MobileNo = model.MobileNumber?.Replace("-", ""), CorrespondenceAddress = model.Address, PermanentAddress = model.Address, Designation_Id = desg?.Id, WDesignation_Id = desg?.Id, Cadre_Id = desg.Cadre_Id });
                }
                var merit = new Merit() { Name = model.Name, FatherName = model.FatherName, CNIC = model.CNIC?.Replace("-", ""), MobileNumber = model.MobileNumber?.Replace("-", ""), Email = model.Email, PMDCNumber = model.PMDCNumber, MeritNumber = model.MeritNumber, ApplicationNumber = model.ApplicationNumber, Designation_Id = model.MeritActiveDesignation.Desg_Id, MeritsActiveDesignationId = model.MeritActiveDesignation?.Id, Address = model.Address, PPSCDate = model.PPSCDate, PPSCNumber = model.PPSCNumber, LetterNumber = model.PPSCNumber, LetterDate = model.PPSCDate, District_Id = model.District_Id, IsDisabled = model.IsDisabled, Status = (user == null) ? "New" : "Existing", HF_Id = model.HF_Id, WorkingStatus = model.WorkingStatus };
                db.Merits.Add(merit);
                var UserManger = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();

                if (user == null && generatePassword)
                {

                    password = Common.RandomString(6);

                    var usr = new ApplicationUser()
                    {
                        UserName = merit.CNIC,
                        Email = model.Email,
                        hashynoty = password,
                        PhoneNumber = merit.MobileNumber,
                        responsibleuser = User.Identity.GetUserName(),
                        CreationDate = DateTime.UtcNow.AddHours(5),
                        isActive = true
                    };

                    IdentityResult result = UserManger.Create(usr, password);
                    if (!result.Succeeded)
                    {

                    }
                    else
                    {
                        merit.Status = "Existing";
                        UserManger.AddToRole(usr.Id, "JobApplicant");
                    }
                }
                else
                {
                    password = user?.hashynoty;
                }
                db.SaveChanges();
                return Ok(password);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }


        [HttpGet]
        [Route("SaveDiplomaStatus/{meritId=meritId}/{isAccepted=isAccepted}")]
        public IHttpActionResult SaveDiplomaStatus(int meritId, bool isAccepted)
        {
            try
            {
                var merit = db.Merits.FirstOrDefault(x => x.Id == meritId);
                if (merit == null) return BadRequest("Not Found");
                merit.DiplomaOffer = isAccepted;
                db.SaveChanges();
                return Ok("ok");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }



        [HttpGet]
        [AllowAnonymous]
        [Route("SaveApplicantTemp")]
        public IHttpActionResult SaveApplicantTemp()
        {
            try
            {

                SaveApplicantTempService();
                return Ok("ok");
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        private void SaveApplicantTempService()
        {
            string jsonStr = File.ReadAllText(@"E:\Work\Projects\DotNet\HISDU\Docs\HRMIS\PreferencesDocsWmo\MO.json");
            List<MeritTemp> temps = JsonConvert.DeserializeObject<List<MeritTemp>>(jsonStr);

            //var dis = temps.Select(x => x.Domicile.ToLower()).Distinct().ToArray();
            //var disHr = db.Districts.Select(x => x.Name.ToLower()).ToArray();
            //var d = dis.Except(disHr).ToArray();

            List<Merit> tempMerit = new List<Merit>();
            foreach (var item in temps)
            {
                var district = db.Districts.FirstOrDefault(x => x.Name.ToLower() == item.Domicile.ToLower().Trim());
                tempMerit.Add(new Merit() { PPSCSrNo = item.SrNo, Name = item.Name, FatherName = item.FatherName, CNIC = item.CNIC?.Replace("-", ""), MobileNumber = "0" + item.MobileNumber?.Replace("-", ""), Email = item.Email, PMDCNumber = null, MeritNumber = item.MeritNumber, ApplicationNumber = item.ApplicationNumber, MeritsActiveDesignationId = 25, Address = item.Address, PPSCDate = new DateTime(2018, 11, 6), PPSCNumber = $"NO. SO (MO) /{item.MeritNumber}/R/2018", LetterNumber = $"NO. SO (MO) /{item.MeritNumber}/R/2018", LetterDate = new DateTime(2018, 11, 6), Designation_Id = 802, District_Id = district?.Id, IsDisabled = false });
            }


            var districtNotFound = tempMerit.Where(x => x.District_Id == null).ToList();
            var mobileInvalid = tempMerit.Where(x => !x.MobileNumber.StartsWith("0")).ToList();
            var cnicInvalid = tempMerit.Where(x => x.CNIC == null || string.IsNullOrWhiteSpace(x.CNIC)).ToList();

            foreach (var item in tempMerit.Where(x => x.MeritNumber > 3480))
            {
                try
                {
                    InsertRecords(item);
                }
                catch (Exception ex)
                {
                    InsertRecords(item);
                }

            }
        }

        private void InsertRecords(Merit item)
        {
            var desg = db.HrDesignations.FirstOrDefault(x => x.Id == item.Designation_Id);
            var profile = db.HrProfiles.FirstOrDefault(x => x.CNIC == item.CNIC.Replace("-", ""));
            var user = GetUserByName(item.CNIC.Replace("-", ""));

            if (profile == null)
            {
                db.HrProfiles.Add(new HrProfile() { EmployeeName = item.Name, FatherName = item.FatherName, EMaiL = item.Email, CNIC = item.CNIC?.Replace("-", ""), MobileNo = item.MobileNumber?.Replace("-", ""), CorrespondenceAddress = item.Address, PermanentAddress = item.Address, Designation_Id = item.Designation_Id, WDesignation_Id = desg.Id, Cadre_Id = desg.Cadre_Id });
            }
            var merit = new Merit() { PPSCSrNo = item.PPSCSrNo, Name = item.Name, FatherName = item.FatherName, CNIC = item.CNIC?.Replace("-", ""), MobileNumber = item.MobileNumber, Email = item.Email, PMDCNumber = item.PMDCNumber, MeritNumber = item.MeritNumber, ApplicationNumber = item.ApplicationNumber, Designation_Id = desg.Id, MeritsActiveDesignationId = item.MeritsActiveDesignationId, Address = item.Address, PPSCDate = item.PPSCDate, PPSCNumber = item.PPSCNumber, LetterNumber = item.PPSCNumber, LetterDate = item.PPSCDate, District_Id = item.District_Id, IsDisabled = item.IsDisabled, Status = (user == null) ? "New" : "Existing" };
            db.Merits.Add(merit);
            db.SaveChanges();
        }


        [HttpPost]
        [Route("UpdateApplicant/{generatePassword}")]
        public IHttpActionResult UpdateApplicant(bool generatePassword, [FromBody] Merit model)
        {
            try
            {
                //var model = JsonConvert.DeserializeObject<Merit>(json);
                var desg = db.HrDesignations.FirstOrDefault(x => x.Id == model.MeritActiveDesignation.Desg_Id);
                var profile = db.HrProfiles.FirstOrDefault(x => x.CNIC == model.CNIC.Replace("-", ""));
                var user = GetUserByName(model.CNIC.Replace("-", ""));
                var savedMerit = db.Merits.FirstOrDefault(x => x.Id == model.Id);


                if (profile == null)
                {
                    db.HrProfiles.Add(new HrProfile() { EmployeeName = model.Name, FatherName = model.FatherName, EMaiL = model.Email, CNIC = model.CNIC?.Replace("-", ""), MobileNo = model.MobileNumber?.Replace("-", ""), CorrespondenceAddress = model.Address, PermanentAddress = model.Address, Designation_Id = desg?.Id, WDesignation_Id = desg?.Id, Cadre_Id = desg.Cadre_Id });
                }
                savedMerit.Name = model.Name;
                savedMerit.FatherName = model.FatherName;
                savedMerit.CNIC = model.CNIC?.Replace("-", "");
                savedMerit.MobileNumber = model.MobileNumber.Replace("-", "");
                savedMerit.Email = model.Email;
                savedMerit.PMDCNumber = model.PMDCNumber;
                savedMerit.MeritNumber = model.MeritNumber;
                savedMerit.ApplicationNumber = model.ApplicationNumber;
                savedMerit.Designation_Id = model.MeritActiveDesignation.Desg_Id;
                savedMerit.MeritsActiveDesignationId = model.MeritActiveDesignation?.Id;
                savedMerit.Address = model.Address;
                savedMerit.PPSCDate = model.PPSCDate;
                savedMerit.PPSCNumber = model.PPSCNumber;
                savedMerit.LetterDate = model.PPSCDate;
                savedMerit.LetterNumber = model.PPSCNumber;
                savedMerit.IsDisabled = model.IsDisabled;
                savedMerit.HF_Id = model.HF_Id;
                savedMerit.WorkingStatus = model.WorkingStatus;
                if (user == null)
                {
                    savedMerit.Status = "New";
                }
                db.SaveChanges();
                return Ok(user?.hashynoty);
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
                return BadRequest();
            }
        }


        [HttpGet]
        [Route("UpdatePreferencesOrder/{meritId=meritId}/{hfmisCodeFrom=hfmisCodeFrom}/{hfmisCodeTo=hfmisCodeTo}")]
        public IHttpActionResult UpdatePreferencesOrder(int meritId, string hfmisCodeFrom, string hfmisCodeTo)
        {
            try
            {
                var prefrencesFrom = db.MeritPreferences.FirstOrDefault(x => x.Merit_Id == meritId && x.HfmisCode == hfmisCodeFrom);
                var prefrencesTo = db.MeritPreferences.FirstOrDefault(x => x.Merit_Id == meritId && x.HfmisCode == hfmisCodeTo);
                var prefrencesOrder = prefrencesFrom.PrefrencesOrder;
                prefrencesFrom.PrefrencesOrder = prefrencesTo.PrefrencesOrder;
                prefrencesTo.PrefrencesOrder = prefrencesOrder;
                db.SaveChanges();
                return Ok("");
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("UpdatePgPreferencesOrder/{meritId=meritId}/{districtIdFrom=districtIdFrom}/{districtIdTo=districtIdTo}")]
        public IHttpActionResult UpdatePgPreferencesOrder(int meritId, int districtIdFrom, int districtIdTo)
        {
            try
            {
                var prefrencesFrom = db.MeritPGDistricts.FirstOrDefault(x => x.Merit_Id == meritId && x.Id == districtIdFrom);
                var prefrencesTo = db.MeritPGDistricts.FirstOrDefault(x => x.Merit_Id == meritId && x.Id == districtIdTo);
                var prefrencesOrder = prefrencesFrom.PrefrencesOrder;
                prefrencesFrom.PrefrencesOrder = prefrencesTo.PrefrencesOrder;
                prefrencesTo.PrefrencesOrder = prefrencesOrder;
                db.SaveChanges();
                return Ok("");
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }


        [HttpGet]
        [Route("GetMeritOrder/{activeDesig}/{from}/{to}")]
        public IHttpActionResult GetMeritOrder(int activeDesig, int from, int to)
        {
            try
            {
                var meritOrders = db.uspMeritOrders(activeDesig).Where(x => x.MeritNumber >= from && x.MeritNumber < to).ToList();
                return Ok(meritOrders);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }


        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [Route("GetHealthFacilityWMO/{code}/{excludeDistrictCode}")]
        [HttpGet]
        public async Task<List<HFList>> GetHealthFacilityWMO(string code = "", string excludeDistrictCode = "")
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(excludeDistrictCode))
                    return await db.HFLists.Where(x => x.HFMISCode.StartsWith(code) && types.Contains(x.HFTypeCode)
                     && !x.DistrictCode.Equals(excludeDistrictCode) && x.IsActive == true).OrderBy(x => x.HFTypeCode).ToListAsync();

                return await db.HFLists.Where(x => x.HFMISCode.StartsWith(code) && types.Contains(x.HFTypeCode) && x.IsActive == true).OrderBy(x => x.HFTypeCode).ToListAsync();
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
            catch (Exception ex)
            {
                return null;
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

        public List<string> GetDAMerits()
        {
            List<string> cnics = new List<string>();
            cnics.Add("3540165879497");
            cnics.Add("3540504095709");
            cnics.Add("3230146540967");
            cnics.Add("3520232410048");
            cnics.Add("3520290929341");
            cnics.Add("3540155922077");
            cnics.Add("3520246865369");
            cnics.Add("3550102697863");
            cnics.Add("3460134232209");
            cnics.Add("3410122851083");
            cnics.Add("3460384706257");
            cnics.Add("3460321918527");
            cnics.Add("3460360694823");
            cnics.Add("3460404330395");
            cnics.Add("3520209383882");
            cnics.Add("3520215793985");
            cnics.Add("5440085759670");
            cnics.Add("3520193711663");
            cnics.Add("3410280770259");
            cnics.Add("3520205746249");
            cnics.Add("3240265877819");
            cnics.Add("3310407521225");
            cnics.Add("3130386511445");
            cnics.Add("3210237987592");
            cnics.Add("3310034808452");
            cnics.Add("3460120247813");
            cnics.Add("3520290006033");
            cnics.Add("3130224602037");
            cnics.Add("3520063883346");
            cnics.Add("3620243600053");
            cnics.Add("3130421042179");
            cnics.Add("3730317880454");
            cnics.Add("3210271000700");
            cnics.Add("3530220249329");
            cnics.Add("3530233131081");
            cnics.Add("4130423136725");
            cnics.Add("3530217423115");
            cnics.Add("3610122344515");
            cnics.Add("1730172117242");
            cnics.Add("3410104096683");
            cnics.Add("3310034808452");
            cnics.Add("3310213561732");
            cnics.Add("3310213561732");
            cnics.Add("3410123736789");
            cnics.Add("3310085966721");
            cnics.Add("3410305682979");
            cnics.Add("3320391405457");
            cnics.Add("3310012451853");
            cnics.Add("3310395904877");
            cnics.Add("3330225928423");
            cnics.Add("3330306635587");
            cnics.Add("3330276583135");
            cnics.Add("3330280730553");
            cnics.Add("3110163790643");
            cnics.Add("3110481454369");
            cnics.Add("3130394110353");
            cnics.Add("3230481524357");
            cnics.Add("3650204701235");
            cnics.Add("3110214157742");
            cnics.Add("3310099415156");
            cnics.Add("3620161434153");
            cnics.Add("3110354199823");
            cnics.Add("3120225876909");
            cnics.Add("3740505265189");
            cnics.Add("3610305316377");
            cnics.Add("3710591039356");
            cnics.Add("4220187601823");
            cnics.Add("4530237905545");
            cnics.Add("3810151471700");
            cnics.Add("3820112130321");
            cnics.Add("3520274605857");
            cnics.Add("3630106159999");
            cnics.Add("3520119468381");
            cnics.Add("3520294167443");
            cnics.Add("6110139428419");
            cnics.Add("3640271382541");
            return cnics;
        }
        public List<string> GetDAMerits2()
        {
            List<string> cnics = new List<string>();
            cnics.Add("3630283237572");
            cnics.Add("3310149449216");
            cnics.Add("3320186665078");
            cnics.Add("3630270512792");
            cnics.Add("3210306735594");
            cnics.Add("3540488825074");
            cnics.Add("3230393762410");
            cnics.Add("3510333314144");
            cnics.Add("3210251386926");
            cnics.Add("3630236653992");
            cnics.Add("3210331878028");
            cnics.Add("3210278837406");
            cnics.Add("3520206124640");
            cnics.Add("3630268240536");
            cnics.Add("4130324796278");
            cnics.Add("3630434745215");
            cnics.Add("3230189767321");
            cnics.Add("3210396783735");
            cnics.Add("3460369238807");
            cnics.Add("3530220445313");
            cnics.Add("3630296473575");
            cnics.Add("3630336156397");
            cnics.Add("3240231814201");
            cnics.Add("3230481823111");
            cnics.Add("3210261123455");
            cnics.Add("4230148736161");
            cnics.Add("3610315778159");
            cnics.Add("3810230527203");
            cnics.Add("4220146490471");
            cnics.Add("3830242877249");
            cnics.Add("3830197145503");
            cnics.Add("3660298779291");
            cnics.Add("3230413520791");
            cnics.Add("3550201493001");
            cnics.Add("3230293907317");
            cnics.Add("3660232654307");
            cnics.Add("3230133499407");
            cnics.Add("3610473508473");
            cnics.Add("3650101441723");
            cnics.Add("3310031395371");
            cnics.Add("3230473335977");
            cnics.Add("3230473441951");
            cnics.Add("3230442821927");
            cnics.Add("3410123736789");
            cnics.Add("3410132807023");
            return cnics;
        }

    }
    public class WMOReportMobileModel
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public List<WMOReportStats> Stats { get; set; }
        public string FooterName { get; set; }
        public int FooterValue { get; set; }
        public string RefreshButtonText { get; set; }
    }
    public class WMOReportStats
    {
        public WMOReportStats(string name, int value)
        {
            Name = name;
            Value = value;
        }
        public string Name { get; set; }
        public int Value { get; set; }
    }

    public class MeritStats
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }

    public class MeritTemp : Merit
    {
        public int SrNo { get; set; }
        public string Domicile { get; set; }
        public int? Cadre_Id { get; set; }
    }

    public class MeritsFilter : Paginator
    {
        public int Merit_Id { get; set; }
        public bool SpecialQuota { get; set; }
        public int DesignationId { get; set; }
        public int ActiveDesignationId { get; set; }
        public string Query { get; set; }
    }
}
