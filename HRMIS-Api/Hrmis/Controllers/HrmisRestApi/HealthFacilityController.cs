using Hrmis.Models.DbModel;
using Hrmis.Models.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
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
    [RoutePrefix("api/HealthFacility")]
    public class HealthFacilityController : ApiController
    {
        private readonly HealthFacilityService _healthFacilityService;

        public HealthFacilityController()
        {
            _healthFacilityService = new HealthFacilityService();
        }
        [HttpPost]
        [Route("SaveHF")]
        public IHttpActionResult SaveHF([FromBody] HFList healthFacility)
        {
            try
            {
                var hf = _healthFacilityService.addHF(healthFacility, User.Identity.GetUserName(), User.Identity.GetUserId());
                if (hf == null) return BadRequest("Invalid");
                return Ok(hf);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("SaveHFMode")]
        public IHttpActionResult SaveHFMode([FromBody] HFMode hFMode)
        {
            try
            {
                var hf = _healthFacilityService.addHFMode(hFMode, User.Identity.GetUserName(), User.Identity.GetUserId());
                if (hf == null) return BadRequest("Invalid");
                return Ok(hf);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("SaveHFUC")]
        public IHttpActionResult SaveHFUC([FromBody] HFUCM hfUc)
        {
            try
            {
                var hf = _healthFacilityService.addHFUCM(hfUc, User.Identity.GetUserName(), User.Identity.GetUserId());
                if (hf == null) return BadRequest("Invalid");
                return Ok(hf);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetHF")]
        public IHttpActionResult GetHF(string hfmisCode)
        {
            try
            {
                return Ok(_healthFacilityService.GetHF(hfmisCode));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetHFDashboardInfo/{hfmisCode}")]
        public IHttpActionResult GetHFDashboardInfo(string hfmisCode = "")
        {
            try
            {
                return Ok(_healthFacilityService.GetHFDashboardInfo(hfmisCode));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
       
        [HttpGet]
        [Route("GetHFHOD/{hfmisCode}")]
        public IHttpActionResult GetHFHOD(string hfmisCode = "")
        {
            try
            {
                return Ok(_healthFacilityService.GetHFHOD(hfmisCode));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetHFVacancy/{hfmisCode}")]
        public IHttpActionResult GetHFVacancy(string hfmisCode = "")
        {
            try
            {
                return Ok(_healthFacilityService.GetHFVacancy(hfmisCode));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetHFMode/{hfmisCode}")]
        public IHttpActionResult GetHFMode(string hfmisCode = "")
        {
            try
            {
                return Ok(_healthFacilityService.GetHFMode(hfmisCode));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetHFUCInfo/{hfmisCode}")]
        public IHttpActionResult GetHFUCInfo(string hfmisCode = "")
        {
            try
            {
                return Ok(_healthFacilityService.GetHFUCInfo(hfmisCode));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetPPSCDesignations/{hfmisCode}")]
        public IHttpActionResult GetPPSC_Designations(string hfmisCode = "")
        {
            try
            {
                return Ok(_healthFacilityService.HealthFacility_PPSC_Designations(hfmisCode));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetPPSCCandidates/{hfmisCode}/{designationId}")]
        public IHttpActionResult GetPPSC_Designations(string hfmisCode = "", int designationId = 0)
        {
            try
            {
                return Ok(_healthFacilityService.HealthFacility_PPSC_Candidates(hfmisCode, designationId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetHFProfileStatuses/{hfmisCode}")]
        public IHttpActionResult GetHFProfileStatuses(string hfmisCode = "")
        {
            try
            {
                return Ok(_healthFacilityService.HealthFacility_Profiles_Status(hfmisCode));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetHFProfileDesignations/{hfmisCode}")]
        public IHttpActionResult GetHFProfileDesignations(string hfmisCode = "")
        {
            try
            {
                return Ok(_healthFacilityService.HealthFacility_Profiles_Designations(hfmisCode));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetHFProfileEmpModes/{hfmisCode}")]
        public IHttpActionResult GetHFProfileEmpModes(string hfmisCode = "")
        {
            try
            {
                return Ok(_healthFacilityService.HealthFacility_Profiles_EmpModes(hfmisCode));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetHFProfiles/{hfmisCode}/{type}/{id}")]
        public IHttpActionResult GetHFProfiles(string hfmisCode, int type, int id)
        {
            try
            {
                return Ok(_healthFacilityService.HealthFacility_Profiles(hfmisCode, type, id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("GetProfilePJHistory")]
        public IHttpActionResult GetProfilePJHistory([FromBody] List<int> ids)
        {
            try
            {
                return Ok(_healthFacilityService.HealthFacility_ProfilePJHistory(ids));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetProfilesAgainstVacancy/{hf_Id}/{designation_Id}")]
        public IHttpActionResult GetProfilesAgainstVacancy(int hf_Id, int designation_Id)
        {
            try
            {
                return Ok(_healthFacilityService.HealthFacility_ProfilesAgainstVacancy(hf_Id, designation_Id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetHFWards/{hfmisCode}")]
        public IHttpActionResult GetHFWards(string hfmisCode)
        {
            try
            {
                return Ok(_healthFacilityService.GetHFWards(hfmisCode));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
       }
        //[HttpGet]
        //[Route("GetHFWardBeds/{hfmisCode}")]
        //public IHttpActionResult GetHFWardBeds(int HF_Id)
        //{
        //    try
        //    {
        //        return Ok(_healthFacilityService.GetHFWards(hfmisCode));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
        [HttpGet]
        [Route("GetHFWardsBeds/{hfmisCode}")]
        public IHttpActionResult GetHFWardsBeds(string hfmisCode)
        {
            try
            {
                return Ok(_healthFacilityService.GetHFWardsBeds(hfmisCode));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetHFServices/{hfmisCode}")]
        public IHttpActionResult GetHFServices(string hfmisCode)
        {
            try
            {
                return Ok(_healthFacilityService.GetHFServices(hfmisCode));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("GetHFList")]
        public IHttpActionResult GetHFList([FromBody] HealthFacilityFilter filter)
        {
            try
            {
                string role = User.IsInRole("Secondary") ? "Secondary" : User.IsInRole("Primary") ? "Primary" : User.IsInRole("Health Facility") ? "Health Facility" : User.IsInRole("PHFMC Admin")  || User.IsInRole("PHFMC") ? "PHFMC Admin" : User.IsInRole("PACP") ? "PACP" : User.IsInRole("South Punjab") ? "South Punjab" : "";
                return Ok(_healthFacilityService.GetHFList(filter, User.Identity.GetUserName(), User.Identity.GetUserId(), role));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException.InnerException.InnerException?.Message ?? ex.InnerException.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet]
        [Route("GetHFApplications/{hfmisCode}")]
        public IHttpActionResult GetHFApplications(string hfmisCode)
        {
            try
            {
                if (hfmisCode == "0")
                {
                    return Ok(_healthFacilityService.ApplicationHealthFacility());
                }
                else
                {
                    return Ok(_healthFacilityService.GetHFApplications(hfmisCode));

                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetHFDesignationApps/{hfId}")]
        public IHttpActionResult GetHFApplications(int hfId)
        {
            try
            {
                var hf = _healthFacilityService.GetHF(hfId);
                var designations = _healthFacilityService.GetHFApps(hfId);
                return Ok(new { hf , designations });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetHFApps/{hfId}/{desigId}")]
        public IHttpActionResult GetHFApps(int hfId, int desigId)
        {
            try
            {
                return Ok(_healthFacilityService.HealthFacility_Apps(hfId, desigId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetHFApplicationsFinal/{hfId}/{desigId}")]
        public IHttpActionResult GetHFApplicationsFinal(int hfId, int desigId)
        {
            try
            {
                var apps = _healthFacilityService.HealthFacility_Apps(hfId, desigId);
                var scoreApp = _healthFacilityService.CalculateFinalScore(apps, desigId);
                var marking = _healthFacilityService.GetMarking(desigId);
                var hf = _healthFacilityService.GetHF(hfId);
                var vacancy = _healthFacilityService.GetHFDesignationVacancy(hfId, desigId);
                return Ok(new { scoreApp, marking, hf, vacancy });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("UploadHFPhoto/{HFId}")]
        public async Task<IHttpActionResult> FileUpload(int HFId)
        {
            try
            {
                using (var db = new HR_System())
                {
                    if (!Request.Content.IsMimeMultipartContent())
                        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                    string RootPath = HttpContext.Current.Server.MapPath("~/") + @"wwwroot\Uploads\Files\HFPics\";
                    var dirPath = RootPath;

                    var provider = new MultipartMemoryStreamProvider();
                    await Request.Content.ReadAsMultipartAsync(provider);



                    CreateDirectoryIfNotExists(dirPath);
                    string filename = "";

                    foreach (var file in provider.Contents)
                    {
                        filename = Guid.NewGuid().ToString() + "_" + HFId + "_" +
                                   file.Headers.ContentDisposition.FileName.Trim('\"');
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

                        HFPhoto hfPhoto = new HFPhoto();
                        hfPhoto.HF_Id = HFId;
                        hfPhoto.ImagePath = filename;

                        Entity_Lifecycle eld = new Entity_Lifecycle();
                        eld.Created_Date = DateTime.UtcNow.AddHours(5);
                        eld.Created_By = User.Identity.GetUserName();
                        eld.Users_Id = User.Identity.GetUserId();
                        eld.IsActive = true;
                        eld.Entity_Id = 5;

                        db.Entity_Lifecycle.Add(eld);
                        await db.SaveChangesAsync();

                        hfPhoto.Entity_Lifecycle_Id = eld.Id;

                        db.HFPhotos.Add(hfPhoto);
                        await db.SaveChangesAsync();
                    }
                    return Ok(new { result = true, src = filename });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("RmvHealthFacility/{HF_Id}/{userhfmisCode}")]
        public IHttpActionResult RmvvHealthFacility(int HF_Id, string userhfmisCode)
        {
            try
            {
                return Ok(_healthFacilityService.RemoveHealthFacility(HF_Id, userhfmisCode, User.Identity.GetUserName(), User.Identity.GetUserId()));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("RmvHFWard/{id}")]
        public IHttpActionResult RmvHFWard(int id)
        {
            try
            {
                return Ok(_healthFacilityService.RmvHFWard(id, User.Identity.GetUserName(), User.Identity.GetUserId()));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("RmvHFWardBeds/{id}")]
        public IHttpActionResult RmvHFWardBeds(int id)
        {
            try
            {
                return Ok(_healthFacilityService.RmvHFWardBed(id, User.Identity.GetUserName(), User.Identity.GetUserId()));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("AddHFServices/{service_Id}/{HF_Id}/{hfmisCode}")]
        [HttpGet]
        public IHttpActionResult AddHFServices(int service_Id, int HF_Id, string hfmisCode)
        {
            try
            {
                var result = _healthFacilityService.AddHFService(service_Id, HF_Id, hfmisCode, User.Identity.GetUserName(), User.Identity.GetUserId());
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("AddHFWardBed")]
        [HttpPost]
        public IHttpActionResult AddHFWardBed([FromBody] HFWardBed hFWardBed)
        {
            try
            {
                var result = _healthFacilityService.AddHFWardBed(hFWardBed, User.Identity.GetUserId(), User.Identity.GetUserName());
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("RmvHFService/{hfId}/{serviceId}")]
        public IHttpActionResult RmvHFService(int hfId, int serviceId)
        {
            try
            {
                return Ok(_healthFacilityService.RmvHFService(hfId, serviceId, User.Identity.GetUserName(), User.Identity.GetUserId()));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("RmvHFVacancy/{id}")]
        public IHttpActionResult RmvHFVacancy(int id)
        {
            try
            {
                return Ok(_healthFacilityService.RmvHFVacancy(id, User.Identity.GetUserName(), User.Identity.GetUserId()));
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("GetHFPhoto/{hf_Id}")]
        public IHttpActionResult GetHFPhoto(int hf_Id, int designation_Id)
        {
            try
            {
                return Ok(_healthFacilityService.HFPhoto(hf_Id));
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
    }
}
