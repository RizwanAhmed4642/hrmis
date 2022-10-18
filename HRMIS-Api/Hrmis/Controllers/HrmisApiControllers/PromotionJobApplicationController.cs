using Hrmis.Controllers.CommonData;
using Hrmis.Models.Common;
using Hrmis.Models.DbModel;
using Newtonsoft.Json;
using Phfmc.Models.Common;
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

namespace Hrmis.Controllers.HrmisApiControllers
{
    [RoutePrefix("api/promotionapplication")]
    public class PromotionJobApplicationController : ApiController
    {
        private HR_System db = new HR_System();
        public PromotionJobApplicationController()
        {
            db.Configuration.ProxyCreationEnabled = false;
        }




        [HttpPost]
        [Route("Create")]
        public async Task<IHttpActionResult> Create()
        {
            try
            {

                if (!Request.Content.IsMimeMultipartContent())
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                var provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);


                PromotionJobApplication promotionJobApplication = null;
                Boolean promotionJobApplicationSaved = false;

                var result = AddPromotionJobApplication(promotionJobApplication, provider);
                if (result.Type == ResultType.Success.ToString())
                {
                    promotionJobApplicationSaved = true;
                    promotionJobApplication = result.Data;

                    Common.SendSMSTelenor(
                        new SMS()
                        {
                            MobileNumber = promotionJobApplication.MobileNumber,
                            Message = $"Dear {promotionJobApplication.Name}, Your Promotion Application was successfuly Submited!"
                        });
                }
                else
                {
                    return Ok(result);
                }

                if (promotionJobApplicationSaved)
                {
                    try
                    {
                        foreach (var file in provider.Contents)
                        {
                            string ContentName = file.Headers.ContentDisposition.Name;
                            if (ContentName != "\"model\"")
                            {
                                var filename = file.Headers.ContentDisposition.FileName.Trim('\"');
                                var buffer = await file.ReadAsByteArrayAsync();
                                var size = ((buffer.Length) / (1024)) / (1024);
                                var ext = Path.GetExtension(filename.Replace("\"", string.Empty));
                                List<string> validExtensions = new List<string>() { ".png", ".PNG", ".jpg", ".JPG", ".JPEG", ".jpeg", ".pdf", ".PDF" };

                                if (!validExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase) || size > 5)
                                {
                                    throw new Exception("Unable to Upload. File Size must be less than 5 MB and File Format must be " + string.Join(",", validExtensions));
                                }


                                string dir = null;
                                if (ContentName == "\"Cow\"") dir = HttpContext.Current.Server.MapPath("/wwwroot/Uploads/PromotionJobApplication/CertificateOfWorking/");
                                if (ContentName == "\"Noe\"") dir = HttpContext.Current.Server.MapPath("/wwwroot/Uploads/PromotionJobApplication/NoEnquiryCertificate/");
                                if (ContentName == "\"Pst\"") dir = HttpContext.Current.Server.MapPath("/wwwroot/Uploads/PromotionJobApplication/MatricFScMbbsCertificate/");
                                if (ContentName == "\"Postgraduate\"") dir = HttpContext.Current.Server.MapPath("/wwwroot/Uploads/PromotionJobApplication/PostgraduateCertificate/");
                                if (ContentName == "\"Pmdc\"") dir = HttpContext.Current.Server.MapPath("/wwwroot/Uploads/PromotionJobApplication/PmdcCertificate/");
                                if (ContentName == "\"Noea\"") dir = HttpContext.Current.Server.MapPath("/wwwroot/Uploads/PromotionJobApplication/NoEnquiryAttestedCertificate/");
                                if (ContentName == "\"SignedCopy\"") dir = HttpContext.Current.Server.MapPath("/wwwroot/Uploads/PromotionJobApplication/SignedCopy/");
                                if (ContentName == "\"SeniorityNo\"") dir = HttpContext.Current.Server.MapPath("/wwwroot/Uploads/PromotionJobApplication/SeniorityNo/");
                                if (ContentName == "\"ExperienceCertifcate\"") dir = HttpContext.Current.Server.MapPath("/wwwroot/Uploads/PromotionJobApplication/ExperienceCert/");

                                var guid = Common.RandomString(5);

                                var NewFileName = guid + "_" + promotionJobApplication.Id + "_" + filename;
                                var FullFilePath = dir + NewFileName;
                                File.WriteAllBytes(FullFilePath, buffer);

                                // Assigning Filenames to profile
                                if (ContentName == "\"Cow\"") promotionJobApplication.CertificateOfWorkingFilepath = NewFileName;
                                if (ContentName == "\"Noe\"") promotionJobApplication.NoEnquiryCeritificateFilepath = NewFileName;
                                if (ContentName == "\"Pst\"") promotionJobApplication.MatricFScMbbsDegreeFilepath = NewFileName;
                                if (ContentName == "\"Postgraduate\"") promotionJobApplication.PostgraduateDegreeFilepath = NewFileName;
                                if (ContentName == "\"Pmdc\"") promotionJobApplication.PmdcFilepath = NewFileName;
                                if (ContentName == "\"Noea\"") promotionJobApplication.NoEnquiryCertifcateAttestedFilepath = NewFileName;
                                if (ContentName == "\"SignedCopy\"") promotionJobApplication.SignedCopyFielpath = NewFileName;
                                if (ContentName == "\"SeniorityNo\"") promotionJobApplication.SeniorityNoFilepath = NewFileName;
                                if (ContentName == "\"ExperienceCertifcate\"") promotionJobApplication.ExperienceCertFilePath = NewFileName;


                            }
                        }
                    }
                    catch (Exception ex)
                    {

                        return Ok(new Result<PromotionJobApplication>
                        {
                            Type = ResultType.Exception.ToString(),
                            exception = ex
                        });

                    }
                    // Updating profile with Photo Names
                    db.SaveChanges();
                }



                return Ok(new Result<PromotionJobApplication>
                {
                    Type = ResultType.Success.ToString(),
                    Data = promotionJobApplication
                });
            }
            catch (Exception ex)
            {
                return Ok(new Result<PromotionJobApplication>
                {
                    Type = ResultType.Exception.ToString(),
                    exception = ex
                });


            }







            //try
            //{

            //    promotionJobApplication.CreatedOn = DateTime.UtcNow.AddHours(5);

            //    db.PromotionJobApplications.Add(promotionJobApplication);
            //    db.SaveChanges();

            //    return Ok(new Result<Object>
            //    {
            //        Type = ResultType.Success.ToString()
            //    });
            //}
            //catch (Exception ex)
            //{
            //    return Ok(new Result<Object>
            //    {
            //        Type = ResultType.Exception.ToString(),
            //        exception = ex,
            //        Message = "Oops!! something went wrong please try again later. Thank you"
            //    });
            //}
        }

        private Result<PromotionJobApplication> AddPromotionJobApplication(PromotionJobApplication promotionJobApplication, MultipartMemoryStreamProvider provider)
        {

            foreach (var file in provider.Contents)
            {
                string ContentName = file.Headers.ContentDisposition.Name;
                if (ContentName == "\"model\"")
                {
                    promotionJobApplication = JsonConvert.DeserializeObject<PromotionJobApplication>(file.ReadAsStringAsync().Result);
                }
            }

            if (promotionJobApplication != null)
            {
                if (ModelState.IsValid)
                {
                    promotionJobApplication.CNIC = promotionJobApplication.CNIC != null ? promotionJobApplication.CNIC.Replace("-", "") : null;
                    promotionJobApplication.MobileNumber = promotionJobApplication.MobileNumber != null ? promotionJobApplication.MobileNumber.Replace("-", "") : null;

                    promotionJobApplication.CreatedOn = DateTime.UtcNow.AddHours(5);
                    promotionJobApplication.IsActive = true;

                    string DuplicateEntry = CheckDuplicateEntry(promotionJobApplication);
                    if (DuplicateEntry != "")
                        return new Result<PromotionJobApplication>()
                        {
                            Type = ResultType.Duplicate.ToString(),
                            Message = DuplicateEntry
                        };

                    db.PromotionJobApplications.Add(promotionJobApplication);
                    db.SaveChanges();
                    return new Result<PromotionJobApplication>
                    {
                        Type = ResultType.Success.ToString(),
                        Data = promotionJobApplication
                    };
                }
                else
                    return new Result<PromotionJobApplication>
                    {
                        Type = ResultType.Error.ToString(),
                        Message = "Given data is not valid"
                    };
            }

            return new Result<PromotionJobApplication>()
            {
                Type = ResultType.Error.ToString()
            };
        }

        private string CheckDuplicateEntry(PromotionJobApplication promotionJobApplication)
        {

            List<string> Duplicatekeys = new List<string>();

            if (db.PromotionJobApplications.Where(x => x.CNIC.Trim().ToLower() == promotionJobApplication.CNIC.Trim().ToLower() && x.IsActive == true).Count() != 0) Duplicatekeys.Add("CNIC");
            if (db.PromotionJobApplications.Where(x => x.MobileNumber.Trim().ToLower() == promotionJobApplication.MobileNumber.Trim().ToLower() && x.IsActive == true).Count() != 0) Duplicatekeys.Add("Mobile Number");
            if (db.PromotionJobApplications.Where(x => x.Email.Trim().ToLower() == promotionJobApplication.Email.Trim().ToLower() && x.IsActive == true).Count() != 0) Duplicatekeys.Add("Email Address");
            if (db.PromotionJobApplications.Where(x => x.Profile_Id == promotionJobApplication.Profile_Id && x.IsActive == true).Count() != 0 && promotionJobApplication.Profile_Id != null) Duplicatekeys.Add("Profile");

            if (Duplicatekeys.Count > 0)
            {
                string result = String.Join(", ", Duplicatekeys.ToArray());
                return "Duplicate entries found with " + result + " fields. Please try again with correct data. Thank you.";
            }
            else return "";

        }







        [HttpGet]
        [Route("BindingData")]
        public async Task<IHttpActionResult> BindingData()
        {
            try
            {

                int[] ConsultantDesignations = new int[]
                {
                    387,
                    383,
                    381,
                    362,
                    365,
                    374,
                    375,
                    369,
                    1594,
                    368,
                    384,
                    390,
                    382,
                    385,
                    388,
                    373,
                    1601
                };

                int[] SrDesignations = new int[]
                {
                    1999,
                    1128,
                    2002,
                    1101,
                    1102,
                    1112,
                    1106,
                    1135,
                    2007,
                    2008,
                    1982,
                    1140,
                    1104,
                    1110,
                    1121,
                    1138,
                    1111,
                    1141,
                    1112,
                    1139,
                    1100,
                    1136,
                    1132,
                    1108,
                    1106,
                    1116,
                    1117,
                    1137,
                    1118,
                    2244,
                    1127,
                    1991,
                    1115,
                    1114,
                    2246,
                    2249,
                    2250,
                    2517,
                    2251,
                    1129,
                    1131,
                    1133,
                    2016
                };

                var data = new
                {
                    Divisions = LocationController.ReturnDivisions("0"),
                    Districts = LocationController.ReturnDistricts("0"),
                    ConsultantDesignations =
                    db.HrDesignations.Where(x => ConsultantDesignations.Contains(x.Id)).OrderBy(x => x.Name).ToList(),
                    SrDesignations =
                    db.HrDesignations.Where(x => SrDesignations.Contains(x.Id)).OrderBy(x => x.Name).ToList(),
                    EligibleDesignations =
                    db.HrDesignations.Where(x => x.Name == "Medical Officer" || x.Name == "Women Medical Officer").OrderBy(x => x.Name).ToList()
                };

                return Ok(new Result<Object>
                {
                    Type = ResultType.Success.ToString(),
                    Data = data

                });
            }
            catch (Exception ex)
            {
                return Ok(new Result<Object>
                {
                    Type = ResultType.Exception.ToString(),
                    exception = ex,
                    Message = "Oops!! something went wrong please try again later. Thank you"
                });
            }
        }

        [HttpGet]
        [Route("CheckProfile/{CNIC}")]
        public async Task<IHttpActionResult> CheckProfile(string CNIC)
        {
            try
            {

                var profile = db.HrProfiles
                    .FirstOrDefault(x => x.CNIC == CNIC && x.Status_Id != 16 && x.HealthFacility_Id != null);
                var promoApp = db.PromoJobApplicationVMs
                    .Where(x => x.CNIC == CNIC && x.IsActive == true).FirstOrDefault();
                var promoAppSS = new List<PromotionJobApplicationServiceStatement>();
                if (promoApp != null)
                { 
                    promoAppSS = db.PromotionJobApplicationServiceStatements
                       .Where(x => x.PromotionJobApplication_Id == promoApp.Id).ToList();
                }

                var data = new
                {
                    Profile = profile,
                    PromotionApp = promoApp,
                    PromotionAppServiceStatements = promoAppSS
                };

                return Ok(new Result<object>
                {
                    Type = ResultType.Success.ToString(),
                    Data = JsonConvert.SerializeObject(data,
     Formatting.Indented,
     new JsonSerializerSettings
     {
         ReferenceLoopHandling = ReferenceLoopHandling.Ignore
     }
     )

                });
            }
            catch (Exception ex)
            {
                return Ok(new Result<object>
                {
                    Type = ResultType.Exception.ToString(),
                    exception = ex,
                    Message = "Oops!! something went wrong please try again later. Thank you"
                });
            }
        }

        [HttpGet]
        [Route("GetPromoApp/{Id}")]
        public async Task<IHttpActionResult> GetPromoApp(int Id)
        {
            try
            {
                var promoApp = db.PromoJobApplicationVMs
                        .Where(x => x.Id == Id).FirstOrDefault();
                if (promoApp != null)
                    return Ok(new Result<object>()
                    {
                        Type = ResultType.Success.ToString(),
                        Data = new
                        {
                            PromoApp = promoApp,
                            ServiceStatements = db.PromotionJobApplicationServiceStatements.Where(x => x.PromotionJobApplication_Id == promoApp.Id).ToList()
                        }
                    });
                else
                    return Ok(new Result<PromoJobApplicationVM>()
                    {
                        Type = ResultType.Error.ToString(),
                        Message = "No Promotion Application found."
                    });
            }
            catch (Exception ex)
            {
                return Ok(new Result<PromoJobApplicationVM>()
                {
                    Type = ResultType.Exception.ToString(),
                    exception = ex,
                    Message = "Oops!! something went wrong. Please try again later. Thank you."
                });
            }
        }

        [HttpGet]
        [Route("GetPromoApps/{skip=skip}/{SearchPhrase=SearchPhrase}/{dateStart=dateStart}/{endDate=endDate}")]
        public async Task<IHttpActionResult> GetPromoApp(int skip, string SearchPhrase = "", DateTime? dateStart = null, DateTime? endDate = null)
        {


            try
            {
                SearchPhrase = HttpUtility.UrlDecode(SearchPhrase?.Trim().ToLower());
                setDateFilter(new DateTime(1970, 1, 1), ref dateStart, ref endDate);
                var query = db.PromoJobApplicationVMs
                    .Where(x => x.IsActive == true && x.CreatedOn >= dateStart && x.CreatedOn < endDate)
                    .OrderByDescending(x => x.CreatedOn).AsQueryable();

                if (!string.IsNullOrEmpty(SearchPhrase))
                {
                    query = query.Where(x => x.AppliedForDesignationName.Trim().ToLower().Contains(SearchPhrase) ||
                    x.CNIC.Trim().ToLower().Contains(SearchPhrase) ||
                    x.CurrentDesignationName.Trim().ToLower().Contains(SearchPhrase) ||
                    x.Name.Trim().ToLower().Contains(SearchPhrase));
                }
                int RecordCount = query.Count();
                if (RecordCount < skip) skip = 0;

                return Ok(new Result<List<PromoJobApplicationVM>>()
                {
                    Type = ResultType.Success.ToString(),
                    Data = query.Skip(skip).Take(100).ToList(),
                    TotalRecords = RecordCount
                });

            }
            catch (Exception ex)
            {
                return Ok(new Result<List<PromoJobApplicationVM>>()
                {
                    Type = ResultType.Exception.ToString(),
                    exception = ex,
                    Message = "Oops!! something went wrong. Please try again later. Thank you."
                });
            }
        }

        private void setDateFilter(DateTime initialDate, ref DateTime? from, ref DateTime? to)
        {

            if (from == null && to == null)
            {
                from = initialDate;
                to = DateTime.Now.AddDays(1);
            }
            else if (from != null && to == null)
            {
                to = DateTime.Now.AddDays(1);
            }
            else if (from == null && to != null)
            {
                from = initialDate;
                to = to.Value.AddDays(1);
            }
            else if (from != null && to != null)
            {
                to = to.Value.AddDays(1);
            }

            if (from > to)
            {
                DateTime? temp = from;
                from = to;
                to = temp;
            }
        }

    }
}
