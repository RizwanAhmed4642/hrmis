using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Hrmis.Models.DbModel;

namespace Hrmis.Controllers.HrmisApiControllers
{
    [RoutePrefix("api/Seniority")]
    [AllowAnonymous]
    public class SeniorityApiController : ApiController
    {
        private HR_System db = new HR_System();

        [Route("AcknowledgePost")]
        [HttpPost]
        public IHttpActionResult AcknowledgePost([FromBody] ProfileDetailsSeniorityView pds)
        {
            if (pds == null)
            {
                return BadRequest(ModelState);
            }
            try
            {

                var findSiniority = db.SeniorityDetails.FirstOrDefault(x => x.Id == pds.SenorityId);

                if (findSiniority != null) findSiniority.Status = "Acknowledged";

                var findProfile = db.HrProfiles.FirstOrDefault(x => x.Id == pds.Id);
                if (findProfile != null) findProfile.SeniorityNo = pds.SeniorityNo;

                db.SaveChanges();

                return Ok("");

            }
            catch (DbEntityValidationException dbEx)
            {

                return Ok(GetDbExMessage(dbEx));

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("SeniorityGrivience")]
        [HttpPost]
        public IHttpActionResult SeniorityGrivience([FromBody] ProfileDetailsSeniorityView pds)
        {
            if (pds == null)
            {
                return BadRequest(ModelState);
            }
            try
            {

                var findSiniority = db.SeniorityDetails.FirstOrDefault(x => x.Id == pds.SenorityId);

                if (findSiniority != null)
                {
                    findSiniority.Grievance = pds.SenorityGrievance;
                    findSiniority.Status = "Grievance";
                    db.SaveChanges();

                }


                return Ok("");

            }
            catch (DbEntityValidationException dbEx)
            {

                return Ok(GetDbExMessage(dbEx));

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("Seniority")]
        [HttpPost]
        public IHttpActionResult Seniority([FromBody] ProfileDetailsSeniorityView pds)
        {
            if (pds == null)
            {
                return BadRequest(ModelState);
            }
            using (var trans = db.Database.BeginTransaction())
            {
                try
                {
                    var findProfile = db.HrProfiles.FirstOrDefault(x => x.Id == pds.Id);
                    if (findProfile != null) findProfile.SeniorityNo = pds.SenioritySeniorityNo.ToString();


                    var findSiniority = db.SeniorityDetails.FirstOrDefault(x => x.Id == pds.SenorityId);

                    if (findSiniority != null)
                    {
                        findSiniority.SeniorityNo = pds.SenioritySeniorityNo;
                        findSiniority.Status = "Saved";
                    }
                    else
                    {
                        var newObj = new SeniorityDetail
                        {
                            DateOfAppointment = findProfile.DateOfFirstAppointment,
                            DateOfBirth = findProfile.DateOfBirth,
                            Designation_Id = findProfile.Designation_Id,
                            FatherName = findProfile.FatherName,
                            Name = findProfile.EmployeeName,
                            Profile_Id = findProfile.Id,
                            SeniorityMaster_Id = null,
                            SeniorityNo = pds.SenioritySeniorityNo,
                            Status = "Saved",

                        };

                        db.SeniorityDetails.Add(newObj);
                    }

                    db.SaveChanges();

                    trans.Commit();
                    return Ok("");

                }
                catch (DbEntityValidationException dbEx)
                {
                    trans.Rollback();
                    return Ok(GetDbExMessage(dbEx));

                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    return BadRequest(ex.Message);
                }
            }
        }

        [Route("UploadFile/{id}")]
        [HttpPost]
        public async Task<IHttpActionResult> UploadFile(int id)
        {
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                var rootPath = HttpContext.Current.Server.MapPath("~/") + @"Uploads\Seniority\SeniorityFiles";
                var dirPath = rootPath;

                var provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);

                CreateDirectoryIfNotExists(dirPath);
                var filename = string.Empty;
                foreach (var file in provider.Contents)
                {
                    filename = file.Headers.ContentDisposition.FileName.Trim('\"');
                    var seniority = db.SeniorityDetails.FirstOrDefault(x => x.Id == id);
                    var buffer = await file.ReadAsByteArrayAsync();
                    var size = ((buffer.Length) / (1024)) / (1024);
                    var ext = Path.GetExtension(filename.Replace("\"", string.Empty));
                    filename = Guid.NewGuid() + ext;
                    List<string> validExtensions = new List<string>() { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".pdf", ".docx", ".doc" };
                    if (!validExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase) || size > 5)
                    {
                        throw new Exception("Unable to Upload. File Size must be less than 5 MB and File Format must be " + string.Join(",", validExtensions));
                    }
                    using (FileStream fsOut = File.OpenWrite(rootPath + @"\" + filename))
                    {
                        fsOut.Write(buffer, 0, buffer.Length);
                    }
                    if (seniority != null) seniority.SupportingDocs = filename;
                    db.SaveChanges();
                }



                return Ok(new { result = true, src = @"/Uploads/ProfilePhotos/" + filename });
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

        private string GetDbExMessage(DbEntityValidationException dbx) { return dbx.EntityValidationErrors.SelectMany(validationErrors => validationErrors.ValidationErrors).Aggregate("", (current, validationError) => current + $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}"); }
    }
}
